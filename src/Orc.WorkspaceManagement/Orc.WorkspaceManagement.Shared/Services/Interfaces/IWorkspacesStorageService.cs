// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspacesStorageService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.Collections.Generic;

    public interface IWorkspacesStorageService
    {
        IEnumerable<IWorkspace> LoadWorkspaces(string path);
        void SaveWorkspaces(string path, IEnumerable<IWorkspace> workspaces);
        IWorkspace LoadWorkspace(string fileName);
        void SaveWorkspace(string fileName, IWorkspace workspace);
        string GetWorkspaceFileName(string directory, IWorkspace workspace);
    }
}