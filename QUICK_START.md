# YouTube Downloader - Quick Start Guide

## 🚀 How to Run the Application

### Method 1: Using dotnet run (Development)
```powershell
dotnet run
```

### Method 2: Using the batch file
```batch
run.bat
```

### Method 3: Using the published executable
```powershell
bin\Release\net6.0\win-x64\publish\YouTubeDownloader.exe
```

## 📝 Command Line Examples

### Download a video directly (via dotnet run):
```powershell
dotnet run -- -u "https://youtube.com/watch?v=dQw4w9WgXcQ" -o "Downloads" -q highest -f mp4
```

### Download audio only:
```powershell
dotnet run -- -u "https://youtube.com/watch?v=dQw4w9WgXcQ" -o "Downloads" -f mp3
```

### Using the published executable:
```powershell
bin\Release\net6.0\win-x64\publish\YouTubeDownloader.exe -u "https://youtube.com/watch?v=dQw4w9WgXcQ" -o "C:\Downloads" -q highest -f mp4
```

## 🎮 Interactive Mode Features

When you run the application without arguments, you get a beautiful colorful menu with options:

1. **Download Video (MP4)** - Interactive video download with quality selection
2. **Download Audio Only (MP3)** - Extract audio from YouTube videos
3. **Batch Download from File** - Use `sample_urls.txt` or create your own URL list
4. **Show Download History** - View previously downloaded content
5. **Settings & Configuration** - Configuration options
6. **About & Help** - Information about the application
7. **Exit** - Exit the application

## 📁 Files Created

- `YouTubeDownloader.vbproj` - Project file with all dependencies
- `Program.vb` - Main application source code
- `README.md` - Comprehensive documentation
- `run.bat` - Batch file to easily run the application
- `sample_urls.txt` - Sample YouTube URLs for testing batch download
- `Downloads/` - Default output directory (created automatically)
- `download_history.txt` - Download history log (created after first download)

## 🔧 Required Libraries (Automatically Handled)

- YoutubeExplode (6.3.7) - YouTube video information and downloading
- YoutubeExplode.Converter (6.3.7) - Video/audio conversion
- FFMpegCore (5.0.2) - Media processing
- Colorful.Console (1.2.15) - Colorful console output
- CommandLineParser (2.9.1) - Command-line argument parsing

## 🎨 Features Showcase

- ✨ Beautiful colorful ASCII art banner
- 🎬 Video download in multiple qualities
- 🎵 Audio extraction to MP3
- 📋 Batch download from URL lists
- 📊 Progress indicators and status messages
- 📜 Download history tracking
- ⚙️ Interactive settings menu
- 🎨 Rich console colors and emojis
- 📱 Command-line interface support

## 🧪 Testing the Application

1. Run `dotnet run` to start the interactive mode
2. Try option 6 (About & Help) to see the feature list
3. Use the sample URLs in `sample_urls.txt` for batch testing
4. Test command-line mode with the examples above

The application is fully functional and ready to use! 🎉
