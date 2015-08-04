// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceManagerExtensions.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.Linq;
    using Catel;
    using Catel.IoC;

    public static class IWorkspaceManagerExtensions
    {
        public static void RefreshCurrentWorkspace(this IWorkspaceManager workspaceManager)
        {
            Argument.IsNotNull(() => workspaceManager);

            workspaceManager.Workspace = workspaceManager.Workspace;
        }

        public static TWorkspace GetWorkspace<TWorkspace>(this IWorkspaceManager workspaceManager)
            where TWorkspace : IWorkspace
        {
            Argument.IsNotNull(() => workspaceManager);

            return (TWorkspace)workspaceManager.Workspace;
        }

        public static void Add(this IWorkspaceManager workspaceManager, IWorkspace workspace, bool autoSelect)
        {
            Argument.IsNotNull(() => workspaceManager);
            Argument.IsNotNull(() => workspace);

            workspaceManager.Add(workspace);

            if (autoSelect)
            {
                workspaceManager.Workspace = workspace;
            }
        }

        public static void AddProvider(this IWorkspaceManager workspaceManager, IWorkspaceProvider workspaceProvider, bool callApplyWorkspaceForCurrentWorkspace)
        {
            Argument.IsNotNull(() => workspaceManager);
            Argument.IsNotNull(() => workspaceProvider);

            workspaceManager.AddProvider(workspaceProvider);

            if (callApplyWorkspaceForCurrentWorkspace)
            {
                var workspace = workspaceManager.Workspace;
                if (workspace != null)
                {
                    workspaceProvider.ApplyWorkspace(workspace);
                }
            }
        }

        public static void AddProvider<TWorkspaceProvider>(this IWorkspaceManager workspaceManager, bool callApplyWorkspaceForCurrentWorkspace)
            where TWorkspaceProvider : IWorkspaceProvider
        {
            Argument.IsNotNull(() => workspaceManager);

            var workspaceProvider = TypeFactory.Default.CreateInstance<TWorkspaceProvider>();
            workspaceManager.AddProvider(workspaceProvider, callApplyWorkspaceForCurrentWorkspace);
        }

        public static void EnsureDefaultWorkspace(this IWorkspaceManager workspaceManager, string defaultWorkspaceName = "Default", bool autoSelect = true)
        {
            Argument.IsNotNull(() => workspaceManager);

            var defaultWorkspace = (from workspace in workspaceManager.Workspaces
                                    where string.Equals(workspace.Title, defaultWorkspaceName)
                                    select workspace).FirstOrDefault();

            if (defaultWorkspace == null)
            {
                defaultWorkspace = new Workspace
                {
                    Title = defaultWorkspaceName,
                    Persist = false,
                    CanEdit = false,
                    CanDelete = false,
                };

                workspaceManager.Add(defaultWorkspace, autoSelect);
            }
        }

        public static void Initialize(this IWorkspaceManager workspaceManager, bool addDefaultWorkspaceIfNoWorkspacesAreFound = true, bool alwaysEnsureDefaultWorkspace = true, 
            string defaultWorkspaceName = "Default", bool autoSelect = true)
        {
            Argument.IsNotNull(() => workspaceManager);

            workspaceManager.Initialize(false);

            if (alwaysEnsureDefaultWorkspace || (addDefaultWorkspaceIfNoWorkspacesAreFound && !workspaceManager.Workspaces.Any()))
            {
                EnsureDefaultWorkspace(workspaceManager, defaultWorkspaceName, autoSelect);
            }

            if (autoSelect && (workspaceManager.Workspace == null || !string.Equals(workspaceManager.Workspace.Title, defaultWorkspaceName)))
            {
                workspaceManager.Workspace = workspaceManager.Workspaces.FirstOrDefault(x => string.Equals(x.Title, defaultWorkspaceName));
            }

            if (autoSelect && workspaceManager.Workspace == null)
            {
                workspaceManager.Workspace = workspaceManager.Workspaces.FirstOrDefault();
            }
        }

        public static void SetWorkspaceSchemesDirectory(this IWorkspaceManager workspaceManager, string directoryName, bool addDefaultWorkspaceIfNoWorkspacesAreFound = true, 
            bool alwaysEnsureDefaultWorkspace = true, string defaultWorkspaceName = "Default", bool autoselectDefault = true)
        {
            Argument.IsNotNull(() => workspaceManager);
            Argument.IsNotNullOrEmpty(() => directoryName);
            Argument.IsNotNullOrEmpty(() => defaultWorkspaceName);

            workspaceManager.BaseDirectory = directoryName;
            workspaceManager.Initialize(addDefaultWorkspaceIfNoWorkspacesAreFound, alwaysEnsureDefaultWorkspace, defaultWorkspaceName, autoselectDefault);
        }

        /// <summary>
        /// Stores the workspace and saves it immediately.
        /// </summary>
        /// <param name="workspaceManager">The workspace manager.</param>
        /// <returns>Task.</returns>
        public static void StoreAndSave(this IWorkspaceManager workspaceManager)
        {
            Argument.IsNotNull(() => workspaceManager);

            workspaceManager.StoreWorkspace();
            workspaceManager.Save();
        }
    }
}