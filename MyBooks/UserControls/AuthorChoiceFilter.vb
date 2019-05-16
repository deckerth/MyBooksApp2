
Imports MyBooks.App.ViewModels

Namespace Global.MyBooks.App.UserControls

    Public Class AuthorChoiceFilter
        Inherits TextChoiceFilter

        Public Sub New()
            PopulateChoices()
        End Sub

        Private Async Sub PopulateChoices()
            Dim authors = Await App.Repository.Authors.GetAsync()
            Dim list As New List(Of ChoiceCheckBox)
            For Each a In authors
                list.Add(New ChoiceCheckBox With {.Value = a.Name})
            Next
            SetChoicesFromList(list)
        End Sub

        Public Overrides Function PassesFilter(item As Object) As Boolean
            Dim entry = DirectCast(item, BookViewModel)
            Dim matches = Choices.Where(Function(x) x.IsChecked AndAlso entry.Authors.Contains(x.Value.Trim, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault()
            Return (matches IsNot Nothing)
        End Function
    End Class

End Namespace
