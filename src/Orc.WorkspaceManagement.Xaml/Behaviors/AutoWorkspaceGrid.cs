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

        public string? RowsToPersist
        {
            get { return (string?)GetValue(RowsToPersistProperty); }
            set { SetValue(RowsToPersistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowsToPersist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowsToPersistProperty =
            DependencyProperty.Register(nameof(RowsToPersist), typeof(string), typeof(AutoWorkspaceGrid), new PropertyMetadata(""));


        public string? ColumnsToPersist
        {
            get { return (string?)GetValue(ColumnsToPersistProperty); }
            set { SetValue(ColumnsToPersistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnsToPersist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsToPersistProperty =
            DependencyProperty.Register(nameof(ColumnsToPersist), typeof(string), typeof(AutoWorkspaceGrid), new PropertyMetadata(""));


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
                var defaultValueName = $"row_{index}_default";
                var defaultRowHeight = FromGridLengthToString(AssociatedObject.RowDefinitions[index].Height);

                _defaultValues[defaultValueName] = defaultRowHeight;
            }

            var columns = GetColumns();

            foreach (var index in columns)
            {
                var defaultValueName = $"column_{index}_default";
                var defaultColumnWidth = FromGridLengthToString(AssociatedObject.ColumnDefinitions[index].Width);

                _defaultValues[defaultValueName] = defaultColumnWidth;
            }
        }

        protected override void SaveSettings(IWorkspace workspace, string? prefix)
        {
            var rows = GetRows();

            foreach (var index in rows)
            {
                var name = $"row_{index}";
                var rowHeight = AssociatedObject.RowDefinitions[index].ActualHeight.ToString();

                AssociatedObject.SaveValueToWorkspace(name, rowHeight, workspace, prefix);
            }

            var columns = GetColumns();
            foreach (var index in columns)
            {
                var name = $"column_{index}";
                var columnWidth = AssociatedObject.ColumnDefinitions[index].ActualWidth.ToString();

                AssociatedObject.SaveValueToWorkspace(name, columnWidth, workspace, prefix);
            }
        }

        protected override void LoadSettings(IWorkspace workspace, string? prefix)
        {
            LoadGridRowSettings(workspace, prefix);

            LoadGridColumnsSettings(workspace, prefix);
        }

        private void LoadGridColumnsSettings(IWorkspace workspace, string? prefix)
        {
            var columns = GetColumns();

            foreach (var index in columns)
            {
                var name = $"column_{index}";
                var columnValue = AssociatedObject.LoadValueFromWorkspace(name, "unknown", workspace, prefix);

                if (columnValue is null)
                {
                    continue;
                }

                GridLength gridLength;
                if (string.Equals(columnValue, "unknown"))
                {
                    var key = $"column_{index}_default";
                    if (!_defaultValues.ContainsKey(key))
                    {
                        continue;
                    }

                    gridLength = FromStringToGridLength(_defaultValues[key]);
                }
                else
                {
                    if (!double.TryParse(columnValue, out var gridValue))
                    {
                        return;
                    }

                    gridLength = new GridLength(gridValue);
                }

                AssociatedObject.ColumnDefinitions[index].SetCurrentValue(ColumnDefinition.WidthProperty, gridLength);
            }
        }

        private void LoadGridRowSettings(IWorkspace workspace, string? prefix)
        {
            var rows = GetRows();

            foreach (var index in rows)
            {
                var name = $"row_{index}";
                var rowValue = AssociatedObject.LoadValueFromWorkspace(name, "unknown", workspace, prefix);

                GridLength gridLength;
                if (string.Equals(rowValue, "unknown"))
                {
                    var key = $"row_{index}_default";
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

                AssociatedObject.RowDefinitions[index].SetCurrentValue(RowDefinition.HeightProperty, gridLength);
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

        private static List<int> GetItemsFromString(string? input, int totalItems)
        {
            var items = new List<int>();

            if (!string.IsNullOrWhiteSpace(input))
            {
                var splittedItems = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var splittedItem in splittedItems)
                {
                    if (int.TryParse(splittedItem, out var index) && index < totalItems)
                    {
                        items.Add(index);
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
                    return $"auto-{gridLength.Value}";

                case GridUnitType.Pixel:
                    return $"{gridLength.Value}";

                case GridUnitType.Star:
                    return $"star-{gridLength.Value}";

                default:
                    throw new ArgumentOutOfRangeException(nameof(gridLength));
            }
        }
    }
}
