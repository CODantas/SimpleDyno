Imports System.Drawing
Imports System.Drawing.Drawing2D

''' <summary>
''' Small vector status icons drawn via GDI+ (no icon library dependency, matches the rest of
''' this project's existing custom-drawn widgets). Fase 0 of the UI modernization plan - not
''' consumed by any existing form yet; intended for the StatusBar/Toolbar built in a later phase.
''' Pure presentation - no reference to Main or any calculation/acquisition code.
''' </summary>
Public NotInheritable Class IconManager
    Private Sub New()
    End Sub

    ''' <summary>Solid circle status dot (e.g. connected/recording indicator), sized to fit diameterPx.</summary>
    Public Shared Function StatusDot(diameterPx As Integer, color As Color) As Bitmap
        Dim bmp As New Bitmap(diameterPx, diameterPx)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.HighQuality
            Using brush As New SolidBrush(color)
                g.FillEllipse(brush, 1, 1, diameterPx - 2, diameterPx - 2)
            End Using
        End Using
        Return bmp
    End Function

    ''' <summary>Simple filled triangle warning glyph, sized to fit sizePx.</summary>
    Public Shared Function WarningTriangle(sizePx As Integer, color As Color) As Bitmap
        Dim bmp As New Bitmap(sizePx, sizePx)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.HighQuality
            Dim points() As Point = {
                New Point(sizePx \ 2, 1),
                New Point(sizePx - 1, sizePx - 1),
                New Point(1, sizePx - 1)
            }
            Using brush As New SolidBrush(color)
                g.FillPolygon(brush, points)
            End Using
        End Using
        Return bmp
    End Function
End Class
