namespace Orc.WorkspaceManagement;

using System;

public class WorkspaceProviderEventArgs : EventArgs
{
    public WorkspaceProviderEventArgs(IWorkspaceProvider workspaceProvider)
    {
        ArgumentNullException.ThrowIfNull(workspaceProvider);

        WorkspaceProvider = workspaceProvider;
    }

    public IWorkspaceProvider WorkspaceProvider { get; }
}
