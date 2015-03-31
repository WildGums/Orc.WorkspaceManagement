// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceProvider.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    /// <summary>
    /// Provider that can be registered in the workspace manager to retrieve information about a workspace.
    /// </summary>
    public interface IWorkspaceProvider
    {
        /// <summary>
        /// Provides the information for the workspace with the current state.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        void ProvideInformation(IWorkspace workspace);

        /// <summary>
        /// Applies the workspace values in response to a workspace change.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        void ApplyWorkspace(IWorkspace workspace);
    }
}