// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceManager.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Services
{
    using System;
    using Models;

    public interface IWorkspaceManager
    {
        void Refresh();
        string CurrentDirectory { get; }
        IWorkspace CurrentWorkspace { get; }
        event EventHandler<EventArgs> WorkspaceUpdated;
        void Save();
    }
}