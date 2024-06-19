// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace ReleaseHelper.ViewModels;

internal partial class MainViewModel : ObservableObject, IDropTarget
{
    #region Constructor
    public MainViewModel()
    {
        EventHandlers();

        CheckCommandLine();

        CheckApiKey();

        ApplyUISettings();
    }
    #endregion Constructor

    #region Properties
    /// <summary>
    /// Complete path for the file to be examined.
    /// </summary>
    [ObservableProperty]
    private string? _filePath;

    /// <summary>
    /// File name of the file to be examined. Does not include path.
    /// </summary>
    [ObservableProperty]
    private string? _fileName;

    /// <summary>
    /// <c>SHA256</c> hash of the file.
    /// </summary>
    [ObservableProperty]
    private string? _fileHash;

    /// <summary>
    /// Size of the file as a string with suffix (bytes, KB, MB, etc.)
    /// </summary>
    [ObservableProperty]
    private string? _fileSize;

    /// <summary>
    /// Progress bar animation.
    /// </summary>
    [ObservableProperty]
    private bool _progressAnimation;

    /// <summary>
    /// <c>FileInfo</c> of the file.
    /// </summary>
    [ObservableProperty]
    private FileInfo? _uploadFileInfo;

    /// <summary>
    /// Message
    /// </summary>
    [ObservableProperty]
    private string? _vTMessage;
    #endregion Properties

    #region Browse for file - Relay command
    /// <summary>
    /// OpenFile dialog used to browse for an input file.
    /// </summary>
    [RelayCommand]
    public void BrowseForFile()
    {
        OpenFileDialog dlgOpen = new()
        {
            Multiselect = false,
            Title = "Browse for file",
            CheckFileExists = true,
            CheckPathExists = true,
            Filter = "EXE or ZIP|*.exe;*.zip|All Files|*.*"
        };
        if (dlgOpen.ShowDialog() == true)
        {
            FilePath = dlgOpen.FileName;
            UploadFileInfo = new(FilePath.Trim('"'));
            GetDetails();
        }
    }
    #endregion Browse for file - Relay command

    #region Clear text boxes - Relay command
    /// <summary>
    /// Clears the form.
    /// </summary>
    [RelayCommand]
    public void Clear()
    {
        FileHash = string.Empty;
        FileName = string.Empty;
        FilePath = string.Empty;
        FileSize = string.Empty;
    }
    #endregion Clear text boxes - Relay command

    #region Copy to clipboard - Relay command
    /// <summary>
    /// Copy the adjacent textbox contents to the clipboard.
    /// </summary>
    /// <param name="sender">Button that was clicked.</param>
    [RelayCommand]
    public void CopyToClipboard(object sender)
    {
        if (sender is Button button)
        {
            switch (button!.Tag.ToString()!.ToLower())
            {
                case "hash":
                    if (FileHash is not null)
                    {
                        Clipboard.Clear();
                        Clipboard.SetText(FileHash);
                    }
                    break;
                case "filename":
                    if (FileName is not null)
                    {
                        Clipboard.Clear();
                        Clipboard.SetText(FileName);
                    }
                    break;
                case "size":
                    if (FileSize is not null)
                    {
                        Clipboard.Clear();
                        Clipboard.SetText(FileSize);
                    }
                    break;
            }
        }
    }
    #endregion Copy to clipboard - Relay command

    #region Key down event - Relay command
    /// <summary>
    /// Key down event for the file name textbox.
    /// </summary>
    /// <param name="e">Event args.</param>
    [RelayCommand]
    public void PreviewKeyDown(KeyEventArgs e)
    {
        if ((e.Key == Key.Enter || e.Key == Key.Tab) && !string.IsNullOrEmpty(FilePath))
        {
            UploadFileInfo = new(FilePath.Trim('"'));
            GetDetails();
        }
    }
    #endregion Key down event - Relay command

    #region Submit to VirusTotal
    /// <summary>
    /// Submit file to VirusTotal for analysis.
    /// </summary>
    [RelayCommand]
    public async Task Submit()
    {
        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
        {
            Clipboard.Clear();
            Clipboard.SetText(VTMessage);
        }
        else
        {
            VTMessage = await VirusTotalHelpers.UploadFileAsync(UploadFileInfo!);
        }
    }
    #endregion Submit to VirusTotal

    #region Settings button - Relay command
    [RelayCommand]
    public static void OpenSettings()
    {
        TextFileViewer.ViewTextFile(ConfigHelpers.SettingsFileName!);
    }
    #endregion Settings button - Relay command

    #region View Log - Relay Command
    [RelayCommand]
    private static void ViewLog()
    {
        TextFileViewer.ViewTextFile(NLogHelpers.GetLogfileName());
    }
    #endregion View Log - Relay Command

