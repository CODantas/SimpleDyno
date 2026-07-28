Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SimpleDyno

<TestClass>
Public Class PrepareArraysTests

    <ClassInitialize>
    Public Shared Sub ClassInit(context As TestContext)
        'New Main() only runs InitializeComponent (no Form1_Load, no hardware enumeration) - see
        'Main.vb's constructor. PrepareArrays only populates the Public Shared DataTags/DataUnitTags/
        'DataUnits/DataAreUsed/DataActions arrays, no UI/IO/hardware side effects.
        Dim m As New Main()
        m.PrepareArrays()
    End Sub

    <TestMethod>
    Public Sub AllDataActionsAreAssigned()
        'Regression guard: this is exactly the shape of bug fixed earlier (DataActions(CHAN1_DUTYCYCLE)
        'was accidentally never assigned due to a copy-paste typo targeting CHAN2_DUTYCYCLE instead).
        For i As Integer = 0 To Main.LAST - 1
            Assert.IsNotNull(Main.DataActions(i), $"Main.DataActions({i}) was never assigned")
        Next
    End Sub

    <TestMethod>
    Public Sub AllDataTagsAreNonEmpty()
        For i As Integer = 0 To Main.LAST - 1
            Assert.IsFalse(String.IsNullOrEmpty(Main.DataTags(i)), $"Main.DataTags({i}) is empty")
        Next
    End Sub

    <TestMethod>
    Public Sub AllDataUnitTagsAreNonEmpty()
        For i As Integer = 0 To Main.LAST - 1
            Assert.IsFalse(String.IsNullOrEmpty(Main.DataUnitTags(i)), $"Main.DataUnitTags({i}) is empty")
        Next
    End Sub

    <TestMethod>
    Public Sub KnownIndicesHaveExpectedTags()
        Assert.AreEqual("Time", Main.DataTags(Main.SESSIONTIME))
        Assert.AreEqual("RPM1 Roller", Main.DataTags(Main.RPM1_ROLLER))
        Assert.AreEqual("Power", Main.DataTags(Main.POWER))
    End Sub

    <TestMethod>
    Public Sub Chan1DutyCycleActionReadsCh1DutyCycleField()
        'Directly exercises the exact bug that was fixed: DataActions(CHAN1_DUTYCYCLE) must read
        'Ch1_Duty_Cycle, not Ch2_Duty_Cycle.
        Dim record As New DataRecord()
        record.Ch1_Duty_Cycle = 12.5
        record.Ch2_Duty_Cycle = 99.9
        Assert.AreEqual(12.5, Main.DataActions(Main.CHAN1_DUTYCYCLE)(record), 0.0001)
    End Sub

End Class
