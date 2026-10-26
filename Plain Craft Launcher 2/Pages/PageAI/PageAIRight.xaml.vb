Public Class PageAIRight

    Private ChatHistory As New List(Of KeyValuePair(Of String, String)) 'role, content
    Private IsBusy As Boolean = False

    Private Const DefaultBase As String = "https://api.deepseek.com/v1"

    Private Sub PageAIRight_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If ComboBase.Items.Count = 0 Then
            ComboBase.Items.Add(DefaultBase)
            ComboBase.Items.Add("https://api.openai.com/v1")
            ComboBase.Items.Add("https://api.moonshot.cn/v1")
            ComboBase.Text = Settings.Get(Of String)("AIClientBase")
            If ComboBase.Text = "" Then ComboBase.Text = DefaultBase
            ComboModel.Text = Settings.Get(Of String)("AIClientModel")
            TextKey.Text = Settings.Get(Of String)("AIClientKey")
            ChkEnabled.IsChecked = Settings.Get(Of Boolean)("AIClientEnabled")
            ChkNetAccess.IsChecked = Settings.Get(Of Boolean)("AIClientNetAccess")
            ComboSafety.SelectedIndex = If(Settings.Get(Of Boolean)("AIClientAutoExec"), 1, 0)
        End If
    End Sub

    Private Sub BtnSaveConfig_Click(sender As Object, e As EventArgs) Handles BtnSaveConfig.Click
        Settings.Set("AIClientBase", ComboBase.Text.Trim())
        Settings.Set("AIClientModel", ComboModel.Text.Trim())
        Settings.Set("AIClientKey", TextKey.Text.Trim())
        Settings.Set("AIClientEnabled", ChkEnabled.IsChecked = True)
        Settings.Set("AIClientNetAccess", ChkNetAccess.IsChecked = True)
        Settings.Set("AIClientAutoExec", ComboSafety.SelectedIndex = 1)
        Hint("AI 助手设置已保存！", HintType.Green)
    End Sub

    Private Sub BtnGetModels_Click(sender As Object, e As EventArgs) Handles BtnGetModels.Click
        If IsBusy Then Return
        Dim Base As String = ComboBase.Text.Trim().TrimEnd("/"c)
        Dim Key As String = TextKey.Text.Trim()
        If Base = "" Then Hint("请先填写接口地址！", HintType.Red) : Return
        BtnGetModels.Text = "获取中…"
        RunInNewThread(Sub()
                           Try
                               Dim Json As String = NetRequestCore(Base & "/models", "GET", Key, Nothing)
                               Dim Models As New List(Of String)
                               Dim Matches As System.Text.RegularExpressions.MatchCollection =
                                   System.Text.RegularExpressions.Regex.Matches(Json, """id""\s*:\s*""([^""]+)""")
                               For Each M As System.Text.RegularExpressions.Match In Matches
                                   Models.Add(M.Groups(1).Value)
                               Next
                               RunInUi(Sub()
                                           ComboModel.Items.Clear()
                                           For Each M In Models.Take(50) : ComboModel.Items.Add(M) : Next
                                           If ComboModel.Text = "" AndAlso Models.Any() Then ComboModel.Text = Models(0)
                                           Hint($"获取到 {Models.Count} 个模型", If(Models.Any, HintType.Green, HintType.Red))
                                           BtnGetModels.Text = "获取模型"
                                       End Sub)
                           Catch ex As Exception
                               RunInUi(Sub()
                                           Hint("获取模型失败：" & ex.Message, HintType.Red)
                                           BtnGetModels.Text = "获取模型"
                                       End Sub)
                           End Try
                       End Sub)
    End Sub

    Private Sub BtnClearChat_Click(sender As Object, e As EventArgs) Handles BtnClearChat.Click
        ChatHistory.Clear()
        PanChatList.Children.Clear()
    End Sub

    Private Sub TextInput_KeyDown(sender As Object, e As Input.KeyEventArgs) Handles TextInput.KeyDown
        If e.Key = Input.Key.Enter AndAlso Not (Input.Keyboard.Modifiers And Input.ModifierKeys.Shift) = Input.ModifierKeys.Shift Then
            BtnSend_Click(Nothing, Nothing)
            e.Handled = True
        End If
    End Sub

    Private Sub BtnSend_Click(sender As Object, e As EventArgs) Handles BtnSend.Click
        If IsBusy Then Hint("AI 正在回复中…", HintType.Orange) : Return
        If ChkEnabled.IsChecked = False Then Hint("请先启用 AI 助手并保存设置！", HintType.Red) : Return
        Dim Msg As String = TextInput.Text.Trim()
        If Msg = "" Then Return
        If Settings.Get(Of String)("AIClientKey") = "" Then Hint("请先填写 API Key！", HintType.Red) : Return
        TextInput.Text = ""
        AddBubble("user", Msg)
        ChatHistory.Add(New KeyValuePair(Of String, String)("user", Msg))
        IsBusy = True
        BtnSend.Text = "思考中…"
        Dim Base As String = Settings.Get(Of String)("AIClientBase").TrimEnd("/"c)
        If Base = "" Then Base = DefaultBase
        Dim Key As String = Settings.Get(Of String)("AIClientKey")
        Dim Model As String = Settings.Get(Of String)("AIClientModel")
        RunInNewThread(Sub()
                           Try
                               Dim Body As New System.Text.StringBuilder
                               Body.Append("{""model"":""" & JsonEscape(Model) & """,""stream"":false,""messages"":[")
                               Dim First As Boolean = True
                               For Each H In ChatHistory
                                   If Not First Then Body.Append(",")
                                   Body.Append("{""role"":""" & H.Key & """,""content"":""" & JsonEscape(H.Value) & """}")
                                   First = False
                               Next
                               Body.Append("]}")
                               Dim Reply As String = NetRequestCore(Base & "/chat/completions", "POST", Key, Body.ToString())
                               Dim Content As String = ExtractJsonString(Reply, "content")
                               If Content = "" Then Content = "（返回解析失败）" & vbCrLf & Reply.Substring(0, Math.Min(300, Reply.Length))
                               RunInUi(Sub()
                                           AddBubble("assistant", Content)
                                           ChatHistory.Add(New KeyValuePair(Of String, String)("assistant", Content))
                                           IsBusy = False
                                           BtnSend.Text = "发送"
                                       End Sub)
                           Catch ex As Exception
                               RunInUi(Sub()
                                           AddBubble("assistant", "请求失败：" & ex.Message)
                                           IsBusy = False
                                           BtnSend.Text = "发送"
                                       End Sub)
                           End Try
                       End Sub)
    End Sub

    Private Sub AddBubble(Role As String, Content As String)
        Dim Tb As New TextBlock With {
            .TextWrapping = TextWrapping.Wrap,
            .Margin = New Thickness(0, 4, 0, 4),
            .FontSize = 13
        }
        If Role = "user" Then
            Dim Run As New Run("你：" & Content) With {.Foreground = New SolidColorBrush(Color.FromRgb(0x2A, 0x2D, 0x32))}
            Tb.Inlines.Add(Run)
        Else
            Dim Run As New Run("AI：" & Content) With {.Foreground = New SolidColorBrush(Color.FromRgb(0x22, 0x66, 0xCC))}
            Tb.Inlines.Add(Run)
        End If
        PanChatList.Children.Add(Tb)
        PanBack.ScrollToEnd()
    End Sub

    Private Shared Function JsonEscape(S As String) As String
        If S Is Nothing Then Return ""
        Return S.Replace("\", "\\").Replace("""", "\""").Replace(vbCr, "").Replace(vbLf, "\n").Replace(vbTab, "\t")
    End Function

    Private Shared Function ExtractJsonString(Json As String, Field As String) As String
        Try
            Dim M As System.Text.RegularExpressions.Match = System.Text.RegularExpressions.Regex.Match(
                Json, """" & Field & """\s*:\s*""((?:[^""\\]|\\.)*)""")
            If Not M.Success Then Return ""
            Return M.Groups(1).Value.Replace("\n", vbCrLf).Replace("\""", """").Replace("\\", "\")
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Shared Function NetRequestCore(Url As String, Method As String, Key As String, Body As String) As String
        Dim Req As Net.HttpWebRequest = CType(Net.WebRequest.Create(Url), Net.HttpWebRequest)
        Req.Method = Method
        Req.Timeout = 60000
        Req.ContentType = "application/json"
        Req.UserAgent = "PCL2-AIClient"
        If Key <> "" Then Req.Headers.Add("Authorization", "Bearer " & Key)
        If Method = "POST" AndAlso Body IsNot Nothing Then
            Dim Bs = System.Text.Encoding.UTF8.GetBytes(Body)
            Req.ContentLength = Bs.Length
            Using S = Req.GetRequestStream()
                S.Write(Bs, 0, Bs.Length)
            End Using
        End If
        Using Resp = CType(Req.GetResponse(), Net.HttpWebResponse)
            Using Sr As New IO.StreamReader(Resp.GetResponseStream(), System.Text.Encoding.UTF8)
                Return Sr.ReadToEnd()
            End Using
        End Using
    End Function

End Class
