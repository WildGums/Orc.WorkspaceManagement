﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceEventArgs.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
    using Catel;

    public class WorkspaceUpdatedEventArgs : EventArgs
    {
        public WorkspaceUpdatedEventArgs(IWorkspace oldWorkspace, IWorkspace newWorkspace)
        {
            OldWorkspace = oldWorkspace;
            NewWorkspace = newWorkspace;
        }

        public IWorkspace OldWorkspace { get; private set; }

        public IWorkspace NewWorkspace { get; private set; }

        public bool IsRefresh
        {
            get
            {
                if (OldWorkspace == null || NewWorkspace == null)
                {
                    return false;
                }

                return ObjectHelper.AreEqual(OldWorkspace.Location, NewWorkspace.Location);
            }
        }
    }
}