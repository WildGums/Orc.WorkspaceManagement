﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Workspace.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
    using System.Collections.Generic;
    using Catel.Configuration;
    using Catel.Data;

    public class Workspace : DynamicConfiguration, IWorkspace
    {
        [NonSerialized] private object _tag;

        #region Constructors
        public Workspace()
        {
            Persist = true;
            CanEdit = true;
            CanDelete = true;
            IsVisible = true;
        }
        #endregion

        #region IWorkspace Members
        public string Title { get; set; }

        public bool Persist { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool IsVisible { get; set; }

        public void SetWorkspaceValue(string name, object value)
        {
            SetConfigurationValue(name, value);
        }

        public T GetWorkspaceValue<T>(string name, T defaultValue)
        {
            if (!IsConfigurationValueSet(name))
            {
                return defaultValue;
            }

            try
            {
                var value = this.GetConfigurationValue(name, default(T));
                return value;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public object GetTag()
        {
            return _tag;
        }

        public void SetTag(object tagValue)
        {
            _tag = tagValue;
        }
        #endregion

        #region Methods
        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            base.ValidateFields(validationResults);

            if (string.IsNullOrWhiteSpace(Title))
            {
                validationResults.Add(FieldValidationResult.CreateError("Title", "Title is required"));
            }
        }

        public override bool Equals(object obj)
        {
            var workspace = obj as Workspace;
            if (workspace == null)
            {
                return false;
            }

            if (!string.Equals(workspace.Title, Title))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Title;
        }
        #endregion
    }
}