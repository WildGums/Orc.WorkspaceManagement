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
    using Catel;
    using Fluent;

    internal class WorkspaceContainerLocator : IWorkspaceContainerLocator
    {
        public Panel GetContainerByWorkspaceParent(UIElement parent)
        {
            Argument.IsNotNull(() => parent);

            var dropDownControl = parent as IDropDownControl;
            if (dropDownControl == null)
            {
                return null;
            }

            return dropDownControl.DropDownPopup.Child as Panel;
        }
    }
}