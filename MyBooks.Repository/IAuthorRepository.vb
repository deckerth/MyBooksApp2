Imports MyBooks.Models

Namespace Global.MyBooks.Repository

    Public Interface IAuthorRepository

        ' Returns all authors. 
        Function GetAsync() As Task(Of IEnumerable(Of Author))

        ' Returns all authors with a data field matching the start of the given string. 
        Function GetAsync(search As String) As Task(Of IEnumerable(Of Author))

        ' Returns the author with the given id. 
        Function GetAsync(id As Guid) As Task(Of Author)

        ' Returns the author with the given name. 
        Function GetAsyncExact(search As String) As Task(Of Author)

        ' Adds a new author if it does not exist
        Function Insert(author As Author) As Task

    End Interface

End Namespace
