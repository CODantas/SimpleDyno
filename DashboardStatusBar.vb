Imports System.Drawing.Drawing2D

''' <summary>
''' Fase 4 chrome widget (UI modernization plan): thin read-only strip anchored at the bottom
''' of Main showing acquisition mode / COM status. Callers only set the two text properties -
''' this class never reads Main's shared state directly and has no timer of its own, so it has
''' zero coupling to acquisition/calculation code. Named DashboardStatusBar (not StatusBar) to
''' avoid any ambiguity with the legacy System.Windows.Forms.StatusBar control.
'''
''' Also hosts the Dark/Light theme switch (Fase 6): a small "Claro / Escuro" control at the
''' right edge, the active word highlighted. It only calls ThemeManager.SetTheme - every other
''' themed window/widget repaints itself independently via ColorPalette.ThemeChanged, so this
''' class still never needs to know about Main or any other widget.
''' </summary>
Public Class DashboardStatusBar
    Inherits Panel

    Private _acquisitionStatus As String = String.Empty
    Private _comStatus As String = String.Empty
    Private _recordingState As Main.AcquisitionStatus = Main.AcquisitionStatus.Idle

    Private Const LightLabel As String = "Claro"
    Private Const DarkLabel As String = "Escuro"
    Private Const ThemeToggleMargin As Integer = 8
    Private Const ThemeToggleSeparator As String = " / "

    'Hit-test rectangles recomputed on every OnPaint (the strip is fixed-height and rarely
    'resized, so this is cheap) - MouseDown/MouseMove only ever read them, never compute layout.
    Private _lightRect As RectangleF
    Private _darkRect As RectangleF

    Public Sub New()
        Me.DoubleBuffered = True
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.DoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
        UpdateStyles()
        Me.BackColor = ColorPalette.Surface
        AddHandler ColorPalette.ThemeChanged, AddressOf Me.OnThemeChanged
    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            RemoveHandler ColorPalette.ThemeChanged, AddressOf Me.OnThemeChanged
        End If
        MyBase.Dispose(disposing)
    End Sub

    Private Sub OnThemeChanged(ByVal sender As Object, ByVal e As EventArgs)
        Me.BackColor = ColorPalette.Surface
        Invalidate()
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

        Using font As Font = TypographyManager.UiFont(8.0F)
            Dim themeToggleWidth As Single = DrawThemeToggle(g, font)

            Using textBrush As New SolidBrush(ColorPalette.TextSecondary)
                Using sf As New StringFormat()
                    sf.LineAlignment = StringAlignment.Center
                    sf.Alignment = StringAlignment.Near
                    sf.FormatFlags = StringFormatFlags.NoWrap
                    sf.Trimming = StringTrimming.EllipsisCharacter
                    Dim textRect As New RectangleF(textLeft, 0, Me.Width - textLeft - themeToggleWidth - ThemeToggleMargin, Me.Height)
                    g.DrawString(displayText, font, textBrush, textRect, sf)
                End Using
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' Draws "Claro / Escuro" right-aligned, the active word in TextPrimary and the other in
    ''' TextSecondary, and records _lightRect/_darkRect for OnMouseDown. Returns the total width
    ''' consumed (including the right margin) so OnPaint can keep the status text from running
    ''' under it.
    ''' </summary>
    Private Function DrawThemeToggle(g As Graphics, font As Font) As Single
        Dim lightSize As SizeF = g.MeasureString(LightLabel, font)
        Dim sepSize As SizeF = g.MeasureString(ThemeToggleSeparator, font)
        Dim darkSize As SizeF = g.MeasureString(DarkLabel, font)

        Dim totalWidth As Single = lightSize.Width + sepSize.Width + darkSize.Width
        Dim startX As Single = Me.Width - ThemeToggleMargin - totalWidth
        Dim centerY As Single = Me.Height / 2.0F

        Dim isDark As Boolean = (ColorPalette.Current = ColorPalette.ThemeKind.Dark)

        _lightRect = New RectangleF(startX, 0, lightSize.Width, Me.Height)
        Dim sepRect As New RectangleF(_lightRect.Right, 0, sepSize.Width, Me.Height)
        _darkRect = New RectangleF(sepRect.Right, 0, darkSize.Width, Me.Height)

        Using sf As New StringFormat()
            sf.LineAlignment = StringAlignment.Center
            sf.Alignment = StringAlignment.Near
            sf.FormatFlags = StringFormatFlags.NoWrap

            Using activeBrush As New SolidBrush(ColorPalette.TextPrimary)
                Using inactiveBrush As New SolidBrush(ColorPalette.TextSecondary)
                    g.DrawString(LightLabel, font, If(isDark, inactiveBrush, activeBrush), _lightRect, sf)
                    g.DrawString(ThemeToggleSeparator, font, inactiveBrush, sepRect, sf)
                    g.DrawString(DarkLabel, font, If(isDark, activeBrush, inactiveBrush), _darkRect, sf)
                End Using
            End Using
        End Using

        Return ThemeToggleMargin + totalWidth
    End Function

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        If e.Button <> MouseButtons.Left Then Return
        Dim p As New PointF(e.X, e.Y)
        If _lightRect.Contains(p) Then
            ThemeManager.SetTheme(ColorPalette.ThemeKind.Light)
        ElseIf _darkRect.Contains(p) Then
            ThemeManager.SetTheme(ColorPalette.ThemeKind.Dark)
        End If
    End Sub

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        Dim p As New PointF(e.X, e.Y)
        Me.Cursor = If(_lightRect.Contains(p) OrElse _darkRect.Contains(p), Cursors.Hand, Cursors.Default)
    End Sub
End Class
