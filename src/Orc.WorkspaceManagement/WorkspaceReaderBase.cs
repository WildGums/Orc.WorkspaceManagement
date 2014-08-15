// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceReaderService.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using Catel;
    using Catel.Logging;

    public abstract class WorkspaceReaderBase : IWorkspaceReader
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public IWorkspace Read(string location)
        {
            Argument.IsNotNullOrWhitespace(() => location);

            Log.Info("Reading data from '{0}'", location);

            var workspace = ReadFromLocation(location);

            workspace.ClearIsDirty();

            Log.Info("Reading data from '{0}'", location);

            return workspace;
        }

        protected abstract IWorkspace ReadFromLocation(string location);
    }
}