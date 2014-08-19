// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceReaderService.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;

    public abstract class WorkspaceReaderBase : IWorkspaceReader
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public async Task<IWorkspace> Read(string location)
        {
            Argument.IsNotNullOrWhitespace(() => location);

            Log.Debug("Reading data from '{0}'", location);

            var workspace = await ReadFromLocation(location);

            workspace.ClearIsDirty();

            Log.Info("Read data from '{0}'", location);

            return workspace;
        }

        protected abstract Task<IWorkspace> ReadFromLocation(string location);
    }
}