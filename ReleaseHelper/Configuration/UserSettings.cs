// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace ReleaseHelper.Configuration;

[INotifyPropertyChanged]
public partial class UserSettings : ConfigManager<UserSettings>
{
    #region Properties (some with default values)
    [ObservableProperty]
    private string? _apiKey = string.Empty;

    [ObservableProperty]
    private bool _includeDebug = true;

    [ObservableProperty]
    private bool _keepOnTop;

    [ObservableProperty]
    private bool _startCentered = true;

    [ObservableProperty]
    private string _uILanguage = "en-US";

    [ObservableProperty]
    private ThemeType _uITheme = ThemeType.System;

    [ObservableProperty]
    private double _windowHeight = 375;

    [ObservableProperty]
    private double _windowLeft = 100;

    [ObservableProperty]
    private double _windowTop = 100;

    [ObservableProperty]
    private double _windowWidth = 670;
    #endregion Properties (some with default values)
}
