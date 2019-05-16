﻿Imports System.Threading
Imports Microsoft.Toolkit.Uwp.Helpers
Imports MyBooks.App.Commands
Imports MyBooks.App.Views
Imports MyBooks.Repository
Imports MyBooks.Repository.Sql
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.Storage.Provider
Imports Windows.UI.Popups

Namespace Global.MyBooks.App.ViewModels

    Public Class BookListPageViewModel
        Inherits BindableBase

        Private Shared _current As BookListPageViewModel
        Public Shared ReadOnly Property Current As BookListPageViewModel
            Get
                If _current Is Nothing Then
                    _current = New BookListPageViewModel()
                End If
                Return _current
            End Get
        End Property

        Public Sub New()
            _current = Me
            Task.Run(AddressOf GetBooksListAsync)
            SyncCommand = New RelayCommand(AddressOf OnSync)
            ImportDbCommand = New RelayCommand(AddressOf OnImportDB)
            ExportDbCommand = New RelayCommand(AddressOf OnExportDB)
            DeleteBookCommand = New RelayCommand(AddressOf OnDeleteBook)
            InitializePaneCommands()
            AddHandler BookViewModel.Modified, AddressOf OnBookModified
            AddHandler BookDetailPageViewModel.OnNewBookCreated, AddressOf OnBookCreated
        End Sub

#Region "Properties"

        Private _isModified As Boolean
        Public Property IsModified As Boolean
            Get
                Return _isModified
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_isModified, value, "IsModified")
            End Set
        End Property

        Private Sub OnBookModified()
            IsModified = True
        End Sub

        Private Async Sub OnBookCreated(newBook As BookViewModel)
            Await DispatcherHelper.ExecuteOnUIThreadAsync(Sub() _books.Add(newBook))
        End Sub

        Private _books As ObservableCollection(Of BookViewModel) = New ObservableCollection(Of BookViewModel)
        Public Property Books As ObservableCollection(Of BookViewModel)
            Get
                Return _books
            End Get
            Set(value As ObservableCollection(Of BookViewModel))
                SetProperty(Of ObservableCollection(Of BookViewModel))(_books, value)
            End Set
        End Property

        Private _selectedBook As New BookViewModel(Nothing)
        Public Property SelectedBook As BookViewModel
            Get
                Return _selectedBook
            End Get
            Set(value As BookViewModel)
                SetProperty(Of BookViewModel)(_selectedBook, value)
            End Set
        End Property

        Private _errorText As String = Nothing
        ' <summary>
        ' Gets Or sets the error text.
        ' </summary>
        Public Property ErrorText As String
            Get
                Return _errorText
            End Get
            Set(value As String)
                SetProperty(Of String)(_errorText, value)
            End Set
        End Property

        Public Property Progress As New ProgressRingViewModel

#End Region

#Region "DataAccess"
        Public Async Function GetBooksListAsync() As Task
            Await Progress.SetIndeterministicAsync()
            Dim repo = Await App.Repository.Books.GetAsync()
            If repo Is Nothing Then
                Return
            End If
            Await DispatcherHelper.ExecuteOnUIThreadAsync(
                Sub()
                    Books.Clear()
                    For Each b In repo
                        Books.Add(New BookViewModel(b) With {.Validate = True})
                    Next
                End Sub)
            Await Progress.HideAsync()
        End Function

        Public Property SyncCommand As RelayCommand

        Private Async Function Synchronize() As Task
            Await Progress.SetIndeterministicAsync()

            Dim modifiedViewModels = Books.Where(Function(x) x.IsModified)
            For Each m In modifiedViewModels
                Await m.Save()
            Next

            Await DispatcherHelper.ExecuteOnUIThreadAsync(Sub() IsModified = False)
            Await Progress.HideAsync()
        End Function

        Private Sub OnSync()
            Task.Run(AddressOf Synchronize)
        End Sub

        Public DeleteBookCommand As RelayCommand

        Public Async Sub OnDeleteBook()
            If SelectedBook IsNot Nothing AndAlso SelectedBook.Title.Length > 0 Then
                Dim dialog = New MessageDialog(App.Texts.GetString("DeleteBookQuestion"))

                ' Add commands and set their callbacks 
                dialog.Commands.Add(New UICommand(App.Texts.GetString("Yes")))
                dialog.Commands.Add(New UICommand(App.Texts.GetString("Cancel"), Sub(command) Cancelled = True))

                Cancelled = False
                Await dialog.ShowAsync()

                If Cancelled = False Then
                    Await App.Repository.Books.DeleteAsync(SelectedBook.Id)
                    Books.Remove(SelectedBook)
                    SelectedBook = Nothing
                End If
            End If
        End Sub

#End Region

