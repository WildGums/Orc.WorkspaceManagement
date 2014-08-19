// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceManager.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
    using System.Threading.Tasks;

    public interface IWorkspaceManager
    {
        IWorkspace Workspace { get; }
        string Location { get; }

        event EventHandler<WorkspaceEventArgs> WorkspaceLoading;
        event EventHandler<WorkspaceEventArgs> WorkspaceLoaded;

        event EventHandler<WorkspaceEventArgs> WorkspaceSaving;
        event EventHandler<WorkspaceEventArgs> WorkspaceSaved;

        event EventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdated;

        event EventHandler<WorkspaceEventArgs> WorkspaceClosing;
        event EventHandler<WorkspaceEventArgs> WorkspaceClosed;

        Task Initialize();
        Task Refresh();
        Task Load(string location);
        Task Save(string location = null);
        void Close();
    }
}