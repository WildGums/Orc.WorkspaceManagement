// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonWorkspaceReader.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Example.Services
{
    using System.IO;
    using System.Threading.Tasks;
    using Models;

    public class PersonWorkspaceReader : WorkspaceReaderBase
    {
        protected override async Task<IWorkspace> ReadFromLocation(string location)
        {
            var workspace = new PersonWorkspace(location);

            if (File.Exists(location))
            {
                using (var fileStream = new FileStream(location, FileMode.Open, FileAccess.Read))
                {
                    using (var textReader = new StreamReader(fileStream))
                    {
                        var content = textReader.ReadLine();

                        var splittedString = content.Split(new[] { ';' });
                        if (splittedString.Length > 0)
                        {
                            workspace.FirstName = splittedString[0];
                        }

                        if (splittedString.Length > 1)
                        {
                            workspace.MiddleName = splittedString[1];
                        }

                        if (splittedString.Length > 2)
                        {
                            workspace.LastName = splittedString[2];
                        }
                    }
                }
            }

            return workspace;
        }
    }
}