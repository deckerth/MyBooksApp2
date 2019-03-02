Imports Microsoft.EntityFrameworkCore
Imports MyBooks.Models

Namespace Global.MyBooks.Repository.Sql

    Public Class SqlBookRepository
        Implements IBookRepository

        Private ReadOnly _db As MyBooksContext

        Public Sub New(db As MyBooksContext)
            _db = db
        End Sub

        Public Async Function GetAsync() As Task(Of IEnumerable(Of Book)) Implements IBookRepository.GetAsync
            Return Await _db.Books.AsNoTracking().ToListAsync()
        End Function

        Public Async Function GetAsync(search As String) As Task(Of IEnumerable(Of Book)) Implements IBookRepository.GetAsync

            Dim parameters As String() = search.Split(" ")
            Return Await _db.Books.Where(
                Function(x As Book) parameters.Any(Function(y As String) x.Title.Contains(y) Or
                                                                         x.Authors.Contains(y) Or
                                                                         x.Keywords.Contains(y) Or
                                                                         x.OriginalTitle.Contains(y) Or
                                                                         x.Storage.Contains(y))
                ).OrderByDescending(
                Function(x As Book) parameters.Count(Function(y As String) x.Title.Contains(y) Or
                                                                         x.Authors.Contains(y) Or
                                                                         x.Keywords.Contains(y) Or
                                                                         x.OriginalTitle.Contains(y) Or
                                                                         x.Storage.Contains(y))
                ).AsNoTracking().ToListAsync()

        End Function

        Public Async Function GetAsync(id As Guid) As Task(Of Book) Implements IBookRepository.GetAsync
            Return Await _db.Books.AsNoTracking().FirstOrDefaultAsync(Function(x As Book) x.Id = id)
        End Function

        Public Async Function Upsert(book As Book) As Task Implements IBookRepository.Upsert
            If Await GetAsync(book.Id) IsNot Nothing Then
                _db.Books.Update(book)
            Else
                Await _db.Books.AddAsync(book)
            End If
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function DeleteAsync(id As Guid) As Task Implements IBookRepository.DeleteAsync
            Dim toDelete As Book = Await GetAsync(id)
            If toDelete IsNot Nothing Then
                _db.Books.Remove(toDelete)
                Await _db.SaveChangesAsync()
            End If
        End Function

        Public Async Function SetBooks(books As List(Of Book)) As Task
            For Each b In _db.Books
                _db.Entry(b).State = EntityState.Deleted
            Next
            Await _db.SaveChangesAsync()
            _db.Books.AddRange(books)
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function AddBooks(books As List(Of Book)) As Task
            Dim saveRequired As Boolean

            For Each b In books
                If Await GetAsync(b.Id) IsNot Nothing Then
                    _db.Entry(b).State = EntityState.Deleted
                    saveRequired = True
                End If
            Next
            If saveRequired Then
                Await _db.SaveChangesAsync()
            End If
            _db.Books.AddRange(books)
            Await _db.SaveChangesAsync()
        End Function

    End Class

End Namespace
