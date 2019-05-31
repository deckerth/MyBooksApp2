Imports System.Text.RegularExpressions
Imports MyBooks.Models
Imports Telerik.Core
Imports MyBooks.App.ValueConverters
Imports MyBooks.Repository

Namespace Global.MyBooks.App.ViewModels

    Public Class BookViewModel
        Inherits ValidateViewModelBase
        Implements INotifyPropertyChanged

        Public Property BrowserAdapter As BrowserSupportViewModel

        ' <summary>
        ' The underlying customer model. Friend so it Is 
        ' Not visible to the RadDataGrid. 
        ' </summary>
        Friend Property Model As Book

        Private _authorNameConversion As AuthorSuggestionsViewModel
        Public ReadOnly Property AuthorNameConversion As AuthorSuggestionsViewModel
            Get
                If _authorNameConversion Is Nothing Then
                    _authorNameConversion = New AuthorSuggestionsViewModel(Me)
                End If
                Return _authorNameConversion
            End Get
        End Property

        ' <summary>
        ' Creates a New customer model.
        ' </summary>
        Public Sub New(model As Book)
            If model Is Nothing Then
                Me.Model = New Book
            Else
                Me.Model = model
            End If
            MediumDescriptor = AllMediaTypes.Where(Function(x) x.Type = Me.Model.Medium).FirstOrDefault()
            BrowserAdapter = New BrowserSupportViewModel(model)
            BrowserAdapter.OnDisplayStandardLibCommand()
        End Sub

        Protected _allMediaTypes As List(Of MediumTypeDescriptor)
        Public ReadOnly Property AllMediaTypes As List(Of MediumTypeDescriptor)
            Get
                If _allMediaTypes Is Nothing Then
                    _allMediaTypes = New List(Of MediumTypeDescriptor)
                    For Each b In Book.AllMediaTypes
                        _allMediaTypes.Add(New MediumTypeDescriptor(b))
                    Next
                End If
                Return _allMediaTypes
            End Get
        End Property

        Public Async Function Refresh() As Task
            Model = Await App.Repository.Books.GetAsync(Id)
            IsModified = False
        End Function

        Public Async Function UpdateIndex() As Task
            If Not String.IsNullOrWhiteSpace(Model.Keywords) Then
                Dim keywords = Model.GetKeywordList()
                For Each k In keywords
                    Await App.Repository.Keywords.Insert(New Models.Keyword(k))
                Next
            End If

            If Not String.IsNullOrWhiteSpace(Model.Storage) Then
                Await App.Repository.Storages.Insert(New Models.Storage(Model.Storage))
            End If

            If Not String.IsNullOrWhiteSpace(Model.Authors) Then
                Await App.Repository.Authors.Insert(New Models.Author(Model.Authors))
                Dim list = Model.GetAuthorList(App.SelectedNameFormat = App.FirstNameLastNameFormat)
                If list.Count > 1 Then
                    For Each a In list
                        Await App.Repository.Authors.Insert(New Models.Author(a))
                    Next
                End If
            End If

        End Function

        Public Async Function Save() As Task(Of IBookRepository.UpsertResult)
            Dim result = Await App.Repository.Books.Upsert(Model)
            IsModified = False
            If result <> IBookRepository.UpsertResult.skipped Then
                Await UpdateIndex()
            End If
            Return result
        End Function

        Public Shared Event Modified()

        ' <summary>
        ' Gets Or sets whether the underlying model has been modified. 
        ' This Is used when sync'ing with the server to reduce load
        ' And only upload the models that changed.
        ' </summary>
        Private _is_Modified As Boolean
        Friend Property IsModified As Boolean
            Get
                Return _is_Modified
            End Get
            Set(value As Boolean)
                If value <> _is_Modified Then
                    _is_Modified = value
                    If _is_Modified Then
                        RaiseEvent Modified()
                    End If
                End If
            End Set
        End Property

        Public Property IsInEdit As Boolean

        ' <summary>
        ' Gets Or sets whether to validate model data. 
        ' </summary>
        Friend Property Validate As Boolean

        Public ReadOnly Property Id As Guid
            Get
                Return Model.Id
            End Get
        End Property

        ' <summary>
        ' Gets Or sets the title.
        ' </summary>
        Public Property Title As String
            Get
                Return Model.Title
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.Title) Then
                    Model.Title = value
                    IsModified = True
                    OnPropertyChanged("Title")
                End If
            End Set
        End Property

        Public Property OriginalTitle As String
            Get
                Return Model.OriginalTitle
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.OriginalTitle) Then
                    Model.OriginalTitle = value
                    IsModified = True
                    OnPropertyChanged("OriginalTitle")
                End If
            End Set
        End Property

        Public Property Authors As String
            Get
                Return Model.Authors
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.Authors) Then
                    Model.Authors = value
                    IsModified = True
                    OnPropertyChanged("Authors")
                End If
            End Set
        End Property

        Public Property Keywords As String
            Get
                Return Model.Keywords
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.Keywords) Then
                    Model.Keywords = value
                    IsModified = True
                    OnPropertyChanged("Keywords")
                End If
            End Set
        End Property

        Private _mediumDescriptor As MediumTypeDescriptor
        Public Property MediumDescriptor As MediumTypeDescriptor
            Get
                Return _mediumDescriptor
            End Get
            Set(value As MediumTypeDescriptor)
                If value IsNot Nothing AndAlso (_mediumDescriptor Is Nothing OrElse _mediumDescriptor.Type <> value.Type) Then
                    _mediumDescriptor = value
                    Medium = value.Type
                    OnPropertyChanged("MediumDescriptor")
                End If
            End Set
        End Property

        Public Property Medium As Book.MediaType
            Get
                Return Model.Medium
            End Get
            Set(value As Book.MediaType)
                If value <> Model.Medium Then
                    Model.Medium = value
                    IsModified = True
                    OnPropertyChanged("Medium")
                End If
            End Set
        End Property

        Public Property Storage As String
            Get
                Return Model.Storage
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.Storage) Then
                    Model.Storage = value
                    IsModified = True
                    OnPropertyChanged("Storage")
                End If
            End Set
        End Property

        Public Property BorrowedDate As String
            Get
                Return Model.BorrowedDate
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.BorrowedDate) Then
                    Model.BorrowedDate = value
                    IsModified = True
                    OnPropertyChanged("BorrowedDate")
                    If Not String.IsNullOrWhiteSpace(Model.BorrowedDate) Then
                        Dim dummy As DateTime
                        If Not DateTime.TryParse(Model.BorrowedDate, dummy) Then
                            AddError("BorrowedDate", App.Texts.GetString("ErrorInvalidDate"))
                        Else
                            RemoveErrors("BorrowedDate")
                        End If
                    Else
                        RemoveErrors("BorrowedDate")
                    End If
                End If
            End Set
        End Property

        Public Property BorrowedTo As String
            Get
                Return Model.BorrowedTo
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.BorrowedTo) Then
                    Model.BorrowedTo = value
                    IsModified = True
                    OnPropertyChanged("BorrowedTo")
                End If
            End Set
        End Property

        Public Property Published As String
            Get
                Return Model.Published
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.Published) Then
                    Model.Published = value
                    IsModified = True
                    OnPropertyChanged("Published")

                    If String.IsNullOrEmpty(value) Then
                        RemoveErrors("Published")
                    Else
                        Dim year As Integer
                        If Integer.TryParse(value, year) Then
                            RemoveErrors("Published")
                        Else
                            AddError("Published", App.Texts.GetString("ErrorInvalidPublished"))
                        End If
                    End If
                End If
            End Set
        End Property

        Public Property OCLCNo As String
            Get
                Return Model.OCLCNo
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.OCLCNo) Then
                    Model.OCLCNo = value
                    IsModified = True
                    OnPropertyChanged("OCLCNo")
                End If
            End Set
        End Property

        Public Property DLCNo As String
            Get
                Return Model.DLCNo
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.DLCNo) Then
                    Model.DLCNo = value
                    IsModified = True
                    OnPropertyChanged("DCLCNo")
                End If
            End Set
        End Property

        Public Property ISBN As String
            Get
                Return Model.ISBN
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.ISBN) Then
                    Model.ISBN = value
                    IsModified = True
                    OnPropertyChanged("ISBN")
                End If
            End Set
        End Property

        Public Property NBACN As String
            Get
                Return Model.NBACN
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.NBACN) Then
                    Model.NBACN = value
                    IsModified = True
                    OnPropertyChanged("NBACN")
                End If
            End Set
        End Property

        Public Property ASIN As String
            Get
                Return Model.ASIN
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.ASIN) Then
                    Model.ASIN = value
                    IsModified = True
                    OnPropertyChanged("ASIN")
                End If
            End Set
        End Property

        Public Property Url As String
            Get
                Return Model.Url
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.Url) Then
                    Model.Url = value
                    IsModified = True
                    OnPropertyChanged("Url")
                End If
            End Set
        End Property

        Public Property GoogleBooksUrl As String
            Get
                Return Model.GoogleBooksUrl
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.GoogleBooksUrl) Then
                    Model.GoogleBooksUrl = value
                    IsModified = True
                    OnPropertyChanged("GoogleBooksUrl")
                End If
            End Set
        End Property

        Protected Overrides Function ValidateAsyncOverride(propertyName As String) As Task
            If String.IsNullOrEmpty(Title) Then
                AddError("Title", App.Texts.GetString("ErrorInvalidTitle"))
            Else
                RemoveErrors("Title")
            End If

            If Not String.IsNullOrEmpty(Published) Then
                Dim year As Integer
                If Integer.TryParse(Published, year) Then
                    If year < 1000 Or year > Date.Now.Date.Year Then
                        AddError("Published", App.Texts.GetString("ErrorInvalidPublished"))
                    Else
                        RemoveErrors("Published")
                    End If
                End If
            End If

            Return MyBase.ValidateAsyncOverride(propertyName)
        End Function

    End Class

End Namespace
