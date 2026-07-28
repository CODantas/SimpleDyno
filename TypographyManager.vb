Imports System.Drawing

''' <summary>
''' Central font factory for the modernized UI (Fase 0 of the UI modernization plan).
''' Segoe UI for general text (ships with Windows since Vista); Consolas for numeric/digital
''' readouts (ships with Windows, monospaced, avoids depending on a font that may not be
''' installed on the user's or his friend's machine - unlike JetBrains Mono/Roboto Mono).
''' Pure presentation - no reference to Main or any calculation/acquisition code.
''' </summary>
Public NotInheritable Class TypographyManager
    Private Sub New()
    End Sub

    Private Const UiFontFamily As String = "Segoe UI"
    Private Const NumericFontFamily As String = "Consolas"

    Public Shared Function UiFont(sizePoints As Single, Optional style As FontStyle = FontStyle.Regular) As Font
        Return New Font(UiFontFamily, sizePoints, style, GraphicsUnit.Point)
    End Function

    Public Shared Function NumericFont(sizePoints As Single, Optional style As FontStyle = FontStyle.Regular) As Font
        Return New Font(NumericFontFamily, sizePoints, style, GraphicsUnit.Point)
    End Function
End Class
