// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManager.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
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

        private readonly IWorkspaceInitializer _workspaceInitializer;
        private readonly IServiceLocator _serviceLocator;

        private readonly List<IWorkspaceProvider> _workspaceProviders = new List<IWorkspaceProvider>();
        private readonly List<IWorkspace> _workspaces = new List<IWorkspace>();

        private IWorkspacesStorageService _workspacesStorageService;
        private IWorkspace _workspace;
        private object _scope;

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
        }
        #endregion

        #region Properties
        public int UniqueIdentifier { get; private set; }

        /// <summary>
        /// Gets or sets the base directory to store the workspaces in.
        /// </summary>
        /// <value>The base directory.</value>
        public string BaseDirectory { get; set; }

        public IEnumerable<IWorkspaceProvider> Providers
        {
            get { return _workspaceProviders.ToArray(); }
        }

        public object Scope
        {
            get { return _scope; }
            set
            {
                _scope = value;
                _workspacesStorageService = _serviceLocator.ResolveType<IWorkspacesStorageService>(_scope);
            }
        }

        public IEnumerable<IWorkspace> Workspaces
        {
            get { return _workspaces.ToArray(); }
        }

        public IWorkspace Workspace
        {
            get { return _workspace; }
            private set { _workspace = value; }
        }
        #endregion

        #region Events
        public event EventHandler<CancelEventArgs> Initializing;
        public event EventHandler<EventArgs> Initialized;

        public event AsyncEventHandler<CancelEventArgs> SavingAsync;
        public event EventHandler<EventArgs> Saved;

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
                throw Log.ErrorAndCreateException<WorkspaceException>(new WorkspaceException(value),
                    "Unable to set value to Workspace property.");
            }
        }

        public async Task<bool> TrySetWorkspaceAsync(IWorkspace value)
        {
            var oldWorkspace = Workspace;
            var newWorkspace = value;

            Log.Debug($"Changing workspace from '{oldWorkspace}' to '{newWorkspace}'");

            var workspaceUpdatingEventArgs = new WorkspaceUpdatingEventArgs(oldWorkspace, newWorkspace);
            await WorkspaceUpdatingAsync.SafeInvokeAsync(this, workspaceUpdatingEventArgs);
            if (workspaceUpdatingEventArgs.Cancel)
            {
                Log.Debug("Changing workspace was canceled");
                return false;
            }
            
            Workspace = newWorkspace;

            await ApplyWorkspaceUsingProvidersAsync(newWorkspace);

            WorkspaceUpdated.SafeInvoke(this, new WorkspaceUpdatedEventArgs(oldWorkspace, newWorkspace));

            if (oldWorkspace != null)
            {
                Log.Debug($"Reloading old workspace '{oldWorkspace}' from disk because it might have unsaved changes");

                await ReloadWorkspaceAsync(oldWorkspace);
            }

            return true;
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
                throw Log.ErrorAndCreateException<WorkspaceManagementInitializationException>(
                    new WorkspaceManagementInitializationException(this), "Unable to initialize WorkspaceManager");
            }
        }

        public Task<bool> TryInitializeAsync()
        {
            return TryInitializeAsync(true);
        }

        public async Task<bool> TryInitializeAsync(bool autoSelect)
        {
            var baseDirectory = BaseDirectory;

            Log.Debug("Initializing workspaces from '{0}'", baseDirectory);

            var cancelEventArgs = new CancelEventArgs();
            Initializing.SafeInvoke(this, cancelEventArgs);
            if (cancelEventArgs.Cancel)
            {
                return false;
            }

            _workspaces.Clear();

            var workspaces = _workspacesStorageService.LoadWorkspaces(baseDirectory);
            foreach (var workspace in workspaces)
            {
                workspace.Scope = Scope;
                _workspaces.Add(workspace);
            }

            if (autoSelect && _workspaces.Any())
            {
                await TrySetWorkspaceAsync(_workspaces.FirstOrDefault());
            }
            else
            {
                await TrySetWorkspaceAsync(null);
            }

            Initialized.SafeInvoke(this);

            Log.Info("Initialized '{0}' workspaces from '{1}'", _workspaces.Count, baseDirectory);

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
            Log.Debug($"Adding provider {workspaceProvider.GetType()} to the WorkspaceManager (Scope = '{Scope ?? "null"}')");
#endif

            lock (_workspaceProviders)
            {
                _workspaceProviders.Add(workspaceProvider);
            }

            WorkspaceProviderAdded.SafeInvoke(this, new WorkspaceProviderEventArgs(workspaceProvider));
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
            Log.Debug($"Removing provider {workspaceProvider.GetType()} from the WorkspaceManager (Tag == \"{Scope ?? "null"}\")");
#endif

            var removed = false;

            lock (_workspaceProviders)
            {
                removed = _workspaceProviders.Remove(workspaceProvider);
            }

            if (removed)
            {
                WorkspaceProviderRemoved.SafeInvoke(this, new WorkspaceProviderEventArgs(workspaceProvider));
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
                await _workspaceInitializer.InitializeAsync(workspace);

                _workspaces.Add(workspace);

                WorkspaceAdded.SafeInvoke(this, new WorkspaceEventArgs(workspace));
                WorkspacesChanged.SafeInvoke(this);
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

            if (!_workspaces.Contains(workspace))
            {
                return false;
            }

            if (!workspace.CanDelete)
            {
                return false;
            }

            var removed = _workspaces.Remove(workspace);

            if (ObjectHelper.AreEqual(workspace, Workspace))
            {
                await TrySetWorkspaceAsync(_workspaces.FirstOrDefault());
            }

            if (removed)
            {
                WorkspaceRemoved.SafeInvoke(this, new WorkspaceEventArgs(workspace));
                WorkspacesChanged.SafeInvoke(this);
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
        public async Task ReloadWorkspaceAsync(IWorkspace workspace)
        {
            Log.Debug($"Reloading workspace '{workspace}'");

             if (workspace == null)
            {
                Log.Error("Workspace is empty, cannot reload workspace");
                return;
            }

            if (!workspace.CanEdit)
            {
                Log.Warning("Workspace is read-only, cannot reload workspace");
                return;
            }

            var workspacePath = _workspacesStorageService.GetWorkspaceFileName(BaseDirectory, workspace);
            var workspaceFromDisk = _workspacesStorageService.LoadWorkspace(workspacePath);
            if (workspaceFromDisk == null)
            {
                Log.Warning($"Failed to reload workspace '{workspace}'");
                return;
            }

            workspace.SynchronizeWithWorkspace(workspaceFromDisk);

            Log.Info($"Reloaded workspace '{workspace}'");
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
            Log.Debug($"Storing workspace '{workspace}'");

            if (workspace == null)
            {
                Log.Error("Workspace is empty, cannot store workspace");
                return;
            }

            if (!workspace.CanEdit)
            {
                Log.Warning("Workspace is read-only, cannot store workspace");
                return;
            }

            // Events first so providers can manipulate data afterwards
            var workspaceEventArgs = new WorkspaceEventArgs(workspace);
            WorkspaceInfoRequested.SafeInvoke(this, workspaceEventArgs);

            await GetInformationFromProvidersAsync(workspace);

            Log.Info($"Stored workspace '{workspace}'");
            Log.Status("Stored workspace");
        }

        /// <summary>
        /// Saves all the workspaces to disk.
        /// </summary>
        public async Task<bool> SaveAsync()
        {
            var baseDirectory = BaseDirectory;

            Log.Debug("Saving all workspaces to '{0}'", baseDirectory);

            var cancelEventArgs = new CancelEventArgs();
            await SavingAsync.SafeInvokeAsync(this, cancelEventArgs);
            if (cancelEventArgs.Cancel)
            {
                return false;
            }

            _workspacesStorageService.SaveWorkspaces(baseDirectory, _workspaces);

            Saved.SafeInvoke(this);

            Log.Info("Saved all workspaces to '{0}'", baseDirectory);

            return true;
        }

        private List<IWorkspaceProvider> GetWorkspaceProviders()
        {
            var providers = new List<IWorkspaceProvider>();

            lock (_workspaceProviders)
            {
                providers.AddRange(_workspaceProviders);
            }

            return providers;
        }

        private async Task GetInformationFromProvidersAsync(IWorkspace workspace)
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
                    Log.Warning(ex, "Failed to get information for workspace using provider '{0}'", provider.GetType().Name);
                }
            }
        }

        private async Task ApplyWorkspaceUsingProvidersAsync(IWorkspace workspace)
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
                    Log.Warning(ex, "Failed to apply workspace using provider '{0}'", provider.GetType().Name);
                }
            }
        }
        #endregion
    }
}