Imports MyBooks.App.Commands
Imports Telerik.Data.Core

Namespace Global.MyBooks.App.UserControls

    Public MustInherit Class TextChoiceFilter
        Implements IFilter

        Private _choices As New ObservableCollection(Of ChoiceCheckBox)
        Public ReadOnly Property Choices As ObservableCollection(Of ChoiceCheckBox)
            Get
                Return _choices
            End Get
        End Property

        Private _visibleChoices As New ObservableCollection(Of ChoiceCheckBox)
        Public ReadOnly Property VisibleChoices As ObservableCollection(Of ChoiceCheckBox)
            Get
                Return _visibleChoices
            End Get
        End Property

        Public Property TextPattern As String = ""

        Public Property SelectAllCommand As RelayCommand
        Public Property DeselectAllCommand As RelayCommand

        Public MustOverride Function PassesFilter(item As Object) As Boolean Implements IFilter.PassesFilter

        Public Sub New()
            InitalizeSelectionCommands()
        End Sub

        Protected Sub SetChoicesFromList(list As List(Of ChoiceCheckBox))
            list.Sort(New ChoiceComparer)
            Choices.Clear()
            For Each e In list
                Choices.Add(e)
                VisibleChoices.Add(e)
            Next
        End Sub

        Private Sub InitalizeSelectionCommands()
            SelectAllCommand = New RelayCommand(AddressOf OnSelectAll)
            DeselectAllCommand = New RelayCommand(AddressOf OnDeselectAll)
        End Sub

        Private Sub OnSelectAll()
            For Each e In VisibleChoices
                e.IsChecked = True
            Next
        End Sub

        Private Sub OnDeselectAll()
            For Each e In VisibleChoices
                e.IsChecked = False
            Next
        End Sub

        Public Sub ApplyTextPattern(textPattern As String)
            Dim pattern = textPattern.Trim().ToUpper()
            Dim visChoicesIndex As Integer = 0

            ' Update procedure
            '
            ' Choices    VisibleChoices   =>  VisibleChoices
            '
            '  a - add        a                a
            '  b              b
            '  c - add                         c
            '  d              d

            For Each t In Choices
                Dim add As Boolean
                If String.IsNullOrEmpty(pattern) Then
                    add = True
                ElseIf t.Value.ToUpperInvariant.Contains(pattern, StringComparison.InvariantCultureIgnoreCase) Then
                    add = True
                Else
                    add = False
                End If

                If add Then
                    If visChoicesIndex = VisibleChoices.Count OrElse Not VisibleChoices(visChoicesIndex).Equals(t) Then
                        VisibleChoices.Insert(visChoicesIndex, t)
                    End If
                    visChoicesIndex = visChoicesIndex + 1
                Else
                    If visChoicesIndex < VisibleChoices.Count AndAlso VisibleChoices(visChoicesIndex).Equals(t) Then
                        VisibleChoices.RemoveAt(visChoicesIndex)
                    End If
                End If
            Next
        End Sub

        Private Class ChoiceComparer
            Implements IComparer(Of ChoiceCheckBox)

            Public Function Compare(ByVal x As ChoiceCheckBox, ByVal y As ChoiceCheckBox) As Integer Implements IComparer(Of ChoiceCheckBox).Compare

                If x Is Nothing Then
                    If y Is Nothing Then
                        ' If x is Nothing and y is Nothing, they're
                        ' equal. 
                        Return 0
                    Else
                        ' If x is Nothing and y is not Nothing, y
                        ' is greater. 
                        Return -1
                    End If
                Else
                    ' If x is not Nothing...
                    '
                    If y Is Nothing Then
                        ' ...and y is Nothing, x is greater.
                        Return 1
                    Else
                        ' ...and y is not Nothing, compare the string
                        Return x.Value.CompareTo(y.Value)
                    End If
                End If
            End Function
        End Class


    End Class

End Namespace
