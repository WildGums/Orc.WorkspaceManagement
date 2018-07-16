
namespace Orc.WorkspaceManagement.Converters
{
    using System;
    using System.Windows;
    using Catel;
    using Catel.IoC;
    using Catel.MVVM.Converters;

    public class IsCurrentWorkspaceToHidingVisibilityConverter : VisibilityConverterBase
    {
        #region Fields
        private readonly IServiceLocator _serviceLocator;
        #endregion

        #region Constructors
        public IsCurrentWorkspaceToHidingVisibilityConverter()
            : base(Visibility.Hidden)
        {
            _serviceLocator = this.GetServiceLocator();
        }
        #endregion

        #region Methods
        protected override bool IsVisible(object value, Type targetType, object parameter)
        {
            var workspace = value as IWorkspace;
            if (workspace == null)
            {
                return false;
            }

            var workspaceManager = _serviceLocator.ResolveType<IWorkspaceManager>(workspace.Scope);

            return workspaceManager != null && ObjectHelper.AreEqual(workspaceManager.Workspace, workspace);
        }
        #endregion
    }
}
