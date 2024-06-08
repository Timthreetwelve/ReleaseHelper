// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace ReleaseHelper.Helpers
{
    public static class FormatHelpers
    {
        /// <summary>
        /// Formats a file size.
        /// </summary>
        /// <param name="fileInfo">FileInfo of the file to be checked.</param>
        /// <returns>Formatted string.</returns>
        public static string GetFileSize(FileInfo fileInfo)
        {
            if (File.Exists(fileInfo.FullName))
            {
                // Get the file size in bytes
                long fileSizeBytes = fileInfo.Length;
                string[] suffixes = ["Bytes", "KB", "MB", "GB", "TB", "PB"];
                int counter = 0;
                double fileSize = fileSizeBytes;
                // Convert to higher units as necessary
                while (fileSize >= 1024 && counter < suffixes.Length - 1)
                {
                    fileSize /= 1024;
                    counter++;
                }
                // Return the file size with the appropriate unit
                return counter > 0 ? $"{fileSize:n2} {suffixes[counter]}" : $"{fileSize} {suffixes[counter]}";
            }
            else
            {
                return "File does not exist.";
            }
        }
    }
}
