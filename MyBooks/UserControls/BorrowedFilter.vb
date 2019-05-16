Imports MyBooks.App.ViewModels
Imports Telerik.Data.Core

Namespace Global.MyBooks.App.UserControls

    Public Class BorrowedFilter
        Implements IFilter

        Public Function PassesFilter(item As Object) As Boolean Implements IFilter.PassesFilter
            Dim entry = DirectCast(item, BookViewModel)
            Return Not String.IsNullOrEmpty(entry.BorrowedDate) OrElse Not String.IsNullOrEmpty(entry.BorrowedTo)
        End Function

    End Class

End Namespace
