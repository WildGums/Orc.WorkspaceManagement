// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceInitializer.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.Threading.Tasks;

    public interface IWorkspaceInitializer
    {
        [ObsoleteEx(ReplacementTypeOrMember = "InitializeAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        void Initialize(IWorkspace workspace);

        Task InitializeAsync(IWorkspace workspace);
    }
}