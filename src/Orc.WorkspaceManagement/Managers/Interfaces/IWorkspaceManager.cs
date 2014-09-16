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
        IEnumerable<IWorkspaceProvider> Providers { get; }

        event EventHandler<EventArgs> Initializing;
        event EventHandler<EventArgs> Initialized;

        event EventHandler<EventArgs> Saving;
        event EventHandler<EventArgs> Saved;

        event EventHandler<EventArgs> WorkspacesChanged;
        event EventHandler<WorkspaceEventArgs> WorkspaceAdded;
        event EventHandler<WorkspaceEventArgs> WorkspaceRemoved;

        event EventHandler<WorkspaceEventArgs> WorkspaceInfoRequested;
        event EventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdated;

        /// <summary>
        /// Initializes the workspaces by reading them from the <see cref="WorkspaceManager.BaseDirectory"/>.
        /// </summary>
        /// <returns>Task.</returns>
        Task Initialize();

        /// <summary>
        /// Adds the provider that will provide information to the workspace when the information is requested.
        /// </summary>
        /// <param name="workspaceProvider">The workspace provider.</param>
        void AddProvider(IWorkspaceProvider workspaceProvider);

        /// <summary>
        /// Removes the provider that will provide information to the workspace when the information is requested.
        /// </summary>
        /// <param name="workspaceProvider">The workspace provider.</param>
        void RemoveProvider(IWorkspaceProvider workspaceProvider);

        /// <summary>
        /// Adds the specified workspace to the list of workspaces.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        void Add(IWorkspace workspace);

        /// <summary>
        /// Removes the specified workspace from the list of workspaces.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        void Remove(IWorkspace workspace);

        /// <summary>
        /// Stores the workspace by requesting information.
        /// </summary>
        void StoreWorkspace();

        /// <summary>
        /// Saves all the workspaces to disk.
        /// </summary>
        /// <returns>Task.</returns>
        Task Save();
    }
}