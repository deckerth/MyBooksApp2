Imports MyBooks.Repository

Namespace Global.MyBooks.App.Views

    Public NotInheritable Class ImportResultDialog
        Inherits ContentDialog

        Public Property ViewModel As UpdateCounters

        Public Sub New(result As UpdateCounters)
            InitializeComponent()
            ViewModel = result
            DataContext = ViewModel

            Dim msg As String
            If result.Sum = 1 Then
                msg = App.Texts.GetString("ImportResultSummarySingular")
            Else
                msg = App.Texts.GetString("ImportResultSummary")
                msg = msg.Replace("&", result.Sum.ToString)
            End If
            Summary.Text = msg
        End Sub

    End Class

End Namespace

