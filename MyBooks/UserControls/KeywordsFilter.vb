Imports MyBooks.App.ViewModels

Namespace Global.MyBooks.App.UserControls

    Public Class KeywordsFilter
        Inherits TextFilter

        Public Overrides Function PassesFilter(item As Object) As Boolean
            Dim entry = DirectCast(item, BookViewModel)
            Return entry.Keywords.Contains(TextPattern.ToUpperInvariant, StringComparison.InvariantCultureIgnoreCase)
        End Function
    End Class

End Namespace
