// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceInitializer.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Example.WorkspaceManagement
{
    public class WorkspaceInitializer : IWorkspaceInitializer
    {
        #region IWorkspaceInitializer Members
        public void Initialize(IWorkspace workspace)
        {
            workspace.SetWorkspaceValue("AView.Width", 200d);
            workspace.SetWorkspaceValue("BView.Width", 200d);
        }
        #endregion
    }
}