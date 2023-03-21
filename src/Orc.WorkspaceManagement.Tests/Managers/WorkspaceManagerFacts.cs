namespace Orc.WorkspaceManagement.Test.Managers;

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
        public async Task AddsTheWorkspaceAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

            var workspace = new Workspace(WorkspaceNameHelper.GetRandomWorkspaceName());

            await workspaceManager.AddAsync(workspace);

            Assert.IsTrue(workspaceManager.Workspaces.Contains(workspace));
        }

        [TestCase]
        public async Task RaisesWorkspaceAddedEventAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

            var eventRaised = false;
            workspaceManager.WorkspaceAdded += (sender, e) => eventRaised = true;

            await workspaceManager.AddAsync(new Workspace(WorkspaceNameHelper.GetRandomWorkspaceName()));

            Assert.IsTrue(eventRaised);
        }

        [TestCase]
        public async Task RaisesWorkspacesChangedEventAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

            var eventRaised = false;
            workspaceManager.WorkspacesChanged += (sender, e) => eventRaised = true;

            await workspaceManager.AddAsync(new Workspace(WorkspaceNameHelper.GetRandomWorkspaceName()));

            Assert.IsTrue(eventRaised);
        }
    }

    [TestFixture]
    public class TheRemoveMethod
    {
        [TestCase]
        public async Task RemovesTheWorkspaceAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

            var workspace = new Workspace(WorkspaceNameHelper.GetRandomWorkspaceName());

            await workspaceManager.AddAsync(workspace);

            Assert.IsTrue(workspaceManager.Workspaces.Contains(workspace));

            await workspaceManager.RemoveAsync(workspace);

            Assert.IsFalse(workspaceManager.Workspaces.Contains(workspace));
        }

        [TestCase]
        public async Task DoesNotRemoveWorkspaceWithCanDeleteIsFalseAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

            var workspace = new Workspace(WorkspaceNameHelper.GetRandomWorkspaceName());
            workspace.CanDelete = false;

            await workspaceManager.AddAsync(workspace);

            Assert.IsTrue(workspaceManager.Workspaces.Contains(workspace));

            await workspaceManager.RemoveAsync(workspace);

            Assert.IsTrue(workspaceManager.Workspaces.Contains(workspace));
        }

        [TestCase]
        public async Task RaisesWorkspaceRemovedEventAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

            var workspace = new Workspace(WorkspaceNameHelper.GetRandomWorkspaceName());

            await workspaceManager.AddAsync(workspace);

            var eventRaised = false;
            workspaceManager.WorkspacesChanged += (sender, e) => eventRaised = true;

            await workspaceManager.RemoveAsync(workspace);

            Assert.IsTrue(eventRaised);
        }

        [TestCase]
        public async Task RaisesWorkspacesChangedEventAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

            var workspace = new Workspace(WorkspaceNameHelper.GetRandomWorkspaceName());

            await workspaceManager.AddAsync(workspace);

            var eventRaised = false;
            workspaceManager.WorkspacesChanged += (sender, e) => eventRaised = true;

            await workspaceManager.RemoveAsync(workspace);

            Assert.IsTrue(eventRaised);
        }
    }

    [TestFixture]
    public class TheInitializeMethod
    {
        [TestCase]
        public async Task RaisesInitializingEventAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

            var eventRaised = false;
            workspaceManager.Initializing += (sender, e) => eventRaised = true;

            await workspaceManager.InitializeAsync();

            Assert.IsTrue(eventRaised);
        }

        [TestCase]
        public async Task RaisesInitializedEventAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

            var eventRaised = false;
            workspaceManager.Initialized += (sender, e) => eventRaised = true;

            await workspaceManager.InitializeAsync();

            Assert.IsTrue(eventRaised);
        }
    }

    [TestFixture]
    public class TheStoreMethod
    {
        [TestCase]
        public async Task PreventsSaveForReadonlyWorkspacesAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

            var workspace = new Workspace(WorkspaceNameHelper.GetRandomWorkspaceName());
            workspace.CanEdit = false;

            await workspaceManager.AddAsync(workspace);
            await workspaceManager.SetWorkspaceAsync(workspace);

            var eventRaised = false;
            workspaceManager.WorkspaceInfoRequested += (sender, e) => eventRaised = true;

            await workspaceManager.StoreWorkspaceAsync();

            Assert.IsFalse(eventRaised);
        }
    }

    [TestFixture]
    public class TheSaveMethod
    {
        [TestCase]
        public async Task RaisesSavingAsyncEventAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

            await workspaceManager.AddAsync(new Workspace("test"));
            await workspaceManager.SetWorkspaceAsync(workspaceManager.Workspaces.First());

            var eventRaised = false;
            workspaceManager.WorkspaceSavingAsync += async (sender, e) => eventRaised = true;

            await workspaceManager.SaveAsync();

            Assert.IsTrue(eventRaised);
        }

        [TestCase]
        public async Task RaisesSavedEventAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

            await workspaceManager.AddAsync(new Workspace("test"));
            await workspaceManager.SetWorkspaceAsync(workspaceManager.Workspaces.First());

            var eventRaised = false;
            workspaceManager.WorkspaceSaved += (sender, e) => eventRaised = true;

            await workspaceManager.SaveAsync();

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
        public async Task CallsProviderWhenStoringWorkspaceAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();
            var workspace = new Workspace(WorkspaceNameHelper.GetRandomWorkspaceName());

            await workspaceManager.AddAsync(workspace, true);

            var workspaceProvider = new WorkspaceProvider("key1", "value1");
            workspaceManager.AddProvider(workspaceProvider);

            await workspaceManager.StoreWorkspaceAsync();

            Assert.AreEqual("value1", workspace.GetWorkspaceValue("key1", "unexpected"));
        }
    }

    [TestFixture]
    public class TheRemoveProviderMethod
    {
        [TestCase]
        public async Task CorrectlyRemovesProviderWhenStoringWorkspaceAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();
            var workspace = new Workspace(WorkspaceNameHelper.GetRandomWorkspaceName());

            await workspaceManager.AddAsync(workspace, true);

            var workspaceProvider = new WorkspaceProvider("key1", "value1");
            workspaceManager.AddProvider(workspaceProvider);
            workspaceManager.RemoveProvider(workspaceProvider);

            await workspaceManager.StoreWorkspaceAsync();

            Assert.AreEqual("expected", workspace.GetWorkspaceValue("key1", "expected"));
        }
    }
}
