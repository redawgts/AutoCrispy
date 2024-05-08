﻿Imports System.IO

Public Class Form1

#Region "VARS"

    Dim Root As String = Application.StartupPath
    Dim AppData As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
    Dim WaitScale As Integer = 0
    Dim SettingsLoc As Point = New Point(240, 166)
    Dim LoadedSettings As FormSettings.Settings

    Public Property ChainControl As DragDropList
    Public Property ChainList As New List(Of FormSettings.ChainObject)
    Public Property ChainThumbs As New List(Of Image)
    Public Property CaffePath As String
    Public Property WaifuNcnnPath As String
    Public Property RealSRNcnnPath As String
    Public Property RealESRGNcnnPath As String
    Public Property SRMDNcnnPath As String
    Public Property WaifuCppPath As String
    Public Property Anime4kPath As String
    Public Property TexConvPath As String
    Public Property xBRZPath As String
    Public Property PyPath As String
    Public Property PyModels As New List(Of String)

#End Region

#Region "Structs"

    <Serializable()> Public Structure ArguementString
        Private Property Arguements As String
        Public Function GetArguements() As String
            Return System.Text.RegularExpressions.Regex.Replace(Arguements, " {2,}", " ").Trim
        End Function
        Public Sub AddArguement(Flag As String)
            Arguements += " " & Flag
        End Sub
        Public Sub AddArguement(Flag As String, Value As String)
            Arguements += " " & Flag & " " & Value
        End Sub
        Public Sub AddArguement(Flag As String, Enabled As Boolean)
            If Enabled Then Arguements += " " & Flag
        End Sub
    End Structure

#End Region

#Region "Loading"

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Size = New Size(660, 413)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Application.CurrentCulture = New Globalization.CultureInfo("EN-US")
        PreloadImageList()
        ChainControl = New DragDropList(ChainPreview, 7)
        Try
            If File.Exists(Root & "\portable.xml") Then
                FormSettings.LoadSettings(Me, Deserialize(Of FormSettings.Settings)(File.ReadAllText(Root & "\portable.xml")))
            ElseIf File.Exists(AppData & "\AutoCrispy\settings.xml") Then
                FormSettings.LoadSettings(Me, Deserialize(Of FormSettings.Settings)(File.ReadAllText(AppData & "\AutoCrispy\settings.xml")))
            Else
                FormSettings.LoadSettings(Me, Deserialize(Of FormSettings.Settings)(My.Resources.default_settings))
                If Not Directory.Exists(AppData & "\AutoCrispy") Then
                    Directory.CreateDirectory(AppData & "\AutoCrispy")
                End If
            End If
        Catch ex As Exception
            MsgBox("Failed to load Settings!  Loading program defaults.")
            FormSettings.LoadSettings(Me, Deserialize(Of FormSettings.Settings)(My.Resources.default_settings))
        End Try
        If ExeTextBox.Text <> "" Then
            Root = ExeTextBox.Text
        End If
        StartUpCheckEXE()
        If ExeComboBox.Items.Count > 0 Then
            ExeComboBox.SelectedIndex = 0
            SetSettingsWindow()
        End If
        ChainControl.DrawList(ChainControl.ListItems)
        WatchDogButton.Select()
        If Environment.GetCommandLineArgs.Count > 1 Then
            WatchDogButton_Click(sender, e)
        End If
    End Sub

    Private Sub Form1_Closing(sender As Object, e As EventArgs) Handles MyBase.Closing
        If PortableCheckBox.Checked = True Then
            File.WriteAllText(Root & "\portable.xml", Serialize(New FormSettings.Settings(Me)))
        Else
            File.WriteAllText(AppData & "\AutoCrispy\settings.xml", Serialize(New FormSettings.Settings(Me)))
        End If
    End Sub

    Private Sub StartUpCheckEXE()
        Dim RootFolders As List(Of String) = Directory.GetDirectories(Root).ToList
        RootFolders.Add(Root)
        ExeComboBox.Items.Clear()
        For Each Folder As String In RootFolders
            If File.Exists(Folder & "\waifu2x-caffe-cui.exe") Then
                ExeComboBox.Items.Add("Waifu2x Caffe")
                CaffePath = "\" & IIf(Folder <> Root, Path.GetFileName(Folder), "") & "\waifu2x-caffe-cui.exe"
            End If
            If File.Exists(Folder & "\waifu2x-ncnn-vulkan.exe") Then
                ExeComboBox.Items.Add("Waifu2x Vulkan")
                WaifuNcnnPath = "\" & IIf(Folder <> Root, Path.GetFileName(Folder), "") & "\waifu2x-ncnn-vulkan.exe"
            End If
            If File.Exists(Folder & "\realsr-ncnn-vulkan.exe") Then
                ExeComboBox.Items.Add("RealSR Vulkan")
                RealSRNcnnPath = "\" & IIf(Folder <> Root, Path.GetFileName(Folder), "") & "\realsr-ncnn-vulkan.exe"
            End If
            If File.Exists(Folder & "\realesrgan-ncnn-vulkan.exe") Then
                ExeComboBox.Items.Add("RealESRGAN Vulkan")
                RealESRGNcnnPath = "\" & IIf(Folder <> Root, Path.GetFileName(Folder), "") & "\realesrgan-ncnn-vulkan.exe"
            End If
            If File.Exists(Folder & "\srmd-ncnn-vulkan.exe") Then
                ExeComboBox.Items.Add("SRMD Vulkan")
                SRMDNcnnPath = "\" & IIf(Folder <> Root, Path.GetFileName(Folder), "") & "\srmd-ncnn-vulkan.exe"
            End If
            If File.Exists(Folder & "\waifu2x-converter-cpp.exe") Then
                ExeComboBox.Items.Add("Waifu2x CPP")
                WaifuCppPath = "\" & IIf(Folder <> Root, Path.GetFileName(Folder), "") & "\waifu2x-converter-cpp.exe"
            End If
            If File.Exists(Folder & "\Anime4KCPP_CLI.exe") Then
                ExeComboBox.Items.Add("Anime4k CPP")
                Anime4kPath = "\" & IIf(Folder <> Root, Path.GetFileName(Folder), "") & "\Anime4KCPP_CLI.exe"
            End If
            If File.Exists(Folder & "\texconv.exe") Then
                ExeComboBox.Items.Add("TexConv")
                TexConvPath = "\" & IIf(Folder <> Root, Path.GetFileName(Folder), "") & "\texconv.exe"
            End If
            If File.Exists(Folder & "\ScalerTest_Windows.exe") Then
                ExeComboBox.Items.Add("xBRZ")
                xBRZPath = "\" & IIf(Folder <> Root, Path.GetFileName(Folder), "") & "\ScalerTest_Windows.exe"
            End If
            If File.Exists(Folder & "\esrgan.exe") Then
                PyModels.Clear()
                PyModel.Items.Clear()
                ExeComboBox.Items.Add("ESRGAN")
                PyPath = "\" & IIf(Folder <> Root, Path.GetFileName(Folder), "") & "\esrgan.exe"
                For Each SubFolder As String In Directory.GetDirectories(Folder)
                    Dim Models As String() = Directory.EnumerateFiles(SubFolder, "*.pth").ToArray
                    For Each PythonModel As String In Models
                        PyModels.Add(PythonModel)
                        PyModel.Items.Add(Path.GetFileName(PythonModel))
                    Next
                Next
                If PyModels.Count > 0 Then
                    PyModel.SelectedIndex = 0
                Else
                    MsgBox("No ESRGAN Models Found!", MsgBoxStyle.Critical)
                End If
            End If
        Next
    End Sub

    Private Sub PreloadImageList()
        ChainThumbs.Add(My.Resources._0)
        ChainThumbs.Add(My.Resources._1)
        ChainThumbs.Add(My.Resources._2)
        ChainThumbs.Add(My.Resources._3)
        ChainThumbs.Add(My.Resources._4)
        ChainThumbs.Add(My.Resources._5)
        ChainThumbs.Add(My.Resources._6)
        ChainThumbs.Add(My.Resources._7)
        ChainThumbs.Add(My.Resources._8)
    End Sub

