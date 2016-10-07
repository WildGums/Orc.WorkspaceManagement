// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceProvider.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.Threading.Tasks;

    /// <summary>
    /// Provider that can be registered in the workspace manager to retrieve information about a workspace.
    /// </summary>
    public interface IWorkspaceProvider
    {
        object Scope { get; set; }

        object Tag { get; set; }

        /// <summary>
        /// Provides the information for the workspace with the current state.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        Task ProvideInformationAsync(IWorkspace workspace);

        /// <summary>
        /// Applies the workspace values in response to a workspace change.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        Task ApplyWorkspaceAsync(IWorkspace workspace);

        /// <summary>
        /// Check if workspace was changed
        /// </summary>
        /// <param name="workspace">The workspace</param>
        /// <returns></returns>
        Task<bool> CheckIsDirtyAsync(IWorkspace workspace);
    }
}