Imports Microsoft.Toolkit.Uwp.Helpers
Imports MyBooks.App.Commands
Imports MyBooks.Repository.Libraries

Namespace Global.MyBooks.App.ViewModels

    Public Class ScannedBooksImportViewModel
        Inherits BindableBase

        Public Sub New()
            InitializeDisplayModeCommands()
            InitalizeSearchCommands()
            InitializePaneCommands()
            InitalizeNavCommands()
            InitializeImportCommands()
        End Sub

        Public Property QueryResult As LibraryBooksViewModel = AppShell.Current.QueryResult

#Region "BookNavigation"

        Private currentBook As Integer = 0
        Private scannedBooks As New ScannedBooks()

        Public Property PrevBookCommand As RelayCommand
        Public Property NextBookCommand As RelayCommand
        Public ReadOnly Property HasPrevBook As Boolean
            Get
                Return currentBook > 0
            End Get
        End Property
        Public ReadOnly Property HasNextBook As Boolean
            Get
                Return currentBook < scannedBooks.ISBNCodes.Count - 1
            End Get
        End Property

        Private _currentBookPosition As String
        Public Property CurrentBookPosition As String
            Get
                Return _currentBookPosition
            End Get
            Set(value As String)
                SetProperty(Of String)(_currentBookPosition, value)
            End Set
        End Property

        Private Sub SetCurrentBookPosition(pos As Integer)
            currentBook = pos
            If scannedBooks.ISBNCodes.Count = 0 Then
                CurrentBookPosition = ""
            Else
                CurrentBookPosition = (currentBook + 1).ToString + "/" + scannedBooks.ISBNCodes.Count.ToString
            End If
        End Sub

        Private Sub InitalizeNavCommands()
            NextBookCommand = New RelayCommand(AddressOf OnNextBookAsync)
            PrevBookCommand = New RelayCommand(AddressOf OnPrevBookAsync)
        End Sub

        Private Async Function OnPrevBookAsync() As Task

            If currentBook = 0 Then
                Return 
            End If

            SetCurrentBookPosition(currentBook - 1)

            SearchISBN = scannedBooks.ISBNCodes(currentBook)

            Await OnSearchAsync()

        End Function

        Private Async Function OnNextBookAsync() As Task

            If currentBook = scannedBooks.ISBNCodes.Count - 1 Then
                Return
            End If

            SetCurrentBookPosition(currentBook + 1)

            SearchISBN = scannedBooks.ISBNCodes(currentBook)

            Await OnSearchAsync()

        End Function

#End Region

#Region "SearchFields"
        Private _searchISBN As String = ""
        Public Property SearchISBN As String
            Get
                Return _searchISBN
            End Get
            Set(value As String)
                SetProperty(Of String)(_searchISBN, value)
            End Set
        End Property


        Private _searchIsInProgress As Boolean
        Public Property SearchIsInProgress As Boolean
            Get
                Return _searchIsInProgress
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_searchIsInProgress, value, "SearchIsInProgress")
            End Set
        End Property
#End Region

#Region "DisplayMode"
        Public Property EnableHubDisplay As RelayCommand
        Public Property EnableListDisplay As RelayCommand

        Private Sub InitializeDisplayModeCommands()
            EnableHubDisplay = New RelayCommand(AddressOf SwitchToHubDisplay)
            EnableListDisplay = New RelayCommand(AddressOf SwitchToListDisplay)
        End Sub

        Private Async Sub SwitchToHubDisplay()
            If Not QueryResult.DisplayModeHub Then
                Await DispatcherHelper.ExecuteOnUIThreadAsync(Sub() QueryResult.DisplayModeHub = True)
            End If
        End Sub

        Private Async Sub SwitchToListDisplay()
            If AppShell.Current.QueryResult.DisplayModeHub Then
                Await DispatcherHelper.ExecuteOnUIThreadAsync(Sub() QueryResult.DisplayModeHub = False)
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
            If QueryResult.CurrentItem IsNot Nothing Then
                If QueryResult.CurrentItem.BrowserAdapter.BibItemUriIsValid Then
                    QueryResult.CurrentItem.BrowserAdapter.CloneTo(AppShell.Current.ViewModel)
                    AppShell.Current.ViewModel.BrowserPaneOpen = True
                End If
            End If
        End Sub

        Private Sub OnCloseBrowserPane()
            AppShell.Current.ViewModel.BrowserPaneOpen = False
            AppShell.Current.ViewModel.CloneTo(QueryResult.CurrentItem.BrowserAdapter)
        End Sub

#End Region

#Region "SearchCommands"

        Public Property SearchCommand As RelayCommand

        Private Sub InitalizeSearchCommands()
            SearchCommand = New RelayCommand(AddressOf OnSearchAsync)
        End Sub

        Private Async Function OnSearchAsync() As Task

            SearchIsInProgress = True

            Dim libraryAccess As ILibraryAccess = LibraryRegistry.Current.ISBNQueryLibrary

            libraryAccess.SetSearchParameters(isbn:=SearchISBN)

            Await libraryAccess.ExecuteQueryAsync(100)

            QueryResult.SetItems(libraryAccess)
            QueryResult.NoOfEntries = libraryAccess.NoOfEntries
            QueryResult.NoOfPages = libraryAccess.NoOfPages
            QueryResult.CurrentPosition = App.Texts.GetString("Page") + " : " + libraryAccess.CurrentPage.ToString + " / " + libraryAccess.NoOfPages.ToString
            QueryResult.HasNextPage = libraryAccess.HasNextPage
            QueryResult.HasPreviousPage = libraryAccess.HasPreviousPage

            SearchIsInProgress = False

        End Function

#End Region

#Region "ImportScannedBooks"

        Public Property ImportScannedBooksCommand As RelayCommand

        Private _filename As String
        Public Property Filename As String
            Get
                Return _filename
            End Get
            Set(value As String)
                SetProperty(Of String)(_filename, value)
            End Set
        End Property

        Private Sub InitializeImportCommands()
            ImportScannedBooksCommand = New RelayCommand(AddressOf OnImportScannedBooksCommand)
        End Sub

        Private Async Function OnImportScannedBooksCommand() As Task

            Dim openPicker = New Windows.Storage.Pickers.FileOpenPicker()
            openPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder
            openPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail

            ' Filter to include a sample subset of file types.
            openPicker.FileTypeFilter.Clear()
            openPicker.FileTypeFilter.Add(".xml")

            ' Open the file picker.
            Dim file = Await openPicker.PickSingleFileAsync()

            ' file is null if user cancels the file picker.
            If file IsNot Nothing Then
                Try
                    SearchIsInProgress = True
                    Filename = file.Name
                    Dim source As String = Await Windows.Storage.FileIO.ReadTextAsync(file)
                    scannedBooks.ImportScannedBooks(source)

                    If scannedBooks.ISBNCodes.Count > 0 Then
                        SetCurrentBookPosition(0)
                        SearchISBN = scannedBooks.ISBNCodes(0)
                        Await OnSearchAsync()
                    End If

                Catch ex As Exception
                    SearchIsInProgress = False
                End Try
            End If

        End Function

#End Region
    End Class

End Namespace
