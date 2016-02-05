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
    }
}