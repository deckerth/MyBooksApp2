Imports System.Text.RegularExpressions
Imports MyBooks.Models
Imports Telerik.Core
Imports MyBooks.App.ValueConverters
Imports MyBooks.Repository

Namespace Global.MyBooks.App.ViewModels

    Public Class MultipleBooksViewModel
        Inherits BookViewModel
        Implements INotifyPropertyChanged

        ' <summary>
        ' The underlying customer model. Friend so it Is 
        ' Not visible to the RadDataGrid. 
        ' </summary>
        Friend Property Models As List(Of BookViewModel)

        ' <summary>
        ' Creates a New customer model.
        ' </summary>
        Public Sub New(models As List(Of BookViewModel))
            MyBase.New(New Book)
            If models Is Nothing Then
                Me.Models = New List(Of BookViewModel)
            Else
                Me.Models = models
            End If

            Title = App.Texts.GetString("KeepValue")
            OriginalTitle = App.Texts.GetString("KeepValue")
            Authors = App.Texts.GetString("KeepValue")
            Keywords = App.Texts.GetString("KeepValue")
            Storage = App.Texts.GetString("KeepValue")
            BorrowedDate = App.Texts.GetString("KeepValue")
            BorrowedTo = App.Texts.GetString("KeepValue")
            Published = App.Texts.GetString("KeepValue")
            OCLCNo = App.Texts.GetString("KeepValue")
            DLCNo = App.Texts.GetString("KeepValue")
            ISBN = App.Texts.GetString("KeepValue")
            NBACN = App.Texts.GetString("KeepValue")
            ASIN = App.Texts.GetString("KeepValue")
            Url = App.Texts.GetString("KeepValue")
            GoogleBooksUrl = App.Texts.GetString("KeepValue")
            SetupLists()
            IsInEdit = True
            IsModified = False
        End Sub

        Private Sub AddStringToList(ByRef storage As List(Of String), text As String, ByRef Optional BlanksFlag As Boolean = False)
            If String.IsNullOrEmpty(text) Then
                BlanksFlag = True
            Else
                If storage.Where(Function(x) x.Equals(text)).FirstOrDefault() Is Nothing Then
                    storage.Add(text)
                End If
            End If
        End Sub

        Private Sub AddBookToLists(item As BookViewModel)
            AddStringToList(_titles, item.Title, TitlesHaveBlanks)
            AddStringToList(_originalTitles, item.OriginalTitle, OriginalTitlesHaveBlanks)
            AddStringToList(_authorsList, item.Authors, AuthorsHaveBlanks)
            AddStringToList(_keywordsList, item.Keywords, KeywordsHaveBlanks)
            AddStringToList(_storages, item.Storage, StoragesHaveBlanks)
            AddStringToList(_borrowedDates, item.BorrowedDate, BorrowedDatesHaveBlanks)
            AddStringToList(_borrowedToList, item.BorrowedTo, BorrowedToListHasBlanks)
            AddStringToList(_publishedList, item.Published, PublishedListHasBlanks)
            AddStringToList(_oclcNoList, item.OCLCNo, OCLCNosHaveBlanks)
            AddStringToList(_dlcNoList, item.DLCNo, DLCNosHaveBlanks)
            AddStringToList(_isbnList, item.ISBN, ISBNsHaveBlanks)
            AddStringToList(_nbacnList, item.NBACN, NBACNListHasBlanks)
            AddStringToList(_asinList, item.ASIN, ASINLIstHasBlanks)
            AddStringToList(_urls, item.Url, UrlsHaveBlanks)
            AddStringToList(_googleBooksUrls, item.GoogleBooksUrl, GoogleUrlsHaveBlanks)
        End Sub

        Private Sub AddDeleteEntryToLists()
            Dim deleteEntry = App.Texts.GetString("DeleteEntry")
            AddStringToList(_titles, deleteEntry)
            AddStringToList(_originalTitles, deleteEntry)
            AddStringToList(_authorsList, deleteEntry)
            AddStringToList(_keywordsList, deleteEntry)
            AddStringToList(_storages, deleteEntry)
            AddStringToList(_borrowedDates, deleteEntry)
            AddStringToList(_borrowedToList, deleteEntry)
            AddStringToList(_publishedList, deleteEntry)
            AddStringToList(_oclcNoList, deleteEntry)
            AddStringToList(_dlcNoList, deleteEntry)
            AddStringToList(_isbnList, deleteEntry)
            AddStringToList(_nbacnList, deleteEntry)
            AddStringToList(_asinList, deleteEntry)
            AddStringToList(_urls, deleteEntry)
            AddStringToList(_googleBooksUrls, deleteEntry)
        End Sub

        Private Sub InitializeModelField(BlanksFlag As Boolean, values As List(Of String), ByRef field As String)
            If Not BlanksFlag AndAlso values.Count = 3 Then
                field = values.Item(2)
            End If
        End Sub

        Private Sub InitializeModelFields()
            InitializeModelField(TitlesHaveBlanks, Titles, Title)
            InitializeModelField(OriginalTitlesHaveBlanks, OriginalTitles, OriginalTitle)
            InitializeModelField(AuthorsHaveBlanks, AuthorsList, Authors)
            InitializeModelField(KeywordsHaveBlanks, KeywordsList, Keywords)
            InitializeModelField(StoragesHaveBlanks, Storages, Storage)
            InitializeModelField(BorrowedDatesHaveBlanks, BorrowedDates, BorrowedDate)
            InitializeModelField(BorrowedToListHasBlanks, BorrowedToList, BorrowedTo)
            InitializeModelField(PublishedListHasBlanks, PublishedList, Published)
            InitializeModelField(OCLCNosHaveBlanks, OCLCNoList, OCLCNo)
            InitializeModelField(DLCNosHaveBlanks, DLCNoList, DLCNo)
            InitializeModelField(ISBNsHaveBlanks, ISBNList, ISBN)
            InitializeModelField(NBACNListHasBlanks, NBACNList, NBACN)
            InitializeModelField(ASINLIstHasBlanks, ASINList, ASIN)
            InitializeModelField(UrlsHaveBlanks, Urls, Url)
            InitializeModelField(GoogleUrlsHaveBlanks, GoogleBooksUrls, GoogleBooksUrl)
        End Sub

        Private Sub SetupLists()
            AddBookToLists(Me) ' keep value
            AddDeleteEntryToLists()
            Dim keepMediaType = New MediumTypeDescriptor(Book.MediaType.KeepValue)
            _allMediaTypes.Insert(0, keepMediaType)
            MediumDescriptor = keepMediaType
            For Each b In Models
                AddBookToLists(b)
            Next
            InitializeModelFields()
        End Sub

        Private Function UpdateString(ByRef storage As String, value As String) As Boolean
            If value.Equals(App.Texts.GetString("KeepValue")) Then
                Return False
            ElseIf value.Equals(App.Texts.GetString("DeleteEntry")) Then
                If Not String.IsNullOrEmpty(storage) Then
                    storage = ""
                    Return True
                Else
                    Return False
                End If
            ElseIf storage.Equals(value) Then
                Return False
            Else
                storage = value
                Return True
            End If
        End Function

        Private Function UpdateBook(item As BookViewModel) As Boolean
            Dim changed As Boolean
            changed = UpdateString(item.Title, Title)
            changed = changed Or UpdateString(item.OriginalTitle, OriginalTitle)
            changed = changed Or UpdateString(item.Authors, Authors)
            changed = changed Or UpdateString(item.Keywords, Keywords)
            changed = changed Or UpdateString(item.MediumDescriptor.Name, MediumDescriptor.Name)
            changed = changed Or UpdateString(item.Storage, Storage)
            changed = changed Or UpdateString(item.BorrowedTo, BorrowedTo)
            changed = changed Or UpdateString(item.BorrowedDate, BorrowedDate)
            changed = changed Or UpdateString(item.Published, Published)
            changed = changed Or UpdateString(item.OCLCNo, OCLCNo)
            changed = changed Or UpdateString(item.DLCNo, DLCNo)
            changed = changed Or UpdateString(item.ISBN, ISBN)
            changed = changed Or UpdateString(item.NBACN, NBACN)
            changed = changed Or UpdateString(item.ASIN, ASIN)
            changed = changed Or UpdateString(item.Url, Url)
            changed = changed Or UpdateString(item.GoogleBooksUrl, GoogleBooksUrl)
            If Medium <> Book.MediaType.KeepValue AndAlso Medium <> item.Medium Then
                changed = True
                item.MediumDescriptor = MediumDescriptor ' also changes the medium field
            End If
            Return changed
        End Function


        Public Async Function SaveAll() As Task(Of UpdateCounters)
            Dim counters = New UpdateCounters
            For Each b In Models
                If UpdateBook(b) Then
                    Dim updateResult = Await App.Repository.Books.Upsert(b.Model)
                    If updateResult <> IBookRepository.UpsertResult.skipped Then
                        Await b.UpdateIndex()
                    End If
                    counters.Increment(updateResult)
                    b.IsModified = False
                Else
                    counters.Increment(IBookRepository.UpsertResult.skipped)
                End If
            Next
            IsModified = False
            Return counters
        End Function

        Private _titles As New List(Of String)
        Public ReadOnly Property Titles As List(Of String)
            Get
                Return _titles
            End Get
        End Property
        Private TitlesHaveBlanks As Boolean

        Private _originalTitles As New List(Of String)
        Public ReadOnly Property OriginalTitles As List(Of String)
            Get
                Return _originalTitles
            End Get
        End Property
        Private OriginalTitlesHaveBlanks As Boolean

        Private _authorsList As New List(Of String)
        Public ReadOnly Property AuthorsList As List(Of String)
            Get
                Return _authorsList
            End Get
        End Property
        Private AuthorsHaveBlanks As Boolean

        Private _keywordsList As New List(Of String)
        Public ReadOnly Property KeywordsList As List(Of String)
            Get
                Return _keywordsList
            End Get
        End Property
        Private KeywordsHaveBlanks As Boolean

        Private _storages As New List(Of String)
        Public ReadOnly Property Storages As List(Of String)
            Get
                Return _storages
            End Get
        End Property
        Private StoragesHaveBlanks As Boolean

        Private _borrowedDates As New List(Of String)
        Public ReadOnly Property BorrowedDates As List(Of String)
            Get
                Return _borrowedDates
            End Get
        End Property
        Private BorrowedDatesHaveBlanks As Boolean

        Private _borrowedToList As New List(Of String)
        Public ReadOnly Property BorrowedToList As List(Of String)
            Get
                Return _borrowedToList
            End Get
        End Property
        Private BorrowedToListHasBlanks As Boolean

        Private _publishedList As New List(Of String)
        Public ReadOnly Property PublishedList As List(Of String)
            Get
                Return _publishedList
            End Get
        End Property
        Private PublishedListHasBlanks As Boolean

        Private _oclcNoList As New List(Of String)
        Public ReadOnly Property OCLCNoList As List(Of String)
            Get
                Return _oclcNoList
            End Get
        End Property
        Private OCLCNosHaveBlanks As Boolean


        Private _dlcNoList As New List(Of String)
        Public ReadOnly Property DLCNoList As List(Of String)
            Get
                Return _dlcNoList
            End Get
        End Property
        Private DLCNosHaveBlanks As Boolean

        Private _isbnList As New List(Of String)
        Public ReadOnly Property ISBNList As List(Of String)
            Get
                Return _isbnList
            End Get
        End Property
        Private ISBNsHaveBlanks As Boolean

        Private _nbacnList As New List(Of String)
        Public ReadOnly Property NBACNList As List(Of String)
            Get
                Return _nbacnList
            End Get
        End Property
        Private NBACNListHasBlanks As Boolean

        Private _asinList As New List(Of String)
        Public ReadOnly Property ASINList As List(Of String)
            Get
                Return _asinList
            End Get
        End Property
        Private ASINLIstHasBlanks As Boolean

        Private _urls As New List(Of String)
        Public ReadOnly Property Urls As List(Of String)
            Get
                Return _urls
            End Get
        End Property
        Private UrlsHaveBlanks As Boolean

        Private _googleBooksUrls As New List(Of String)
        Public ReadOnly Property GoogleBooksUrls As List(Of String)
            Get
                Return _googleBooksUrls
            End Get
        End Property
        Private GoogleUrlsHaveBlanks As Boolean

    End Class

End Namespace
