Imports MyBooks.App.Commands

Namespace Global.MyBooks.App.UserControls

    Public Class AdvancedAutoSuggestBoxEntryViewModel
        Implements INotifyPropertyChanged

        Public Class DeleteEntryEventArgs
            Public entry As AdvancedAutoSuggestBoxEntryViewModel

            Public Sub New(toDelete As AdvancedAutoSuggestBoxEntryViewModel)
                entry = toDelete
            End Sub
        End Class

        Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

        Public Event DeleteEntry(ByVal e As DeleteEntryEventArgs)

        Public Event SuggestionChosen(sender As AdvancedAutoSuggestBox, args As AutoSuggestBoxSuggestionChosenEventArgs)

        Protected Overridable Sub OnPropertyChanged(ByVal PropertyName As String)
            ' Raise the event, and make this procedure
            ' overridable, should someone want to inherit from
            ' this class and override this behavior:
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(PropertyName))
        End Sub

        Public Property Text As String
        Public Property RemoveCommand As RelayCommand
        Public Property Width As Integer
        Public Shared Property InputBoxWidth As Integer = 0 'Auto

        Public Sub New(aText As String)
            Text = aText
            RemoveCommand = New RelayCommand(AddressOf OnDelete)
            Width = InputBoxWidth - 25
        End Sub

        Private Sub OnDelete()
            RaiseEvent DeleteEntry(New DeleteEntryEventArgs(Me))
        End Sub

        Private _pointerEntered As Boolean = False
        Public Property PointerEntered As Boolean
            Get
                Return _pointerEntered
            End Get
            Set(value As Boolean)
                If value <> _pointerEntered Then
                    _pointerEntered = value
                    OnPropertyChanged("PointerEntered")
                End If
            End Set
        End Property

    End Class

End Namespace
