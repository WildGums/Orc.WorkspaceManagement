// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceUpdatedEventArgsFacts.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Test.EventArgs
{
    using Mocks;
    using NUnit.Framework;

    [TestFixture]
    public class WorkspaceUpdatedEventArgsFacts
    {
        [TestCase(null, null, false)]
        [TestCase(null, "B", false)]
        [TestCase("A", null, false)]
        [TestCase("A", "B", false)]
        [TestCase("A", "A", true)]
        public void TheIsRefreshProperty(string workspaceLocation1, string workspaceLocation2, bool expectedResult)
        {
            var workspace1 = !string.IsNullOrEmpty(workspaceLocation1) ? new Workspace(workspaceLocation1) : null;
            var workspace2 = !string.IsNullOrEmpty(workspaceLocation2) ? new Workspace(workspaceLocation2) : null;

            var workspaceUpdatedEventArgs = new WorkspaceUpdatedEventArgs(workspace1, workspace2);

            Assert.AreEqual(expectedResult, workspaceUpdatedEventArgs.IsRefresh);
        }
    }
}