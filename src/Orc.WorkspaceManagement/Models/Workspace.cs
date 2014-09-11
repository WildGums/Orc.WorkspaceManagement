// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Workspace.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
    using System.Collections.Generic;
    using Catel;
    using Catel.Configuration;
    using Catel.Data;

    public class Workspace : DynamicConfiguration, IWorkspace
    {
        #region Constructors
        public Workspace()
        {
        }
        #endregion

        #region IWorkspace Members
        public string Title { get; set; }

        public void SetWorkspaceValue(string name, object value)
        {
            SetConfigurationValue(name, value);
        }

        public T GetWorkspaceValue<T>(string name, T defaultValue)
        {
            if (!IsConfigurationKeyAvailable(name))
            {
                return defaultValue;
            }

            try
            {
                var value = this.GetConfigurationValue<T>(name);
                if (ObjectHelper.AreEqual(value, default(T)))
                {
                    return defaultValue;
                }

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

        public override string ToString()
        {
            return Title;
        }
        #endregion
    }
}