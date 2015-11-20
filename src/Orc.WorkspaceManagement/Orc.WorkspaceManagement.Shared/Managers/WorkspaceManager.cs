// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManager.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET40 || SL5
#define USE_TASKEX
#endif

namespace Orc.WorkspaceManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;
    using Catel.IO;
    using Catel.Logging;
    using Catel.Threading;

    public class WorkspaceManager : IWorkspaceManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IWorkspaceInitializer _workspaceInitializer;
        private readonly IWorkspaceProviderLocator _workspaceProviderLocator;
        private readonly List<IWorkspaceProvider> _workspaceProviders = new List<IWorkspaceProvider>();
        private readonly List<IWorkspace> _workspaces = new List<IWorkspace>();
        private IWorkspacesStorageService _workspacesStorageService;
        private readonly IWorkspaceManagerInitializer _workspaceManagerInitializer;
        private readonly IServiceLocator _serviceLocator;
        private readonly AsyncLock _lockObject = new AsyncLock();
        private bool _isInitialized;
        private IWorkspace _workspace;
        private object _tag;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceManager"/> class.
        /// </summary>
        /// <param name="workspaceInitializer">The workspace initializer.</param>
        /// <param name="workspaceProviderLocator"></param>
        /// <param name="workspacesStorageService">The for saving and loading workspaces</param>
        /// <param name="workspaceManagerInitializer">The workspace initializer</param>
        /// <param name="serviceLocator"></param>
        public WorkspaceManager(IWorkspaceInitializer workspaceInitializer, IWorkspaceProviderLocator workspaceProviderLocator, IWorkspacesStorageService workspacesStorageService,
            IWorkspaceManagerInitializer workspaceManagerInitializer, IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => workspaceInitializer);
            Argument.IsNotNull(() => workspaceProviderLocator);
            Argument.IsNotNull(() => workspaceManagerInitializer);
            Argument.IsNotNull(() => serviceLocator);

            _workspaceInitializer = workspaceInitializer;
            _workspaceProviderLocator = workspaceProviderLocator;
            _workspacesStorageService = workspacesStorageService;
            _workspaceManagerInitializer = workspaceManagerInitializer;
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

        public object Tag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                _workspacesStorageService = _serviceLocator.ResolveType<IWorkspacesStorageService>(_tag);
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
        public event EventHandler<EventArgs> Initializing;
        public event EventHandler<EventArgs> Initialized;

        public event EventHandler<EventArgs> Saving;
        public event EventHandler<EventArgs> Saved;

        public event EventHandler<EventArgs> WorkspacesChanged;
        public event EventHandler<WorkspaceEventArgs> WorkspaceAdded;
        public event EventHandler<WorkspaceEventArgs> WorkspaceRemoved;
        public event EventHandler<WorkspaceProviderEventArgs> WorkspaceProviderAdded;
        public event EventHandler<WorkspaceProviderEventArgs> WorkspaceProviderRemoved;

        public event EventHandler<WorkspaceEventArgs> WorkspaceInfoRequested;

        public event EventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdating;
        public event EventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdated;
#endregion

