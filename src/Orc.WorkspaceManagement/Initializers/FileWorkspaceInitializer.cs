// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceInitializer.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using Catel;
    using Services;

    public class FileWorkspaceInitializer : IWorkspaceInitializer
    {
        private readonly ICommandLineService _commandLineService;

        public FileWorkspaceInitializer(ICommandLineService commandLineService)
        {
            Argument.IsNotNull(() => commandLineService);

            _commandLineService = commandLineService;
        }

        public string GetInitialLocation()
        {
            string filePath = null;

            if (_commandLineService.Arguments.Length > 0)
            {
                filePath = _commandLineService.Arguments[0];
            }

            return filePath;
        }
    }
}