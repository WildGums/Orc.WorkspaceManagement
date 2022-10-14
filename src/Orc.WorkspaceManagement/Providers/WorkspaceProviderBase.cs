namespace Orc.WorkspaceManagement
{
    using System;
    using System.Threading.Tasks;
    using Catel.IoC;

    /// <summary>
    /// Base implementation for workspace providers.
    /// </summary>
    public abstract class WorkspaceProviderBase : IWorkspaceProvider
    {
        protected readonly IServiceLocator ServiceLocator;

        private object? _scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceProviderBase"/> class.
        /// </summary>
        /// <param name="workspaceManager">The workspace manager.</param>
        /// <param name="serviceLocator"></param>
        protected WorkspaceProviderBase(IWorkspaceManager workspaceManager, IServiceLocator serviceLocator)
        {
            ArgumentNullException.ThrowIfNull(workspaceManager);
            ArgumentNullException.ThrowIfNull(serviceLocator);

            ServiceLocator = serviceLocator;

            WorkspaceManager = workspaceManager;
        }

        /// <summary>
        /// Gets the workspace manager.
        /// </summary>
        /// <value>The workspace manager.</value>
        protected IWorkspaceManager WorkspaceManager { get; set; }
 
        public virtual object? Scope
        {
            get { return _scope; }
            set
            {
                WorkspaceManager = ServiceLocator.ResolveRequiredType<IWorkspaceManager>(value);
                _scope = value;
            }
        }

        public object? Tag { get; set; }

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
            return Task.FromResult(false);
        }
    }
}
