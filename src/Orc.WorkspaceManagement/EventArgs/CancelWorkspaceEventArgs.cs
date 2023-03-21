namespace Orc.WorkspaceManagement;

using System;
using System.ComponentModel;

public class CancelWorkspaceEventArgs : CancelEventArgs
{
    public CancelWorkspaceEventArgs(IWorkspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);

        Workspace = workspace;
    }

    public IWorkspace Workspace { get; }
}
