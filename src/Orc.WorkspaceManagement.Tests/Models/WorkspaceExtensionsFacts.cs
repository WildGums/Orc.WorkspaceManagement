namespace Orc.WorkspaceManagement.Tests.Models;

using NUnit.Framework;
using Test;

[TestFixture]
public class WorkspaceExtensionsFacts
{
    [Test]
    public void SynchronizesWorkspaces()
    {
        var workspaceA = new Workspace(WorkspaceNameHelper.GetRandomWorkspaceName());
        workspaceA.SetWorkspaceValue("A", 1);
        workspaceA.SetWorkspaceValue("B", 2);
        workspaceA.SetWorkspaceValue("C", 3);

        var workspaceB = new Workspace(WorkspaceNameHelper.GetRandomWorkspaceName());
        workspaceB.SetWorkspaceValue("D", 4);
        workspaceB.SetWorkspaceValue("E", 5);
        workspaceB.SetWorkspaceValue("F", 6);

        workspaceA.SynchronizeWithWorkspace(workspaceB);

        Assert.That(workspaceA.GetWorkspaceValue("A", 0), Is.EqualTo(0));
        Assert.That(workspaceA.GetWorkspaceValue("B", 0), Is.EqualTo(0));
        Assert.That(workspaceA.GetWorkspaceValue("C", 0), Is.EqualTo(0));
        Assert.That(workspaceA.GetWorkspaceValue("D", 0), Is.EqualTo(4));
        Assert.That(workspaceA.GetWorkspaceValue("E", 0), Is.EqualTo(5));
        Assert.That(workspaceA.GetWorkspaceValue("F", 0), Is.EqualTo(6));
    }
}
