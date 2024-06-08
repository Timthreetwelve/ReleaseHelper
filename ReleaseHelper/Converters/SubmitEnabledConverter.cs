// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace ReleaseHelper.Converters;

internal class SubmitEnabledConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is string apiKey)
        {
            return !string.IsNullOrEmpty((string)value) && apiKey.Length == 64;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}
