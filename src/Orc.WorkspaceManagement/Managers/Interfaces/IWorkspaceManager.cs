// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceManager.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Catel;

    public interface IWorkspaceManager
    {
        string BaseDirectory { get; set; }
        IEnumerable<IWorkspace> Workspaces { get; }
        IWorkspace Workspace { get; }
        IEnumerable<IWorkspaceProvider> Providers { get; }
        string DefaultWorkspaceTitle { get; set; }
        object Scope { get; set; }

        event EventHandler<CancelEventArgs> Initializing;
        event EventHandler<EventArgs> Initialized;

        event AsyncEventHandler<CancelEventArgs> SavingAsync;
        event EventHandler<EventArgs> Saved;

        event EventHandler<EventArgs> WorkspacesChanged;

        event EventHandler<WorkspaceEventArgs> WorkspaceAdded;
        event EventHandler<WorkspaceEventArgs> WorkspaceRemoved;

        event EventHandler<WorkspaceProviderEventArgs> WorkspaceProviderAdded;
        event EventHandler<WorkspaceProviderEventArgs> WorkspaceProviderRemoved;

        event EventHandler<WorkspaceEventArgs> WorkspaceInfoRequested;

        event AsyncEventHandler<WorkspaceUpdatingEventArgs> WorkspaceUpdatingAsync;
        event EventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdated;

        Task SetWorkspaceAsync(IWorkspace value);
        Task<bool> TrySetWorkspaceAsync(IWorkspace value);

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

        Task<bool> TryInitializeAsync();

        Task<bool> TryInitializeAsync(bool autoSelect);

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
        Task<bool> SaveAsync();

        /// <summary>
        /// Stores the workspace by requesting information.
        /// </summary>
        Task StoreWorkspaceAsync(IWorkspace workspace);

        List<IWorkspaceProvider> GetWorkspaceProviders();
        Task GetInformationFromProvidersAsync(IWorkspace workspace);
        Task ApplyWorkspaceUsingProvidersAsync(IWorkspace workspace);
    }
}