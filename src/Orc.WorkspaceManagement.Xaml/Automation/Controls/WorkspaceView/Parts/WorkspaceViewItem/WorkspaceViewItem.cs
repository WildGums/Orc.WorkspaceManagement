﻿namespace Orc.WorkspaceManagement.Automation;

using System.Windows.Automation;
using Orc.Automation;
using Orc.Automation.Controls;

public class WorkspaceViewItem(AutomationElement element) 
    : ListItem(element)
{
    private WorkspaceViewItemMap Map => Map<WorkspaceViewItemMap>();

    public bool IsActive => Map.CurrentWorkspaceTextBlock is not null;

    public override void Select()
    {
        Element.MouseClick();
    }

    public bool CanRefresh()
    {
        return Map.RefreshWorkspaceButton?.IsVisible() ?? false;
    }

    public bool CanEdit()
    {
        return Map.EditWorkspaceButton?.IsVisible() ?? false;
    }

    public void Refresh()
    {
        if (CanRefresh())
        {
            Map.RefreshWorkspaceButton?.Click();
        }
    }

    public WorkspaceWindow? Edit()
    {
        if (!CanEdit())
        {
            return null;
        }

        Map.EditWorkspaceButton?.Click();

        Wait.UntilResponsive();

        var editWorkspaceWindow = Window.WaitForWindow<WorkspaceWindow>();

        return editWorkspaceWindow;
    }

    public bool CanDelete()
    {
        return Map.RemoveWorkspaceButton?.IsVisible() ?? false;
    }

    public void Delete()
    {
        if (!CanDelete())
        {
            return;
        }
            
        var hostWindow = Element.GetHostWindow();
        hostWindow?.SetFocus();

        Map.RemoveWorkspaceButton?.Click();

        Wait.UntilResponsive();

        var messageBox = hostWindow?.Find<MessageBox>();
        messageBox?.Yes();

        Wait.UntilResponsive();

        hostWindow?.SetFocus();
    }
}
