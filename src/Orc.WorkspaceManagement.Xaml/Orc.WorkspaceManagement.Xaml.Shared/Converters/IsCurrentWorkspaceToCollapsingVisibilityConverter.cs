// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsCurrentWorkspaceToCollapsingVisibilityConverter.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Converters
{
    using System;
    using System.Windows;
    using Catel;
    using Catel.IoC;
    using Catel.MVVM.Converters;

    public class IsCurrentWorkspaceToCollapsingVisibilityConverter : VisibilityConverterBase
    {
        #region Fields
        private readonly IWorkspaceManager _workspaceManager;
        #endregion

        #region Constructors
        public IsCurrentWorkspaceToCollapsingVisibilityConverter()
            : base(Visibility.Collapsed)
        {
            var dependencyResolver = this.GetDependencyResolver();
            _workspaceManager = dependencyResolver.Resolve<IWorkspaceManager>();
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

            return ObjectHelper.AreEqual(_workspaceManager.Workspace, workspace);
        }
        #endregion
    }
}