namespace Orc.WorkspaceManagement.Automation;

using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;
using Orc.Automation;
using Orc.Automation.Controls;

[Control(ControlTypeName = nameof(ControlType.Pane))]
public class WorkspaceViewGroupList(AutomationElement element)
    : FrameworkElement(element)
{
    public IReadOnlyList<WorkspaceViewGroupItem> GetGroupItems()
    {
        var dataItems = By.Many<DataItem>();

        var groupNamesElements = dataItems
            .Select(x => x.By.One<Text>()?.Value)
            .ToList();

        var itemList = dataItems
            .Select(x => x.By.One<ListBox>())
            .Where(x => x is not null)
            .ToArray();

        if (groupNamesElements.Count < itemList.Length)
        {
            //Insert default group name
            groupNamesElements.Insert(0, null);
        }

        var groupItems = new List<WorkspaceViewGroupItem>();
        foreach (var (groupName, groupList) in groupNamesElements.Zip(itemList))
        {
            if (groupList is null)
            {
                continue;
            }

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
