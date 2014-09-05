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
    using System.Windows.Documents;

    public class AutoWorkspaceGrid : WorkspaceBehaviorBase<Grid>
    {
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

        protected override void SaveSettings(IWorkspace workspace, string prefix)
        {
            var rows = GetRows();
            foreach (var index in rows)
            {
                var name = string.Format("row_{0}", index);
                var rowHeight = AssociatedObject.RowDefinitions[index].ActualHeight;

                AssociatedObject.SaveValueToWorkspace(name, rowHeight, workspace, prefix);
            }

            var columns = GetColumns();
            foreach (var index in columns)
            {
                var name = string.Format("column_{0}", index);
                var columnWidth = AssociatedObject.ColumnDefinitions[index].ActualWidth;

                AssociatedObject.SaveValueToWorkspace(name, columnWidth, workspace, prefix);
            }
        }

        protected override void LoadSettings(IWorkspace workspace, string prefix)
        {
            var rows = GetRows();
            foreach (var index in rows)
            {
                var name = string.Format("row_{0}", index);
                var rowHeight = AssociatedObject.RowDefinitions[index].ActualHeight;

                rowHeight = AssociatedObject.LoadValueFromWorkspace(name, rowHeight, workspace, prefix);

                AssociatedObject.RowDefinitions[index].Height = new GridLength(rowHeight);
            }

            var columns = GetColumns();
            foreach (var index in columns)
            {
                var name = string.Format("column_{0}", index);
                var columnWidth = AssociatedObject.ColumnDefinitions[index].ActualWidth;

                columnWidth = AssociatedObject.LoadValueFromWorkspace(name, columnWidth, workspace, prefix);

                AssociatedObject.ColumnDefinitions[index].Width = new GridLength(columnWidth);
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
                var splittedItems = input.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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
    }
}