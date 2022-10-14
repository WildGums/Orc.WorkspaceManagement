namespace Orc.WorkspaceManagement
{
    using System;

    public static class WorkspaceExtensions
    {
        public static void SynchronizeWithWorkspace(this IWorkspace workspace, IWorkspace newWorkspaceData)
        {
            ArgumentNullException.ThrowIfNull(workspace);
            ArgumentNullException.ThrowIfNull(newWorkspaceData);

            workspace.ClearWorkspaceValues();

            foreach (var workspaceValueName in newWorkspaceData.GetAllWorkspaceValueNames())
            {
                var workspaceValue = newWorkspaceData.GetWorkspaceValue<object?>(workspaceValueName, null);
                workspace.SetWorkspaceValue(workspaceValueName, workspaceValue);
            }
        }
    }
}
