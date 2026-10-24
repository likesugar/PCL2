Public Class PageDiy

    Private Sub PageDiy_Inited(sender As Object, e As EventArgs) Handles Me.Initialized
        LabWindowSize.Text = "当前窗口大小：" & CInt(FrmMain.ActualWidth) & " × " & CInt(FrmMain.ActualHeight)
    End Sub

    '背景
    Private Sub BtnBgDefault_Click(sender As Object, e As EventArgs) Handles BtnBgDefault.Click
        For Each F In Directory.GetFiles(PathTemp & "Backgrounds")
            FileUtils.Delete(F)
        Next
        BackgroundRefresh(False, True)
        Hint("已恢复默认背景。")
    End Sub
    Private Sub BtnBgCustom_Click(sender As Object, e As EventArgs) Handles BtnBgCustom.Click
        Dim Addr = Dialogs.SelectFile("选择背景图片", False, filter:={({"jpg", "jpeg", "png", "webp", "bmp"}, "常用图片文件")}).FirstOrDefault()
        If Addr Is Nothing OrElse Not FileUtils.Exists(Addr) Then Return
        Dim NewPath As String = PathTemp & "Backgrounds" & GetUuid() & PathUtils.GetExtension(Addr)
        Directory.CreateDirectory(PathTemp & "Backgrounds")
        FileUtils.Copy(Addr, NewPath)
        BackgroundRefresh(False, True)
        Hint("背景图片已更换！")
    End Sub

    '花瓣颜色
    Private Sub BtnSakuraColor_Click(sender As Object, e As EventArgs) Handles BtnSakuraColor.Click
        Dim Dialog = New Forms.ColorDialog With {.FullOpen = True, .Color = CType(ColorConverter.ConvertFromString(Settings.Get(Of String)("UiSakuraColor")), Drawing.Color)}
        If Dialog.ShowDialog() = Forms.DialogResult.OK Then
            Settings.Set("UiSakuraColor", "#" & Dialog.Color.Name)
            Hint("花瓣颜色已更新。")
        End If
    End Sub

    '主题色预设
    Private Sub SetTheme(Hue As Integer, Sat As Integer, Light As Integer)
        Settings.Set("UiLauncherHue", Hue)
        Settings.Set("UiLauncherSat", Sat)
        Settings.Set("UiLauncherLight", Light)
        Settings.Set("UiLauncherTheme", 14)
        ThemeRefresh()
        Hint("主题色已更换！")
    End Sub
    Private Sub BtnThemePink_Click(sender As Object, e As EventArgs) Handles BtnThemePink.Click
        SetTheme(330, 70, 0)
    End Sub
    Private Sub BtnThemeBlue_Click(sender As Object, e As EventArgs) Handles BtnThemeBlue.Click
        SetTheme(210, 85, 0)
    End Sub
    Private Sub BtnThemePurple_Click(sender As Object, e As EventArgs) Handles BtnThemePurple.Click
        SetTheme(270, 70, 0)
    End Sub
    Private Sub BtnThemeGreen_Click(sender As Object, e As EventArgs) Handles BtnThemeGreen.Click
        SetTheme(120, 60, 0)
    End Sub
    Private Sub BtnThemeOrange_Click(sender As Object, e As EventArgs) Handles BtnThemeOrange.Click
        SetTheme(30, 90, 0)
    End Sub

    '窗口大小
    Private Sub BtnWindowSize_Click(sender As Object, e As EventArgs) Handles BtnWindowSize.Click
        Settings.Set("WindowHeight", 480.0)
        Settings.Set("WindowWidth", 800.0)
        FrmMain.Width = 800 : FrmMain.Height = 480
        LabWindowSize.Text = "当前窗口大小：800 × 480"
        Hint("窗口大小已恢复默认。")
    End Sub

    '恢复默认
    Private Sub BtnResetAll_Click(sender As Object, e As EventArgs) Handles BtnResetAll.Click
        If MyMsgBox("将重置全部 DIY 设置（背景、樱花、主题色、界面调整），确定继续？", "恢复默认设置", "确定", "取消", IsWarn:=True) = 1 Then
            Settings.Set("UiSakuraEnabled", False)
            Settings.Set("UiSakuraCount", 15)
            Settings.Set("UiSakuraStyle", 0)
            Settings.Set("UiSakuraPauseOnBlur", True)
            Settings.Set("UiSakuraColor", "#FFB3C6")
            Settings.Set("UiBackgroundBlur", 0)
            Settings.Set("UiBackgroundOpacity", 1000)
            Settings.Set("UiWindowRadius", 6)
            For Each F In Directory.GetFiles(PathTemp & "Backgrounds")
                FileUtils.Delete(F)
            Next
            BackgroundRefresh(False, True)
            Hint("已恢复默认 DIY 设置。")
        End If
    End Sub

End Class
