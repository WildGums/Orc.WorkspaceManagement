// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceBehavior.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Behaviors
{
    public interface IWorkspaceBehavior
    {
        void Load(IWorkspace workspace);
        void Save(IWorkspace workspace);
    }
}