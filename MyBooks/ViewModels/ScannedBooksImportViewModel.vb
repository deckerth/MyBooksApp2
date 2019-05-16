Imports Microsoft.Toolkit.Uwp.Helpers
Imports MyBooks.App.Commands
Imports MyBooks.App.Views
Imports MyBooks.Repository
Imports MyBooks.Repository.Libraries
Imports Telerik.UI.Xaml.Controls.Grid
Imports Windows.Storage
Imports Windows.UI.Popups

Namespace Global.MyBooks.App.ViewModels

    Public Class ScannedBooksImportViewModel
        Inherits BindableBase

        Public Sub New()
            InitializeDisplayModeCommands()
            InitalizeSearchCommands()
            InitializePaneCommands()
            InitalizeNavCommands()
            InitializeImportCommands()
            InitializeAddBooksCommand()
        End Sub

        Public Property QueryResult As LibraryBooksViewModel = AppShell.Current.QueryResult
        Public Property Progress As New ProgressRingViewModel

#Region "BookNavigation"

        Private currentBook As Integer = 0
        Private scannedBooks As New ScannedBooks()
        Private kindleLib As New KindleLibrary()

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

        Private _selectAllVisibility As Visibility
        Public Property SelectAllVisibility As Visibility
            Get
                Return _selectAllVisibility
            End Get
            Set(value As Visibility)
                If value <> _selectAllVisibility Then
                    SetProperty(Of Visibility)(_selectAllVisibility, value)
                End If
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

            Await Progress.SetIndeterministicAsync()

            Dim libraryAccess As ILibraryAccess = LibraryRegistry.Current.ISBNQueryLibrary

            libraryAccess.SetSearchParameters(isbn:=SearchISBN)

            Await libraryAccess.ExecuteQueryAsync(100)

            Await QueryResult.SetItemsAsync(Progress, libraryAccess)
            QueryResult.NoOfEntries = libraryAccess.NoOfEntries
            QueryResult.NoOfPages = libraryAccess.NoOfPages
            QueryResult.CurrentPosition = App.Texts.GetString("Page") + " : " + libraryAccess.CurrentPage.ToString + " / " + libraryAccess.NoOfPages.ToString
            QueryResult.HasNextPage = libraryAccess.HasNextPage
            QueryResult.HasPreviousPage = libraryAccess.HasPreviousPage
            QueryResult.SelectionMode = DataGridSelectionMode.Single

            Await Progress.HideAsync()

        End Function

#End Region

