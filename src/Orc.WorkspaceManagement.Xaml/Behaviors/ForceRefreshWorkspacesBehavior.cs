// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ForceRefreshWorkspacesBehavior.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2015 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Catel.IoC;
    using Catel.Windows.Interactivity;
    using Views;

    public class ForceRefreshWorkspacesBehavior : BehaviorBase<WorkspacesView>
    {
        private Panel _parentContainer;
        private WorkspacesView _workspacesView;
        private readonly IWorkspaceContainerLocator _workspaceContainerLocator;
        private readonly IWorkspaceManager _workspaceManager;

        public ForceRefreshWorkspacesBehavior()
        {
            var serviceLocator = this.GetServiceLocator();

            _workspaceManager = serviceLocator.ResolveType<IWorkspaceManager>();
            _workspaceContainerLocator = serviceLocator.ResolveType<IWorkspaceContainerLocator>();  
        }

        protected override void OnAssociatedObjectLoaded()
        {
            
            _workspaceManager.Initialized += OnWorkspaceManagerInitialized;
            _workspaceManager.WorkspacesChanged += OnWorkspacesChanged;
        }

        protected override void OnAssociatedObjectUnloaded()
        {
            _workspaceManager.Initialized -= OnWorkspaceManagerInitialized;
            _workspaceManager.WorkspacesChanged -= OnWorkspacesChanged;
        }

        private void OnWorkspacesChanged(object sender, EventArgs e)
        {
            ForceRefreshPopup();
        }

        private void OnWorkspaceManagerInitialized(object sender, EventArgs e)
        {
            _parentContainer = _workspaceContainerLocator.GetContainerByWorkspaceParent(AssociatedObject);

            ForceRefreshPopup();
        }

        private void ForceRefreshPopup()
        {
            if (_parentContainer != null)
            {
                var children = _parentContainer.Children;

                int i = 0;
                var count = children.Count;
                while (i < count && _workspacesView == null)
                {
                    _workspacesView = children[i++] as WorkspacesView;
                }

                if (_workspacesView == null)
                {
                    _workspacesView = new WorkspacesView();
                }

                children.Clear();
                children.Add(_workspacesView);
            }
        }
    }
}