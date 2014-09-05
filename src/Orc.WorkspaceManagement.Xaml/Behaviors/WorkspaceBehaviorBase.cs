// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceBehaviorBase.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Behaviors
{
    using System.Windows;
    using Catel.IoC;
    using Catel.Windows.Interactivity;

    public abstract class WorkspaceBehaviorBase<T> : BehaviorBase<T>
        where T : FrameworkElement
    {
        #region Constructors
        protected WorkspaceBehaviorBase()
        {
            var dependencyResolver = this.GetDependencyResolver();
            WorkspaceManager = dependencyResolver.Resolve<IWorkspaceManager>();
        }
        #endregion

        #region Properties
        protected IWorkspaceManager WorkspaceManager { get; private set; }

        public string KeyPrefix
        {
            get { return (string)GetValue(KeyPrefixProperty); }
            set { SetValue(KeyPrefixProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyPrefix.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyPrefixProperty =
            DependencyProperty.Register("KeyPrefix", typeof(string), typeof(WorkspaceBehaviorBase<T>), new PropertyMetadata(""));
        #endregion

        #region Methods
        protected override void OnAssociatedObjectLoaded()
        {
            base.OnAssociatedObjectLoaded();

            WorkspaceManager.WorkspaceUpdated += OnWorkspaceUpdated;
            WorkspaceManager.WorkspaceInfoRequested += OnWorkspaceInfoRequested;

            var workspace = WorkspaceManager.Workspace;
            if (workspace != null)
            {
                LoadSettings(workspace, KeyPrefix);
            }
        }

        protected override void OnAssociatedObjectUnloaded()
        {
            WorkspaceManager.WorkspaceUpdated -= OnWorkspaceUpdated;
            WorkspaceManager.WorkspaceInfoRequested -= OnWorkspaceInfoRequested;

            base.OnAssociatedObjectUnloaded();
        }

        private void OnWorkspaceInfoRequested(object sender, WorkspaceEventArgs e)
        {
            SaveSettings(e.Workspace, KeyPrefix);
        }

        private void OnWorkspaceUpdated(object sender, WorkspaceUpdatedEventArgs e)
        {
            LoadSettings(e.NewWorkspace, KeyPrefix);
        }

        protected abstract void SaveSettings(IWorkspace workspace, string prefix);

        protected abstract void LoadSettings(IWorkspace workspace, string prefix);
        #endregion
    }
}