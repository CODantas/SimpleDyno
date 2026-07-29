Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' Ties ColorPalette + TypographyManager together and applies them to WinForms controls.
''' Fase 0 of the UI modernization plan - no existing form calls this yet, so it has zero
''' effect on the running app until a later phase opts in. Pure presentation - no reference
''' to Main or any calculation/acquisition code.
''' </summary>
Public NotInheritable Class ThemeManager
    Private Sub New()
    End Sub

    ''' <summary>
    ''' Applies the dark theme's background/foreground colors (and, optionally, its base UI font)
    ''' to a single control. Does not recurse into child controls - callers decide the scope.
    ''' </summary>
    Public Shared Sub ApplyBaseStyle(control As Control, Optional useCardBackground As Boolean = False, Optional applyFont As Boolean = True)
        control.BackColor = If(useCardBackground, ColorPalette.CardBackground, ColorPalette.Surface)
        control.ForeColor = ColorPalette.TextPrimary
        If applyFont Then
            control.Font = TypographyManager.UiFont(9.0F)
        End If
    End Sub

    ''' <summary>Recursively applies ApplyBaseStyle to a control and every descendant.</summary>
    Public Shared Sub ApplyThemeRecursively(root As Control)
        ApplyBaseStyle(root)
        For Each child As Control In root.Controls
            ApplyThemeRecursively(child)
        Next
    End Sub

    Private Const LightSettingValue As String = "Claro"
    Private Const DarkSettingValue As String = "Escuro"

    ''' <summary>
    ''' Switches ColorPalette.Current and persists the choice to My.Settings.Tema (User scope,
    ''' same mechanism as the existing Idioma language setting) so it survives a restart. Setting
    ''' ColorPalette.Current raises ThemeChanged, which every open widget/window is already
    ''' listening for - callers never need to manually repaint anything.
    ''' </summary>
    Public Shared Sub SetTheme(kind As ColorPalette.ThemeKind)
        ColorPalette.Current = kind
        Try
            My.Settings.Tema = If(kind = ColorPalette.ThemeKind.Light, LightSettingValue, DarkSettingValue)
            My.Settings.Save()
        Catch
            'A failed settings write (e.g. read-only profile folder) should never block the live
            'theme switch the user just asked for - it just won't be remembered next launch.
        End Try
    End Sub

    Public Shared Sub ToggleTheme()
        SetTheme(If(ColorPalette.Current = ColorPalette.ThemeKind.Dark, ColorPalette.ThemeKind.Light, ColorPalette.ThemeKind.Dark))
    End Sub

    ''' <summary>
    ''' Restores the last-saved theme choice into ColorPalette.Current. Call once, at application
    ''' startup, before any themed control/widget is created (see MyApplication_Startup) - unlike
    ''' SetTheme/ToggleTheme this does not re-save the setting it just read.
    ''' </summary>
    Public Shared Sub LoadSavedTheme()
        Dim saved As String = DarkSettingValue
        Try
            saved = My.Settings.Tema
        Catch
            'No persisted value yet (first run) - keep the Dark default.
        End Try
        ColorPalette.Current = If(saved = LightSettingValue, ColorPalette.ThemeKind.Light, ColorPalette.ThemeKind.Dark)
    End Sub
End Class
