Namespace Global.MyBooks.App.Navigation

    Public Class NavMenuItem

        Implements INotifyPropertyChanged

        Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

        Protected Overridable Sub OnPropertyChanged(ByVal PropertyName As String)
            ' Raise the event, and make this procedure
            ' overridable, should someone want to inherit from
            ' this class and override this behavior:
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(PropertyName))
        End Sub

        Public Property Label As String
        Public Property Symbol As Symbol
        Public Property PathIconMarkup As String
        Public Property HelpUri As Uri


        Public ReadOnly Property SymbolAsChar As Char
            Get
                Return Convert.ToChar(Symbol)
            End Get
        End Property

        Private _is_Selected As Boolean
        Public Property IsSelected As Boolean
            Get
                Return _is_Selected
            End Get
            Set(value As Boolean)
                _is_Selected = value
                If value Then
                    SelectedVis = Visibility.Visible
                Else
                    SelectedVis = Visibility.Collapsed
                End If
                OnPropertyChanged("IsSelected")
            End Set
        End Property

        Private _selectedVis As Visibility
        Public Property SelectedVis As Visibility
            Get
                Return _selectedVis
            End Get
            Set(value As Visibility)
                _selectedVis = value
                OnPropertyChanged("SelectedVis")
            End Set
        End Property

        Public Property DestPage As Type

        Public Property Arguments As Object

    End Class

End Namespace
