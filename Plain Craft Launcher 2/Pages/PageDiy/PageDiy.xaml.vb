Public Class PageDiy

    Private IsLoading As Boolean = True

    Private Sub PageDiy_Loaded() Handles Me.Loaded
        '读取设置
        CheckDiySakura.Checked = Settings.Get(Of Boolean)("DiySakura")
        CheckDiySakuraPause.Checked = Settings.Get(Of Boolean)("DiySakuraPause")
        SliderDiySakuraCount.Value = Math.Max(5, Math.Min(35, Settings.Get(Of Integer)("DiySakuraCount")))
        LabDiySakuraCount.Text = SliderDiySakuraCount.Value
        SliderDiyBlur.Value = Settings.Get(Of Integer)("UiBackgroundBlur")
        LabDiyBlur.Text = SliderDiyBlur.Value
        SliderDiyRadius.Value = Settings.Get(Of Integer)("DiyWindowRadius")
        LabDiyRadius.Text = SliderDiyRadius.Value
        SliderDiyOpacity.Value = Settings.Get(Of Integer)("DiyWindowOpacity")
        LabDiyOpacity.Text = SliderDiyOpacity.Value
        RefreshStyleButtons()
        RefreshColorBlocks()
        IsLoading = False
    End Sub

    '樱花动画
    Private Sub CheckDiySakura_Change(sender As Object, user As Boolean) Handles CheckDiySakura.Change
        If IsLoading OrElse Not user Then Return
        Settings.Set("DiySakura", CheckDiySakura.Checked)
        FrmMain.SakuraRefresh()
    End Sub
    Private Sub CheckDiySakuraPause_Change(sender As Object, user As Boolean) Handles CheckDiySakuraPause.Change
        If IsLoading OrElse Not user Then Return
        Settings.Set("DiySakuraPause", CheckDiySakuraPause.Checked)
    End Sub
    Private Sub SliderDiySakuraCount_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles SliderDiySakuraCount.ValueChanged
        If IsLoading Then Return
        LabDiySakuraCount.Text = SliderDiySakuraCount.Value
        Settings.Set("DiySakuraCount", CInt(SliderDiySakuraCount.Value))
        FrmMain.SakuraRefresh()
    End Sub
    Private Sub SakuraStyleSet(StyleName As String)
        Settings.Set("DiySakuraStyle", StyleName)
        RefreshStyleButtons()
        FrmMain.SakuraRefresh()
    End Sub
    Private Sub RefreshStyleButtons()
        Dim Current = Settings.Get(Of String)("DiySakuraStyle")
        For Each B As MyButton In {BtnDiyStyleSakura, BtnDiyStyleLeaf, BtnDiyStyleSnow, BtnDiyStyleStar}
            B.ColorType = If(B.Name.EndsWith(Current), MyButton.ColorState.Highlight, MyButton.ColorState.Normal)
        Next
    End Sub
    Private Sub BtnDiyStyleSakura_Click(sender As Object, e As EventArgs) Handles BtnDiyStyleSakura.Click
        SakuraStyleSet("Sakura")
    End Sub
    Private Sub BtnDiyStyleLeaf_Click(sender As Object, e As EventArgs) Handles BtnDiyStyleLeaf.Click
        SakuraStyleSet("Leaf")
    End Sub
    Private Sub BtnDiyStyleSnow_Click(sender As Object, e As EventArgs) Handles BtnDiyStyleSnow.Click
        SakuraStyleSet("Snow")
    End Sub
    Private Sub BtnDiyStyleStar_Click(sender As Object, e As EventArgs) Handles BtnDiyStyleStar.Click
        SakuraStyleSet("Star")
    End Sub

    '界面调整
    Private Sub SliderDiyBlur_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles SliderDiyBlur.ValueChanged
        If IsLoading Then Return
        LabDiyBlur.Text = SliderDiyBlur.Value
        Settings.Set("UiBackgroundBlur", CInt(SliderDiyBlur.Value))
        PageSetupUI.BackgroundRefresh(False, True)
    End Sub
    Private Sub SliderDiyRadius_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles SliderDiyRadius.ValueChanged
        If IsLoading Then Return
        LabDiyRadius.Text = SliderDiyRadius.Value
        Settings.Set("DiyWindowRadius", CInt(SliderDiyRadius.Value))
        FrmMain.RectForm.RadiusX = SliderDiyRadius.Value
        FrmMain.RectForm.RadiusY = SliderDiyRadius.Value
    End Sub
    Private Sub SliderDiyOpacity_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles SliderDiyOpacity.ValueChanged
        If IsLoading Then Return
        LabDiyOpacity.Text = SliderDiyOpacity.Value
        Settings.Set("DiyWindowOpacity", CInt(SliderDiyOpacity.Value))
        FrmMain.Opacity = SliderDiyOpacity.Value / 100.0
    End Sub


    '花瓣颜色（图1：色块选择）
    Private SakuraColors() As String = {"#FAB4C8", "#FFFFFF", "#FF8FA3", "#C3A6E8", "#A3C9F7", "#F7D794"}
    Private Sub SakuraColorPick(Idx As Integer)
        Settings.Set("DiySakuraColor", SakuraColors(Idx))
        RefreshColorBlocks()
        FrmMain.SakuraRefresh()
    End Sub
    Private Sub RefreshColorBlocks()
        Dim Current = Settings.Get(Of String)("DiySakuraColor").ToLower()
        For I = 0 To 5
            Dim B As Border = CType(FindName("DiySakuraColor" & I), Border)
            If B IsNot Nothing Then B.BorderBrush = If(SakuraColors(I).ToLower() = Current, Media.Brushes.DodgerBlue, Media.Brushes.Transparent)
        Next
    End Sub
    Private Sub DiySakuraColor0_MouseLeftButtonDown(sender As Object, e As Input.MouseButtonEventArgs) Handles DiySakuraColor0.MouseLeftButtonDown
        SakuraColorPick(0)
    End Sub
    Private Sub DiySakuraColor1_MouseLeftButtonDown(sender As Object, e As Input.MouseButtonEventArgs) Handles DiySakuraColor1.MouseLeftButtonDown
        SakuraColorPick(1)
    End Sub
    Private Sub DiySakuraColor2_MouseLeftButtonDown(sender As Object, e As Input.MouseButtonEventArgs) Handles DiySakuraColor2.MouseLeftButtonDown
        SakuraColorPick(2)
    End Sub
    Private Sub DiySakuraColor3_MouseLeftButtonDown(sender As Object, e As Input.MouseButtonEventArgs) Handles DiySakuraColor3.MouseLeftButtonDown
        SakuraColorPick(3)
    End Sub
    Private Sub DiySakuraColor4_MouseLeftButtonDown(sender As Object, e As Input.MouseButtonEventArgs) Handles DiySakuraColor4.MouseLeftButtonDown
        SakuraColorPick(4)
    End Sub
    Private Sub DiySakuraColor5_MouseLeftButtonDown(sender As Object, e As Input.MouseButtonEventArgs) Handles DiySakuraColor5.MouseLeftButtonDown
        SakuraColorPick(5)
    End Sub
End Class
