// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceWriterService.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using Catel;
    using Catel.Logging;

    public abstract class WorkspaceWriterBase<TWorkspace> : IWorkspaceWriter
        where TWorkspace : IWorkspace
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public void Write(IWorkspace workspace, string location)
        {
            Argument.IsNotNull(() => workspace);
            Argument.IsNotNullOrWhitespace(() => location);

            Log.Debug("Writing all data to '{0}'", location);

            WriteToLocation((TWorkspace)workspace, location);

            workspace.ClearIsDirty();

            Log.Info("Wrote all data to '{0}'", location);
        }

        protected abstract void WriteToLocation(TWorkspace workspace, string location);
    }
}