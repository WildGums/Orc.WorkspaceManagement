namespace Orc.WorkspaceManagement
{
    using Catel.Threading;
    using System.Threading.Tasks;

    public class EmptyWorkspaceInitializer : IWorkspaceInitializer
    {
        public Task InitializeAsync(IWorkspace workspace)
        {
            // nothing
            return Task.CompletedTask;
        }
    }
}
