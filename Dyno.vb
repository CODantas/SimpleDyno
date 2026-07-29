Public Class Dyno
    'Dyno Parameters
    Friend CarMass As Double
    Friend AxleMass As Double
    Friend AxleDiameter As Double
    Friend EndCapMass As Double
    Friend FrontalArea As Double
    Friend DragCoefficient As Double
    Friend SignalsPerRPM As Double
    Friend SignalsPerRPM2 As Double
    Friend WheelDiameter As Double
    Friend RollerDiameter As Double
    Friend RollerCircumference As Double
    Friend RollerWallThickness As Double
    Friend RollerMass As Double
    Friend ExtraDiameter As Double
    Friend ExtraWallThickness As Double
    Friend ExtraMass As Double

    'Dyno Calculations
    Friend IdealMomentOfInertia As Double
    Friend IdealRollerMass As Double

    'Global Temporary Double for Checking the numeric input of the textboxes
    Private TempDouble As Double
    Friend Sub Dyno_Setup()
        txtCarMass.Select()
    End Sub
    Private Sub Dyno_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        'Prevents form from actually closing, rather it hides
        If e.CloseReason <> CloseReason.FormOwnerClosing Then
            Me.Hide()
            e.Cancel = True
            Main.btnShow_click(Me, System.EventArgs.Empty)
        End If
    End Sub
    Private Sub txtCarMass_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCarMass.Enter
        picDynoSettings.BackgroundImage = My.Resources.CarMass
        lblDynoSettings.Text = resources.GetString("Dyno_Help_CarMass")
    End Sub
    Private Sub txtCarMass_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCarMass.Leave
        Dim LocalMin As Double = 1
        Dim LocalMax As Double = 999999
        If Double.TryParse(CType(sender, TextBox).Text, TempDouble) AndAlso Main.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) Then
            CarMass = TempDouble
            UpdateMomentOfInertias()
        Else
            MsgBox(CType(sender, TextBox).Name & resources.GetString("Dyno_ValueMustBeBetween") & LocalMin & resources.GetString("Dyno_And") & LocalMax, MsgBoxStyle.Exclamation)
            With CType(sender, TextBox)
                .Text = CarMass.ToString
                .Focus()
            End With
        End If
    End Sub
    Private Sub txtFrontalArea_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtFrontalArea.Enter
        picDynoSettings.BackgroundImage = My.Resources.FrontalArea
        lblDynoSettings.Text = resources.GetString("Dyno_Help_FrontalArea")
    End Sub
    Private Sub txtFrontalArea_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtFrontalArea.Leave
        Dim LocalMin As Double = 0
        Dim LocalMax As Double = 999999
        If Double.TryParse(CType(sender, TextBox).Text, TempDouble) AndAlso Main.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) Then
            FrontalArea = TempDouble
            UpdateMomentOfInertias()
        Else
            MsgBox(CType(sender, TextBox).Name & resources.GetString("Dyno_ValueMustBeBetween") & LocalMin & resources.GetString("Dyno_And") & LocalMax, MsgBoxStyle.Exclamation)
            With CType(sender, TextBox)
                .Text = FrontalArea.ToString
                .Focus()
            End With
        End If
    End Sub
    Private Sub txtDragCoefficient_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDragCoefficient.Enter
        picDynoSettings.BackgroundImage = My.Resources.DragImage
        lblDynoSettings.Text = resources.GetString("Dyno_Help_DragCoefficient")
    End Sub
    Private Sub txtDragCoefficient_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDragCoefficient.Leave
        Dim LocalMin As Double = 0
        Dim LocalMax As Double = 1
        If Double.TryParse(CType(sender, TextBox).Text, TempDouble) AndAlso Main.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) Then
            DragCoefficient = TempDouble
            UpdateMomentOfInertias()
        Else
            MsgBox(CType(sender, TextBox).Name & resources.GetString("Dyno_ValueMustBeBetween") & LocalMin & resources.GetString("Dyno_And") & LocalMax, MsgBoxStyle.Exclamation)
            With CType(sender, TextBox)
                .Text = DragCoefficient.ToString
                .Focus()
            End With
        End If
    End Sub
    Private Sub txtGearRatio_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGearRatio.Enter
        picDynoSettings.BackgroundImage = My.Resources.GearRatio
        lblDynoSettings.Text = resources.GetString("Dyno_Help_GearRatio")
    End Sub
    Private Sub txtGearRatio_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGearRatio.Leave
        Dim LocalMin As Double = 0.1
        Dim LocalMax As Double = 999999
        If Double.TryParse(CType(sender, TextBox).Text, TempDouble) AndAlso Main.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) Then
            Main.GearRatio = TempDouble
            UpdateMomentOfInertias()
        Else
            MsgBox(CType(sender, TextBox).Name & resources.GetString("Dyno_ValueMustBeBetween") & LocalMin & resources.GetString("Dyno_And") & LocalMax, MsgBoxStyle.Exclamation)
            With CType(sender, TextBox)
                .Text = Main.GearRatio.ToString
                .Focus()
            End With
        End If
    End Sub
    Private Sub txtWheelDiameter_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtWheelDiameter.Enter
        picDynoSettings.BackgroundImage = My.Resources.WheelDiameter
        lblDynoSettings.Text = resources.GetString("Dyno_Help_WheelDiameter")
    End Sub
    Private Sub txtWheelDiameter_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtWheelDiameter.Leave
        Dim LocalMin As Double = 1
        Dim LocalMax As Double = 999999
        If Double.TryParse(CType(sender, TextBox).Text, TempDouble) AndAlso Main.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) Then
            WheelDiameter = TempDouble
            UpdateMomentOfInertias()
        Else
            MsgBox(CType(sender, TextBox).Name & resources.GetString("Dyno_ValueMustBeBetween") & LocalMin & resources.GetString("Dyno_And") & LocalMax, MsgBoxStyle.Exclamation)
            With CType(sender, TextBox)
                .Text = WheelDiameter.ToString
                .Focus()
            End With
        End If
    End Sub
    Private Sub txtRollerDiameter_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRollerDiameter.Enter
        picDynoSettings.BackgroundImage = My.Resources.RollerDiameter
        lblDynoSettings.Text = resources.GetString("Dyno_Help_RollerDiameter")
    End Sub
    Private Sub txtRollerDiameter_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRollerDiameter.Leave
        Dim LocalMin As Double = 1
        Dim LocalMax As Double = 999999
        If Double.TryParse(CType(sender, TextBox).Text, TempDouble) AndAlso Main.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) Then
            RollerDiameter = TempDouble
            UpdateMomentOfInertias()
        Else
            MsgBox(CType(sender, TextBox).Name & resources.GetString("Dyno_ValueMustBeBetween") & LocalMin & resources.GetString("Dyno_And") & LocalMax, MsgBoxStyle.Exclamation)
            With CType(sender, TextBox)
                .Text = RollerDiameter.ToString
                .Focus()
            End With
        End If
    End Sub
    Private Sub txtRollerWallThickness_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRollerWallThickness.Enter
        picDynoSettings.BackgroundImage = My.Resources.RollerWallThickness
        lblDynoSettings.Text = resources.GetString("Dyno_Help_RollerWallThickness")
    End Sub
    Private Sub txtRollerWallThickness_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRollerWallThickness.Leave
        Dim LocalMin As Double = 1
        Dim LocalMax As Double = RollerDiameter / 2
        If Double.TryParse(CType(sender, TextBox).Text, TempDouble) AndAlso Main.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) Then
            RollerWallThickness = TempDouble
            UpdateMomentOfInertias()
        Else
            MsgBox(CType(sender, TextBox).Name & resources.GetString("Dyno_ValueMustBeBetween") & LocalMin & resources.GetString("Dyno_And") & LocalMax, MsgBoxStyle.Exclamation)
            With CType(sender, TextBox)
                .Text = RollerWallThickness.ToString
                .Focus()
            End With
        End If
    End Sub
    Private Sub txtRollerMass_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRollerMass.Enter
        picDynoSettings.BackgroundImage = My.Resources.RollerMass
        lblDynoSettings.Text = resources.GetString("Dyno_Help_RollerMass")
    End Sub
    Private Sub txtRollerMass_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRollerMass.Leave
        Dim LocalMin As Double = 1
        Dim LocalMax As Double = 999999
        If Double.TryParse(CType(sender, TextBox).Text, TempDouble) AndAlso Main.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) Then
            RollerMass = TempDouble
            UpdateMomentOfInertias()
        Else
            MsgBox(CType(sender, TextBox).Name & resources.GetString("Dyno_ValueMustBeBetween") & LocalMin & resources.GetString("Dyno_And") & LocalMax, MsgBoxStyle.Exclamation)
            With CType(sender, TextBox)
                .Text = RollerMass.ToString
                .Focus()
            End With
        End If
    End Sub
    Private Sub txtAxleDiameter_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtAxleDiameter.Enter
        picDynoSettings.BackgroundImage = My.Resources.AxleDiameter
        lblDynoSettings.Text = resources.GetString("Dyno_Help_AxleDiameter")
    End Sub
    Private Sub txtAxleDiameter_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtAxleDiameter.Leave
        Dim LocalMin As Double = 0
        Dim LocalMax As Double = 999999
        If Double.TryParse(CType(sender, TextBox).Text, TempDouble) AndAlso Main.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) Then
            AxleDiameter = TempDouble
            UpdateMomentOfInertias()
        Else
            MsgBox(CType(sender, TextBox).Name & resources.GetString("Dyno_ValueMustBeBetween") & LocalMin & resources.GetString("Dyno_And") & LocalMax, MsgBoxStyle.Exclamation)
            With CType(sender, TextBox)
                .Text = AxleDiameter.ToString
                .Focus()
            End With
        End If
    End Sub
    Private Sub txtAxleMass_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtAxleMass.Enter
        picDynoSettings.BackgroundImage = My.Resources.AxelMass
        lblDynoSettings.Text = resources.GetString("Dyno_Help_AxleMass")
    End Sub
    Private Sub txtAxleMass_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtAxleMass.Leave
        Dim LocalMin As Double = 0
        Dim LocalMax As Double = 999999
        If Double.TryParse(CType(sender, TextBox).Text, TempDouble) AndAlso Main.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) Then
            AxleMass = TempDouble
            UpdateMomentOfInertias()
        Else
            MsgBox(CType(sender, TextBox).Name & resources.GetString("Dyno_ValueMustBeBetween") & LocalMin & resources.GetString("Dyno_And") & LocalMax, MsgBoxStyle.Exclamation)
            With CType(sender, TextBox)
                .Text = AxleMass.ToString
                .Focus()
            End With
        End If
    End Sub
    Private Sub txtEndCapMass_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtEndCapMass.Enter
        picDynoSettings.BackgroundImage = My.Resources.EndCapMass
        lblDynoSettings.Text = resources.GetString("Dyno_Help_EndCapMass")
    End Sub
    Private Sub txtEndCapMass_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtEndCapMass.Leave
        Dim LocalMin As Double = 0
        Dim LocalMax As Double = 999999
        If Double.TryParse(CType(sender, TextBox).Text, TempDouble) AndAlso Main.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) Then
            EndCapMass = TempDouble
            UpdateMomentOfInertias()
        Else
            MsgBox(CType(sender, TextBox).Name & resources.GetString("Dyno_ValueMustBeBetween") & LocalMin & resources.GetString("Dyno_And") & LocalMax, MsgBoxStyle.Exclamation)
            With CType(sender, TextBox)
                .Text = EndCapMass.ToString
                .Focus()
            End With
        End If
    End Sub
    Private Sub txtExtraDiameter_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtExtraDiameter.Enter
        picDynoSettings.BackgroundImage = My.Resources.ExtraDiameter
        lblDynoSettings.Text = resources.GetString("Dyno_Help_ExtraDiameter")
    End Sub
    Private Sub txtExtraDiameter_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtExtraDiameter.Leave
        Dim LocalMin As Double = 0
        Dim LocalMax As Double = 999999
        If Double.TryParse(CType(sender, TextBox).Text, TempDouble) AndAlso Main.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) Then
            ExtraDiameter = TempDouble
            UpdateMomentOfInertias()
        Else
            MsgBox(CType(sender, TextBox).Name & resources.GetString("Dyno_ValueMustBeBetween") & LocalMin & resources.GetString("Dyno_And") & LocalMax, MsgBoxStyle.Exclamation)
            With CType(sender, TextBox)
                .Text = ExtraDiameter.ToString
                .Focus()
            End With
        End If
    End Sub
    Private Sub txtExtraWallThickness_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtExtraWallThickness.Enter
        picDynoSettings.BackgroundImage = My.Resources.ExtraWallThickness
        lblDynoSettings.Text = resources.GetString("Dyno_Help_ExtraWallThickness")
    End Sub
    Private Sub txtExtraWallThickness_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtExtraWallThickness.Leave
        Dim LocalMin As Double = 0
        Dim LocalMax As Double = 999999
        If Double.TryParse(CType(sender, TextBox).Text, TempDouble) AndAlso Main.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) Then
            ExtraWallThickness = TempDouble
            UpdateMomentOfInertias()
        Else
            MsgBox(CType(sender, TextBox).Name & resources.GetString("Dyno_ValueMustBeBetween") & LocalMin & resources.GetString("Dyno_And") & LocalMax, MsgBoxStyle.Exclamation)
            With CType(sender, TextBox)
                .Text = ExtraWallThickness.ToString
                .Focus()
            End With
        End If
    End Sub
    Private Sub txtExtraMass_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtExtraMass.Enter
        picDynoSettings.BackgroundImage = My.Resources.ExtraMass
        lblDynoSettings.Text = resources.GetString("Dyno_Help_ExtraMass")
    End Sub
    Private Sub txtExtraMass_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtExtraMass.Leave
        Dim LocalMin As Double = 0
        Dim LocalMax As Double = 999999
        If Double.TryParse(CType(sender, TextBox).Text, TempDouble) AndAlso Main.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) Then
            ExtraMass = TempDouble
            UpdateMomentOfInertias()
        Else
            MsgBox(CType(sender, TextBox).Name & resources.GetString("Dyno_ValueMustBeBetween") & LocalMin & resources.GetString("Dyno_And") & LocalMax, MsgBoxStyle.Exclamation)
            With CType(sender, TextBox)
                .Text = ExtraMass.ToString
                .Focus()
            End With
        End If
    End Sub
    Private Sub txtSignalsPerRPM_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSignalsPerRPM1.Enter
        picDynoSettings.BackgroundImage = My.Resources.SignalsPerRPM
        lblDynoSettings.Text = resources.GetString("Dyno_Help_SignalsPerRPM1")
    End Sub
    Private Sub txtSignalsPerRPM_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSignalsPerRPM1.Leave
        Dim LocalMin As Double = 0.1
        Dim LocalMax As Double = 50
        If Double.TryParse(CType(sender, TextBox).Text, TempDouble) AndAlso Main.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) Then
            SignalsPerRPM = TempDouble
            UpdateMomentOfInertias()
        Else
            MsgBox(CType(sender, TextBox).Name & resources.GetString("Dyno_ValueMustBeBetween") & LocalMin & resources.GetString("Dyno_And") & LocalMax, MsgBoxStyle.Exclamation)
            With CType(sender, TextBox)
                .Text = SignalsPerRPM.ToString
                .Focus()
            End With
        End If
    End Sub
    Private Sub txtSignalsPerRPM2_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSignalsPerRPM2.Enter
        picDynoSettings.BackgroundImage = My.Resources.SignalsPerRPM
        lblDynoSettings.Text = resources.GetString("Dyno_Help_SignalsPerRPM2")
    End Sub
    Private Sub txtSignalsPerRPM2_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSignalsPerRPM2.Leave
        Dim LocalMin As Double = 0.1
        Dim LocalMax As Double = 50
        If Double.TryParse(CType(sender, TextBox).Text, TempDouble) AndAlso Main.CheckNumericalLimits(LocalMin, LocalMax, TempDouble) Then
            SignalsPerRPM2 = TempDouble
            UpdateMomentOfInertias()
        Else
            MsgBox(CType(sender, TextBox).Name & resources.GetString("Dyno_ValueMustBeBetween") & LocalMin & resources.GetString("Dyno_And") & LocalMax, MsgBoxStyle.Exclamation)
            With CType(sender, TextBox)
                .Text = SignalsPerRPM2.ToString
                .Focus()
            End With
        End If
    End Sub
    Friend Sub UpdateMomentOfInertias()
        Dim RollerMomentOfInertia As Double
        Dim AxleMomentOfInertia As Double
        Dim EndCapMomentOfInertia As Double
        Dim ExtraMomentOfInertia As Double

        Dim r1 As Double, r2 As Double, m As Double

        'Using I = 1/2 x m x (r1^2 + r2^2)
        'Roller
        m = RollerMass / 1000.0
        r1 = (RollerDiameter / 2.0 - RollerWallThickness) / 1000.0
        r2 = RollerDiameter / 2.0 / 1000.0
        RollerMomentOfInertia = 1 / 2 * m * (r1 ^ 2 + r2 ^ 2)
        'Axle
        m = AxleMass / 1000.0
        r1 = 0
        r2 = AxleDiameter / 2.0 / 1000.0
        AxleMomentOfInertia = 1 / 2 * m * (r1 ^ 2 + r2 ^ 2)
        'End Cap
        m = EndCapMass / 1000.0
        r1 = AxleDiameter / 2.0 / 1000.0
        r2 = (RollerDiameter / 2.0 - RollerWallThickness) / 1000.0
        EndCapMomentOfInertia = 1 / 2 * m * (r1 ^ 2 + r2 ^ 2)
        'Extras
        m = ExtraMass / 1000.0
        r1 = ExtraDiameter / 2.0 / 1000.0
        r2 = (ExtraDiameter / 2.0 - ExtraWallThickness) / 1000.0
        ExtraMomentOfInertia = 1 / 2 * m * (r1 ^ 2 + r2 ^ 2)
        'Total
        Main.DynoMomentOfInertia = RollerMomentOfInertia + AxleMomentOfInertia + EndCapMomentOfInertia + ExtraMomentOfInertia
        'Ideal Roller Mass
        'Car outputs 1 N force which will give F/m acceleration
        Dim CarAcceleration As Double = 1 / (CarMass / 1000.0) 'm/s^2
        'This equals the angular acceleration of the roller
        Dim RollerAcceleration As Double = CarAcceleration / (RollerDiameter / 1000.0 / 2.0) 'radians/s^2
        'Torque is the same force through the radius of the roller
        Dim RollerTorque As Double = 1 * RollerDiameter / 1000.0 / 2.0
        'Torque is also the moment of inertia by angular accleration
        'Therefore, Torque / angular acceleration = ideal moment of inertia
        IdealMomentOfInertia = RollerTorque / RollerAcceleration
        'r1 and r2 are the same as when calculated for the roller moment of inertia
        r1 = (RollerDiameter / 2.0 - RollerWallThickness) / 1000.0
        r2 = RollerDiameter / 2.0 / 1000.0
        'So (Note - actually not going to use this)
        IdealRollerMass = IdealMomentOfInertia * 2.0 / (r1 ^ 2 + r2 ^ 2) * 1000
        'For Rollout calculations
        RollerCircumference = RollerDiameter * Math.PI 'circumference in mm
        Main.WheelCircumference = WheelDiameter * Math.PI

        'For Wheel and Motor RPM conversions and speed conversion
        Main.RollerRPMtoWheelRPM = RollerDiameter / WheelDiameter
        Main.RollerRPMtoMotorRPM = RollerDiameter / WheelDiameter * Main.GearRatio
        Main.RollerRadsPerSecToMetersPerSec = (RollerCircumference / 1000) / (2 * Math.PI)

        If Main.DynoMomentOfInertia >= 0.0009 Then
            lblActualMomentOfInertia.Text = resources.GetString("Dyno_ActualMoiPrefix") & Main.DynoMomentOfInertia.ToString("0.000") & " kg.m^2"
        Else
            lblActualMomentOfInertia.Text = resources.GetString("Dyno_ActualMoiPrefix") & (1000 * Main.DynoMomentOfInertia).ToString("0.000") & " g.m^2 "
        End If

        lblTargetMomentOfInertia.Text = resources.GetString("Dyno_TargetMoiPrefix") & (Main.DynoMomentOfInertia / IdealMomentOfInertia * 100).ToString("0.0") & "%"

        lblTargetRollerMass.Text = resources.GetString("Dyno_TargetRollerMassPrefix") & IdealRollerMass.ToString("0") & resources.GetString("Dyno_TargetRollerMassSuffix")

        'Update the drag coefficient and air resistance
        Main.ForceAir = 0.5 * (FrontalArea / 1000000) * 1.2 * DragCoefficient

        'Update conversions for time to RPM
        Main.ElapsedTimeToRadPerSec = 2 * Math.PI / SignalsPerRPM
        Main.ElapsedTimeToRadPerSec2 = 2 * Math.PI / SignalsPerRPM2

    End Sub

End Class