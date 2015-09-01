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
    using Catel.Threading;

    public class BehaviorWorkspaceProvider : WorkspaceProviderBase
    {
        private readonly IWorkspaceBehavior _workspaceBehavior;

        public BehaviorWorkspaceProvider(IWorkspaceManager workspaceManager, IWorkspaceBehavior workspaceBehavior) 
            : base(workspaceManager)
        {
            Argument.IsNotNull(() => workspaceBehavior);

            _workspaceBehavior = workspaceBehavior;
        }

        public override Task ProvideInformationAsync(IWorkspace workspace)
        {
            _workspaceBehavior.Save(workspace);
            
            return TaskHelper.Completed;
        }

        public override Task ApplyWorkspaceAsync(IWorkspace workspace)
        {
            _workspaceBehavior.Load(workspace);

            return TaskHelper.Completed;
        }
    }
}