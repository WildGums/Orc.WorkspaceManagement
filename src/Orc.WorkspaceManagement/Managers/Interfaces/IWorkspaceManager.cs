// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceManager.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWorkspaceManager
    {
        string BaseDirectory { get; set; }
        IEnumerable<IWorkspace> Workspaces { get; }
        IWorkspace Workspace { get; set; }

        event EventHandler<EventArgs> Initializing;
        event EventHandler<EventArgs> Initialized;

        event EventHandler<EventArgs> Saving;
        event EventHandler<EventArgs> Saved;

        event EventHandler<EventArgs> WorkspacesChanged;
        event EventHandler<WorkspaceEventArgs> WorkspaceAdded;
        event EventHandler<WorkspaceEventArgs> WorkspaceRemoved;

        event EventHandler<WorkspaceEventArgs> WorkspaceInfoRequested;
        event EventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdated;

        Task Initialize();

        void Add(IWorkspace workspace);
        void Remove(IWorkspace workspace);

        Task Save();
    }
}