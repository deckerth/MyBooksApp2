Imports MyBooks.Models

Namespace Global.MyBooks.Repository

    Public Interface IBookRepository

        Enum UpsertResult
            added
            updated
            skipped
        End Enum

        ' Returns all books. 
        Function GetAsync() As Task(Of IEnumerable(Of Book))

        ' Returns all books with a data field matching the start of the given string. 
        Function GetAsync(search As String) As Task(Of IEnumerable(Of Book))

        ' Returns the book with the given id. 
        Function GetAsync(id As Guid) As Task(Of Book)

        ' Returns a book with the given title, author, mediatype, storage. 
        Function GetAsync(title As String, author As String, mediatype As Book.MediaType, storage As String) As Task(Of Book)

        ' Adds a new book if it does not exist, updates the 
        ' existing book otherwise.
        Function Upsert(book As Book) As Task(Of UpsertResult)

        ' Deletes a book.
        Function DeleteAsync(id As Guid) As Task

    End Interface

End Namespace
