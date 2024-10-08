namespace Orc.WorkspaceManagement;

using System;
using System.Linq;
using System.Threading.Tasks;
using Catel;
using Catel.IoC;

public static class IWorkspaceManagerExtensions
{
    public static Task RefreshCurrentWorkspaceAsync(this IWorkspaceManager workspaceManager)
    {
        ArgumentNullException.ThrowIfNull(workspaceManager);

        return workspaceManager.TrySetWorkspaceAsync(workspaceManager.Workspace);
    }

    public static TWorkspace? GetWorkspace<TWorkspace>(this IWorkspaceManager workspaceManager)
        where TWorkspace : IWorkspace
    {
        ArgumentNullException.ThrowIfNull(workspaceManager);

        return (TWorkspace?)workspaceManager.Workspace;
    }

    public static IWorkspace? FindWorkspace(this IWorkspaceManager workspaceManager, string workspaceName)
    {
        ArgumentNullException.ThrowIfNull(workspaceManager);
        Argument.IsNotNullOrWhitespace(() => workspaceName);

        return (from workspace in workspaceManager.Workspaces
            where string.Equals(workspace.Title, workspaceName)
            select workspace).FirstOrDefault();
    }

    public static TWorkspace? FindWorkspace<TWorkspace>(this IWorkspaceManager workspaceManager, string workspaceName)
        where TWorkspace : IWorkspace
    {
        ArgumentNullException.ThrowIfNull(workspaceManager);

        return (TWorkspace?) FindWorkspace(workspaceManager, workspaceName);
    }

    public static async Task AddProviderAsync(this IWorkspaceManager workspaceManager, IWorkspaceProvider workspaceProvider, bool callApplyWorkspaceForCurrentWorkspace)
    {
        ArgumentNullException.ThrowIfNull(workspaceManager);
        ArgumentNullException.ThrowIfNull(workspaceProvider);

        workspaceManager.AddProvider(workspaceProvider);

        if (callApplyWorkspaceForCurrentWorkspace)
        {
            var workspace = workspaceManager.Workspace;
            if (workspace is not null)
            {
                await workspaceProvider.ApplyWorkspaceAsync(workspace);
            }
        }
    }

    public static Task AddProviderAsync<TWorkspaceProvider>(this IWorkspaceManager workspaceManager, bool callApplyWorkspaceForCurrentWorkspace)
        where TWorkspaceProvider : IWorkspaceProvider
    {
        ArgumentNullException.ThrowIfNull(workspaceManager);

        var workspaceProvider = TypeFactory.Default.CreateRequiredInstance<TWorkspaceProvider>();
        return workspaceManager.AddProviderAsync(workspaceProvider, callApplyWorkspaceForCurrentWorkspace);
    }

    /// <summary>
    /// Stores the workspace and saves it immediately.
    /// </summary>
    /// <param name="workspaceManager">The workspace manager.</param>
    /// <returns>Task.</returns>
    public static async Task StoreAndSaveAsync(this IWorkspaceManager workspaceManager)
    {
        ArgumentNullException.ThrowIfNull(workspaceManager);

        await workspaceManager.StoreWorkspaceAsync();
        await workspaceManager.SaveAsync();
    }

    public static async Task<bool> CheckIsDirtyAsync(this IWorkspaceManager workspaceManager)
    {
        ArgumentNullException.ThrowIfNull(workspaceManager);

        var workspace = workspaceManager.Workspace;
        if (workspace is null)
        {
            return true;
        }

        if (!workspace.Persist || !workspace.CanEdit)
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

    public static async Task<bool> IsWorkspaceDirtyAsync(this IWorkspaceManager workspaceManager, IWorkspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspaceManager);
        ArgumentNullException.ThrowIfNull(workspace);

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
