// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManagementInitializationException.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;

    public class WorkspaceManagementInitializationException : Exception
    {
        public WorkspaceManagementInitializationException(IWorkspaceManager workspaceManager)
            : base("Unable to initialize WorkspaceManager. Probably initialization was canceled.")
        {
            WorkspaceManager = workspaceManager;
        }

        public IWorkspaceManager WorkspaceManager { get; private set; }
    }
}