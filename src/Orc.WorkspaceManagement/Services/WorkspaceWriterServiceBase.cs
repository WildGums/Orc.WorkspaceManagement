// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceWriterService.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Services
{
    using Catel;
    using Catel.Logging;
    using Models;

    public abstract class WorkspaceWriterServiceBase : IWorkspaceWriterService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public void Write(IWorkspace workspace, string directory)
        {
            Argument.IsNotNull(() => workspace);
            Argument.IsNotNullOrWhitespace(() => directory);

            Log.Info("Writing all data to '{0}'", directory);

            WriteToDirectory(workspace, directory);

            workspace.ClearIsDirty();

            Log.Info("Wrote all data to '{0}'", directory);
        }

        protected abstract IWorkspace WriteToDirectory(IWorkspace workspace, string directory);
    }
}