Imports MyBooks.App.ViewModels

Namespace Global.MyBooks.App.Views

    Public NotInheritable Class QueryResultViewer
        Inherits Page

        Public Property ViewModel As LibraryBooksViewModel = AppShell.Current.QueryResult

        Public Sub New()
            InitializeComponent()
            DataContext = ViewModel
        End Sub

        Private Sub BibItem_Click(sender As Object, e As ItemClickEventArgs)
            Dim item As BookBrowserViewModel = DirectCast(e.ClickedItem, BookBrowserViewModel)
            ViewModel.SetCurrentItem(item)
        End Sub

        Private Sub OnCurrentItemOfGridChanged(sender As Object, e As EventArgs)

        End Sub
    End Class

End Namespace
