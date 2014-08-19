// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryWorkspaceWriter.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Test.Mocks
{
    using System.Threading.Tasks;

    public class MemoryWorkspaceWriter : WorkspaceWriterBase<Workspace>
    {
        protected override async Task WriteToLocation(Workspace workspace, string location)
        {
            // no implementation required
        }
    }
}