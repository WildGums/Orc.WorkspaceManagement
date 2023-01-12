﻿namespace Orc.WorkspaceManagement.Test.EventArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class WorkspaceUpdatedEventArgsFacts
    {
        [TestCase(null, null, false)]
        [TestCase(null, "B", false)]
        [TestCase("A", null, false)]
        [TestCase("A", "B", false)]
        [TestCase("A", "A", true)]
        public void TheIsRefreshProperty(string title1, string title2, bool expectedResult)
        {
            var workspace1 = !string.IsNullOrEmpty(title1) ? new Workspace(title1) : null;
            var workspace2 = !string.IsNullOrEmpty(title2) ? new Workspace(title2) : null;

            var workspaceUpdatedEventArgs = new WorkspaceUpdatedEventArgs(workspace1, workspace2);

            Assert.AreEqual(expectedResult, workspaceUpdatedEventArgs.IsRefresh);
        }
    }
}
