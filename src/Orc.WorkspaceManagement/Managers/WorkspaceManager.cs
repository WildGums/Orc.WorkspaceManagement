// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManager.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;
    using Catel.Logging;
    using Path = Catel.IO.Path;

    public class WorkspaceManager : IWorkspaceManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private const string WorkspaceFileExtension = ".xml";

        private readonly IWorkspaceInitializer _workspaceInitializer;
        private readonly List<IWorkspace> _workspaces = new List<IWorkspace>();
        private readonly List<IWorkspaceProvider> _workspaceProviders = new List<IWorkspaceProvider>();

        private IWorkspace _workspace;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceManager"/> class.
        /// </summary>
        /// <param name="workspaceInitializer">The workspace initializer.</param>
        public WorkspaceManager(IWorkspaceInitializer workspaceInitializer)
        {
            Argument.IsNotNull(() => workspaceInitializer);

            _workspaceInitializer = workspaceInitializer;

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

        public IEnumerable<IWorkspace> Workspaces
        {
            get { return _workspaces.ToArray(); }
        }

        public IWorkspace Workspace
        {
            get { return _workspace; }
            set
            {
                Argument.IsNotNull("workspace", value);

                var oldWorkspace = _workspace;
                var newWorkspace = value;

                if (ObjectHelper.AreEqual(oldWorkspace, newWorkspace))
                {
                    return;
                }

                WorkspaceUpdating.SafeInvoke(this, new WorkspaceUpdatedEventArgs(oldWorkspace, newWorkspace));

                _workspace = value;

                ApplyWorkspaceUsingProviders(_workspace);

                WorkspaceUpdated.SafeInvoke(this, new WorkspaceUpdatedEventArgs(oldWorkspace, newWorkspace));
            }
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

        public event EventHandler<WorkspaceEventArgs> WorkspaceInfoRequested;

        public event EventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdating;
        public event EventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdated;
        #endregion

        #region IWorkspaceManager Members
        /// <summary>
        /// Initializes the workspaces by reading them from the <see cref="BaseDirectory"/>.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task Initialize()
        {
            var baseDirectory = BaseDirectory;

            Log.Debug("Initializing workspaces from '{0}'", baseDirectory);

            Initializing.SafeInvoke(this);

            if (Directory.Exists(baseDirectory))
            {
                foreach (var workspaceFile in Directory.GetFiles(baseDirectory, string.Format("*{0}", WorkspaceFileExtension)))
                {
                    try
                    {
                        Log.Debug("Loading workspace from '{0}'", workspaceFile);

                        using (var fileStream = new FileStream(workspaceFile, FileMode.Open))
                        {
                            var workspace = ModelBase.Load<Workspace>(fileStream, SerializationMode.Xml);
                            if (workspace == null || string.IsNullOrEmpty(workspace.Title))
                            {
                                Log.Warning("File '{0}' doesn't look like a workspace, ignoring file", workspaceFile);
                            }
                            else
                            {
                                _workspaces.Add(workspace);

                                Log.Debug("Loaded workspace");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to load workspace from '{0}'", workspaceFile);
                    }
                }
            }

            if (_workspaces.Any())
            {
                Workspace = _workspaces.FirstOrDefault();
            }

            Initialized.SafeInvoke(this);

            Log.Info("Initialized '{0}' workspaces from '{1}'", _workspaces.Count, baseDirectory);
        }

        /// <summary>
        /// Adds the provider that will provide information to the workspace when the information is requested.
        /// </summary>
        /// <param name="workspaceProvider">The workspace provider.</param>
        public void AddProvider(IWorkspaceProvider workspaceProvider)
        {
            Argument.IsNotNull(() => workspaceProvider);

            _workspaceProviders.Add(workspaceProvider);
        }

        /// <summary>
        /// Removes the provider that will provide information to the workspace when the information is requested.
        /// </summary>
        /// <param name="workspaceProvider">The workspace provider.</param>
        /// <returns><c>true</c> if the workspace provider is deleted; otherwise <c>false</c>.</returns>
        public bool RemoveProvider(IWorkspaceProvider workspaceProvider)
        {
            Argument.IsNotNull(() => workspaceProvider);

            return _workspaceProviders.Remove(workspaceProvider);
        }

        /// <summary>
        /// Adds the specified workspace to the list of workspaces.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        public void Add(IWorkspace workspace)
        {
            Argument.IsNotNull(() => workspace);

            if (_workspaces.Contains(workspace))
            {
                return;
            }

            _workspaceInitializer.Initialize(workspace);

            _workspaces.Add(workspace);

            WorkspaceAdded.SafeInvoke(this, new WorkspaceEventArgs(workspace));
            WorkspacesChanged.SafeInvoke(this);
        }

        /// <summary>
        /// Removes the specified workspace from the list of workspaces.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <returns><c>true</c> if the workspace is deleted; otherwise <c>false</c>.</returns>
        public bool Remove(IWorkspace workspace)
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
                Workspace = _workspaces.FirstOrDefault();
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
        public void StoreWorkspace()
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

            GetInformationFromProviders(workspace);

            Log.Info("Stored workspace");
        }

        /// <summary>
        /// Saves all the workspaces to disk.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task Save()
        {
            var baseDirectory = BaseDirectory;

            Log.Debug("Saving all workspaces to '{0}'", baseDirectory);

            Saving.SafeInvoke(this);

            if (!Directory.Exists(baseDirectory))
            {
                Log.Debug("Creating base directory '{0}'", baseDirectory);

                Directory.CreateDirectory(baseDirectory);
            }

            Log.Debug("Deleting previous workspace files");

            foreach (var workspaceFile in Directory.GetFiles(baseDirectory, string.Format("*{0}", WorkspaceFileExtension)))
            {
                try
                {
                    Log.Debug("Deleting file '{0}'", workspaceFile);

                    File.Delete(workspaceFile);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to delete file '{0}'", workspaceFile);
                }
            }

            foreach (var workspace in _workspaces)
            {
                var workspaceFile = Path.Combine(baseDirectory, string.Format("{0}{1}", workspace.Title.GetSlug(), WorkspaceFileExtension));

                Log.Debug("Saving workspace '{0}' to '{1}'", workspace, workspaceFile);

                ((Workspace)workspace).SaveAsXml(workspaceFile);
            }

            Saved.SafeInvoke(this);

            Log.Info("Saved all workspaces to '{0}'", baseDirectory);
        }

        private void GetInformationFromProviders(IWorkspace workspace)
        {
            foreach (var provider in _workspaceProviders)
            {
                try
                {
                    provider.ProvideInformation(workspace);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to get information for workspace using provider '{0}'", provider.GetType().Name);
                }
            }
        }

        private void ApplyWorkspaceUsingProviders(IWorkspace workspace)
        {
            foreach (var provider in _workspaceProviders)
            {
                try
                {
                    provider.ApplyWorkspace(workspace);
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