    #region Check API key
    /// <summary>
    /// Check VT API key
    /// </summary>
    private void CheckApiKey()
    {
        VTMessage = VirusTotalHelpers.VirusTotalKey?.Length == 64 ? Strings.MsgText_ApiKeyOK : Strings.MsgText_ApiKeyInvalid;
    }
    #endregion Check API key

    #region Command line
    /// <summary>
    /// Check for file name passed on the command line.
    /// </summary>
    private void CheckCommandLine()
    {
        string[] args = Environment.GetCommandLineArgs();
        if (args.Length > 1 && File.Exists(args[1]))
        {
            FilePath = args[1];
            _log.Info($"Found command line argument {FilePath}");
            UploadFileInfo = new(FilePath.Trim('"'));
            GetDetails();
        }
    }
    #endregion Command line

    #region Drag & drop
    /// <summary>
    /// DragOver handler.
    /// </summary>
    void IDropTarget.DragOver(IDropInfo dropInfo)
    {
        if ((dropInfo.Data as DataObject)?.ContainsFileDropList() == true)
        {
            dropInfo.Effects = DragDropEffects.Copy;
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        }
    }

    /// <summary>
    /// Drop handler.
    /// </summary>
    void IDropTarget.Drop(IDropInfo dropInfo)
    {
        UploadFileInfo = new(((DataObject)dropInfo.Data).GetFileDropList().Cast<string>().FirstOrDefault()!);
        GetDetails();
    }
    #endregion Drag & drop

    #region Get file details
    /// <summary>
    /// Gets details for a file if it exists.
    /// </summary>
    private void GetDetails()
    {
        _log.Debug("Getting details...");
        if (File.Exists(UploadFileInfo!.FullName))
        {
            FilePath = UploadFileInfo!.FullName;
            FileSize = FormatHelpers.GetFileSize(UploadFileInfo);
            FileName = UploadFileInfo.Name;
            _log.Info($"File path is: {FilePath}");
            _log.Info($"File size is: {FileSize}");
            _log.Info($"File name is: {FileName}");
            CalculateHash();
        }
        else
        {
            _log.Error($"File not found: {UploadFileInfo!.FullName}");
            _ = MessageBox.Show($"{Strings.MsgText_FileNotFound}\n\n{UploadFileInfo!.FullName}",
                "Release Helper - Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
    #endregion Get file details

    #region Calculate SHA256 on background worker
    private void CalculateHash()
    {
        FileHash = string.Empty;
        BackgroundWorker worker = new();
        worker.DoWork += GetHash!;
        worker.RunWorkerCompleted += CompletedHash!;
        worker.RunWorkerAsync();
    }

    private void GetHash(object sender, DoWorkEventArgs e)
    {
        ProgressAnimation = true;
        e.Result = GetSHA256(UploadFileInfo!);
    }

    private void CompletedHash(object sender, RunWorkerCompletedEventArgs e)
    {
        FileHash = e.Result!.ToString();
        ProgressAnimation = false;
        _log.Info($"SHA256 hash is: {FileHash}");
    }
    #endregion Calculate SHA256 on background worker

    #region Compute SHA256 hash
    /// <summary>
    /// Computes the SHA256 hash for a file.
    /// </summary>
    /// <param name="info"><c>FileInfo</c> for the file to be examined.</param>
    /// <returns>SHA256 as a string.</returns>
    private static string GetSHA256(FileInfo info)
    {
        string file = info.FullName;
        using FileStream stream = File.OpenRead(file);
        using SHA256 sha256 = SHA256.Create();
        byte[] checksum = sha256.ComputeHash(stream);
        return BitConverter.ToString(checksum).Replace("-", string.Empty).ToLower();
    }
    #endregion Compute SHA256 hash

    #region MainWindow Instance
    private static readonly MainWindow? _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region Event handlers
    /// <summary>
    /// Event handlers.
    /// </summary>
    internal static void EventHandlers()
    {
        // Settings change events
        UserSettings.Setting!.PropertyChanged += SettingChange.UserSettingChanged!;
        TempSettings.Setting!.PropertyChanged += SettingChange.TempSettingChanged!;

        // Window closing event
        _mainWindow!.Closing += MainWindowHelpers.MainWindow_Closing!;
    }
    #endregion Event handlers

    #region Apply UI settings
    /// <summary>
    /// Single method called during startup to apply UI settings.
    /// </summary>
    public static void ApplyUISettings()
    {
        // Put version number in window title
        _mainWindow!.Title = MainWindowHelpers.WindowTitleVersionAdmin();

        // Window position
        MainWindowHelpers.SetWindowPosition();

        // Light or dark theme
        MainWindowHelpers.SetBaseTheme(UserSettings.Setting!.UITheme);
    }
    #endregion Apply UI settings
}
