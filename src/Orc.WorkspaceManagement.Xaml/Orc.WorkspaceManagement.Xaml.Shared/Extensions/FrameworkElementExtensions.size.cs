// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameworkElementExtensions.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.Windows;
    using System.Windows.Controls;
    using Catel.IoC;
    using Catel.Windows.Controls;

    public static partial class FrameworkElementExtensions
    {
        public static void SaveSizeToWorkspace(this FrameworkElement frameworkElement, IWorkspace workspace = null, string prefix = null)
        {
            frameworkElement.SaveValueToWorkspace("Width", frameworkElement.ActualWidth, workspace, prefix);
            frameworkElement.SaveValueToWorkspace("Height", frameworkElement.ActualHeight, workspace, prefix);
        }

        public static void LoadSizeFromWorkspace(this FrameworkElement frameworkElement, IWorkspace workspace = null, string prefix = null)
        {
            var width = frameworkElement.LoadValueFromWorkspace("Width", frameworkElement.Width, workspace, prefix);
            var height = frameworkElement.LoadValueFromWorkspace("Height", frameworkElement.Height, workspace, prefix);

            frameworkElement.Width = width;
            frameworkElement.Height = height;
        }
    }
}