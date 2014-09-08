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

    public static class IWorkspaceManagerExtensions
    {
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

        public static async Task Initialize(this IWorkspaceManager workspaceManager, bool addDefaultWorkspaceIfNoWorkspacesAreFound,
            string defaultWorkspaceName = "Default")
        {
            Argument.IsNotNull(() => workspaceManager);

            await workspaceManager.Initialize();

            if (!workspaceManager.Workspaces.Any())
            {
                var defaultWorkspace = new Workspace();
                defaultWorkspace.Title = defaultWorkspaceName;

                workspaceManager.Add(defaultWorkspace, true);
            }
        }
    }
}