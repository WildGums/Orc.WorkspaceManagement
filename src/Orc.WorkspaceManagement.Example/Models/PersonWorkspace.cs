// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonWorkspace.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Example.Models
{
    public class PersonWorkspace : WorkspaceBase
    {
        public PersonWorkspace(string title) 
            : base(title)
        {
        }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }
    }
}