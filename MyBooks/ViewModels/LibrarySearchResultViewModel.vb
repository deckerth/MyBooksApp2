Imports Microsoft.Toolkit.Uwp.Helpers
Imports MyBooks.App.Commands
Imports MyBooks.App.ValueComparers
Imports MyBooks.Models
Imports MyBooks.Repository.Libraries

Namespace Global.MyBooks.App.ViewModels

    Public Class LibrarySearchResultViewModel
        Inherits BindableBase

        Public Sub New()
            EnableHubDisplay = New RelayCommand(AddressOf SwitchToHubDisplay)
            EnableListDisplay = New RelayCommand(AddressOf SwitchToListDisplay)
            Library = LibraryRegistry.Current.Libraries.Item(0)
            InitalizeSearchCommands()
            InitializePaneCommands()
        End Sub

        Private libraryAccess As ILibraryAccess

        Public ReadOnly Property Libraries As List(Of ILibraryAccess)
            Get
                Return LibraryRegistry.Current.Libraries
            End Get
        End Property

        Private _queryResult As New LibraryBooksViewModel
        Public Property QueryResult As LibraryBooksViewModel
            Set(value As LibraryBooksViewModel)
                SetProperty(Of LibraryBooksViewModel)(_queryResult, value)
            End Set
            Get
                Return _queryResult
            End Get
        End Property

        Private _currentItem As BookBrowserViewModel = New BookBrowserViewModel()
        Public Property CurrentItem As BookBrowserViewModel
            Get
                Return _currentItem
            End Get
            Set(value As BookBrowserViewModel)
                If value IsNot Nothing Then
                    value.InitializeBookLinks()
                End If
                SetProperty(Of BookBrowserViewModel)(_currentItem, value)
            End Set
        End Property

        Public Property Progress As New ProgressRingViewModel()

        Public Sub SetCurrentItem(item As BookBrowserViewModel)
            CurrentItem = item
            CurrentItem.InitializeBookLinks()
            OnPropertyChanged("CurrentItem")
            If CurrentItem.BrowserAdapter.BibItemUriIsValid Then
                CurrentItem.BrowserAdapter.CloneTo(AppShell.Current.ViewModel)
                AppShell.Current.ViewModel.BrowserPaneOpen = True
            End If
        End Sub

#Region "SearchFields"
        Private _searchTitle As String = ""
        Public Property SearchTitle As String
            Get
                Return _searchTitle
            End Get
            Set(value As String)
                SetProperty(Of String)(_searchTitle, value)
            End Set
        End Property


        Private _searchAuthor As String = ""
        Public Property SearchAuthor As String
            Get
                Return _searchAuthor
            End Get
            Set(value As String)
                SetProperty(Of String)(_searchAuthor, value)
            End Set
        End Property

        Private _maxHits As Integer = 50
        Public Property MaxHits As Integer
            Get
                Return _maxHits
            End Get
            Set(value As Integer)
                SetProperty(Of Integer)(_maxHits, value)
            End Set
        End Property

        Private _library As ILibraryAccess
        Public Property Library As ILibraryAccess
            Get
                Return _library
            End Get
            Set(value As ILibraryAccess)
                SetProperty(Of ILibraryAccess)(_library, value)
            End Set
        End Property

#End Region

#Region "DisplayMode"
        Private _displayModeHub As Boolean = False
        Public Property DisplayModeHub As Boolean
            Get
                Return _displayModeHub
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_displayModeHub, value)
            End Set
        End Property

        Public Property EnableHubDisplay As RelayCommand
        Public Property EnableListDisplay As RelayCommand

        Private Async Sub SwitchToHubDisplay()
            If Not _displayModeHub Then
                Await DispatcherHelper.ExecuteOnUIThreadAsync(Sub() DisplayModeHub = True)
            End If
        End Sub

        Private Async Sub SwitchToListDisplay()
            If _displayModeHub Then
                Await DispatcherHelper.ExecuteOnUIThreadAsync(Sub() DisplayModeHub = False)
            End If
        End Sub

