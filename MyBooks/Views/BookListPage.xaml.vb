Imports System.Collections.Specialized
Imports System.Globalization
Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Extensions
Imports MyBooks.App.UserControls
Imports MyBooks.App.ViewModels
Imports MyBooks.Models
Imports Telerik.Data.Core
Imports Telerik.UI.Xaml.Controls.Grid
Imports Telerik.UI.Xaml.Controls.Grid.Commands
Imports Telerik.UI.Xaml.Controls.Input
Imports Telerik.UI.Xaml.Controls.Input.AutoCompleteBox

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

        Public Overrides Async Sub Execute(parameter As Object)
            Dim context As EditContext = DirectCast(parameter, EditContext)
            BookListPage.Current.ViewModel.SelectedBook.IsInEdit = False
            Await BookListPage.Current.ViewModel.SelectedBook.Refresh()
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

        Private AuthorTitleSortDescriptors As New List(Of SortDescriptorBase)

        Public Sub New()
            InitializeComponent()
            Current = Me
            NavigationCacheMode = NavigationCacheMode.Enabled
            ViewModel = New BookListPageViewModel()
            DataContext = ViewModel
            ViewModel.SelectedItems = DataGrid.SelectedItems
            AuthorTitleSortDescriptors.Add(DataGrid.SortDescriptors.Item(0)) 'Authors
            AuthorTitleSortDescriptors.Add(DataGrid.SortDescriptors.Item(1)) 'Titles
            AddHandler ViewModel.SelectAll, AddressOf OnSelectAll
            AddHandler ViewModel.DeselectAll, AddressOf OnDeselectAll
            AddHandler ViewModel.ResetSorting, AddressOf OnResetSorting
            AddHandler ViewModel.SortAuthorsTitles, AddressOf OnSortAuthorsTitles
            AddHandler DataGrid.GroupDescriptors.CollectionChanged, AddressOf OnGroupCollectionChanged
        End Sub

        Private Sub OnGroupCollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs)
            Dim grouping As GroupDescriptorCollection = DirectCast(sender, GroupDescriptorCollection)
            For Each column In DataGrid.Columns
                Dim templateColumn As DataGridTemplateColumn = TryCast(column, DataGridTemplateColumn)
                If templateColumn IsNot Nothing Then
                    column.CanUserGroup = Not grouping.Contains(templateColumn.GroupDescriptor)
                End If
            Next
        End Sub

        Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
            MyBase.OnNavigatedTo(e)
        End Sub

        Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
            MyBase.OnNavigatedFrom(e)
        End Sub

#Region "CRUD"
        Private Sub CreateBook_Click(sender As Object, e As RoutedEventArgs)
            Frame.Navigate(GetType(BookDetailPage))
        End Sub

        Private Sub EditBook_Click(sender As Object, e As RoutedEventArgs)
            'Frame.Navigate(GetType(BlankPage1), ViewModel.SelectedBook)
            'Return
            If ViewModel.MultipleSelectionMode Then
                If DataGrid.SelectedItems.Count > 0 Then
                    If DataGrid.SelectedItems.Count = 1 Then
                        Frame.Navigate(GetType(BookDetailPage), DataGrid.SelectedItems.ElementAt(0))
                    Else
                        Dim bookSet As New List(Of BookViewModel)
                        For Each b In ViewModel.SelectedItems
                            bookSet.Add(b)
                        Next
                        Dim bookSetViewModel As New MultipleBooksViewModel(bookSet)
                        Frame.Navigate(GetType(BookDetailPage), bookSetViewModel)
                    End If
                End If
            Else
                If ViewModel.SelectedBook IsNot Nothing AndAlso ViewModel.SelectedBook.Title.Length > 0 Then
                    Frame.Navigate(GetType(BookDetailPage), ViewModel.SelectedBook)
                End If
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

#End Region