#Region "ImportExport"
        Public ImportDbCommand As RelayCommand
        Public ExportDbCommand As RelayCommand

        Public Async Function OnExportDB() As Task

            Dim savepicker As New FileSavePicker With {
            .SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        }

            ' Dropdown of file types the user can save the file as
            savepicker.FileTypeChoices.Add("Excel Workbook", New List(Of String) From {".xlsx"})
            ' Default file name if the user does Not type one in Or select a file to replace
            savepicker.SuggestedFileName = "MyBooks"

            Dim File As StorageFile = Await savepicker.PickSaveFileAsync()
            If File IsNot Nothing Then
                Using stream = Await File.OpenStreamForWriteAsync()
                    stream.SetLength(0)
                    ' Prevent updates to the remote version of the file until we finish making changes And call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(File)
                    Await App.Repository.ImportExportService.ExportAsync(stream)

                    Dim status As FileUpdateStatus = Await CachedFileManager.CompleteUpdatesAsync(File)
                    If status = FileUpdateStatus.Complete Then
                        Await New MessageDialog(App.Texts.GetString("DatabaseSaved")).ShowAsync()
                    Else
                        Await New MessageDialog(App.Texts.GetString("DatabaseNotSaved")).ShowAsync()
                    End If
                End Using
            End If
        End Function

        Private ImportDecisionDialogResult As IImportExportService.ImportOptions
        Private Cancelled As Boolean

        Private Async Function ImportDialog() As Task(Of Boolean)

            Dim dialog = New MessageDialog(App.Texts.GetString("ImportDecision"))

            ' Add commands and set their callbacks 
            dialog.Commands.Add(New UICommand(App.Texts.GetString("AddImport"), Sub(command) ImportDecisionDialogResult = IImportExportService.ImportOptions.AddBooks))
            dialog.Commands.Add(New UICommand(App.Texts.GetString("ReplaceCollection"), Sub(command) ImportDecisionDialogResult = IImportExportService.ImportOptions.ReplaceBooks))
            dialog.Commands.Add(New UICommand(App.Texts.GetString("Cancel"), Sub(command) Cancelled = True))

            Cancelled = False
            Await dialog.ShowAsync()

            Return Not Cancelled

        End Function

        Public Async Function OnImportDB() As Task

            If Await ImportDialog() Then
                Dim openPicker As FileOpenPicker = New FileOpenPicker()
                openPicker.ViewMode = PickerViewMode.Thumbnail
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary
                openPicker.FileTypeFilter.Add(".xlsx")
                Dim File As StorageFile = Await openPicker.PickSingleFileAsync()
                Dim Counters As UpdateCounters
                If File IsNot Nothing Then
                    Await Progress.SetIndeterministicAsync()
                    Counters = Await App.Repository.ImportExportService.ImportAsync(Await File.OpenStreamForReadAsync(), ImportDecisionDialogResult)
                    Await GetBooksListAsync()
                    Await Progress.HideAsync()
                    Dim dialog = New ImportResultDialog(Counters)
                    Await dialog.ShowAsync()
                End If
            End If

        End Function

#End Region

#Region "UpdateIndex"

        Public Async Function UpdateIndexAsync() As Task
            Await Progress.SetDeterministicAsync(Books.Count)
            App.Repository.StartMassUpdate()
            Await App.Repository.Authors.ClearAsync()
            Await App.Repository.Keywords.ClearAsync()
            Await App.Repository.Storages.ClearAsync()
            For Each b In Books
                Await b.UpdateIndex()
                Await Progress.IncrementAsync(1)
            Next
            Await App.Repository.EndMassUpdateAsync()

            Await Progress.HideAsync()
        End Function

#End Region

#Region "BrowserPane"

        Private Sub InitializePaneCommands()
            OpenBrowserPane = New RelayCommand(AddressOf OnOpenBrowserPane)
            CloseBrowserPane = New RelayCommand(AddressOf OnCloseBrowserPane)
        End Sub

        Public Property CloseBrowserPane As RelayCommand
        Public Property OpenBrowserPane As RelayCommand

        Private Sub OnOpenBrowserPane()
            If SelectedBook IsNot Nothing Then
                If SelectedBook.BrowserAdapter.BibItemUriIsValid Then
                    SelectedBook.BrowserAdapter.CloneTo(AppShell.Current.ViewModel)
                    AppShell.Current.ViewModel.BrowserPaneOpen = True
                End If
            End If
        End Sub

        Private Sub OnCloseBrowserPane()
            AppShell.Current.ViewModel.BrowserPaneOpen = False
            AppShell.Current.ViewModel.CloneTo(SelectedBook.BrowserAdapter)
        End Sub

#End Region

    End Class

End Namespace
