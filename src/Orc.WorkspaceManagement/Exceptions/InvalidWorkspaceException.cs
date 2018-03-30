// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidWorkspaceException.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using Catel;

    public class InvalidWorkspaceException : WorkspaceException
    {
        public InvalidWorkspaceException(IWorkspace workspace)
            : base(workspace, string.Format("Workspace '{0}' is invalid at this stage",ObjectToStringHelper.ToString(workspace)))
        {
        }
    }
}