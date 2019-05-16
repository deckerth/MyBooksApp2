Imports MyBooks.App.ViewModels

Namespace Global.MyBooks.App.UserControls

    Public Class StorageFilter
        Inherits TextFilter

        Public Overrides Function PassesFilter(item As Object) As Boolean
            Dim entry = DirectCast(item, BookViewModel)
            Return entry.Storage.Contains(TextPattern.ToUpperInvariant, StringComparison.InvariantCultureIgnoreCase)
        End Function
    End Class

End Namespace
