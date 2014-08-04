// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceReaderService.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Services
{
    using Models;

    public interface IWorkspaceReaderService
    {
        IWorkspace Read(string directory);
    }
}