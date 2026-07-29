Imports System.Drawing

''' <summary>
''' Central color palette for the modernized UI (Fase 0 of the UI modernization plan).
''' Pure presentation constants - no reference to Main or any calculation/acquisition code.
''' </summary>
Public NotInheritable Class ColorPalette
    Private Sub New()
    End Sub

    Public Shared ReadOnly Background As Color = ColorTranslator.FromHtml("#1B1B1B")
    Public Shared ReadOnly Surface As Color = ColorTranslator.FromHtml("#262626")
    Public Shared ReadOnly CardBackground As Color = ColorTranslator.FromHtml("#303030")

    Public Shared ReadOnly TextPrimary As Color = ColorTranslator.FromHtml("#FFFFFF")
    Public Shared ReadOnly TextSecondary As Color = ColorTranslator.FromHtml("#B0B0B0")

    Public Shared ReadOnly GridLines As Color = ColorTranslator.FromHtml("#404040")

    Public Shared ReadOnly AccentRpm As Color = ColorTranslator.FromHtml("#00E676")
    Public Shared ReadOnly AccentWarning As Color = ColorTranslator.FromHtml("#FFD600")
    Public Shared ReadOnly AccentDanger As Color = ColorTranslator.FromHtml("#FF3D00")
    Public Shared ReadOnly AccentInfo As Color = ColorTranslator.FromHtml("#42A5F5")
End Class
