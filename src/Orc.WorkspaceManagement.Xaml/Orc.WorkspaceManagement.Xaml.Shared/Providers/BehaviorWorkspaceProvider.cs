// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BehaviorWorkspaceProvider.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.Threading.Tasks;
    using Behaviors;
    using Catel;
    using Catel.Services;
    using Catel.Threading;

    public class BehaviorWorkspaceProvider : WorkspaceProviderBase
    {
        private readonly IWorkspaceBehavior _workspaceBehavior;
        private readonly IDispatcherService _dispatcherService;

        public BehaviorWorkspaceProvider(IWorkspaceManager workspaceManager, IWorkspaceBehavior workspaceBehavior, IDispatcherService dispatcherService) 
            : base(workspaceManager)
        {
            Argument.IsNotNull(() => workspaceBehavior);
            Argument.IsNotNull(() => dispatcherService);

            _workspaceBehavior = workspaceBehavior;
            _dispatcherService = dispatcherService;
        }

        public override Task ProvideInformationAsync(IWorkspace workspace)
        {
            if (workspace == null)
            {
                return TaskHelper.Completed;
            }

            return _dispatcherService.InvokeAsync(() => _workspaceBehavior.Save(workspace));
        }

        public override Task ApplyWorkspaceAsync(IWorkspace workspace)
        {
            if (workspace == null)
            {
                return TaskHelper.Completed;
            }

            return _dispatcherService.InvokeAsync(() => _workspaceBehavior.Load(workspace));
        }
    }
}