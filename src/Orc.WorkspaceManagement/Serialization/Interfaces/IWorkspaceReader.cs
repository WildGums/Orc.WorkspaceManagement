// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceReaderService.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    public interface IWorkspaceReader
    {
        IWorkspace Read(string location);
    }
}