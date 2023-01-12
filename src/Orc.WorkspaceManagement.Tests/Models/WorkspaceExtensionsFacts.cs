namespace Orc.WorkspaceManagement.Tests.Models
{
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

            Assert.AreEqual(0, workspaceA.GetWorkspaceValue("A", 0));
            Assert.AreEqual(0, workspaceA.GetWorkspaceValue("B", 0));
            Assert.AreEqual(0, workspaceA.GetWorkspaceValue("C", 0));
            Assert.AreEqual(4, workspaceA.GetWorkspaceValue("D", 0));
            Assert.AreEqual(5, workspaceA.GetWorkspaceValue("E", 0));
            Assert.AreEqual(6, workspaceA.GetWorkspaceValue("F", 0));
        }
    }
}
