# 🎬 YouTube Downloader - GUI Version

A modern, user-friendly Windows GUI application for downloading YouTube videos and audio. Built with VB.NET and Windows Forms, featuring an intuitive interface with video preview, format selection, and download progress tracking.

![Screenshot](https://img.shields.io/badge/Platform-Windows-blue)
![.NET](https://img.shields.io/badge/.NET-6.0-purple)
![License](https://img.shields.io/badge/License-Open%20Source-green)

## ✨ Key Features

### 🖥️ **Modern GUI Interface**
- Clean, dark-themed Windows Forms interface
- Real-time video information display
- Thumbnail preview with video details
- Progress bar with status updates
- Professional styling with smooth user experience

### 📺 **Advanced Video Analysis**
- **URL Analysis**: Paste any YouTube URL and get instant video information
- **Format Detection**: Automatically detects all available video/audio formats
- **Quality Options**: Choose from multiple resolutions and file sizes
- **Format Types**: 
  - Video + Audio (MP4, WebM)
  - Video Only (for custom editing)
  - Audio Only (MP3 equivalent)

### 💾 **Smart Download Management**
- **Custom Output Location**: Choose where to save your downloads
- **Download History**: Track all downloads with searchable history
- **File Management**: Open files or folders directly from the app
- **Progress Tracking**: Real-time download progress with speed indicators

### 🎯 **User-Friendly Features**
- **One-Click Analysis**: Simply paste URL and click "Analyze"
- **Format Preview**: See file sizes before downloading
- **Thumbnail Display**: Preview video thumbnail while selecting format
- **Status Updates**: Clear status messages throughout the process
- **Error Handling**: Helpful error messages with suggested solutions

## 🚀 Quick Start Guide

### Option 1: Use Pre-built Executable (Recommended)
1. **Download** `YouTubeDownloaderGUI.exe` from the ReleaseGUI folder (155 MB)
2. **Double-click** to run - no installation required!
3. **Windows Defender**: May show security warning - click "More info" → "Run anyway"

### Option 2: Build from Source
```bash
git clone https://github.com/catozan/yt-downloader-standalone.git
cd "yt-downloader-standalone"
dotnet build YouTubeDownloaderGUI.vbproj
dotnet run --project YouTubeDownloaderGUI.vbproj
```

## 📱 How to Use

### Step 1: Enter YouTube URL
- Copy any YouTube video URL
- Paste it in the "YouTube URL" field
- Press Enter or click "Analyze Video"

### Step 2: Review Video Information
- Video title, author, duration, and view count will display
- Thumbnail image appears on the right
- All available formats populate in the list

### Step 3: Choose Format & Quality
- **[VIDEO + AUDIO]**: Complete video files ready to watch
- **[VIDEO ONLY]**: Video without audio (for editing)
- **[AUDIO ONLY]**: Extract audio only (MP3-like quality)
- Each format shows quality, container type, and file size

### Step 4: Set Download Location
- Default: Your Videos folder → "YouTube Downloads"
- Click "Browse" to choose a different location
- Path is remembered for future downloads

### Step 5: Download
- Click "Download" button
- Watch real-time progress bar
- Get completion notification
- Option to open download folder automatically

## 🎨 Interface Overview

```
╔══════════════════════════════════════════════════════════════════════╗
║  🎬 YouTube Video Downloader                                         ║
╠══════════════════════════════════════════════════════════════════════╣
║  YouTube URL: [___________________________] [Analyze Video]          ║
║                                                                      ║
║  ┌─Video Info─────────────────────┐  ┌─Thumbnail─┐                   ║
║  │ Title: Amazing Video           │  │           │                   ║
║  │ Author: Creator Name           │  │  [IMG]    │                   ║
║  │ Duration: 05:23                │  │           │                   ║
║  │ Views: 1,234,567               │  └───────────┘                   ║
║  └────────────────────────────────┘                                  ║
║                                                                      ║
║  Available Formats & Quality:                                        ║
║  ┌────────────────────────────────────────────────────────────────┐  ║
║  │ [VIDEO + AUDIO] 1080p (mp4) - 45.2 MB                        │  ║
║  │ [VIDEO + AUDIO] 720p (mp4) - 28.1 MB                         │  ║
║  │ [VIDEO ONLY] 1080p (mp4) - 38.7 MB                           │  ║
║  │ [AUDIO ONLY] 128kbps (webm) - 6.8 MB                         │  ║
║  └────────────────────────────────────────────────────────────────┘  ║
║                                                                      ║
║  Download Location: [C:\Users\...\YouTube Downloads] [Browse] [Set]  ║
║                                                                      ║
║  [████████████████████████████████████] 100%                        ║
║  Status: Download completed successfully!                            ║
║                                           [History] [Download]       ║
╚══════════════════════════════════════════════════════════════════════╝
```

## 🗂️ Download History

Access your complete download history:
- **View History**: Click "History" button to see all downloads
- **Open Files**: Double-click entries to open downloaded files
- **Open Folders**: Right-click to open containing folder
- **Clear History**: Remove all history entries
- **JSON Storage**: History saved in `download_history.json`

## ⚙️ Technical Specifications

### System Requirements
- **OS**: Windows 7/8/10/11 (x64)
- **RAM**: 2 GB minimum, 4 GB recommended
- **Storage**: 200 MB for application + download space
- **Internet**: Required for video analysis and downloading

### Technologies Used
- **Framework**: .NET 6.0 Windows Forms
- **Language**: VB.NET
- **YouTube API**: YoutubeExplode library
- **JSON**: Newtonsoft.Json for data storage
- **HTTP**: Modern HttpClient for downloads

### File Information
- **Executable Size**: ~155 MB (self-contained)
- **Dependencies**: All included in single executable
- **Installation**: Not required - portable application
- **Updates**: Manual download of new versions

## 📂 Project Structure

```
YouTubeDownloader-GUI/
├── YouTubeDownloaderGUI.vbproj    # Project configuration
├── ProgramGUI.vb                  # Application entry point
├── MainForm.vb                    # Main GUI interface
├── HistoryForm.vb                 # Download history window
├── ReleaseGUI/                    # Standalone executable
│   └── YouTubeDownloaderGUI.exe   # Ready-to-run application
└── README_GUI.md                  # This documentation
```

## 🔧 Building Standalone Executable

To create your own portable executable:

```bash
# Navigate to project directory
cd "YT Downloader"

# Build release version
dotnet publish YouTubeDownloaderGUI.vbproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o "./ReleaseGUI"

# Result: YouTubeDownloaderGUI.exe (155 MB)
```

## 🚨 Troubleshooting

### Common Issues

**"Windows protected your PC" warning**
- Click "More info" → "Run anyway"
- This is normal for unsigned executables

**Application won't start**
- Ensure you're running Windows 7 or newer
- Check if antivirus is blocking the file
- Try running as administrator

**Download fails**
- Check internet connection
- Verify YouTube URL is valid and accessible
- Some videos may be region-blocked or private

**No formats available**
- Video might be age-restricted
- Check if URL is correct
- Try different video URL

**Permission denied when downloading**
- Choose different download folder
- Run application as administrator
- Check disk space availability

## 🛡️ Privacy & Security

- **No Data Collection**: App doesn't collect or send personal data
- **Local Storage**: All downloads and history stored locally
- **No Telemetry**: No usage tracking or analytics
- **Open Source**: Code available for security review

## 📜 Legal Notice

This application is for **educational and personal use only**. Users must:
- Comply with YouTube's Terms of Service
- Respect copyright laws and content creators' rights
- Use downloaded content responsibly
- Not redistribute copyrighted material

The developers are not responsible for any misuse of this tool.

## 🤝 Support & Contributing

- **Issues**: Report bugs or request features via GitHub issues
- **Contributing**: Pull requests welcome for improvements
- **Documentation**: Help improve this guide
- **Testing**: Test on different Windows versions

## 🎉 Version History

### v1.0.0 (Current)
- ✅ Modern Windows Forms GUI
- ✅ Real-time video analysis
- ✅ Multiple format support
- ✅ Download progress tracking
- ✅ History management
- ✅ Thumbnail preview
- ✅ Standalone executable
- ✅ Professional dark theme interface

---

**Made with ❤️ by Shalan Kareem**

🔗 **[GitHub Repository](https://github.com/catozan/yt-downloader-standalone)** | 🎬 **[Download Latest Release](https://github.com/catozan/yt-downloader-standalone/releases)**
