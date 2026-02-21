Public Class clsSettings
    Private Const DEFAULT_EDITOR = "notepad"
    Private Const DEFAULT_NOTEPAD_PATH = "notepad.exe"
    Private Const DEFAULT_SAKURA_PATH = "C:\Program Files\sakura\sakura.exe"
    Private Const DEFAULT_HIDEMARU_PATH = "C:\Program Files\Hidemaru\Hidemaru.exe"
    Private Const DEFAULT_PROGRAMID_PATTERN = ""
    '    Private Const DEFAULT_PROGRAMID_PATTERN = "(...[OB]Z\d\d\d\d)"
    Private regkey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\CRUDExplorer")
    Public Enum enmListDblClickMode As Integer
        ExecTextEditor = 0
        AnalyzeQuery = 1
        NoAction = 2
    End Enum

    Public TextEditor As String
    Public NotepadPath As String
    Public SakuraPath As String
    Public HidemaruPath As String
    Public ListDblClickMode As enmListDblClickMode
    Public ProgramIdPattern As String
    Public DebugMode As Boolean
    Public LicenseKey As String = ""
    Public EMailAddr As String = ""

    Public Sub LoadSettings()
        TextEditor = regkey.GetValue("TextEditor")
        NotepadPath = regkey.GetValue("NotepadPath")
        SakuraPath = regkey.GetValue("SakuraPath")
        HidemaruPath = regkey.GetValue("HidemaruPath")
        ListDblClickMode = regkey.GetValue("ListDblClickMode")
        ProgramIdPattern = regkey.GetValue("ProgramIdPattern")
        DebugMode = regkey.GetValue("DebugMode")
        LicenseKey = regkey.GetValue("LicenseKey")
        EMailAddr = regkey.GetValue("EMailAddr")

        If TextEditor = "" Then TextEditor = DEFAULT_EDITOR
        If NotepadPath = "" Then NotepadPath = DEFAULT_NOTEPAD_PATH
        If SakuraPath = "" Then SakuraPath = DEFAULT_SAKURA_PATH
        If HidemaruPath = "" Then HidemaruPath = DEFAULT_HIDEMARU_PATH
        If ProgramIdPattern = "" Then ProgramIdPattern = DEFAULT_PROGRAMID_PATTERN
        If LicenseKey Is Nothing Then LicenseKey = ""

    End Sub

    Public Sub SaveSettings()
        regkey.SetValue("TextEditor", TextEditor)
        regkey.SetValue("NotepadPath", NotepadPath)
        regkey.SetValue("SakuraPath", SakuraPath)
        regkey.SetValue("HidemaruPath", HidemaruPath)
        regkey.SetValue("ListDblClickMode", CInt(ListDblClickMode))
        regkey.SetValue("ProgramIdPattern", ProgramIdPattern)
        regkey.SetValue("DebugMode", CInt(DebugMode))
        regkey.SetValue("LicenseKey", LicenseKey)
        On Error Resume Next
        regkey.SetValue("EMailAddr", EMailAddr)
    End Sub

    Public Sub SaveFormSize(ByVal objForm As Windows.Forms.Form)
        regkey.SetValue(objForm.Name & "_w", objForm.Width)
        regkey.SetValue(objForm.Name & "_h", objForm.Height)
        regkey.SetValue(objForm.Name & "_l", objForm.Left)
        regkey.SetValue(objForm.Name & "_t", objForm.Top)
        regkey.SetValue(objForm.Name & "_m", objForm.WindowState = FormWindowState.Maximized)
    End Sub

    Public Sub LoadFormSize(ByVal objForm As Windows.Forms.Form)
        On Error Resume Next
        objForm.Width = regkey.GetValue(objForm.Name & "_w", objForm.Width)
        objForm.Height = regkey.GetValue(objForm.Name & "_h", objForm.Height)
        objForm.Left = regkey.GetValue(objForm.Name & "_l", objForm.Left)
        objForm.Top = regkey.GetValue(objForm.Name & "_t", objForm.Top)
        If regkey.GetValue(objForm.Name & "_m", objForm.WindowState = FormWindowState.Maximized) Then
            objForm.WindowState = FormWindowState.Maximized
        End If
        '        objForm.Left = (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - objForm.Width) / 2
        '       objForm.Top = (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - objForm.Height) / 2
    End Sub
End Class
