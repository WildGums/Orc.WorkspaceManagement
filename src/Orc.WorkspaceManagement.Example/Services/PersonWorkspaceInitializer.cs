// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonWorkspaceInitializer.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Example.Services
{
    using WorkspaceManagement.Services;

    public class PersonWorkspaceInitializer : FileWorkspaceInitializer
    {
        public PersonWorkspaceInitializer(ICommandLineService commandLineService) 
            : base(commandLineService)
        {
        }

        public override string GetInitialLocation()
        {
            var baseLocation =  base.GetInitialLocation();

            if (string.IsNullOrEmpty(baseLocation))
            {
                baseLocation = "Data\\ExamplePerson.txt";
            }

            return baseLocation;
        }
    }
}