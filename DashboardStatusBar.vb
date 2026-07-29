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
    Private _recordingState As Main.AcquisitionStatus = Main.AcquisitionStatus.Idle

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

    ''' <summary>
    ''' Fase 5: fed by Main.NotifyAcquisitionStatusChanged_ThreadSafe, called right next to the
    ''' existing btnStartPowerRun/btnStartLoggingRaw.BackColor mutations inside
    ''' myWaveHandler_ProcessWave/DataReceivedHandler - never replaces them, only mirrors the
    ''' same information here as a colored dot.
    ''' </summary>
    Public Property RecordingState As Main.AcquisitionStatus
        Get
            Return _recordingState
        End Get
        Set(value As Main.AcquisitionStatus)
            If _recordingState <> value Then
                _recordingState = value
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

        Dim textLeft As Integer = 8
        Dim dotColor As Color = Color.Empty
        Select Case _recordingState
            Case Main.AcquisitionStatus.PowerRunRecording, Main.AcquisitionStatus.LogRawRecording
                dotColor = ColorPalette.AccentRpm
            Case Main.AcquisitionStatus.PowerRunBufferFull, Main.AcquisitionStatus.LogRawBufferFull,
                 Main.AcquisitionStatus.PowerRunArmed, Main.AcquisitionStatus.LogRawArmed
                dotColor = ColorPalette.AccentDanger
        End Select

        If dotColor <> Color.Empty Then
            Const DotDiameter As Integer = 8
            Dim dotRect As New RectangleF(8, (Me.Height - DotDiameter) / 2.0F, DotDiameter, DotDiameter)
            Using dotBrush As New SolidBrush(dotColor)
                g.FillEllipse(dotBrush, dotRect)
            End Using
            textLeft = 8 + DotDiameter + 6
        End If

        Using textBrush As New SolidBrush(ColorPalette.TextSecondary)
            Using font As Font = TypographyManager.UiFont(8.0F)
                Using sf As New StringFormat()
                    sf.LineAlignment = StringAlignment.Center
                    sf.Alignment = StringAlignment.Near
                    sf.FormatFlags = StringFormatFlags.NoWrap
                    sf.Trimming = StringTrimming.EllipsisCharacter
                    Dim textRect As New RectangleF(textLeft, 0, Me.Width - textLeft - 8, Me.Height)
                    g.DrawString(displayText, font, textBrush, textRect, sf)
                End Using
            End Using
        End Using
    End Sub
End Class
