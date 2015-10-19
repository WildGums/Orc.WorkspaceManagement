// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspace.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    public interface IWorkspace
    {
        #region Properties
        string Title { get; set; }

        bool Persist { get; set; }
        bool CanEdit { get; set; }
        bool CanDelete { get; set; }
        bool IsVisible { get; set; }
        #endregion

        #region Methods
        void SetWorkspaceValue(string name, object value);
        T GetWorkspaceValue<T>(string name, T defaultValue);
        object GetTag();
        void SetTag(object tagValue);
        #endregion
    }
}