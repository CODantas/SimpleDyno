Imports System.Drawing.Drawing2D

''' <summary>
''' Modernized replacement for SimpleDynoSubGauge (Fase 1 of the UI modernization plan).
''' Same SimpleDynoSubForm contract (Initialize/ControlSpecificResize/DrawToBuffer/etc.) and the
''' same tick/needle geometry, so it is a drop-in replacement at every instantiation point.
''' Visual additions only: colored range band, needle motion smoothing, digital readout, subtle
''' shadows, dark theme defaults. No physical calculation lives in this class.
''' </summary>
Public Class ModernGauge
    Inherits SimpleDynoSubForm

    'Resource manager for translated (resx satellite) strings.
    Private ReadOnly resources As New System.ComponentModel.ComponentResourceManager(GetType(ModernGauge))

    Private myGaugeSurface As Rectangle
    Private myDialRectangle As Rectangle

    Private MajorTickOuter() As Point
    Private MajorTickInner() As Point
    Private MinorTickOuter() As Point
    Private MinorTickInner() As Point
    Private TickLabelPositions() As Point
    Private ParameterPosition As Point
    Private UnitPosition As Point
    Private EndOfNeedle As Point
    Private Center As Point
    Private SweepClockwise As Integer
    Private NumberOfMajorTicks As Integer
    Private NumberOfMinorTicks As Integer
    Private Angle As Single
    Private PointAngle As Single
    Private StartAngle As Single
    Private TickLabels() As String

    'Theme is applied once, on the first resize after construction - if the widget is then
    'restored from a saved .sdi file, CreateFromSerializedData overwrites colors with whatever
    'was saved, exactly as it does for every other saved setting (min/max, font, etc).
    Private ThemeApplied As Boolean = False

    'Needle motion smoothing state - purely visual, never fed back into Y_Result/calculation.
    Private DisplayedNeedleValue As Double
    Private HasDisplayedNeedleValue As Boolean = False
    Private Const NeedleSmoothingFactor As Double = 0.35

    Public Overrides Sub ControlSpecificInitialization()

        myType = "Gauge"
        Y_Number_Allowed = 1
        XY_Selected = 1

        myConfiguration = "270 270 1"
        Angle = 270
        PointAngle = 270 'CHECK - not been used now - forward compatability item
        StartAngle = PointAngle - Angle / 2
        SweepClockwise = 1
        NumberOfMajorTicks = 5
        NumberOfMinorTicks = 21

        ReDim MajorTickOuter(NumberOfMajorTicks)
        ReDim MajorTickInner(NumberOfMajorTicks)
        ReDim MinorTickOuter(NumberOfMinorTicks)
        ReDim MinorTickInner(NumberOfMinorTicks)
        ReDim TickLabelPositions(NumberOfMajorTicks)
        ReDim TickLabels(NumberOfMajorTicks)

    End Sub

    Private Sub ApplyDefaultTheme()
        BackClr = ColorPalette.CardBackground
        AxisClr = ColorPalette.TextSecondary
        AxisBrush.Color = AxisClr
        AxisPen.Color = AxisClr
        Y_DataClr(XY_Selected) = ColorPalette.AccentRpm
        Y_DataBrush(XY_Selected).Color = Y_DataClr(XY_Selected)
        Y_DataPen(XY_Selected).Color = Y_DataClr(XY_Selected)
        Y_AxisFont = TypographyManager.UiFont(8.0F)
        X_AxisFont = TypographyManager.UiFont(8.0F)
    End Sub

    Public Overrides Sub ControlSpecificResize()

        If Not ThemeApplied Then
            ApplyDefaultTheme()
            ThemeApplied = True
        End If

        Dim Count As Integer
        Dim MajorTickLength As Double, MinorTickLength As Double
        Dim Increment As Double

        With myGaugeSurface
            .Width = CInt(Me.ClientSize.Width * 0.9) 'padding 1% each side
            .Height = CInt(Me.ClientSize.Height * 0.9) 'padding 1% each side
            .X = CInt(Me.ClientSize.Width * 0.05) 'Puts the drawing surface top corner
            .Y = CInt(Me.ClientSize.Height * 0.05) ' in a posisition to pad 5 all around
        End With

        Dim MinX As Double = 1, MinY As Double = 1, MaxX As Double = -1, MaxY As Double = -1, TempX As Double, TempY As Double
        Dim TempWidth As Double, TempHeight As Double, TempCenterX As Double, TempCenterY As Double

        For Arc As Integer = CInt(StartAngle) To CInt(StartAngle + Angle)
            TempX = Math.Cos(ConvertedToRadians(360 - Arc))
            TempY = Math.Sin(ConvertedToRadians(360 - Arc))
            If TempX < MinX Then MinX = TempX
            If TempX > MaxX Then MaxX = TempX
            If TempY < MinY Then MinY = TempY
            If TempY > MaxY Then MaxY = TempY
        Next

        MaxX = (CInt(MaxX * 1000) / 1000)
        MaxY = (CInt(MaxY * 1000) / 1000)
        MinX = (CInt(MinX * 1000) / 1000)
        MinY = (CInt(MinY * 1000) / 1000)

        If MinX >= 0 Then
            TempWidth = MaxX
            TempCenterX = 0
        Else
            If MaxX > 0 Then
                TempWidth = Math.Abs(MaxX - MinX)
                TempCenterX = TempWidth / Math.Abs(TempWidth / MinX)
            Else
                TempWidth = Math.Abs(MinX)
                TempCenterX = TempWidth '1
            End If
        End If

        If MinY >= 0 Then
            TempHeight = MaxY
            TempCenterY = TempHeight
        Else
            If MaxY > 0 Then
                TempHeight = Math.Abs(MaxY - MinY)
                TempCenterY = TempHeight / Math.Abs(TempHeight / MaxY)
            Else
                TempHeight = Math.Abs(MinY)
                TempCenterY = 0
            End If
        End If

        Dim FoldWidth As Double, FoldHeight As Double
        FoldWidth = myGaugeSurface.Width / TempWidth
        FoldHeight = myGaugeSurface.Height / TempHeight

        If FoldWidth >= FoldHeight Then
            myDialRectangle.Height = CInt(2 * FoldHeight)
            myDialRectangle.Width = CInt(2 * FoldHeight)
            MajorTickLength = myDialRectangle.Height * 0.15
            MinorTickLength = MajorTickLength / 2
            Center.X = CInt(myGaugeSurface.X + myGaugeSurface.Width / 2 - TempWidth * FoldHeight / 2 + TempWidth * FoldHeight * TempCenterX / TempWidth)
            Center.Y = CInt(myGaugeSurface.Y + myGaugeSurface.Height * (TempCenterY / TempHeight))
        Else
            myDialRectangle.Height = CInt(2 * FoldWidth)
            myDialRectangle.Width = CInt(2 * FoldWidth)
            MajorTickLength = myDialRectangle.Width * 0.15
            MinorTickLength = MajorTickLength / 2
            Center.X = CInt(myGaugeSurface.X + myGaugeSurface.Width * (TempCenterX / TempWidth))
            Center.Y = CInt(myGaugeSurface.Y + myGaugeSurface.Height / 2 - TempHeight * FoldWidth / 2 + TempHeight * FoldWidth * TempCenterY / TempHeight)
        End If

        myDialRectangle.X = Center.X - CInt(myDialRectangle.Width / 2)
        myDialRectangle.Y = Center.Y - CInt(myDialRectangle.Height / 2)

        With myDialRectangle
            For Count = 1 To NumberOfMajorTicks
                MajorTickOuter(Count).X = CInt(Center.X + .Width / 2 * Math.Cos(ConvertedToRadians(StartAngle + (Angle / (NumberOfMajorTicks - 1) * (Count - 1)))))
                MajorTickOuter(Count).Y = CInt(Center.Y + .Height / 2 * Math.Sin(ConvertedToRadians(StartAngle + (Angle / (NumberOfMajorTicks - 1) * (Count - 1)))))
                MajorTickInner(Count).X = CInt(Center.X + (.Width - MajorTickLength) / 2 * Math.Cos(ConvertedToRadians(StartAngle + (Angle / (NumberOfMajorTicks - 1) * (Count - 1)))))
                MajorTickInner(Count).Y = CInt(Center.Y + (.Height - MajorTickLength) / 2 * Math.Sin(ConvertedToRadians(StartAngle + (Angle / (NumberOfMajorTicks - 1) * (Count - 1)))))
                If SweepClockwise = 1 Then
                    TickLabels(Count) = NewCustomFormat(Y_Minimum(Y_Number_Allowed) + (Y_Maximum(Y_Number_Allowed) - Y_Minimum(Y_Number_Allowed)) / (NumberOfMajorTicks - 1) * (Count - 1))
                Else
                    TickLabels(Count) = NewCustomFormat(Y_Maximum(Y_Number_Allowed) - (Y_Maximum(Y_Number_Allowed) - Y_Minimum(Y_Number_Allowed)) / (NumberOfMajorTicks - 1) * (Count - 1))
                End If
            Next
            For Count = 1 To NumberOfMinorTicks
                MinorTickOuter(Count).X = CInt(Center.X + .Width / 2 * Math.Cos(ConvertedToRadians(StartAngle + (Angle / (NumberOfMinorTicks - 1) * (Count - 1)))))
                MinorTickOuter(Count).Y = CInt(Center.Y + .Height / 2 * Math.Sin(ConvertedToRadians(StartAngle + (Angle / (NumberOfMinorTicks - 1) * (Count - 1)))))
                MinorTickInner(Count).X = CInt(Center.X + (.Width - MinorTickLength) / 2 * Math.Cos(ConvertedToRadians(StartAngle + (Angle / (NumberOfMinorTicks - 1) * (Count - 1)))))
                MinorTickInner(Count).Y = CInt(Center.Y + (.Height - MinorTickLength) / 2 * Math.Sin(ConvertedToRadians(StartAngle + (Angle / (NumberOfMinorTicks - 1) * (Count - 1)))))
            Next
        End With

        Dim TickLabelWidths(NumberOfMajorTicks) As Double
        Dim TickLabelHeights(NumberOfMajorTicks) As Double
        With myDialRectangle
            Dim l As Double, Score As Integer

            Increment = 0
            Do
                Increment += 0.1
                Y_AxisFont = New Font(Y_AxisFont.Name, CSng(Increment))
                Score = 0
                For Count = 1 To NumberOfMajorTicks
                    TickLabelWidths(Count) = Grafx.Graphics.MeasureString(TickLabels(Count), Y_AxisFont).Width
                    TickLabelHeights(Count) = Grafx.Graphics.MeasureString(TickLabels(Count), Y_AxisFont).Height
                    l = ((TickLabelWidths(Count) / 2) ^ 2 + (TickLabelHeights(Count)) ^ 2) ^ 0.5
                    TickLabelPositions(Count).X = CInt(Center.X + (.Width - MajorTickLength - l) / 2 * Math.Cos(ConvertedToRadians(StartAngle + (Angle / (NumberOfMajorTicks - 1) * (Count - 1))))) - CInt(Grafx.Graphics.MeasureString(TickLabels(Count), Y_AxisFont).Width / 2)
                    TickLabelPositions(Count).Y = CInt(Center.Y + (.Height - MajorTickLength - l) / 2 * Math.Sin(ConvertedToRadians(StartAngle + (Angle / (NumberOfMajorTicks - 1) * (Count - 1))))) - CInt(Grafx.Graphics.MeasureString(TickLabels(Count), Y_AxisFont).Height / 2)
                Next
                TickLabelHeights(0) = Grafx.Graphics.MeasureString(Y_PrimaryLabel(Y_Number_Allowed), Y_AxisFont).Height * 2 'To cover primary lavel and units
                TickLabelWidths(0) = Grafx.Graphics.MeasureString(Y_PrimaryLabel(Y_Number_Allowed), Y_AxisFont).Width
                TickLabelPositions(0).Y = CInt(Center.Y + (MajorTickInner(3).Y - Center.Y) / 2 - TickLabelHeights(0) / 2)
                TickLabelPositions(0).X = CInt(Center.X + (MajorTickInner(3).X - Center.X) / 2 - TickLabelWidths(0) / 2)

                For o As Integer = 0 To NumberOfMajorTicks
                    For i As Integer = 0 To NumberOfMajorTicks
                        If TickLabelPositions(o).X < TickLabelPositions(i).X + TickLabelWidths(i) AndAlso _
                            TickLabelPositions(o).X + TickLabelWidths(o) > TickLabelPositions(i).X AndAlso _
                            TickLabelPositions(o).Y < TickLabelPositions(i).Y + TickLabelHeights(i) AndAlso _
                            TickLabelPositions(o).Y + TickLabelHeights(o) > TickLabelPositions(i).Y Then
                            'No overlap
                            Score += 1
                        Else

                        End If
                    Next
                Next

            Loop Until Score > NumberOfMajorTicks + 1
            'Need to check that the end ticks (1 and 5) are not outside the Gaugesurface area
            If TickLabelPositions(1).X < Me.ClientRectangle.X Then TickLabelPositions(1).X = Me.ClientRectangle.X
            If TickLabelPositions(5).X < Me.ClientRectangle.X Then TickLabelPositions(5).X = Me.ClientRectangle.X
            If TickLabelPositions(1).Y < Me.ClientRectangle.Y Then TickLabelPositions(1).Y = Me.ClientRectangle.Y
            If TickLabelPositions(5).Y < Me.ClientRectangle.Y Then TickLabelPositions(5).Y = Me.ClientRectangle.Y

        End With

        ParameterPosition.Y = TickLabelPositions(0).Y
        ParameterPosition.X = TickLabelPositions(0).X

        UnitPosition.X = CInt(ParameterPosition.X + TickLabelWidths(0) / 2 - Grafx.Graphics.MeasureString(myMinCurMaxAbb(Y_MinCurMaxPointer(XY_Selected)) & " " & Y_UnitsLabel(Y_Number_Allowed), Y_AxisFont).Width / 2)
        UnitPosition.Y = ParameterPosition.Y + Y_AxisFont.Height

        Y_DataPen(XY_Selected).Width = 4

    End Sub

    Overrides Sub DrawToBuffer(ByVal g As Graphics)

        Dim TickCount As Integer
        If Y_Result(XY_Selected) > Y_Maximum(Y_Number_Allowed) Then Y_Result(XY_Selected) = Y_Maximum(Y_Number_Allowed)
        If Y_Result(XY_Selected) < Y_Minimum(Y_Number_Allowed) Then Y_Result(XY_Selected) = Y_Minimum(Y_Number_Allowed)
        Grafx.Graphics.Clear(BackClr)

        g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
        g.PixelOffsetMode = PixelOffsetMode.HighQuality

        'Needle motion smoothing - visual only, never written back into Y_Result.
        If Not HasDisplayedNeedleValue Then
            DisplayedNeedleValue = Y_Result(XY_Selected)
            HasDisplayedNeedleValue = True
        Else
            DisplayedNeedleValue += (Y_Result(XY_Selected) - DisplayedNeedleValue) * NeedleSmoothingFactor
        End If

        With Grafx.Graphics

            'Colored range band (green/yellow/red), drawn behind the ticks, redline-style.
            Dim BandPenWidth As Single = CSng(Math.Max(3, myDialRectangle.Width * 0.05))
            Dim BandInset As Single = BandPenWidth * 1.6F
            Dim BandRect As New Rectangle(
                CInt(myDialRectangle.X + BandInset), CInt(myDialRectangle.Y + BandInset),
                CInt(myDialRectangle.Width - 2 * BandInset), CInt(myDialRectangle.Height - 2 * BandInset))
            Dim GreenSweep As Single = Angle * 0.7F
            Dim YellowSweep As Single = Angle * 0.15F
            Dim RedSweep As Single = Angle - GreenSweep - YellowSweep

            Using shadowPen As New Pen(Color.FromArgb(70, Color.Black), BandPenWidth)
                Dim ShadowRect As Rectangle = BandRect
                ShadowRect.Offset(2, 2)
                .DrawArc(shadowPen, ShadowRect, StartAngle, Angle)
            End Using
            Using bandPen As New Pen(ColorPalette.AccentRpm, BandPenWidth)
                .DrawArc(bandPen, BandRect, StartAngle, GreenSweep)
            End Using
            Using bandPen As New Pen(ColorPalette.AccentWarning, BandPenWidth)
                .DrawArc(bandPen, BandRect, StartAngle + GreenSweep, YellowSweep)
            End Using
            Using bandPen As New Pen(ColorPalette.AccentDanger, BandPenWidth)
                .DrawArc(bandPen, BandRect, StartAngle + GreenSweep + YellowSweep, RedSweep)
            End Using

            .DrawArc(AxisPen, myDialRectangle, StartAngle, Angle)
            For TickCount = 1 To NumberOfMajorTicks
                .DrawLine(AxisPen, MajorTickOuter(TickCount), MajorTickInner(TickCount))
                .DrawString(TickLabels(TickCount), Y_AxisFont, AxisBrush, TickLabelPositions(TickCount))
            Next
            For TickCount = 1 To NumberOfMinorTicks
                .DrawLine(AxisPen, MinorTickOuter(TickCount), MinorTickInner(TickCount))
            Next
            .DrawString(Y_PrimaryLabel(XY_Selected), Y_AxisFont, AxisBrush, ParameterPosition)
            .DrawString(myMinCurMaxAbb(Y_MinCurMaxPointer(XY_Selected)) & " " & Y_UnitsLabel(XY_Selected), Y_AxisFont, AxisBrush, UnitPosition)

            'Digital readout - shows the actual (non-smoothed) value, Consolas, above the label.
            Dim NumericText As String = NewCustomFormat(Y_Result(XY_Selected))
            Dim NumericFontSize As Single = CSng(Math.Max(6, myDialRectangle.Height * 0.11))
            Using numFont As Font = TypographyManager.NumericFont(NumericFontSize, FontStyle.Bold)
                Dim TextSize As SizeF = .MeasureString(NumericText, numFont)
                Dim TextX As Single = Center.X - TextSize.Width / 2
                Dim TextY As Single = ParameterPosition.Y - TextSize.Height - 2
                Using shadowBrush As New SolidBrush(Color.FromArgb(120, Color.Black))
                    .DrawString(NumericText, numFont, shadowBrush, TextX + 1, TextY + 1)
                End Using
                .DrawString(NumericText, numFont, Y_DataBrush(XY_Selected), TextX, TextY)
            End Using

            With myDialRectangle
                If SweepClockwise = 1 Then
                    EndOfNeedle.X = CInt(Center.X + .Width / 2 * Math.Cos(ConvertedToRadians(StartAngle + ((DisplayedNeedleValue - Y_Minimum(Y_Number_Allowed)) / (Y_Maximum(Y_Number_Allowed) - Y_Minimum(Y_Number_Allowed)) * Angle))))
                    EndOfNeedle.Y = CInt(Center.Y + .Height / 2 * Math.Sin(ConvertedToRadians(StartAngle + ((DisplayedNeedleValue - Y_Minimum(Y_Number_Allowed)) / (Y_Maximum(Y_Number_Allowed) - Y_Minimum(Y_Number_Allowed)) * Angle))))
                Else
                    EndOfNeedle.X = CInt(Center.X + .Width / 2 * Math.Cos(ConvertedToRadians(StartAngle + Angle - ((DisplayedNeedleValue - Y_Minimum(Y_Number_Allowed)) / (Y_Maximum(Y_Number_Allowed) - Y_Minimum(Y_Number_Allowed)) * Angle))))
                    EndOfNeedle.Y = CInt(Center.Y + .Height / 2 * Math.Sin(ConvertedToRadians(StartAngle + Angle - ((DisplayedNeedleValue - Y_Minimum(Y_Number_Allowed)) / (Y_Maximum(Y_Number_Allowed) - Y_Minimum(Y_Number_Allowed)) * Angle))))
                End If
            End With

            Dim ShadowCenter As New Point(Center.X + 2, Center.Y + 2)
            Dim ShadowEnd As New Point(EndOfNeedle.X + 2, EndOfNeedle.Y + 2)
            Using shadowNeedlePen As New Pen(Color.FromArgb(90, Color.Black), Y_DataPen(XY_Selected).Width)
                .DrawLine(shadowNeedlePen, ShadowCenter, ShadowEnd)
            End Using
            .DrawLine(Y_DataPen(XY_Selected), Center, EndOfNeedle)

            Dim HubRadius As Integer = CInt(Math.Max(3, myDialRectangle.Width * 0.035))
            Using hubShadowBrush As New SolidBrush(Color.FromArgb(90, Color.Black))
                .FillEllipse(hubShadowBrush, Center.X - HubRadius + 1, Center.Y - HubRadius + 2, HubRadius * 2, HubRadius * 2)
            End Using
            Using hubBrush As New SolidBrush(Y_DataClr(XY_Selected))
                .FillEllipse(hubBrush, Center.X - HubRadius, Center.Y - HubRadius, HubRadius * 2, HubRadius * 2)
            End Using

        End With

    End Sub

    Overrides Sub AddControlSpecificOptionItems()

        Dim TestStrip As ToolStripMenuItem
        Dim str1 As String
        Dim str2 As String()
        Dim str3 As String()

        str1 = resources.GetString("SDG_Configuration")
        str2 = {resources.GetString("SDG_ArcWidthDegrees"), resources.GetString("SDG_DirectionDegrees")}
        str3 = {"TXT"}

        TestStrip = CreateAToolStripMenuItem("F", str1, str2, str3)
        Contextmnu.Items.Add(TestStrip)

        str1 = resources.GetString("SDG_SweepDirection")
        str2 = {resources.GetString("SDG_Clockwise"), resources.GetString("SDG_Anticlockwise")}
        str3 = {}

        TestStrip = CreateAToolStripMenuItem("O", str1, str2, str3)
        Contextmnu.Items.Add(TestStrip)

        str1 = resources.GetString("SDG_Range")
        str2 = {resources.GetString("SDG_Minimum"), resources.GetString("SDG_Maximum")}
        str3 = {"TXT"}

        TestStrip = CreateAToolStripMenuItem("M", str1, str2, str3)
        Contextmnu.Items.Add(TestStrip)

    End Sub

    Public Overrides Sub ControlSpecificOptionSelection(ByVal Sent As String)
        Select Case Sent
            Case Is = "O_0"
                SweepClockwise = 1
                myConfiguration = Angle.ToString & " " & PointAngle.ToString & " " & SweepClockwise
            Case Is = "O_1"
                SweepClockwise = 0
                myConfiguration = Angle.ToString & " " & PointAngle.ToString & " " & SweepClockwise
            Case Else
                Dim Temp As String()
                Temp = Split(Sent, " ")
                If Temp(0) = "M_0_0" Then Y_Minimum(Y_Number_Allowed) = CDbl(Temp(1))
                If Temp(0) = "M_1_0" Then Y_Maximum(Y_Number_Allowed) = CDbl(Temp(1))
                If Temp(0) = "F_0_0" Then
                    Angle = CSng(Temp(1))
                    StartAngle = PointAngle - Angle / 2
                    myConfiguration = Angle.ToString & " " & PointAngle.ToString & " " & SweepClockwise
                End If
                If Temp(0) = "F_1_0" Then
                    PointAngle = CSng(Temp(1))
                    StartAngle = PointAngle - Angle / 2
                    myConfiguration = Angle.ToString & " " & PointAngle.ToString & " " & SweepClockwise
                End If
        End Select
    End Sub

    Public Overrides Function ControlSpecificSerializationData() As String

    End Function

    Public Overrides Sub ControlSpecficCreateFromSerializedData(ByVal Sent As String())
        Dim TempString() As String
        TempString = Split(myConfiguration, " ")
        Angle = CSng(TempString(0))
        If UBound(TempString) = 1 Then 'This is an older gauge version
            PointAngle = 270 'Up 'CHECK IT MAY MAKE MORE SENSE TO STORE THE ARC STARTING ANGLE
            SweepClockwise = 1
            myConfiguration = Angle.ToString & " " & PointAngle.ToString & " " & SweepClockwise.ToString
        Else
            PointAngle = CSng(TempString(1))
            SweepClockwise = CInt(TempString(2))
        End If
        StartAngle = PointAngle - Angle / 2
    End Sub
End Class
