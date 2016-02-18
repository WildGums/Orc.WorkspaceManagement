// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceProviderBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;
    using Catel.IoC;
    using Catel.Threading;

    /// <summary>
    /// Base implementation for workspace providers.
    /// </summary>
    public abstract class WorkspaceProviderBase : IWorkspaceProvider
    {
        #region Fields
        protected readonly IServiceLocator ServiceLocator;

        private object _scope;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceProviderBase"/> class.
        /// </summary>
        /// <param name="workspaceManager">The workspace manager.</param>
        [ObsoleteEx(ReplacementTypeOrMember = "WorkspaceProviderBase(IWorkspaceManager, IServiceLocator)", TreatAsErrorFromVersion = "1.0",
            RemoveInVersion = "2.0")]
        protected WorkspaceProviderBase(IWorkspaceManager workspaceManager)
            : this(workspaceManager, Catel.IoC.ServiceLocator.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceProviderBase"/> class.
        /// </summary>
        /// <param name="workspaceManager">The workspace manager.</param>
        /// <param name="serviceLocator"></param>
        protected WorkspaceProviderBase(IWorkspaceManager workspaceManager, IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => workspaceManager);
            Argument.IsNotNull(() => serviceLocator);

            ServiceLocator = serviceLocator;

            WorkspaceManager = workspaceManager;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the workspace manager.
        /// </summary>
        /// <value>The workspace manager.</value>
        protected IWorkspaceManager WorkspaceManager { get; set; }
        #endregion

        #region IWorkspaceProvider Members
        public virtual object Scope
        {
            get { return _scope; }
            set
            {
                var workspaceManager = ServiceLocator.ResolveType<IWorkspaceManager>(value);
                if (workspaceManager == null)
                {
                    throw new PropertyNotNullableException("WorkspaceManager", typeof(IWorkspaceManager));
                }

                WorkspaceManager = workspaceManager;
                _scope = value;
            }
        }

        public object Tag { get; set; }

        /// <summary>
        /// Provides the information for the workspace with the current state.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        public abstract Task ProvideInformationAsync(IWorkspace workspace);

        /// <summary>
        /// Applies the workspace values in response to a workspace change.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        public abstract Task ApplyWorkspaceAsync(IWorkspace workspace);

        public virtual Task<bool> CheckIsDirtyAsync(IWorkspace workspace)
        {
            return TaskHelper<bool>.FromResult(false);
        }
        #endregion
    }
}