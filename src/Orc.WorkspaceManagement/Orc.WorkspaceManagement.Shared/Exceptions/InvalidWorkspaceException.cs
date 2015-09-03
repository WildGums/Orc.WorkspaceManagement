// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidWorkspaceException.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
    using Catel;

    public class InvalidWorkspaceException : Exception
    {
        public InvalidWorkspaceException(IWorkspace workspace)
            : base(string.Format("Workspace '{0}' is invalid at this stage", ObjectToStringHelper.ToString(workspace)))
        {
            Workspace = workspace;
        }

        public IWorkspace Workspace { get; private set; }
    }
}