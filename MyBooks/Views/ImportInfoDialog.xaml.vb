
Imports Windows.System

Namespace Global.MyBooks.App.Views

    Public NotInheritable Class ImportInfoDialog
        Inherits ContentDialog

        Private cancelled As Boolean = False

        Public Sub New(textid As String)
            InitializeComponent()
            InfoBox.NavigateToString(App.Texts.GetString(textid))
        End Sub

        Public Async Function EnableKindleNameInputBox(settingsName As String, prefixName As String) As Task
            Dim settings = Windows.Storage.ApplicationData.Current.LocalSettings
            Dim Name = settings.Values(settingsName)

            If String.IsNullOrEmpty(Name) Then
                Name = ""
                Try
                    Dim users As IReadOnlyList(Of User) = Await User.FindAllAsync()
                    Dim current = users.Where(Function(p) p.AuthenticationStatus = UserAuthenticationStatus.LocallyAuthenticated And p.Type = UserType.LocalUser).FirstOrDefault()
                    If current IsNot Nothing Then
                        Dim userName = Await current.GetPropertyAsync(KnownUserProperties.FirstName)
                        If String.IsNullOrEmpty(userName) Then
                            userName = Await current.GetPropertyAsync(KnownUserProperties.AccountName)
                        End If
                        If Not String.IsNullOrEmpty(userName) Then
                            Name = App.Texts.GetString(prefixName) + " " + userName
                        End If
                    End If
                Catch ex As Exception
                End Try
            End If

            KindleName.Text = Name
            KindleName.Visibility = Visibility.Visible
        End Function

        Function GetLibraryName() As String
            Return KindleName.Text
        End Function

        Function DialogCancelled() As Boolean
            Return cancelled
        End Function

        Private Sub ContentDialog_PrimaryButtonClick(sender As ContentDialog, args As ContentDialogButtonClickEventArgs)
            If KindleName.Visibility = Visibility.Visible AndAlso KindleName.Text.Length > 0 Then
                Dim settings = Windows.Storage.ApplicationData.Current.LocalSettings
                settings.Values("KindleName") = KindleName.Text
            End If
        End Sub

        Private Sub ContentDialog_SecondaryButtonClick(sender As ContentDialog, args As ContentDialogButtonClickEventArgs)
            cancelled = True
        End Sub
    End Class

End Namespace
