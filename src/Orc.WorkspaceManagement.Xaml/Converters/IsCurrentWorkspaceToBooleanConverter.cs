// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsCurrentWorkspaceToBooleanConverter.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Converters
{
    using System;
    using Catel;
    using Catel.IoC;
    using Catel.MVVM.Converters;
    using Orc.WorkspaceManagement;

    public class IsCurrentWorkspaceToBooleanConverter : ValueConverterBase
    {
        #region Fields
        private readonly IWorkspaceManager _workspaceManager;
        #endregion

        #region Constructors
        public IsCurrentWorkspaceToBooleanConverter()
        {
            var dependencyResolver = this.GetDependencyResolver();
            _workspaceManager = dependencyResolver.Resolve<IWorkspaceManager>();
        }
        #endregion

        #region Methods
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var workspace = value as IWorkspace;
            if (workspace is null)
            {
                return false;
            }

            return ObjectHelper.AreEqual(_workspaceManager.Workspace, workspace);
        }
        #endregion
    }
}
