# YouTube Downloader VB.NET

A powerful, feature-rich YouTube video and audio downloader built with VB.NET featuring colorful ASCII art, interactive menus, and command-line support.

## 🌟 Features

- **Video Downloads**: Download videos in MP4 format with quality options (Highest, Medium, Lowest)
- **Audio Extraction**: Extract audio as MP3 files
- **Batch Downloads**: Download multiple videos from a URL list file
- **Command-line Support**: Pass URLs and options directly via command line
- **Interactive Menu**: Beautiful colorful console interface with ASCII art
- **Download History**: Track your download history
- **Cross-platform**: Built on .NET 6.0

## 🚀 Installation & Setup

### Prerequisites
- .NET 6.0 Runtime or SDK
- FFmpeg (automatically handled by FFMpegCore package)

### Build Instructions

1. **Clone or download the project files**
2. **Open PowerShell/Command Prompt in the project directory**
3. **Restore NuGet packages:**
   ```powershell
   dotnet restore
   ```
4. **Build the project:**
   ```powershell
   dotnet build
   ```
5. **Run the application:**
   ```powershell
   dotnet run
   ```

### Create Executable
To create a standalone executable:
```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## 💻 Usage

### Interactive Mode
Simply run the application without arguments:
```powershell
dotnet run
```
or
```powershell
YouTubeDownloader.exe
```

### Command Line Mode
```powershell
dotnet run -- -u "https://youtube.com/watch?v=VIDEO_ID" -o "Downloads" -q highest -f mp4
```

### Command Line Arguments

| Argument | Short | Description | Default |
|----------|-------|-------------|---------|
| `--url` | `-u` | YouTube video URL to download | (required in CLI mode) |
| `--output` | `-o` | Output directory path | "Downloads" |
| `--quality` | `-q` | Video quality (highest, medium, lowest) | "highest" |
| `--format` | `-f` | Output format (mp4, mp3) | "mp4" |
| `--help` | `-h` | Show help information | - |

### Examples

**Download video in highest quality:**
```powershell
YouTubeDownloader.exe -u "https://youtube.com/watch?v=dQw4w9WgXcQ" -q highest
```

**Download audio only:**
```powershell
YouTubeDownloader.exe -u "https://youtube.com/watch?v=dQw4w9WgXcQ" -f mp3
```

**Download to specific directory:**
```powershell
YouTubeDownloader.exe -u "https://youtube.com/watch?v=dQw4w9WgXcQ" -o "C:\MyVideos"
```

**Batch download:**
Create a text file (e.g., `urls.txt`) with YouTube URLs (one per line):
```
https://youtube.com/watch?v=VIDEO_ID_1
https://youtube.com/watch?v=VIDEO_ID_2
https://youtube.com/watch?v=VIDEO_ID_3
```

Then use the interactive menu option 3 to batch download.

## 🎨 Features Overview

### Main Menu Options
1. **Download Video (MP4)** - Download videos in MP4 format
2. **Download Audio Only (MP3)** - Extract audio as MP3
3. **Batch Download from File** - Download multiple videos from a URL list
4. **Show Download History** - View your download history
5. **Settings & Configuration** - Configuration options
6. **About & Help** - Application information and help
7. **Exit** - Exit the application

### Quality Options
- **Highest**: Best available quality (recommended)
- **Medium**: Balanced quality and file size
- **Lowest**: Fastest download, smallest file size

## 📦 Dependencies

The project uses the following NuGet packages:
- `YoutubeExplode` (6.3.7) - YouTube video information and stream access
- `YoutubeExplode.Converter` (6.3.7) - Video/audio conversion capabilities
- `FFMpegCore` (5.0.2) - Video processing and conversion
- `Colorful.Console` (1.2.15) - Console text coloring
- `CommandLineParser` (2.9.1) - Command-line argument parsing

## 🛠️ Technical Details

- **Framework**: .NET 6.0
- **Language**: VB.NET
- **Architecture**: Console Application
- **Output Types**: MP4 (video), MP3 (audio)
- **Platform**: Cross-platform (Windows, Linux, macOS)

## 📁 Project Structure

```
YouTubeDownloader/
├── YouTubeDownloader.vbproj    # Project file
├── Program.vb                   # Main application code
├── README.md                    # This file
└── Downloads/                   # Default output directory (created automatically)
```

## 🐛 Troubleshooting

### Common Issues

1. **"FFmpeg not found" error**
   - The FFMpegCore package should handle this automatically
   - If issues persist, install FFmpeg manually

2. **Network/Download errors**
   - Check your internet connection
   - Ensure the YouTube URL is valid and the video is accessible
   - Some videos may be geo-blocked or require authentication

3. **Permission errors**
   - Ensure you have write permissions to the output directory
   - Run as administrator if necessary

4. **Package restore errors**
   - Clear NuGet cache: `dotnet nuget locals all --clear`
   - Restore packages again: `dotnet restore`

## 📄 License

This project is for educational purposes. Please respect YouTube's Terms of Service and copyright laws when downloading content.

## 🤝 Contributing

Feel free to submit issues, feature requests, or pull requests to improve the application.

## ⚠️ Disclaimer

This tool is for educational and personal use only. Users are responsible for complying with YouTube's Terms of Service and applicable copyright laws. The developers are not responsible for any misuse of this tool.
