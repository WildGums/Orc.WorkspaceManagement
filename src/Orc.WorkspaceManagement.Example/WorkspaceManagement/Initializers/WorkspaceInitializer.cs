namespace Orc.WorkspaceManagement.Example.WorkspaceManagement;

using System;
using System.Threading.Tasks;
using Catel;
using Catel.Threading;

public class WorkspaceInitializer : IWorkspaceInitializer
{
    public void Initialize(IWorkspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);

        workspace.SetWorkspaceValue("AView.Width", 200d);
        workspace.SetWorkspaceValue("BView.Width", 200d);
    }

    public Task InitializeAsync(IWorkspace workspace)
    {
        Initialize(workspace);

        return Task.CompletedTask;
    }
}
