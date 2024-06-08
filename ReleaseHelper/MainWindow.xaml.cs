// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace ReleaseHelper;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}