// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspacesStorageService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWorkspacesStorageService
    {
        [ObsoleteEx(ReplacementTypeOrMember = "LoadWorkspacesAsync", TreatAsErrorFromVersion = "3.0", RemoveInVersion = "4.0")]
        IEnumerable<IWorkspace> LoadWorkspaces(string path);
        [ObsoleteEx(ReplacementTypeOrMember = "LoadWorkspaceAsync", TreatAsErrorFromVersion = "3.0", RemoveInVersion = "4.0")]
        IWorkspace LoadWorkspace(string fileName);

        [ObsoleteEx(ReplacementTypeOrMember = "SaveWorkspacesAsync", TreatAsErrorFromVersion = "3.0", RemoveInVersion = "4.0")]
        void SaveWorkspaces(string path, IEnumerable<IWorkspace> workspaces);
        [ObsoleteEx(ReplacementTypeOrMember = "SaveWorkspaceAsync", TreatAsErrorFromVersion = "3.0", RemoveInVersion = "4.0")]
        void SaveWorkspace(string fileName, IWorkspace workspace);

        Task<IEnumerable<IWorkspace>> LoadWorkspacesAsync(string path);
        Task<IWorkspace> LoadWorkspaceAsync(string fileName);

        Task SaveWorkspacesAsync(string path, IEnumerable<IWorkspace> workspaces);
        Task SaveWorkspaceAsync(string fileName, IWorkspace workspace);

        string GetWorkspaceFileName(string directory, IWorkspace workspace);
    }
}