#End Region

#Region "BrowserPane"

        Private Sub InitializePaneCommands()
            OpenBrowserPane = New RelayCommand(AddressOf OnOpenBrowserPane)
            CloseBrowserPane = New RelayCommand(AddressOf OnCloseBrowserPane)
        End Sub

        Public Property CloseBrowserPane As RelayCommand
        Public Property OpenBrowserPane As RelayCommand

        Private Sub OnOpenBrowserPane()
            If CurrentItem IsNot Nothing Then
                If CurrentItem.BrowserAdapter.BibItemUriIsValid Then
                    CurrentItem.BrowserAdapter.CloneTo(AppShell.Current.ViewModel)
                    AppShell.Current.ViewModel.BrowserPaneOpen = True
                End If
            End If
        End Sub

        Private Sub OnCloseBrowserPane()
            AppShell.Current.ViewModel.BrowserPaneOpen = False
            AppShell.Current.ViewModel.CloneTo(CurrentItem.BrowserAdapter)
        End Sub

#End Region

#Region "SearchCommands"

        Public Property SearchCommand As RelayCommand

        Public Property PrevPageCommand As RelayCommand
        Public Property NextPageCommand As RelayCommand

        Private Sub InitalizeSearchCommands()
            SearchCommand = New RelayCommand(AddressOf OnSearchAsync)
            NextPageCommand = New RelayCommand(AddressOf OnNextPageAsync)
            PrevPageCommand = New RelayCommand(AddressOf OnPrevPageAsync)
        End Sub

        Private Async Function OnSearchAsync() As Task

            If Library Is Nothing Then
                Return
            End If

            Await Progress.SetIndeterministicAsync()

            libraryAccess = Library

            libraryAccess.SetSearchParameters(author:=SearchAuthor, title:=SearchTitle)

            Await libraryAccess.ExecuteQueryAsync(MaxHits)

            Await QueryResult.SetItemsAsync(Progress, libraryAccess)
            QueryResult.NoOfEntries = libraryAccess.NoOfEntries
            QueryResult.NoOfPages = libraryAccess.NoOfPages
            QueryResult.CurrentPosition = App.Texts.GetString("Page") + " : " + libraryAccess.CurrentPage.ToString + " / " + libraryAccess.NoOfPages.ToString
            QueryResult.HasNextPage = libraryAccess.HasNextPage
            QueryResult.HasPreviousPage = libraryAccess.HasPreviousPage

            Await Progress.HideAsync()
        End Function

        Private Async Function OnPrevPageAsync() As Task

            Await Progress.SetIndeterministicAsync()

            If libraryAccess Is Nothing OrElse Not libraryAccess.HasPreviousPage Then
                Return
            End If

            Await libraryAccess.ReadPreviousPageAsync()

            Await QueryResult.SetItemsAsync(Progress, libraryAccess)
            QueryResult.CurrentPosition = App.Texts.GetString("Page") + " : " + libraryAccess.CurrentPage.ToString + " / " + libraryAccess.NoOfPages.ToString
            QueryResult.HasNextPage = libraryAccess.HasNextPage
            QueryResult.HasPreviousPage = libraryAccess.HasPreviousPage
            Await Progress.HideAsync()

        End Function

        Private Async Function OnNextPageAsync() As Task

            Await Progress.SetIndeterministicAsync()

            If libraryAccess Is Nothing OrElse Not libraryAccess.HasNextPage Then
                Return
            End If

            Await libraryAccess.ReadNextPageAsync()

            Await QueryResult.SetItemsAsync(Progress, libraryAccess)
            QueryResult.CurrentPosition = App.Texts.GetString("Page") + " : " + libraryAccess.CurrentPage.ToString + " / " + libraryAccess.NoOfPages.ToString
            QueryResult.HasNextPage = libraryAccess.HasNextPage
            QueryResult.HasPreviousPage = libraryAccess.HasPreviousPage
            Await Progress.HideAsync()

        End Function

#End Region

    End Class

End Namespace
