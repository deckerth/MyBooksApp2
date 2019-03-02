Imports MyBooks.App.ViewModels

Namespace Global.MyBooks.App.Views

    Public NotInheritable Class BarcodeImportPage
        Inherits Page

        Public Property ViewModel As ScannedBooksImportViewModel = New ScannedBooksImportViewModel()

        Public ReadOnly Property BarcodeImportIcon As String
            Get
                Return "F0 " &
                       "M10,0 h2 v19 h-2 v-19 " &
                       "M14,0 h1 v16 h-1 v-16 " &
                       "M17,0 h1 v16 h-1 v-16 " &
                       "M21,0 h1 v16 h-1 v-16 " &
                       "M24,0 h1 v16 h-1 v-16 " &
                       "M27,0 h2 v19 h-2 v-19 " &
                       "M5,7 h16 v2 h-16 v-2 " &
                       "M 4,7 l2,-2 l-1,-1 l-4,4 l4,4 l1,-1 l-2,-2"
            End Get
        End Property

        Public Sub New()
            InitializeComponent()
            DataContext = ViewModel
            QueryResultFrame.Navigate(GetType(QueryResultViewer))
        End Sub

        Public Function QueryResultFrame() As Frame
            Return contentFrame
        End Function

        Private Sub OnNavigatingToPage(sender As Object, e As NavigatingCancelEventArgs)

        End Sub

        Private Sub CurrentStateChanged(sender As Object, e As VisualStateChangedEventArgs)
            Dim x = 0
        End Sub
    End Class

End Namespace

