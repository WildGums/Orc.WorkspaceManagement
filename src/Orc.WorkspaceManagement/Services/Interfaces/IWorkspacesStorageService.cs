namespace Orc.WorkspaceManagement.Services
{
    using System.Collections.Generic;

    public interface IWorkspacesStorageService
    {
        IEnumerable<IWorkspace> LoadWorkspaces(string path);

        void SaveWorkspaces(string path, IEnumerable<IWorkspace> workspaces);
    }
}