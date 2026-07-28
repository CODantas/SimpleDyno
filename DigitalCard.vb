Imports System.Drawing.Drawing2D

''' <summary>
''' Modernized replacement for SimpleDynoSubLabel (Fase 2 of the UI modernization plan).
''' Same SimpleDynoSubForm contract and the same Vertical/Horizontal text-layout algorithm as
''' the original, so it is a drop-in replacement at every instantiation point. Visual additions
''' only: rounded card panel over the page background, subtle drop shadow, Segoe UI for the
''' parameter/unit labels, Consolas for the big digital value. No physical calculation lives here.
''' </summary>
Public Class DigitalCard
    Inherits SimpleDynoSubForm

    'Resource manager for translated (resx satellite) strings.
    Private ReadOnly resources As New System.ComponentModel.ComponentResourceManager(GetType(DigitalCard))

    'Control Specific Text Positions
    Private ParameterLabel As Point
    Private ResultLabel As Point
    Private UnitLabel As Point

    'Theme is applied once, on the first resize after construction - if the widget is then
    'restored from a saved .sdi file, CreateFromSerializedData overwrites colors with whatever
    'was saved, exactly as it does for every other saved setting (min/max, font, etc).
    Private ThemeApplied As Boolean = False

    Public Overrides Sub ControlSpecificInitialization()
        myType = "Label"
        Y_Number_Allowed = 1
        XY_Selected = 1
        myConfiguration = "Vertical"
    End Sub

    Private Sub ApplyDefaultTheme()
        BackClr = ColorPalette.CardBackground
        AxisClr = ColorPalette.TextSecondary
        AxisBrush.Color = AxisClr
        AxisPen.Color = AxisClr
        Y_DataClr(XY_Selected) = ColorPalette.TextPrimary
        Y_DataBrush(XY_Selected).Color = Y_DataClr(XY_Selected)
        Y_DataPen(XY_Selected).Color = Y_DataClr(XY_Selected)
        Y_AxisFont = TypographyManager.UiFont(Y_AxisFont.Size)
        X_AxisFont = TypographyManager.UiFont(X_AxisFont.Size)
        Y_DataFont(XY_Selected) = TypographyManager.NumericFont(Y_DataFont(XY_Selected).Size, FontStyle.Bold)
    End Sub

    Public Overrides Sub ControlSpecificResize()

        If Not ThemeApplied Then
            ApplyDefaultTheme()
            ThemeApplied = True
        End If

        Dim Increment As Single = 0.1
        Dim DataTestString As String = "999999" 'Assumes no value displayed will be > 999999

        'Need to find the longer of the Parameter and unit strings
        Dim LabelTestString As String = " "
        For Count As Integer = 1 To Y_Number_Allowed
            If Y_PrimaryLabel(Count).Length > LabelTestString.Length Then LabelTestString = Y_PrimaryLabel(Count)
            If myMinCurMaxAbb(Y_MinCurMaxPointer(XY_Selected)).Length + " ".Length + Y_UnitsLabel(Count).Length > LabelTestString.Length Then LabelTestString = myMinCurMaxAbb(Y_MinCurMaxPointer(XY_Selected)).Length & " " & Y_UnitsLabel(Count)
        Next

        Select Case myConfiguration
            Case Is = "Vertical"
                'Divide The window height into GR proportion
                Dim DataFontHeight As Single = (Me.ClientSize.Height / GoldenRatio)
                'The remainder divided by two is the height for the primary and unit labels
                Dim LabelFontHeight As Single = (Me.ClientSize.Height - DataFontHeight) / 2
                'create a temporary font
                Dim TempFont As New System.Drawing.Font(Y_DataFont(Y_Number_Allowed).Name, Increment)
                'now scale the data font
                Do Until Grafx.Graphics.MeasureString(DataTestString, TempFont).Width >= Me.ClientSize.Width Or Grafx.Graphics.MeasureString(DataTestString, TempFont).Height >= DataFontHeight
                    TempFont = New System.Drawing.Font(Y_DataFont(Y_Number_Allowed).Name, TempFont.Size + Increment)
                Loop
                'set the datafont to the tempfont size
                Y_DataFont(Y_Number_Allowed) = New System.Drawing.Font(Y_DataFont(Y_Number_Allowed).Name, TempFont.Size)
                'now repeat for the label font
                'reset tempfont
                TempFont = New System.Drawing.Font(Y_AxisFont.Name, Increment)
                'scale the labelfont
                Do Until Grafx.Graphics.MeasureString(LabelTestString, TempFont).Width >= Me.ClientSize.Width Or Grafx.Graphics.MeasureString(LabelTestString, TempFont).Height >= LabelFontHeight
                    TempFont = New System.Drawing.Font(Y_AxisFont.Name, TempFont.Size + Increment)
                Loop
                'set the labelfont to the tempfont size
                Y_AxisFont = New System.Drawing.Font(Y_AxisFont.Name, TempFont.Size)
                'Set up text positions based on available data
                With Grafx.Graphics
                    ParameterLabel.X = CInt((Me.ClientSize.Width - .MeasureString(Y_PrimaryLabel(XY_Selected), Y_AxisFont).Width) / 2)
                    ResultLabel.Y = CInt((Me.ClientSize.Height - .MeasureString(DataTestString, Y_DataFont(XY_Selected)).Height) / 2)
                    ParameterLabel.Y = CInt((ResultLabel.Y - .MeasureString(Y_PrimaryLabel(XY_Selected), Y_AxisFont).Height) / 2)
                    UnitLabel.X = CInt((Me.ClientSize.Width - .MeasureString(myMinCurMaxAbb(Y_MinCurMaxPointer(XY_Selected)) & " " & Y_UnitsLabel(XY_Selected), Y_AxisFont).Width) / 2)
                    UnitLabel.Y = CInt(Me.ClientSize.Height - .MeasureString(Y_UnitsLabel(XY_Selected), Y_AxisFont).Height - ParameterLabel.Y)
                End With
            Case Is = "Horizontal"
                'Divide The window height into GR proportion
                Dim DataFontWidth As Double = Me.ClientSize.Width / 2 'GoldenRatio
                'The remainder divided by two is the height for the primary and unit labels
                Dim LabelFontWidth As Double = (Me.ClientSize.Width - DataFontWidth) / 2
                'create a temporary font
                Dim TempFont As New System.Drawing.Font(Y_DataFont(Y_Number_Allowed).Name, Increment)
                'now scale the data font
                Do Until Grafx.Graphics.MeasureString(DataTestString, TempFont).Width >= DataFontWidth Or Grafx.Graphics.MeasureString(DataTestString, TempFont).Height >= Me.ClientSize.Height
                    TempFont = New System.Drawing.Font(Y_DataFont(Y_Number_Allowed).Name, TempFont.Size + Increment)
                Loop
                'set the datafont to the tempfont size
                Y_DataFont(Y_Number_Allowed) = New System.Drawing.Font(Y_DataFont(Y_Number_Allowed).Name, TempFont.Size)
                'now repeat for the label font
                'reset tempfont
                TempFont = New System.Drawing.Font(Y_AxisFont.Name, Increment)
                'scale the labelfont
                Do Until Grafx.Graphics.MeasureString(LabelTestString, TempFont).Width >= LabelFontWidth Or Grafx.Graphics.MeasureString(LabelTestString, TempFont).Height >= Me.ClientSize.Height
                    TempFont = New System.Drawing.Font(Y_AxisFont.Name, TempFont.Size + Increment)
                Loop
                'set the labelfont to the tempfont size
                Y_AxisFont = New System.Drawing.Font(Y_AxisFont.Name, TempFont.Size)
                'Set up text positions based on available data
                With Grafx.Graphics
                    ParameterLabel.Y = CInt((Me.ClientSize.Height - .MeasureString(Y_PrimaryLabel(XY_Selected), Y_AxisFont).Height) / 2)
                    ParameterLabel.X = Me.ClientRectangle.Left
                    ResultLabel.Y = CInt((Me.ClientSize.Height - .MeasureString(DataTestString, Y_DataFont(XY_Selected)).Height) / 2)
                    UnitLabel.X = CInt(Me.ClientRectangle.Right - .MeasureString(myMinCurMaxAbb(Y_MinCurMaxPointer(XY_Selected)) & " " & Y_UnitsLabel(XY_Selected), Y_AxisFont).Width)
                    UnitLabel.Y = CInt((Me.ClientSize.Height - .MeasureString(Y_UnitsLabel(XY_Selected), Y_AxisFont).Height) / 2)
                End With
        End Select

    End Sub

    Private Function RoundedRect(rect As Rectangle, radius As Integer) As GraphicsPath
        Dim path As New GraphicsPath()
        Dim d As Integer = radius * 2
        If d > rect.Width Then d = rect.Width
        If d > rect.Height Then d = rect.Height
        path.AddArc(rect.X, rect.Y, d, d, 180, 90)
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90)
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90)
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90)
        path.CloseFigure()
        Return path
    End Function

    Overrides Sub DrawToBuffer(ByVal g As Graphics)

        Dim StringResult As String
        StringResult = NewCustomFormat(Y_Result(XY_Selected))
        ResultLabel.X = CInt((Me.ClientSize.Width - Grafx.Graphics.MeasureString(StringResult, Y_DataFont(XY_Selected)).Width) / 2)

        g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
        g.PixelOffsetMode = PixelOffsetMode.HighQuality

        Grafx.Graphics.Clear(ColorPalette.Background)

        With Grafx.Graphics
            Dim CardRect As New Rectangle(1, 1, Math.Max(1, Me.ClientSize.Width - 3), Math.Max(1, Me.ClientSize.Height - 3))
            Dim CornerRadius As Integer = CInt(Math.Min(CardRect.Width, CardRect.Height) * 0.12)

            Dim ShadowRect As Rectangle = CardRect
            ShadowRect.Offset(2, 2)
            Using shadowPath As GraphicsPath = RoundedRect(ShadowRect, CornerRadius)
                Using shadowBrush As New SolidBrush(Color.FromArgb(70, Color.Black))
                    .FillPath(shadowBrush, shadowPath)
                End Using
            End Using

            Using cardPath As GraphicsPath = RoundedRect(CardRect, CornerRadius)
                Using cardBrush As New SolidBrush(BackClr)
                    .FillPath(cardBrush, cardPath)
                End Using
                Using borderPen As New Pen(Color.FromArgb(60, ColorPalette.TextPrimary), 1)
                    .DrawPath(borderPen, cardPath)
                End Using
            End Using

            .DrawString(Y_PrimaryLabel(XY_Selected), Y_AxisFont, AxisBrush, ParameterLabel)
            .DrawString(StringResult, Y_DataFont(XY_Selected), Y_DataBrush(XY_Selected), ResultLabel)
            .DrawString(myMinCurMaxAbb(Y_MinCurMaxPointer(XY_Selected)) & " " & Y_UnitsLabel(XY_Selected), Y_AxisFont, AxisBrush, UnitLabel)
        End With

    End Sub

    Overrides Sub AddControlSpecificOptionItems()

        Dim TestStrip As ToolStripMenuItem
        Dim str1 As String
        Dim str2 As String()
        Dim str3 As String()

        str1 = resources.GetString("SDL_Configuration")
        str2 = {resources.GetString("SDL_Vertical"), resources.GetString("SDL_Horizontal")}
        str3 = {}

        TestStrip = CreateAToolStripMenuItem("O", str1, str2, str3) ', str4, str5)
        contextmnu.Items.Add(TestStrip)

    End Sub

    Public Overrides Sub ControlSpecificOptionSelection(ByVal Sent As String)
        Select Case Sent
            Case Is = "O_0"
                myConfiguration = "Vertical"
            Case Is = "O_1"
                myConfiguration = "Horizontal"
        End Select
    End Sub

    Public Overrides Function ControlSpecificSerializationData() As String

    End Function

    Public Overrides Sub ControlSpecficCreateFromSerializedData(ByVal Sent As String())

    End Sub
End Class
