﻿Imports MyBooks.App.ViewModels
Imports MyBooks.Models
Imports Telerik.UI.Xaml.Controls.Input

Namespace Global.MyBooks.App.Views

    Public NotInheritable Class AddBookFromLibraryDialog
        Inherits ContentDialog

        Public Property ViewModel As BookDetailPageViewModel

        Public Sub New(newBook As Book)

            InitializeComponent()

            ViewModel = New BookDetailPageViewModel() With {.IsNewBook = True, .Book = New BookViewModel(newBook)}
            DataContext = ViewModel
        End Sub

        Private Sub AddBookButton_Click(sender As ContentDialog, args As ContentDialogButtonClickEventArgs)
            ViewModel.SaveCommand.Execute(Nothing)
        End Sub

        Private Sub CancelButton_Click(sender As ContentDialog, args As ContentDialogButtonClickEventArgs)
        End Sub

        Private Async Sub OnAuthors_TextChanged(sender As RadAutoCompleteBox, args As TextChangedEventArgs)

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

        Private Async Sub OnStorage_TextChanged(sender As RadAutoCompleteBox, args As TextChangedEventArgs)

            ' Only get results when it was a user typing,
            ' otherwise assume the value got filled in by TextMemberPath
            ' Or the handler for SuggestionChosen.
            Dim hits = Await App.Repository.Storages.GetAsync(sender.Text)
            Dim dataset As New List(Of String)
            For Each a In hits
                dataset.Add(a.Name)
            Next
            ' Set the ItemsSource to be your filtered dataset
            sender.ItemsSource = dataset

        End Sub

        Private Async Sub OnKeywords_TextChanged(sender As RadAutoCompleteBox, args As TextChangedEventArgs)

            ' Only get results when it was a user typing,
            ' otherwise assume the value got filled in by TextMemberPath
            ' Or the handler for SuggestionChosen.
            Dim hits = Await App.Repository.Keywords.GetAsync(sender.Text)
            Dim dataset As New List(Of String)
            For Each a In hits
                dataset.Add(a.Name)
            Next
            ' Set the ItemsSource to be your filtered dataset
            sender.ItemsSource = dataset

        End Sub

        Private Sub Published_TextChanged(sender As Object, e As TextChangedEventArgs)
            IsPrimaryButtonEnabled = String.IsNullOrWhiteSpace(ViewModel.ErrorText)
        End Sub

#Region "FormatAuthors"
        Private ReformatAuthorsFlyoutBase As FlyoutBase

        Private Sub ReformatAuthorsLastNameFirstName_Click(sender As Object, e As RoutedEventArgs)
            ViewModel.AuthorNameConversion.SetAuthors(ViewModel.Book.Authors)
            ViewModel.AuthorNameConversion.SetConversionMode(AuthorSuggestionsViewModel.ConversionMode.LastNameFirstName)
            ViewModel.AuthorNameConversion.ComputeSuggestion()
            ReformatAuthorsFlyoutBase = FlyoutBase.GetAttachedFlyout(ReformatAuthorsLastNameFirstName)
            ReformatAuthorsFlyoutBase.ShowAt(ReformatAuthorsLastNameFirstName)
        End Sub

        Private Sub ReformatAuthorsFirstNameLastName_Click(sender As Object, e As RoutedEventArgs)
            ViewModel.AuthorNameConversion.SetAuthors(ViewModel.Book.Authors)
            ViewModel.AuthorNameConversion.SetConversionMode(AuthorSuggestionsViewModel.ConversionMode.FirstNameLastName)
            ViewModel.AuthorNameConversion.ComputeSuggestion()
            ReformatAuthorsFlyoutBase = FlyoutBase.GetAttachedFlyout(ReformatAuthorsFirstNameLastName)
            ReformatAuthorsFlyoutBase.ShowAt(ReformatAuthorsFirstNameLastName)
        End Sub

        Private Sub AcceptSuggestion_Click(sender As Object, e As RoutedEventArgs)
            ReformatAuthorsFlyoutBase.Hide()
        End Sub

        Private Sub RecomputeAuthorConversion_Click(sender As Object, e As RoutedEventArgs)
            Dim start = InputToConvert.SelectionStart
            Dim length = InputToConvert.SelectionLength
            InputToConvert.SelectionStart = 0
            InputToConvert.SelectionLength = 0
            ViewModel.AuthorNameConversion.SetAuthors(InputToConvert.Text, start, length)
            ViewModel.AuthorNameConversion.ComputeSuggestion()
        End Sub
#End Region
    End Class

End Namespace