#End Region

#Region "UI"

    Private Sub ExeComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ExeComboBox.SelectedIndexChanged
        SetSettingsWindow()
    End Sub

    Private Sub InputBrowse_Click(sender As Object, e As EventArgs) Handles InputBrowse.Click
        InputTextBox.Text = GetFolder(InputTextBox.Text)
    End Sub

    Private Sub OutputBrowse_Click(sender As Object, e As EventArgs) Handles OutputBrowse.Click
        OutputTextBox.Text = GetFolder(OutputTextBox.Text)
    End Sub

    Private Sub ExeBrowse_Click(sender As Object, e As EventArgs) Handles ExeBrowse.Click
        ExeTextBox.Text = GetFolder(ExeTextBox.Text)
    End Sub

    Private Sub ExeTextBox_TextChanged(sender As Object, e As EventArgs) Handles ExeTextBox.TextChanged
        If Directory.Exists(ExeTextBox.Text) = True Then
            Root = ExeTextBox.Text
        Else
            Root = Application.StartupPath
        End If
        StartUpCheckEXE()
        If ExeComboBox.Items.Count > 0 Then
            ExeComboBox.SelectedIndex = 0
            SetSettingsWindow()
        End If
    End Sub

    Private Sub DefringeCheck_CheckedChanged(sender As Object, e As EventArgs) Handles DefringeCheck.CheckedChanged
        DefringeThresh.Enabled = DefringeCheck.Checked
    End Sub

    Private Sub ChainSave_Click(sender As Object, e As EventArgs) Handles ChainSave.Click
        Using SFD As New SaveFileDialog With {.Filter = "XML Files|*.xml|All Files|*.*"}
            If SFD.ShowDialog = DialogResult.OK Then
                File.WriteAllText(SFD.FileName, Serialize(ChainList))
            End If
        End Using
    End Sub

    Private Sub ChainLoad_Click(sender As Object, e As EventArgs) Handles ChainLoad.Click
        Using OFD As New OpenFileDialog With {.Filter = "XML Files|*.xml|All Files|*.*"}
            If OFD.ShowDialog = DialogResult.OK Then
                ChainControl.ListItems.Clear()
                ChainList.Clear()
                ChainList = Deserialize(Of List(Of FormSettings.ChainObject))(File.ReadAllText(OFD.FileName))
                For Each ChainItem As FormSettings.ChainObject In ChainList
                    ChainControl.ListItems.Add(New DragDropList.DragDropItem(ChainList.IndexOf(ChainItem), ChainItem.Name, ChainThumbs.Item(ChainItem.IconIndex)))
                Next
                ChainControl.DrawList(ChainControl.ListItems)
            End If
        End Using
    End Sub

    Private Sub ChainAdd_Click(sender As Object, e As EventArgs) Handles ChainAdd.Click
        AddModelToChain(ExeComboBox.SelectedItem)
    End Sub

    Private Sub RemoveItemFromChain(sender As Object, e As EventArgs) Handles ChainContextDelete.Click
        Dim Remove As Integer = ChainControl.GetCurrentIndex
        ChainList.RemoveAt(Remove)
        ChainControl.ListItems.RemoveAt(Remove)
        ChainControl.ReorderList()
        ChainControl.DrawList(ChainControl.ListItems)
    End Sub

    Private Sub ChainContextEdit_Click(sender As Object, e As EventArgs) Handles ChainContextEdit.Click
        Dim ItemIndex As Integer = ChainControl.GetCurrentIndex
        Using ECD As New EditChainDialog(Serialize(ChainList(ItemIndex)))
            If ECD.ShowDialog = DialogResult.OK Then
                Try
                    Dim NewChainItem As FormSettings.ChainObject = Deserialize(Of FormSettings.ChainObject)(ECD.ResultText)
                    ChainList(ItemIndex) = NewChainItem
                Catch ex As Exception
                    MsgBox("Error: New settings could not be parsed.")
                End Try
            End If
        End Using
    End Sub

    Private Sub ChainPreview_MouseUp(sender As Object, e As MouseEventArgs) Handles ChainPreview.MouseUp

        If e.Button = MouseButtons.Left Then
            Dim TempList As New List(Of FormSettings.ChainObject)
            For Each Item As DragDropList.DragDropItem In ChainControl.ListItems
                TempList.Add(ChainList(Item.Index))
            Next
            ChainList = TempList
            ChainControl.ReorderList()
        End If

    End Sub

    Private Sub DDxFormatListBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDxFormatListBox.SelectedIndexChanged
        DDxFormatLabel.Text = "Format: " & DDxFormatListBox.SelectedItem
    End Sub

    Private Sub DDxModeBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDxModeBox.SelectedIndexChanged
        Select Case DDxModeBox.SelectedIndex
            Case 0
                DDxConvFormat.Enabled = True
            Case 1
                DDxConvFormat.Enabled = False
        End Select
    End Sub

    Private Sub RunOnceButton_Click(sender As Object, e As EventArgs) Handles RunOnceButton.Click
        Using OFD As New OpenFileDialog With {.Filter = "Image Files|*.png;*.jpg;*.bmp"}
            If OFD.ShowDialog = DialogResult.OK Then
                Using SFD As New SaveFileDialog With {.Filter = "PNG Images|*.png"}
                    If SFD.ShowDialog = DialogResult.OK Then
                        Dim TempPath As String = Path.GetTempPath & "Single_0"
                        Directory.CreateDirectory(Path.GetTempPath & "Single_0")
                        File.Copy(OFD.FileName, TempPath & "\" & Path.GetFileName(SFD.FileName), True)
                        LoadedSettings = New FormSettings.Settings(Me)
                        LoadedSettings.Paths = New FormSettings.ProgramPaths(TempPath, Directory.GetParent(SFD.FileName).FullName, Root)
                        If ChainControl.ListItems.Count = 0 Then
                            AddModelToChain(ExeComboBox.SelectedItem, False)
                        End If
                        SwitchGroups(False)
                        WorkHorse.RunWorkerAsync()
                    End If
                End Using
            End If
        End Using
    End Sub

    Private Sub WatchDogButton_Click(sender As Object, e As EventArgs) Handles WatchDogButton.Click
        If (Not (Directory.Exists(InputTextBox.Text) = True)) OrElse (Not (Directory.Exists(OutputTextBox.Text) = True)) Then
            MsgBox("No path specified, or path invalid!", MsgBoxStyle.Critical, "Error")
        ElseIf WorkHorse.IsBusy = True Then
            WatchDogButton.Enabled = False
            WorkHorse.CancelAsync()
        Else
            WatchDog.Enabled = Not WatchDog.Enabled
            WatchDogButton.Text = "Running: " & WatchDog.Enabled
            SwitchGroups(Not WatchDog.Enabled)
        End If
    End Sub

    Private Sub ThreadComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ThreadComboBox.SelectedIndexChanged
        If ThreadComboBox.SelectedIndex = 1 Then
            NumericThreads.Enabled = True
        Else
            NumericThreads.Enabled = False
        End If
    End Sub

    Private Sub SeamsBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles SeamsBox.SelectedIndexChanged
        If SeamsBox.SelectedIndex > 0 Then
            SeamScale.Enabled = True
            SeamMargin.Enabled = True
        Else
            SeamScale.Enabled = False
            SeamMargin.Enabled = False
        End If
    End Sub

    Private Sub Anime4kCheck_Changes(sender As Object, e As EventArgs) Handles MyBase.Load, AnimeCppPre.CheckedChanged, AnimeCppPreFilter.CheckedChanged, AnimeCppPost.CheckedChanged, AnimeCppPostFilter.CheckedChanged, AnimeCPPCnn.CheckedChanged
        If AnimeCPPCnn.Checked = True Then
            AnimeCppPre.Checked = False
            AnimeCppPost.Checked = False
        End If
        AnimeCppPreFilter.Enabled = AnimeCppPre.Checked
        AnimeCppPreFilters.Enabled = AnimeCppPreFilter.Checked
        If AnimeCppPre.Checked = False Then AnimeCppPreFilter.Checked = False
        AnimeCppPostFilter.Enabled = AnimeCppPost.Checked
        AnimeCppPostFilters.Enabled = AnimeCppPostFilter.Checked
        If AnimeCppPost.Checked = False Then AnimeCppPostFilter.Checked = False
    End Sub

    Private Sub SetSettingsWindow()
        CaffeGroup.Visible = False
        VulkanGroup.Visible = False
        WaifuCPPGroup.Visible = False
        AnimeCPPGroup.Visible = False
        DDxGroup.Visible = False
        xBRZGroup.Visible = False
        PyGroup.Visible = False
        VulkanNoise.Enabled = True
        Select Case ExeComboBox.SelectedItem
            Case "Waifu2x Caffe"
                MoveShowGroup(CaffeGroup)
            Case "Waifu2x Vulkan"
                MoveShowGroup(VulkanGroup)
                VulkanScale.Value = 2
                VulkanScale.Enabled = True
            Case "RealSR Vulkan", "RealESRGAN Vulkan"
                MoveShowGroup(VulkanGroup)
                VulkanScale.Value = 4
                VulkanScale.Enabled = False
                VulkanNoise.Enabled = False
            Case "SRMD Vulkan"
                MoveShowGroup(VulkanGroup)
                VulkanScale.Value = 4
                VulkanScale.Enabled = False
            Case "Waifu2x CPP"
                MoveShowGroup(WaifuCPPGroup)
            Case "Anime4k CPP"
                MoveShowGroup(AnimeCPPGroup)
            Case "TexConv"
                MoveShowGroup(DDxGroup)
            Case "xBRZ"
                MoveShowGroup(xBRZGroup)
            Case "ESRGAN"
                MoveShowGroup(PyGroup)
        End Select
    End Sub

    Sub MoveShowGroup(ByRef Source As GroupBox)
        Source.Location = SettingsLoc
        Source.Visible = True
    End Sub

    Private Sub SwitchGroups(Enabled As Boolean)
        TabGroup.Enabled = Enabled
        SettingsGroup.Enabled = Enabled
        CaffeGroup.Enabled = Enabled
        VulkanGroup.Enabled = Enabled
        WaifuCPPGroup.Enabled = Enabled
        AnimeCPPGroup.Enabled = Enabled
        DDxGroup.Enabled = Enabled
        PyGroup.Enabled = Enabled
    End Sub

#End Region

#Region "Background"

    Private Sub WatchDog_Tick(sender As Object, e As EventArgs) Handles WatchDog.Tick
        Dim Source = Directory.GetFiles(InputTextBox.Text, "*.*", SearchOption.AllDirectories).Count
        Dim FileCheck = GetMissingFiles(InputTextBox.Text, OutputTextBox.Text).Count
        If Source = 0 OrElse FileCheck = 0 Then
            WaitScale = Math.Min(WaitScale + 1, 100)
            WatchDog.Interval = 1000 + (WaitScale * 590)
        Else
            WaitScale = 0
            WatchDog.Interval = 1000
            LoadedSettings = New FormSettings.Settings(Me)
            If ChainControl.ListItems.Count = 0 Then
                AddModelToChain(ExeComboBox.SelectedItem, False)
            End If
            WorkHorse.RunWorkerAsync()
        End If
    End Sub

    Private Sub WorkHorse_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles WorkHorse.DoWork
        WatchDog.Stop()
        MakeUpscale()
        If WorkHorse.CancellationPending = True Then
            e.Cancel = True
        End If
    End Sub

    Private Sub WorkHorse_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles WorkHorse.ProgressChanged
        UpscaleProgress.Value = e.ProgressPercentage
        If (HotKeyCheckbox.Checked = True) AndAlso (GetActiveWindow <> Me.Handle) Then SendKeys.Send("%`") : Threading.Thread.Sleep(200) : SendKeys.Send("%`")
    End Sub

    Private Sub WorkHorse_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles WorkHorse.RunWorkerCompleted
        UpscaleProgress.Value = 0
        If ChainControl.ListItems.Count = 0 Then
            ChainList.Clear()
        End If
        If e.Cancelled = True Then
            WatchDog.Enabled = False
            WatchDogButton.Text = "Running: " & False
            SwitchGroups(Not WatchDog.Enabled)
            WatchDogButton.Enabled = True
            Exit Sub
        End If
        If WatchDogButton.Text = "Running: True" Then
            WatchDog.Start()
        Else
            Directory.Delete(Path.GetTempPath & "Single_0", True)
            SwitchGroups(True)
        End If
    End Sub

#End Region

#Region "Upscale Routine"

    Private Sub MakeUpscale()
        Dim TempPath As String = Path.GetTempPath & "Temp_0"
        Dim Source = GetMissingFiles(LoadedSettings.Paths.InputPath, LoadedSettings.Paths.OutputPath)
        For i = 0 To Source.Count - 1 Step GetThreads(LoadedSettings.BasicSettings.ThreadIndex, LoadedSettings.BasicSettings.ThreadCount)
            Dim ChainPaths As New List(Of String)
            Dim DeletedChainPaths As New List(Of String)
            ChainPaths.Add(TempPath)
            For j = 0 To ChainList.Count - 2
                Dim TempName As String = Path.GetTempPath & "Chain_" & j
                ChainPaths.Add(TempName)
                Directory.CreateDirectory(TempName)
            Next
            ChainPaths.Add(LoadedSettings.Paths.OutputPath)
            Directory.CreateDirectory(TempPath)
            For j = i To i + GetThreads(LoadedSettings.BasicSettings.ThreadIndex, LoadedSettings.BasicSettings.ThreadCount) - 1
                If j <= Source.Count - 1 Then
                    File.Copy(Source(j), TempPath & "\" & Path.GetFileName(Source(j)), True)
                End If
            Next
            For Each Model In ChainList
                Dim NewImages As New List(Of String)
                Dim DiffImages = GetMissingFiles(ChainPaths(0), LoadedSettings.Paths.OutputPath)
                For Each NewImage As String In DiffImages
                    Dim AcceptExt As Boolean = Model.Package.FileTypes.Contains(Path.GetExtension(NewImage).ToLower)
                    If File.Exists(NewImage) AndAlso AcceptExt = True Then
                        NewImages.Add(NewImage)
                        If LoadedSettings.BasicSettings.FixPS2 = True Then
                            If (ChainList.IndexOf(Model) = 0 AndAlso Model.Name <> "TexConv") OrElse (ChainList(0).Name = "TexConv" AndAlso ChainList.IndexOf(Model) = 1) Then
                                RemovePS2Alpha(NewImage)
                            End If
                        End If
                        If LoadedSettings.ExpertSettings.SeamlessMode > 0 Then
                            If (ChainList.IndexOf(Model) = 0 AndAlso Model.Name <> "TexConv") OrElse (ChainList(0).Name = "TexConv" AndAlso ChainList.IndexOf(Model) = 1) Then
                                Dim SeamlessImage As Bitmap = GetUnlockedImage(NewImage)
                                SeamlessImage = MakeSeamless(SeamlessImage, LoadedSettings.ExpertSettings.SeamlessMode, LoadedSettings.ExpertSettings.SeamlessMargin)
                                SeamlessImage.Save(NewImage)
                            End If
                        End If
                    End If
                Next
                If NewImages.Count > 0 Then
                    Dim BuildProcess As ProcessStartInfo
                    If Model.PackageType = "ESRGAN" OrElse Model.PackageType.Contains("Vulkan") Then
                        BuildProcess = New ProcessStartInfo(Root & Model.FileLocation, MakeCommand(ChainPaths(0), ChainPaths(1), Model.PackageType, Model.Package))
                        BuildProcess.WorkingDirectory = Directory.GetParent(Root & Model.FileLocation).FullName
                        BuildProcess.RedirectStandardOutput = True
                        BuildProcess.RedirectStandardError = True
                        BuildProcess.UseShellExecute = False
                        BuildProcess.CreateNoWindow = True
                        Dim BatchProcess As Process = Process.Start(BuildProcess)
                        BatchProcess.WaitForExit()
                        If LoadedSettings.ExpertSettings.Logging = True Then
                            WriteLog(BatchProcess, LoadedSettings.Paths.OutputPath)
                        End If
                    Else
                        Dim ProcessBag As New List(Of Process)
                        For j = 0 To NewImages.Count - 1
                            Dim NewImage As String = ChainPaths(1) & "\" & Path.GetFileName(NewImages(j))
                            BuildProcess = New ProcessStartInfo(Root & Model.FileLocation, MakeCommand(NewImages(j), NewImage, Model.PackageType, Model.Package))
                            BuildProcess.WorkingDirectory = Directory.GetParent(Root & Model.FileLocation).FullName
                            BuildProcess.RedirectStandardOutput = True
                            BuildProcess.RedirectStandardError = True
                            BuildProcess.UseShellExecute = False
                            BuildProcess.CreateNoWindow = True
                            Dim BatchProcess As Process = Process.Start(BuildProcess)
                            ProcessBag.Add(BatchProcess)
                            If LoadedSettings.ExpertSettings.Logging = True Then
                                WriteLog(BatchProcess, LoadedSettings.Paths.OutputPath)
                            End If
                        Next
                        Do
                            Dim CompletionStatus As New List(Of Boolean)
                            For Each Job As Process In ProcessBag
                                CompletionStatus.Add(Job.HasExited)
                            Next
                            If Not CompletionStatus.Contains(False) Then
                                Exit Do
                            End If
                        Loop
                    End If
                    For Each TempImage As String In Directory.GetFiles(ChainPaths(0))
                        File.Delete(TempImage)
                    Next
                End If
                DeletedChainPaths.Add(ChainPaths(0))
                ChainPaths.RemoveAt(0)
                If (ChainList.IndexOf(Model) = ChainList.Count - 1 AndAlso Model.Name <> "TexConv") OrElse (ChainList(ChainList.Count - 1).Name = "TexConv" AndAlso ChainList.IndexOf(Model) = ChainList.Count - 2) Then
                    If LoadedSettings.BasicSettings.Defringe = True Then
                        For Each NewImage In NewImages
                            If File.Exists(ChainPaths(0) & "\" & Path.GetFileName(NewImage)) Then Defringe(ChainPaths(0) & "\" & Path.GetFileName(NewImage), LoadedSettings.BasicSettings.DefringeThreshold)
                        Next
                    End If
                    If LoadedSettings.ExpertSettings.SeamlessMode > 0 Then
                        For Each NewImage In NewImages
                            If File.Exists(ChainPaths(0) & "\" & Path.GetFileName(NewImage)) Then
                                Dim ScaleVal As Integer = LoadedSettings.ExpertSettings.SeamlessScale * LoadedSettings.ExpertSettings.SeamlessMargin
                                Dim CroppedImage As Bitmap = GetUnlockedImage(ChainPaths(0) & "\" & Path.GetFileName(NewImage))
                                CroppedImage = CropImage(CroppedImage, ScaleVal, ScaleVal, CroppedImage.Width - (ScaleVal * 2), CroppedImage.Height - (ScaleVal * 2), 0)
                                CroppedImage.Save(ChainPaths(0) & "\" & Path.GetFileName(NewImage))
                            End If
                        Next
                    End If
                    If LoadedSettings.BasicSettings.FixPS2 = True Then
                        For Each NewImage In NewImages
                            If File.Exists(ChainPaths(0) & "\" & Path.GetFileName(NewImage)) Then
                                AddPS2Alpha(ChainPaths(0) & "\" & Path.GetFileName(NewImage))
                            End If
                        Next
                    End If
                End If
                If WorkHorse.CancellationPending = True Then
                    Directory.Delete(TempPath, True)
                    For j = 0 To ChainList.Count - 2
                        Dim TempName As String = Path.GetTempPath & "Chain_" & j
                        Directory.Delete(TempName, True)
                    Next
                    Exit Sub
                End If
            Next
            For Each ChainDir As String In DeletedChainPaths
                Directory.Delete(ChainDir, True)
            Next
            WorkHorse.ReportProgress(Math.Round(((i * 100) + 1) / Source.Count, 0))
        Next
        If CleanupCheckBox.Checked = True Then
            For Each SourceImage As String In Source
                File.Delete(SourceImage)
            Next
        End If
    End Sub

#End Region

#Region "Packageing"

    Private Function MakeCommand(Source As String, Dest As String, Mode As String, Package As Object) As String
        Select Case Mode
            Case "Waifu2x Caffe"
                Return MakeCaffeCommand(Source, Dest, Package)
            Case "Waifu2x Vulkan"
                Return MakeVulkanCommand(Source, Dest, False, Package)
            Case "RealSR Vulkan"
                Return MakeVulkanCommand(Source, Dest, True, Package)
            Case "RealESRGAN Vulkan"
                Return MakeVulkanCommand(Source, Dest, True, Package)
            Case "SRMD Vulkan"
                Return MakeVulkanCommand(Source, Dest, False, Package)
            Case "Waifu2x CPP"
                Return MakeCPPCommand(Source, Dest, Package)
            Case "Anime4k CPP"
                Return MakeA4KCommand(Source, Dest, Package)
            Case "TexConv"
                Return MakeTexConvCommand(Source, Dest, Package)
            Case "xBRZ"
                Return MakeXBRZCommand(Source, Dest, Package)
            Case "ESRGAN"
                Return MakePyCommand(Source, Dest, Package)
        End Select
        Return ""
    End Function

    Private Sub AddModelToChain(Mode As String, Optional AddPreview As Boolean = True)
        Select Case Mode
            Case "Waifu2x Caffe"
                ChainControl.ListItems.Add(New DragDropList.DragDropItem(ChainList.Count, "Caffe", ChainThumbs.Item(0)))
                ChainList.Add(New FormSettings.ChainObject("Caffe", 0, CaffePath, "Waifu2x Caffe", Me))
            Case "Waifu2x Vulkan"
                ChainControl.ListItems.Add(New DragDropList.DragDropItem(ChainList.Count, "Waifu Vulkan", ChainThumbs.Item(1)))
                ChainList.Add(New FormSettings.ChainObject("Waifu Vulkan", 1, WaifuNcnnPath, "Waifu2x Vulkan", Me))
            Case "RealSR Vulkan"
                ChainControl.ListItems.Add(New DragDropList.DragDropItem(ChainList.Count, "RealSR Vulkan", ChainThumbs.Item(2)))
                ChainList.Add(New FormSettings.ChainObject("RealSR Vulkan", 2, RealSRNcnnPath, "RealSR Vulkan", Me))
            Case "RealESRGAN Vulkan"
                ChainControl.ListItems.Add(New DragDropList.DragDropItem(ChainList.Count, "RealESRGAN Vulkan", ChainThumbs.Item(8)))
                ChainList.Add(New FormSettings.ChainObject("RealESRGAN Vulkan", 8, RealESRGNcnnPath, "RealESRGAN Vulkan", Me))
            Case "SRMD Vulkan"
                ChainControl.ListItems.Add(New DragDropList.DragDropItem(ChainList.Count, "SRMD Vulkan", ChainThumbs.Item(3)))
                ChainList.Add(New FormSettings.ChainObject("SRMD Vulkan", 3, SRMDNcnnPath, "SRMD Vulkan", Me))
            Case "Waifu2x CPP"
                ChainControl.ListItems.Add(New DragDropList.DragDropItem(ChainList.Count, "Waifu CPP", ChainThumbs.Item(4)))
                ChainList.Add(New FormSettings.ChainObject("Waifu CPP", 4, WaifuCppPath, "Waifu2x CPP", Me))
            Case "Anime4k CPP"
                ChainControl.ListItems.Add(New DragDropList.DragDropItem(ChainList.Count, "Anime4k", ChainThumbs.Item(5)))
                ChainList.Add(New FormSettings.ChainObject("Anime4k", 5, Anime4kPath, "Anime4k CPP", Me))
            Case "TexConv"
                ChainControl.ListItems.Add(New DragDropList.DragDropItem(ChainList.Count, "TexConv", ChainThumbs.Item(7)))
                ChainList.Add(New FormSettings.ChainObject("TexConv", 7, TexConvPath, "TexConv", Me))
            Case "xBRZ"
                ChainControl.ListItems.Add(New DragDropList.DragDropItem(ChainList.Count, "xBRZ", ChainThumbs.Item(0)))
                ChainList.Add(New FormSettings.ChainObject("xBRZ", 0, xBRZPath, "xBRZ", Me))
            Case "ESRGAN"
                ChainControl.ListItems.Add(New DragDropList.DragDropItem(ChainList.Count, "ESRGAN", ChainThumbs.Item(6)))
                ChainList.Add(New FormSettings.ChainObject("ESRGAN", 6, PyPath, "ESRGAN", Me))
        End Select
        ChainControl.DrawList(ChainControl.ListItems)
    End Sub

#End Region

#Region "Commands"

    Private Function MakeCaffeCommand(SourceImage As String, NewImage As String, Package As FormSettings.Waifu2xCaffePackage) As String
        Dim Result As New ArguementString
        Result.AddArguement("-i", Quote(SourceImage))
        Result.AddArguement("-o", Quote(NewImage))
        Result.AddArguement(LoadedSettings.ExpertSettings.ExpertFlags)
        Result.AddArguement("-m", Package.Mode)
        Result.AddArguement("-s", Package.Scale.ToString)
        Result.AddArguement("-n", Package.Noise.ToString)
        Result.AddArguement("-p", Package.Process)
        Result.AddArguement("-t", IIf(Package.TAA = True, 1, 0))
        Return Result.GetArguements
    End Function

    Private Function MakeVulkanCommand(SourceImage As String, NewImage As String, NoNoise As Boolean, Package As FormSettings.VulkanNcnnPackage) As String
        Dim Result As New ArguementString
        Result.AddArguement("-i", Quote(SourceImage))
        Result.AddArguement("-o", Quote(NewImage))
        Result.AddArguement(LoadedSettings.ExpertSettings.ExpertFlags)
        Result.AddArguement("-s", Package.Scale.ToString)
        If NoNoise = False Then Result.AddArguement("-n", Package.Noise.ToString)
        Result.AddArguement("-f", Package.Format)
        Result.AddArguement(IIf(Package.TAA = True, "-x", ""))
        Return Result.GetArguements
    End Function

    Private Function MakeCPPCommand(SourceImage As String, NewImage As String, Package As FormSettings.Waifu2xCppPackage) As String
        Dim Result As New ArguementString
        Result.AddArguement("-i", Quote(SourceImage))
        Result.AddArguement("-o", Quote(Path.ChangeExtension(NewImage, Package.Format)))
        Result.AddArguement(LoadedSettings.ExpertSettings.ExpertFlags)
        Result.AddArguement("-m", Package.Mode)
        Result.AddArguement("--scale-ratio", Package.Scale.ToString)
        Result.AddArguement("--noise-level", Package.Noise.ToString)
        Result.AddArguement("-f", Package.Format)
        Result.AddArguement("--disable-gpu", Package.GPU)
        Result.AddArguement("--force-OpenCL", Package.ForceOpenCL)
        Return Result.GetArguements
    End Function

    Private Function MakeA4KCommand(SourceImage As String, NewImage As String, Package As FormSettings.Anime4kPackage) As String
        Dim Result As New ArguementString
        Result.AddArguement("-i", Quote(SourceImage))
        Result.AddArguement("-o", Quote(NewImage))
        Result.AddArguement(LoadedSettings.ExpertSettings.ExpertFlags)
        Result.AddArguement("-p", Package.Passes.ToString)
        Result.AddArguement("-n", Package.PushColors.ToString)
        Result.AddArguement("-c", Package.PushColorStrength.ToString)
        Result.AddArguement("-g", Package.PushGradStrength.ToString)
        Result.AddArguement("-z", Package.Scale.ToString)
        Result.AddArguement("-b", Package.PreProcess)
        Result.AddArguement("-r " & Package.PreFilterType, Package.PreFilter)
        Result.AddArguement("-a", Package.PostProcess)
        Result.AddArguement("-e " & Package.PostFilterType, Package.PostFilter)
        Result.AddArguement("-q", Package.GPU)
        Result.AddArguement("-w", Package.CNN)
        Result.AddArguement("-A")
        Return Result.GetArguements
    End Function

    Private Function MakeTexConvCommand(SourceImage As String, NewImage As String, Package As FormSettings.DDxPackage) As String
        Dim Result As New ArguementString
        Result.AddArguement("-f", Package.Format)
        Result.AddArguement("-nologo")
        Select Case Package.Mode
            Case "DDS Input"
                Result.AddArguement("-ft " & Package.ConversionFormat.ToLower)
            Case "DDS Output"
                Result.AddArguement("-fl " & Package.FeatureLevel, Package.FeatureLevel <> "11.0")
                Result.AddArguement("-dx9", Package.ForceDx9)
                Result.AddArguement("-dx10", Package.ForceDx10)
        End Select
        Result.AddArguement("-sepalpha", Package.SeperateAlpha)
        Result.AddArguement("-pmalpha", Package.PremultiplyAlpha)
        Result.AddArguement("-alpha", Package.StraightAlpha)
        Result.AddArguement("-o", Quote(Path.GetDirectoryName(NewImage).TrimEnd({"\"c, "/"c})))
        Result.AddArguement(Quote(SourceImage))
        Return Result.GetArguements
    End Function

    Private Function MakeXBRZCommand(SourceImage As String, NewImage As String, Package As FormSettings.xBRZPackage) As String
        Dim Result As New ArguementString
        Result.AddArguement("", "-" & CInt(Package.Scale) & "xBRZ")
        Result.AddArguement("", Quote(SourceImage))
        Result.AddArguement("", Quote(NewImage))
        Return Result.GetArguements
    End Function

    Private Function MakePyCommand(SourceFolder As String, DestFolder As String, Package As FormSettings.PythonPackage) As String
        Dim Result As New ArguementString
        Result.AddArguement(Quote(Package.Model))
        Result.AddArguement("--input", Quote(SourceFolder))
        Result.AddArguement("--output", Quote(DestFolder))
        Result.AddArguement("--tile_size", Package.TileSize.ToString)
        Result.AddArguement("--cpu", Package.CPUOnly.ToString)
        Return Result.GetArguements
    End Function

#End Region

#Region "Graphics"

    Private Function MakeSeamless(Source As Bitmap, Mirrored As Integer, Margin As Integer) As Bitmap
        Dim Result As New Bitmap(Source.Width * 3, Source.Height * 3, Source.PixelFormat)
        Using g As Graphics = Graphics.FromImage(Result)
            g.CompositingMode = Drawing2D.CompositingMode.SourceCopy
            g.PixelOffsetMode = Drawing2D.PixelOffsetMode.None
            g.SmoothingMode = Drawing2D.SmoothingMode.None
            g.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
            If Mirrored = 2 Then
                Dim X = Source.Width
                Dim Y = Source.Height
                Dim fX As New Bitmap(Source) : fX.RotateFlip(RotateFlipType.RotateNoneFlipX)
                Dim fY As New Bitmap(Source) : fY.RotateFlip(RotateFlipType.RotateNoneFlipY)
                Dim fXY As New Bitmap(Source) : fXY.RotateFlip(RotateFlipType.RotateNoneFlipXY)
                g.DrawImage(fXY, 0, 0, X, Y) : g.DrawImage(fY, X, 0, X, Y) : g.DrawImage(fXY, 2 * X, 0, X, Y)
                g.DrawImage(fX, 0, Y, X, Y) : g.DrawImage(Source, X, Y, X, Y) : g.DrawImage(fX, 2 * X, Y, X, Y)
                g.DrawImage(fXY, 0, 2 * Y, X, Y) : g.DrawImage(fY, X, 2 * Y, X, Y) : g.DrawImage(fXY, 2 * X, 2 * Y, X, Y)
            ElseIf Mirrored = 1 Then
                For i = 0 To Source.Width * 2 Step Source.Width
                    For j = 0 To Source.Height * 2 Step Source.Height
                        g.DrawImage(Source, i, j, Source.Width, Source.Height)
                    Next
                Next
            Else
                Return Source
            End If
        End Using
        Return CropImage(Result, Source.Width, Source.Height, Source.Width, Source.Height, Margin)
    End Function

    Private Function CropImage(Source As Bitmap, OffsetX As Integer, OffsetY As Integer, Width As Integer, Height As Integer, Margins As Integer) As Bitmap
        Dim CropSize As New Rectangle(OffsetX - Margins, OffsetY - Margins, Width + (2 * Margins), Height + (2 * Margins))
        Dim Result = New Bitmap(CropSize.Width, CropSize.Height, Source.PixelFormat)
        Using g As Graphics = Graphics.FromImage(Result)
            g.CompositingMode = Drawing2D.CompositingMode.SourceCopy
            g.PixelOffsetMode = Drawing2D.PixelOffsetMode.None
            g.SmoothingMode = Drawing2D.SmoothingMode.None
            g.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
            g.DrawImage(Source, New Rectangle(0, 0, CropSize.Width, CropSize.Height), CropSize, GraphicsUnit.Pixel)
        End Using
        Return Result
    End Function

    Private Sub Defringe(Source As String, Threshold As Integer)
        Dim NewImage As New DirectBitmap(GetUnlockedImage(Source))
        For X = 0 To NewImage.Width - 1
            For Y = 0 To NewImage.Height - 1
                If NewImage.GetPixel(X, Y).A <Threshold Then
                    NewImage.SetPixel(X, Y, Color.Transparent)
                End If
            Next
        Next
        NewImage.Bitmap.Save(Source)
    End Sub

    Private Sub RemovePS2Alpha(Source As String)
        Dim NewImage As New DirectBitmap(GetUnlockedImage(Source))
        Dim AlphaMax As Integer = 0
        For X = 0 To NewImage.Width - 1
            For Y = 0 To NewImage.Height - 1
                Dim TempColor As Color = NewImage.GetPixel(X, Y)
                If TempColor.A > AlphaMax Then
                    AlphaMax = TempColor.A
                End If
                If Not AlphaMax <= 128 Then
                    NewImage.Dispose()
                    Exit Sub
                End If
                If TempColor.A <> 0 Then
                    NewImage.SetPixel(X, Y, Color.FromArgb((TempColor.A * 2) - 1, TempColor.R, TempColor.G, TempColor.B))
                End If
            Next
        Next
        NewImage.Bitmap.Save(Source)
    End Sub

    Private Sub AddPS2Alpha(Source As String)
        Dim NewImage As New DirectBitmap(GetUnlockedImage(Source))
        For X = 0 To NewImage.Width - 1
            For Y = 0 To NewImage.Height - 1
                Dim TempColor As Color = NewImage.GetPixel(X, Y)
                If TempColor.A <> 0 Then
                    NewImage.SetPixel(X, Y, Color.FromArgb((TempColor.A + 1) / 2, TempColor.R, TempColor.G, TempColor.B))
                End If
            Next
        Next
        NewImage.Bitmap.Save(Source)
    End Sub

#End Region

#Region "XML"

    Public Shared Function Serialize(Of T)(Source As T) As String
        Dim Result As String = ""
        Using XmlStream As New MemoryStream
            Dim XmlSerializer As New Xml.Serialization.XmlSerializer(GetType(T))
            Dim XmlSettings As New Xml.XmlWriterSettings With {.Indent = True, .CloseOutput = True}
            Dim XmlWriter As Xml.XmlWriter = Xml.XmlWriter.Create(XmlStream, XmlSettings)
            XmlSerializer.Serialize(XmlWriter, Source)
            Dim XmlReader As New StreamReader(XmlStream)
            XmlStream.Position = 0
            Result = XmlReader.ReadToEnd()
            XmlWriter.Flush()
            XmlWriter.Close()
            XmlReader.Dispose()
        End Using
        Return Result
    End Function

    Public Shared Function Deserialize(Of T)(Xml As String) As T
        Dim Result As New Object
        Using XmlStream As New MemoryStream
            Dim XmlSerializer As New Xml.Serialization.XmlSerializer(GetType(T))
            Dim XmlWriter As New StreamWriter(XmlStream)
            XmlWriter.Write(Xml)
            XmlWriter.Flush()
            XmlStream.Position = 0
            Dim XmlReader As New Xml.XmlTextReader(XmlStream)
            If XmlSerializer.CanDeserialize(XmlReader) Then
                Result = DirectCast(XmlSerializer.Deserialize(XmlReader), T)
            End If
            XmlWriter.Close()
            XmlReader.Dispose()
        End Using
        Return Result
    End Function

#End Region

#Region "Utils"

    Private Declare Function GetActiveWindow Lib "user32" Alias "GetActiveWindow" () As IntPtr

    Private Function GetMissingFiles(Path1 As String, Path2 As String) As String()
        Dim Result As New List(Of String)
        Dim Path1MasterList = Directory.GetFiles(Path1, "*.*", SearchOption.AllDirectories)
        Dim Path1List = Directory.GetFiles(Path1, "*.*", SearchOption.AllDirectories).ToList
        Dim Path2List = Directory.GetFiles(Path2, "*.*", SearchOption.AllDirectories).ToList
        For i = 0 To Path1List.Count - 1
            Path1List(i) = Path.GetFileNameWithoutExtension(Path1List(i)).ToLower
        Next
        For i = 0 To Path2List.Count - 1
            Path2List(i) = Path.GetFileNameWithoutExtension(Path2List(i)).ToLower
        Next
        For i = 0 To Path1List.Count - 1
            If Not Path2List.Contains(Path1List(i)) Then Result.Add(Path1MasterList(i))
        Next
        Return Result.ToArray
    End Function

    Private Function GetThreads(Index As Integer, Count As Integer)
        Select Case Index
            Case 0
                Return 1
            Case 1
                Return Count
            Case 2
                Return Environment.ProcessorCount
        End Select
        Return 512
    End Function

    Private Function GetUnlockedImage(Source As String) As Bitmap
        Dim SourceImage As Bitmap = Image.FromFile(Source)
        Dim UnlockedImage As New Bitmap(SourceImage)
        SourceImage.Dispose()
        Return UnlockedImage
    End Function

    Private Function GetFolder() As String
        Using FBD As New FolderBrowserDialog
            If FBD.ShowDialog = DialogResult.OK Then
                Return FBD.SelectedPath
            End If
        End Using
        Return ""
    End Function

    Private Function GetFolder(Path As String) As String
        Using FBD As New FolderBrowserDialog
            FBD.SelectedPath = Path
            If FBD.ShowDialog = DialogResult.OK Then
                Return FBD.SelectedPath
            End If
        End Using
        Return Path
    End Function

    Private Function Quote(Source As String) As String
        Return ControlChars.Quote & Source & ControlChars.Quote
    End Function

    Private Sub WriteLog(Source As Process, SaveLoc As String)
        Dim Filename As String = SaveLoc & "\Log_" & Now.ToString("yyyy-MM-dd_HH-mm-ss") & ".txt"
        Dim Output As String = ""
        Output += Source.StartInfo.FileName & " "
        Output += Source.StartInfo.Arguments
        Output += vbNewLine & vbNewLine
        Output += Source.StandardOutput.ReadToEnd
        Output += vbNewLine & vbNewLine
        Output += Source.StandardError.ReadToEnd
        File.WriteAllText(Filename, Output)
    End Sub

#End Region

End Class