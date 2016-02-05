// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Workspace.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
    using System.Collections.Generic;
    using Catel.Configuration;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    public class Workspace : DynamicConfiguration, IWorkspace
    {
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

        [ExcludeFromSerialization]
        public object Tag { get; set; } 

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