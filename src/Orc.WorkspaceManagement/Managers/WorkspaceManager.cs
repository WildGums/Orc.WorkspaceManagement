// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManager.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;
    using Catel.IO;
    using Catel.Logging;

    public class WorkspaceManager : IWorkspaceManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IServiceLocator _serviceLocator;

        private readonly IWorkspaceInitializer _workspaceInitializer;

        private readonly List<IWorkspaceProvider> _workspaceProviders = new List<IWorkspaceProvider>();
        private readonly List<IWorkspace> _workspaces = new List<IWorkspace>();
        private object _scope;

        private IWorkspacesStorageService _workspacesStorageService;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceManager"/> class.
        /// </summary>
        /// <param name="workspaceInitializer">The workspace initializer.</param>
        /// <param name="workspacesStorageService">The for saving and loading workspaces</param>
        /// <param name="serviceLocator"></param>
        public WorkspaceManager(IWorkspaceInitializer workspaceInitializer, IWorkspacesStorageService workspacesStorageService,
            IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => workspaceInitializer);
            Argument.IsNotNull(() => serviceLocator);

            _workspaceInitializer = workspaceInitializer;
            _workspacesStorageService = workspacesStorageService;
            _serviceLocator = serviceLocator;

            UniqueIdentifier = UniqueIdentifierHelper.GetUniqueIdentifier<WorkspaceManager>();
            BaseDirectory = Path.Combine(Path.GetApplicationDataDirectory(), "workspaces");
            DefaultWorkspaceTitle = "Default";
        }
        #endregion

        #region Properties
        public int UniqueIdentifier { get; }

        /// <summary>
        /// Gets or sets the base directory to store the workspaces in.
        /// </summary>
        /// <value>The base directory.</value>
        public string BaseDirectory { get; set; }

        public IEnumerable<IWorkspaceProvider> Providers
        {
            get
            {
                lock (_workspaceProviders)
                {
                    return _workspaceProviders.ToArray();
                }
            }
        }

        public object Scope
        {
            get => _scope;
            set
            {
                _scope = value;
                _workspacesStorageService = _serviceLocator.ResolveType<IWorkspacesStorageService>(_scope);
            }
        }

        public IEnumerable<IWorkspace> Workspaces => _workspaces.ToArray();

        public IWorkspace Workspace { get; private set; }

        public string DefaultWorkspaceTitle { get; set; }

        public IWorkspace RefreshingWorkspace { get; private set; }
        public bool AutoRefreshEnabled { get; set; } = true;
        #endregion

        #region Events
        public event EventHandler<CancelEventArgs> Initializing;
        public event EventHandler<EventArgs> Initialized;

        public event AsyncEventHandler<CancelWorkspaceEventArgs> SavingAsync;
        public event EventHandler<WorkspaceEventArgs> Saved;

        public event EventHandler<EventArgs> WorkspacesChanged;

        public event EventHandler<WorkspaceEventArgs> WorkspaceAdded;
        public event EventHandler<WorkspaceEventArgs> WorkspaceRemoved;

        public event EventHandler<WorkspaceProviderEventArgs> WorkspaceProviderAdded;
        public event EventHandler<WorkspaceProviderEventArgs> WorkspaceProviderRemoved;

        public event EventHandler<WorkspaceEventArgs> WorkspaceInfoRequested;

        public event AsyncEventHandler<WorkspaceUpdatingEventArgs> WorkspaceUpdatingAsync;
        public event EventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdated;
        #endregion

        #region IWorkspaceManager Members
        public async Task SetWorkspaceAsync(IWorkspace value)
        {
            Argument.IsNotNull(() => value);

            if (!await TrySetWorkspaceAsync(value))
            {
                throw Log.ErrorAndCreateException(message => new WorkspaceException(value, message),
                    "Unable to set value to Workspace property.");
            }
        }

        public async Task<bool> TrySetWorkspaceAsync(IWorkspace value)
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

            Log.Debug($"[{Scope}] Changing workspace from '{oldWorkspace}' to '{newWorkspace}'");

            var workspaceUpdatingEventArgs = new WorkspaceUpdatingEventArgs(oldWorkspace, newWorkspace);
            await WorkspaceUpdatingAsync.SafeInvokeAsync(this, workspaceUpdatingEventArgs);
            if (workspaceUpdatingEventArgs.Cancel)
            {
                Log.Debug($"[{Scope}] Changing workspace was canceled");
                return false;
            }

            if (!(oldWorkspace is null))
            {
                await UpdateIsDirtyFlagAsync(oldWorkspace);
            }

            var oldIsDirty = oldWorkspace?.IsDirty;

            Workspace = newWorkspace;

            await ApplyWorkspaceUsingProvidersAsync(newWorkspace);

            WorkspaceUpdated?.Invoke(this, new WorkspaceUpdatedEventArgs(oldWorkspace, newWorkspace));

            if (AutoRefreshEnabled && !(oldWorkspace is null) && !oldWorkspace.Title.EqualsIgnoreCase(DefaultWorkspaceTitle))
            {
                Log.Debug($"[{Scope}] Reloading old workspace '{oldWorkspace}' from disk because it might have unsaved changes");

                await ReloadWorkspaceAsync(oldWorkspace);
                oldWorkspace.UpdateIsDirtyFlag(oldIsDirty.Value);
            }

            if (!(newWorkspace is null))
            {
                await UpdateIsDirtyFlagAsync(newWorkspace);
            }

            return true;
        }

        public async Task UpdateIsDirtyFlagAsync(IWorkspace workspace)
        {
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
                throw Log.ErrorAndCreateException(message => new WorkspaceManagementInitializationException(this, message),
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

            Log.Debug($"[{Scope}] Initializing workspaces from '{baseDirectory}'");

            var cancelEventArgs = new CancelEventArgs();
            Initializing?.Invoke(this, cancelEventArgs);
            if (cancelEventArgs.Cancel)
            {
                return false;
            }

            _workspaces.Clear();

            var workspaces = await _workspacesStorageService.LoadWorkspacesAsync(baseDirectory);

            foreach (var workspace in workspaces)
            {
                workspace.Scope = Scope;
                _workspaces.Add(workspace);
                workspace.UpdateIsDirtyFlag(false);
            }

            if (autoSelect && _workspaces.Any())
            {
                await TrySetWorkspaceAsync(_workspaces.FirstOrDefault());
            }
            else
            {
                await TrySetWorkspaceAsync(null);
            }

            Initialized?.Invoke(this, EventArgs.Empty);

            Log.Info($"[{Scope}] Initialized '{_workspaces.Count}' workspaces from '{baseDirectory}'");

            return true;
        }

        /// <summary>
        /// Adds the provider that will provide information to the workspace when the information is requested.
        /// </summary>
        /// <param name="workspaceProvider">The workspace provider.</param>
        public void AddProvider(IWorkspaceProvider workspaceProvider)
        {
            Argument.IsNotNull(() => workspaceProvider);

#if DEBUG
            Log.Debug($"[{Scope}] Adding provider {workspaceProvider.GetType()} to the WorkspaceManager (Scope = '{Scope ?? "null"}')");
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
            Argument.IsNotNull(() => workspaceProvider);

#if DEBUG
            Log.Debug($"[{Scope}] Removing provider {workspaceProvider.GetType()} from the WorkspaceManager (Tag == \"{Scope ?? "null"}\")");
#endif

            var removed = false;

            lock (_workspaceProviders)
            {
                removed = _workspaceProviders.Remove(workspaceProvider);
            }

            if (removed)
            {
                WorkspaceProviderRemoved?.Invoke(this, new WorkspaceProviderEventArgs(workspaceProvider));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds the specified workspace to the list of workspaces.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        public async Task AddAsync(IWorkspace workspace)
        {
            Argument.IsNotNull(() => workspace);

            if (!_workspaces.Contains(workspace))
            {
                Log.Debug($"[{Scope}] Adding workspace '{workspace}'");

                await _workspaceInitializer.InitializeAsync(workspace);

                _workspaces.Add(workspace);

                WorkspaceAdded?.Invoke(this, new WorkspaceEventArgs(workspace));
                WorkspacesChanged?.Invoke(this, EventArgs.Empty);
            }

            workspace.Scope = Scope;
        }

        /// <summary>
        /// Removes the specified workspace from the list of workspaces.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <returns><c>true</c> if the workspace is deleted; otherwise <c>false</c>.</returns>
        public async Task<bool> RemoveAsync(IWorkspace workspace)
        {
            Argument.IsNotNull(() => workspace);

            Log.Debug($"[{Scope}] Deleting workspace '{workspace}'");

            if (!_workspaces.Contains(workspace))
            {
                Log.Debug($"[{Scope}] Can't delete workspace '{workspace}', workspace is not contained by the manager");
                return false;
            }

            if (!workspace.CanDelete)
            {
                Log.Debug($"[{Scope}] Can't delete workspace '{workspace}', CanDelete = false");
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
            return ReloadWorkspaceAsync(Workspace);
        }

        /// <summary>
        /// Stores the workspace by requesting information.
        /// </summary>
        private async Task ReloadWorkspaceAsync(IWorkspace workspace)
        {
            Log.Debug($"[{Scope}] Reloading workspace '{workspace}'");

            if (workspace is null)
            {
                Log.Error($"[{Scope}] Workspace is empty, cannot reload workspace");
                return;
            }

            if (!workspace.CanEdit)
            {
                Log.Warning($"[{Scope}] Workspace is read-only, cannot reload workspace");
                return;
            }

            //TODO: implement reloding (resetting) default workspace as well
            var workspacePath = _workspacesStorageService.GetWorkspaceFileName(BaseDirectory, workspace);
            var workspaceFromDisk = await _workspacesStorageService.LoadWorkspaceAsync(workspacePath);
            if (workspaceFromDisk is null)
            {
                Log.Warning($"[{Scope}] Failed to reload workspace '{workspace}'");
                return;
            }

            workspace.SynchronizeWithWorkspace(workspaceFromDisk);

            Log.Info($"[{Scope}] Reloaded workspace '{workspace}'");
        }

        /// <summary>
        /// Stores the workspace by requesting information.
        /// </summary>
        public Task StoreWorkspaceAsync()
        {
            return StoreWorkspaceAsync(Workspace);
        }

        /// <summary>
        /// Stores the workspace by requesting information.
        /// </summary>
        public async Task StoreWorkspaceAsync(IWorkspace workspace)
        {
            Log.Debug($"[{Scope}] Storing workspace '{workspace}'");

            if (workspace is null)
            {
                Log.Error($"[{Scope}] Workspace is empty, cannot store workspace");
                return;
            }

            if (!workspace.CanEdit)
            {
                Log.Warning($"[{Scope}] Workspace is read-only, cannot store workspace");
                return;
            }

            // Events first so providers can manipulate data afterwards
            var workspaceEventArgs = new WorkspaceEventArgs(workspace);
            WorkspaceInfoRequested?.Invoke(this, workspaceEventArgs);

            await GetInformationFromProvidersAsync(workspace);

            Log.Info($"[{Scope}] Stored workspace '{workspace}'");
            Log.Status("Stored workspace");
        }

        /// <summary>
        /// Saves workspace to disk.
        /// </summary>
        public async Task<bool> SaveAsync()
        {
            var baseDirectory = BaseDirectory;

            Log.Debug($"[{Scope}] Saving workspace to '{baseDirectory}'");

            var workspace = Workspace;

            var cancelEventArgs = new CancelWorkspaceEventArgs(workspace);
            await SavingAsync.SafeInvokeAsync(this, cancelEventArgs);
            if (cancelEventArgs.Cancel)
            {
                return false;
            }

            await _workspacesStorageService.SaveWorkspacesAsync(baseDirectory, _workspaces);

            workspace?.UpdateIsDirtyFlag(false);

            Saved?.Invoke(this, new WorkspaceEventArgs(workspace));

            Log.Info($"[{Scope}] Saved current workspace to '{baseDirectory}'");

            return true;
        }

        public List<IWorkspaceProvider> GetWorkspaceProviders()
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
                    Log.Warning(ex, $"[{Scope}] Failed to get information for workspace using provider '{provider.GetType().Name}'");
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
                    Log.Warning(ex, $"[{Scope}] Failed to apply workspace using provider '{provider.GetType().Name}'");
                }
            }
        }

        public async Task RefreshWorkspaceAsync(IWorkspace workspace)
        {
            var oldValue = RefreshingWorkspace;

            RefreshingWorkspace = workspace;

            try
            {
                // TODO: use ReloadWorkspaceAsync(workspace)
                await TrySetWorkspaceAsync(workspace);
            }
            finally
            {
                RefreshingWorkspace = oldValue;
            }
        }
        #endregion
    }
}
