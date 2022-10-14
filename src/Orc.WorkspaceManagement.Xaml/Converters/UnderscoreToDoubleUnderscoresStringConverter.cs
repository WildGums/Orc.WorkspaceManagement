namespace Orc.WorkspaceManagement.Converters
{
    using System;
    using Catel.MVVM.Converters;

    public class UnderscoreToDoubleUnderscoresStringConverter : ValueConverterBase<string>
    {
        protected override object? Convert(string? value, Type targetType, object? parameter)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return value.Replace("_", "__");
        }
    }
}
