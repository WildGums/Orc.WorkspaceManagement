// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceManager.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
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
        object Tag { get; set; }

        [ObsoleteEx(ReplacementTypeOrMember = nameof(InitializingAsync), TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        event EventHandler<EventArgs> Initializing;
        event AsyncEventHandler<CancelEventArgs> InitializingAsync;
        [ObsoleteEx(ReplacementTypeOrMember = nameof(InitializedAsync), TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        event EventHandler<EventArgs> Initialized;
        event AsyncEventHandler<EventArgs> InitializedAsync;

        [ObsoleteEx(ReplacementTypeOrMember = nameof(SavingAsync), TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        event EventHandler<EventArgs> Saving;
        event AsyncEventHandler<CancelEventArgs> SavingAsync;
        [ObsoleteEx(ReplacementTypeOrMember = nameof(SavedAsync), TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        event EventHandler<EventArgs> Saved;
        event AsyncEventHandler<EventArgs> SavedAsync;

        [ObsoleteEx(ReplacementTypeOrMember = nameof(WorkspacesChangedAsync), TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        event EventHandler<EventArgs> WorkspacesChanged;
        event AsyncEventHandler<EventArgs> WorkspacesChangedAsync;
        [ObsoleteEx(ReplacementTypeOrMember = nameof(WorkspaceAddedAsync), TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        event EventHandler<WorkspaceEventArgs> WorkspaceAdded;
        event AsyncEventHandler<WorkspaceEventArgs> WorkspaceAddedAsync;
        [ObsoleteEx(ReplacementTypeOrMember = nameof(WorkspaceRemovedAsync), TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        event EventHandler<WorkspaceEventArgs> WorkspaceRemoved;
        event AsyncEventHandler<WorkspaceEventArgs> WorkspaceRemovedAsync;

        [ObsoleteEx(ReplacementTypeOrMember = nameof(WorkspaceProviderAddedAsync), TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        event EventHandler<WorkspaceProviderEventArgs> WorkspaceProviderAdded;
        event AsyncEventHandler<WorkspaceProviderEventArgs> WorkspaceProviderAddedAsync;

        [ObsoleteEx(ReplacementTypeOrMember = nameof(WorkspaceProviderRemovedAsync), TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        event EventHandler<WorkspaceProviderEventArgs> WorkspaceProviderRemoved;
        event AsyncEventHandler<WorkspaceProviderEventArgs> WorkspaceProviderRemovedAsync;

        [ObsoleteEx(ReplacementTypeOrMember = nameof(WorkspaceInfoRequestedAsync), TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        event EventHandler<WorkspaceEventArgs> WorkspaceInfoRequested;
        event AsyncEventHandler<WorkspaceEventArgs> WorkspaceInfoRequestedAsync;

        [ObsoleteEx(ReplacementTypeOrMember = nameof(WorkspaceUpdatingAsync), TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        event EventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdating;
        event AsyncEventHandler<WorkspaceUpdatingEventArgs> WorkspaceUpdatingAsync;
        [ObsoleteEx(ReplacementTypeOrMember = nameof(WorkspaceUpdatedAsync), TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        event EventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdated;
        event AsyncEventHandler<WorkspaceUpdatedEventArgs> WorkspaceUpdatedAsync;

        Task SetWorkspaceAsync(IWorkspace value);
        Task<bool> TrySetWorkspaceAsync(IWorkspace value);

        /// <summary>
        /// Initializes the workspaces by reading them from the <see cref="WorkspaceManager.BaseDirectory" />.
        /// </summary>
        /// <returns>Task.</returns>
        Task InitializeAsync();
        Task<bool> TryInitializeAsync();

        /// <summary>
        /// Initializes the specified automatic select.
        /// </summary>
        /// <param name="autoSelect">if set to <c>true</c> [automatic select].</param>
        Task InitializeAsync(bool autoSelect);
        Task<bool> TryInitializeAsync(bool autoSelect);

        /// <summary>
        /// Adds the provider that will provide information to the workspace when the information is requested.
        /// </summary>
        /// <param name="workspaceProvider">The workspace provider.</param>
        [ObsoleteEx(ReplacementTypeOrMember = nameof(AddProviderAsync), TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        void AddProvider(IWorkspaceProvider workspaceProvider);
        Task AddProviderAsync(IWorkspaceProvider workspaceProvider);

        /// <summary>
        /// Removes the provider that will provide information to the workspace when the information is requested.
        /// </summary>
        /// <param name="workspaceProvider">The workspace provider.</param>
        /// <returns><c>true</c> if the workspace provider is deleted; otherwise <c>false</c>.</returns>
        [ObsoleteEx(ReplacementTypeOrMember = nameof(RemoveProviderAsync), TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        bool RemoveProvider(IWorkspaceProvider workspaceProvider);
        Task<bool> RemoveProviderAsync(IWorkspaceProvider workspaceProvider);

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
        [ObsoleteEx(ReplacementTypeOrMember = nameof(SaveAsync), TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        void Save();
        Task<bool> SaveAsync();
    }
}