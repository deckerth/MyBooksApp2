Imports MyBooks.Models

Namespace Global.MyBooks.Repository

    Public Interface IStorageRepository

        ' Returns all storages. 
        Function GetAsync() As Task(Of IEnumerable(Of Storage))

        ' Returns all storages with a data field matching the start of the given string. 
        Function GetAsync(search As String) As Task(Of IEnumerable(Of Storage))

        ' Returns the storage with the given id. 
        Function GetAsync(id As Guid) As Task(Of Storage)

        ' Returns the storage with the given name. 
        Function GetExactAsync(search As String) As Task(Of Storage)

        ' Adds a new storage if it does not exist
        Function Insert(storage As Storage) As Task
    End Interface

End Namespace
