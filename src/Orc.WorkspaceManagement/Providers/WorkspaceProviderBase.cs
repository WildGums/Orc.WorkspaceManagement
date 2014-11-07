// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceProviderBase.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using Catel;

    /// <summary>
    /// Base implementation for workspace providers.
    /// </summary>
    public abstract class WorkspaceProviderBase : IWorkspaceProvider
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceProviderBase"/> class.
        /// </summary>
        /// <param name="workspaceManager">The workspace manager.</param>
        protected WorkspaceProviderBase(IWorkspaceManager workspaceManager)
        {
            Argument.IsNotNull(() => workspaceManager);

            WorkspaceManager = workspaceManager;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the workspace manager.
        /// </summary>
        /// <value>The workspace manager.</value>
        protected IWorkspaceManager WorkspaceManager { get; private set; }
        #endregion

        #region IWorkspaceProvider Members
        /// <summary>
        /// Provides the information for the workspace with the current state.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        public abstract void ProvideInformation(IWorkspace workspace);

        /// <summary>
        /// Applies the workspace values in response to a workspace change.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        public abstract void ApplyWorkspace(IWorkspace workspace);
        #endregion
    }
}