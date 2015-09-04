// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceManagerExtensions.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;

    public static class IWorkspaceManagerExtensions
    {
        public static Task RefreshCurrentWorkspaceAsync(this IWorkspaceManager workspaceManager)
        {
            Argument.IsNotNull(() => workspaceManager);

            return workspaceManager.SetWorkspaceAsync(workspaceManager.Workspace);
        }

        public static TWorkspace GetWorkspace<TWorkspace>(this IWorkspaceManager workspaceManager)
            where TWorkspace : IWorkspace
        {
            Argument.IsNotNull(() => workspaceManager);

            return (TWorkspace) workspaceManager.Workspace;
        }

        public static async Task AddAsync(this IWorkspaceManager workspaceManager, IWorkspace workspace, bool autoSelect)
        {
            Argument.IsNotNull(() => workspaceManager);
            Argument.IsNotNull(() => workspace);

            await workspaceManager.AddAsync(workspace);

            if (autoSelect)
            {
                await workspaceManager.SetWorkspaceAsync(workspace);
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

                await workspaceManager.AddAsync(defaultWorkspace, autoSelect);
            }
        }

        public static async Task InitializeAsync(this IWorkspaceManager workspaceManager, bool addDefaultWorkspaceIfNoWorkspacesAreFound = true, bool alwaysEnsureDefaultWorkspace = true,
            string defaultWorkspaceName = "Default", bool autoSelect = true)
        {
            Argument.IsNotNull(() => workspaceManager);

            await workspaceManager.InitializeAsync(false);

            if (alwaysEnsureDefaultWorkspace || (addDefaultWorkspaceIfNoWorkspacesAreFound && !workspaceManager.Workspaces.Any()))
            {
                await EnsureDefaultWorkspaceAsync(workspaceManager, defaultWorkspaceName, autoSelect);
            }

            if (autoSelect && (workspaceManager.Workspace == null || !string.Equals(workspaceManager.Workspace.Title, defaultWorkspaceName)))
            {
                await workspaceManager.SetWorkspaceAsync(workspaceManager.Workspaces.FirstOrDefault(x => string.Equals(x.Title, defaultWorkspaceName)));
            }

            if (autoSelect && workspaceManager.Workspace == null)
            {
                await workspaceManager.SetWorkspaceAsync(workspaceManager.Workspaces.FirstOrDefault());
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
            workspaceManager.Save();
        }

        public static async Task<bool> CheckIsDirtyAsync(this IWorkspaceManager workspaceManager)
        {
            Argument.IsNotNull(() => workspaceManager);

            var workspace = workspaceManager.Workspace;
            if (workspace == null)
            {
                return true;
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