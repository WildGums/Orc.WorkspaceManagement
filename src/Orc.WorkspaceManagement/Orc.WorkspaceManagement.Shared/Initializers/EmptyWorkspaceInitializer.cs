// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceInitializer.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using Catel.Threading;
    using System.Threading.Tasks;

    public class EmptyWorkspaceInitializer : IWorkspaceInitializer
    {
        public Task InitializeAsync(IWorkspace workspace)
        {
            // nothing
            return TaskHelper.Completed;
        }
    }
}