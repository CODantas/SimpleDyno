Imports System.Drawing.Drawing2D

''' <summary>
''' Fase 4 chrome widget (UI modernization plan): thin read-only strip anchored at the bottom
''' of Main showing acquisition mode / COM status. Callers only set the two text properties -
''' this class never reads Main's shared state directly and has no timer of its own, so it has
''' zero coupling to acquisition/calculation code. Named DashboardStatusBar (not StatusBar) to
''' avoid any ambiguity with the legacy System.Windows.Forms.StatusBar control.
''' </summary>
Public Class DashboardStatusBar
    Inherits Panel

    Private _acquisitionStatus As String = String.Empty
    Private _comStatus As String = String.Empty

    Public Sub New()
        Me.DoubleBuffered = True
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.DoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
        UpdateStyles()
        Me.BackColor = ColorPalette.Surface
    End Sub

    Public Property AcquisitionStatus As String
        Get
            Return _acquisitionStatus
        End Get
        Set(value As String)
            If _acquisitionStatus <> value Then
                _acquisitionStatus = value
                Invalidate()
            End If
        End Set
    End Property

    Public Property ComStatus As String
        Get
            Return _comStatus
        End Get
        Set(value As String)
            If _comStatus <> value Then
                _comStatus = value
                Invalidate()
            End If
        End Set
    End Property

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.HighQuality

        Using backBrush As New SolidBrush(ColorPalette.Surface)
            g.FillRectangle(backBrush, Me.ClientRectangle)
        End Using

        Using topBorder As New Pen(ColorPalette.GridLines)
            g.DrawLine(topBorder, 0, 0, Me.Width, 0)
        End Using

        Dim displayText As String = _acquisitionStatus
        If Not String.IsNullOrEmpty(_comStatus) Then
            If Not String.IsNullOrEmpty(displayText) Then displayText &= "   |   "
            displayText &= _comStatus
        End If

        Using textBrush As New SolidBrush(ColorPalette.TextSecondary)
            Using font As Font = TypographyManager.UiFont(8.0F)
                Using sf As New StringFormat()
                    sf.LineAlignment = StringAlignment.Center
                    sf.Alignment = StringAlignment.Near
                    Dim textRect As New RectangleF(8, 0, Me.Width - 16, Me.Height)
                    g.DrawString(displayText, font, textBrush, textRect, sf)
                End Using
            End Using
        End Using
    End Sub
End Class
