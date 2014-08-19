// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceWriterService.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.Threading.Tasks;

    public interface IWorkspaceWriter
    {
        Task Write(IWorkspace workspace, string location);
    }
}