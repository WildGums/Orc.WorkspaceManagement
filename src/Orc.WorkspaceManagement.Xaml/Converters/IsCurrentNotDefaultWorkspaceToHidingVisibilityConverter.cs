
namespace Orc.WorkspaceManagement.Converters
{
    using System;
    using System.Windows;
    using Catel;
    using Catel.IoC;
    using Catel.MVVM.Converters;

    public class IsCurrentNotDefaultWorkspaceToHidingVisibilityConverter : VisibilityConverterBase
    {
        #region Fields
        private readonly IServiceLocator _serviceLocator;
        #endregion

        #region Constructors
        public IsCurrentNotDefaultWorkspaceToHidingVisibilityConverter()
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

            if (workspaceManager == null || 
                (string.Equals(workspace.Title, workspaceManager.DefaultWorkspaceTitle)))
            {
                return false;
            }

            return ObjectHelper.AreEqual(workspaceManager.Workspace, workspace);
        }
        #endregion
    }
}
