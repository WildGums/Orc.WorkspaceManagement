// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceManager.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;

    public interface IWorkspaceManager
    {
        void Refresh();
        string CurrentLocation { get; }
        IWorkspace CurrentWorkspace { get; }
        event EventHandler<EventArgs> WorkspaceUpdated;
        void Save();
    }
}