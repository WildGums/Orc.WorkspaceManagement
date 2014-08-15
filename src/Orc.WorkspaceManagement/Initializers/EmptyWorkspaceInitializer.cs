// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceInitializer.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    public class EmptyWorkspaceInitializer : IWorkspaceInitializer
    {
        public string GetInitialLocation()
        {
            return null;
        }
    }
}