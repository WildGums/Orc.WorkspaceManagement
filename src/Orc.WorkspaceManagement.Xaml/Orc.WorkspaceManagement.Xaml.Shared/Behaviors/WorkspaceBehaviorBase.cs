// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceBehaviorBase.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Behaviors
{
    using System.Windows;
    using System.Windows.Media.Animation;
    using Catel.IoC;
    using Catel.Services;
    using Catel.Windows.Interactivity;

    public abstract class WorkspaceBehaviorBase<T> : BehaviorBase<T>, IWorkspaceBehavior
        where T : FrameworkElement
    {
        private readonly BehaviorWorkspaceProvider _workspaceProvider;

        #region Constructors
        protected WorkspaceBehaviorBase()
        {
            var dependencyResolver = this.GetDependencyResolver();
            WorkspaceManager = dependencyResolver.Resolve<IWorkspaceManager>();
            var dispatcherService = dependencyResolver.Resolve<IDispatcherService>();

            _workspaceProvider = new BehaviorWorkspaceProvider(WorkspaceManager, this, dispatcherService, this.GetServiceLocator());
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

            WorkspaceManager.AddProvider(_workspaceProvider);

            var workspace = WorkspaceManager.Workspace;
            if (workspace != null)
            {
                LoadSettings(workspace, KeyPrefix);
            }
        }

        protected override void OnAssociatedObjectUnloaded()
        {
            WorkspaceManager.RemoveProvider(_workspaceProvider);

            base.OnAssociatedObjectUnloaded();
        }

        protected abstract void SaveSettings(IWorkspace workspace, string prefix);

        protected abstract void LoadSettings(IWorkspace workspace, string prefix);

        public void Load(IWorkspace workspace)
        {
            LoadSettings(workspace, KeyPrefix);
        }

        public void Save(IWorkspace workspace)
        {
            SaveSettings(workspace, KeyPrefix);
        }
        #endregion
    }
}