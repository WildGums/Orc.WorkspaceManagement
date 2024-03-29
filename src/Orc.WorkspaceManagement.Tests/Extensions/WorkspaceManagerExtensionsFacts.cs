﻿namespace Orc.WorkspaceManagement.Test;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

public class WorkspaceManagerExtensionsFacts
{
    [TestFixture]
    public class SetWorkspaceSchemesDirectoryMethod
    {
        [Test]
        public async Task BaseDirectoryChangedAsync()
        {
            const string someDirectoryName = "Some directory";

            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

            await workspaceManager.SetWorkspaceSchemesDirectoryAsync(someDirectoryName);

            Assert.That(workspaceManager.BaseDirectory, Is.EqualTo(someDirectoryName));
        }

        [TestCase(1)]
        [TestCase(4)]
        public async Task ClearWorkspacesEachTimeAsync(int timesToRepeat)
        {
            var mock = new Mock<IWorkspacesStorageService>();

            const int workspacesCount = 3;

            var workspaces = new List<Workspace>();
            for (int i = 0; i < workspacesCount; i++)
            {
                workspaces.Add(new Workspace(WorkspaceNameHelper.GetRandomWorkspaceName()));
            }

            mock.Setup(x => x.LoadWorkspacesAsync(It.IsAny<string>())).ReturnsAsync(workspaces);

            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer(mock.Object);

            for (var i = 0; i < timesToRepeat; i++)
            {
                await workspaceManager.SetWorkspaceSchemesDirectoryAsync("Some directory");
            }

            Assert.That(workspaceManager.Workspaces.Count(), Is.EqualTo(workspacesCount + 1));
        }

        [TestCase("1", "2", "3")]
        [TestCase("1", "2")]
        public async Task LoadsCorrectWorkspacesAsync(params string[] titles)
        {
            var mock = new Mock<IWorkspacesStorageService>();

            mock.Setup(x => x.LoadWorkspacesAsync(It.IsAny<string>())).ReturnsAsync(titles.Select(t => new Workspace(t)));

            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer(mock.Object);

            await workspaceManager.SetWorkspaceSchemesDirectoryAsync("Some directory");

            var titles2 = workspaceManager.Workspaces.Select(w => w.Title).ToArray();
            var intersect = titles.Intersect(titles2);
            Assert.That(intersect.Count(), Is.EqualTo(titles.Count()));
        }

        [Test]
        public async Task SetWorkspaceToNullWhenDirectoryIsEmptyAsync()
        {
            var mock = new Mock<IWorkspacesStorageService>();

            const string emptyDirectory = "Empty directory";

            mock.Setup(x => x.LoadWorkspacesAsync(emptyDirectory)).ReturnsAsync(Enumerable.Empty<IWorkspace>());

            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer(mock.Object);

            await workspaceManager.SetWorkspaceSchemesDirectoryAsync(emptyDirectory, false, false);

            Assert.That(workspaceManager.Workspace, Is.EqualTo(null));
        }

        [Test]
        public async Task SetWorkspaceToDefaultWhenDirectoryIsNotEmptyAsync()
        {
            var mock = new Mock<IWorkspacesStorageService>();

            const string defaultWorkspace = "Default name";
            const string notEmptyDirectory = "Not empty directory";

            mock.Setup(x => x.LoadWorkspacesAsync(notEmptyDirectory)).ReturnsAsync(new[] { new Workspace("Not default 1"), new Workspace(defaultWorkspace), new Workspace("Not default 2") });
            var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer(mock.Object);

            await workspaceManager.SetWorkspaceSchemesDirectoryAsync(notEmptyDirectory, defaultWorkspaceName: defaultWorkspace);

            Assert.That(workspaceManager.Workspace.Title, Is.EqualTo(defaultWorkspace));
        }
    }
}
