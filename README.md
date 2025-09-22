FUTTAGE - Video Editor for YouTube

Version: 2.0.0
Platform: Windows (.NET 8.0)
📋 About

Futtage is a desktop application for Windows that allows you to concatenate, trim, and upload videos directly to YouTube. Built in C# with a modern interface and complete integration with the YouTube API.
✨ Key Features

    Video Concatenation: Merge multiple MP4 videos into a single file
    Video Trimming: Cut videos to specific time ranges
    YouTube Integration: Direct upload to YouTube with metadata
    Modern UI: Clean interface with progress tracking
    Authentication: Secure Google OAuth2 integration
    Thumbnail Support: Custom thumbnail selection for uploads
    Error Handling: Robust error management and logging
    Multiple Formats: Support for MP4, AVI, MOV, MKV, WMV, FLV

🏗️ Project Structure

Futtage/
├── Core/                          # Business logic layer
│   ├── Models/                    # Data models
│   │   ├── VideoInfo.cs          # Video information
│   │   ├── YoutubeUploadRequest.cs # Upload request model
│   │   ├── UserInfo.cs           # User data
│   │   └── ProcessingProgress.cs # Processing progress tracking
│   └── Services/                  # Core services
│       ├── IVideoProcessingService.cs # Video processing interface
│       ├── VideoProcessingService.cs  # Video processing implementation
│       ├── IYoutubeService.cs         # YouTube service interface
│       ├── FuttageYouTubeService.cs   # YouTube service implementation
│       ├── IFileService.cs            # File service interface
│       └── FileService.cs             # File service implementation
├── Infrastructure/                # Infrastructure layer
│   ├── Configuration/             # App configuration
│   ├── Extensions/                # Dependency injection extensions
│   └── Logging/                   # Logging system
├── Presentation/                  # User interface layer
│   ├── Views/                     # Forms and dialogs
│   ├── Presenters/                # MVP pattern presenters
│   └── Common/                    # Shared UI components
├── Resources/                     # Application resources
├── Program.cs                     # Application entry point
├── appsettings.json              # Configuration file
└── ffmpeg.exe                    # FFmpeg binary for video processing

🚀 How to Use
Step 1: Select and Order Videos

    Click "Select Files..." to choose multiple MP4 videos
    Use the arrow buttons to reorder videos as needed
    Remove unwanted videos with the X button
    Login with your Google account for YouTube access

Step 2: Concatenate Videos

    Review your selected videos
    Click "Join Videos" and choose output location
    Wait for processing to complete (progress bar will show status)

Step 3: Trim Video (Optional)

    Set start time in HH:MM:SS format
    Set end time in HH:MM:SS format
    Click "Cut" to trim the video, or "Skip Cut" to proceed

Step 4: Select Thumbnail (Optional)

    A default thumbnail is automatically generated
    Click "Change Default Cover..." to select a custom image
    Supported formats: JPG, PNG, BMP
    Click "Next Step" to continue

Step 5: Upload to YouTube

    Fill in video details:
        Title: Your video title (up to 100 characters)
        Description: Video description (up to 5000 characters)
        Privacy: Choose Private, Unlisted, or Public
        Child-friendly content: Check if appropriate
    Click "Upload to YouTube"
    Wait for upload completion

⚙️ Configuration
appsettings.json Setup

Create or modify the appsettings.json file in the application directory:
json

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

Environment Variables (Alternative)

You can also configure YouTube credentials via environment variables:

    FUTTAGE_YOUTUBE_CLIENTID: Your Google Client ID
    FUTTAGE_YOUTUBE_CLIENTSECRET: Your Google Client Secret

Google API Setup

    Go to Google Cloud Console
    Create a new project or select existing one
    Enable the YouTube Data API v3
    Create OAuth 2.0 credentials (Desktop Application)
    Add your Client ID and Secret to the configuration

🔧 System Requirements

    Operating System: Windows 10 64-bit or higher
    Framework: .NET 8.0 Runtime
    Memory: 4 GB RAM minimum, 8 GB recommended
    Storage: 1 GB free space (more for video processing)
    FFmpeg: Included with the application
    Internet: Required for YouTube uploads
    Google Account: Valid YouTube account for uploads

📁 Supported Formats
Input Video Formats

    MP4 (H.264/H.265)
    AVI
    MOV
    MKV
    WMV
    FLV
    WEBM

Output Format

    MP4 (H.264) - optimized for YouTube

Thumbnail Formats

    JPEG/JPG
    PNG
    BMP
    WEBP

🐛 Troubleshooting
FFmpeg Not Found

Problem: Video processing fails with FFmpeg error Solution:

    Ensure ffmpeg.exe is in the application folder
    Download FFmpeg from: https://www.gyan.dev/ffmpeg/builds/
    Extract ffmpeg.exe to the same folder as Futtage.exe

YouTube Authentication Failed

Problem: Cannot login to Google/YouTube Solutions:

    Verify your Client ID and Secret in appsettings.json
    Check that YouTube Data API v3 is enabled in Google Cloud Console
    Ensure OAuth consent screen is properly configured
    Try logging out and logging back in

Video Processing Errors

Problem: Concatenation or trimming fails Solutions:

    Verify all input videos are not corrupted
    Ensure sufficient disk space is available
    Check that all videos have compatible codecs
    Try processing smaller batches of videos

Upload Failures

Problem: YouTube upload fails or times out Solutions:

    Check your internet connection stability
    Verify video file size (YouTube limit is 256GB or 12 hours)
    Ensure you have sufficient upload quota
    Try uploading during off-peak hours

Log Files

Application logs are stored in: %LOCALAPPDATA%\Futtage\Logs\
🔒 Privacy & Security

    Local Processing: All video processing happens locally on your machine
    Secure Authentication: Uses Google's OAuth 2.0 for secure login
    No Data Collection: Futtage doesn't collect or store personal data
    Temporary Files: Automatically cleaned up after processing
    Credentials: Stored securely using Windows Credential Manager

🚀 Advanced Usage
Command Line Arguments

Currently, Futtage is designed as a GUI application, but future versions may support command-line operations.
Batch Processing

    Select multiple videos at once for efficient processing
    Videos are processed in the order you arrange them
    Use the reorder buttons to change sequence

Custom Thumbnails

    Thumbnails are automatically generated from the first frame
    Custom thumbnails should be 1280x720 pixels for best quality
    JPG format is recommended for smaller file sizes

📞 Support & Contributing
Getting Help

    GitHub Repository: https://github.com/eaj1998/futtage
    Issues: Report bugs via GitHub Issues
    Email: edipo1998@gmail.com

Contributing

Contributions are welcome! Please:

    Fork the repository
    Create a feature branch
    Submit a pull request with detailed description

Building from Source

    Install .NET 8.0 SDK
    Clone the repository
    Download FFmpeg and place in project root
    Configure appsettings.json with your credentials
    Build with dotnet build or Visual Studio

📝 License

Copyright © 2025 EAJ. All rights reserved.

This software is provided "as is" without warranty. Use at your own risk.
