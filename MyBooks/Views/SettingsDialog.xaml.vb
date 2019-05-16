Imports Microsoft.Toolkit.Uwp.Helpers
Imports MyBooks.App.ViewModels

Namespace Global.MyBooks.App.Views

    Public NotInheritable Class SettingsDialog
        Inherits ContentDialog

        Public Property ViewModel As BookListPageViewModel

        Public Sub New()
            InitializeComponent()

            ViewModel = BookListPageViewModel.Current
            DataContext = ViewModel

            Select Case App.SelectedApplicationTheme
                Case App.ApplicationThemeDark
                    ThemeComboBox.SelectedItem = ThemeDark
                Case App.ApplicationThemeLight
                    ThemeComboBox.SelectedItem = ThemeLight
            End Select

            If App.SelectedNameFormat = App.FirstNameLastNameFormat Then
                NameFormatComboBox.SelectedItem = FirstNameLastName
            Else
                NameFormatComboBox.SelectedItem = LastNameFirstName
            End If
        End Sub

        Private Sub SaveNameFormat()
            Dim settings = Windows.Storage.ApplicationData.Current.LocalSettings
            Select Case NameFormatComboBox.SelectedIndex
                Case 0 ' FirstNameLastName
                    settings.Values("NameFormat") = App.FirstNameLastNameFormat
                    App.SelectedNameFormat = App.FirstNameLastNameFormat
                Case 1
                    settings.Values("NameFormat") = App.LastNameFirstNameFormat
                    App.SelectedNameFormat = App.LastNameFirstNameFormat
            End Select
        End Sub

        Private Async Sub SettingsDialog_PrimaryButtonClick(sender As ContentDialog, args As ContentDialogButtonClickEventArgs)

            Dim settings = Windows.Storage.ApplicationData.Current.LocalSettings
            Dim themeChanged As Boolean = False

            SaveNameFormat()

            Select Case ThemeComboBox.SelectedIndex
                Case 0 'App.ApplicationThemeLight
                    If App.SelectedApplicationTheme = App.ApplicationThemeDark Then
                        settings.Values("ApplicationTheme") = App.ApplicationThemeLight
                        themeChanged = True
                    End If
                Case 1 'App.ApplicationThemeDark
                    If App.SelectedApplicationTheme = App.ApplicationThemeLight Then
                        settings.Values("ApplicationTheme") = App.ApplicationThemeDark
                        themeChanged = True
                    End If
            End Select

            If themeChanged Then
                Dim msg = New Windows.UI.Popups.MessageDialog(App.Texts.GetString("RestartAppForThemeChangeRequired"))
                Await msg.ShowAsync()
            End If

            Hide()

        End Sub

        Private Async Sub ComputeIndex_Click(sender As Object, e As RoutedEventArgs) Handles ComputeIndex.Click
            SaveNameFormat()
            ' Enforce the execution on another thread than the UI thread
            Await Task.Run(Sub() ViewModel.UpdateIndexAsync())
        End Sub

        Private Async Sub ImportDB_Click(sender As Object, e As RoutedEventArgs) Handles ImportDB.Click
            Hide()
            Await ViewModel.OnImportDB()
        End Sub

        Private Async Sub ExportDB_Click(sender As Object, e As RoutedEventArgs) Handles ExportDB.Click
            Hide()
            Await ViewModel.OnExportDB()
        End Sub
    End Class

End Namespace
