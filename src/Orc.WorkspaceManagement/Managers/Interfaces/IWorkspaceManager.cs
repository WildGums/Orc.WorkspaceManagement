// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceManager.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;

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

        //void Refresh();
        //void Save();
        void Refresh();
        void Load(string location);
        void Save(string location = null);
        void Close();
    }
}