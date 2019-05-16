Imports MyBooks.App.ViewModels
Imports Telerik.Data.Core

Namespace Global.MyBooks.App.UserControls

    Public MustInherit Class TextFilter
        Implements IFilter

        Public Property TextPattern As String = ""

        Public MustOverride Function PassesFilter(item As Object) As Boolean Implements IFilter.PassesFilter
    End Class

End Namespace
