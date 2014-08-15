// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceWriterService.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    public interface IWorkspaceWriter
    {
        void Write(IWorkspace workspace, string location);
    }
}