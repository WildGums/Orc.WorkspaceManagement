namespace Orc.WorkspaceManagement;

using System.Threading.Tasks;
using Behaviors;
using Catel.Services;

public class BehaviorWorkspaceProvider : WorkspaceProviderBase
{
    private readonly IWorkspaceBehavior _workspaceBehavior;
    private readonly IDispatcherService _dispatcherService;

    public BehaviorWorkspaceProvider(IWorkspaceManager workspaceManager, IWorkspaceBehavior workspaceBehavior, 
        IDispatcherService dispatcherService) 
        : base(workspaceManager)
    {
        _workspaceBehavior = workspaceBehavior;
        _dispatcherService = dispatcherService;
    }

    public override Task ProvideInformationAsync(IWorkspace workspace)
    {
        if (workspace is null)
        {
            return Task.CompletedTask;
        }

        return _dispatcherService.InvokeAsync(() => _workspaceBehavior.Save(workspace));
    }

    public override Task ApplyWorkspaceAsync(IWorkspace workspace)
    {
        if (workspace is null)
        {
            return Task.CompletedTask;
        }

        return _dispatcherService.InvokeAsync(() => _workspaceBehavior.Load(workspace));
    }
}
