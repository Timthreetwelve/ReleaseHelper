# ReleaseHelper

Release Helper is an app that I put together to help me get some of the information about the files (exe installer or zip archive) that I plan to release here on GitHub. The goal was to have information such as file size and SHA256 hash in one place and make it easy to copy and paste into release notes.

Release Helper runs on Windows 10 build 19041 or greater and needs .NET 8 which can be downloaded [here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

### Features
- Simple user interface.
- Accepts file name via open file dialog, drag & drop, command line parameter, pasting the file name, or by typing.
- Calculates SHA265 hash for the input file.
- Calculates size to two decimal places.
- Shows file name without path.
- Easily copy the above three to the clipboard.
- Optionally submit the installer or zip archive to VirusTotal for analysis.
  - Submission to VirusTotal requires a VirusTotal account and API key.
  - Check results on [VirusTotal.com](https://www.virustotal.com/gui/home/) using the generated hash.
- While there isn't a Settings page, the Settings file can be accessed via the three-dot menu.

### Screenshots

![ReleaseHelper_2024-06-08_20-27-43](https://github.com/Timthreetwelve/ReleaseHelper/assets/43152358/6fc4f48a-daae-474f-8db8-e44a9ffe6c1d)

![ReleaseHelper_2024-06-08_20-26-16](https://github.com/Timthreetwelve/ReleaseHelper/assets/43152358/1402f6fc-d237-4a74-b933-dad553a33e82)
