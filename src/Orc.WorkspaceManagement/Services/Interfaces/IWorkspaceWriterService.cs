// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceWriterService.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Services
{
    using Models;

    public interface IWorkspaceWriterService
    {
        void Write(IWorkspace workspace, string directory);
    }
}