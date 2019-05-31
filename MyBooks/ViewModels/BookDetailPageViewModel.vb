Imports Microsoft.Toolkit.Uwp.Helpers
Imports MyBooks.App.Commands
Imports MyBooks.Repository
Imports Telerik.Core

Namespace Global.MyBooks.App.ViewModels

    Public Class BookDetailPageViewModel
        Inherits BindableBase

        Public Sub New()
            InitializeCommands()
        End Sub

        Public Sub New(bookSet As MultipleBooksViewModel)
            InitializeCommands()
            Books = bookSet
        End Sub

        Private Sub InitializeCommands()
            SaveCommand = New RelayCommand(Async Sub()
                                               Await Save()
                                           End Sub)
            CancelEditsCommand = New RelayCommand(AddressOf OnCancelEdits)
            StartEditCommand = New RelayCommand(AddressOf OnStartEdit)
        End Sub

        Public Shared Event OnNewBookCreated(eNewBookCreatedArgs As BookViewModel)

        Private _isLoading As Boolean = False
        ' <summary>
        ' Gets Or sets whether to show the data loading progress wheel. 
        ' </summary>
        Public Property IsLoading As Boolean
            Get
                Return _isLoading
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_isLoading, value)
            End Set
        End Property

        Private _isNewBook As Boolean = False
        ' <summary>
        ' Indicates whether this Is a New book. 
        ' </summary>
        Public Property IsNewBook As Boolean
            Get
                Return _isNewBook
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_isNewBook, value)
            End Set
        End Property

        Private _multipleBooks As Boolean = False
        Public Property MultipleBooks As Boolean
            Get
                Return _multipleBooks
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_multipleBooks, value)
            End Set
        End Property

        Private _isInEdit As Boolean = False
        ' <summary>
        ' Gets or sets the current edit mode 
        ' </summary>
        Public Property IsInEdit As Boolean
            Get
                Return _isInEdit
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_isInEdit, value)
            End Set
        End Property

        Private _book As BookViewModel
        Public Property Book As BookViewModel
            Get
                Return _book
            End Get
            Set(value As BookViewModel)
                If SetProperty(Of BookViewModel)(_book, value) Then
                    If String.IsNullOrWhiteSpace(Book.Title) Then
                        IsInEdit = True
                    End If
                End If
                MultipleBooks = False
                AddHandler _book.ErrorsChanged, AddressOf OnErrorsChanged
            End Set
        End Property

        Private _books As MultipleBooksViewModel
        Public Property Books As MultipleBooksViewModel
            Get
                Return _books
            End Get
            Set(value As MultipleBooksViewModel)
                SetProperty(Of MultipleBooksViewModel)(_books, value)
                If value Is Nothing Then
                    MultipleBooks = False
                Else
                    Book = value
                    MultipleBooks = True
                    IsInEdit = True
                    IsNewBook = False
                    For Each b In Books.Models
                        AddHandler b.ErrorsChanged, AddressOf OnErrorsChanged
                    Next
                End If
            End Set
        End Property

        Private errorBuffer As String

        Private Sub AppendErrors(msgs As IEnumerable)
            Dim errorMsg As String
            If msgs IsNot Nothing Then
                For Each msg In msgs
                    errorMsg = TryCast(msg, String)
                    If errorMsg IsNot Nothing Then
                        If errorBuffer.Length > 0 Then
                            errorBuffer = errorBuffer + vbCrLf
                        End If
                        errorBuffer = errorBuffer + errorMsg
                    End If
                Next
            End If
        End Sub

        Protected Sub OnErrorsChanged(sender As Object, e As DataErrorsChangedEventArgs)
            If _book.HasErrors Then
                errorBuffer = ""
                AppendErrors(_book.GetErrors("Title"))
                AppendErrors(_book.GetErrors("BorrowedDate"))
                AppendErrors(_book.GetErrors("Published"))
                ErrorText = errorBuffer
            Else
                ErrorText = Nothing
            End If
        End Sub

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

        Private existenceCheckTaskRunning As Boolean
        Private existenceCheckRequired As Boolean = False
        Private bookExistsDetected As Boolean = False

        Private Async Function CheckExistenceAsync() As Task
            existenceCheckTaskRunning = True
            Do
                existenceCheckRequired = False
                Dim existingCopy = Await App.Repository.Books.GetAsync(title:=_book.Title, author:=_book.Authors, mediatype:=_book.Medium, storage:=_book.Storage)
                If existingCopy Is Nothing Then
                    If bookExistsDetected Then
                        bookExistsDetected = False
                        ErrorText = ""
                    End If
                Else
                    bookExistsDetected = True
                    ErrorText = App.Texts.GetString("BookDoesAlreadyExist")
                End If
            Loop While existenceCheckRequired
            existenceCheckTaskRunning = False
        End Function


        Public Sub CheckExistence()
            If existenceCheckTaskRunning Then
                existenceCheckRequired = True
            Else
                Dim task = CheckExistenceAsync()
            End If
        End Sub

        Public Property SaveCommand As RelayCommand

        ' <summary>
        ' Saves book data that has been edited.
        ' </summary>
        Public Async Function Save() As Task(Of IBookRepository.UpsertResult)
            Dim result As IBookRepository.UpsertResult = IBookRepository.UpsertResult.skipped
            Await _book.ValidateAsync("Published")
            If Not _book.HasErrors() Then
                If _book.BorrowedDate Is Nothing Then
                    _book.BorrowedDate = Date.MinValue
                End If
                result = Await _book.Save()
                If result = Repository.IBookRepository.UpsertResult.added Then
                    RaiseEvent OnNewBookCreated(_book)
                End If
                If _book.BorrowedDate.Equals(Date.MinValue) Then
                    _book.BorrowedDate = Nothing
                End If
            End If
            Return result
        End Function

        Public Property CancelEditsCommand As RelayCommand

        ' <summary>
        ' Cancels any in progress edits.
        ' </summary>
        Private Sub OnCancelEdits()
            IsInEdit = False
        End Sub

        Public Property StartEditCommand As RelayCommand

        ' <summary>
        ' Starts editing.
        ' </summary>
        Private Sub OnStartEdit()
            IsInEdit = True
        End Sub

    End Class

End Namespace
