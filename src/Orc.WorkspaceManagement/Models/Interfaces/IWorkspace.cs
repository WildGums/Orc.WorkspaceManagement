// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspace.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.Collections.Generic;
    using Catel.Runtime.Serialization;

    public interface IWorkspace
    {
        #region Properties
        string Title { get; }

        bool Persist { get; set; }
        bool CanEdit { get; set; }
        bool CanDelete { get; set; }
        bool IsVisible { get; set; }
        bool IsDirty { get; }

        string WorkspaceGroup { get; set; }
        string DisplayName { get; }

        [ExcludeFromSerialization]
        object Scope { get; set; }
        #endregion

        #region Methods
        void SetWorkspaceValue(string name, object value);
        T GetWorkspaceValue<T>(string name, T defaultValue);
        List<string> GetAllWorkspaceValueNames();
        void ClearWorkspaceValues();
        void UpdateIsDirtyFlag(bool isDirty);
        #endregion
    }
}
