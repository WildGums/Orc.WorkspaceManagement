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

            Assert.That(workspaceManager.Workspaces.Contains(workspace), Is.True);
        }

        [TestCase]
        public async Task RaisesWorkspaceAddedEventAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

            var eventRaised = false;
            workspaceManager.WorkspaceAdded += (sender, e) => eventRaised = true;

            await workspaceManager.AddAsync(new Workspace(WorkspaceNameHelper.GetRandomWorkspaceName()));

            Assert.That(eventRaised, Is.True);
        }

        [TestCase]
        public async Task RaisesWorkspacesChangedEventAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

            var eventRaised = false;
            workspaceManager.WorkspacesChanged += (sender, e) => eventRaised = true;

            await workspaceManager.AddAsync(new Workspace(WorkspaceNameHelper.GetRandomWorkspaceName()));

            Assert.That(eventRaised, Is.True);
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

            Assert.That(workspaceManager.Workspaces.Contains(workspace), Is.True);

            await workspaceManager.RemoveAsync(workspace);

            Assert.That(workspaceManager.Workspaces.Contains(workspace), Is.False);
        }

        [TestCase]
        public async Task DoesNotRemoveWorkspaceWithCanDeleteIsFalseAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

            var workspace = new Workspace(WorkspaceNameHelper.GetRandomWorkspaceName());
            workspace.CanDelete = false;

            await workspaceManager.AddAsync(workspace);

            Assert.That(workspaceManager.Workspaces.Contains(workspace), Is.True);

            await workspaceManager.RemoveAsync(workspace);

            Assert.That(workspaceManager.Workspaces.Contains(workspace), Is.True);
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

            Assert.That(eventRaised, Is.True);
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

            Assert.That(eventRaised, Is.True);
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

            Assert.That(eventRaised, Is.True);
        }

        [TestCase]
        public async Task RaisesInitializedEventAsync()
        {
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

            var eventRaised = false;
            workspaceManager.Initialized += (sender, e) => eventRaised = true;

            await workspaceManager.InitializeAsync();

            Assert.That(eventRaised, Is.True);
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

            Assert.That(eventRaised, Is.False);
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

            Assert.That(eventRaised, Is.True);
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

            Assert.That(eventRaised, Is.True);
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

            Assert.That(workspace.GetWorkspaceValue("key1", "unexpected"), Is.EqualTo("value1"));
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

            Assert.That(workspace.GetWorkspaceValue("key1", "expected"), Is.EqualTo("expected"));
        }
    }
}
