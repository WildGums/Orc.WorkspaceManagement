namespace Orc.WorkspaceManagement.Tests;

using Automation;
using FilterBuilder.Tests;
using NUnit.Framework;
using Orc.Automation;

[Explicit]
[TestFixture]
public class WorkspacesViewFacts : StyledControlTestFacts<Views.WorkspacesView>
{
    [Target]
    public WorkspacesView Target { get; set; }

    protected override void InitializeTarget(string id)
    {
        base.InitializeTarget(id);

        var target = Target;

        target.Execute<InitWorkspacesViewMethodRun>();
    }

   // [TestCase(WorkspacesViewTestData.TestScopeWith3CustomRecords)]
    [TestCase(WorkspacesViewTestData.TestScopeWith5CustomRecords)]
    [TestCase(WorkspacesViewTestData.TestScopeWith0CustomRecords)]
    public void CorrectlyInitializeScope(string scope)
    {
        var target = Target;
        var current = target.Current;

        //Initialize scope items
        current.Scope = scope;

        Wait.UntilResponsive();

        var groupItems = target.GroupItems;
        var expectedGroups = WorkspacesViewTestData.GetWorkspacesGroups(scope);

        Assert.That(groupItems, Is.EquivalentTo(expectedGroups)
            .Using<WorkspaceViewGroupItem, WorkspaceGroup>((x, y) => Equals(x.GroupName, y.Title)));

        for (var index = 0; index < expectedGroups.Count; index++)
        {
            var expectedGroup = expectedGroups[index];
            var group = groupItems[index];

            var expectedItems = expectedGroup.Workspaces;
            var items = group.Items;

            Assert.That(items, Is.EquivalentTo(expectedItems)
                .Using<WorkspaceViewItem, IWorkspace>((x, y) => Equals(x.DisplayText, y.Title)));
        }
    }

    [Test]
    public void CorrectlyEditWorkspace()
    {
        var target = Target;
        var current = target.Current;

        //Initialize scope items
        current.Scope = WorkspacesViewTestData.TestScopeWith3CustomRecords;

        Wait.UntilResponsive();

        var workspaceItems = target.GroupItems[0].Items;

        foreach (var workspaceViewItem in workspaceItems)
        {
            var editWindow = workspaceViewItem.Edit();

            var itemText = workspaceViewItem.DisplayText;

            Assert.That(editWindow, Is.Not.Null);
            Assert.That(editWindow.Title, Is.EqualTo(itemText));

            editWindow.Title = itemText + "_test";

            editWindow.Accept();

            Wait.UntilResponsive();
        }

        workspaceItems = target.GroupItems[0].Items;

        Assert.That(workspaceItems, Has.All
            .Property(nameof(WorkspaceViewItem.DisplayText)).EndWith("_test"));
    }

    [Test]
    public void CorrectlyRemoveWorkspace()
    {
        var target = Target;
        var current = target.Current;

        //Initialize scope items
        current.Scope = WorkspacesViewTestData.TestScopeWith3CustomRecords;

        Wait.UntilResponsive();

        var workspaceItems = target.GroupItems[0].Items;
        var deleteItem = workspaceItems[2];

        deleteItem.Delete();

        Wait.UntilResponsive();

        workspaceItems = target.GroupItems[0].Items;

        Assert.That(workspaceItems, Does.Not.Contains(deleteItem)
            .Using<WorkspaceViewItem, WorkspaceViewItem>((x, y) => Equals(x.DisplayText, y.DisplayText)));
    }
}
