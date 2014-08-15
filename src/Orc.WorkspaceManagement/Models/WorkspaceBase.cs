// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Workspace.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using Catel.Data;

    public abstract class WorkspaceBase : ModelBase, IWorkspace
    {
        protected WorkspaceBase(string title)
        {
            Title = title;
        }

        public string Title { get; private set; }

        public void ClearIsDirty()
        {
            IsDirty = false;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}