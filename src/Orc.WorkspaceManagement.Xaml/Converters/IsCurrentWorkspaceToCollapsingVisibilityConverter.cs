﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsCurrentWorkspaceToCollapsingVisibilityConverter.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
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
#pragma warning disable IDISP006 // Implement IDisposable.
        private readonly IServiceLocator _serviceLocator;
#pragma warning restore IDISP006 // Implement IDisposable.
        #endregion

        #region Constructors
        public IsCurrentWorkspaceToCollapsingVisibilityConverter()
            : base(Visibility.Collapsed)
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
