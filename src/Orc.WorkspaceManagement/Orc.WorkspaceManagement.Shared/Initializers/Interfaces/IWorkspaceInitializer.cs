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
        Task InitializeAsync(IWorkspace workspace);
    }
}