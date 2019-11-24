
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
            list.Add(New ChoiceCheckBox With {.Value = ""})
            SetChoicesFromList(list)
        End Sub

        Public Overrides Function PassesFilter(item As Object) As Boolean
            Dim entry = DirectCast(item, BookViewModel)
            Dim matches = Choices.Where(Function(x) x.IsChecked AndAlso
                                            If(String.IsNullOrEmpty(x.Value),
                                               String.IsNullOrEmpty(entry.Authors),
                                               entry.Authors.Contains(x.Value, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault()
            Return (matches IsNot Nothing)
        End Function
    End Class

End Namespace
