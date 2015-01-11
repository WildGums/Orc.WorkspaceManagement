// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManagerExtensionsFacts.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2015 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Test.Extensions
{
    using System.Linq;
    using Helpers;
    using Moq;
    using NUnit.Framework;
    using Services;

    public class WorkspaceManagerExtensionsFacts
    {
        [TestFixture]
        public class SetWorkspaceSchemesDirectoryMethod
        {
            [TestCase]
            public void BaseDirectoryChanged()
            {
                const string someDirectoryName = "Some directory";

                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer();

                workspaceManager.SetWorkspaceSchemesDirectory(someDirectoryName).Wait();

                Assert.AreEqual(someDirectoryName, workspaceManager.BaseDirectory);
            }

            [TestCase(1)]
            [TestCase(4)]
            public void ClearWorkspacesEachTime(int timesToRepeat)
            {
                var mock = new Mock<IWorkspacesStorageService>();

                const int workspacesCount = 3;

                mock.Setup(x => x.LoadWorkspaces(It.IsAny<string>())).Returns(Enumerable.Repeat(new Workspace(), workspacesCount));

                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer(mock.Object);

                for (var i = 0; i < timesToRepeat; i++)
                {
                    // using async/await doesn't work
                    workspaceManager.SetWorkspaceSchemesDirectory("Some directory").Wait();
                }

                Assert.AreEqual(workspacesCount, workspaceManager.Workspaces.Count());
            }

            [TestCase("1", "2", "3")]
            [TestCase("1", "2")]
            public void LoadsCorrectWorkspaces(params string[] titles)
            {
                var mock = new Mock<IWorkspacesStorageService>();

                mock.Setup(x => x.LoadWorkspaces(It.IsAny<string>())).Returns(titles.Select(t => new Workspace {Title = t}));

                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer(mock.Object);

                workspaceManager.SetWorkspaceSchemesDirectory("Some directory").Wait();

                var titles2 = workspaceManager.Workspaces.Select(w => w.Title).ToArray();
                var intersect = titles.Intersect(titles2);
                Assert.AreEqual(titles.Count(), intersect.Count());
            }

            [TestCase]
            public void SetWorkspaceToNullWhenDirectoryIsEmpty()
            {
                var mock = new Mock<IWorkspacesStorageService>();

                const string emptyDirectory = "Empty directory";

                mock.Setup(x => x.LoadWorkspaces(emptyDirectory)).Returns(Enumerable.Empty<IWorkspace>());

                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer(mock.Object);

                workspaceManager.SetWorkspaceSchemesDirectory(emptyDirectory).Wait();

                Assert.AreEqual(null, workspaceManager.Workspace);
            }

            [TestCase]
            public void SetWorkspaceToDefaultWhenDirectoryIsNotEmpty()
            {
                var mock = new Mock<IWorkspacesStorageService>();

                const string defaultWorkspace = "Default name";
                const string notEmptyDirectory = "Not empty directory";

                mock.Setup(x => x.LoadWorkspaces(notEmptyDirectory)).Returns(new[] {new Workspace {Title = "Not defailt 1"}, new Workspace {Title = defaultWorkspace}, new Workspace {Title = "Not defailt 2"}});
                var workspaceManager = Factories.WorkspaceManager.WithEmptyInitializer(mock.Object);

                workspaceManager.SetWorkspaceSchemesDirectory(notEmptyDirectory, true, defaultWorkspace).Wait();

                Assert.AreEqual(defaultWorkspace, workspaceManager.Workspace.Title);
            }
        }
    }
}