namespace Orc.WorkspaceManagement;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Catel;
using Catel.Logging;
using FileSystem;
using Microsoft.Extensions.Logging;
using Orc.Serialization.Json;

public class WorkspacesStorageService : IWorkspacesStorageService
{
    private const string WorkspaceFileExtension = ".json";

    private static readonly ILogger Logger = LogManager.GetLogger(typeof(WorkspacesStorageService));

    protected readonly IJsonSerializerFactory _jsonSerializerFactory;
    protected readonly IFileService _fileService;
    protected readonly IDirectoryService _directoryService;

    public WorkspacesStorageService(IJsonSerializerFactory jsonSerializerFactory,
        IFileService fileService, IDirectoryService directoryService)
    {
        _jsonSerializerFactory = jsonSerializerFactory;
        _fileService = fileService;
        _directoryService = directoryService;
    }

    public virtual async Task<IReadOnlyList<IWorkspace>> LoadWorkspacesAsync(string path)
    {
        Argument.IsNotNullOrEmpty(() => path);

        var workspaces = new List<IWorkspace>();

        try
        {
            if (_directoryService.Exists(path))
            {
                foreach (var workspaceFile in _directoryService.GetFiles(path, $"*{WorkspaceFileExtension}"))
                {
                    var workspace = await LoadWorkspaceAsync(workspaceFile);
                    if (workspace is not null)
                    {
                        workspaces.Add(workspace);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Failed to load workspaces '{path}'");
        }

        return workspaces;
    }

    public virtual async Task<IWorkspace?> LoadWorkspaceAsync(string fileName)
    {
        Argument.IsNotNullOrEmpty(() => fileName);

        IWorkspace? result = null;

        try
        {
            Logger.LogDebug("Loading workspace from '{0}'", fileName);

            if (!_fileService.Exists(fileName))
            {
                Logger.LogWarning("File '{0}' not found. Maybe this workspace hasn't been saved yet or doesn't need a save location'.", fileName);

                return null;
            }

            var serializer = _jsonSerializerFactory.CreateSerializer();

            using (var fileStream = _fileService.Open(fileName, FileMode.Open))
            {
                var workspace = serializer.Deserialize<Workspace>(fileStream);
                if (workspace is null || string.IsNullOrEmpty(workspace.Title))
                {
                    Logger.LogWarning("File '{0}' doesn't look like a workspace, ignoring file", fileName);
                }
                else
                {
                    result = workspace;

                    Logger.LogDebug("Loaded workspace");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load workspace from '{0}'", fileName);
        }

        return result;
    }

    public virtual async Task SaveWorkspacesAsync(string path, IReadOnlyList<IWorkspace> workspaces)
    {
        Argument.IsNotNullOrEmpty(() => path);
        ArgumentNullException.ThrowIfNull(workspaces);

        _directoryService.Create(path);

        Logger.LogDebug("Deleting previous workspace files");

        foreach (var workspaceFile in _directoryService.GetFiles(path, $"*{WorkspaceFileExtension}"))
        {
            try
            {
                Logger.LogDebug("Deleting file '{0}'", workspaceFile);

                _fileService.Delete(workspaceFile);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to delete file '{0}'", workspaceFile);
            }
        }

        foreach (var workspace in workspaces)
        {
            var fileName = GetWorkspaceFileName(path, workspace);
            await SaveWorkspaceAsync(fileName, workspace);
        }
    }

    public virtual async Task SaveWorkspaceAsync(string fileName, IWorkspace workspace)
    {
        Argument.IsNotNullOrEmpty(() => fileName);
        ArgumentNullException.ThrowIfNull(workspace);

        if (!workspace.Persist)
        {
            Logger.LogDebug("Workspace '{0}' should not be persisted, skipping save of workspace", workspace);
            return;
        }

        Logger.LogDebug("Saving workspace '{0}' to '{1}'", workspace, fileName);

        var serializer = _jsonSerializerFactory.CreateSerializer();

        using (var fileStream = _fileService.Create(fileName))
        {
            serializer.Serialize(fileStream, workspace);
        }
    }

    public string GetWorkspaceFileName(string directory, IWorkspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);

        var workspaceFile = Path.Combine(directory, $"{workspace.Title.GetSlug()}{WorkspaceFileExtension}");
        return workspaceFile;
    }
}
