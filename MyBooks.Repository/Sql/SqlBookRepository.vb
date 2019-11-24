Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.ChangeTracking
Imports MyBooks.ContextProvider
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

        Public Async Function GetAsync(title As String, author As String, mediatype As Book.MediaType, storage As String) As Task(Of Book) Implements IBookRepository.GetAsync
            Return Await _db.Books.AsNoTracking().FirstOrDefaultAsync(Function(x As Book) x.Title = title And x.Authors = author And x.Storage = storage And x.Medium = mediatype)
        End Function

        Private Async Function GetAsyncWithTracking(id As Guid) As Task(Of Book)
            Return Await _db.Books.FirstOrDefaultAsync(Function(x As Book) x.Id = id)
        End Function

        Private Async Function GetAsyncWithTracking(title As String, author As String, mediatype As Book.MediaType, storage As String) As Task(Of Book)
            Return Await _db.Books.FirstOrDefaultAsync(Function(x As Book) x.Title = title And x.Authors = author And x.Storage = storage And x.Medium = mediatype)
        End Function

        Public Async Function Upsert(book As Book) As Task(Of IBookRepository.UpsertResult) Implements IBookRepository.Upsert
            Dim result As IBookRepository.UpsertResult = IBookRepository.UpsertResult.skipped

            DisplayTrackedEntities(_db.ChangeTracker)

            Dim existing = Await GetAsyncWithTracking(book.Id)
            If existing IsNot Nothing Then
                Try
                    If existing.UpdateFrom(book) Then
                        _db.Books.Update(existing)
                        result = IBookRepository.UpsertResult.updated
                    End If
                Catch ex As Exception

                End Try
            Else
                existing = Await GetAsyncWithTracking(book.Title, book.Authors, book.Medium, book.Storage)
                If existing Is Nothing Then
                    Await _db.Books.AddAsync(book)
                    result = IBookRepository.UpsertResult.added
                Else
                    If existing.UpdateFrom(book) Then
                        _db.Books.Update(existing)
                        result = IBookRepository.UpsertResult.updated
                    End If
                End If
            End If
            Await _db.SaveChangesAsync()
            DisplayTrackedEntities(_db.ChangeTracker)
            Return result
        End Function

        Public Async Function DeleteAsync(id As Guid) As Task Implements IBookRepository.DeleteAsync
            Dim toDelete As Book = Await GetAsyncWithTracking(id)
            If toDelete IsNot Nothing Then
                _db.Books.Remove(toDelete)
                Await _db.SaveChangesAsync()
            End If
        End Function

        Public Async Function SetBooks(books As List(Of Book)) As Task(Of UpdateCounters)
            For Each b In _db.Books
                _db.Entry(b).State = EntityState.Deleted
            Next
            Try
                Await _db.SaveChangesAsync()
                _db.Books.AddRange(books)
                Await _db.SaveChangesAsync()
            Catch ex As Exception
                Return New UpdateCounters()
            End Try
            Return New UpdateCounters With {.Added = books.Count}
        End Function

        Public Async Function AddBooks(books As List(Of Book)) As Task(Of UpdateCounters)
            Dim counters As New UpdateCounters
            _db.StartMassUpdate()
            For Each b In books
                counters.Increment(Await Upsert(b))
            Next
            Await _db.EndMassUpdateModeAsync()
            Return counters
        End Function

        Private Shared LoggingActive As Boolean = False

        Private Sub DisplayTrackedEntities(changeTracker As ChangeTracker)
            If Not LoggingActive Then
                Return
            End If

            Debug.WriteLine("")
            Dim entries = changeTracker.Entries()
            For Each entry In entries
                Debug.WriteLine("Entity Name: {0}", entry.Entity.GetType().FullName)
                Debug.WriteLine("Status: {0}", entry.State)
            Next
            Debug.WriteLine("")
            Debug.WriteLine("---------------------------------------")
        End Sub
    End Class

End Namespace
