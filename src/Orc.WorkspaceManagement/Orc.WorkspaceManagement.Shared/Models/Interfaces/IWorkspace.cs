﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspace.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using Catel.Runtime.Serialization;

    public interface IWorkspace
    {
        #region Properties
        string Title { get; set; }

        bool Persist { get; set; }
        bool CanEdit { get; set; }
        bool CanDelete { get; set; }
        bool IsVisible { get; set; }

        [ExcludeFromSerialization]
        object Tag { get; set; }
        #endregion

        #region Methods
        void SetWorkspaceValue(string name, object value);
        T GetWorkspaceValue<T>(string name, T defaultValue);
        #endregion
    }
}