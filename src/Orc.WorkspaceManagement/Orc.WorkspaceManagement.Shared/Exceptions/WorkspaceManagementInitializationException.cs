// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManagementInitializationException.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
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