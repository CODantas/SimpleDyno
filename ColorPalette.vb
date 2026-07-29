Imports System.Drawing

''' <summary>
''' Central color palette for the modernized UI. Holds a Dark and a Light set and exposes only
''' the currently active one - never hardcode a hex value against these properties elsewhere,
''' always go through ColorPalette so both themes stay in sync automatically. Switch themes via
''' ThemeManager.SetTheme/ToggleTheme, not by setting Current directly, so the choice also gets
''' persisted to My.Settings.
''' </summary>
Public NotInheritable Class ColorPalette
    Private Sub New()
    End Sub

    Public Enum ThemeKind
        Dark
        Light
    End Enum

    ''' <summary>Raised after Current changes, so every open widget/window can repaint itself.</summary>
    Public Shared Event ThemeChanged As EventHandler

    Private Shared _current As ThemeKind = ThemeKind.Dark

    Public Shared Property Current As ThemeKind
        Get
            Return _current
        End Get
        Set(value As ThemeKind)
            If _current <> value Then
                _current = value
                RaiseEvent ThemeChanged(Nothing, EventArgs.Empty)
            End If
        End Set
    End Property

    Public Shared ReadOnly Property Background As Color
        Get
            Return If(_current = ThemeKind.Dark, ColorTranslator.FromHtml("#1B1B1B"), ColorTranslator.FromHtml("#ECECEC"))
        End Get
    End Property

    Public Shared ReadOnly Property Surface As Color
        Get
            Return If(_current = ThemeKind.Dark, ColorTranslator.FromHtml("#262626"), ColorTranslator.FromHtml("#F5F5F5"))
        End Get
    End Property

    Public Shared ReadOnly Property CardBackground As Color
        Get
            Return If(_current = ThemeKind.Dark, ColorTranslator.FromHtml("#303030"), ColorTranslator.FromHtml("#FFFFFF"))
        End Get
    End Property

    Public Shared ReadOnly Property TextPrimary As Color
        Get
            Return If(_current = ThemeKind.Dark, ColorTranslator.FromHtml("#FFFFFF"), ColorTranslator.FromHtml("#1A1A1A"))
        End Get
    End Property

    Public Shared ReadOnly Property TextSecondary As Color
        Get
            Return If(_current = ThemeKind.Dark, ColorTranslator.FromHtml("#B0B0B0"), ColorTranslator.FromHtml("#5A5A5A"))
        End Get
    End Property

    Public Shared ReadOnly Property GridLines As Color
        Get
            Return If(_current = ThemeKind.Dark, ColorTranslator.FromHtml("#404040"), ColorTranslator.FromHtml("#D0D0D0"))
        End Get
    End Property

    'Data/accent colors are deliberately theme-independent (same vivid green/yellow/red/blue in
    'both Dark and Light) - they carry meaning (RPM, warning, danger) that should read the same
    'regardless of theme, and all four already have enough contrast against both backgrounds.
    Public Shared ReadOnly AccentRpm As Color = ColorTranslator.FromHtml("#00E676")
    Public Shared ReadOnly AccentWarning As Color = ColorTranslator.FromHtml("#FFD600")
    Public Shared ReadOnly AccentDanger As Color = ColorTranslator.FromHtml("#FF3D00")
    Public Shared ReadOnly AccentInfo As Color = ColorTranslator.FromHtml("#42A5F5")
End Class
