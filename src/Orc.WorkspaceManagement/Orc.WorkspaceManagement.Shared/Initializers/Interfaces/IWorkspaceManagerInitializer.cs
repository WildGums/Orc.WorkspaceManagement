// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceManagerInitializer.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.Threading.Tasks;

    public interface IWorkspaceManagerInitializer
    {
        Task InitializeAsync(IWorkspaceManager workspaceManager);
    }
}