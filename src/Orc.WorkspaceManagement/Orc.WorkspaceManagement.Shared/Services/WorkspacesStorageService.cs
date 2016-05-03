// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspacesStorageService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Catel;
    using Catel.Data;
    using Catel.Logging;
    using Path = Catel.IO.Path;

    public class WorkspacesStorageService : IWorkspacesStorageService
    {
        private const string WorkspaceFileExtension = ".xml";

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public IEnumerable<IWorkspace> LoadWorkspaces(string path)
        {
            Argument.IsNotNullOrEmpty(() => path);

            if (Directory.Exists(path))
            {
                foreach (var workspaceFile in Directory.GetFiles(path, $"*{WorkspaceFileExtension}"))
                {
                    var workspace = LoadWorkspace(workspaceFile);
                    if (workspace != null)
                    {
                        yield return workspace;
                    }
                }
            }
        }

        public IWorkspace LoadWorkspace(string fileName)
        {
            Argument.IsNotNullOrEmpty(() => fileName);

            IWorkspace result = null;

            try
            {
                Log.Debug("Loading workspace from '{0}'", fileName);

                using (var fileStream = new FileStream(fileName, FileMode.Open))
                {
                    var workspace = ModelBase.Load<Workspace>(fileStream, SerializationMode.Xml);
                    if (workspace == null || string.IsNullOrEmpty(workspace.Title))
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

        public void SaveWorkspaces(string path, IEnumerable<IWorkspace> workspaces)
        {
            Argument.IsNotNullOrEmpty(() => path);
            Argument.IsNotNull(() => workspaces);

            if (!Directory.Exists(path))
            {
                Log.Debug("Creating base directory '{0}'", path);

                Directory.CreateDirectory(path);
            }

            Log.Debug("Deleting previous workspace files");

            foreach (var workspaceFile in Directory.GetFiles(path, $"*{WorkspaceFileExtension}"))
            {
                try
                {
                    Log.Debug("Deleting file '{0}'", workspaceFile);

                    File.Delete(workspaceFile);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to delete file '{0}'", workspaceFile);
                }
            }

            foreach (var workspace in workspaces)
            {
                var fileName = GetWorkspaceFileName(path, workspace);
                SaveWorkspace(fileName, workspace);
            }
        }

        public void SaveWorkspace(string fileName, IWorkspace workspace)
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