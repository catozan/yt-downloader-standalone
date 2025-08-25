Imports System
Imports System.Drawing
Imports System.IO
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports YoutubeExplode
Imports YoutubeExplode.Videos.Streams
Imports Newtonsoft.Json
Imports System.Diagnostics

Public Class MainForm
    Inherits Form

    ' Controls
    Private lblTitle As Label
    Private txtUrl As TextBox
    Private btnAnalyze As Button
    Private lstFormats As ListBox
    Private lblVideoInfo As Label
    Private btnDownload As Button
    Private progressBar As ProgressBar
    Private lblStatus As Label
    Private btnSelectFolder As Button
    Private lblOutputPath As Label
    Private txtOutputPath As TextBox
    Private btnHistory As Button
    Private btnSettings As Button
    Private picThumbnail As PictureBox

    ' YouTube client
    Private youtube As New YoutubeClient()
    
    ' Current video info
    Private currentVideo As YoutubeExplode.Videos.Video
    Private streamManifest As StreamManifest
    Private selectedStreamInfo As IStreamInfo
    
    ' Settings
    Private outputPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "YouTube Downloads")
    
    Public Sub New()
        InitializeComponent()
        SetupEventHandlers()
        CreateOutputDirectory()
    End Sub

    Private Sub InitializeComponent()
        ' Form settings
        Me.Text = "YouTube Downloader - GUI"
        Me.Size = New Size(800, 600)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.BackColor = Color.FromArgb(45, 45, 48)
        Me.ForeColor = Color.White
        Me.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)

        ' Title Label
        lblTitle = New Label()
        lblTitle.Text = "🎬 YouTube Video Downloader"
        lblTitle.Font = New Font("Segoe UI", 16.0F, FontStyle.Bold)
        lblTitle.ForeColor = Color.FromArgb(0, 122, 204)
        lblTitle.Location = New Point(20, 20)
        lblTitle.Size = New Size(400, 30)
        Me.Controls.Add(lblTitle)

        ' URL Input
        Dim lblUrl As New Label()
        lblUrl.Text = "YouTube URL:"
        lblUrl.Location = New Point(20, 70)
        lblUrl.Size = New Size(100, 20)
        Me.Controls.Add(lblUrl)

        txtUrl = New TextBox()
        txtUrl.Location = New Point(20, 95)
        txtUrl.Size = New Size(500, 25)
        txtUrl.BackColor = Color.FromArgb(62, 62, 66)
        txtUrl.ForeColor = Color.White
        txtUrl.Font = New Font("Segoe UI", 9.0F)
        Me.Controls.Add(txtUrl)

        btnAnalyze = New Button()
        btnAnalyze.Text = "Analyze Video"
        btnAnalyze.Location = New Point(530, 93)
        btnAnalyze.Size = New Size(100, 29)
        btnAnalyze.BackColor = Color.FromArgb(0, 122, 204)
        btnAnalyze.ForeColor = Color.White
        btnAnalyze.FlatStyle = FlatStyle.Flat
        btnAnalyze.Font = New Font("Segoe UI", 9.0F)
        Me.Controls.Add(btnAnalyze)

        ' Video Info Panel
        lblVideoInfo = New Label()
        lblVideoInfo.Location = New Point(20, 140)
        lblVideoInfo.Size = New Size(500, 80)
        lblVideoInfo.BackColor = Color.FromArgb(37, 37, 38)
        lblVideoInfo.ForeColor = Color.White
        lblVideoInfo.Font = New Font("Segoe UI", 9.0F)
        lblVideoInfo.BorderStyle = BorderStyle.FixedSingle
        lblVideoInfo.Text = "Enter a YouTube URL and click 'Analyze Video' to see video information"
        Me.Controls.Add(lblVideoInfo)

        ' Thumbnail
        picThumbnail = New PictureBox()
        picThumbnail.Location = New Point(530, 140)
        picThumbnail.Size = New Size(120, 90)
        picThumbnail.SizeMode = PictureBoxSizeMode.StretchImage
        picThumbnail.BorderStyle = BorderStyle.FixedSingle
        picThumbnail.BackColor = Color.FromArgb(37, 37, 38)
        Me.Controls.Add(picThumbnail)

        ' Available Formats
        Dim lblFormats As New Label()
        lblFormats.Text = "Available Formats & Quality:"
        lblFormats.Location = New Point(20, 240)
        lblFormats.Size = New Size(200, 20)
        Me.Controls.Add(lblFormats)

        lstFormats = New ListBox()
        lstFormats.Location = New Point(20, 265)
        lstFormats.Size = New Size(630, 120)
        lstFormats.BackColor = Color.FromArgb(62, 62, 66)
        lstFormats.ForeColor = Color.White
        lstFormats.Font = New Font("Consolas", 9.0F)
        lstFormats.SelectionMode = SelectionMode.One
        Me.Controls.Add(lstFormats)

        ' Output Path
        Dim lblOutput As New Label()
        lblOutput.Text = "Download Location:"
        lblOutput.Location = New Point(20, 400)
        lblOutput.Size = New Size(120, 20)
        Me.Controls.Add(lblOutput)

        txtOutputPath = New TextBox()
        txtOutputPath.Text = outputPath
        txtOutputPath.Location = New Point(20, 425)
        txtOutputPath.Size = New Size(500, 25)
        txtOutputPath.BackColor = Color.FromArgb(62, 62, 66)
        txtOutputPath.ForeColor = Color.White
        txtOutputPath.ReadOnly = True
        Me.Controls.Add(txtOutputPath)

        btnSelectFolder = New Button()
        btnSelectFolder.Text = "Browse"
        btnSelectFolder.Location = New Point(530, 423)
        btnSelectFolder.Size = New Size(80, 29)
        btnSelectFolder.BackColor = Color.FromArgb(0, 122, 204)
        btnSelectFolder.ForeColor = Color.White
        btnSelectFolder.FlatStyle = FlatStyle.Flat
        Me.Controls.Add(btnSelectFolder)

        ' Progress Bar
        progressBar = New ProgressBar()
        progressBar.Location = New Point(20, 470)
        progressBar.Size = New Size(500, 25)
        progressBar.Style = ProgressBarStyle.Continuous
        Me.Controls.Add(progressBar)

        ' Download Button
        btnDownload = New Button()
        btnDownload.Text = "Download"
        btnDownload.Location = New Point(530, 470)
        btnDownload.Size = New Size(120, 25)
        btnDownload.BackColor = Color.FromArgb(0, 150, 0)
        btnDownload.ForeColor = Color.White
        btnDownload.FlatStyle = FlatStyle.Flat
        btnDownload.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        btnDownload.Enabled = False
        Me.Controls.Add(btnDownload)

        ' Status Label
        lblStatus = New Label()
        lblStatus.Text = "Ready"
        lblStatus.Location = New Point(20, 510)
        lblStatus.Size = New Size(500, 20)
        lblStatus.ForeColor = Color.LightGray
        Me.Controls.Add(lblStatus)

        ' Additional Buttons
        btnHistory = New Button()
        btnHistory.Text = "History"
        btnHistory.Location = New Point(660, 470)
        btnHistory.Size = New Size(80, 25)
        btnHistory.BackColor = Color.FromArgb(128, 128, 128)
        btnHistory.ForeColor = Color.White
        btnHistory.FlatStyle = FlatStyle.Flat
        Me.Controls.Add(btnHistory)

        btnSettings = New Button()
        btnSettings.Text = "Settings"
        btnSettings.Location = New Point(660, 423)
        btnSettings.Size = New Size(80, 29)
        btnSettings.BackColor = Color.FromArgb(128, 128, 128)
        btnSettings.ForeColor = Color.White
        btnSettings.FlatStyle = FlatStyle.Flat
        Me.Controls.Add(btnSettings)
    End Sub

    Private Sub SetupEventHandlers()
        AddHandler btnAnalyze.Click, AddressOf BtnAnalyze_Click
        AddHandler btnDownload.Click, AddressOf BtnDownload_Click
        AddHandler btnSelectFolder.Click, AddressOf BtnSelectFolder_Click
        AddHandler btnHistory.Click, AddressOf BtnHistory_Click
        AddHandler btnSettings.Click, AddressOf BtnSettings_Click
        AddHandler lstFormats.SelectedIndexChanged, AddressOf LstFormats_SelectedIndexChanged
        AddHandler txtUrl.KeyDown, AddressOf TxtUrl_KeyDown
    End Sub

    Private Sub CreateOutputDirectory()
        Try
            If Not Directory.Exists(outputPath) Then
                Directory.CreateDirectory(outputPath)
            End If
        Catch ex As Exception
            MessageBox.Show($"Error creating output directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub TxtUrl_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter Then
            BtnAnalyze_Click(Nothing, Nothing)
        End If
    End Sub

    Private Async Sub BtnAnalyze_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtUrl.Text) Then
            MessageBox.Show("Please enter a YouTube URL", "Missing URL", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            btnAnalyze.Enabled = False
            lblStatus.Text = "Analyzing video..."
            lstFormats.Items.Clear()
            btnDownload.Enabled = False
            currentVideo = Nothing
            streamManifest = Nothing

            ' Validate URL format first
            If Not IsValidYouTubeUrl(txtUrl.Text) Then
                Throw New ArgumentException("Please enter a valid YouTube URL (youtube.com/watch?v= or youtu.be/)")
            End If

            ' Create new client instance to avoid cached issues
            youtube = New YoutubeClient()

            lblStatus.Text = "Getting video information..."
            
            ' Try multiple approaches for better reliability against cipher errors
            Dim success As Boolean = False
            Dim lastError As Exception = Nothing

            ' Method 1: Standard approach with retries
            For attempt As Integer = 1 To 3
                Try
                    lblStatus.Text = $"Analyzing video... attempt {attempt}/3"
                    
                    ' Use fresh client for each attempt
                    youtube = New YoutubeClient()
                    
                    ' Progressive delay between attempts
                    If attempt > 1 Then
                        System.Threading.Thread.Sleep(2000 * attempt)
                    End If

                    currentVideo = Await youtube.Videos.GetAsync(txtUrl.Text)
                    streamManifest = Await youtube.Videos.Streams.GetManifestAsync(currentVideo.Id)
                    
                    success = True
                    Exit For
                    
                Catch ex As Exception
                    lastError = ex
                    If attempt < 3 Then
                        lblStatus.Text = $"Attempt {attempt} failed, retrying..."
                    End If
                    
                    ' Clear any cached data
                    GC.Collect()
                End Try
            Next

            ' Method 2: Try with extracted video ID only
            If Not success Then
                lblStatus.Text = "Trying alternative method..."
                Try
                    Dim videoId = ExtractVideoId(txtUrl.Text)
                    If Not String.IsNullOrEmpty(videoId) Then
                        youtube = New YoutubeClient()
                        currentVideo = Await youtube.Videos.GetAsync(videoId)
                        
                        ' Small delay before getting streams
                        System.Threading.Thread.Sleep(1500)
                        streamManifest = Await youtube.Videos.Streams.GetManifestAsync(videoId)
                        success = True
                    End If
                Catch ex As Exception
                    lastError = ex
                End Try
            End If

            ' Method 3: Try different URL formats
            If Not success Then
                lblStatus.Text = "Trying URL variations..."
                Dim videoId = ExtractVideoId(txtUrl.Text)
                If Not String.IsNullOrEmpty(videoId) Then
                    Dim urlVariations() As String = {
                        $"https://www.youtube.com/watch?v={videoId}",
                        $"https://youtu.be/{videoId}",
                        $"https://m.youtube.com/watch?v={videoId}"
                    }
                    
                    For Each testUrl In urlVariations
                        Try
                            youtube = New YoutubeClient()
                            currentVideo = Await youtube.Videos.GetAsync(testUrl)
                            System.Threading.Thread.Sleep(1000)
                            streamManifest = Await youtube.Videos.Streams.GetManifestAsync(currentVideo.Id)
                            success = True
                            Exit For
                        Catch
                            Continue For
                        End Try
                    Next
                End If
            End If

            If Not success OrElse currentVideo Is Nothing OrElse streamManifest Is Nothing Then
                ' Method 4: Fallback to yt-dlp for video info
                lblStatus.Text = "Trying yt-dlp fallback..."
                Try
                    Dim ytDlpInfo = Await GetVideoInfoWithYtDlp(txtUrl.Text)
                    If ytDlpInfo IsNot Nothing Then
                        ' Create a mock video object for display
                        lblVideoInfo.Text = $"Title: {ytDlpInfo("title")}" & vbCrLf &
                                          $"Duration: {ytDlpInfo("duration")}" & vbCrLf &
                                          $"[Using yt-dlp fallback method]"
                        
                        ' Add a simple download option
                        lstFormats.Items.Clear()
                        lstFormats.Items.Add(New YtDlpFormatItem("Best Quality Video (yt-dlp)", "best"))
                        lstFormats.Items.Add(New YtDlpFormatItem("Best Quality Audio (yt-dlp)", "bestaudio"))
                        lstFormats.SelectedIndex = 0
                        
                        lblStatus.Text = "Video analyzed with yt-dlp. Select format to download."
                        success = True
                        Return
                    End If
                Catch ex As Exception
                    lastError = ex
                End Try
            End If

            If Not success Then
                ' Enhanced error message based on the type of error
                Dim errorMessage As String = "Could not analyze this video using any method."
                
                If lastError IsNot Nothing Then
                    If lastError.Message.Contains("cipher") OrElse lastError.Message.Contains("manifest") Then
                        errorMessage = "YouTube cipher protection detected." & vbCrLf & vbCrLf &
                                     "This video has enhanced protection. Solutions:" & vbCrLf &
                                     "• Try a different, older video to test the app" & vbCrLf &
                                     "• Popular/recent videos often have stronger protection" & vbCrLf &
                                     "• Try again in 10-15 minutes" & vbCrLf &
                                     "• Some videos may be permanently blocked from downloading" & vbCrLf & vbCrLf &
                                     "Consider using the console version with yt-dlp for better compatibility."
                    Else
                        errorMessage &= vbCrLf & vbCrLf & "Last error: " & lastError.Message
                    End If
                End If
                
                Throw New Exception(errorMessage)
            End If

            ' Update video info
            UpdateVideoInfo()

            ' Load thumbnail
            Await LoadThumbnail()

            ' Populate format list
            PopulateFormatList()

            If lstFormats.Items.Count = 0 Then
                Throw New Exception("No downloadable formats found for this video")
            End If

            lblStatus.Text = "Video analyzed successfully. Select a format to download."

        Catch ex As ArgumentException
            MessageBox.Show(ex.Message, "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            lblStatus.Text = "Invalid YouTube URL"
        Catch ex As Exception
            Dim errorMsg As String = "Error analyzing video:" & vbCrLf & vbCrLf & ex.Message
            
            If ex.Message.Contains("cipher") OrElse ex.Message.Contains("manifest") Then
                errorMsg &= vbCrLf & vbCrLf & "Troubleshooting tips:" & vbCrLf &
                          "• Try a different video URL" & vbCrLf &
                          "• Wait a few minutes and try again" & vbCrLf &
                          "• Check if the video is public and not age-restricted" & vbCrLf &
                          "• Some videos may be temporarily unavailable"
            End If
            
            MessageBox.Show(errorMsg, "Analysis Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            lblStatus.Text = "Error analyzing video"
        Finally
            btnAnalyze.Enabled = True
        End Try
    End Sub

    Private Async Function GetVideoInfoWithYtDlp(url As String) As Task(Of Dictionary(Of String, String))
        Try
            Dim startInfo As New ProcessStartInfo With {
                .FileName = "python",
                .Arguments = $"-m yt_dlp --print title,duration --no-download ""{url}""",
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True
            }

            Using process As New Process With {.StartInfo = startInfo}
                If process.Start() Then
                    Dim output = Await process.StandardOutput.ReadToEndAsync()
                    process.WaitForExit()
                    
                    If process.ExitCode = 0 AndAlso Not String.IsNullOrWhiteSpace(output) Then
                        Dim lines = output.Split(vbLf).Where(Function(l) Not String.IsNullOrWhiteSpace(l)).ToArray()
                        If lines.Length >= 2 Then
                            Return New Dictionary(Of String, String) From {
                                {"title", lines(0).Trim()},
                                {"duration", lines(1).Trim()}
                            }
                        End If
                    End If
                End If
            End Using
        Catch
            ' Ignore yt-dlp errors
        End Try
        
        Return Nothing
    End Function

    Private Function IsValidYouTubeUrl(url As String) As Boolean
        If String.IsNullOrWhiteSpace(url) Then Return False
        
        ' Common YouTube URL patterns
        Dim patterns As String() = {
            "youtube.com/watch",
            "youtu.be/",
            "youtube.com/embed/",
            "youtube.com/v/",
            "m.youtube.com/watch"
        }
        
        Return patterns.Any(Function(pattern) url.ToLower().Contains(pattern))
    End Function

    Private Function ExtractVideoId(url As String) As String
        Try
            If String.IsNullOrWhiteSpace(url) Then Return Nothing
            
            ' Handle different URL formats
            If url.Contains("youtube.com/watch?v=") Then
                Dim match = System.Text.RegularExpressions.Regex.Match(url, "v=([^&]+)")
                If match.Success Then Return match.Groups(1).Value
            ElseIf url.Contains("youtu.be/") Then
                Dim parts = url.Split("/"c)
                If parts.Length > 0 Then
                    Return parts.Last().Split("?"c)(0)
                End If
            ElseIf url.Contains("youtube.com/embed/") Then
                Dim match = System.Text.RegularExpressions.Regex.Match(url, "embed/([^?]+)")
                If match.Success Then Return match.Groups(1).Value
            End If
            
            Return Nothing
        Catch
            Return Nothing
        End Try
    End Function

    Private Sub UpdateVideoInfo()
        If currentVideo IsNot Nothing Then
            Dim duration As String = If(currentVideo.Duration IsNot Nothing, currentVideo.Duration.Value.ToString("hh\:mm\:ss"), "Unknown")
            Dim info As String = $"Title: {currentVideo.Title}" & vbCrLf &
                               $"Author: {currentVideo.Author}" & vbCrLf &
                               $"Duration: {duration}" & vbCrLf &
                               $"Views: {currentVideo.Engagement.ViewCount:N0}"
            
            lblVideoInfo.Text = info
        End If
    End Sub

    Private Async Function LoadThumbnail() As Task
        Try
            If currentVideo IsNot Nothing AndAlso currentVideo.Thumbnails IsNot Nothing AndAlso currentVideo.Thumbnails.Count > 0 Then
                Dim thumbnailUrl As String = currentVideo.Thumbnails.OrderByDescending(Function(t) t.Resolution.Area).First().Url
                Using httpClient As New System.Net.Http.HttpClient()
                    Dim imageData As Byte() = Await httpClient.GetByteArrayAsync(thumbnailUrl)
                    Using ms As New MemoryStream(imageData)
                        picThumbnail.Image = Image.FromStream(ms)
                    End Using
                End Using
            End If
        Catch
            ' Ignore thumbnail loading errors
        End Try
    End Function

    Private Sub PopulateFormatList()
        If streamManifest Is Nothing Then Return

        lstFormats.Items.Clear()

        ' Add video streams with audio
        Dim muxedStreams = streamManifest.GetMuxedStreams().OrderByDescending(Function(s) s.VideoQuality.MaxHeight)
        For Each stream In muxedStreams
            Dim item As String = $"[VIDEO + AUDIO] {stream.VideoQuality.Label} ({stream.Container.Name}) - {FormatSize(stream.Size)}"
            lstFormats.Items.Add(New FormatItem(item, stream))
        Next

        ' Add video-only streams
        Dim videoStreams = streamManifest.GetVideoOnlyStreams().OrderByDescending(Function(s) s.VideoQuality.MaxHeight)
        For Each stream In videoStreams
            Dim item As String = $"[VIDEO ONLY] {stream.VideoQuality.Label} ({stream.Container.Name}) - {FormatSize(stream.Size)}"
            lstFormats.Items.Add(New FormatItem(item, stream))
        Next

        ' Add audio-only streams
        Dim audioStreams = streamManifest.GetAudioOnlyStreams().OrderByDescending(Function(s) s.Bitrate.BitsPerSecond)
        For Each stream In audioStreams
            Dim item As String = $"[AUDIO ONLY] {stream.Bitrate} ({stream.Container.Name}) - {FormatSize(stream.Size)}"
            lstFormats.Items.Add(New FormatItem(item, stream))
        Next

        If lstFormats.Items.Count > 0 Then
            lstFormats.SelectedIndex = 0
        End If
    End Sub

    Private Function FormatSize(size As FileSize) As String
        If size.Bytes < 1024 * 1024 Then
            Return $"{size.Bytes / 1024:F1} KB"
        ElseIf size.Bytes < 1024 * 1024 * 1024 Then
            Return $"{size.Bytes / (1024 * 1024):F1} MB"
        Else
            Return $"{size.Bytes / (1024 * 1024 * 1024):F1} GB"
        End If
    End Function

    Private Function FormatFileSize(bytes As Long) As String
        If bytes < 1024 Then
            Return $"{bytes} B"
        ElseIf bytes < 1024 * 1024 Then
            Return $"{bytes / 1024:F1} KB"
        ElseIf bytes < 1024L * 1024 * 1024 Then
            Return $"{bytes / (1024 * 1024):F1} MB"
        Else
            Return $"{bytes / (1024L * 1024 * 1024):F1} GB"
        End If
    End Function

    Private Sub LstFormats_SelectedIndexChanged(sender As Object, e As EventArgs)
        If lstFormats.SelectedItem IsNot Nothing Then
            If TypeOf lstFormats.SelectedItem Is FormatItem Then
                Dim formatItem As FormatItem = CType(lstFormats.SelectedItem, FormatItem)
                selectedStreamInfo = formatItem.StreamInfo
                btnDownload.Enabled = True
            ElseIf TypeOf lstFormats.SelectedItem Is YtDlpFormatItem Then
                ' Enable download for yt-dlp items
                btnDownload.Enabled = True
            End If
        Else
            btnDownload.Enabled = False
        End If
    End Sub

    Private Async Sub BtnDownload_Click(sender As Object, e As EventArgs)
        If lstFormats.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a format to download", "No Format Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Check if this is a yt-dlp format item
        If TypeOf lstFormats.SelectedItem Is YtDlpFormatItem Then
            Await DownloadWithYtDlpGui(CType(lstFormats.SelectedItem, YtDlpFormatItem))
            Return
        End If

        If selectedStreamInfo Is Nothing OrElse currentVideo Is Nothing Then
            MessageBox.Show("Please select a format to download", "No Format Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            btnDownload.Enabled = False
            btnAnalyze.Enabled = False
            progressBar.Value = 0
            lblStatus.Text = "Preparing download..."

            ' Generate safe filename
            Dim safeTitle As String = String.Join("_", currentVideo.Title.Split(Path.GetInvalidFileNameChars()))
            If safeTitle.Length > 100 Then
                safeTitle = safeTitle.Substring(0, 100)
            End If
            
            Dim extension As String = selectedStreamInfo.Container.Name
            Dim fileName As String = $"{safeTitle}.{extension}"
            Dim filePath As String = Path.Combine(txtOutputPath.Text, fileName)

            ' Check if file already exists
            If File.Exists(filePath) Then
                Dim result = MessageBox.Show($"File '{fileName}' already exists." & vbCrLf & vbCrLf &
                                           "Do you want to overwrite it?", 
                                           "File Exists", 
                                           MessageBoxButtons.YesNo, 
                                           MessageBoxIcon.Question)
                
                If result = DialogResult.No Then
                    lblStatus.Text = "Download cancelled"
                    Return
                End If
            End If

            ' Create progress reporter
            Dim progress As New Progress(Of Double)(Sub(p) 
                                                      Try
                                                          Me.Invoke(Sub() 
                                                                        progressBar.Value = Math.Min(CInt(p * 100), 100)
                                                                        lblStatus.Text = $"Downloading... {p:P0}"
                                                                    End Sub)
                                                      Catch
                                                          ' Ignore invoke errors during download
                                                      End Try
                                                   End Sub)

            lblStatus.Text = "Starting download..."

            ' Download with retry logic
            Dim maxRetries As Integer = 2
            Dim retryCount As Integer = 0
            Dim downloadSuccess As Boolean = False

            Do While retryCount <= maxRetries AndAlso Not downloadSuccess
                Try
                    If retryCount > 0 Then
                        lblStatus.Text = $"Retrying download... attempt {retryCount + 1}"
                        ' Create fresh client for retry
                        youtube = New YoutubeClient()
                        System.Threading.Thread.Sleep(3000) ' Wait 3 seconds before retry
                    End If

                    ' Perform the download
                    Await youtube.Videos.Streams.DownloadAsync(selectedStreamInfo, filePath, progress)
                    downloadSuccess = True

                Catch ex As Exception
                    retryCount += 1
                    If retryCount <= maxRetries Then
                        lblStatus.Text = $"Download failed, retrying... ({retryCount}/{maxRetries})"
                        
                        ' Clean up partial file if it exists
                        If File.Exists(filePath) Then
                            Try
                                File.Delete(filePath)
                            Catch
                                ' Ignore cleanup errors
                            End Try
                        End If
                    Else
                        Throw ' Re-throw if we've exhausted retries
                    End If
                End Try
            Loop

            ' Verify file was created and has content
            If Not File.Exists(filePath) OrElse New FileInfo(filePath).Length = 0 Then
                Throw New Exception("Download completed but file is empty or missing")
            End If

            ' Update status
            lblStatus.Text = $"Download completed: {fileName}"
            progressBar.Value = 100

            ' Save to history
            SaveDownloadHistory(currentVideo.Title, currentVideo.Url, filePath)

            ' Ask to open folder
            Dim openResult = MessageBox.Show("Download completed successfully!" & vbCrLf & vbCrLf &
                                       $"File: {fileName}" & vbCrLf &
                                       $"Size: {FormatFileSize(New FileInfo(filePath).Length)}" & vbCrLf & vbCrLf &
                                       "Would you like to open the download folder?", 
                                       "Download Complete", 
                                       MessageBoxButtons.YesNo, 
                                       MessageBoxIcon.Information)
            
            If openResult = DialogResult.Yes Then
                Process.Start("explorer.exe", $"/select,""{filePath}""")
            End If

        Catch ex As Exception
            ' Clean up partial file
            Try
                Dim filePath As String = Path.Combine(txtOutputPath.Text, $"{String.Join("_", currentVideo.Title.Split(Path.GetInvalidFileNameChars()))}.{selectedStreamInfo.Container.Name}")
                If File.Exists(filePath) Then
                    File.Delete(filePath)
                End If
            Catch
                ' Ignore cleanup errors
            End Try

            Dim errorMsg As String = $"Download failed: {ex.Message}"
            
            If ex.Message.Contains("403") OrElse ex.Message.Contains("Forbidden") Then
                errorMsg &= vbCrLf & vbCrLf & "This might be due to:" & vbCrLf &
                          "• Video access restrictions" & vbCrLf &
                          "• Temporary YouTube API limits" & vbCrLf &
                          "• Try again in a few minutes"
            ElseIf ex.Message.Contains("404") OrElse ex.Message.Contains("Not Found") Then
                errorMsg &= vbCrLf & vbCrLf & "The video or stream may no longer be available"
            End If
            
            MessageBox.Show(errorMsg, "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            lblStatus.Text = "Download failed"
        Finally
            btnDownload.Enabled = True
            btnAnalyze.Enabled = True
        End Try
    End Sub

    Private Async Function DownloadWithYtDlpGui(formatItem As YtDlpFormatItem) As Task
        Try
            btnDownload.Enabled = False
            btnAnalyze.Enabled = False
            progressBar.Value = 0
            lblStatus.Text = "Starting yt-dlp download..."

            ' Create output filename
            Dim fileName As String = "%(title)s.%(ext)s"
            Dim outputPath As String = txtOutputPath.Text
            
            If Not Directory.Exists(outputPath) Then
                Directory.CreateDirectory(outputPath)
            End If

            Dim arguments As String
            If formatItem.Format = "bestaudio" Then
                arguments = $"--extract-audio --audio-format mp3 --audio-quality 0 -o ""{Path.Combine(outputPath, fileName)}"" ""{txtUrl.Text}"""
            Else
                arguments = $"-f {formatItem.Format} --no-playlist -o ""{Path.Combine(outputPath, fileName)}"" ""{txtUrl.Text}"""
            End If

            Dim startInfo As New ProcessStartInfo With {
                .FileName = "python",
                .Arguments = $"-m yt_dlp {arguments}",
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True,
                .WorkingDirectory = outputPath
            }

            Using process As New Process With {.StartInfo = startInfo}
                If Not process.Start() Then
                    Throw New Exception("Failed to start yt-dlp process")
                End If

                ' Monitor progress
                Dim progressTask = Task.Run(Async Function()
                    Dim reader = process.StandardOutput
                    While Not reader.EndOfStream
                        Try
                            Dim line = Await reader.ReadLineAsync()
                            If Not String.IsNullOrWhiteSpace(line) Then
                                Me.Invoke(Sub()
                                    If line.Contains("[download]") AndAlso line.Contains("%") Then
                                        ' Extract percentage
                                        Dim percentMatch = System.Text.RegularExpressions.Regex.Match(line, "(\d+\.?\d*)%")
                                        If percentMatch.Success Then
                                            Dim percent As Double
                                            If Double.TryParse(percentMatch.Groups(1).Value, percent) Then
                                                progressBar.Value = Math.Min(CInt(percent), 100)
                                                lblStatus.Text = $"Downloading... {percent:F1}%"
                                            End If
                                        End If
                                    End If
                                End Sub)
                            End If
                        Catch
                            Continue While
                        End Try
                    End While
                End Function)

                process.WaitForExit()
                Await progressTask

                If process.ExitCode = 0 Then
                    progressBar.Value = 100
                    lblStatus.Text = "Download completed with yt-dlp!"
                    
                    MessageBox.Show("Download completed successfully using yt-dlp!" & vbCrLf & vbCrLf &
                                  "Files saved to: " & outputPath, 
                                  "Download Complete", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Information)
                Else
                    Throw New Exception("yt-dlp download failed")
                End If
            End Using

        Catch ex As Exception
            MessageBox.Show($"yt-dlp download failed: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            lblStatus.Text = "Download failed"
        Finally
            btnDownload.Enabled = True
            btnAnalyze.Enabled = True
        End Try
    End Function

    Private Sub SaveDownloadHistory(title As String, url As String, filePath As String)
        Try
            Dim historyFile As String = Path.Combine(Application.StartupPath, "download_history.json")
            Dim history As List(Of Object) = New List(Of Object)()
            
            If File.Exists(historyFile) Then
                Dim json As String = File.ReadAllText(historyFile)
                history = If(JsonConvert.DeserializeObject(Of List(Of Object))(json), New List(Of Object)())
            End If
            
            history.Add(New With {
                .Title = title,
                .Url = url,
                .FilePath = filePath,
                .DownloadDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            })
            
            ' Keep only last 100 entries
            If history.Count > 100 Then
                history = history.Skip(history.Count - 100).ToList()
            End If
            
            File.WriteAllText(historyFile, JsonConvert.SerializeObject(history, Formatting.Indented))
        Catch
            ' Ignore history save errors
        End Try
    End Sub

    Private Sub BtnSelectFolder_Click(sender As Object, e As EventArgs)
        Using folderDialog As New FolderBrowserDialog()
            folderDialog.SelectedPath = txtOutputPath.Text
            folderDialog.Description = "Select download folder"
            
            If folderDialog.ShowDialog() = DialogResult.OK Then
                txtOutputPath.Text = folderDialog.SelectedPath
                outputPath = folderDialog.SelectedPath
            End If
        End Using
    End Sub

    Private Sub BtnHistory_Click(sender As Object, e As EventArgs)
        Dim historyForm As New HistoryForm()
        historyForm.ShowDialog()
    End Sub

    Private Sub BtnSettings_Click(sender As Object, e As EventArgs)
        MessageBox.Show("Settings feature coming soon!", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ' Helper class for format list items
    Private Class FormatItem
        Public Property DisplayText As String
        Public Property StreamInfo As IStreamInfo
        
        Public Sub New(displayText As String, streamInfo As IStreamInfo)
            Me.DisplayText = displayText
            Me.StreamInfo = streamInfo
        End Sub
        
        Public Overrides Function ToString() As String
            Return DisplayText
        End Function
    End Class

    ' Helper class for yt-dlp format items
    Private Class YtDlpFormatItem
        Public Property DisplayText As String
        Public Property Format As String
        
        Public Sub New(displayText As String, format As String)
            Me.DisplayText = displayText
            Me.Format = format
        End Sub
        
        Public Overrides Function ToString() As String
            Return DisplayText
        End Function
    End Class
End Class
