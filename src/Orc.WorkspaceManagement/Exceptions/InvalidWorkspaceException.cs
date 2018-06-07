// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidWorkspaceException.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
    using Catel;

    public class InvalidWorkspaceException : WorkspaceException
    {
        public InvalidWorkspaceException(IWorkspace workspace)
            : base(workspace, $"Workspace '{ObjectToStringHelper.ToString(workspace)}' is invalid at this stage")
        {
        }

        public InvalidWorkspaceException(IWorkspace workspace, string message)
            : base(workspace, message)
        {
        }

        public InvalidWorkspaceException(IWorkspace workspace, string message, Exception innerException)
            : base(workspace, message, innerException)
        {
        }
    }
}
