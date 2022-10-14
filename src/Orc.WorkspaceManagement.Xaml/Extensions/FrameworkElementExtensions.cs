﻿namespace Orc.WorkspaceManagement
{
    using System;
    using System.Windows;
    using Catel.IoC;

    public static partial class FrameworkElementExtensions
    {
        public static void SaveValueToWorkspace(this FrameworkElement frameworkElement, string name, object? value, IWorkspace? workspace = null, string? prefix = null)
        {
            ArgumentNullException.ThrowIfNull(frameworkElement);

            workspace = GetWorkspace(workspace);
            if (workspace is null)
            {
                return;
            }

            prefix = GetPrefix(frameworkElement, prefix);

            var key = string.Format("{0}.{1}", prefix, name);

            workspace.SetWorkspaceValue(key, value);
        }

        public static T LoadValueFromWorkspace<T>(this FrameworkElement frameworkElement, string name, T defaultValue, IWorkspace? workspace = null, string? prefix = null)
        {
            ArgumentNullException.ThrowIfNull(frameworkElement);

            workspace = GetWorkspace(workspace);
            if (workspace is null)
            {
                return defaultValue;
            }

            prefix = GetPrefix(frameworkElement, prefix);

            var key = string.Format("{0}.{1}", prefix, name);

            return workspace.GetWorkspaceValue(key, defaultValue);
        }

        private static IWorkspace? GetWorkspace(IWorkspace? workspace)
        {
            if (workspace is null)
            {
                var workspaceManager = ServiceLocator.Default.ResolveType<IWorkspaceManager>();
                workspace = workspaceManager?.Workspace;
            }

            return workspace;
        }

        private static string? GetPrefix(FrameworkElement frameworkElement, string? prefix = null)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                prefix = frameworkElement.GetType().FullName;
            }

            return prefix;
        }
    }
}
