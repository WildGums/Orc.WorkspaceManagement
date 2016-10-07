// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceUpdatingEventArgs.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.ComponentModel;
    using Catel;

    public class WorkspaceUpdatingEventArgs : CancelEventArgs
    {
        public WorkspaceUpdatingEventArgs(IWorkspace oldWorkspace, IWorkspace newWorkspace, bool cancel = false)
            : base(cancel)
        {
            OldWorkspace = oldWorkspace;
            NewWorkspace = newWorkspace;
        }

        public IWorkspace OldWorkspace { get; set; }
        public IWorkspace NewWorkspace { get; set; }

        public bool IsRefresh
        {
            get
            {
                if (OldWorkspace == null || NewWorkspace == null)
                {
                    return false;
                }

                return ObjectHelper.AreEqual(OldWorkspace, NewWorkspace);
            }
        }
    }
}