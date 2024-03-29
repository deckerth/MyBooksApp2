﻿Imports MyBooks.App.ViewModels
Imports Telerik.UI.Xaml.Controls.Input
Imports Microsoft.Toolkit.Uwp.UI.Extensions
Imports Telerik.UI.Xaml.Controls.Input.AutoCompleteBox

Namespace Global.MyBooks.App.Views

    Public NotInheritable Class BookDetailPage
        Inherits Page

        Public Property ViewModel As BookDetailPageViewModel

        Public Sub New()
            InitializeComponent()
            DataContext = ViewModel
        End Sub

        ' <summary>
        ' Displays the selected book data.
        ' </summary>
        Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
            If e.Parameter IsNot Nothing Then
                If TypeOf e.Parameter Is MultipleBooksViewModel Then
                    ViewModel = New BookDetailPageViewModel(e.Parameter)
                Else
                    Dim book As BookViewModel = DirectCast(e.Parameter, BookViewModel)
                    If ViewModel Is Nothing OrElse Not ViewModel.Book.Equals(book) Then
                        ViewModel = New BookDetailPageViewModel With {
                    .Book = book
                }
                        If book.Id.Equals(Models.Book.NewBookId) Then
                            ViewModel.IsNewBook = True
                            ViewModel.Book.Model.Id = Guid.NewGuid()
                            PageHeaderText.Text = App.Texts.GetString("NewBook")
                        End If
                        ViewModel.Book.Validate = False
                        Bindings.Update()
                    End If
                End If
            Else
                ViewModel = New BookDetailPageViewModel With {
                    .IsNewBook = True,
                    .Book = New BookViewModel(New Models.Book()) With {.Validate = True}
                }
                Bindings.Update()
                PageHeaderText.Text = App.Texts.GetString("NewBook")
            End If

            ViewModel.IsInEdit = True

            MyBase.OnNavigatedTo(e)
        End Sub

        Private Sub CommandBar_Loaded(sender As Object, e As RoutedEventArgs)

        End Sub

        Private Async Function SaveChangesDialog() As Task(Of ContentDialogResult)
            Dim promtDialog As New ContentDialog With {
            .Title = "",
            .Content = App.Texts.GetString("DoYouWantToSave"),
            .CloseButtonText = App.Texts.GetString("No"),
            .PrimaryButtonText = App.Texts.GetString("Yes")
            }
            Return Await promtDialog.ShowAsync()
        End Function

        Private Sub Save_Click(sender As Object, e As RoutedEventArgs)

            If Frame.CanGoBack Then
                Frame.GoBack()
            End If

        End Sub

        Private Async Sub CancelEditButton_Click(sender As Object, e As RoutedEventArgs)

            If ViewModel.Book.IsModified Then
                Dim result As ContentDialogResult = Await SaveChangesDialog()
                If result = ContentDialogResult.Primary Then
                    ViewModel.SaveCommand.Execute(Nothing)
                Else
                    Await ViewModel.Book.Refresh()
                End If
            End If

            If Frame.CanGoBack Then
                Frame.GoBack()
            End If

        End Sub

        Private Async Sub OnAuthors_TextChanged(sender As UserControls.AdvancedAutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)

            ' Only get results when it was a user typing,
            ' otherwise assume the value got filled in by TextMemberPath
            ' Or the handler for SuggestionChosen.
            If sender.Text IsNot Nothing AndAlso sender.Text.Length > 1 Then
                Dim hits = Await App.Repository.Authors.GetAsync(sender.Text)
                Dim dataset As New Collection(Of String)
                For Each a In hits
                    dataset.Add(a.Name)
                Next
                ' Set the ItemsSource to be your filtered dataset
                sender.ItemsSource = dataset
            End If

        End Sub

        Private Async Sub OnAuthors_DeleteSuggestion(sender As UserControls.AdvancedAutoSuggestBox, e As UserControls.AdvancedAutoSuggestBoxDeleteSuggestionArgs)
            Await App.Repository.Authors.DeleteAsyncExact(e.SuggestionToDelete)
        End Sub

        Private Async Sub OnKeywords_TextChanged(sender As UserControls.AdvancedAutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)

            ' Only get results when it was a user typing,
            ' otherwise assume the value got filled in by TextMemberPath
            ' Or the handler for SuggestionChosen.
            Dim hits = Await App.Repository.Keywords.GetAsync(sender.Text)
            Dim dataset As New Collection(Of String)
            For Each a In hits
                dataset.Add(a.Name)
            Next
            ' Set the ItemsSource to be your filtered dataset
            sender.ItemsSource = dataset
        End Sub

        Private Async Sub OnKeywords_DeleteSuggestion(sender As UserControls.AdvancedAutoSuggestBox, e As UserControls.AdvancedAutoSuggestBoxDeleteSuggestionArgs)
            Await App.Repository.Keywords.DeleteAsyncExact(e.SuggestionToDelete)
        End Sub

        Private Async Sub OnStorage_TextChanged(sender As UserControls.AdvancedAutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)

            ' Only get results when it was a user typing,
            ' otherwise assume the value got filled in by TextMemberPath
            ' Or the handler for SuggestionChosen.
            Dim hits = Await App.Repository.Storages.GetAsync(sender.Text)
            Dim dataset As New Collection(Of String)
            For Each a In hits
                dataset.Add(a.Name)
            Next
            ' Set the ItemsSource to be your filtered dataset
            sender.ItemsSource = dataset
        End Sub

        Private Async Sub OnStorage_DeleteSuggestion(sender As UserControls.AdvancedAutoSuggestBox, e As UserControls.AdvancedAutoSuggestBoxDeleteSuggestionArgs)
            Await App.Repository.Storages.DeleteAsyncExact(e.SuggestionToDelete)
        End Sub

#Region "FormatAuthors"
        Private ReformatAuthorsFlyoutBase As FlyoutBase

        Private Sub ReformatAuthorsLastNameFirstName_Click(sender As Object, e As RoutedEventArgs)
            ViewModel.Book.AuthorNameConversion.SetAuthors(ViewModel.Book.Authors, 0, 0)
            ViewModel.Book.AuthorNameConversion.SetConversionMode(AuthorSuggestionsViewModel.ConversionMode.LastNameFirstName)
            ViewModel.Book.AuthorNameConversion.ComputeSuggestion()
            ReformatAuthorsFlyoutBase = FlyoutBase.GetAttachedFlyout(ReformatAuthorsLastNameFirstName)
            ReformatAuthorsFlyoutBase.ShowAt(ReformatAuthorsLastNameFirstName)
        End Sub

        Private Sub ReformatAuthorsFirstNameLastName_Click(sender As Object, e As RoutedEventArgs)
            ViewModel.Book.AuthorNameConversion.SetAuthors(ViewModel.Book.Authors, 0, 0)
            ViewModel.Book.AuthorNameConversion.SetConversionMode(AuthorSuggestionsViewModel.ConversionMode.FirstNameLastName)
            ViewModel.Book.AuthorNameConversion.ComputeSuggestion()
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
            ViewModel.Book.AuthorNameConversion.SetAuthors(InputToConvert.Text, start, length)
            ViewModel.Book.AuthorNameConversion.ComputeSuggestion()
        End Sub

#End Region
    End Class

End Namespace
