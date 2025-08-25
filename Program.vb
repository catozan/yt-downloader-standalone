Imports System
Imports System.IO
Imports System.Threading.Tasks
Imports System.Drawing
Imports System.Net.Http
Imports System.Text.RegularExpressions
Imports System.Diagnostics
Imports Colorful
Imports CommandLine
Imports YoutubeExplode
Imports YoutubeExplode.Videos
Imports YoutubeExplode.Videos.Streams

Module Program
    ' Command line options
    Public Class Options
        <[Option]("u"c, "url", Required:=False, HelpText:="YouTube video URL to download")>
        Public Property Url As String

        <[Option]("o"c, "output", Required:=False, HelpText:="Output directory path", Default:="Downloads")>
        Public Property OutputPath As String

        <[Option]("q"c, "quality", Required:=False, HelpText:="Video quality (highest, medium, lowest)", Default:="highest")>
        Public Property Quality As String

        <[Option]("f"c, "format", Required:=False, HelpText:="Output format (mp4, mp3)", Default:="mp4")>
        Public Property Format As String

        <[Option]("p"c, "proxy", Required:=False, HelpText:="Use proxy server for download attempts")>
        Public Property UseProxy As Boolean

        <[Option]("r"c, "retry", Required:=False, HelpText:="Number of retry attempts", Default:=3)>
        Public Property RetryCount As Integer

        <[Option]("d"c, "delay", Required:=False, HelpText:="Delay between requests in seconds", Default:=2)>
        Public Property DelaySeconds As Integer

        <[Option]("h"c, "help", Required:=False, HelpText:="Show help information")>
        Public Property ShowHelp As Boolean
    End Class

    Sub Main(args As String())
        Try
            ' Parse command line arguments
            Dim result = Parser.Default.ParseArguments(Of Options)(args)
            
            result.WithParsed(
                Sub(opts)
                    RunApplication(opts).GetAwaiter().GetResult()
                End Sub) _
                .WithNotParsed(
                Sub(errors)
                    ' Only show interactive menu, don't double display header
                    ShowInteractiveMenu()
                End Sub)

        Catch ex As Exception
            System.Console.ForegroundColor = ConsoleColor.Red
            System.Console.WriteLine($"Fatal error: {ex.Message}")
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.WriteLine("Press any key to exit...")
            System.Console.ResetColor()
            System.Console.ReadKey()
        End Try
    End Sub

    Private Async Function RunApplication(opts As Options) As Task
        DisplayASCIIArt()
        
        If Not String.IsNullOrEmpty(opts.Url) Then
            ' Direct URL download mode
            Await DownloadVideo(opts.Url, opts.OutputPath, opts.Quality, opts.Format)
        Else
            ' Interactive mode
            ShowInteractiveMenu()
        End If
    End Function

    Private Sub DisplayASCIIArt()
        System.Console.Clear()
        
        Dim art As String() = {
            "╔═══════════════════════════════════════════════════════════════════════════════╗",
            "║  ██╗   ██╗████████╗    ██████╗  ██████╗ ██╗    ██╗███╗   ██╗██╗      ██████╗  ║",
            "║  ╚██╗ ██╔╝╚══██╔══╝    ██╔══██╗██╔═══██╗██║    ██║████╗  ██║██║     ██╔═══██╗ ║",
            "║   ╚████╔╝    ██║       ██║  ██║██║   ██║██║ █╗ ██║██╔██╗ ██║██║     ██║   ██║ ║",
            "║    ╚██╔╝     ██║       ██║  ██║██║   ██║██║███╗██║██║╚██╗██║██║     ██║   ██║ ║",
            "║     ██║      ██║       ██████╔╝╚██████╔╝╚███╔███╔╝██║ ╚████║███████╗╚██████╔╝ ║",
            "║     ╚═╝      ╚═╝       ╚═════╝  ╚═════╝  ╚══╝╚══╝ ╚═╝  ╚═══╝╚══════╝ ╚═════╝  ║",
            "║                                                                               ║",
            "║                     YouTube Video & Audio Downloader                          ║",
            "║                      Fast - Reliable - Feature-Rich                           ║",
            "╚═══════════════════════════════════════════════════════════════════════════════╝"
        }
        
        ' Gemini CLI inspired gradient using ConsoleColor for better compatibility
        Dim consoleColors As ConsoleColor() = {
            ConsoleColor.Cyan,        ' Light Blue
            ConsoleColor.Blue,        ' Blue  
            ConsoleColor.DarkBlue,    ' Dark Blue
            ConsoleColor.DarkMagenta, ' Dark Magenta
            ConsoleColor.Magenta,     ' Magenta
            ConsoleColor.Red,         ' Red
            ConsoleColor.DarkRed,     ' Dark Red
            ConsoleColor.Yellow,      ' Yellow
            ConsoleColor.White,       ' White
            ConsoleColor.Gray         ' Gray
        }
        
        For i As Integer = 0 To art.Length - 1
            System.Console.ForegroundColor = consoleColors(i Mod consoleColors.Length)
            System.Console.WriteLine(art(i))
            Threading.Thread.Sleep(80)
        Next
        
        System.Console.ResetColor()
        System.Console.WriteLine()
        System.Console.ForegroundColor = ConsoleColor.Magenta
        System.Console.WriteLine("                           Made with <3 by Shalan Kareem")
        System.Console.ResetColor()
        System.Console.WriteLine()
    End Sub

    Private Sub ShowInteractiveMenu()
        While True
            System.Console.Clear()
            
            ' Display header
            DisplayASCIIArt()
            System.Console.WriteLine()
            
            ' Display menu
            System.Console.ForegroundColor = ConsoleColor.Blue
            System.Console.WriteLine("╔══════════════════════════════════════════╗")
            System.Console.ForegroundColor = ConsoleColor.DarkMagenta
            System.Console.WriteLine("║              MAIN MENU                   ║")
            System.Console.ForegroundColor = ConsoleColor.Blue
            System.Console.WriteLine("╠══════════════════════════════════════════╣")
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine("║  1. Download Video (MP4)                 ║")
            System.Console.WriteLine("║  2. Download Audio Only (MP3)            ║")
            System.Console.WriteLine("║  3. Batch Download from File             ║")
            System.Console.WriteLine("║  4. Show Download History                ║")
            System.Console.WriteLine("║  5. Advanced Download Options            ║")
            System.Console.WriteLine("║  6. About & Help                         ║")
            System.Console.WriteLine("║  7. Exit                                 ║")
            System.Console.ForegroundColor = ConsoleColor.Blue
            System.Console.WriteLine("╚══════════════════════════════════════════╝")
            System.Console.ResetColor()
            System.Console.WriteLine()
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.Write("Enter your choice (1-7): ")
            System.Console.ResetColor()
            
            Dim choice As String = System.Console.ReadLine()
            
            Select Case choice
                Case "1"
                    HandleVideoDownload()
                Case "2"
                    HandleAudioDownload()
                Case "3"
                    HandleBatchDownload()
                Case "4"
                    ShowDownloadHistory()
                Case "5"
                    ShowAdvancedOptions()
                Case "6"
                    ShowAbout()
                Case "7"
                    ShowExitMessage()
                    Return
                Case Else
                    System.Console.ForegroundColor = ConsoleColor.Red
                    System.Console.WriteLine("[X] Invalid choice! Please select 1-7.")
                    System.Console.ResetColor()
                    Threading.Thread.Sleep(1500)
            End Select
            
            System.Console.Clear()
            DisplayASCIIArt()
        End While
    End Sub

    Private Sub HandleVideoDownload()
        System.Console.Clear()
        Colorful.Console.WriteLine("🎬 VIDEO DOWNLOAD MODE", Color.FromArgb(138, 43, 226))
        Colorful.Console.WriteLine("═══════════════════════", Color.FromArgb(138, 43, 226))
        System.Console.WriteLine()
        
        Colorful.Console.Write("Enter YouTube URL: ", Color.FromArgb(255, 182, 193))
        Dim url As String = System.Console.ReadLine()
        
        If String.IsNullOrEmpty(url) Then
            Colorful.Console.WriteLine("❌ URL cannot be empty!", Color.FromArgb(255, 105, 180))
            Threading.Thread.Sleep(2000)
            Return
        End If
        
        Colorful.Console.Write("Enter output directory (or press Enter for default): ", Color.FromArgb(255, 182, 193))
        Dim outputPath As String = System.Console.ReadLine()
        If String.IsNullOrEmpty(outputPath) Then outputPath = "Downloads"
        
        Colorful.Console.WriteLine()
        Colorful.Console.WriteLine("Quality Options:", Color.FromArgb(100, 149, 237))
        Colorful.Console.WriteLine("1. Highest (Best Quality)", Color.White)
        Colorful.Console.WriteLine("2. Medium (Balanced)", Color.White)
        Colorful.Console.WriteLine("3. Lowest (Fastest)", Color.White)
        Colorful.Console.Write("Select quality (1-3): ", Color.FromArgb(255, 182, 193))
        
        Dim qualityChoice As String = System.Console.ReadLine()
        Dim quality As String = "highest"
        
        Select Case qualityChoice
            Case "2"
                quality = "medium"
            Case "3"
                quality = "lowest"
        End Select
        
        DownloadVideoWithRetries(url, outputPath, quality, "mp4", 3).Wait()
    End Sub

    Private Sub HandleAudioDownload()
        System.Console.Clear()
        Colorful.Console.WriteLine("🎵 AUDIO DOWNLOAD MODE", Color.FromArgb(138, 43, 226))
        Colorful.Console.WriteLine("══════════════════════", Color.FromArgb(138, 43, 226))
        System.Console.WriteLine()
        
        Colorful.Console.Write("Enter YouTube URL: ", Color.FromArgb(255, 182, 193))
        Dim url As String = System.Console.ReadLine()
        
        If String.IsNullOrEmpty(url) Then
            Colorful.Console.WriteLine("❌ URL cannot be empty!", Color.FromArgb(255, 105, 180))
            Threading.Thread.Sleep(2000)
            Return
        End If
        
        Colorful.Console.Write("Enter output directory (or press Enter for default): ", Color.FromArgb(255, 182, 193))
        Dim outputPath As String = System.Console.ReadLine()
        If String.IsNullOrEmpty(outputPath) Then outputPath = "Downloads"
        
        DownloadVideoWithRetries(url, outputPath, "highest", "mp3", 3).Wait()
    End Sub

    Private Async Function DownloadVideo(url As String, outputPath As String, quality As String, format As String) As Task
        Try
            Colorful.Console.WriteLine()
            Colorful.Console.WriteLine("🚀 Starting download process...", Color.Green)
            
            ' Create YouTube client
            Dim youtube = New YoutubeClient()
            
            ' Display progress
            Colorful.Console.WriteLine("📋 Fetching video information...", Color.Yellow)
            
            ' Get video metadata
            Dim video = Await youtube.Videos.GetAsync(url)
            
            Colorful.Console.WriteLine($"📺 Title: {video.Title}", Color.Cyan)
            Colorful.Console.WriteLine($"👤 Author: {video.Author.ChannelTitle}", Color.Cyan)
            Colorful.Console.WriteLine($"⏱️ Duration: {video.Duration}", Color.Cyan)
            System.Console.WriteLine()
            
            ' Create output directory
            If Not Directory.Exists(outputPath) Then
                Directory.CreateDirectory(outputPath)
            End If
            
            ' Clean filename
            Dim fileName As String = GetSafeFileName(video.Title)
            Dim filePath As String = Path.Combine(outputPath, $"{fileName}.{format}")
            
            ' Get stream manifest
            Colorful.Console.WriteLine("🔍 Analyzing available streams...", Color.Yellow)
            Dim streamManifest = Await youtube.Videos.Streams.GetManifestAsync(video.Id)
            
            If format.ToLower() = "mp3" Then
                ' Audio only download
                Dim audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate()
                If audioStreamInfo IsNot Nothing Then
                    Colorful.Console.WriteLine($"⬇️ Downloading audio: {audioStreamInfo.Bitrate}", Color.Green)
                    ShowProgressBar()
                    
                    Await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, filePath)
                    
                    Colorful.Console.WriteLine($"✅ Audio downloaded successfully!", Color.Green)
                    Colorful.Console.WriteLine($"📁 Location: {filePath}", Color.Cyan)
                Else
                    Colorful.Console.WriteLine("❌ No audio stream available!", Color.Red)
                End If
            Else
                ' Video download
                Dim videoStreamInfo As IStreamInfo = Nothing
                
                Select Case quality.ToLower()
                    Case "highest"
                        videoStreamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality()
                    Case "medium"
                        Dim streams = streamManifest.GetMuxedStreams().OrderBy(Function(s) CType(s, MuxedStreamInfo).VideoQuality.MaxHeight).ToArray()
                        If streams.Length > 0 Then
                            videoStreamInfo = streams(streams.Length \ 2)
                        End If
                    Case "lowest"
                        videoStreamInfo = streamManifest.GetMuxedStreams().OrderBy(Function(s) CType(s, MuxedStreamInfo).VideoQuality.MaxHeight).FirstOrDefault()
                End Select
                
                If videoStreamInfo IsNot Nothing Then
                    Dim muxedStream = CType(videoStreamInfo, MuxedStreamInfo)
                    Colorful.Console.WriteLine($"⬇️ Downloading video: {muxedStream.VideoQuality} | {muxedStream.Container}", Color.Green)
                    ShowProgressBar()
                    
                    Await youtube.Videos.Streams.DownloadAsync(videoStreamInfo, filePath)
                    
                    Colorful.Console.WriteLine($"✅ Video downloaded successfully!", Color.Green)
                    Colorful.Console.WriteLine($"📁 Location: {filePath}", Color.Cyan)
                Else
                    Colorful.Console.WriteLine("❌ No suitable video stream found!", Color.Red)
                End If
            End If
            
            ' Save to history
            SaveDownloadHistory(video.Title, url, filePath, DateTime.Now)
            
        Catch ex As Exception
            Colorful.Console.WriteLine($"❌ Error: {ex.Message}", Color.Red)
        Finally
            System.Console.WriteLine()
            Colorful.Console.WriteLine("Press any key to continue...", Color.Yellow)
            System.Console.ReadKey()
        End Try
    End Function

    Private Sub ShowProgressBar()
        Colorful.Console.Write("Progress: [", Color.Yellow)
        For i As Integer = 0 To 19
            Threading.Thread.Sleep(50)
            Colorful.Console.Write("█", Color.Green)
        Next
        Colorful.Console.WriteLine("] 100%", Color.Yellow)
    End Sub

    Private Function GetSafeFileName(fileName As String) As String
        Dim invalidChars As Char() = Path.GetInvalidFileNameChars()
        For Each c As Char In invalidChars
            fileName = fileName.Replace(c, "_"c)
        Next
        Return fileName.Substring(0, Math.Min(fileName.Length, 200))
    End Function

    Private Sub HandleBatchDownload()
        System.Console.Clear()
        Colorful.Console.WriteLine("📋 BATCH DOWNLOAD MODE", Color.Orange)
        Colorful.Console.WriteLine("══════════════════════", Color.Orange)
        System.Console.WriteLine()
        
        Colorful.Console.WriteLine("Create a text file with YouTube URLs (one per line) and specify the path:", Color.Cyan)
        Colorful.Console.Write("Enter file path: ", Color.Yellow)
        Dim filePath As String = System.Console.ReadLine()
        
        If Not File.Exists(filePath) Then
            Colorful.Console.WriteLine("❌ File not found!", Color.Red)
            Threading.Thread.Sleep(2000)
            Return
        End If
        
        Try
            Dim urls As String() = File.ReadAllLines(filePath)
            Colorful.Console.WriteLine($"📄 Found {urls.Length} URLs to download", Color.Green)
            
            For i As Integer = 0 To urls.Length - 1
                Colorful.Console.WriteLine($"🎬 Downloading {i + 1}/{urls.Length}: ", Color.Cyan)
                DownloadVideo(urls(i), "Downloads", "highest", "mp4").Wait()
                Threading.Thread.Sleep(1000)
            Next
            
            Colorful.Console.WriteLine("🎉 Batch download completed!", Color.Green)
        Catch ex As Exception
            Colorful.Console.WriteLine($"❌ Error processing batch file: {ex.Message}", Color.Red)
        End Try
        
        System.Console.WriteLine()
        Colorful.Console.WriteLine("Press any key to continue...", Color.Yellow)
        System.Console.ReadKey()
    End Sub

    Private Sub ShowDownloadHistory()
        System.Console.Clear()
        Colorful.Console.WriteLine("📜 DOWNLOAD HISTORY", Color.Blue)
        Colorful.Console.WriteLine("═══════════════════", Color.Blue)
        System.Console.WriteLine()
        
        Dim historyFile As String = "download_history.txt"
        If File.Exists(historyFile) Then
            Try
                Dim lines As String() = File.ReadAllLines(historyFile)
                If lines.Length = 0 Then
                    Colorful.Console.WriteLine("📭 No download history found.", Color.Yellow)
                Else
                    For Each line As String In lines.Take(20) ' Show last 20 downloads
                        Colorful.Console.WriteLine($"📁 {line}", Color.White)
                    Next
                End If
            Catch ex As Exception
                Colorful.Console.WriteLine($"❌ Error reading history: {ex.Message}", Color.Red)
            End Try
        Else
            Colorful.Console.WriteLine("📭 No download history found.", Color.Yellow)
        End If
        
        System.Console.WriteLine()
        Colorful.Console.WriteLine("Press any key to continue...", Color.Yellow)
        System.Console.ReadKey()
    End Sub

    Private Sub SaveDownloadHistory(title As String, url As String, filePath As String, downloadTime As DateTime)
        Try
            Dim historyLine As String = $"[{downloadTime:yyyy-MM-dd HH:mm}] {title} - {Path.GetFileName(filePath)}"
            File.AppendAllText("download_history.txt", historyLine & Environment.NewLine)
        Catch ex As Exception
            ' Silently ignore history save errors
        End Try
    End Sub

    Private Sub ShowAdvancedOptions()
        System.Console.Clear()
        Colorful.Console.WriteLine("⚙️ ADVANCED DOWNLOAD OPTIONS", Color.FromArgb(138, 43, 226))
        Colorful.Console.WriteLine("════════════════════════════", Color.FromArgb(138, 43, 226))
        System.Console.WriteLine()
        
        Colorful.Console.WriteLine("🛠️ Workarounds for YouTube Restrictions:", Color.FromArgb(100, 149, 237))
        Colorful.Console.WriteLine("• Method 1: Multiple retry attempts with delays", Color.White)
        Colorful.Console.WriteLine("• Method 2: Alternative stream detection", Color.White)
        Colorful.Console.WriteLine("• Method 3: Fallback to audio extraction", Color.White)
        Colorful.Console.WriteLine("• Method 4: External downloader integration", Color.White)
        System.Console.WriteLine()
        
        Colorful.Console.WriteLine("Available Options:", Color.FromArgb(255, 182, 193))
        Colorful.Console.WriteLine("1. Try enhanced download with retries", Color.White)
        Colorful.Console.WriteLine("2. Download using yt-dlp (if installed)", Color.White)
        Colorful.Console.WriteLine("3. Show video information only", Color.White)
        Colorful.Console.WriteLine("4. Generate download command", Color.White)
        Colorful.Console.WriteLine("5. Back to main menu", Color.White)
        
        System.Console.WriteLine()
        Colorful.Console.Write("Enter your choice (1-5): ", Color.FromArgb(255, 182, 193))
        Dim choice As String = System.Console.ReadLine()
        
        Select Case choice
            Case "1"
                HandleEnhancedDownload()
            Case "2"
                HandleExternalDownload()
            Case "3"
                ShowVideoInfoOnly()
            Case "4"
                GenerateDownloadCommand()
            Case "5"
                Return
            Case Else
                Colorful.Console.WriteLine("❌ Invalid choice! Please select 1-5.", Color.FromArgb(255, 105, 180))
                Threading.Thread.Sleep(2000)
        End Select
        
        System.Console.WriteLine()
        Colorful.Console.WriteLine("Press any key to continue...", Color.FromArgb(255, 182, 193))
        System.Console.ReadKey()
    End Sub

    Private Sub ShowAbout()
        System.Console.Clear()
        Colorful.Console.WriteLine("ℹ️ ABOUT & HELP", Color.Green)
        Colorful.Console.WriteLine("═══════════════", Color.Green)
        System.Console.WriteLine()
        
        Colorful.Console.WriteLine("🎬 YouTube Downloader v1.0", Color.Cyan)
        Colorful.Console.WriteLine("A powerful, feature-rich YouTube video and audio downloader", Color.White)
        System.Console.WriteLine()
        
        Colorful.Console.WriteLine("✨ Features:", Color.Yellow)
        Colorful.Console.WriteLine("• Download videos in various qualities (MP4)", Color.White)
        Colorful.Console.WriteLine("• Extract audio as MP3 files", Color.White)
        Colorful.Console.WriteLine("• Batch download from URL lists", Color.White)
        Colorful.Console.WriteLine("• Command-line argument support", Color.White)
        Colorful.Console.WriteLine("• Download history tracking", Color.White)
        Colorful.Console.WriteLine("• Colorful console interface", Color.White)
        System.Console.WriteLine()
        
        Colorful.Console.WriteLine("📝 Command Line Usage:", Color.Yellow)
        Colorful.Console.WriteLine("YouTubeDownloader.exe -u ""https://youtube.com/watch?v=..."" -o ""C:\\Downloads"" -q highest -f mp4", Color.LightGray)
        System.Console.WriteLine()
        
        Colorful.Console.WriteLine("🔧 Built with:", Color.Yellow)
        Colorful.Console.WriteLine("• YoutubeExplode - YouTube API library", Color.White)
        Colorful.Console.WriteLine("• Colorful.Console - Console coloring", Color.White)
        Colorful.Console.WriteLine("• CommandLineParser - Argument parsing", Color.White)
        System.Console.WriteLine()
        
        Colorful.Console.WriteLine("Press any key to continue...", Color.Yellow)
        System.Console.ReadKey()
    End Sub

    Private Sub HandleEnhancedDownload()
        System.Console.Clear()
        Colorful.Console.WriteLine("🚀 ENHANCED DOWNLOAD MODE", Color.FromArgb(138, 43, 226))
        Colorful.Console.WriteLine("═════════════════════════", Color.FromArgb(138, 43, 226))
        System.Console.WriteLine()
        
        Colorful.Console.Write("Enter YouTube URL: ", Color.FromArgb(255, 182, 193))
        Dim url As String = System.Console.ReadLine()
        
        If String.IsNullOrEmpty(url) Then
            Colorful.Console.WriteLine("❌ URL cannot be empty!", Color.FromArgb(255, 105, 180))
            Threading.Thread.Sleep(2000)
            Return
        End If
        
        Colorful.Console.Write("Enter output directory (or press Enter for default): ", Color.FromArgb(255, 182, 193))
        Dim outputPath As String = System.Console.ReadLine()
        If String.IsNullOrEmpty(outputPath) Then outputPath = "Downloads"
        
        DownloadVideoWithRetries(url, outputPath, "highest", "mp4", 5).Wait()
    End Sub

    Private Sub HandleExternalDownload()
        System.Console.Clear()
        Colorful.Console.WriteLine("🔧 EXTERNAL DOWNLOADER", Color.FromArgb(138, 43, 226))
        Colorful.Console.WriteLine("══════════════════════", Color.FromArgb(138, 43, 226))
        System.Console.WriteLine()
        
        Colorful.Console.Write("Enter YouTube URL: ", Color.FromArgb(255, 182, 193))
        Dim url As String = System.Console.ReadLine()
        
        If String.IsNullOrEmpty(url) Then
            Colorful.Console.WriteLine("❌ URL cannot be empty!", Color.FromArgb(255, 105, 180))
            Threading.Thread.Sleep(2000)
            Return
        End If
        
        TryExternalDownloader(url).Wait()
    End Sub

    Private Sub ShowVideoInfoOnly()
        System.Console.Clear()
        Colorful.Console.WriteLine("ℹ️ VIDEO INFORMATION", Color.FromArgb(138, 43, 226))
        Colorful.Console.WriteLine("════════════════════", Color.FromArgb(138, 43, 226))
        System.Console.WriteLine()
        
        Colorful.Console.Write("Enter YouTube URL: ", Color.FromArgb(255, 182, 193))
        Dim url As String = System.Console.ReadLine()
        
        If String.IsNullOrEmpty(url) Then
            Colorful.Console.WriteLine("❌ URL cannot be empty!", Color.FromArgb(255, 105, 180))
            Threading.Thread.Sleep(2000)
            Return
        End If
        
        GetVideoInfoOnly(url).Wait()
    End Sub

    ' Modern yt-dlp download method (most reliable for 2025)
    Private Function CheckAndInstallFFmpeg() As Boolean
        Try
            ' Check if ffmpeg is available
            Dim processInfo As New ProcessStartInfo() With {
                .FileName = "ffmpeg",
                .Arguments = "-version",
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True
            }
            
            Using process As Process = Process.Start(processInfo)
                process.WaitForExit()
                If process.ExitCode = 0 Then
                    Return True ' FFmpeg is available
                End If
            End Using
        Catch
            ' FFmpeg not found, continue to installation
        End Try
        
        ' FFmpeg not found, provide instructions
        System.Console.ForegroundColor = ConsoleColor.Yellow
        System.Console.WriteLine("⚠️  FFmpeg not found! This is required for audio/video processing.")
        System.Console.WriteLine("📦 Installing FFmpeg automatically...")
        System.Console.ResetColor()
        
        Try
            ' Try to install ffmpeg using winget (Windows Package Manager)
            Dim installInfo As New ProcessStartInfo() With {
                .FileName = "winget",
                .Arguments = "install ffmpeg --accept-source-agreements --accept-package-agreements",
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True
            }
            
            Using installProcess As Process = Process.Start(installInfo)
                installProcess.WaitForExit(30000) ' 30 second timeout
                
                If installProcess.ExitCode = 0 Then
                    System.Console.ForegroundColor = ConsoleColor.Green
                    System.Console.WriteLine("[OK] FFmpeg installed successfully!")
                    System.Console.ResetColor()
                    Return True
                End If
            End Using
        Catch
            ' Winget failed, show manual instructions
        End Try
        
        ' Show manual installation instructions
        System.Console.ForegroundColor = ConsoleColor.Red
        System.Console.WriteLine("❌ Automatic FFmpeg installation failed.")
        System.Console.WriteLine("Please install FFmpeg manually:")
        System.Console.WriteLine("1. Visit: https://ffmpeg.org/download.html")
        System.Console.WriteLine("2. Download FFmpeg for Windows")
        System.Console.WriteLine("3. Add FFmpeg to your system PATH")
        System.Console.WriteLine("4. Or use: winget install ffmpeg")
        System.Console.ResetColor()
        Return False
    End Function

    Private Async Function DownloadWithYtDlp(url As String, outputPath As String, quality As String, format As String) As Task(Of Boolean)
        Try
            System.Console.ForegroundColor = ConsoleColor.Green
            System.Console.WriteLine("[*] Trying yt-dlp method (most reliable for 2025)...")
            System.Console.ResetColor()
            
            ' Check FFmpeg availability for audio processing
            If format = "mp3" AndAlso Not CheckAndInstallFFmpeg() Then
                System.Console.ForegroundColor = ConsoleColor.Yellow
                System.Console.WriteLine("⚠️  Continuing without FFmpeg - may affect audio quality...")
                System.Console.ResetColor()
            End If
            
            ' Create output directory
            If Not Directory.Exists(outputPath) Then
                Directory.CreateDirectory(outputPath)
            End If
            
            ' Configure yt-dlp command based on format
            Dim ytDlpCmd As String = ""
            Dim outputTemplate As String = Path.Combine(outputPath, "%(title)s.%(ext)s")
            
            If format = "mp3" Then
                ' Audio download (single video only) - requires FFmpeg, so warn user
                System.Console.ForegroundColor = ConsoleColor.Yellow
                System.Console.WriteLine("[WARNING] MP3 download requires FFmpeg. Attempting basic audio extraction...")
                System.Console.ResetColor()
                ytDlpCmd = $"C:/Users/Shadow/AppData/Local/Microsoft/WindowsApps/python3.11.exe -m yt_dlp --no-playlist --extract-audio --audio-format mp3 --audio-quality 320K --progress --no-check-certificate -o ""{outputTemplate}"" ""{url}"""
            Else
                ' Video download with quality selection (single video only, no FFmpeg required)
                ' Use format that downloads pre-merged files to avoid FFmpeg dependency
                Dim qualitySelector As String = "best[ext=mp4][height<=?1080]"
                Select Case quality.ToLower()
                    Case "highest", "best"
                        qualitySelector = "best[ext=mp4][height<=?2160]"
                    Case "medium", "720p"
                        qualitySelector = "best[ext=mp4][height<=?720]"
                    Case "lowest", "480p"
                        qualitySelector = "best[ext=mp4][height<=?480]"
                End Select
                
                System.Console.ForegroundColor = ConsoleColor.Cyan
                System.Console.WriteLine("[VIDEO] Downloading pre-merged MP4 (no post-processing required)...")
                System.Console.ResetColor()
                
                ' Remove --embed-thumbnail and --add-metadata to avoid FFmpeg dependency
                ytDlpCmd = $"C:/Users/Shadow/AppData/Local/Microsoft/WindowsApps/python3.11.exe -m yt_dlp --no-playlist -f ""{qualitySelector}"" --progress --no-check-certificate -o ""{outputTemplate}"" ""{url}"""
            End If
            
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.WriteLine("[>] Starting download... (downloading single video only)")
            System.Console.ResetColor()
            
            ' Execute yt-dlp with live output for progress
            Dim processInfo As New ProcessStartInfo() With {
                .FileName = "cmd.exe",
                .Arguments = "/c " & ytDlpCmd,
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True
            }
            
            Using process As Process = Process.Start(processInfo)
                ' Read output in real-time to show progress
                Dim outputTasks As New List(Of Task)
                
                ' Task to handle standard output (progress info)
                outputTasks.Add(Task.Run(Async Function()
                    Dim reader = process.StandardOutput
                    Dim lastProgressTime As DateTime = DateTime.MinValue
                    
                    While Not reader.EndOfStream
                        Try
                            Dim line = Await reader.ReadLineAsync()
                            If Not String.IsNullOrWhiteSpace(line) Then
                                ' Show progress lines that contain download info (but throttle updates)
                                If line.Contains("[download]") AndAlso line.Contains("%") Then
                                    Dim now = DateTime.Now
                                    
                                    ' Update every 2 seconds to avoid flooding
                                    If now.Subtract(lastProgressTime).TotalSeconds >= 2 Then
                                        System.Console.ForegroundColor = ConsoleColor.Cyan
                                        System.Console.Write($"{vbCr}[%] {line}                    ")
                                        System.Console.ResetColor()
                                        lastProgressTime = now
                                    End If
                                ElseIf line.Contains("100%") AndAlso line.Contains("in ") Then
                                    System.Console.WriteLine()
                                    System.Console.ForegroundColor = ConsoleColor.Green
                                    System.Console.WriteLine($"[OK] Download completed! {line}")
                                    System.Console.ResetColor()
                                ElseIf line.Contains("Merging formats") OrElse line.Contains("Deleting original file") Then
                                    System.Console.WriteLine()
                                    System.Console.ForegroundColor = ConsoleColor.Yellow
                                    System.Console.WriteLine($"🔄 {line}")
                                    System.Console.ResetColor()
                                End If
                            End If
                        Catch ex As Exception
                            ' Continue processing even if one line fails
                            Continue While
                        End Try
                    End While
                End Function))
                
                ' Task to handle error output
                outputTasks.Add(Task.Run(Async Function()
                    Dim errorReader = process.StandardError
                    Dim stderr As String = Await errorReader.ReadToEndAsync()
                    Return stderr
                End Function))
                
                ' Wait for process to complete
                process.WaitForExit()
                Await Task.WhenAll(outputTasks)
                
                Dim processErrorOutput As String = CType(outputTasks(1), Task(Of String)).Result
                
                If process.ExitCode = 0 Then
                    System.Console.ForegroundColor = ConsoleColor.Green
                    System.Console.WriteLine("[OK] yt-dlp download completed successfully! Single video downloaded.")
                    System.Console.ResetColor()
                    Return True
                Else
                    System.Console.ForegroundColor = ConsoleColor.Red
                    System.Console.WriteLine($"[ERROR] yt-dlp failed: {processErrorOutput}")
                    If processErrorOutput.Contains("ffmpeg not found") Then
                        System.Console.WriteLine("[TIP] For MP3 downloads or video merging, install FFmpeg:")
                        System.Console.WriteLine("   Run: winget install ffmpeg")
                        System.Console.WriteLine("   Or download from: https://ffmpeg.org/download.html")
                    End If
                    System.Console.ResetColor()
                    Return False
                End If
            End Using
            
        Catch ex As Exception
            System.Console.ForegroundColor = ConsoleColor.Red
            System.Console.WriteLine($"❌ yt-dlp error: {ex.Message}")
            System.Console.ResetColor()
            Return False
        End Try
    End Function

    Private Async Function DownloadVideoWithRetries(url As String, outputPath As String, quality As String, format As String, maxRetries As Integer) As Task
        Dim retryCount As Integer = 0
        
        While retryCount < maxRetries
            Try
                System.Console.WriteLine()
                System.Console.ForegroundColor = ConsoleColor.Cyan
                System.Console.WriteLine($"🚀 Attempt {retryCount + 1}/{maxRetries} - Starting download process...")
                System.Console.ResetColor()
                
                ' Strategy 1: Try yt-dlp first (most reliable for 2025)
                If retryCount = 0 Then
                    Dim ytDlpSuccess = Await DownloadWithYtDlp(url, outputPath, quality, format)
                    If ytDlpSuccess Then
                        System.Console.ForegroundColor = ConsoleColor.Green
                        System.Console.WriteLine("🎉 Download completed successfully with yt-dlp!")
                        System.Console.ResetColor()
                        Return
                    End If
                    retryCount += 1
                    Continue While
                End If
                
                ' Strategy 2-5: YoutubeExplode fallback methods
                Dim youtube = New YoutubeClient()
                
                ' Add delay to avoid rate limiting
                If retryCount > 0 Then
                    System.Console.ForegroundColor = ConsoleColor.Yellow
                    System.Console.WriteLine($"⏳ Waiting {2 + retryCount} seconds before retry...")
                    System.Console.ResetColor()
                    Await Task.Delay((2 + retryCount) * 1000)
                End If
                
                ' Display progress
                System.Console.ForegroundColor = ConsoleColor.Yellow
                System.Console.WriteLine("📋 Fetching video information...")
                System.Console.ResetColor()
                
                ' Get video metadata
                Dim video = Await youtube.Videos.GetAsync(url)
                
                System.Console.ForegroundColor = ConsoleColor.Cyan
                System.Console.WriteLine($"📺 Title: {video.Title}")
                System.Console.WriteLine($"👤 Author: {video.Author.ChannelTitle}")
                System.Console.WriteLine($"⏱️ Duration: {video.Duration}")
                System.Console.ResetColor()
                System.Console.WriteLine()
                
                ' Create output directory
                If Not Directory.Exists(outputPath) Then
                    Directory.CreateDirectory(outputPath)
                End If
                
                ' Clean filename
                Dim fileName As String = GetSafeFileName(video.Title)
                Dim filePath As String = Path.Combine(outputPath, $"{fileName}.{format}")
                
                ' Get stream manifest with retry logic
                System.Console.ForegroundColor = ConsoleColor.Yellow
                System.Console.WriteLine($"🔍 Analyzing available streams (method {retryCount + 1})...")
                System.Console.ResetColor()
                Dim streamManifest = Await youtube.Videos.Streams.GetManifestAsync(video.Id)
                
                ' Try different approaches based on retry count
                Dim success As Boolean = False
                
                Select Case retryCount
                    Case 0 ' Standard approach
                        success = Await TryStandardDownload(youtube, streamManifest, filePath, format, quality)
                    Case 1 ' Try alternative streams
                        success = Await TryAlternativeStreams(youtube, streamManifest, filePath, format)
                    Case 2 ' Audio only fallback
                        success = Await TryAudioFallback(youtube, streamManifest, filePath)
                    Case Else ' Adaptive streams
                        success = Await TryAdaptiveStreams(youtube, streamManifest, filePath, format)
                End Select
                
                If success Then
                    Colorful.Console.WriteLine($"✅ Download completed successfully!", Color.FromArgb(0, 255, 127))
                    Colorful.Console.WriteLine($"📁 Location: {filePath}", Color.FromArgb(135, 206, 255))
                    SaveDownloadHistory(video.Title, url, filePath, DateTime.Now)
                    Return
                End If
                
            Catch ex As Exception
                Colorful.Console.WriteLine($"❌ Attempt {retryCount + 1} failed: {ex.Message}", Color.FromArgb(255, 105, 180))
            End Try
            
            retryCount += 1
        End While
        
        ' All retries failed, suggest alternatives
        Colorful.Console.WriteLine()
        Colorful.Console.WriteLine("💡 All download attempts failed. Try these alternatives:", Color.FromArgb(255, 182, 193))
        Colorful.Console.WriteLine("1. Use yt-dlp: yt-dlp """ + url + """", Color.White)
        Colorful.Console.WriteLine("2. Try online downloaders", Color.White)
        Colorful.Console.WriteLine("3. Check if video is region-locked", Color.White)
        
    End Function

    Private Async Function TryStandardDownload(youtube As YoutubeClient, streamManifest As StreamManifest, filePath As String, format As String, quality As String) As Task(Of Boolean)
        Try
            If format.ToLower() = "mp3" Then
                Dim audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate()
                If audioStreamInfo IsNot Nothing Then
                    Await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, filePath)
                    Return True
                End If
            Else
                Dim videoStreamInfo As IStreamInfo = Nothing
                Select Case quality.ToLower()
                    Case "highest"
                        videoStreamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality()
                    Case "lowest"
                        videoStreamInfo = streamManifest.GetMuxedStreams().OrderBy(Function(s) CType(s, MuxedStreamInfo).VideoQuality.MaxHeight).FirstOrDefault()
                End Select
                
                If videoStreamInfo IsNot Nothing Then
                    Await youtube.Videos.Streams.DownloadAsync(videoStreamInfo, filePath)
                    Return True
                End If
            End If
        Catch ex As Exception
            ' Silent fail, let retry logic handle it
        End Try
        Return False
    End Function

    Private Async Function TryAlternativeStreams(youtube As YoutubeClient, streamManifest As StreamManifest, filePath As String, format As String) As Task(Of Boolean)
        Try
            ' Try adaptive streams
            If format.ToLower() = "mp3" Then
                Dim audioStreams = streamManifest.GetAudioOnlyStreams()
                For Each stream In audioStreams
                    Try
                        Await youtube.Videos.Streams.DownloadAsync(stream, filePath)
                        Return True
                    Catch
                        Continue For
                    End Try
                Next
            Else
                Dim videoStreams = streamManifest.GetVideoOnlyStreams()
                For Each stream In videoStreams
                    Try
                        Await youtube.Videos.Streams.DownloadAsync(stream, filePath)
                        Return True
                    Catch
                        Continue For
                    End Try
                Next
            End If
        Catch ex As Exception
            ' Silent fail
        End Try
        Return False
    End Function

    Private Async Function TryAudioFallback(youtube As YoutubeClient, streamManifest As StreamManifest, filePath As String) As Task(Of Boolean)
        Try
            ' Force audio-only download even if video was requested
            Dim audioStreamInfo = streamManifest.GetAudioOnlyStreams().FirstOrDefault()
            If audioStreamInfo IsNot Nothing Then
                Dim audioPath As String = Path.ChangeExtension(filePath, "mp3")
                Await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, audioPath)
                Colorful.Console.WriteLine("🎵 Downloaded audio only (video streams unavailable)", Color.FromArgb(255, 182, 193))
                Return True
            End If
        Catch ex As Exception
            ' Silent fail
        End Try
        Return False
    End Function

    Private Async Function TryAdaptiveStreams(youtube As YoutubeClient, streamManifest As StreamManifest, filePath As String, format As String) As Task(Of Boolean)
        Try
            ' Try any available stream
            Dim allStreams = streamManifest.GetMuxedStreams().Concat(streamManifest.GetAudioOnlyStreams()).Concat(streamManifest.GetVideoOnlyStreams())
            
            For Each stream In allStreams
                Try
                    Await youtube.Videos.Streams.DownloadAsync(stream, filePath)
                    Colorful.Console.WriteLine($"📥 Downloaded using adaptive method: {stream.GetType().Name}", Color.FromArgb(255, 182, 193))
                    Return True
                Catch
                    Continue For
                End Try
            Next
        Catch ex As Exception
            ' Silent fail
        End Try
        Return False
    End Function

    Private Async Function TryExternalDownloader(url As String) As Task
        Try
            Colorful.Console.WriteLine("🔍 Checking for external downloaders...", Color.FromArgb(255, 182, 193))
            
            ' Try yt-dlp first
            Try
                Dim ytDlpProcess = New Process()
                ytDlpProcess.StartInfo.FileName = "yt-dlp"
                ytDlpProcess.StartInfo.Arguments = $"--version"
                ytDlpProcess.StartInfo.UseShellExecute = False
                ytDlpProcess.StartInfo.RedirectStandardOutput = True
                ytDlpProcess.StartInfo.CreateNoWindow = True
                
                ytDlpProcess.Start()
                Dim output = Await ytDlpProcess.StandardOutput.ReadToEndAsync()
                ytDlpProcess.WaitForExit()
                
                If ytDlpProcess.ExitCode = 0 Then
                    Colorful.Console.WriteLine($"✅ Found yt-dlp version: {output.Trim()}", Color.FromArgb(0, 255, 127))
                    Colorful.Console.WriteLine("🚀 Starting download with yt-dlp...", Color.FromArgb(100, 149, 237))
                    
                    Dim downloadProcess = New Process()
                    downloadProcess.StartInfo.FileName = "yt-dlp"
                    downloadProcess.StartInfo.Arguments = $"""{url}"""
                    downloadProcess.StartInfo.UseShellExecute = False
                    downloadProcess.StartInfo.RedirectStandardOutput = True
                    downloadProcess.StartInfo.RedirectStandardError = True
                    
                    downloadProcess.Start()
                    
                    ' Read output in real-time
                    While Not downloadProcess.HasExited
                        Dim line = Await downloadProcess.StandardOutput.ReadLineAsync()
                        If Not String.IsNullOrEmpty(line) Then
                            Colorful.Console.WriteLine($"📥 {line}", Color.White)
                        End If
                    End While
                    
                    downloadProcess.WaitForExit()
                    
                    If downloadProcess.ExitCode = 0 Then
                        Colorful.Console.WriteLine("✅ Download completed with yt-dlp!", Color.FromArgb(0, 255, 127))
                    Else
                        Colorful.Console.WriteLine("❌ yt-dlp download failed", Color.FromArgb(255, 105, 180))
                    End If
                    
                    Return
                End If
                
            Catch
                ' yt-dlp not found, try youtube-dl
            End Try
            
            ' Try youtube-dl
            Try
                Dim ytDlProcess = New Process()
                ytDlProcess.StartInfo.FileName = "youtube-dl"
                ytDlProcess.StartInfo.Arguments = $"--version"
                ytDlProcess.StartInfo.UseShellExecute = False
                ytDlProcess.StartInfo.RedirectStandardOutput = True
                ytDlProcess.StartInfo.CreateNoWindow = True
                
                ytDlProcess.Start()
                Dim output = Await ytDlProcess.StandardOutput.ReadToEndAsync()
                ytDlProcess.WaitForExit()
                
                If ytDlProcess.ExitCode = 0 Then
                    Colorful.Console.WriteLine($"✅ Found youtube-dl version: {output.Trim()}", Color.FromArgb(0, 255, 127))
                    Colorful.Console.WriteLine("🚀 Starting download with youtube-dl...", Color.FromArgb(100, 149, 237))
                    
                    Dim downloadProcess = New Process()
                    downloadProcess.StartInfo.FileName = "youtube-dl"
                    downloadProcess.StartInfo.Arguments = $"""{url}"""
                    downloadProcess.Start()
                    downloadProcess.WaitForExit()
                    
                    If downloadProcess.ExitCode = 0 Then
                        Colorful.Console.WriteLine("✅ Download completed with youtube-dl!", Color.FromArgb(0, 255, 127))
                    Else
                        Colorful.Console.WriteLine("❌ youtube-dl download failed", Color.FromArgb(255, 105, 180))
                    End If
                    
                    Return
                End If
            Catch
                ' youtube-dl not found
            End Try
            
            ' No external downloaders found
            Colorful.Console.WriteLine("❌ No external downloaders found", Color.FromArgb(255, 105, 180))
            Colorful.Console.WriteLine()
            Colorful.Console.WriteLine("💡 To use external downloaders, install:", Color.FromArgb(255, 182, 193))
            Colorful.Console.WriteLine("• yt-dlp: pip install yt-dlp", Color.White)
            Colorful.Console.WriteLine("• youtube-dl: pip install youtube-dl", Color.White)
            
        Catch ex As Exception
            Colorful.Console.WriteLine($"❌ Error with external downloader: {ex.Message}", Color.FromArgb(255, 105, 180))
        End Try
    End Function

    Private Async Function GetVideoInfoOnly(url As String) As Task
        Try
            Colorful.Console.WriteLine("📋 Fetching detailed video information...", Color.FromArgb(255, 182, 193))
            
            Dim youtube = New YoutubeClient()
            Dim video = Await youtube.Videos.GetAsync(url)
            
            System.Console.WriteLine()
            Colorful.Console.WriteLine("📺 VIDEO INFORMATION", Color.FromArgb(138, 43, 226))
            Colorful.Console.WriteLine("═══════════════════", Color.FromArgb(138, 43, 226))
            System.Console.WriteLine()
            
            Colorful.Console.WriteLine($"📝 Title: {video.Title}", Color.FromArgb(135, 206, 255))
            Colorful.Console.WriteLine($"👤 Author: {video.Author.ChannelTitle}", Color.FromArgb(135, 206, 255))
            Colorful.Console.WriteLine($"📅 Upload Date: {video.UploadDate:yyyy-MM-dd}", Color.FromArgb(135, 206, 255))
            Colorful.Console.WriteLine($"⏱️ Duration: {video.Duration}", Color.FromArgb(135, 206, 255))
            Colorful.Console.WriteLine($"👁️ View Count: {video.Engagement.ViewCount:N0}", Color.FromArgb(135, 206, 255))
            Colorful.Console.WriteLine($"👍 Like Count: {video.Engagement.LikeCount:N0}", Color.FromArgb(135, 206, 255))
            Colorful.Console.WriteLine($"📄 Description Length: {video.Description.Length} characters", Color.FromArgb(135, 206, 255))
            
            System.Console.WriteLine()
            Colorful.Console.WriteLine($"🔗 URL: {video.Url}", Color.FromArgb(255, 182, 193))
            Colorful.Console.WriteLine($"🆔 Video ID: {video.Id}", Color.FromArgb(255, 182, 193))
            
            ' Try to get stream info
            Try
                Dim streamManifest = Await youtube.Videos.Streams.GetManifestAsync(video.Id)
                System.Console.WriteLine()
                Colorful.Console.WriteLine("🎬 AVAILABLE STREAMS", Color.FromArgb(138, 43, 226))
                Colorful.Console.WriteLine("═══════════════════", Color.FromArgb(138, 43, 226))
                
                Dim muxedStreams = streamManifest.GetMuxedStreams()
                If muxedStreams.Any() Then
                    Colorful.Console.WriteLine($"📹 Video+Audio Streams: {muxedStreams.Count()}", Color.FromArgb(135, 206, 255))
                    For Each stream In muxedStreams.Take(5)
                        Dim muxed = CType(stream, MuxedStreamInfo)
                        Colorful.Console.WriteLine($"   • {muxed.VideoQuality} - {muxed.Container} - {muxed.Size}", Color.White)
                    Next
                End If
                
                Dim audioStreams = streamManifest.GetAudioOnlyStreams()
                If audioStreams.Any() Then
                    Colorful.Console.WriteLine($"🎵 Audio Streams: {audioStreams.Count()}", Color.FromArgb(135, 206, 255))
                    For Each stream In audioStreams.Take(3)
                        Colorful.Console.WriteLine($"   • {stream.Bitrate} - {stream.Container} - {stream.Size}", Color.White)
                    Next
                End If
                
                Dim videoStreams = streamManifest.GetVideoOnlyStreams()
                If videoStreams.Any() Then
                    Colorful.Console.WriteLine($"🎬 Video-Only Streams: {videoStreams.Count()}", Color.FromArgb(135, 206, 255))
                    For Each stream In videoStreams.Take(3)
                        Dim videoOnly = CType(stream, VideoOnlyStreamInfo)
                        Colorful.Console.WriteLine($"   • {videoOnly.VideoQuality} - {videoOnly.Container} - {videoOnly.Size}", Color.White)
                    Next
                End If
                
            Catch ex As Exception
                Colorful.Console.WriteLine($"⚠️ Could not retrieve stream information: {ex.Message}", Color.FromArgb(255, 105, 180))
            End Try
            
        Catch ex As Exception
            Colorful.Console.WriteLine($"❌ Error getting video information: {ex.Message}", Color.FromArgb(255, 105, 180))
        End Try
    End Function

    Private Sub GenerateDownloadCommand()
        System.Console.Clear()
        Colorful.Console.WriteLine("📋 DOWNLOAD COMMAND GENERATOR", Color.FromArgb(138, 43, 226))
        Colorful.Console.WriteLine("═════════════════════════════", Color.FromArgb(138, 43, 226))
        System.Console.WriteLine()
        
        Colorful.Console.Write("Enter YouTube URL: ", Color.FromArgb(255, 182, 193))
        Dim url As String = System.Console.ReadLine()
        
        If String.IsNullOrEmpty(url) Then
            Colorful.Console.WriteLine("❌ URL cannot be empty!", Color.FromArgb(255, 105, 180))
            Threading.Thread.Sleep(2000)
            Return
        End If
        
        Colorful.Console.WriteLine()
        Colorful.Console.WriteLine("📄 Generated Commands:", Color.FromArgb(100, 149, 237))
        System.Console.WriteLine()
        
        ' yt-dlp commands
        Colorful.Console.WriteLine("yt-dlp commands:", Color.FromArgb(255, 182, 193))
        Colorful.Console.WriteLine($"yt-dlp ""{url}""", Color.White)
        Colorful.Console.WriteLine($"yt-dlp -f best ""{url}""", Color.White)
        Colorful.Console.WriteLine($"yt-dlp -x --audio-format mp3 ""{url}""", Color.White)
        System.Console.WriteLine()
        
        ' youtube-dl commands
        Colorful.Console.WriteLine("youtube-dl commands:", Color.FromArgb(255, 182, 193))
        Colorful.Console.WriteLine($"youtube-dl ""{url}""", Color.White)
        Colorful.Console.WriteLine($"youtube-dl -f best ""{url}""", Color.White)
        System.Console.WriteLine()
        
        ' Our application command
        Colorful.Console.WriteLine("This application:", Color.FromArgb(255, 182, 193))
        Colorful.Console.WriteLine($"YouTubeDownloader.exe -u ""{url}"" -q highest -f mp4", Color.White)
        System.Console.WriteLine()
        
        Colorful.Console.WriteLine("💡 Tip: Copy and paste these commands into your terminal", Color.FromArgb(135, 206, 255))
    End Sub

    Private Sub ShowExitMessage()
        System.Console.Clear()
        
        Dim exitArt As String() = {
            "╔═══════════════════════════════════════╗",
            "║          Thank you for using         ║",
            "║       🎬 YouTube Downloader 🎵       ║",
            "║                                       ║",
            "║        Happy downloading! 🚀          ║",
            "╚═══════════════════════════════════════╝"
        }
        
        ' Use Gemini-style gradient colors for exit message
        Dim exitColors As Color() = {
            Color.FromArgb(135, 206, 255),
            Color.FromArgb(138, 43, 226),
            Color.FromArgb(199, 21, 133),
            Color.FromArgb(255, 105, 180),
            Color.FromArgb(255, 182, 193),
            Color.FromArgb(255, 192, 203)
        }
        
        For i As Integer = 0 To exitArt.Length - 1
            Colorful.Console.WriteLine(exitArt(i), exitColors(i Mod exitColors.Length))
            Threading.Thread.Sleep(200)
        Next
        
        Threading.Thread.Sleep(2000)
    End Sub

End Module
