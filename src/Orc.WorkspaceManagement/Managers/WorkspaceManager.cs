namespace Orc.WorkspaceManagement;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Catel;
using Catel.IO;
using Catel.Logging;
using Catel.Services;
using Microsoft.Extensions.Logging;

public class WorkspaceManager : IWorkspaceManager
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(WorkspaceManager));

    private readonly IAppDataService _appDataService;
    private readonly IWorkspaceInitializer _workspaceInitializer;

    private readonly List<IWorkspaceProvider> _workspaceProviders;
    private readonly List<IWorkspace> _workspaces = new();

    private readonly IWorkspacesStorageService _workspacesStorageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkspaceManager"/> class.
    /// </summary>
    /// <param name="workspaceInitializer">The workspace initializer.</param>
    /// <param name="workspacesStorageService">The for saving and loading workspaces</param>
    /// <param name="appDataService">The app data service.</param>
    /// <param name="workspaceProviders"></param>
    public WorkspaceManager(IWorkspaceInitializer workspaceInitializer, IWorkspacesStorageService workspacesStorageService,
        IAppDataService appDataService, IEnumerable<IWorkspaceProvider> workspaceProviders)
    {
        _workspaceInitializer = workspaceInitializer;
        _workspacesStorageService = workspacesStorageService;
        _appDataService = appDataService;

        _workspaceProviders = workspaceProviders.ToList();

        UniqueIdentifier = UniqueIdentifierHelper.GetUniqueIdentifier<WorkspaceManager>();
        BaseDirectory = System.IO.Path.Combine(_appDataService.GetApplicationDataDirectory(ApplicationDataTarget.UserRoaming), "workspaces");
        DefaultWorkspaceTitle = "Default";
    }

    public int UniqueIdentifier { get; }

    /// <summary>
    /// Gets or sets the base directory to store the workspaces in.
    /// </summary>
    /// <value>The base directory.</value>
    public string BaseDirectory { get; set; }

    public IReadOnlyList<IWorkspaceProvider> Providers
    {
        get
        {
            lock (_workspaceProviders)
            {
                return _workspaceProviders;
            }
        }
    }

    public IReadOnlyList<IWorkspace> Workspaces => _workspaces.ToArray();

    public IWorkspace? Workspace { get; private set; }

    public string DefaultWorkspaceTitle { get; set; }

    public IWorkspace? RefreshingWorkspace { get; private set; }
    public bool AutoRefreshEnabled { get; set; } = true;

    public event EventHandler<CancelEventArgs>? Initializing;
    public event EventHandler<EventArgs>? Initialized;

    public event EventHandler<EventArgs>? WorkspacesChanged;

    public event EventHandler<WorkspaceEventArgs>? WorkspaceAdded;
    public event EventHandler<WorkspaceEventArgs>? WorkspaceRemoved;

    public event EventHandler<WorkspaceProviderEventArgs>? WorkspaceProviderAdded;
    public event EventHandler<WorkspaceProviderEventArgs>? WorkspaceProviderRemoved;

    public event EventHandler<WorkspaceEventArgs>? WorkspaceInfoRequested;

    public event AsyncEventHandler<WorkspaceUpdatingEventArgs>? WorkspaceUpdatingAsync;
    public event EventHandler<WorkspaceUpdatedEventArgs>? WorkspaceUpdated;

    public event AsyncEventHandler<CancelWorkspaceEventArgs>? WorkspaceSavingAsync;
    public event EventHandler<WorkspaceEventArgs>? WorkspaceSaved;

    public async Task SetWorkspaceAsync(IWorkspace? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!await TrySetWorkspaceAsync(value))
        {
            throw Logger.LogErrorAndCreateException(message => new WorkspaceException(value, message),
                "Unable to set value to Workspace property.");
        }
    }

    public async Task<bool> TrySetWorkspaceAsync(IWorkspace? value)
    {
        var oldWorkspace = Workspace;
        var newWorkspace = value;

        if (oldWorkspace is null && newWorkspace is null)
        {
            return true;
        }

        if (Equals(oldWorkspace, newWorkspace) && !Equals(RefreshingWorkspace, newWorkspace))
        {
            return true;
        }

        Logger.LogDebug($"Changing workspace from '{oldWorkspace}' to '{newWorkspace}'");

        var workspaceUpdatingEventArgs = new WorkspaceUpdatingEventArgs(oldWorkspace, newWorkspace);
        await WorkspaceUpdatingAsync.SafeInvokeAsync(this, workspaceUpdatingEventArgs);
        if (workspaceUpdatingEventArgs.Cancel)
        {
            Logger.LogDebug($"Changing workspace was canceled");
            return false;
        }

        if (oldWorkspace is not null)
        {
            await UpdateIsDirtyFlagAsync(oldWorkspace);
        }

        var oldIsDirty = oldWorkspace?.IsDirty;

        Workspace = newWorkspace;

        if (newWorkspace is not null)
        {
            await ApplyWorkspaceUsingProvidersAsync(newWorkspace);
        }

        WorkspaceUpdated?.Invoke(this, new WorkspaceUpdatedEventArgs(oldWorkspace, newWorkspace));

        if (AutoRefreshEnabled && oldWorkspace is not null && !oldWorkspace.Title.EqualsIgnoreCase(DefaultWorkspaceTitle))
        {
            Logger.LogDebug($"Reloading old workspace '{oldWorkspace}' from disk because it might have unsaved changes");

            await ReloadWorkspaceAsync(oldWorkspace);

            if (oldIsDirty.HasValue)
            {
                oldWorkspace.UpdateIsDirtyFlag(oldIsDirty.Value);
            }
        }

        if (newWorkspace is not null)
        {
            await UpdateIsDirtyFlagAsync(newWorkspace);
        }

        return true;
    }

    public async Task UpdateIsDirtyFlagAsync(IWorkspace workspace)
    {
        if (workspace is null)
        {
            return;
        }

        var isDirty = await this.IsWorkspaceDirtyAsync(workspace);
        workspace.UpdateIsDirtyFlag(isDirty);
    }

    /// <summary>
    /// Initializes the workspaces by reading them from the <see cref="BaseDirectory"/>.
    /// </summary>
    /// <returns>Task.</returns>
    public Task InitializeAsync()
    {
        return InitializeAsync(true);
    }

    public async Task InitializeAsync(bool autoSelect)
    {
        if (!await TryInitializeAsync(autoSelect))
        {
            throw Logger.LogErrorAndCreateException(message => new WorkspaceManagementInitializationException(this, message),
                "Unable to initialize WorkspaceManager");
        }
    }

    public Task<bool> TryInitializeAsync()
    {
        return TryInitializeAsync(true);
    }

    public async Task<bool> TryInitializeAsync(bool autoSelect)
    {
        var baseDirectory = BaseDirectory;

        Logger.LogDebug($"Initializing workspaces from '{baseDirectory}'");

        var cancelEventArgs = new CancelEventArgs();
        Initializing?.Invoke(this, cancelEventArgs);
        if (cancelEventArgs.Cancel)
        {
            return false;
        }

        _workspaces.Clear();

        var workspaces = await _workspacesStorageService.LoadWorkspacesAsync(baseDirectory);
        if (workspaces is not null)
        {
            foreach (var workspace in workspaces)
            {
                if (string.IsNullOrWhiteSpace(workspace.Title) &&
                    string.IsNullOrWhiteSpace(workspace.DisplayName))
                {
                    continue;
                }

                _workspaces.Add(workspace);
                workspace.UpdateIsDirtyFlag(false);
            }
        }

        if (autoSelect && _workspaces.Any())
        {
            await TrySetWorkspaceAsync(_workspaces.First());
        }
        else
        {
            await TrySetWorkspaceAsync(null);
        }

        Initialized?.Invoke(this, EventArgs.Empty);

        Logger.LogInformation($"Initialized '{_workspaces.Count}' workspace(s) from '{baseDirectory}'");

        return true;
    }

    /// <summary>
    /// Adds the provider that will provide information to the workspace when the information is requested.
    /// </summary>
    /// <param name="workspaceProvider">The workspace provider.</param>
    public void AddProvider(IWorkspaceProvider workspaceProvider)
    {
        ArgumentNullException.ThrowIfNull(workspaceProvider);

#if DEBUG
        Logger.LogDebug($"Adding provider {workspaceProvider.GetType()} to the WorkspaceManager");
#endif

        lock (_workspaceProviders)
        {
            _workspaceProviders.Add(workspaceProvider);
        }

        WorkspaceProviderAdded?.Invoke(this, new WorkspaceProviderEventArgs(workspaceProvider));
    }

    /// <summary>
    /// Removes the provider that will provide information to the workspace when the information is requested.
    /// </summary>
    /// <param name="workspaceProvider">The workspace provider.</param>
    /// <returns><c>true</c> if the workspace provider is deleted; otherwise <c>false</c>.</returns>
    public bool RemoveProvider(IWorkspaceProvider workspaceProvider)
    {
        ArgumentNullException.ThrowIfNull(workspaceProvider);

#if DEBUG
        Logger.LogDebug($"Removing provider {workspaceProvider.GetType()} from the WorkspaceManager");
#endif

        bool removed;

        lock (_workspaceProviders)
        {
            removed = _workspaceProviders.Remove(workspaceProvider);
        }

        if (!removed)
        {
            return false;
        }

        WorkspaceProviderRemoved?.Invoke(this, new WorkspaceProviderEventArgs(workspaceProvider));
        return true;

    }

    /// <summary>
    /// Adds the specified workspace to the list of workspaces.
    /// </summary>
    /// <param name="workspace">The workspace.</param>
    public async Task AddAsync(IWorkspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);

        if (!_workspaces.Contains(workspace))
        {
            Logger.LogDebug($"Adding workspace '{workspace}'");

            await _workspaceInitializer.InitializeAsync(workspace);

            _workspaces.Add(workspace);

            WorkspaceAdded?.Invoke(this, new WorkspaceEventArgs(workspace));
            WorkspacesChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Removes the specified workspace from the list of workspaces.
    /// </summary>
    /// <param name="workspace">The workspace.</param>
    /// <returns><c>true</c> if the workspace is deleted; otherwise <c>false</c>.</returns>
    public async Task<bool> RemoveAsync(IWorkspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);

        Logger.LogDebug($"Deleting workspace '{workspace}'");

        if (!_workspaces.Contains(workspace))
        {
            Logger.LogDebug($"Can't delete workspace '{workspace}', workspace is not contained by the manager");
            return false;
        }

        if (!workspace.CanDelete)
        {
            Logger.LogDebug($"Can't delete workspace '{workspace}', CanDelete = false");
            return false;
        }

        var removed = _workspaces.Remove(workspace);

        if (ObjectHelper.AreEqual(workspace, Workspace))
        {
            await TrySetWorkspaceAsync(_workspaces.FirstOrDefault());
        }

        if (removed)
        {
            WorkspaceRemoved?.Invoke(this, new WorkspaceEventArgs(workspace));
            WorkspacesChanged?.Invoke(this, EventArgs.Empty);
        }

        return removed;
    }

    /// <summary>
    /// Reloads the workspace by reading the information from the original location.
    /// </summary>
    public Task ReloadWorkspaceAsync()
    {
        var workspace = Workspace;
        return workspace is not null ? ReloadWorkspaceAsync(workspace) : Task.CompletedTask;
    }

    /// <summary>
    /// Stores the workspace by requesting information.
    /// </summary>
    private async Task ReloadWorkspaceAsync(IWorkspace workspace)
    {
        Logger.LogDebug($"Reloading workspace '{workspace}'");

        if (workspace is null)
        {
            Logger.LogError($"Workspace is empty, cannot reload workspace");
            return;
        }

        //TODO: implement reloading (resetting) default workspace as well
        var workspacePath = _workspacesStorageService.GetWorkspaceFileName(BaseDirectory, workspace);
        var workspaceFromDisk = await _workspacesStorageService.LoadWorkspaceAsync(workspacePath);
        if (workspaceFromDisk is null)
        {
            Logger.LogWarning($"Failed to reload workspace '{workspace}'");
            return;
        }

        workspace.SynchronizeWithWorkspace(workspaceFromDisk);

        Logger.LogInformation($"Reloaded workspace '{workspace}'");
    }

    /// <summary>
    /// Stores the workspace by requesting information.
    /// </summary>
    public Task StoreWorkspaceAsync()
    {
        var workspace = Workspace;
        if (workspace is null)
        {
            return Task.CompletedTask;
        }

        return StoreWorkspaceAsync(workspace);
    }

    /// <summary>
    /// Stores the workspace by requesting information.
    /// </summary>
    public async Task StoreWorkspaceAsync(IWorkspace workspace)
    {
        Logger.LogDebug($"Storing workspace '{workspace}'");

        if (workspace is null)
        {
            Logger.LogError($"Workspace is empty, cannot store workspace");
            return;
        }

        if (!workspace.CanEdit)
        {
            Logger.LogWarning($"Workspace is read-only, cannot store workspace");
            return;
        }

        // Events first so providers can manipulate data afterwards
        var workspaceEventArgs = new WorkspaceEventArgs(workspace);
        WorkspaceInfoRequested?.Invoke(this, workspaceEventArgs);

        await GetInformationFromProvidersAsync(workspace);

        Logger.LogInformation($"Stored workspace '{workspace}'");
        //Logger.LogStatus("Stored workspace");
    }

    /// <summary>
    /// Saves workspace to disk.
    /// </summary>
    public async Task<bool> SaveAsync()
    {
        var workspace = Workspace;
        if (workspace is null)
        {
            return false;
        }

        var baseDirectory = BaseDirectory;

        Logger.LogDebug($"Saving workspace to '{baseDirectory}'");

        var cancelEventArgs = new CancelWorkspaceEventArgs(workspace);
        await WorkspaceSavingAsync.SafeInvokeAsync(this, cancelEventArgs);
        if (cancelEventArgs.Cancel)
        {
            return false;
        }

        await _workspacesStorageService.SaveWorkspacesAsync(baseDirectory, _workspaces);

        workspace.UpdateIsDirtyFlag(false);

        var workspaceEventArgs = new WorkspaceEventArgs(workspace);
        WorkspaceSaved?.Invoke(this, workspaceEventArgs);

        Logger.LogInformation($"Saved current workspace to '{baseDirectory}'");

        return true;
    }

    public IReadOnlyList<IWorkspaceProvider> GetWorkspaceProviders()
    {
        var providers = new List<IWorkspaceProvider>();

        lock (_workspaceProviders)
        {
            providers.AddRange(_workspaceProviders);
        }

        return providers;
    }

    public async Task GetInformationFromProvidersAsync(IWorkspace workspace)
    {
        var workspaceProviders = GetWorkspaceProviders();

        foreach (var provider in workspaceProviders)
        {
            try
            {
                await provider.ProvideInformationAsync(workspace);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, $"Failed to get information for workspace using provider '{provider.GetType().Name}'");
            }
        }
    }

    public async Task ApplyWorkspaceUsingProvidersAsync(IWorkspace workspace)
    {
        var workspaceProviders = GetWorkspaceProviders();

        foreach (var provider in workspaceProviders)
        {
            try
            {
                await provider.ApplyWorkspaceAsync(workspace);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, $"Failed to apply workspace using provider '{provider.GetType().Name}'");
            }
        }
    }

    public async Task RefreshWorkspaceAsync(IWorkspace workspace)
    {
        var oldValue = RefreshingWorkspace;

        RefreshingWorkspace = workspace;

        try
        {
            await ReloadWorkspaceAsync(workspace);
            await TrySetWorkspaceAsync(workspace);
        }
        finally
        {
            RefreshingWorkspace = oldValue;
        }
    }
}
