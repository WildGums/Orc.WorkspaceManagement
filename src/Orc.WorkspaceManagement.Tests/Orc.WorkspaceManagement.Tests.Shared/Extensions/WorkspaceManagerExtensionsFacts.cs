// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManagerExtensionsFacts.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2015 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Test.Extensions
{
    using System;
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
            public async void BaseDirectoryChanged()
            {
                const string someDirectoryName = "Some directory";

                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

                await workspaceManager.SetWorkspaceSchemesDirectoryAsync(someDirectoryName);

                Assert.AreEqual(someDirectoryName, workspaceManager.BaseDirectory);
            }

            [TestCase(1)]
            [TestCase(4)]
            public async void ClearWorkspacesEachTime(int timesToRepeat)
            {
                var mock = new Mock<IWorkspacesStorageService>();

                const int workspacesCount = 3;

                mock.Setup(x => x.LoadWorkspaces(It.IsAny<string>())).Returns(Enumerable.Repeat(new Workspace(), workspacesCount));

                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer(mock.Object);

                for (var i = 0; i < timesToRepeat; i++)
                {
                    await workspaceManager.SetWorkspaceSchemesDirectoryAsync("Some directory");
                }

                Assert.AreEqual(workspacesCount + 1, workspaceManager.Workspaces.Count());
            }

            [TestCase("1", "2", "3")]
            [TestCase("1", "2")]
            public async void LoadsCorrectWorkspaces(params string[] titles)
            {
                var mock = new Mock<IWorkspacesStorageService>();

                mock.Setup(x => x.LoadWorkspaces(It.IsAny<string>())).Returns(titles.Select(t => new Workspace { Title = t }));

                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer(mock.Object);

                await workspaceManager.SetWorkspaceSchemesDirectoryAsync("Some directory");

                var titles2 = workspaceManager.Workspaces.Select(w => w.Title).ToArray();
                var intersect = titles.Intersect(titles2);
                Assert.AreEqual(titles.Count(), intersect.Count());
            }

            [Test]
            public async void SetWorkspaceToNullWhenDirectoryIsEmpty()
            {
                var mock = new Mock<IWorkspacesStorageService>();

                const string emptyDirectory = "Empty directory";

                mock.Setup(x => x.LoadWorkspaces(emptyDirectory)).Returns(Enumerable.Empty<IWorkspace>());

                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer(mock.Object);

                await workspaceManager.SetWorkspaceSchemesDirectoryAsync(emptyDirectory, false, false);

                Assert.AreEqual(null, workspaceManager.Workspace);
            }

            [Test]
            public async void SetWorkspaceToDefaultWhenDirectoryIsNotEmpty()
            {
                var mock = new Mock<IWorkspacesStorageService>();

                const string defaultWorkspace = "Default name";
                const string notEmptyDirectory = "Not empty directory";

                mock.Setup(x => x.LoadWorkspaces(notEmptyDirectory)).Returns(new[] { new Workspace { Title = "Not default 1" }, new Workspace { Title = defaultWorkspace }, new Workspace { Title = "Not default 2" } });
                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer(mock.Object);

                await workspaceManager.SetWorkspaceSchemesDirectoryAsync(notEmptyDirectory, defaultWorkspaceName: defaultWorkspace);

                Assert.AreEqual(defaultWorkspace, workspaceManager.Workspace.Title);
            }
        }
    }
}