namespace Orc.WorkspaceManagement
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Catel;
    using Catel.Configuration;
    using Catel.Data;
    using Catel.Logging;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Xml;
    using Path = Catel.IO.Path;
    using Orc.FileSystem;
    using System.Threading.Tasks;

    public class WorkspacesStorageService : IWorkspacesStorageService
    {
        private const string WorkspaceFileExtension = ".xml";

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        protected readonly ISerializationManager _serializationManager;
        protected readonly IXmlSerializer _xmlSerializer;
        protected readonly IFileService _fileService;
        protected readonly IDirectoryService _directoryService;

        public WorkspacesStorageService(ISerializationManager serializationManager, IXmlSerializer xmlSerializer,
            IFileService fileService, IDirectoryService directoryService)
        {
            Argument.IsNotNull(() => serializationManager);
            Argument.IsNotNull(() => xmlSerializer);
            Argument.IsNotNull(() => fileService);
            Argument.IsNotNull(() => directoryService);

            _serializationManager = serializationManager;
            _xmlSerializer = xmlSerializer;
            _fileService = fileService;
            _directoryService = directoryService;
        }

        public virtual async Task<IEnumerable<IWorkspace>> LoadWorkspacesAsync(string path)
        {
            Argument.IsNotNullOrEmpty(() => path);

            var workspaces = new List<IWorkspace>();

            if (_directoryService.Exists(path))
            {
                // Note: since Catel caches serializable members of an object, we might have introduced new dynamic members,
                // so we need to clear the cache in order to make sure we always (deserialize) the right members
                _serializationManager.Clear(typeof(Workspace));
                _serializationManager.Clear(typeof(DynamicConfiguration));

                foreach (var workspaceFile in _directoryService.GetFiles(path, $"*{WorkspaceFileExtension}"))
                {
                    var workspace = await LoadWorkspaceAsync(workspaceFile);
                    if (workspace != null)
                    {
                        workspaces.Add(workspace);
                    }
                }
            }

            return workspaces;
        }

        public virtual async Task<IWorkspace> LoadWorkspaceAsync(string fileName)
        {
            Argument.IsNotNullOrEmpty(() => fileName);

            IWorkspace result = null;

            try
            {
                Log.Debug("Loading workspace from '{0}'", fileName);

                if (!_fileService.Exists(fileName))
                {
                    Log.Warning("File '{0}' not found. Maybe this workspace hasn't been saved yet or doesn't need a save location'.", fileName);

                    return null;
                }

                using (var fileStream = _fileService.Open(fileName, FileMode.Open))
                {
                    var workspace = _xmlSerializer.Deserialize<Workspace>(fileStream);
                    if (workspace is null || string.IsNullOrEmpty(workspace.Title))
                    {
                        Log.Warning("File '{0}' doesn't look like a workspace, ignoring file", fileName);
                    }
                    else
                    {
                        result = workspace;

                        Log.Debug("Loaded workspace");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load workspace from '{0}'", fileName);
            }

            return result;
        }

        public virtual async Task SaveWorkspacesAsync(string path, IEnumerable<IWorkspace> workspaces)
        {
            Argument.IsNotNullOrEmpty(() => path);
            Argument.IsNotNull(() => workspaces);

            _directoryService.Create(path);

            Log.Debug("Deleting previous workspace files");

            foreach (var workspaceFile in _directoryService.GetFiles(path, $"*{WorkspaceFileExtension}"))
            {
                try
                {
                    Log.Debug("Deleting file '{0}'", workspaceFile);

                    _fileService.Delete(workspaceFile);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to delete file '{0}'", workspaceFile);
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
            Argument.IsNotNull(() => workspace);

            if (!workspace.Persist)
            {
                Log.Debug("Workspace '{0}' should not be persisted, skipping save of workspace", workspace);
                return;
            }

            Log.Debug("Saving workspace '{0}' to '{1}'", workspace, fileName);

            ((Workspace)workspace).SaveAsXml(fileName);
        }


        public string GetWorkspaceFileName(string directory, IWorkspace workspace)
        {
            Argument.IsNotNull(() => workspace);

            var workspaceFile = Path.Combine(directory, $"{workspace.Title.GetSlug()}{WorkspaceFileExtension}");
            return workspaceFile;
        }
    }
}
