namespace Orc.WorkspaceManagement.Test
{
    using System;

    public static class WorkspaceNameHelper
    {
        public static string GetRandomWorkspaceName(string prefix = "test workspace")
        {
            return string.Format("{0} - {1}", prefix, Guid.NewGuid());
        }
    }
}