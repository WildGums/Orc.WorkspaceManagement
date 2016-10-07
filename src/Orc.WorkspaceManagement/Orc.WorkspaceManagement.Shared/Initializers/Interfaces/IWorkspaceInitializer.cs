// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceInitializer.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
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