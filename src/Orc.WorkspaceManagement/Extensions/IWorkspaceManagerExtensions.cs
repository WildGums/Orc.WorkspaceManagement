// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceManagerExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;

    public static class IWorkspaceManagerExtensions
    {
        public static Task RefreshCurrentWorkspaceAsync(this IWorkspaceManager workspaceManager)
        {
            Argument.IsNotNull(() => workspaceManager);

            return workspaceManager.TrySetWorkspaceAsync(workspaceManager.Workspace);
        }

        public static TWorkspace GetWorkspace<TWorkspace>(this IWorkspaceManager workspaceManager)
            where TWorkspace : IWorkspace
        {
            Argument.IsNotNull(() => workspaceManager);

            return (TWorkspace)workspaceManager.Workspace;
        }

        public static IWorkspace FindWorkspace(this IWorkspaceManager workspaceManager, string workspaceName)
        {
            Argument.IsNotNull(() => workspaceManager);
            Argument.IsNotNullOrWhitespace(() => workspaceName);

            return (from workspace in workspaceManager.Workspaces
                    where string.Equals(workspace.Title, workspaceName)
                    select workspace).FirstOrDefault();
        }

        public static TWorkspace FindWorkspace<TWorkspace>(this IWorkspaceManager workspaceManager, string workspaceName)
            where TWorkspace : IWorkspace
        {
            Argument.IsNotNull(() => workspaceManager);

            return (TWorkspace) FindWorkspace(workspaceManager, workspaceName);
        }

        public static async Task AddAsync(this IWorkspaceManager workspaceManager, IWorkspace workspace, bool autoSelect)
        {
            Argument.IsNotNull(() => workspaceManager);
            Argument.IsNotNull(() => workspace);

            await workspaceManager.AddAsync(workspace);

            if (autoSelect)
            {
                await workspaceManager.TrySetWorkspaceAsync(workspace);
            }
        }

        public static async Task AddProviderAsync(this IWorkspaceManager workspaceManager, IWorkspaceProvider workspaceProvider, bool callApplyWorkspaceForCurrentWorkspace)
        {
            Argument.IsNotNull(() => workspaceManager);
            Argument.IsNotNull(() => workspaceProvider);

            workspaceManager.AddProvider(workspaceProvider);

            if (callApplyWorkspaceForCurrentWorkspace)
            {
                var workspace = workspaceManager.Workspace;
                if (workspace != null)
                {
                    await workspaceProvider.ApplyWorkspaceAsync(workspace);
                }
            }
        }

        public static Task AddProviderAsync<TWorkspaceProvider>(this IWorkspaceManager workspaceManager, bool callApplyWorkspaceForCurrentWorkspace)
            where TWorkspaceProvider : IWorkspaceProvider
        {
            Argument.IsNotNull(() => workspaceManager);

            var workspaceProvider = TypeFactory.Default.CreateInstance<TWorkspaceProvider>();
            return workspaceManager.AddProviderAsync(workspaceProvider, callApplyWorkspaceForCurrentWorkspace);
        }

        public static async Task EnsureDefaultWorkspaceAsync(this IWorkspaceManager workspaceManager, string defaultWorkspaceName = "Default", bool autoSelect = true)
        {
            Argument.IsNotNull(() => workspaceManager);

            workspaceManager.DefaultWorkspaceTitle = defaultWorkspaceName;

            var defaultWorkspace = (from workspace in workspaceManager.Workspaces
                                    where string.Equals(workspace.Title, defaultWorkspaceName)
                                    select workspace).FirstOrDefault();

            if (defaultWorkspace is null)
            {
                defaultWorkspace = new Workspace(defaultWorkspaceName)
                {
                    Persist = false,
                    CanEdit = false,
                    CanDelete = false
                };

                await workspaceManager.AddAsync(defaultWorkspace, autoSelect);
            }
        }

        public static async Task InitializeAsync(this IWorkspaceManager workspaceManager, bool addDefaultWorkspaceIfNoWorkspacesAreFound = true, bool alwaysEnsureDefaultWorkspace = true,
            string defaultWorkspaceName = "Default", bool autoSelect = true)
        {
            Argument.IsNotNull(() => workspaceManager);

            if (!await workspaceManager.TryInitializeAsync(false))
            {
                return;
            }

            workspaceManager.DefaultWorkspaceTitle = defaultWorkspaceName;

            if (alwaysEnsureDefaultWorkspace || (addDefaultWorkspaceIfNoWorkspacesAreFound && !workspaceManager.Workspaces.Any()))
            {
                await workspaceManager.EnsureDefaultWorkspaceAsync(defaultWorkspaceName, false);
            }

            if (autoSelect && workspaceManager.Workspace is null && workspaceManager.Workspaces.Any())
            {
                var workspace = workspaceManager.Workspaces.FirstOrDefault(x => string.Equals(x.Title, defaultWorkspaceName))
                    ?? workspaceManager.Workspaces.FirstOrDefault();

                await workspaceManager.TrySetWorkspaceAsync(workspace);
            }
        }

        public static Task SetWorkspaceSchemesDirectoryAsync(this IWorkspaceManager workspaceManager, string directoryName, bool addDefaultWorkspaceIfNoWorkspacesAreFound = true,
            bool alwaysEnsureDefaultWorkspace = true, string defaultWorkspaceName = "Default", bool autoselectDefault = true)
        {
            Argument.IsNotNull(() => workspaceManager);
            Argument.IsNotNullOrEmpty(() => directoryName);
            Argument.IsNotNullOrEmpty(() => defaultWorkspaceName);

            workspaceManager.BaseDirectory = directoryName;
            return workspaceManager.InitializeAsync(addDefaultWorkspaceIfNoWorkspacesAreFound, alwaysEnsureDefaultWorkspace, defaultWorkspaceName, autoselectDefault);
        }

        /// <summary>
        /// Stores the workspace and saves it immediately.
        /// </summary>
        /// <param name="workspaceManager">The workspace manager.</param>
        /// <returns>Task.</returns>
        public static async Task StoreAndSaveAsync(this IWorkspaceManager workspaceManager)
        {
            Argument.IsNotNull(() => workspaceManager);

            await workspaceManager.StoreWorkspaceAsync();
            await workspaceManager.SaveAsync();
        }

        public static async Task<bool> CheckIsDirtyAsync(this IWorkspaceManager workspaceManager)
        {
            Argument.IsNotNull(() => workspaceManager);

            var workspace = workspaceManager.Workspace;
            if (workspace is null)
            {
                return true;
            }

            if (workspace.Equals(workspaceManager.RefreshingWorkspace))
            {
                return false;
            }

            foreach (var provider in workspaceManager.Providers)
            {
                if (await provider.CheckIsDirtyAsync(workspace))
                {
                    return true;
                }
            }

            return false;
        }

        public static async Task<bool> IsWorkspaceDirtyAsync(this IWorkspaceManager workspaceManager, IWorkspace workspace)
        {
            Argument.IsNotNull(() => workspaceManager);
            Argument.IsNotNull(() => workspace);

            if (workspaceManager.Providers is null)
            {
                return false;
            }

            if (workspace.Equals(workspaceManager.RefreshingWorkspace))
            {
                return false;
            }

            foreach (var provider in workspaceManager.Providers)
            {
                if (await provider.CheckIsDirtyAsync(workspace))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
