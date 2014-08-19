// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManager.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;

    public class WorkspaceManager : IWorkspaceManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IWorkspaceInitializer _workspaceInitializer;
        private readonly IWorkspaceReader _workspaceReader;
        private readonly IWorkspaceWriter _workspaceWriter;

        private IWorkspace _workspace;

        #region Constructors
        public WorkspaceManager(IWorkspaceInitializer workspaceInitializer, IWorkspaceReader workspaceReader, IWorkspaceWriter workspaceWriter)
        {
            Argument.IsNotNull(() => workspaceInitializer);
            Argument.IsNotNull(() => workspaceReader);
            Argument.IsNotNull(() => workspaceWriter);

            _workspaceInitializer = workspaceInitializer;
            _workspaceReader = workspaceReader;
            _workspaceWriter = workspaceWriter;

            var location = workspaceInitializer.GetInitialLocation();

            Location = location;
        }
        #endregion

        #region Properties
        public string Location { get; private set; }

        public IWorkspace Workspace
        {
            get { return _workspace; }
            private set
            {
                var oldWorkspace = _workspace;
                var newWorkspace = value;

                _workspace = value;

                WorkspaceUpdated.SafeInvoke(this, new WorkspaceUpdatedEventArgs(oldWorkspace, newWorkspace));
            }
        }
        #endregion

        #region Events
        public event EventHandler<WorkspaceEventArgs> WorkspaceLoading;
        public event EventHandler<WorkspaceEventArgs> WorkspaceLoaded;

        public event EventHandler<WorkspaceEventArgs> WorkspaceSaving;
        public event EventHandler<WorkspaceEventArgs> WorkspaceSaved;

        public event EventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdated;

        public event EventHandler<WorkspaceEventArgs> WorkspaceClosing;
        public event EventHandler<WorkspaceEventArgs> WorkspaceClosed;
        #endregion

        #region IWorkspaceManager Members
        public async Task Initialize()
        {
            var location = Location;
            if (!string.IsNullOrEmpty(location))
            {
                Log.Debug("Initial location is '{0}', loading initial workspace", location);

                // TODO: Determine if this should be moved to a separate method
                await Load(location);
            }
        }

        public async Task Refresh()
        {
            if (Workspace == null)
            {
                return;
            }

            var location = Location;

            Log.Debug("Refreshing workspace from '{0}'", location);

            await Load(location);

            Log.Info("Refreshed workspace from '{0}'", location);
        }

        public async Task Load(string location)
        {
            Argument.IsNotNullOrWhitespace("location", location);

            Log.Debug("Loading workspace from '{0}'", location);

            WorkspaceLoading.SafeInvoke(this, new WorkspaceEventArgs(location));

            var workspace = await _workspaceReader.Read(location);

            Location = location;
            Workspace = workspace;

            WorkspaceLoaded.SafeInvoke(this, new WorkspaceEventArgs(workspace));

            Log.Info("Loaded workspace from '{0}'", location);
        }

        public async Task Save(string location = null)
        {
            var workspace = Workspace;
            if (workspace == null)
            {
                Log.Error("Cannot save empty workspace");
                throw new InvalidWorkspaceException(workspace);
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                location = Location;
            }

            Log.Debug("Saving workspace '{0}' to '{1}'", workspace, location);

            var eventArgs = new WorkspaceEventArgs(workspace);
            WorkspaceSaving.SafeInvoke(this, eventArgs);

            await _workspaceWriter.Write(workspace, location);
            Location = location;

            WorkspaceSaved.SafeInvoke(this, eventArgs);

            Log.Info("Saved workspace '{0}' to '{1}'", workspace, location);
        }

        public void Close()
        {
            var workspace = Workspace;
            if (workspace == null)
            {
                return;
            }

            Log.Debug("Closing workspace '{0}'", workspace);

            var eventArgs = new WorkspaceEventArgs(workspace);
            WorkspaceClosing.SafeInvoke(this, eventArgs);

            Workspace = null;
            Location = null;

            WorkspaceClosed.SafeInvoke(this, eventArgs);

            Log.Info("Closed workspace '{0}'", workspace);
        }
        #endregion
    }
}