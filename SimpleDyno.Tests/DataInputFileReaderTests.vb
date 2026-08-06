Imports System
Imports System.Globalization
Imports System.IO
Imports System.Threading
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SimpleDyno

<TestClass>
Public Class DataInputFileReaderTests

    Private Shared FixturePath As String

    <ClassInitialize>
    Public Shared Sub ClassInit(context As TestContext)
        'ReadDataFile2 maps column titles to Main.DataTags/DataUnitTags, so PrepareArrays must have run
        'first - see DataInputFileReader.vb's ReadDataFile2, which builds each SearchString from those
        'Public Shared arrays.
        Dim m As New Main()
        m.PrepareArrays()

        FixturePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test-data", "sample_power_run.sdp")
    End Sub

    <TestMethod>
    Public Sub ReadDataFile2_ReturnsAllTwentyRecords()
        Dim reader As New DataInputFileReader()
        Dim records = reader.ReadDataFile2(FixturePath)
        Assert.AreEqual(20, records.Count)
    End Sub

    ''' <summary>
    ''' ReadDataFile2 converts each field with Double.Parse(..., CultureInfo.InvariantCulture) (fixed by
    ''' commit ad1c6c7), so the fixture's "." is always the decimal point regardless of the machine's
    ''' regional settings. Expected values are hardcoded Double literals (parsed by the VB compiler, not
    ''' at test-run time) rather than CDbl(String) - using CDbl(String) here would silently reintroduce
    ''' the exact bug this test is meant to guard against, since CDbl parses using the CurrentCulture of
    ''' whatever machine runs the test (see ReadDataFile2_ParsesCorrectlyUnderPtBrCulture below).
    ''' </summary>
    <TestMethod>
    Public Sub ReadDataFile2_FirstRecordMatchesKnownValues()
        Dim reader As New DataInputFileReader()
        Dim records = reader.ReadDataFile2(FixturePath)
        Dim first = records(0)

        Assert.AreEqual(0.4, first.Time, 0.0001)
        Assert.AreEqual(111.0029, first.RPM1_Roller, 0.0001)
        Assert.AreEqual(992.2116, first.Power, 0.0001)
        Assert.AreEqual(48.0, first.Voltage, 0.0001)
        Assert.AreEqual(28.885, first.Current, 0.0001)
    End Sub

    <TestMethod>
    Public Sub ReadDataFile2_LastRecordMatchesKnownValues()
        Dim reader As New DataInputFileReader()
        Dim records = reader.ReadDataFile2(FixturePath)
        Dim last = records(records.Count - 1)

        Assert.AreEqual(8.0, last.Time, 0.0001)
        Assert.AreEqual(628.3185, last.RPM1_Roller, 0.0001)
        Assert.AreEqual(5026.5482, last.Power, 0.0001)
    End Sub

    ''' <summary>
    ''' Regression guard for commit ad1c6c7 (culture-dependent number parsing). The app never forces
    ''' Thread.CurrentThread.CurrentCulture to Invariant (only CurrentUICulture is changed, in
    ''' ApplicationEvents.vb) - so on a Windows machine set to pt-BR, this test's CurrentCulture during
    ''' the run is pt-BR too, same as the target user's PC. Under pt-BR, "." is a group separator, not a
    ''' decimal point (e.g. CDbl("111.0029") = 1110029, confirmed empirically), so if ReadDataFile2 ever
    ''' regresses back to CDbl/culture-dependent parsing instead of Double.Parse(InvariantCulture), this
    ''' test fails loudly instead of only failing silently on an actual user's machine.
    ''' </summary>
    <TestMethod>
    Public Sub ReadDataFile2_ParsesCorrectlyUnderPtBrCulture()
        Dim originalCulture = Thread.CurrentThread.CurrentCulture
        Try
            Thread.CurrentThread.CurrentCulture = New CultureInfo("pt-BR")

            Dim reader As New DataInputFileReader()
            Dim records = reader.ReadDataFile2(FixturePath)
            Dim first = records(0)

            Assert.AreEqual(0.4, first.Time, 0.0001)
            Assert.AreEqual(111.0029, first.RPM1_Roller, 0.0001)
            Assert.AreEqual(992.2116, first.Power, 0.0001)
        Finally
            Thread.CurrentThread.CurrentCulture = originalCulture
        End Try
    End Sub

    <TestMethod>
    <ExpectedException(GetType(FileNotFoundException))>
    Public Sub ReadDataFile2_EmptyFileNameThrowsFileNotFoundException()
        Dim reader As New DataInputFileReader()
        reader.ReadDataFile2("")
    End Sub

    <TestMethod>
    <ExpectedException(GetType(FileNotFoundException))>
    Public Sub ReadDataFile2_MissingFileThrowsFileNotFoundException()
        Dim reader As New DataInputFileReader()
        reader.ReadDataFile2(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "this_file_does_not_exist.sdp"))
    End Sub

End Class
