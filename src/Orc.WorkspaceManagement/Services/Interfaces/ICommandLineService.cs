// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandLineService.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Services
{
    public interface ICommandLineService
    {
        /// <summary>
        /// Gets the application command line argument.
        /// </summary>
        string[] Arguments { get; }
    }
}