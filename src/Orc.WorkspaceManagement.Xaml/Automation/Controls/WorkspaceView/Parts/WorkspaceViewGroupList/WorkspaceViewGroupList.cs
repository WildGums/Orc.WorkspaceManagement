namespace Orc.WorkspaceManagement.Automation;

using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;
using Orc.Automation;
using Orc.Automation.Controls;

[AutomatedControl(ControlTypeName = nameof(ControlType.Pane))]
public class WorkspaceViewGroupList : FrameworkElement
{
    public WorkspaceViewGroupList(AutomationElement element) 
        : base(element)
    {
    }

    public IReadOnlyList<WorkspaceViewGroupItem> GetGroupItems()
    {
        var childElements = Element.GetChildElements()
            .ToList();

        var groupNamesElements = childElements
            .Where(x => Equals(x.Current.ControlType, ControlType.Text))
            .Select(x => x.As<Text>()?.Value)
            .ToList();

        var itemList = childElements
            .Where(x => Equals(x.Current.ControlType, ControlType.List))
            .Select(x => x.As<List>())
            .ToList();

        if (groupNamesElements.Count < itemList.Count)
        {
            //Insert default group name
            groupNamesElements.Insert(0, null);
        }

        var groupItems = new List<WorkspaceViewGroupItem>();

        foreach (var group in groupNamesElements.Zip(itemList))
        {
            var groupName = group.First;
            var groupList = group.Second;

            var items = groupList.GetItemsOfType<WorkspaceViewItem>();
            groupItems.Add(new WorkspaceViewGroupItem
            {
                GroupName = groupName,
                Items = items
            });
        }

        return groupItems;
    }
}
