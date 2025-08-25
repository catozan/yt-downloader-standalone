Imports System
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports Newtonsoft.Json
Imports System.Diagnostics

Public Class HistoryForm
    Inherits Form

    Private lstHistory As ListView
    Private btnClear As Button
    Private btnOpenFile As Button
    Private btnOpenFolder As Button
    Private btnRefresh As Button

    Public Sub New()
        InitializeComponent()
        LoadHistory()
    End Sub

    Private Sub InitializeComponent()
        ' Form settings
        Me.Text = "Download History"
        Me.Size = New Size(900, 500)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.BackColor = Color.FromArgb(45, 45, 48)
        Me.ForeColor = Color.White
        Me.Font = New Font("Segoe UI", 9.0F)

        ' History ListView
        lstHistory = New ListView()
        lstHistory.Location = New Point(20, 20)
        lstHistory.Size = New Size(840, 380)
        lstHistory.View = View.Details
        lstHistory.FullRowSelect = True
        lstHistory.GridLines = True
        lstHistory.BackColor = Color.FromArgb(62, 62, 66)
        lstHistory.ForeColor = Color.White
        lstHistory.Font = New Font("Segoe UI", 9.0F)

        ' Add columns
        lstHistory.Columns.Add("Title", 300)
        lstHistory.Columns.Add("Date", 150)
        lstHistory.Columns.Add("File Path", 390)

        Me.Controls.Add(lstHistory)

        ' Buttons
        btnRefresh = New Button()
        btnRefresh.Text = "Refresh"
        btnRefresh.Location = New Point(20, 420)
        btnRefresh.Size = New Size(80, 30)
        btnRefresh.BackColor = Color.FromArgb(0, 122, 204)
        btnRefresh.ForeColor = Color.White
        btnRefresh.FlatStyle = FlatStyle.Flat
        Me.Controls.Add(btnRefresh)

        btnOpenFile = New Button()
        btnOpenFile.Text = "Open File"
        btnOpenFile.Location = New Point(120, 420)
        btnOpenFile.Size = New Size(80, 30)
        btnOpenFile.BackColor = Color.FromArgb(0, 150, 0)
        btnOpenFile.ForeColor = Color.White
        btnOpenFile.FlatStyle = FlatStyle.Flat
        btnOpenFile.Enabled = False
        Me.Controls.Add(btnOpenFile)

        btnOpenFolder = New Button()
        btnOpenFolder.Text = "Open Folder"
        btnOpenFolder.Location = New Point(220, 420)
        btnOpenFolder.Size = New Size(100, 30)
        btnOpenFolder.BackColor = Color.FromArgb(0, 150, 0)
        btnOpenFolder.ForeColor = Color.White
        btnOpenFolder.FlatStyle = FlatStyle.Flat
        btnOpenFolder.Enabled = False
        Me.Controls.Add(btnOpenFolder)

        btnClear = New Button()
        btnClear.Text = "Clear History"
        btnClear.Location = New Point(780, 420)
        btnClear.Size = New Size(80, 30)
        btnClear.BackColor = Color.FromArgb(150, 0, 0)
        btnClear.ForeColor = Color.White
        btnClear.FlatStyle = FlatStyle.Flat
        Me.Controls.Add(btnClear)

        ' Event handlers
        AddHandler btnRefresh.Click, AddressOf BtnRefresh_Click
        AddHandler btnOpenFile.Click, AddressOf BtnOpenFile_Click
        AddHandler btnOpenFolder.Click, AddressOf BtnOpenFolder_Click
        AddHandler btnClear.Click, AddressOf BtnClear_Click
        AddHandler lstHistory.SelectedIndexChanged, AddressOf LstHistory_SelectedIndexChanged
    End Sub

    Private Sub LoadHistory()
        lstHistory.Items.Clear()
        
        Try
            Dim historyFile As String = Path.Combine(Application.StartupPath, "download_history.json")
            If Not File.Exists(historyFile) Then
                Return
            End If

            Dim json As String = File.ReadAllText(historyFile)
            Dim history = JsonConvert.DeserializeObject(json)

            If TypeOf history Is Newtonsoft.Json.Linq.JArray Then
                Dim historyArray = CType(history, Newtonsoft.Json.Linq.JArray)
                
                For Each item In historyArray.Reverse()
                    Dim listItem As New ListViewItem()
                    
                    listItem.Text = If(item("Title") IsNot Nothing, item("Title").ToString(), "Unknown")
                    listItem.SubItems.Add(If(item("DownloadDate") IsNot Nothing, item("DownloadDate").ToString(), "Unknown"))
                    listItem.SubItems.Add(If(item("FilePath") IsNot Nothing, item("FilePath").ToString(), "Unknown"))
                    listItem.Tag = If(item("FilePath") IsNot Nothing, item("FilePath").ToString(), Nothing)
                    
                    lstHistory.Items.Add(listItem)
                Next
            End If

        Catch ex As Exception
            MessageBox.Show($"Error loading history: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnRefresh_Click(sender As Object, e As EventArgs)
        LoadHistory()
    End Sub

    Private Sub BtnOpenFile_Click(sender As Object, e As EventArgs)
        If lstHistory.SelectedItems.Count = 0 Then Return
        
        Dim filePath As String = CStr(lstHistory.SelectedItems(0).Tag)
        If File.Exists(filePath) Then
            Try
                Process.Start(New ProcessStartInfo(filePath) With {.UseShellExecute = True})
            Catch ex As Exception
                MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            MessageBox.Show("File not found. It may have been moved or deleted.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub BtnOpenFolder_Click(sender As Object, e As EventArgs)
        If lstHistory.SelectedItems.Count = 0 Then Return
        
        Dim filePath As String = CStr(lstHistory.SelectedItems(0).Tag)
        Dim folderPath As String = Path.GetDirectoryName(filePath)
        
        If Directory.Exists(folderPath) Then
            Try
                Process.Start("explorer.exe", $"/select,""{filePath}""")
            Catch ex As Exception
                MessageBox.Show($"Error opening folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            MessageBox.Show("Folder not found. It may have been moved or deleted.", "Folder Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub BtnClear_Click(sender As Object, e As EventArgs)
        Dim result = MessageBox.Show("Are you sure you want to clear all download history?", 
                                   "Clear History", 
                                   MessageBoxButtons.YesNo, 
                                   MessageBoxIcon.Question)
        
        If result = DialogResult.Yes Then
            Try
                Dim historyFile As String = Path.Combine(Application.StartupPath, "download_history.json")
                If File.Exists(historyFile) Then
                    File.Delete(historyFile)
                End If
                
                lstHistory.Items.Clear()
                MessageBox.Show("History cleared successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                
            Catch ex As Exception
                MessageBox.Show($"Error clearing history: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub LstHistory_SelectedIndexChanged(sender As Object, e As EventArgs)
        btnOpenFile.Enabled = lstHistory.SelectedItems.Count > 0
        btnOpenFolder.Enabled = lstHistory.SelectedItems.Count > 0
    End Sub
End Class
