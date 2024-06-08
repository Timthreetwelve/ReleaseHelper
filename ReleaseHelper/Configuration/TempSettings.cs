﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace ReleaseHelper.Configuration;

/// <summary>
/// Class for non-persistent settings.
/// </summary>
[INotifyPropertyChanged]
internal partial class TempSettings : ConfigManager<TempSettings>
{
    [ObservableProperty]
    private static bool _appExpanderOpen;

    [ObservableProperty]
    private static bool _langExpanderOpen;

    [ObservableProperty]
    private static bool _uIExpanderOpen;
}
