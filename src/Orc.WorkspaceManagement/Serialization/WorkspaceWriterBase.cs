// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceWriterService.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;

    public abstract class WorkspaceWriterBase<TWorkspace> : IWorkspaceWriter
        where TWorkspace : IWorkspace
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public async Task Write(IWorkspace workspace, string location)
        {
            Argument.IsNotNull(() => workspace);
            Argument.IsNotNullOrWhitespace(() => location);

            Log.Debug("Writing all data to '{0}'", location);

            await WriteToLocation((TWorkspace)workspace, location);

            workspace.Location = location;
            workspace.ClearIsDirty();

            Log.Info("Wrote all data to '{0}'", location);
        }

        protected abstract Task WriteToLocation(TWorkspace workspace, string location);
    }
}