// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Workspace.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
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
        private static readonly HashSet<string> IgnoredProperties = new HashSet<string>(new[]
        {
            nameof(Title),
            nameof(Persist),
            nameof(CanEdit),
            nameof(CanDelete),
            nameof(IsVisible),
            nameof(Scope),
            nameof(Tag),
            nameof(IsReadOnly),
            nameof(IsDirty),
            nameof(DisplayName)
        });

        private bool _updatingDisplayName;

        #region Constructors
        public Workspace()
        {
            Persist = true;
            CanEdit = true;
            CanDelete = true;
            IsVisible = true;
            IsDirty = false;
        }
        #endregion

        #region IWorkspace Members
        public string Title { get; set; }
        public string DisplayName { get; private set; }

        public bool Persist { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool IsVisible { get; set; }
        public new bool IsDirty { get; private set; }

        [ExcludeFromSerialization]
        public object Scope { get; set; }

        [ExcludeFromSerialization]
        public object Tag { get; set; }

        public void ClearWorkspaceValues()
        {
            var workspaceValueNames = GetAllWorkspaceValueNames();

            foreach (var workspaceValueName in workspaceValueNames)
            {
                SetWorkspaceValue(workspaceValueName, null);
            }
        }

        protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
        {
            if (_updatingDisplayName)
            {
                return;
            }

            if (string.Equals(e.PropertyName, nameof(Title)) ||
                string.Equals(e.PropertyName, nameof(IsDirty)))
            {
                UpdateDisplayName();
            }

            if (IgnoredProperties.Contains(e.PropertyName))
            {
                base.OnPropertyChanged(e);
                return;
            }

            IsDirty = true;
        }

        private void UpdateDisplayName()
        {
            _updatingDisplayName = true;

            try
            {
                DisplayName = IsDirty
                    ? $"{Title}*"
                    : Title;
            }
            finally
            {
                _updatingDisplayName = false;
            }
        }

        public List<string> GetAllWorkspaceValueNames()
        {
            var valueNames = new List<string>();

            var propertyData = PropertyDataManager.Default.GetCatelTypeInfo(GetType());

            foreach (var catelProperty in propertyData.GetCatelProperties())
            {
                if (catelProperty.Value.IsModelBaseProperty)
                {
                    continue;
                }

                if (IgnoredProperties.Contains(catelProperty.Key))
                {
                    continue;
                }

                valueNames.Add(catelProperty.Key);
            }

            return valueNames;
        }

        public void UpdateIsDirtyFlag(bool isDirty)
        {
            IsDirty = isDirty;
        }

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
