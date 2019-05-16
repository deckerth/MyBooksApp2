Imports MyBooks.App.ValueConverters
Imports MyBooks.App.ViewModels
Imports MyBooks.Models

Namespace Global.MyBooks.App.UserControls

    Public Class MediumChoiceFilter
        Inherits TextChoiceFilter

        Public Sub New()
            If allMediaTypes Is Nothing Then
                allMediaTypes = New List(Of MediumTypeDescriptor)
                For Each b In Book.AllMediaTypes
                    allMediaTypes.Add(New MediumTypeDescriptor(b))
                Next
            End If
            PopulateChoices()
        End Sub

        Private Shared allMediaTypes As List(Of MediumTypeDescriptor)

        Private Sub PopulateChoices()
            Dim list As New List(Of ChoiceCheckBox)
            For Each e In allMediaTypes
                list.Add(New ChoiceCheckBox With {.Value = e.Name})
            Next
            SetChoicesFromList(list)
        End Sub

        Public Overrides Function PassesFilter(item As Object) As Boolean
            Dim entry = DirectCast(item, BookViewModel)
            Dim mediumType = New MediumTypeToStringConverter()
            Dim typeName = mediumType.Convert(entry.Medium, GetType(String), Nothing, "")
            Dim matches = Choices.Where(Function(x) x.IsChecked AndAlso typeName.Equals(x.Value)).FirstOrDefault()
            Return (matches IsNot Nothing)
        End Function
    End Class

End Namespace

