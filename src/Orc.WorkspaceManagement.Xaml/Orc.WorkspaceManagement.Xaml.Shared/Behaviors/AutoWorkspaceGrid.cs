// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoWorkspaceGrid.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    public class AutoWorkspaceGrid : WorkspaceBehaviorBase<Grid>
    {
        private readonly Dictionary<string, string> _defaultValues = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoWorkspaceGrid"/> class.
        /// </summary>
        public AutoWorkspaceGrid()
        {
        }

        #region Properties
        public string RowsToPersist
        {
            get { return (string)GetValue(RowsToPersistProperty); }
            set { SetValue(RowsToPersistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowsToPersist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowsToPersistProperty =
            DependencyProperty.Register("RowsToPersist", typeof(string), typeof(AutoWorkspaceGrid), new PropertyMetadata(""));

        public string ColumnsToPersist
        {
            get { return (string)GetValue(ColumnsToPersistProperty); }
            set { SetValue(ColumnsToPersistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnsToPersist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsToPersistProperty =
            DependencyProperty.Register("ColumnsToPersist", typeof(string), typeof(AutoWorkspaceGrid), new PropertyMetadata(""));
        #endregion

        protected override void OnAssociatedObjectLoaded()
        {
            base.OnAssociatedObjectLoaded();

            GetDefaultValues();
        }

        private void GetDefaultValues()
        {
            var rows = GetRows();
            foreach (var index in rows)
            {
                var defaultValueName = string.Format("row_{0}_default", index);
                var defaultRowHeight = FromGridLengthToString(AssociatedObject.RowDefinitions[index].Height);

                _defaultValues[defaultValueName] = defaultRowHeight;
            }

            var columns = GetColumns();
            foreach (var index in columns)
            {
                var defaultValueName = string.Format("column_{0}_default", index);
                var defaultColumnWidth = FromGridLengthToString(AssociatedObject.ColumnDefinitions[index].Width);

                _defaultValues[defaultValueName] = defaultColumnWidth;
            }
        }

        protected override void SaveSettings(IWorkspace workspace, string prefix)
        {
            var rows = GetRows();
            foreach (var index in rows)
            {
                var name = string.Format("row_{0}", index);
                var rowHeight = AssociatedObject.RowDefinitions[index].ActualHeight.ToString();

                AssociatedObject.SaveValueToWorkspace(name, rowHeight, workspace, prefix);
            }

            var columns = GetColumns();
            foreach (var index in columns)
            {
                var name = string.Format("column_{0}", index);
                var columnWidth = AssociatedObject.ColumnDefinitions[index].ActualWidth.ToString();

                AssociatedObject.SaveValueToWorkspace(name, columnWidth, workspace, prefix);
            }
        }

        protected override void LoadSettings(IWorkspace workspace, string prefix)
        {
            var rows = GetRows();
            foreach (var index in rows)
            {
                var name = string.Format("row_{0}", index);
                var rowValue = AssociatedObject.LoadValueFromWorkspace(name, "unknown", workspace, prefix);

                GridLength gridLength;
                if (string.Equals(rowValue, "unknown"))
                {
                    var key = string.Format("row_{0}_default", index);
                    if (!_defaultValues.ContainsKey(key))
                    {
                        continue;
                    }

                    gridLength = FromStringToGridLength(_defaultValues[key]);
                }
                else
                {
                    gridLength = new GridLength(double.Parse(rowValue));
                }

                AssociatedObject.RowDefinitions[index].Height = gridLength;
            }

            var columns = GetColumns();
            foreach (var index in columns)
            {
                var name = string.Format("column_{0}", index);
                var columnValue = AssociatedObject.LoadValueFromWorkspace(name, "unknown", workspace, prefix);

                if (columnValue == null)
                {
                    continue;
                }

                GridLength gridLength;
                if (string.Equals(columnValue, "unknown"))
                {
                    var key = string.Format("column_{0}_default", index);
                    if (!_defaultValues.ContainsKey(key))
                    {
                        continue;
                    }

                    gridLength = FromStringToGridLength(_defaultValues[key]);
                }
                else
                {
                    gridLength = new GridLength(double.Parse(columnValue));
                }

                AssociatedObject.ColumnDefinitions[index].Width = gridLength;
            }
        }

        private List<int> GetRows()
        {
            return GetItemsFromString(RowsToPersist, AssociatedObject.RowDefinitions.Count);
        }

        private List<int> GetColumns()
        {
            return GetItemsFromString(ColumnsToPersist, AssociatedObject.ColumnDefinitions.Count);
        }

        private static List<int> GetItemsFromString(string input, int totalItems)
        {
            var items = new List<int>();

            if (!string.IsNullOrWhiteSpace(input))
            {
                var splittedItems = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var splittedItem in splittedItems)
                {
                    var index = 0;
                    if (int.TryParse(splittedItem, out index))
                    {
                        if (index < totalItems)
                        {
                            items.Add(index);
                        }
                    }
                }
            }
            else
            {
                for (var i = 0; i < totalItems; i++)
                {
                    items.Add(i);
                }
            }

            return items;
        }

        private static GridLength FromStringToGridLength(string str)
        {
            if (str.StartsWith("star-"))
            {
                str = str.Replace("star-", string.Empty);
                return new GridLength(double.Parse(str), GridUnitType.Star);
            }

            if (str.StartsWith("auto-"))
            {
                str = str.Replace("auto-", string.Empty);
                return new GridLength(double.Parse(str), GridUnitType.Auto);
            }

            return new GridLength(double.Parse(str));
        }

        private static string FromGridLengthToString(GridLength gridLength)
        {
            switch (gridLength.GridUnitType)
            {
                case GridUnitType.Auto:
                    return string.Format("auto-{0}", gridLength.Value);

                case GridUnitType.Pixel:
                    return string.Format("{0}", gridLength.Value);

                case GridUnitType.Star:
                    return string.Format("star-{0}", gridLength.Value);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}