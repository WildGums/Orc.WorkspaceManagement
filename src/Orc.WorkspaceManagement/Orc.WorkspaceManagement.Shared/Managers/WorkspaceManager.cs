// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManager.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET40 || SL5
#define USE_TASKEX
#endif

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
        private readonly List<IWorkspaceProvider> _workspaceProviders = new List<IWorkspaceProvider>();
        private readonly List<IWorkspace> _workspaces = new List<IWorkspace>();
        private IWorkspacesStorageService _workspacesStorageService;
        private readonly IServiceLocator _serviceLocator;
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

            BaseDirectory = Path.Combine(Path.GetApplicationDataDirectory(), "workspaces");
        }
        #endregion

        #region Properties
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
        [ObsoleteEx(ReplacementTypeOrMember = "InitializingAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        public event EventHandler<EventArgs> Initializing;
        public event AsyncEventHandler<CancelEventArgs> InitializingAsync;
        [ObsoleteEx(ReplacementTypeOrMember = "InitializedAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        public event EventHandler<EventArgs> Initialized;
        public event AsyncEventHandler<EventArgs> InitializedAsync;

        [ObsoleteEx(ReplacementTypeOrMember = "SavingAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        public event EventHandler<EventArgs> Saving;
        public event AsyncEventHandler<CancelEventArgs> SavingAsync;
        [ObsoleteEx(ReplacementTypeOrMember = "SavedAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        public event EventHandler<EventArgs> Saved;
        public event AsyncEventHandler<EventArgs> SavedAsync;

        [ObsoleteEx(ReplacementTypeOrMember = "WorkspacesChangedAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        public event EventHandler<EventArgs> WorkspacesChanged;
        public event AsyncEventHandler<EventArgs> WorkspacesChangedAsync;
        [ObsoleteEx(ReplacementTypeOrMember = "WorkspaceAddedAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        public event EventHandler<WorkspaceEventArgs> WorkspaceAdded;
        public event AsyncEventHandler<WorkspaceEventArgs> WorkspaceAddedAsync;
        [ObsoleteEx(ReplacementTypeOrMember = "WorkspaceRemovedAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        public event EventHandler<WorkspaceEventArgs> WorkspaceRemoved;
        public event AsyncEventHandler<WorkspaceEventArgs> WorkspaceRemovedAsync;

        [ObsoleteEx(ReplacementTypeOrMember = "WorkspaceProviderAddedAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        public event EventHandler<WorkspaceProviderEventArgs> WorkspaceProviderAdded;
        public event AsyncEventHandler<WorkspaceProviderEventArgs> WorkspaceProviderAddedAsync;
        [ObsoleteEx(ReplacementTypeOrMember = "WorkspaceProviderRemovedAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        public event EventHandler<WorkspaceProviderEventArgs> WorkspaceProviderRemoved;
        public event AsyncEventHandler<WorkspaceProviderEventArgs> WorkspaceProviderRemovedAsync;

        [ObsoleteEx(ReplacementTypeOrMember = "WorkspaceInfoRequestedAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        public event EventHandler<WorkspaceEventArgs> WorkspaceInfoRequested;
        public event AsyncEventHandler<WorkspaceEventArgs> WorkspaceInfoRequestedAsync;

        [ObsoleteEx(ReplacementTypeOrMember = "WorkspaceUpdatingAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        public event EventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdating;
        public event AsyncEventHandler<WorkspaceUpdatingEventArgs> WorkspaceUpdatingAsync;
        [ObsoleteEx(ReplacementTypeOrMember = "WorkspaceUpdatedAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        public event EventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdated;
        public event AsyncEventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdatedAsync;
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

            var workspaceUpdatingEventArgs = new WorkspaceUpdatingEventArgs(oldWorkspace, newWorkspace);
            await WorkspaceUpdatingAsync.SafeInvokeAsync(this, workspaceUpdatingEventArgs);
            if (workspaceUpdatingEventArgs.Cancel)
            {
                return false;
            }

            WorkspaceUpdating.SafeInvoke(this, new WorkspaceUpdatedEventArgs(oldWorkspace, newWorkspace));

            Workspace = value;

            await ApplyWorkspaceUsingProvidersAsync(Workspace);

            WorkspaceUpdated.SafeInvoke(this, new WorkspaceUpdatedEventArgs(oldWorkspace, newWorkspace));
            await WorkspaceUpdatedAsync.SafeInvokeAsync(this, new WorkspaceUpdatedEventArgs(oldWorkspace, newWorkspace));

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

        public Task<bool> TryInitializeAsync()
        {
            return TryInitializeAsync(true);
        }

        public async Task InitializeAsync(bool autoSelect)
        {
            if (!await TryInitializeAsync(autoSelect))
            {
                throw Log.ErrorAndCreateException<WorkspaceManagementInitializationException>(
                    new WorkspaceManagementInitializationException(this), "Unable to initialize WorkspaceManager");
            }
        }

        public async Task<bool> TryInitializeAsync(bool autoSelect)
        {
            var baseDirectory = BaseDirectory;

            Log.Debug("Initializing workspaces from '{0}'", baseDirectory);

            var cancelEventArgs = new CancelEventArgs();
            await InitializingAsync.SafeInvokeAsync(this, cancelEventArgs);
            if (cancelEventArgs.Cancel)
            {
                return false;
            }

            Initializing.SafeInvoke(this);

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

            await InitializedAsync.SafeInvokeAsync(this);
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
            Log.Debug(string.Format("Adding provider {0} to the WorkspaceManager (Tag == \"{1}\")", workspaceProvider.GetType(), Scope ?? "null"));
#endif

            _workspaceProviders.Add(workspaceProvider);
            WorkspaceProviderAdded.SafeInvoke(this, new WorkspaceProviderEventArgs(workspaceProvider));
        }

        public Task AddProviderAsync(IWorkspaceProvider workspaceProvider)
        {
            AddProvider(workspaceProvider);

            return WorkspaceProviderAddedAsync.SafeInvokeAsync(this, new WorkspaceProviderEventArgs(workspaceProvider));
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
            Log.Debug(string.Format("Removing provider {0} from the WorkspaceManager (Tag == \"{1}\")", workspaceProvider.GetType(), Scope ?? "null"));
#endif

            if (_workspaceProviders.Remove(workspaceProvider))
            {
                WorkspaceProviderRemoved.SafeInvoke(this, new WorkspaceProviderEventArgs(workspaceProvider));
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveProviderAsync(IWorkspaceProvider workspaceProvider)
        {
            if (RemoveProvider(workspaceProvider))
            {
                await WorkspaceProviderRemovedAsync.SafeInvokeAsync(this, new WorkspaceProviderEventArgs(workspaceProvider));
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
                await WorkspaceAddedAsync.SafeInvokeAsync(this, new WorkspaceEventArgs(workspace));
                WorkspacesChanged.SafeInvoke(this);
                await WorkspacesChangedAsync.SafeInvokeAsync(this);
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
                await WorkspaceRemovedAsync.SafeInvokeAsync(this, new WorkspaceEventArgs(workspace));
                WorkspacesChanged.SafeInvoke(this);
                await WorkspacesChangedAsync.SafeInvokeAsync(this);
            }

            return removed;
        }

        /// <summary>
        /// Stores the workspace by requesting information.
        /// </summary>
        public async Task StoreWorkspaceAsync()
        {
            Log.Debug("Storing workspace");

            var workspace = Workspace;
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
            await WorkspaceInfoRequestedAsync.SafeInvokeAsync(this, workspaceEventArgs);

            await GetInformationFromProvidersAsync(workspace);

            Log.Info("Stored workspace");
        }

        /// <summary>
        /// Saves all the workspaces to disk.
        /// </summary>
        [ObsoleteEx(ReplacementTypeOrMember = "SaveAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        public void Save()
        {
            var baseDirectory = BaseDirectory;

            Log.Debug("Saving all workspaces to '{0}'", baseDirectory);

            Saving.SafeInvoke(this);

            _workspacesStorageService.SaveWorkspaces(baseDirectory, _workspaces);

            Saved.SafeInvoke(this);

            Log.Info("Saved all workspaces to '{0}'", baseDirectory);
        }

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

            Saving.SafeInvoke(this);

            _workspacesStorageService.SaveWorkspaces(baseDirectory, _workspaces);

            await SavedAsync.SafeInvokeAsync(this);
            Saved.SafeInvoke(this);

            Log.Info("Saved all workspaces to '{0}'", baseDirectory);

            return true;
        }

        private async Task GetInformationFromProvidersAsync(IWorkspace workspace)
        {
            foreach (var provider in _workspaceProviders)
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
            foreach (var provider in _workspaceProviders)
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