Imports System
Imports System.IO
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SimpleDyno

<TestClass>
Public Class WriteRawDataToFileTests

    Private Shared MainInstance As Main

    <ClassInitialize>
    Public Shared Sub ClassInit(context As TestContext)
        'WriteRawDataToFile's header writes Main.DataUnitTags/DataTags-derived column titles, so
        'PrepareArrays must have run first. It also reads Main.frmDyno.* (Car_Mass, Frontal_Area, etc.) -
        'normally created in Form1_Load, which New Main() alone does not run - so it must be created here
        'too, or Main.frmDyno stays Nothing and WriteRawDataToFile throws a NullReferenceException.
        MainInstance = New Main()
        MainInstance.PrepareArrays()
        Main.frmDyno = New Dyno()
    End Sub

    ''' <summary>
    ''' Fills Main.CollectedData/DataPoints with synthetic, monotonically-increasing session data and
    ''' sets the cached session fields WriteRawDataToFile reads instead of live ComboBoxes.
    ''' </summary>
    Private Shared Sub SeedSyntheticSession(dataPoints As Integer)
        Main.DataPoints = dataPoints
        For i As Integer = 0 To dataPoints
            Main.CollectedData(Main.SESSIONTIME, i) = i
            Main.CollectedData(Main.RPM1_ROLLER, i) = 100 + i * 10
            Main.CollectedData(Main.RPM2, i) = 0
            Main.CollectedData(Main.VOLTS, i) = 12
            Main.CollectedData(Main.AMPS, i) = 1
        Next
        MainInstance.SessionAcquisitionText = "COM Port Only"
        MainInstance.SessionCOMPortText = "COM9"
        MainInstance.SessionBaudRateText = "9600"
    End Sub

    <TestMethod>
    Public Sub WriteRawDataToFile_CreatesFileWithHeaderAndDataSection()
        SeedSyntheticSession(5)
        Dim targetFile = Path.Combine(Path.GetTempPath(), $"SimpleDynoTest_{Guid.NewGuid()}.sdr")
        Try
            MainInstance.WriteRawDataToFile(targetFile)

            Assert.IsTrue(File.Exists(targetFile))
            Dim lines = File.ReadAllLines(targetFile)
            Assert.IsTrue(lines.Contains("PRIMARY_CHANNEL_RAW_DATA"))
            Assert.IsTrue(lines.Any(Function(l) l.StartsWith("Acquisition: COM Port Only")))
            Assert.IsTrue(lines.Any(Function(l) l.StartsWith("COM_Port: COM9")))
            Assert.IsTrue(lines.Any(Function(l) l.StartsWith("Baud_Rate: 9600")))
            Assert.IsTrue(lines.Any(Function(l) l.StartsWith("NUMBER_OF_POINTS_COLLECTED 5")))
        Finally
            If File.Exists(targetFile) Then File.Delete(targetFile)
        End Try
    End Sub

    <TestMethod>
    Public Sub WriteRawDataToFile_UsesNoComPortAndNoBaudRatePlaceholdersWhenEmpty()
        SeedSyntheticSession(3)
        MainInstance.SessionCOMPortText = ""
        MainInstance.SessionBaudRateText = ""
        Dim targetFile = Path.Combine(Path.GetTempPath(), $"SimpleDynoTest_{Guid.NewGuid()}.sdr")
        Try
            MainInstance.WriteRawDataToFile(targetFile)

            Dim lines = File.ReadAllLines(targetFile)
            Assert.IsTrue(lines.Contains("No_COM_Port_Selected"))
            Assert.IsTrue(lines.Contains("No_Baud_Rate_Selected"))
        Finally
            If File.Exists(targetFile) Then File.Delete(targetFile)
        End Try
    End Sub

    ''' <summary>
    ''' Regression guard for the DataPoints-1 bound: DataReceivedHandler/myWaveHandler_ProcessWave
    ''' increment DataPoints BEFORE populating CollectedData(*, DataPoints), so that last row can still be
    ''' mid-write while a session is active. WriteRawDataToFile must never emit that in-flight row.
    ''' </summary>
    <TestMethod>
    Public Sub WriteRawDataToFile_NeverWritesTheInFlightLastRow()
        Const dataPoints As Integer = 6
        SeedSyntheticSession(dataPoints)
        Const sentinelRpm As Double = 999999.0
        Main.CollectedData(Main.RPM1_ROLLER, dataPoints) = sentinelRpm 'still "in flight", must be skipped

        Dim targetFile = Path.Combine(Path.GetTempPath(), $"SimpleDynoTest_{Guid.NewGuid()}.sdr")
        Try
            MainInstance.WriteRawDataToFile(targetFile)

            Dim lines = File.ReadAllLines(targetFile)
            Dim dataSectionStart = Array.IndexOf(lines, "PRIMARY_CHANNEL_RAW_DATA")
            Assert.IsTrue(dataSectionStart >= 0)
            'Layout after PRIMARY_CHANNEL_RAW_DATA: NUMBER_OF_POINTS_COLLECTED line, column-header line,
            'then one line per recalculated data point (count = 1 To DataPoints - 1). Skip +3 to get past
            'both the NUMBER_OF_POINTS_COLLECTED line AND the column-header line, not just one of them.
            Dim dataLines = lines.Skip(dataSectionStart + 3).Where(Function(l) l.Trim() <> "").ToList()

            Assert.AreEqual(dataPoints - 1, dataLines.Count, "Expected exactly DataPoints - 1 data rows")
            Assert.IsFalse(dataLines.Any(Function(l) l.Contains(sentinelRpm.ToString("0"))), "The in-flight last row must never be written")
        Finally
            If File.Exists(targetFile) Then File.Delete(targetFile)
        End Try
    End Sub

End Class
