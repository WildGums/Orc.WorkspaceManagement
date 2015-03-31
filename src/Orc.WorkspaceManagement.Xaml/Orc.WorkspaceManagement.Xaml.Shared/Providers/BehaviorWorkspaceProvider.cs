// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BehaviorWorkspaceProvider.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using Behaviors;
    using Catel;

    public class BehaviorWorkspaceProvider : WorkspaceProviderBase
    {
        private readonly IWorkspaceBehavior _workspaceBehavior;

        public BehaviorWorkspaceProvider(IWorkspaceManager workspaceManager, IWorkspaceBehavior workspaceBehavior) 
            : base(workspaceManager)
        {
            Argument.IsNotNull(() => workspaceBehavior);

            _workspaceBehavior = workspaceBehavior;
        }

        public override void ProvideInformation(IWorkspace workspace)
        {
            _workspaceBehavior.Save(workspace);
        }

        public override void ApplyWorkspace(IWorkspace workspace)
        {
            _workspaceBehavior.Load(workspace);
        }
    }
}