#Region "InPlaceEditing"
        Private Async Sub OnAuthors_TextChanged(sender As AdvancedAutoSuggestBox, e As AutoSuggestBoxTextChangedEventArgs)
            ' Only get results when it was a user typing,
            ' otherwise assume the value got filled in by TextMemberPath
            ' Or the handler for SuggestionChosen.

            Dim hits = Await App.Repository.Authors.GetAsync(sender.Text)
            Dim dataset As New Collection(Of String)
            For Each a In hits
                dataset.Add(a.Name)
            Next
            ' Set the ItemsSource to be your filtered dataset
            sender.ItemsSource = dataset

        End Sub

        Private Async Sub OnAuthors_DeleteSuggestion(sender As UserControls.AdvancedAutoSuggestBox, e As UserControls.AdvancedAutoSuggestBoxDeleteSuggestionArgs)
            Await App.Repository.Authors.DeleteAsyncExact(e.SuggestionToDelete)
        End Sub

        Private Async Sub OnStorage_TextChanged(sender As AdvancedAutoSuggestBox, e As AutoSuggestBoxTextChangedEventArgs)

            Dim hits As IEnumerable(Of Storage) = Await App.Repository.Storages.GetAsync(sender.Text)
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

        Private Async Sub OnKeywords_TextChanged(sender As AdvancedAutoSuggestBox, e As AutoSuggestBoxTextChangedEventArgs)

            Dim hits As IEnumerable(Of Keyword) = Await App.Repository.Keywords.GetAsync(sender.Text)
            Dim dataset As New Collection(Of String)
            For Each a In hits
                dataset.Add(a.Name)
            Next
            ' Set the ItemsSource to be your filtered dataset
            sender.ItemsSource = dataset

        End Sub

        Private Async Sub OnKeywords_DeleteSuggestion(sender As UserControls.AdvancedAutoSuggestBox, e As UserControls.AdvancedAutoSuggestBoxDeleteSuggestionArgs)
            Await App.Repository.Storages.DeleteAsyncExact(e.SuggestionToDelete)
        End Sub

        Private _currentAuthorBox As RadAutoCompleteBox
        'Private _internalTextBox As AutoCompleteTextBox

        ' Traverse the child visual tree to find the AutoCompleteTextBox
        ' And SUBSCRIBE the ContextMenuOpening event handler
        Private Sub OnAuthors_Loaded(sender As Object, e As RoutedEventArgs)
            _currentAuthorBox = DirectCast(sender, RadAutoCompleteBox)
            'Dim descendants = _currentAuthorBox.FindDescendants(Of AutoCompleteTextBox)
            '_internalTextBox = descendants.FirstOrDefault()

            'AddHandler _internalTextBox.ContextMenuOpening, AddressOf InternalTextBox_ContextMenuOpening
        End Sub

        'Private Sub OnAuthors_Unloaded(sender As Object, e As RoutedEventArgs)
        '    Dim box = DirectCast(sender, RadAutoCompleteBox)
        '    Dim descendants = box.FindDescendants(Of AutoCompleteTextBox)
        '    Dim internalTextBox As AutoCompleteTextBox = descendants.FirstOrDefault()

        '    RemoveHandler internalTextBox.ContextMenuOpening, AddressOf InternalTextBox_ContextMenuOpening
        '    _currentAuthorBox = Nothing
        '    _internalTextBox = Nothing
        'End Sub

        'Private Sub InternalTextBox_ContextMenuOpening(sender As Object, e As ContextMenuEventArgs)
        '    e.Handled = True
        'End Sub

        Private ReformatAuthorsFlyoutBase As FlyoutBase

        Private Sub ConvertToLastNameFirstName_Click(sender As Object, e As RoutedEventArgs)
            If ViewModel.SelectedBook IsNot Nothing Then
                Dim start = 0
                Dim length = ViewModel.SelectedBook.Authors.Length
                ViewModel.SelectedBook.AuthorNameConversion.SetAuthors(ViewModel.SelectedBook.Authors, start, length)
                ViewModel.SelectedBook.AuthorNameConversion.SetConversionMode(AuthorSuggestionsViewModel.ConversionMode.LastNameFirstName)
                ViewModel.SelectedBook.AuthorNameConversion.ComputeSuggestion()
                ReformatAuthorsFlyoutBase = FlyoutBase.GetAttachedFlyout(sender)
                ReformatAuthorsFlyoutBase.ShowAt(_currentAuthorBox)
            End If
        End Sub

        Private Sub ConvertToFirstNameLastName_Click(sender As Object, e As RoutedEventArgs)
            If ViewModel.SelectedBook IsNot Nothing Then
                ViewModel.SelectedBook.AuthorNameConversion.SetAuthors(ViewModel.SelectedBook.Authors)
                ViewModel.SelectedBook.AuthorNameConversion.SetConversionMode(AuthorSuggestionsViewModel.ConversionMode.FirstNameLastName)
                ViewModel.SelectedBook.AuthorNameConversion.ComputeSuggestion()
                ReformatAuthorsFlyoutBase = FlyoutBase.GetAttachedFlyout(sender)
                ReformatAuthorsFlyoutBase.ShowAt(_currentAuthorBox)
            End If
        End Sub

        Private Sub AcceptSuggestion_Click(sender As Object, e As RoutedEventArgs)
            ReformatAuthorsFlyoutBase.Hide()
        End Sub

        Private Sub RecomputeAuthorConversion_Click(sender As Object, e As RoutedEventArgs)
            Dim start = InputToConvert.SelectionStart
            Dim length = InputToConvert.SelectionLength
            InputToConvert.SelectionStart = 0
            InputToConvert.SelectionLength = 0
            ViewModel.SelectedBook.AuthorNameConversion.SetAuthors(InputToConvert.Text, start, length)
            ViewModel.SelectedBook.AuthorNameConversion.ComputeSuggestion()
        End Sub

