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
        IWorkspace Workspace { get; }
        IEnumerable<IWorkspaceProvider> Providers { get; }

        event EventHandler<EventArgs> Initializing;
        event EventHandler<EventArgs> Initialized;

        event EventHandler<EventArgs> Saving;
        event EventHandler<EventArgs> Saved;

        event EventHandler<EventArgs> WorkspacesChanged;
        event EventHandler<WorkspaceEventArgs> WorkspaceAdded;
        event EventHandler<WorkspaceEventArgs> WorkspaceRemoved;

        event EventHandler<WorkspaceEventArgs> WorkspaceInfoRequested;

        event EventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdating;
        event EventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdated;

        Task SetWorkspaceAsync(IWorkspace value);

        /// <summary>
        /// Initializes the workspaces by reading them from the <see cref="WorkspaceManager.BaseDirectory" />.
        /// </summary>
        /// <returns>Task.</returns>
        Task InitializeAsync();

        /// <summary>
        /// Initializes the specified automatic select.
        /// </summary>
        /// <param name="autoSelect">if set to <c>true</c> [automatic select].</param>
        Task InitializeAsync(bool autoSelect);

        /// <summary>
        /// Adds the provider that will provide information to the workspace when the information is requested.
        /// </summary>
        /// <param name="workspaceProvider">The workspace provider.</param>
        void AddProvider(IWorkspaceProvider workspaceProvider);

        /// <summary>
        /// Removes the provider that will provide information to the workspace when the information is requested.
        /// </summary>
        /// <param name="workspaceProvider">The workspace provider.</param>
        /// <returns><c>true</c> if the workspace provider is deleted; otherwise <c>false</c>.</returns>
        bool RemoveProvider(IWorkspaceProvider workspaceProvider);

        /// <summary>
        /// Adds the specified workspace to the list of workspaces.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        Task AddAsync(IWorkspace workspace);

        /// <summary>
        /// Removes the specified workspace from the list of workspaces.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <returns><c>true</c> if the workspace is deleted; otherwise <c>false</c>.</returns>
        Task<bool> RemoveAsync(IWorkspace workspace);

        /// <summary>
        /// Stores the workspace by requesting information.
        /// </summary>
        Task StoreWorkspaceAsync();

        /// <summary>
        /// Saves all the workspaces to disk.
        /// </summary>
        void Save();
    }
}