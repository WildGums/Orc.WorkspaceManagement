// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceContainerLocator.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2015 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;

    public interface IWorkspaceContainerLocator
    {
        Panel GetContainerByWorkspaceParent(UIElement parent);
    }
}