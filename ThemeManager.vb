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
End Class
