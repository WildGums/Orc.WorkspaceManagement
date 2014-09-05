// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspace.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    public interface IWorkspace
    {
        string Title { get; set; }

        void SetWorkspaceValue(string name, object value);
        T GetWorkspaceValue<T>(string name, T defaultValue);
    }
}