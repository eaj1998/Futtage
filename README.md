# Futtage — Video Editor and Uploader for YouTube (Windows, .NET 8)

Version: 2.0.0

Futtage is a Windows desktop application that lets you concatenate, trim, and upload videos directly to YouTube. It provides a clean, modern UI and integrates with the YouTube Data API for a streamlined creator workflow.

- **Platform**: Windows 10+ (x64)
- **Framework**: .NET 8.0

## Key Features

- **Video Concatenation**: Merge multiple videos into a single output
- **Video Trimming**: Cut videos to precise time ranges
- **YouTube Integration**: Upload with title, description, privacy, and audience settings
- **Modern UI**: Progress tracking and straightforward controls
- **Secure Authentication**: Google OAuth 2.0
- **Thumbnail Support**: Auto or custom thumbnail selection
- **Robust Logging**: Error handling and logs for diagnostics
- **Multiple Formats**: MP4, AVI, MOV, MKV, WMV, FLV, WEBM

## Project Structure

```
Futtage/
├─ Core/                          # Business logic
│  ├─ Models/                     # Data models
│  │  ├─ VideoInfo.cs
│  │  ├─ YoutubeUploadRequest.cs
│  │  ├─ UserInfo.cs
│  │  └─ ProcessingProgress.cs
│  └─ Services/                   # Core services
│     ├─ IVideoProcessingService.cs
│     ├─ VideoProcessingService.cs
│     ├─ IYoutubeService.cs
│     ├─ FuttageYouTubeService.cs
│     ├─ IFileService.cs
│     └─ FileService.cs
├─ Infrastructure/                # Configuration, DI, logging
├─ Presentation/                  # UI (views, presenters, common components)
├─ Resources/                     # App resources
├─ Program.cs                     # Entry point
├─ appsettings.json               # Configuration
└─ ffmpeg.exe                     # Bundled FFmpeg binary
```

## Getting Started

### Prerequisites

- Windows 10 64-bit or newer
- .NET 8.0 Runtime (or SDK for building from source)
- Internet connection for YouTube uploads
- A Google account with access to YouTube

### Download & Run

- Download the latest release from the repository’s Releases page.
- Ensure `ffmpeg.exe` exists in the application directory (root of the app). If missing, see Troubleshooting.
- Double-click `Futtage.exe` to launch.

### Build From Source

1. Install the [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download)
2. Clone the repository
3. Place `ffmpeg.exe` in the project root
4. Configure `appsettings.json` with your credentials (see Configuration)
5. Build using `dotnet build` or Visual Studio

## Usage

1. **Select and order videos**
   - Click “Select Files…” and choose multiple videos (e.g., MP4)
   - Reorder with arrow buttons; remove with the X button
   - Sign in with Google to enable YouTube upload
2. **Concatenate**
   - Review your selection and click “Join Videos”
   - Choose the output location and wait for processing
3. **Trim (optional)**
   - Enter start and end times in `HH:MM:SS`
   - Click “Cut” to trim or “Skip Cut” to continue without trimming
4. **Thumbnail (optional)**
   - A default thumbnail is generated automatically
   - Click “Change Default Cover…” to select a custom image (JPG/PNG/BMP)
5. **Upload to YouTube**
   - Fill in: Title, Description, Privacy (Private/Unlisted/Public), Audience (Made for kids)
   - Click “Upload to YouTube” and wait for completion

## Configuration

You can configure via `appsettings.json` or environment variables.

### appsettings.json

```json
{
  "YouTube": {
    "ClientId": "your-google-client-id",
    "ClientSecret": "your-google-client-secret",
    "ApplicationName": "Futtage"
  },
  "Video": {
    "DefaultQuality": "copy",
    "DeleteTempFiles": true
  },
  "UI": {
    "Theme": "Light",
    "ShowTooltips": true,
    "EnableAnimations": true
  }
}
```

### Google Cloud setup

1. Open the [Google Cloud Console](https://console.cloud.google.com/)
2. Create or select a project
3. Enable the “YouTube Data API v3”
4. Create OAuth 2.0 credentials (Desktop Application)
5. Copy the Client ID and Client Secret into configuration

## Supported Formats

- **Input video**: MP4 (H.264/H.265), AVI, MOV, MKV, WMV, FLV, WEBM
- **Output video**: MP4 (H.264), optimized for YouTube
- **Thumbnails**: JPEG/JPG, PNG, BMP, WEBP

## Troubleshooting

- **FFmpeg not found**
  - Ensure `ffmpeg.exe` is in the application folder
  - Download FFmpeg from [`https://www.gyan.dev/ffmpeg/builds/`](https://www.gyan.dev/ffmpeg/builds/)
  - Place `ffmpeg.exe` next to `Futtage.exe`
- **YouTube authentication failed**
  - Verify Client ID/Secret in `appsettings.json`
  - Confirm YouTube Data API v3 is enabled
  - Ensure OAuth consent screen is configured
  - Try signing out and back in
- **Video processing errors** (concat/trim)
  - Check inputs are not corrupted and codecs are compatible
  - Ensure sufficient disk space
  - Try smaller batches
- **Upload failures/timeouts**
  - Check internet connection and available YouTube quota
  - Verify file size limits (YouTube up to 256GB or 12 hours)
  - Try off-peak hours

### Logs

Application logs: `%LOCALAPPDATA%\Futtage\Logs\`

## Privacy & Security

- **Local processing**: All video operations run on your machine
- **Secure auth**: Google OAuth 2.0 is used for sign-in
- **No data collection**: The app does not collect personal data
- **Temporary files**: Cleaned after processing
- **Credential storage**: Uses Windows Credential Manager

## Roadmap

- Potential command-line support for batch workflows
- Expanded presets and encoding options

## Support & Contributing

- **Repository**: [`https://github.com/eaj1998/futtage`](https://github.com/eaj1998/futtage)
- **Issues**: Please report bugs via GitHub Issues
- **Email**: `edipo1998@gmail.com`

Contributions are welcome:

1. Fork the repository
2. Create a feature branch
3. Open a pull request with a clear description

## License

Copyright © 2025 EAJ.

This software is provided “as is” without warranty of any kind. Use at your own risk.


