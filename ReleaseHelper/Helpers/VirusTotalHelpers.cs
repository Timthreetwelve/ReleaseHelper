// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace ReleaseHelper.Helpers;

internal static class VirusTotalHelpers
{
    #region Properties
    /// <summary>
    /// Size after which VirusTotal requires a special URL for uploading files.
    /// </summary>
    public static double MaxBytes { get; } = 33554432; //32 Mb

    /// <summary>
    /// URL used to request the special URL.
    /// </summary>
    public static string UploadURL { get; } = "https://www.virustotal.com/api/v3/files/upload_url";

    /// <summary>
    /// My personal VT API key. Needed to be moved to a settings file.
    /// </summary>
    public static string? VirusTotalKey { get; } = UserSettings.Setting!.ApiKey!;
    #endregion Properties

    #region Get URL for upload
    /// <summary>
    /// Gets URL for uploading file. VirusTotal requires a special URL for files over 32Mb.
    /// </summary>
    /// <returns>The URL</returns>
    public static async Task<string> GetVTUrlAsync(FileInfo fileInfo)
    {
        if (fileInfo is null)
            return string.Empty;

        if (fileInfo!.Length > MaxBytes)
        {
            _log.Info($"{fileInfo.Name} is larger than {MaxBytes:N0} bytes. Using custom URL for upload.");

            RestClient client = new(UploadURL);
            RestRequest request = new("");
            request.AddHeader("x-apikey", VirusTotalKey!);
            RestResponse response = await client.GetAsync(request);

            JsonDocument? json = JsonDocument.Parse(response.Content!);
            string? result = json.RootElement.GetProperty("data").GetString();
            return result!;
        }
        else
        {
            return "https://www.virustotal.com/api/v3/files";
        }
    }
    #endregion Get URL for upload

    #region Send file to VirusTotal
    /// <summary>
    /// Submit the file to VirusTotal for analysis.
    /// </summary>
    /// <param name="fileInfo">The file to be submitted.</param>
    /// <returns>Response from VirusTotal</returns>
    public static async Task<string> UploadFileAsync(FileInfo fileInfo)
    {
        string uploadURL = await GetVTUrlAsync(fileInfo);

        if (uploadURL != null)
        {
            _log.Debug($"The upload URL is: {uploadURL}");

            RestClient client = new(uploadURL);
            RestRequest request = new RestRequest("")
                        .AddHeader("x-apikey", VirusTotalKey!)
                        .AddFile("file", fileInfo.FullName, "multipart/form-data");
            try
            {
                RestResponse response = await client.ExecutePostAsync(request);
                JsonDocument? document = JsonDocument.Parse(response.Content!);
                if (response is not { IsSuccessful: true })
                {
                    bool error_result = document.RootElement.TryGetProperty("error", out JsonElement errorElement);
                    if (error_result)
                    {
                        _ = errorElement.TryGetProperty("code", out JsonElement vt_code);
                        _ = errorElement.TryGetProperty("message", out JsonElement vt_msg);
                        _log.Error($"Code:    {vt_code}");
                        _log.Error($"Message: {vt_msg}");

                        _ = MessageBox.Show(vt_code.ToString(),
                                Strings.MsgText_ErrorCaption,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        return vt_code.ToString();
                    }
                    else
                    {
                        _log.Error($"JSON: {response.Content}");
                        return string.Empty;
                    }
                }
                bool result = document.RootElement.TryGetProperty("data", out JsonElement element);
                if (result)
                {
                    _ = element.TryGetProperty("type", out JsonElement vt_type);
                    _ = element.TryGetProperty("id", out JsonElement vt_id);
                    _log.Info($"Type: {vt_type}");
                    _log.Info($"ID:   {vt_id}");
                    string now = DateTime.Now.ToString("HH:mm");
                    if (vt_type.ToString().Equals("analysis", StringComparison.OrdinalIgnoreCase))
                    {
                        return $"Submitted for analysis at {now}";
                    }
                    else
                    {
                        return $"VirusTotal returned {vt_type}";
                    }
                }
                else
                {
                    _log.Debug($"JSON: {response.Content}");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Exception uploading file");
                return ex.Message;
            }
        }
        return "Unexpected error. Check the log file.";
    }
    #endregion Send file to VirusTotal
}
