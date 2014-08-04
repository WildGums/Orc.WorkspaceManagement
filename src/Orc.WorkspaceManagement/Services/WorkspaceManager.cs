// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManager.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Services
{
    using System;
    using System.IO;
    using Catel;
    using Catel.Configuration;
    using Catel.Logging;
    using Models;

    public class WorkspaceManager : IWorkspaceManager
    {
        private readonly IWorkspaceReaderService _workspaceReaderService;
        private readonly IWorkspaceWriterService _workspaceWriterService;
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private IWorkspace _workspace;

        #region Constructors
        public WorkspaceManager(IConfigurationService configurationService, IWorkspaceReaderService workspaceReaderService,
            IWorkspaceWriterService workspaceWriterService, ICommandLineService commandLineService)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => workspaceReaderService);
            Argument.IsNotNull(() => workspaceWriterService);
            Argument.IsNotNull(() => commandLineService);

            _workspaceReaderService = workspaceReaderService;
            _workspaceWriterService = workspaceWriterService;

            var dataDirectory = configurationService.GetValue<string>("DataLocation");
            if (string.IsNullOrWhiteSpace(dataDirectory))
            {
                dataDirectory = Path.Combine(Catel.IO.Path.GetApplicationDataDirectory(), "data");

                if (!Directory.Exists(dataDirectory))
                {
                    Directory.CreateDirectory(dataDirectory);
                }

                Log.Info("DataLocation is empty in configuration, determining the data directory automatically to '{0}'", dataDirectory);
            }

            if (commandLineService.Arguments.Length > 0)
            {
                dataDirectory = commandLineService.Arguments[0];
            }

            var fullPath = Path.GetFullPath(dataDirectory);
            if (!Directory.Exists(fullPath))
            {
                Log.ErrorAndThrowException<InvalidOperationException>("Cannot use the data directory '{0}', it does not exist", fullPath);
            }

            CurrentDirectory = fullPath;
            CurrentWorkspace = Load();
        }
        #endregion

        #region Properties
        public string CurrentDirectory { get; private set; }

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
            return _workspaceReaderService.Read(CurrentDirectory);
        }

        public void Save()
        {
            _workspaceWriterService.Write(CurrentWorkspace, CurrentDirectory);
        }
        #endregion
    }
}