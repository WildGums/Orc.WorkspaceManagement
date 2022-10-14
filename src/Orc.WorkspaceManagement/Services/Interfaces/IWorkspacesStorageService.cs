namespace Orc.WorkspaceManagement
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWorkspacesStorageService
    {
        Task<IEnumerable<IWorkspace>> LoadWorkspacesAsync(string path);
        Task<IWorkspace?> LoadWorkspaceAsync(string fileName);

        Task SaveWorkspacesAsync(string path, IEnumerable<IWorkspace> workspaces);
        Task SaveWorkspaceAsync(string fileName, IWorkspace workspace);

        string GetWorkspaceFileName(string directory, IWorkspace workspace);
    }
}
