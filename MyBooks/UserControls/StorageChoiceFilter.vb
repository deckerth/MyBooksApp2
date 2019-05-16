Imports MyBooks.App.ViewModels

Namespace Global.MyBooks.App.UserControls

    Public Class StorageChoiceFilter
        Inherits TextChoiceFilter

        Public Sub New()
            PopulateChoices()
        End Sub

        Private Async Sub PopulateChoices()
            Dim storages = Await App.Repository.Storages.GetAsync()
            Dim list As New List(Of ChoiceCheckBox)
            For Each e In storages
                list.Add(New ChoiceCheckBox With {.Value = e.Name})
            Next
            SetChoicesFromList(list)
        End Sub

        Public Overrides Function PassesFilter(item As Object) As Boolean
            Dim entry = DirectCast(item, BookViewModel)
            Dim matches = Choices.Where(Function(x) x.IsChecked AndAlso entry.Storage.Contains(x.Value, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault()
            Return (matches IsNot Nothing)
        End Function
    End Class

End Namespace

