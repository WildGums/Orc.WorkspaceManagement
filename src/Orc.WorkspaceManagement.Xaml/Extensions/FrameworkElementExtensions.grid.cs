// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameworkElementExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.Windows;
    using System.Windows.Controls;

    public static partial class FrameworkElementExtensions
    {
        public static void SaveGridValuesToWorkspace(this FrameworkElement frameworkElement, IWorkspace workspace = null, string prefix = null)
        {
            frameworkElement.SaveValueToWorkspace("GridRow", Grid.GetRow(frameworkElement), workspace, prefix);
            frameworkElement.SaveValueToWorkspace("GridColumn", Grid.GetColumn(frameworkElement), workspace, prefix);   
        }

        public static void LoadGridValuesFromWorkspace(this FrameworkElement frameworkElement, IWorkspace workspace = null, string prefix = null)
        {
            var row = frameworkElement.LoadValueFromWorkspace("GridRow", Grid.GetRow(frameworkElement), workspace, prefix);
            var column = frameworkElement.LoadValueFromWorkspace("GridColumn", Grid.GetColumn(frameworkElement), workspace, prefix);

            Grid.SetRow(frameworkElement, row);
            Grid.SetColumn(frameworkElement, column);
        }
    }
}