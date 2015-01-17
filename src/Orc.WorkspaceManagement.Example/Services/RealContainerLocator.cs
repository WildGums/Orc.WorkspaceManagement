// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceContainerLocator.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2015 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.WorkspaceManagement.Example.Services
{
    using System.Windows;
    using System.Windows.Controls;
    using Behaviors;

    public class WorkspaceContainerLocator : IWorkspaceContainerLocator
    {
        public Panel GetContainerByWorkspaceParent(UIElement parent)
        {
            return null;
        }
    }
}