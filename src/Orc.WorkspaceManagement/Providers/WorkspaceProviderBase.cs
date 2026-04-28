namespace Orc.WorkspaceManagement;

using System.Threading.Tasks;

/// <summary>
/// Base implementation for workspace providers.
/// </summary>
public abstract class WorkspaceProviderBase : IWorkspaceProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkspaceProviderBase"/> class.
    /// </summary>
    protected WorkspaceProviderBase()
    {
    }

    /// <summary>
    /// Provides the information for the workspace with the current state.
    /// </summary>
    /// <param name="workspace">The workspace.</param>
    public abstract Task ProvideInformationAsync(IWorkspace workspace);

    /// <summary>
    /// Applies the workspace values in response to a workspace change.
    /// </summary>
    /// <param name="workspace">The workspace.</param>
    public abstract Task ApplyWorkspaceAsync(IWorkspace workspace);

    public virtual Task<bool> CheckIsDirtyAsync(IWorkspace workspace)
    {
        return Task.FromResult(false);
    }
}
