// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManagerFacts.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Test.Managers
{
    using Mocks;
    using NUnit.Framework;

    public class WorkspaceManagerFacts
    {
        [TestFixture]
        public class TheLoadMethod
        {
            [TestCase("myLocation")]
            public void UpdatesLocationAfterLoadingWorkspace(string newLocation)
            {
                var workspaceManager = new WorkspaceManager(new EmptyWorkspaceInitializer(), new MemoryWorkspaceReader(), new MemoryWorkspaceWriter());

                Assert.AreEqual(null, workspaceManager.Location);

                workspaceManager.Load(newLocation);

                Assert.AreEqual(newLocation, workspaceManager.Location);
            }

            [TestCase]
            public void RaisesWorkspaceLoadingEvent()
            {
                var workspaceManager = new WorkspaceManager(new EmptyWorkspaceInitializer(), new MemoryWorkspaceReader(), new MemoryWorkspaceWriter());

                var eventRaised = false;
                workspaceManager.WorkspaceLoading += (sender, e) => eventRaised = true;

                workspaceManager.Load("dummyLocation");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public void RaisesWorkspaceLoadedEvent()
            {
                var workspaceManager = new WorkspaceManager(new EmptyWorkspaceInitializer(), new MemoryWorkspaceReader(), new MemoryWorkspaceWriter());

                var eventRaised = false;
                workspaceManager.WorkspaceLoaded += (sender, e) => eventRaised = true;

                workspaceManager.Load("dummyLocation");

                Assert.IsTrue(eventRaised);
            }
        }

        [TestFixture]
        public class TheRefreshMethod
        {
            [TestCase]
            public void DoesNothingWithoutWorkspace()
            {
                var workspaceManager = new WorkspaceManager(new EmptyWorkspaceInitializer(), new MemoryWorkspaceReader(), new MemoryWorkspaceWriter());

                Assert.IsNull(workspaceManager.Workspace);

                workspaceManager.Refresh();

                Assert.IsNull(workspaceManager.Workspace);
            }

            [TestCase]
            public void RaisesWorkspaceUpdatedEvent()
            {
                var workspaceManager = new WorkspaceManager(new EmptyWorkspaceInitializer(), new MemoryWorkspaceReader(), new MemoryWorkspaceWriter());

                workspaceManager.Load("dummyLocation");

                var eventRaised = false;
                workspaceManager.WorkspaceUpdated += (sender, e) => eventRaised = true;

                workspaceManager.Refresh(); 

                Assert.IsTrue(eventRaised);
            }
        }

        [TestFixture]
        public class TheSaveMethod
        {
            [TestCase("myLocation")]
            public void UpdatesLocationAfterSavingWorkspace(string newLocation)
            {
                var workspaceManager = new WorkspaceManager(new EmptyWorkspaceInitializer(), new MemoryWorkspaceReader(), new MemoryWorkspaceWriter());

                workspaceManager.Load("dummyLocation");

                Assert.AreEqual("dummyLocation", workspaceManager.Location);

                workspaceManager.Save(newLocation);

                Assert.AreEqual(newLocation, workspaceManager.Location);
            }

            [TestCase]
            public void RaisesWorkspaceSavingEvent()
            {
                var workspaceManager = new WorkspaceManager(new EmptyWorkspaceInitializer(), new MemoryWorkspaceReader(), new MemoryWorkspaceWriter());

                workspaceManager.Load("dummyLocation");

                var eventRaised = false;
                workspaceManager.WorkspaceSaving += (sender, e) => eventRaised = true;

                workspaceManager.Save();

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public void RaisesWorkspaceLoadedEvent()
            {
                var workspaceManager = new WorkspaceManager(new EmptyWorkspaceInitializer(), new MemoryWorkspaceReader(), new MemoryWorkspaceWriter());

                workspaceManager.Load("dummyLocation");

                var eventRaised = false;
                workspaceManager.WorkspaceSaved += (sender, e) => eventRaised = true;

                workspaceManager.Save();

                Assert.IsTrue(eventRaised);
            }
        }

        [TestFixture]
        public class TheCloseMethod
        {
            [TestCase]
            public void UpdatesWorkspaceAfterClosingWorkspace()
            {
                var workspaceManager = new WorkspaceManager(new EmptyWorkspaceInitializer(), new MemoryWorkspaceReader(), new MemoryWorkspaceWriter());

                workspaceManager.Load("dummyLocation");

                Assert.IsNotNull(workspaceManager.Workspace);

                workspaceManager.Close();

                Assert.IsNull(workspaceManager.Workspace);
            }

            [TestCase]
            public void UpdatesLocationAfterClosingWorkspace()
            {
                var workspaceManager = new WorkspaceManager(new EmptyWorkspaceInitializer(), new MemoryWorkspaceReader(), new MemoryWorkspaceWriter());

                workspaceManager.Load("dummyLocation");

                Assert.AreEqual("dummyLocation", workspaceManager.Location);

                workspaceManager.Close();

                Assert.AreEqual(null, workspaceManager.Location);
            }

            [TestCase]
            public void RaisesWorkspaceClosingEvent()
            {
                var workspaceManager = new WorkspaceManager(new EmptyWorkspaceInitializer(), new MemoryWorkspaceReader(), new MemoryWorkspaceWriter());

                workspaceManager.Load("dummyLocation");

                var eventRaised = false;
                workspaceManager.WorkspaceClosing += (sender, e) => eventRaised = true;

                workspaceManager.Close();

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public void RaisesWorkspaceClosedEvent()
            {
                var workspaceManager = new WorkspaceManager(new EmptyWorkspaceInitializer(), new MemoryWorkspaceReader(), new MemoryWorkspaceWriter());

                workspaceManager.Load("dummyLocation");

                var eventRaised = false;
                workspaceManager.WorkspaceClosed += (sender, e) => eventRaised = true;

                workspaceManager.Close();

                Assert.IsTrue(eventRaised);
            }
        }
    }
}