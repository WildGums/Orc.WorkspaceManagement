// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryWorkspaceReader.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Test.Mocks
{
    using System.Threading.Tasks;

    public class MemoryWorkspaceReader : WorkspaceReaderBase
    {
        protected override async Task<IWorkspace> ReadFromLocation(string location)
        {
            return new Workspace(location);
        }
    }
}