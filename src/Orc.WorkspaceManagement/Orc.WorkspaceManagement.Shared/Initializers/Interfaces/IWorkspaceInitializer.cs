// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceInitializer.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    public interface IWorkspaceInitializer
    {
        void Initialize(IWorkspace workspace);
    }
}