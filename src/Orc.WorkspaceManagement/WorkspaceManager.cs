// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManager.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
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

            CurrentLocation = location;

            if (!string.IsNullOrEmpty(CurrentLocation))
            {
                Log.Debug("Initial location is '{0}', loading initial workspace");

                CurrentWorkspace = Load();
            }
        }
        #endregion

        #region Properties
        public string CurrentLocation { get; private set; }

        public IWorkspace CurrentWorkspace
        {
            get { return _workspace; }
            private set
            {
                _workspace = value;
                WorkspaceUpdated.SafeInvoke(this);
            }
        }
        #endregion

        #region Events
        public event EventHandler<EventArgs> WorkspaceUpdated;
        #endregion

        #region IWorkspaceManager Members
        public void Refresh()
        {
            Log.Info("Refreshing workspace");

            CurrentWorkspace = Load();
        }

        private IWorkspace Load()
        {
            return _workspaceReader.Read(CurrentLocation);
        }

        public void Save()
        {
            _workspaceWriter.Write(CurrentWorkspace, CurrentLocation);
        }
        #endregion
    }
}