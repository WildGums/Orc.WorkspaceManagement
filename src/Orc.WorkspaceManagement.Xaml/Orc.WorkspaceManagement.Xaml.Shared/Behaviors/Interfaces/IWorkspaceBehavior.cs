// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceBehavior.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
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