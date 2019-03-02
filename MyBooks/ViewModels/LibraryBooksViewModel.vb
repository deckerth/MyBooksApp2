Imports MyBooks.Models
Imports MyBooks.App.ValueComparers
Imports MyBooks.Repository.Libraries

Namespace Global.MyBooks.App.ViewModels

    Public Class LibraryBooksViewModel
        Inherits BindableBase

        Public Property BookItems As New ObservableCollection(Of BookBrowserViewModel)
        Public Property AudioItems As New ObservableCollection(Of BookBrowserViewModel)
        Public Property AllItems As New ObservableCollection(Of BookBrowserViewModel)
        Public Property NoOfEntries As Integer
        Public Property NoOfPages As Integer

        Private _displayModeHub As Boolean = False
        Public Property DisplayModeHub As Boolean
            Get
                Return _displayModeHub
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_displayModeHub, value)
            End Set
        End Property

        Private _currentPosition As String
        Public Property CurrentPosition As String
            Get
                Return _currentPosition
            End Get
            Set(value As String)
                SetProperty(Of String)(_currentPosition, value)
            End Set
        End Property

        Private _hasPreviousPage As Boolean
        Public Property HasPreviousPage As Boolean
            Get
                Return _hasPreviousPage
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_hasPreviousPage, value)
            End Set
        End Property

        Private _hasNextPage As Boolean
        Public Property HasNextPage As Boolean
            Get
                Return _hasNextPage
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_hasNextPage, value)
            End Set
        End Property

        Private Library As ILibraryAccess

        Public Sub SetItems(library As ILibraryAccess, Optional sort As Boolean = True)

            Me.Library = library
            Dim _comparer As IComparer(Of Book) = New BookComparer_TitleAscending

            If sort Then
                library.BookItems.Sort(_comparer)
                library.AudioItems.Sort(_comparer)
            End If

            Dim all As List(Of Book)
            all = library.BookItems
            all.AddRange(library.AudioItems)
            all.Sort(_comparer)

            AllItems.Clear()
            BookItems.Clear()
            AudioItems.Clear()

            For Each i In library.BookItems
                BookItems.Add(New BookBrowserViewModel(Me.Library, i))
            Next

            For Each i In library.AudioItems
                AudioItems.Add(New BookBrowserViewModel(Me.Library, i))
            Next

            For Each i In all
                AllItems.Add(New BookBrowserViewModel(Me.Library, i))
            Next
        End Sub

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

        Public Sub SetCurrentItem(item As BookBrowserViewModel)
            CurrentItem = item
            CurrentItem.InitializeBookLinks()
            OnPropertyChanged("CurrentItem")
            If CurrentItem.BrowserAdapter.BibItemUriIsValid Then
                CurrentItem.BrowserAdapter.CloneTo(AppShell.Current.ViewModel)
                AppShell.Current.ViewModel.BrowserPaneOpen = True
            End If
        End Sub

    End Class

End Namespace
