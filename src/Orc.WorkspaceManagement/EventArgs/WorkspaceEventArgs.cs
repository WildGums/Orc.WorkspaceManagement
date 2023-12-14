namespace Orc.WorkspaceManagement;

using System;

public class WorkspaceEventArgs : EventArgs
{
    public WorkspaceEventArgs(IWorkspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);

        Workspace = workspace;
    }

    public IWorkspace Workspace { get; }
}
