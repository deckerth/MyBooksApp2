Imports MyBooks.Models

Namespace Global.MyBooks.Repository.Libraries

    Public Interface ILibraryAccess

        Property LibraryName As String

        Property BookItems As List(Of Book)
        Property AudioItems As List(Of Book)
        Property NoOfEntries As Integer
        Property NoOfPages As Integer
        Property CurrentPage As Integer
        ReadOnly Property HasNextPage As Boolean
        ReadOnly Property HasPreviousPage As Boolean

        Sub SetSearchParameters(Optional author As String = "", Optional title As String = "", Optional isbn As String = "")

        Sub ExecuteQuery(maxNumberOfHits As Integer)
        Function ExecuteQueryAsync(maxNumberOfHits As Integer) As Task

        Sub ReadNextPage()
        Function ReadNextPageAsync() As Task

        Sub ReadPreviousPage()
        Function ReadPreviousPageAsync() As Task

        Function GetLink(item As Book) As Uri
        Function GetGoogleBooksLink(item As Book) As Uri
    End Interface

End Namespace
