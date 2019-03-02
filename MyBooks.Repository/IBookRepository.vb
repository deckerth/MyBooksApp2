Imports MyBooks.Models

Namespace Global.MyBooks.Repository

    Public Interface IBookRepository

        ' Returns all books. 
        Function GetAsync() As Task(Of IEnumerable(Of Book))

        ' Returns all books with a data field matching the start of the given string. 
        Function GetAsync(search As String) As Task(Of IEnumerable(Of Book))

        ' Returns the book with the given id. 
        Function GetAsync(id As Guid) As Task(Of Book)

        ' Adds a new book if it does not exist, updates the 
        ' existing book otherwise.
        Function Upsert(book As Book) As Task

        ' Deletes a book.
        Function DeleteAsync(id As Guid) As Task

    End Interface

End Namespace
