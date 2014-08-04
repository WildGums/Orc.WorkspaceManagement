// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceReaderService.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Services
{
    using Catel;
    using Catel.Logging;
    using Models;

    public abstract class WorkspaceReaderServiceBase : IWorkspaceReaderService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public IWorkspace Read(string directory)
        {
            Argument.IsNotNullOrWhitespace(() => directory);

            Log.Info("Loading data from '{0}'", directory);

            var workspace = ReadFromDirectory(directory);

            workspace.ClearIsDirty();

            Log.Info("Loaded data from '{0}'", directory);

            return workspace;
        }

        protected abstract IWorkspace ReadFromDirectory(string directory);
    }
}