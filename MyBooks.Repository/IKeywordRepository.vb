Imports MyBooks.Models

Namespace Global.MyBooks.Repository

    Public Interface IKeywordRepository

        ' Returns all keywords. 
        Function GetAsync() As Task(Of IEnumerable(Of Keyword))

        ' Returns all keywords with a data field matching the start of the given string. 
        Function GetAsync(search As String) As Task(Of IEnumerable(Of Keyword))

        ' Returns the keyword with the given id. 
        Function GetAsync(id As Guid) As Task(Of Keyword)

        ' Returns the keyword with the given text. 
        Function GetExactAsync(search As String) As Task(Of Keyword)

        ' Adds a new keyword if it does not exist
        Function Insert(word As Keyword) As Task
    End Interface

End Namespace
