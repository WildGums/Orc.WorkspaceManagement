// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManagerExtensionsFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Test
{
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

                Assert.AreEqual(someDirectoryName, workspaceManager.BaseDirectory);
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

                Assert.AreEqual(workspacesCount + 1, workspaceManager.Workspaces.Count());
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
                Assert.AreEqual(titles.Count(), intersect.Count());
            }

            [Test]
            public async Task SetWorkspaceToNullWhenDirectoryIsEmptyAsync()
            {
                var mock = new Mock<IWorkspacesStorageService>();

                const string emptyDirectory = "Empty directory";

                mock.Setup(x => x.LoadWorkspacesAsync(emptyDirectory)).ReturnsAsync(Enumerable.Empty<IWorkspace>());

                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer(mock.Object);

                await workspaceManager.SetWorkspaceSchemesDirectoryAsync(emptyDirectory, false, false);

                Assert.AreEqual(null, workspaceManager.Workspace);
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

                Assert.AreEqual(defaultWorkspace, workspaceManager.Workspace.Title);
            }
        }
    }
}
