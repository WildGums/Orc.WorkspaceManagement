// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using Catel;

    public static class WorkspaceExtensions
    {
        public static void SynchronizeWithWorkspace(this IWorkspace workspace, IWorkspace newWorkspaceData)
        {
            Argument.IsNotNull(() => workspace);
            Argument.IsNotNull(() => newWorkspaceData);

            workspace.ClearWorkspaceValues();

            foreach (var workspaceValueName in newWorkspaceData.GetAllWorkspaceValueNames())
            {
                var workspaceValue = newWorkspaceData.GetWorkspaceValue<object>(workspaceValueName, null);
                workspace.SetWorkspaceValue(workspaceValueName, workspaceValue);
            }
        }
    }
}