#End Region

#Region "Searchbox"
        Private Async Sub BookSearchBox_TextChanged(sender As AutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)

            ' We only want to get results when it was a user typing,
            ' otherwise we assume the value got filled in by TextMemberPath
            ' Or the handler for SuggestionChosen.
            If args.Reason = AutoSuggestionBoxTextChangeReason.UserInput Then
                If String.IsNullOrEmpty(sender.Text) Then
                    'Await DispatcherHelper.ExecuteOnUIThreadAsync(Async Function()
                    '                                                  Await ViewModel.GetBooksListAsync()
                    '                                              End Function)
                    sender.ItemsSource = Nothing
                    Await CLearFilter()
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

        Private Async Function CLearFilter() As Task
            If ViewModel.FilterIsSet Then
                ViewModel.FilterIsSet = False
                If ViewModel.IsBackupValid() Then
                    Await DispatcherHelper.ExecuteOnUIThreadAsync(Sub() ViewModel.RestoreBackup())
                Else
                    Await ViewModel.Progress.SetIndeterministicAsync()
                    Await DispatcherHelper.ExecuteOnUIThreadAsync(Async Function()
                                                                      Await ViewModel.GetBooksListAsync()
                                                                  End Function)
                    Await ViewModel.Progress.HideAsync()
                End If
            End If
        End Function

        Private Async Sub BookSearchBox_QuerySubmitted(sender As AutoSuggestBox, args As AutoSuggestBoxQuerySubmittedEventArgs)

            If String.IsNullOrEmpty(args.QueryText) Then
                Await CLearFilter()
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
                                                                  ViewModel.CreateBackup()
                                                                  ViewModel.Books.Clear()
                                                                  For Each match In matches
                                                                      ViewModel.Books.Add(match)
                                                                  Next
                                                                  ViewModel.FilterIsSet = True
                                                                  DataGrid.SelectedItems.Clear()
                                                              End Sub)

            End If
        End Sub

#End Region

#Region "Selection"
        Private Sub OnSelectAll()
            DataGrid.SelectAll()
        End Sub

        Private Sub OnDeselectAll()
            DataGrid.DeselectAll()
        End Sub

        Private Sub DataGrid_SelectionChanged(sender As Object, e As DataGridSelectionChangedEventArgs)
            ViewModel.DataGrid_SelectionChanged()
            ViewModel.Statistics.SetSelectedBooks(DirectCast(sender, RadDataGrid).SelectedItems.Count)
        End Sub

#End Region

#Region "Sorting"
        Private Sub OnResetSorting()
            DataGrid.SortDescriptors.Clear()
        End Sub

        Private Sub OnSortAuthorsTitles()
            OnResetSorting()
            DataGrid.SortDescriptors.Add(AuthorTitleSortDescriptors.Item(0)) 'Authors
            DataGrid.SortDescriptors.Add(AuthorTitleSortDescriptors.Item(1)) 'Titles
        End Sub
#End Region

#Region "GridCommands"
        Private Sub DataGrid_KeyDown(sender As Object, e As KeyRoutedEventArgs)
            Select Case e.Key
                Case Windows.System.VirtualKey.Home
                    DataGrid.ScrollIndexIntoView(0)
                    e.Handled = True
                Case Windows.System.VirtualKey.End
                    DataGrid.ScrollIndexIntoView(ViewModel.Books.Count - 1)
                    e.Handled = True
            End Select
        End Sub

#End Region

    End Class

End Namespace
