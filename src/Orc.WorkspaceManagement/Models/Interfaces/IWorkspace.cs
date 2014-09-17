// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspace.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    public interface IWorkspace
    {
        #region Properties
        string Title { get; set; }
        bool CanEdit { get; set; }
        bool CanDelete { get; set; }
        #endregion

        #region Methods
        void SetWorkspaceValue(string name, object value);
        T GetWorkspaceValue<T>(string name, T defaultValue);
        #endregion
    }
}