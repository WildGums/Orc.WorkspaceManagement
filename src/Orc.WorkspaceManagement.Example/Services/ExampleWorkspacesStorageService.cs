namespace Orc.WorkspaceManagement.Example.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Xml;
    using Orc.FileSystem;

    public class ExampleWorkspacesStorageService : WorkspacesStorageService
    {
        public ExampleWorkspacesStorageService(ISerializationManager serializationManager, IXmlSerializer xmlSerializer, 
            IFileService fileService, IDirectoryService directoryService) 
            : base(serializationManager, xmlSerializer, fileService, directoryService)
        {
        }

        public override async Task<IEnumerable<IWorkspace>> LoadWorkspacesAsync(string path)
        {
            var workspaces = new List<IWorkspace>(await base.LoadWorkspacesAsync(path));

            workspaces.Add(new Workspace
            {
                Title = "Demo workspace",
                WorkspaceGroup = "Group name",
                CanDelete = false,
                CanEdit = false,
                Persist = false
            });

            return workspaces;
        }

        //public override async Task SaveWorkspacesAsync(string path, IEnumerable<IWorkspace> workspaces)
        //{
        //    await base.SaveWorkspacesAsync(path, workspaces.Where(x => x.WorkspaceGroup is null));
        //}
    }
}
