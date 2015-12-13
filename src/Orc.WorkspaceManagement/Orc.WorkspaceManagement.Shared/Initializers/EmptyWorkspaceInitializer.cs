// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceInitializer.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
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