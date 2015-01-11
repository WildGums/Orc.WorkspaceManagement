// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspacesStorageService.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2015 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Services
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
                foreach (var workspaceFile in Directory.GetFiles(path, string.Format("*{0}", WorkspaceFileExtension)))
                {
                    var workspace = LoadWorkspaceFromFile(workspaceFile);
                    if (workspace != null)
                    {
                        yield return workspace;
                    }
                }
            }
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

            foreach (var workspaceFile in Directory.GetFiles(path, string.Format("*{0}", WorkspaceFileExtension)))
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
                var workspaceFile = Path.Combine(path, string.Format("{0}{1}", workspace.Title.GetSlug(), WorkspaceFileExtension));

                Log.Debug("Saving workspace '{0}' to '{1}'", workspace, workspaceFile);

                ((Workspace) workspace).SaveAsXml(workspaceFile);
            }
        }

        private static IWorkspace LoadWorkspaceFromFile(string workspaceFile)
        {
            Argument.IsNotNullOrEmpty(() => workspaceFile);

            IWorkspace result = null;

            try
            {
                Log.Debug("Loading workspace from '{0}'", workspaceFile);

                using (var fileStream = new FileStream(workspaceFile, FileMode.Open))
                {
                    var workspace = ModelBase.Load<Workspace>(fileStream, SerializationMode.Xml);
                    if (workspace == null || string.IsNullOrEmpty(workspace.Title))
                    {
                        Log.Warning("File '{0}' doesn't look like a workspace, ignoring file", workspaceFile);
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
                Log.Error(ex, "Failed to load workspace from '{0}'", workspaceFile);
            }

            return result;
        }
    }
}