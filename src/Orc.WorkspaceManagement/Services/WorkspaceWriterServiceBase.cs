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

    public abstract class WorkspaceWriterServiceBase<TWorkspace> : IWorkspaceWriterService
        where TWorkspace : IWorkspace
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public void Write(IWorkspace workspace, string directory)
        {
            Argument.IsNotNull(() => workspace);
            Argument.IsNotNullOrWhitespace(() => directory);

            Log.Info("Writing all data to '{0}'", directory);

            WriteToDirectory((TWorkspace)workspace, directory);

            workspace.ClearIsDirty();

            Log.Info("Wrote all data to '{0}'", directory);
        }

        protected abstract void WriteToDirectory(TWorkspace workspace, string directory);
    }
}