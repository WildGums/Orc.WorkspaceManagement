// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManagerFacts.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Test.Managers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Mocks;
    using Moq;
    using NUnit.Framework;

    public class WorkspaceManagerFacts
    {
        [TestFixture]
        public class TheAddMethod
        {
            [TestCase]
            public void AddsTheWorkspace()
            {
                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

                var workspace = new Workspace
                {
                    Title = WorkspaceNameHelper.GetRandomWorkspaceName()
                };

                workspaceManager.Add(workspace);

                Assert.IsTrue(workspaceManager.Workspaces.Contains(workspace));
            }

            [TestCase]
            public void RaisesWorkspaceAddedEvent()
            {
                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

                var eventRaised = false;
                workspaceManager.WorkspaceAdded += (sender, e) => eventRaised = true;

                workspaceManager.Add(new Workspace());

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public void RaisesWorkspacesChangedEvent()
            {
                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

                var eventRaised = false;
                workspaceManager.WorkspacesChanged += (sender, e) => eventRaised = true;

                workspaceManager.Add(new Workspace());

                Assert.IsTrue(eventRaised);
            }
        }

        [TestFixture]
        public class TheRemoveMethod
        {
            [TestCase]
            public void RemovesTheWorkspace()
            {
                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

                var workspace = new Workspace
                {
                    Title = WorkspaceNameHelper.GetRandomWorkspaceName()
                };

                workspaceManager.Add(workspace);

                Assert.IsTrue(workspaceManager.Workspaces.Contains(workspace));

                workspaceManager.Remove(workspace);

                Assert.IsFalse(workspaceManager.Workspaces.Contains(workspace));
            }

            [TestCase]
            public void DoesNotRemoveWorkspaceWithCanDeleteIsFalse()
            {
                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

                var workspace = new Workspace
                {
                    Title = WorkspaceNameHelper.GetRandomWorkspaceName(),
                    CanDelete = false
                };

                workspaceManager.Add(workspace);

                Assert.IsTrue(workspaceManager.Workspaces.Contains(workspace));

                workspaceManager.Remove(workspace);

                Assert.IsTrue(workspaceManager.Workspaces.Contains(workspace));
            }

            [TestCase]
            public void RaisesWorkspaceRemovedEvent()
            {
                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

                var workspace = new Workspace
                {
                    Title = WorkspaceNameHelper.GetRandomWorkspaceName()
                };

                workspaceManager.Add(workspace);

                var eventRaised = false;
                workspaceManager.WorkspacesChanged += (sender, e) => eventRaised = true;

                workspaceManager.Remove(workspace);

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public void RaisesWorkspacesChangedEvent()
            {
                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

                var workspace = new Workspace
                {
                    Title = WorkspaceNameHelper.GetRandomWorkspaceName()
                };

                workspaceManager.Add(workspace);

                var eventRaised = false;
                workspaceManager.WorkspacesChanged += (sender, e) => eventRaised = true;

                workspaceManager.Remove(workspace);

                Assert.IsTrue(eventRaised);
            }
        }

        [TestFixture]
        public class TheInitializeMethod
        {
            [TestCase]
            public async Task RaisesInitializingEvent()
            {
                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

                var eventRaised = false;
                workspaceManager.Initializing += (sender, e) => eventRaised = true;

                await workspaceManager.Initialize();

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesInitializedEvent()
            {
                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

                var eventRaised = false;
                workspaceManager.Initialized += (sender, e) => eventRaised = true;

                await workspaceManager.Initialize();

                Assert.IsTrue(eventRaised);
            }
        }

        [TestFixture]
        public class TheStoreMethod
        {
            [TestCase]
            public void PreventsSaveForReadonlyWorkspaces()
            {
                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

                var workspace = new Workspace
                {
                    Title = WorkspaceNameHelper.GetRandomWorkspaceName(),
                    CanEdit = false
                };

                workspaceManager.Add(workspace);
                workspaceManager.Workspace = workspace;

                var eventRaised = false;
                workspaceManager.WorkspaceInfoRequested += (sender, e) => eventRaised = true;

                workspaceManager.StoreWorkspace();

                Assert.IsFalse(eventRaised);
            }
        }

        [TestFixture]
        public class TheSaveMethod
        {
            [TestCase]
            public async Task RaisesSavingEvent()
            {
                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

                var eventRaised = false;
                workspaceManager.Saving += (sender, e) => eventRaised = true;

                await workspaceManager.Save();

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesSavedEvent()
            {
                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

                var eventRaised = false;
                workspaceManager.Saved += (sender, e) => eventRaised = true;

                await workspaceManager.Save();

                Assert.IsTrue(eventRaised);
            }
        }

        [TestFixture]
        public class ThePersistenceLogic
        {
            // TODO : write unit tests
        }

        [TestFixture]
        public class TheAddProviderMethod
        {
            [TestCase]
            public void CallsProviderWhenStoringWorkspace()
            {
                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();
                var workspace = new Workspace
                {
                    Title = WorkspaceNameHelper.GetRandomWorkspaceName()
                };

                workspaceManager.Add(workspace, true);

                var workspaceProvider = new WorkspaceProvider("key1", "value1");
                workspaceManager.AddProvider(workspaceProvider);

                workspaceManager.StoreWorkspace();

                Assert.AreEqual("value1", workspace.GetWorkspaceValue("key1", "unexpected"));
            }
        }

        [TestFixture]
        public class TheRemoveProviderMethod
        {
            [TestCase]
            public void CorrectlyRemovesProviderWhenStoringWorkspace()
            {
                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();
                var workspace = new Workspace
                {
                    Title = WorkspaceNameHelper.GetRandomWorkspaceName()
                };

                workspaceManager.Add(workspace, true);

                var workspaceProvider = new WorkspaceProvider("key1", "value1");
                workspaceManager.AddProvider(workspaceProvider);
                workspaceManager.RemoveProvider(workspaceProvider);

                workspaceManager.StoreWorkspace();

                Assert.AreEqual("expected", workspace.GetWorkspaceValue("key1", "expected"));
            }
        }
    }
}