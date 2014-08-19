// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonWorkspaceWriter.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Example.Services
{
    using System.IO;
    using System.Threading.Tasks;
    using Models;

    public class PersonWorkspaceWriter : WorkspaceWriterBase<PersonWorkspace>
    {
        protected override async Task WriteToLocation(PersonWorkspace workspace, string location)
        {
            using (var fileStream = new FileStream(location, FileMode.Create, FileAccess.Write))
            {
                using (var textWriter = new StreamWriter(fileStream))
                {
                    textWriter.WriteLine("{0};{1};{2}", workspace.FirstName ?? string.Empty,
                        workspace.MiddleName ?? string.Empty, workspace.LastName ?? string.Empty);
                }
            }
        }
    }
}