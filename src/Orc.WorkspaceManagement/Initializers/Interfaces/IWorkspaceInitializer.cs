namespace Orc.WorkspaceManagement;

using System.Threading.Tasks;

public interface IWorkspaceInitializer
{
    Task InitializeAsync(IWorkspace workspace);
}
