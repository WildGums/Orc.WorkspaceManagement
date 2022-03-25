﻿
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
#pragma warning disable IDISP006 // Implement IDisposable.
        private readonly IServiceLocator _serviceLocator;
#pragma warning restore IDISP006 // Implement IDisposable.
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
            if (workspace is null)
            {
                return false;
            }

            var workspaceManager = _serviceLocator.ResolveType<IWorkspaceManager>(workspace.Scope);

            return workspaceManager is not null && ObjectHelper.AreEqual(workspaceManager.Workspace, workspace);
        }
        #endregion
    }
}
