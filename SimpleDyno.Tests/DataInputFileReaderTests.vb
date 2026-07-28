Imports System
Imports System.IO
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
    ''' ReadDataFile2 converts each field with CDbl(String), which parses using the process's
    ''' CurrentCulture (not necessarily period-as-decimal - on a pt-BR machine "." is a group separator,
    ''' so CDbl("0.4") <> 0.4). Comparing against CDbl(literal) instead of a hardcoded Double keeps this
    ''' test a valid regression guard for column-name-to-field mapping under any host culture, without
    ''' asserting a specific (possibly wrong here) numeric interpretation of the fixture text.
    ''' </summary>
    <TestMethod>
    Public Sub ReadDataFile2_FirstRecordMatchesKnownValues()
        Dim reader As New DataInputFileReader()
        Dim records = reader.ReadDataFile2(FixturePath)
        Dim first = records(0)

        Assert.AreEqual(CDbl("0.4"), first.Time, 0.0001)
        Assert.AreEqual(CDbl("111.0029"), first.RPM1_Roller, 0.0001)
        Assert.AreEqual(CDbl("992.2116"), first.Power, 0.0001)
        Assert.AreEqual(CDbl("48.0"), first.Voltage, 0.0001)
        Assert.AreEqual(CDbl("28.885"), first.Current, 0.0001)
    End Sub

    <TestMethod>
    Public Sub ReadDataFile2_LastRecordMatchesKnownValues()
        Dim reader As New DataInputFileReader()
        Dim records = reader.ReadDataFile2(FixturePath)
        Dim last = records(records.Count - 1)

        Assert.AreEqual(CDbl("8.0"), last.Time, 0.0001)
        Assert.AreEqual(CDbl("628.3185"), last.RPM1_Roller, 0.0001)
        Assert.AreEqual(CDbl("5026.5482"), last.Power, 0.0001)
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
