Imports System.Drawing.Drawing2D

Public Class ChartControl
    'Resource manager for translated (resx satellite) strings.
    Private ReadOnly resources As New System.ComponentModel.ComponentResourceManager(GetType(ChartControl))
    Public PicOverlayHeight As Integer
    Public PicOverlayWidth As Integer
    Public XOverlayStartFraction As Double
    Public XOverlayEndFraction As Double
    Public YOverlayStartFraction As Double
    Public YOverlayEndFraction As Double
    Public OverlayPlotMax As Boolean
    Public OverlayFileCount As Integer = 0
    Public AnalyzedData(MAXDATAFILES, Main.LAST, Main.MAXDATAPOINTS) As Double
    Private Const MAXDATAFILES As Integer = 5
    Public xAxis As Double

    ''' <summary>
    ''' Draws one Y overlay axis (ticks, legend, column title/units/results, and plotted lines).
    ''' Shared by all four Y axes (Y1-Y4) in AnalysisForm's overlay chart - each axis differs only
    ''' in which side of the chart it's anchored to and which direction its ticks/labels extend:
    '''   TickBase: X pixel coordinate of the axis line itself
    '''     - Y1/Y2 (left axes):  CInt(PicOverlayWidth * XOverlayStartFraction)
    '''     - Y3/Y4 (right axes): CInt(PicOverlayWidth * XOverlayEndFraction)
    '''   TickSign: direction ticks/labels extend from TickBase, in units of TickLength
    '''     - Y1/Y3 (outer/inner, labels point away from TickBase): -1
    '''     - Y2/Y4 (labels point in the +X direction from TickBase): +1
    ''' </summary>
    Public Sub DrawOverlay(OverlayBMP As Graphics, TickBase As Integer, TickSign As Integer, AxisPen As Pen, AxisFont As Font, AxisBrush As SolidBrush, HeadingsFont As Font, YFont As Font, YBrush As SolidBrush, YPen As Pen, ResultsFont As Font,
                            yAxisValue As Double, YColumn As Integer, yMax As Double(), TitleLine As Integer, UnitsLine As Integer, ResultsLine As Integer(), yMaxAtX As Double(), yMaxAtSelectedX As Double(),
                            OverlayDashes As DashStyle(), EqualSpacingCount As Integer, EqualSpacingPointers As Double(,), cmbOverlayDataYSelectedIndex As Integer, cmbOverlayUnitsYSelectedIndex As Integer,
                           TickLength As Integer, cmbOverlayDataXSelectedIndex As Integer, cmbOverlayUnitsXSelectedIndex As Integer)
        Dim TickInterval As Double, TempString As String
        Dim FileCount As Integer

        With OverlayBMP

            TickInterval = PicOverlayHeight * (YOverlayEndFraction - YOverlayStartFraction) * 1 / 5
            Dim Counter As Integer
            For Counter = 0 To 4
                TempString = Main.NewCustomFormat((((yAxisValue) * Main.DataUnits(cmbOverlayDataYSelectedIndex, cmbOverlayUnitsYSelectedIndex)) / 5 * (5 - Counter)))
                .DrawLine(AxisPen, TickBase, CInt(PicOverlayHeight * YOverlayStartFraction + (TickInterval * Counter)), TickBase + TickSign * TickLength, CInt(PicOverlayHeight * YOverlayStartFraction + (TickInterval * Counter)))
                Dim TickLabelX As Integer = TickBase + TickSign * TickLength
                If TickSign < 0 Then TickLabelX -= CInt(.MeasureString(TempString, AxisFont).Width)
                .DrawString(TempString, AxisFont, AxisBrush, TickLabelX, CInt(PicOverlayHeight * YOverlayStartFraction + (TickInterval * Counter) - .MeasureString(TempString, AxisFont).Height / 2))
            Next
            TempString = Main.DataTags(cmbOverlayDataYSelectedIndex) & vbCrLf & "(" & Split(Main.DataUnitTags(cmbOverlayDataYSelectedIndex), " ")(cmbOverlayUnitsYSelectedIndex) & ")"
            Dim LegendX As Integer = TickBase
            If TickSign < 0 Then LegendX -= CInt(.MeasureString(TempString, YFont).Width)
            .DrawString(TempString, YFont, YBrush, LegendX, CInt(PicOverlayHeight * YOverlayStartFraction - 5 - .MeasureString(TempString, YFont).Height)) ' * 1.5))
            TempString = Main.DataTags(cmbOverlayDataYSelectedIndex)
            .DrawString(TempString, HeadingsFont, AxisBrush, YColumn - .MeasureString(TempString, HeadingsFont).Width / 2, TitleLine)
            If OverlayPlotMax Then
                TempString = resources.GetString("ChartControl_MaxPrefix") & Split(Main.DataUnitTags(cmbOverlayDataYSelectedIndex), " ")(cmbOverlayUnitsYSelectedIndex) & ")"
            Else
                TempString = "(" & Split(Main.DataUnitTags(cmbOverlayDataYSelectedIndex), " ")(cmbOverlayUnitsYSelectedIndex) & ")"
            End If

            .DrawString(TempString, HeadingsFont, AxisBrush, YColumn - .MeasureString(TempString, HeadingsFont).Width / 2, UnitsLine)
            For FileCount = 1 To OverlayFileCount
                If OverlayPlotMax Then
                    TempString = Main.NewCustomFormat(yMax(FileCount) * Main.DataUnits(cmbOverlayDataYSelectedIndex, cmbOverlayUnitsYSelectedIndex)) & " @ " & Main.NewCustomFormat(yMaxAtX(FileCount) * Main.DataUnits(cmbOverlayDataXSelectedIndex, cmbOverlayUnitsXSelectedIndex)) & " " & Split(Main.DataUnitTags(cmbOverlayDataXSelectedIndex), " ")(cmbOverlayUnitsXSelectedIndex)
                    .DrawString(TempString, ResultsFont, AxisBrush, YColumn - .MeasureString(TempString, ResultsFont).Width / 2, ResultsLine(FileCount))
                Else
                    TempString = Main.NewCustomFormat(yMaxAtSelectedX(FileCount) * Main.DataUnits(cmbOverlayDataYSelectedIndex, cmbOverlayUnitsYSelectedIndex)) ' & " @ " & Main.NewCustomFormat(OverlayXSelected * Main.DataUnits(cmbOverlayDataXSelectedIndex, cmbOverlayUnitsXSelectedIndex)) & " " & Split(Main.DataUnitTags(cmbOverlayDataXSelectedIndex), " ")(cmbOverlayUnitsXSelectedIndex)
                    .DrawString(TempString, ResultsFont, AxisBrush, YColumn - .MeasureString(TempString, ResultsFont).Width / 2, ResultsLine(FileCount))
                End If

                YPen.DashStyle = OverlayDashes(FileCount)
                For Counter = 2 To EqualSpacingCount - 1
                    .DrawLine(YPen, CInt(XOverlayStartFraction * PicOverlayWidth + ((AnalyzedData(FileCount, cmbOverlayDataXSelectedIndex, CInt(EqualSpacingPointers(FileCount, Counter)))) / xAxis) * (XOverlayEndFraction - XOverlayStartFraction) * PicOverlayWidth), CInt(YOverlayEndFraction * PicOverlayHeight - (AnalyzedData(FileCount, cmbOverlayDataYSelectedIndex, CInt(EqualSpacingPointers(FileCount, Counter))) / yAxisValue) * (YOverlayEndFraction - YOverlayStartFraction) * PicOverlayHeight), CInt(XOverlayStartFraction * PicOverlayWidth + ((AnalyzedData(FileCount, cmbOverlayDataXSelectedIndex, CInt(EqualSpacingPointers(FileCount, Counter + 1)))) / xAxis) * (XOverlayEndFraction - XOverlayStartFraction) * PicOverlayWidth), CInt(YOverlayEndFraction * PicOverlayHeight - (AnalyzedData(FileCount, cmbOverlayDataYSelectedIndex, CInt(EqualSpacingPointers(FileCount, Counter + 1))) / yAxisValue) * (YOverlayEndFraction - YOverlayStartFraction) * PicOverlayHeight))
                Next
            Next
        End With

    End Sub

End Class
