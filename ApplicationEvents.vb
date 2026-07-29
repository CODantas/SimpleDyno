Namespace My

    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup
            'Only CurrentUICulture changes (resource/string lookup). CurrentCulture stays
            'Invariant/en-US so number parsing/formatting and the .sdp/.sdr file format
            '(decimal point) are not affected by the selected UI language.
            Try
                System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(My.Settings.Idioma)
            Catch ex As Exception
                System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("pt-BR")
            End Try

            'Restore the user's last-picked Dark/Light theme before any themed form/widget is
            'created (Main's constructor calls ApplyDashboardTheme() immediately after this).
            ThemeManager.LoadSavedTheme()
        End Sub

    End Class

End Namespace
