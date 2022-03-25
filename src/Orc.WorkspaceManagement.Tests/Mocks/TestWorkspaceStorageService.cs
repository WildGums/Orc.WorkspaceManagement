namespace Orc.WorkspaceManagement.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Catel;
    using FilterBuilder.Tests;

    public class TestWorkspaceStorageService : IWorkspacesStorageService
    {
        public async Task<IEnumerable<IWorkspace>> LoadWorkspacesAsync(string path)
        {
            var scope = path;

            return WorkspacesViewTestData.GetWorkspaces(scope);
        }

        public async Task<IWorkspace> LoadWorkspaceAsync(string fileName)
        {
            var scopeTitle = fileName.Split('/');

            var workspaces = WorkspacesViewTestData.GetWorkspaces(scopeTitle[0]);
            var workspace = workspaces.FirstOrDefault(x => Equals(x.Title, scopeTitle[1]));

            return workspace;
        }

        public async Task SaveWorkspacesAsync(string path, IEnumerable<IWorkspace> workspaces)
        {
            
        }

        public async Task SaveWorkspaceAsync(string fileName, IWorkspace workspace)
        {
            
        }

        public string GetWorkspaceFileName(string directory, IWorkspace workspace)
        {
            Argument.IsNotNull(() => workspace);

            return $"{(workspace as Workspace)?.Scope}/{workspace.Title}";
        }
    }
}
