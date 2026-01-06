namespace Orc.WorkspaceManagement.Example.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using Orc.FileSystem;
using Orc.Serialization.Json;

public class ExampleWorkspacesStorageService : WorkspacesStorageService
{
    public ExampleWorkspacesStorageService(IJsonSerializerFactory jsonSerializerFactory,
        IFileService fileService, IDirectoryService directoryService) 
        : base(jsonSerializerFactory, fileService, directoryService)
    {
    }

    public override async Task<IReadOnlyList<IWorkspace>> LoadWorkspacesAsync(string path)
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
