Imports System.Globalization
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SimpleDyno

<TestClass>
Public Class MainPureFunctionsTests

    <TestMethod>
    Public Sub CustomRound_ZeroStaysZero()
        Assert.AreEqual(0.0, Main.CustomRound(0.0), 0.0001)
    End Sub

    <TestMethod>
    Public Sub CustomRound_NegativeValuesPassThroughUnchanged()
        'CustomRound only transforms Sent when Sent > 0; negative input is returned as-is.
        Assert.AreEqual(-5.0, Main.CustomRound(-5.0), 0.0001)
    End Sub

    <TestMethod>
    Public Sub CustomRound_RoundsUpWithinMagnitudeBand()
        Assert.AreEqual(40.0, Main.CustomRound(37.0), 0.0001)
        Assert.AreEqual(45.0, Main.CustomRound(40.0), 0.0001)
        Assert.AreEqual(4.5, Main.CustomRound(4.0), 0.0001)
        Assert.AreEqual(0.045, Main.CustomRound(0.04), 0.0001)
    End Sub

    ''' <summary>
    ''' NewCustomFormat's Double.ToString(format) uses Thread.CurrentThread.CurrentCulture's decimal
    ''' separator, which for this app can be a comma (pt-BR). Parse the result back with the same
    ''' culture instead of comparing literal strings, so the test doesn't depend on which culture the
    ''' test host happens to run under.
    ''' </summary>
    Private Function ParseFormatted(formatted As String) As Double
        Return Double.Parse(formatted, NumberStyles.Any, CultureInfo.CurrentCulture)
    End Function

    <TestMethod>
    Public Sub NewCustomFormat_HundredsAndAboveRoundToInteger()
        Assert.AreEqual(151.0, ParseFormatted(Main.NewCustomFormat(150.6)), 0.0001)
    End Sub

    <TestMethod>
    Public Sub NewCustomFormat_TensUseOneDecimal()
        Assert.AreEqual(15.7, ParseFormatted(Main.NewCustomFormat(15.678)), 0.01)
    End Sub

    <TestMethod>
    Public Sub NewCustomFormat_OnesUseTwoDecimals()
        Assert.AreEqual(5.43, ParseFormatted(Main.NewCustomFormat(5.4321)), 0.001)
    End Sub

    <TestMethod>
    Public Sub NewCustomFormat_TenthsUseThreeDecimals()
        Assert.AreEqual(0.543, ParseFormatted(Main.NewCustomFormat(0.5432)), 0.0001)
    End Sub

    <TestMethod>
    Public Sub NewCustomFormat_ThousandthsUseFourDecimals()
        Assert.AreEqual(0.0054, ParseFormatted(Main.NewCustomFormat(0.005432)), 0.00001)
    End Sub

    <TestMethod>
    Public Sub NewCustomFormat_BelowThousandthsFallsBackToInteger()
        Assert.AreEqual(0.0, ParseFormatted(Main.NewCustomFormat(0.0001)), 0.0001)
    End Sub

    <TestMethod>
    Public Sub CheckNumericalLimits_WithinRangeIsTrue()
        Assert.IsTrue(Main.CheckNumericalLimits(0.0, 10.0, 5.0))
        Assert.IsTrue(Main.CheckNumericalLimits(0.0, 10.0, 0.0))
        Assert.IsTrue(Main.CheckNumericalLimits(0.0, 10.0, 10.0))
    End Sub

    <TestMethod>
    Public Sub CheckNumericalLimits_OutsideRangeIsFalse()
        Assert.IsFalse(Main.CheckNumericalLimits(0.0, 10.0, -0.001))
        Assert.IsFalse(Main.CheckNumericalLimits(0.0, 10.0, 10.001))
    End Sub

End Class
