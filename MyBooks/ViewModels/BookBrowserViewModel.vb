Imports MyBooks.App.Commands
Imports MyBooks.Models
Imports MyBooks.Repository.Libraries
Imports MyBooks.App.Views
Imports MyBooks.App.ValueConverters
Imports MyBooks.Repository
Imports Microsoft.Toolkit.Uwp.Helpers

Namespace Global.MyBooks.App.ViewModels

    Public Class BookBrowserViewModel

        Inherits BindableBase

        Private _model As Book
        Public ReadOnly Property Model As Book
            Get
                Return _model
            End Get
        End Property

#Region "ModelFields"
        Private _title As String
        Public ReadOnly Property Title As String
            Get
                Return _model.Title
            End Get
        End Property

        Private _originalTitle As String
        Public ReadOnly Property OriginalTitle As String
            Get
                Return _model.OriginalTitle
            End Get
        End Property

        Private _authors As String
        Public ReadOnly Property Authors As String
            Get
                Return _model.Authors
            End Get
        End Property

        Private MediumConverter As New MediumTypeToStringConverter
        Private _mediumText As String
        Public ReadOnly Property MediumText As String
            Get
                Return MediumConverter.Convert(_model.Medium, Nothing, "", "")
            End Get
        End Property

        Private _published As String
        Public ReadOnly Property Published As String
            Get
                Return _model.Published
            End Get
        End Property
#End Region

        Public Property BrowserAdapter As BrowserSupportViewModel

        Private Library As ILibraryAccess

        Public Sub New()
            _model = New Book()
            BrowserAdapter = New BrowserSupportViewModel()
        End Sub

        Public Property AddBookCommand As RelayCommand

        Public Sub New(library As ILibraryAccess, item As Book)
            _model = item
            Me.Library = library
            BrowserAdapter = New BrowserSupportViewModel(_model)
            AddBookCommand = New RelayCommand(AddressOf OnAddBookCommand)
        End Sub

        Private BookLinksInitialized As Boolean = False

        Public Sub InitializeBookLinks()
            If BookLinksInitialized Then
                Return ' Already initialized
            End If
            BookLinksInitialized = True

            If Library IsNot Nothing Then
                BrowserAdapter.BibItemLibraryUri = Library.GetLink(Model)
                BrowserAdapter.BibItemGoogleBooksUri = Library.GetGoogleBooksLink(Model)
                BrowserAdapter.OnDisplayStandardLibCommand()
            End If
        End Sub

        Private Sub InitializeModelLinks()
            InitializeBookLinks()
            If BrowserAdapter.BibItemLibraryUri IsNot Nothing Then
                Model.Url = BrowserAdapter.BibItemLibraryUri.ToString
            End If
            If BrowserAdapter.BibItemGoogleBooksUri IsNot Nothing Then
                Model.GoogleBooksUrl = BrowserAdapter.BibItemGoogleBooksUri.ToString
            End If
        End Sub


        Public Async Sub OnAddBookCommand()
            InitializeModelLinks()
            Await DispatcherHelper.ExecuteOnUIThreadAsync(Async Function()
                                                              Dim AddBookDialog As AddBookFromLibraryDialog
                                                              AddBookDialog = New AddBookFromLibraryDialog(Model)
                                                              Await AddBookDialog.ShowAsync()
                                                          End Function)
        End Sub

        Public Async Function AddBookAsync() As Task(Of IBookRepository.UpsertResult)
            InitializeModelLinks()

            Dim BookDetailViewModel As BookDetailPageViewModel
            BookDetailViewModel = New BookDetailPageViewModel() With {.IsNewBook = True, .Book = New BookViewModel(Model)}
            Return Await BookDetailViewModel.Save()
        End Function
    End Class

End Namespace