#Region "ImportScannedBooks"

        Public Property ImportScannedBooksCommand As RelayCommand
        Public Property ImportKindleBooksCommand As RelayCommand

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
            ImportKindleBooksCommand = New RelayCommand(AddressOf OnImportKindleBooksCommand)
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
            Dim errorOccurred As Boolean = False
            If file IsNot Nothing Then
                Try
                    Await Progress.SetIndeterministicAsync()
                    Filename = file.Name
                    Dim source As String = Await Windows.Storage.FileIO.ReadTextAsync(file)
                    scannedBooks.ImportScannedBooks(source)

                    If scannedBooks.ISBNCodes.Count > 0 Then
                        SetCurrentBookPosition(0)
                        SearchISBN = scannedBooks.ISBNCodes(0)
                        Await OnSearchAsync()
                    End If

                Catch ex As Exception
                    errorOccurred = True
                End Try
                If errorOccurred Then
                    Await Progress.HideAsync()
                End If
            End If

        End Function

        Private Async Function OnImportKindleBooksCommand() As Task

            Dim infoDialog = New ImportInfoDialog("KindleImportInfo")
            Await infoDialog.EnableKindleNameInputBox()
            Await infoDialog.ShowAsync()
            If infoDialog.DialogCancelled() Then
                Return
            End If

            ' Copy kindle path to clipboard
            Dim path = ApplicationData.Current.LocalFolder.Path 'path = "C:\Users\Thomas\AppData\Local\Packages\16412ThomasDecker.MeineBcher_ntc13x805381j\LocalState"
            Dim pos = path.IndexOf("AppData")
            If pos >= 0 Then
                Dim appFolder = path.Substring(0, pos + 8) 'appFolder = "C:\Users\Thomas\AppData\"
                Dim kindleFolder = appFolder + "Local\Amazon\Kindle\Cache"
                Dim kindleFolderPackage As New DataTransfer.DataPackage
                kindleFolderPackage.SetText(kindleFolder)
                DataTransfer.Clipboard.SetContent(kindleFolderPackage)
            End If

            Dim openPicker = New Windows.Storage.Pickers.FolderPicker()
            openPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder
            openPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail

            ' Filter to include a sample subset of file types.
            openPicker.FileTypeFilter.Clear()
            openPicker.FileTypeFilter.Add(".xml")

            ' Open the file picker.
            Dim folder = Await openPicker.PickSingleFolderAsync()

            ' file is null if user cancels the file picker.
            If folder IsNot Nothing Then
                Try
                    Await Progress.SetIndeterministicAsync()

                    Dim library As StorageFile = Nothing
                    Try
                        library = Await folder.GetFileAsync("KindleSyncMetadataCache.xml")
                    Catch ex As Exception
                    End Try
                    If library Is Nothing Then
                        Await Progress.HideAsync()
                        Return
                    Else
                        Filename = library.Name
                        Dim source As String = Await Windows.Storage.FileIO.ReadTextAsync(library)
                        kindleLib.ImportKindleLibrary(source)
                        Dim kindleName = infoDialog.GetKindleName()
                        If Not String.IsNullOrEmpty(kindleName) Then
                            SetStorage(kindleLib, kindleName)
                        End If

                        Await QueryResult.SetItemsAsync(Progress, kindleLib)

                        QueryResult.NoOfEntries = kindleLib.NoOfEntries
                        QueryResult.NoOfPages = kindleLib.NoOfPages
                        QueryResult.CurrentPosition = ""
                        QueryResult.HasNextPage = False
                        QueryResult.HasPreviousPage = False
                        QueryResult.SelectionMode = DataGridSelectionMode.Multiple
                    End If
                Catch ex As Exception
                End Try
                Await Progress.HideAsync()
            End If

        End Function

        Private Sub SetStorage(kindleLib As KindleLibrary, kindleName As String)
            For Each b In kindleLib.BookItems
                b.Storage = kindleName
            Next
        End Sub

#End Region

#Region "AddBooks"
        Public Property AddBooksCommand As RelayCommand

        Private Sub InitializeAddBooksCommand()
            AddBooksCommand = New RelayCommand(AddressOf StartAddBooksTaskAsync)
        End Sub

        Private Async Sub StartAddBooksTaskAsync()
            ' Enforce the execution on another thread than the UI thread
            Await Task.Run(Sub() OnAddBooks())
        End Sub

        Private Async Function OnAddBooks() As Task
            Dim counters As New UpdateCounters
            If QueryResult.SelectionMode = DataGridSelectionMode.Multiple Then
                If QueryResult.SelectedItems IsNot Nothing Then
                    If QueryResult.SelectedItems.Count = 1 Then
                        Dim importedBook As BookBrowserViewModel = DirectCast(QueryResult.SelectedItems(0), BookBrowserViewModel)
                        importedBook.AddBookCommand.Execute(Nothing)
                    Else
                        Await Progress.SetDeterministicAsync(QueryResult.SelectedItems.Count)

                        App.Repository.StartMassUpdate()
                        For Each b In QueryResult.SelectedItems
                            Dim importedBook As BookBrowserViewModel = DirectCast(b, BookBrowserViewModel)
                            counters.Increment(Await importedBook.AddBookAsync())
                            Await Progress.IncrementAsync(1)
                        Next
                        Await App.Repository.EndMassUpdateAsync()
                        Await Progress.HideAsync()
                        Await DispatcherHelper.ExecuteOnUIThreadAsync(Async Function() As Task
                                                                          Dim dialog = New ImportResultDialog(counters)
                                                                          Await dialog.ShowAsync()
                                                                      End Function)
                    End If
                End If
            Else
                If QueryResult.CurrentItem IsNot Nothing Then
                    QueryResult.CurrentItem.AddBookCommand.Execute(Nothing)
                End If
            End If
        End Function
#End Region
    End Class

End Namespace
