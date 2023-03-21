namespace Orc.WorkspaceManagement.Converters;

using System;
using System.Windows.Data;
using Catel.MVVM.Converters;

/// <summary>
/// Workaround class for bug with non-evaluating commands with command parameters:
/// http://stackoverflow.com/questions/335849/wpf-commandparameter-is-null-first-time-canexecute-is-called
/// </summary>
public class TriggerConverter : IMultiValueConverter
{
    public object? Convert(object?[]? values, Type targetType, object? parameter, System.Globalization.CultureInfo? culture)
    {
        if (values is null)
        {
            return null;
        }

        // First value is target value.
        // All others are update triggers only.
        return values.Length >= 1 ? values[0] : ConverterHelper.UnsetValue;
    }

    public object?[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, System.Globalization.CultureInfo? culture)
    {
        return null;
    }
}
