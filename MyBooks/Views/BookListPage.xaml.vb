﻿Imports System.Globalization
Imports Microsoft.Toolkit.Uwp.Helpers
Imports MyBooks.App.TelerikStrings
Imports MyBooks.App.ViewModels
Imports MyBooks.Models
Imports Telerik.Data.Core
Imports Telerik.UI.Xaml.Controls.Grid
Imports Telerik.UI.Xaml.Controls.Grid.Commands
Imports Telerik.UI.Xaml.Controls.Input

Namespace Global.MyBooks.App.Views

    Public Class CustomCommitEditCommand
        Inherits DataGridCommand

        Public Sub New()
            Id = CommandId.CommitEdit
        End Sub

        Public Overrides Function CanExecute(parameter As Object) As Boolean
            Return True
        End Function

        Public Overrides Sub Execute(parameter As Object)
            Dim context As EditContext = DirectCast(parameter, EditContext)
            BookListPage.Current.ViewModel.SelectedBook.IsInEdit = False
            Owner.CommandService.ExecuteDefaultCommand(CommandId.CommitEdit, context)
            BookListPage.Current.ViewModel.SyncCommand.Execute(Nothing)
        End Sub
    End Class

    Public Class CustomCancelEditCommand
        Inherits DataGridCommand

        Public Sub New()
            Id = CommandId.CancelEdit
        End Sub

        Public Overrides Function CanExecute(parameter As Object) As Boolean
            Return True
        End Function

        Public Overrides Sub Execute(parameter As Object)
            Dim context As EditContext = DirectCast(parameter, EditContext)
            BookListPage.Current.ViewModel.SelectedBook.IsInEdit = False
            Owner.CommandService.ExecuteDefaultCommand(CommandId.CancelEdit, context)
        End Sub
    End Class

    Public Class CustomBeginEditCommand
        Inherits DataGridCommand

        Public Sub New()
            Id = CommandId.BeginEdit
        End Sub

        Public Overrides Function CanExecute(parameter As Object) As Boolean
            Return True
        End Function

        Public Overrides Sub Execute(parameter As Object)
            Dim context As EditContext = DirectCast(parameter, EditContext)
            BookListPage.Current.ViewModel.SelectedBook.IsInEdit = True
            Owner.CommandService.ExecuteDefaultCommand(CommandId.BeginEdit, context)
        End Sub
    End Class

    Public Class AuthorsDataTemplateSelector
        Inherits DataTemplateSelector

        Public Property AuthorsEditTemplate As DataTemplate
        Public Property AuthorsDisplayTemplate As DataTemplate

        Protected Overrides Function SelectTemplateCore(item As Object, container As DependencyObject) As DataTemplate
            Dim row = DirectCast(item, BookViewModel)
            If row.IsInEdit Then
                Return AuthorsEditTemplate
            Else
                Return AuthorsDisplayTemplate
            End If
        End Function
    End Class

    Public Class StorageDataTemplateSelector
        Inherits DataTemplateSelector

        Public Property EditTemplate As DataTemplate
        Public Property DisplayTemplate As DataTemplate

        Protected Overrides Function SelectTemplateCore(item As Object, container As DependencyObject) As DataTemplate
            Dim row = DirectCast(item, BookViewModel)
            If row.IsInEdit Then
                Return EditTemplate
            Else
                Return DisplayTemplate
            End If
        End Function
    End Class

    Public Class KeywordsDataTemplateSelector
        Inherits DataTemplateSelector

        Public Property EditTemplate As DataTemplate
        Public Property DisplayTemplate As DataTemplate

        Protected Overrides Function SelectTemplateCore(item As Object, container As DependencyObject) As DataTemplate
            Dim row = DirectCast(item, BookViewModel)
            If row.IsInEdit Then
                Return EditTemplate
            Else
                Return DisplayTemplate
            End If
        End Function
    End Class

    Public NotInheritable Class BookListPage
        Inherits Page

        Public Shared Current As BookListPage

        Public Property ViewModel As BookListPageViewModel

        Public Sub New()
            InitializeComponent()
            Current = Me
            DataContext = ViewModel
            NavigationCacheMode = NavigationCacheMode.Enabled
            ViewModel = New BookListPageViewModel()
        End Sub

        Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
            MyBase.OnNavigatedTo(e)
        End Sub

        Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
            MyBase.OnNavigatedFrom(e)
        End Sub

        Private Sub CreateBook_Click(sender As Object, e As RoutedEventArgs)
            Frame.Navigate(GetType(BookDetailPage))
        End Sub

        Private Sub EditBook_Click(sender As Object, e As RoutedEventArgs)

            If ViewModel.SelectedBook IsNot Nothing AndAlso ViewModel.SelectedBook.Title.Length > 0 Then
                Frame.Navigate(GetType(BookDetailPage), ViewModel.SelectedBook)
            End If
        End Sub

        Private Sub DuplicateBook_Click(sender As Object, e As RoutedEventArgs)

            If ViewModel.SelectedBook IsNot Nothing AndAlso ViewModel.SelectedBook.Title.Length > 0 Then
                Dim duplicate = ViewModel.SelectedBook.Model.Clone()
                duplicate.Id = Book.NewBookId
                Dim duplicateViewModel = New BookViewModel(duplicate)
                Frame.Navigate(GetType(BookDetailPage), duplicateViewModel)
            End If

        End Sub

        Private Sub BookSearchBox_Loaded(sender As Object, e As RoutedEventArgs)

        End Sub

        Private Sub CommandBar_Loaded(sender As Object, e As RoutedEventArgs)

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

        Private Async Sub OnStorage_TextChanged(sender As RadAutoCompleteBox, e As TextChangedEventArgs)

            Dim hits As IEnumerable(Of Storage) = Await App.Repository.Storages.GetAsync(sender.Text)
            Dim dataset As New List(Of String)
            For Each a In hits
                dataset.Add(a.Name)
            Next
            ' Set the ItemsSource to be your filtered dataset
            sender.ItemsSource = dataset

        End Sub

        Private Async Sub OnKeywords_TextChanged(sender As RadAutoCompleteBox, e As TextChangedEventArgs)

            Dim hits As IEnumerable(Of Keyword) = Await App.Repository.Keywords.GetAsync(sender.Text)
            Dim dataset As New List(Of String)
            For Each a In hits
                dataset.Add(a.Name)
            Next
            ' Set the ItemsSource to be your filtered dataset
            sender.ItemsSource = dataset

        End Sub

        Private Async Sub BookSearchBox_TextChanged(sender As AutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)

            ' We only want to get results when it was a user typing,
            ' otherwise we assume the value got filled in by TextMemberPath
            ' Or the handler for SuggestionChosen.
            If args.Reason = AutoSuggestionBoxTextChangeReason.UserInput Then
                If String.IsNullOrEmpty(sender.Text) Then
                    Await DispatcherHelper.ExecuteOnUIThreadAsync(Async Function()
                                                                      Await ViewModel.GetBooksListAsync()
                                                                  End Function)
                    sender.ItemsSource = Nothing
                Else
                    Dim parameters As String() = sender.Text.Split({" ", ",", ":", ";"}, StringSplitOptions.RemoveEmptyEntries)
                    sender.ItemsSource = ViewModel.Books.Where(
                        Function(x As BookViewModel) parameters.Any(Function(y As String) x.Title.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Authors.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Keywords.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.OriginalTitle.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Storage.StartsWith(y, StringComparison.OrdinalIgnoreCase))
                        ).OrderByDescending(
                        Function(x As BookViewModel) parameters.Count(Function(y As String) x.Title.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Authors.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Keywords.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.OriginalTitle.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Storage.StartsWith(y, StringComparison.OrdinalIgnoreCase))
                        ).Select(Of String)(
                        Function(x As BookViewModel) As String
                            Return x.Authors + ":" + x.Title
                        End Function)
                End If
            End If

        End Sub

        Private Async Sub BookSearchBox_QuerySubmitted(sender As AutoSuggestBox, args As AutoSuggestBoxQuerySubmittedEventArgs)

            If String.IsNullOrEmpty(args.QueryText) Then
                Await DispatcherHelper.ExecuteOnUIThreadAsync(Async Function()
                                                                  Await ViewModel.GetBooksListAsync()
                                                              End Function)
            Else
                Dim parameters As String() = args.QueryText.Split({" ", ",", ":", ";"}, StringSplitOptions.RemoveEmptyEntries)
                Dim matches = ViewModel.Books.Where(
                        Function(x As BookViewModel) parameters.Any(Function(y As String) x.Title.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Authors.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Keywords.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.OriginalTitle.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Storage.StartsWith(y, StringComparison.OrdinalIgnoreCase))
                        ).OrderByDescending(
                        Function(x As BookViewModel) parameters.Count(Function(y As String) x.Title.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Authors.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Keywords.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.OriginalTitle.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Storage.StartsWith(y, StringComparison.OrdinalIgnoreCase))
                        ).ToList()

                Await DispatcherHelper.ExecuteOnUIThreadAsync(Sub()
                                                                  ViewModel.Books.Clear()
                                                                  For Each match In matches
                                                                      ViewModel.Books.Add(match)
                                                                  Next
                                                              End Sub)

            End If
        End Sub

        Private Sub OnAuthors_ContextRequested(sender As UIElement, args As ContextRequestedEventArgs)
            'Dim autoCompleteBox = DirectCast(sender, RadAutoCompleteBox)
            'Dim menu = New MenuFlyout()
            'menu.Items.Add(New MenuFlyoutItem() With {.Text = "AB -> A,B"})
            'autoCompleteBox.ContextFlyout = menu
            'args.Handled = True
        End Sub
    End Class

End Namespace