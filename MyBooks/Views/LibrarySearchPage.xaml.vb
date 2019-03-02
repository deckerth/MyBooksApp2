Imports MyBooks.App.ViewModels
Imports MyBooks.Models
Imports Telerik.UI.Xaml.Controls.Input

Namespace Global.MyBooks.App.Views

    Public NotInheritable Class LibrarySearchPage
        Inherits Page

        Public Property ViewModel As LibrarySearchResultViewModel = New LibrarySearchResultViewModel()

        Public Sub New()
            InitializeComponent()
            DataContext = ViewModel
            NavigationCacheMode = NavigationCacheMode.Enabled
        End Sub

        Private Sub BibItem_Click(sender As Object, e As ItemClickEventArgs)
            Dim item As BookBrowserViewModel = DirectCast(e.ClickedItem, BookBrowserViewModel)
            ViewModel.SetCurrentItem(item)
        End Sub

        Private Async Sub OnAuthorSearchTextBox_TextChanged(sender As AutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)
            ' Only get results when it was a user typing,
            ' otherwise assume the value got filled in by TextMemberPath
            ' Or the handler for SuggestionChosen.
            Dim hits = Await App.Repository.Authors.GetAsync(sender.Text)
            Dim dataset As New List(Of String)
            For Each a In hits
                dataset.Add(a.Name)
            Next
            ' Set the ItemsSource to be your filtered dataset
            sender.ItemsSource = dataset
        End Sub

        Private Sub OnCurrentItemOfGridChanged(sender As Object, e As EventArgs)
        End Sub

        Private Sub VisualStates_CurrentStateChanged(sender As Object, e As VisualStateChangedEventArgs)
            Dim x = 0

        End Sub
    End Class

End Namespace
