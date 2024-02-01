Imports MyBooks.Models
Imports MyBooks.App.ValueComparers
Imports MyBooks.Repository.Libraries
Imports Telerik.UI.Xaml.Controls.Grid
Imports MyBooks.App.Commands
Imports Microsoft.Toolkit.Uwp.Helpers

Namespace Global.MyBooks.App.ViewModels

    Public Class LibraryBooksViewModel
        Inherits BindableBase

        Public Property BookItems As New ObservableCollection(Of BookBrowserViewModel)
        Public Property AudioItems As New ObservableCollection(Of BookBrowserViewModel)
        Public Property AllItems As New ObservableCollection(Of BookBrowserViewModel)
        Public Property NoOfEntries As Integer
        Public Property NoOfPages As Integer

        Public Sub New()
            InitalizeSelectionCommands()
        End Sub

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

        Public Async Function SetItemsAsync(progress As ProgressRingViewModel, library As ILibraryAccess, Optional sort As Boolean = True) As Task
            Await Task.Run(Async Function() As Task
                               Await SetItemsTaskAsync(progress, library)
                           End Function)
        End Function

        Private Async Function SetItemsTaskAsync(progress As ProgressRingViewModel, library As ILibraryAccess, Optional sort As Boolean = True) As Task

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

            Await DispatcherHelper.ExecuteOnUIThreadAsync(Sub()
                                                              AllItems.Clear()
                                                              BookItems.Clear()
                                                              AudioItems.Clear()
                                                          End Sub)

            If progress IsNot Nothing Then
                Await progress.SetDeterministicAsync(all.Count)
            End If

            For Each i In all
                Dim browserModel = New BookBrowserViewModel(Me.Library, i)
                Await DispatcherHelper.ExecuteOnUIThreadAsync(Sub()
                                                                  AllItems.Add(browserModel)
                                                                  If i.Medium = Book.MediaType.AudioBook Then
                                                                      AudioItems.Add(browserModel)
                                                                  Else
                                                                      BookItems.Add(browserModel)
                                                                  End If
                                                              End Sub)
                If progress IsNot Nothing Then
                    Await progress.IncrementAsync(1)
                End If
            Next
        End Function

        Private _currentItem As BookBrowserViewModel
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

#Region "ItemSelection"
        Private _selectionMode As DataGridSelectionMode = DataGridSelectionMode.Single
        Public Property SelectionMode As DataGridSelectionMode
            Get
                Return _selectionMode
            End Get
            Set(value As DataGridSelectionMode)
                If value <> _selectionMode Then
                    SetProperty(Of DataGridSelectionMode)(_selectionMode, value)
                    MultipleSelectionMode = (value = DataGridSelectionMode.Multiple)
                End If
            End Set
        End Property

        Private _multipleSelectionMode As Boolean = False
        Public Property MultipleSelectionMode As Boolean
            Get
                Return _multipleSelectionMode
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_multipleSelectionMode, value)
            End Set
        End Property

        Public Property SelectAllCommand As RelayCommand
        Public Property DeselectAllCommand As RelayCommand
        Public EnableSingleSelectionModeCommand As RelayCommand
        Public EnableMultipleSelectionModeCommand As RelayCommand

        Private Sub InitalizeSelectionCommands()
            SelectAllCommand = New RelayCommand(AddressOf OnSelectAll)
            DeselectAllCommand = New RelayCommand(AddressOf OnDeselectAll)
            EnableMultipleSelectionModeCommand = New RelayCommand(AddressOf OnEnableMultipleSelectionMode)
            EnableSingleSelectionModeCommand = New RelayCommand(AddressOf OnEnableSingleSelectionMode)
        End Sub

        Public Property SelectedItems As ObservableCollection(Of Object)

        Public Event SelectAll()
        Public Event DeselectAll()

        Private Sub OnSelectAll()
            RaiseEvent SelectAll()
        End Sub

        Private Sub OnDeselectAll()
            RaiseEvent DeselectAll()
        End Sub

        Private Sub OnEnableSingleSelectionMode()
            SelectionMode = DataGridSelectionMode.Single
        End Sub

        Private Sub OnEnableMultipleSelectionMode()
            SelectionMode = DataGridSelectionMode.Multiple
        End Sub
#End Region

    End Class

End Namespace
