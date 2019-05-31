Imports Microsoft.Toolkit.Uwp.Helpers

Namespace Global.MyBooks.App.ViewModels

    Public Class ProgressRingViewModel
        Inherits BindableBase

        Public Shared indeterministic As Integer = -1

        Private _active As Boolean = False
        Public Property Active As Boolean
            Get
                Return _active
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_active, value, "Active")
            End Set
        End Property

        Private _value As Integer

        Private _booksProcessed As Integer = indeterministic
        Public Property BooksProcessed As Integer
            Get
                Return _booksProcessed
            End Get
            Set(value As Integer)
                SetProperty(Of Integer)(_booksProcessed, value)
                _value = value
                If _booksProcessed = indeterministic Then
                    ProgressString = App.Texts.GetString("SearchProgressRing.Content")
                Else
                    ProgressString = _booksProcessed.ToString() + " / " + TotalNumberOfBooks.ToString() + " " + App.Texts.GetString("Titles")
                    End If
            End Set
        End Property

        Private _progressString As String = App.Texts.GetString("SearchProgressRing.Content")
        Public Property ProgressString As String
            Get
                Return _progressString
            End Get
            Set(value As String)
                SetProperty(Of String)(_progressString, value, "ProgressString")
            End Set
        End Property

        Public Property TotalNumberOfBooks As Integer = 0

        Public Sub Increment(delta As Integer)
            _value = _value + delta
            If _value Mod 10 = 0 Then
                BooksProcessed = _value
            End If
        End Sub

        Public Async Function IncrementAsync(delta As Integer) As Task
            Await DispatcherHelper.ExecuteOnUIThreadAsync(Sub() Increment(delta))
        End Function

        Public Sub SetDeterministic(total As Integer)
            BooksProcessed = 0
            TotalNumberOfBooks = total
            Active = True
        End Sub

        Public Async Function SetDeterministicAsync(total As Integer) As Task
            Await DispatcherHelper.ExecuteOnUIThreadAsync(Sub() SetDeterministic(total))
            If AppShell.Current IsNot Nothing AndAlso AppShell.Current.AppViewModel IsNot Nothing Then
                Await AppShell.Current.AppViewModel.SetNavigationAllowedAsync(False)
            End If
        End Function

        Public Sub SetIndeterministic()
            BooksProcessed = indeterministic
            Active = True
        End Sub

        Public Async Function SetIndeterministicAsync() As Task
            Await DispatcherHelper.ExecuteOnUIThreadAsync(Sub() SetIndeterministic())
            If AppShell.Current IsNot Nothing AndAlso AppShell.Current.AppViewModel IsNot Nothing Then
                Await AppShell.Current.AppViewModel.SetNavigationAllowedAsync(False)
            End If
        End Function

        Private Sub Hide()
            Active = False
        End Sub

        Public Async Function HideAsync() As Task
            Await DispatcherHelper.ExecuteOnUIThreadAsync(Sub() Hide())
            Await AppShell.Current.AppViewModel.SetNavigationAllowedAsync(True)
        End Function

    End Class

End Namespace
