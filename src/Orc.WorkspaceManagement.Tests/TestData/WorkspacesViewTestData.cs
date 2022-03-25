namespace Orc.FilterBuilder.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using WorkspaceManagement;

    public static class WorkspacesViewTestData
    {
        public static List<string> AvailableScopes = new()
        {
            TestScopeWith0CustomRecords,
            TestScopeWith3CustomRecords,
            TestScopeWith5CustomRecords
        };

        public const string TestScopeWith0CustomRecords = nameof(TestScopeWith0CustomRecords);
        public const string TestScopeWith3CustomRecords = nameof(TestScopeWith3CustomRecords);
        public const string TestScopeWith5CustomRecords = nameof(TestScopeWith5CustomRecords);

        public static List<WorkspaceGroup> GetWorkspacesGroups(object scope)
        {
            var workspaceGroups = (from workspace in GetWorkspaces(scope)
                                   where workspace.IsVisible
                orderby workspace.WorkspaceGroup, workspace.Title, workspace.CanDelete
                group workspace by workspace.WorkspaceGroup into g
                select new WorkspaceGroup(string.IsNullOrWhiteSpace(g.Key) ? null : g.Key, g)).ToList();

            return workspaceGroups;
        }

        public static List<IWorkspace> GetWorkspaces(object scope)
        {
            if (Equals(scope, TestScopeWith3CustomRecords))
            {
                return new List<IWorkspace>
                {
                    new Workspace("First")
                    {
                        Scope = scope
                    },
                    new Workspace("Second")
                    {
                        Scope = scope
                    },
                    new Workspace("Third")
                    {
                        Scope = scope
                    },
                };
            }

            if (Equals(scope, TestScopeWith5CustomRecords))
            {
                return new List<IWorkspace>
                {
                    new Workspace("One")
                    {
                        WorkspaceGroup = "Numbers",
                        Scope = scope
                    },
                    new Workspace("Two")
                    {
                        WorkspaceGroup = "Numbers",
                        Scope = scope
                    },
                    new Workspace("Three")
                    {
                        WorkspaceGroup = "Numbers",
                        Scope = scope
                    },
                    new Workspace("Four")
                    {
                        WorkspaceGroup = "Numbers",
                        Scope = scope
                    },
                    new Workspace("Five")
                    {
                        WorkspaceGroup = "Numbers",
                        Scope = scope
                    },
                    new Workspace("Five")
                    {
                        WorkspaceGroup = "Animals",
                        Scope = scope
                    },
                    new Workspace("Mouse")
                    {
                        WorkspaceGroup = "Animals",
                        Scope = scope
                    },
                    new Workspace("Platypus")
                    {
                        WorkspaceGroup = "Animals",
                        Scope = scope
                    },
                    new Workspace("Elephant")
                    {
                        WorkspaceGroup = "Animals",
                        Scope = scope
                    },
                    new Workspace("Tyrannosaur")
                    {
                        WorkspaceGroup = "Dinosaurs",
                        Scope = scope
                    },
                    new Workspace("Ankylosaurus")
                    {
                        WorkspaceGroup = "Dinosaurs",
                        Scope = scope
                    },
                };
            }

            return new List<IWorkspace>();
        }
    }
}