#region IWorkspaceManager Members
        public async Task SetWorkspaceAsync(IWorkspace value)
        {
            var oldWorkspace = Workspace;
            var newWorkspace = value;

            WorkspaceUpdating.SafeInvoke(this, new WorkspaceUpdatedEventArgs(oldWorkspace, newWorkspace));

            Workspace = value;

            await ApplyWorkspaceUsingProvidersAsync(Workspace);

            WorkspaceUpdated.SafeInvoke(this, new WorkspaceUpdatedEventArgs(oldWorkspace, newWorkspace));
        }

        /// <summary>
        /// Initializes the workspaces by reading them from the <see cref="BaseDirectory"/>.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task InitializeAsync()
        {
            await InitializeAsync(true);
        }

        public async Task InitializeAsync(bool autoSelect)
        {
#if DEBUG
            Log.Debug(string.Format("Trying to initialize WorkspaceManager (Tag == \"{0}\")", Tag ?? "null"));
#endif
            if (await IsInitializedAsync())
            {
#if DEBUG
                Log.Debug(string.Format("The WorkspaceManager (Tag == \"{0}\") was already initialized before.", Tag??"null"));
#endif
                return;
            }

            using (await _lockObject.LockAsync())
            {
                var baseDirectory = BaseDirectory;
#if DEBUG
                Log.Debug(string.Format("Initializing WorkspaceManager (Tag == \"{0}\") from '{1}'", Tag ?? "null", baseDirectory));
#endif
                Initializing.SafeInvoke(this);

                _workspaces.Clear();

                await _workspaceManagerInitializer.InitializeAsync(this);

                if (autoSelect && _workspaces.Any())
                {
                    await SetWorkspaceAsync(_workspaces.FirstOrDefault());
                }
                else
                {
                    await SetWorkspaceAsync(null);
                }

                _isInitialized = true;
                Initialized.SafeInvoke(this);

                Log.Info("Initialized '{0}' workspaces from '{1}'", _workspaces.Count, baseDirectory);
            }
        }

        public async Task<bool> IsInitializedAsync()
        {
            using (await _lockObject.LockAsync())
            {
                return _isInitialized;
            }
        }

        /// <summary>
        /// Adds the provider that will provide information to the workspace when the information is requested.
        /// </summary>
        /// <param name="workspaceProvider">The workspace provider.</param>
        public void AddProvider(IWorkspaceProvider workspaceProvider)
        {
            Argument.IsNotNull(() => workspaceProvider);
#if DEBUG
            Log.Debug(string.Format("Adding provider {0} to the WorkspaceManager (Tag == \"{1}\")", workspaceProvider.GetType(), Tag ?? "null"));
#endif

            _workspaceProviders.Add(workspaceProvider);
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
            Log.Debug(string.Format("Removing provider {0} from the WorkspaceManager (Tag == \"{1}\")", workspaceProvider.GetType(), Tag ?? "null"));
#endif

            if (_workspaceProviders.Remove(workspaceProvider))
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
#if DEBUG
                Log.Debug(string.Format("Adding workspace \"{0}\" to the WorkspaceManager (Tag == \"{1}\")", workspace.Title, Tag ?? "null"));
#endif
                await _workspaceInitializer.InitializeAsync(workspace);

                _workspaces.Add(workspace);

                WorkspaceAdded.SafeInvoke(this, new WorkspaceEventArgs(workspace));
                WorkspacesChanged.SafeInvoke(this);
            }

            workspace.Tag = Tag;
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

#if DEBUG
            Log.Debug(string.Format("Removing workspace \"{0}\" from the WorkspaceManager (Tag == \"{1}\")", workspace.Title, Tag ?? "null"));
#endif

            var removed = _workspaces.Remove(workspace);

            if (ObjectHelper.AreEqual(workspace, Workspace))
            {
                await SetWorkspaceAsync(_workspaces.FirstOrDefault());
            }

            if (removed)
            {
                WorkspaceRemoved.SafeInvoke(this, new WorkspaceEventArgs(workspace));
                WorkspacesChanged.SafeInvoke(this);
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
            WorkspaceInfoRequested.SafeInvoke(this, new WorkspaceEventArgs(workspace));

            await GetInformationFromProvidersAsync(workspace);

            Log.Info("Stored workspace");
        }

        /// <summary>
        /// Saves all the workspaces to disk.
        /// </summary>
        public void Save()
        {
            var baseDirectory = BaseDirectory;

            Log.Debug("Saving all workspaces to '{0}'", baseDirectory);

            Saving.SafeInvoke(this);

            _workspacesStorageService.SaveWorkspaces(baseDirectory, _workspaces);

            Saved.SafeInvoke(this);

            Log.Info("Saved all workspaces to '{0}'", baseDirectory);
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