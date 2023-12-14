namespace Orc.WorkspaceManagement.Automation;

using System;
using System.Linq;

public static class WorkspacesViewExtensions
{
    public static void SelectWorkspace(this WorkspacesView workspacesView, string name, string? groupName = null)
    {
        ArgumentNullException.ThrowIfNull(workspacesView);

        var item = GetItem(workspacesView, name, groupName);

        item?.Select();
    }

    public static void DeleteItem(this WorkspacesView workspacesView, string name, string? groupName = null)
    {
        ArgumentNullException.ThrowIfNull(workspacesView);

        var item = GetItem(workspacesView, name, groupName);

        item?.Delete();
    }

    public static WorkspaceWindow? EditItem(this WorkspacesView workspacesView, string name, string? groupName = null)
    {
        ArgumentNullException.ThrowIfNull(workspacesView);

        var item = GetItem(workspacesView, name, groupName);

        return item?.Edit();
    }

    public static WorkspaceViewItem? GetItem(this WorkspacesView workspacesView, string name, string? groupName = null)
    {
        ArgumentNullException.ThrowIfNull(workspacesView);

        if (string.IsNullOrEmpty(groupName))
        {
            groupName = null;
        }

        var group = workspacesView?.GroupItems?.FirstOrDefault(x => Equals(x.GroupName, groupName));
        return group?.Items?.FirstOrDefault(x => Equals(x.DisplayText, name));
    }
}