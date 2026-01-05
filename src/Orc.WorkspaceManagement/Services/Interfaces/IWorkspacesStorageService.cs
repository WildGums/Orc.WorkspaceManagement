namespace Orc.WorkspaceManagement;

using System.Collections.Generic;
using System.Threading.Tasks;

public interface IWorkspacesStorageService
{
    Task<IReadOnlyList<IWorkspace>> LoadWorkspacesAsync(string path);
    Task<IWorkspace?> LoadWorkspaceAsync(string fileName);

    Task SaveWorkspacesAsync(string path, IReadOnlyList<IWorkspace> workspaces);
    Task SaveWorkspaceAsync(string fileName, IWorkspace workspace);

    string GetWorkspaceFileName(string directory, IWorkspace workspace);
}
