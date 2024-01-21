Imports Microsoft.EntityFrameworkCore
Imports MyBooks.ContextProvider
Imports MyBooks.Models

Namespace Global.MyBooks.Repository.Sql

    Public Class SqlAuthorRepository
        Implements IAuthorRepository

        Private ReadOnly _db As MyBooksContext

        Public Sub New(db As MyBooksContext)
            _db = db
        End Sub

        Public Async Function GetAsync() As Task(Of IEnumerable(Of Author)) Implements IAuthorRepository.GetAsync
            Return Await _db.Authors.AsNoTracking().ToListAsync()
        End Function

        Public Async Function GetAsync(search As String) As Task(Of IEnumerable(Of Author)) Implements IAuthorRepository.GetAsync

            Dim parameters As String() = search.Split(" ")
            Return Await _db.Authors.Where(
                Function(x As Author) parameters.Any(Function(y As String) x.Name.StartsWith(y, comparisonType:=StringComparison.InvariantCultureIgnoreCase))
                ).OrderByDescending(
                Function(x As Author) parameters.Count(Function(y As String) x.Name.StartsWith(y, comparisonType:=StringComparison.InvariantCultureIgnoreCase))
                ).AsNoTracking().ToListAsync()

        End Function

        Public Async Function GetAsyncExact(search As String) As Task(Of Author) Implements IAuthorRepository.GetAsyncExact
            Return Await _db.Authors.AsNoTracking().FirstOrDefaultAsync(Function(x As Author) x.Name = search)
        End Function

        Public Async Function GetAsync(id As Guid) As Task(Of Author) Implements IAuthorRepository.GetAsync
            Return Await _db.Authors.AsNoTracking().FirstOrDefaultAsync(Function(x As Author) x.Id = id)
        End Function

        Public Async Function Insert(author As Author) As Task Implements IAuthorRepository.Insert
            If Await GetAsyncExact(author.Name) Is Nothing Then
                If author.Name.Contains("Carriger, Gail") Then
                    Dim x = 0
                End If
                Await _db.Authors.AddAsync(author)
                Await _db.SaveChangesAsync()
            End If
        End Function

        Public Async Function SetAuthors(authors As List(Of Author)) As Task
            Await ClearAsync()
            _db.Authors.AddRange(authors)
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function AddAuthors(authors As List(Of Author)) As Task
            _db.StartMassUpdate()
            For Each i In authors
                Await Insert(i)
            Next
            Await _db.EndMassUpdateModeAsync()
        End Function

        Public Async Function ClearAsync() As Task Implements IAuthorRepository.ClearAsync
            For Each b In _db.Authors
                _db.Entry(b).State = EntityState.Deleted
            Next
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function DeleteAsyncExact(search As String) As Task Implements IAuthorRepository.DeleteAsyncExact
            Dim toDelete = Await _db.Authors.AsNoTracking().FirstOrDefaultAsync(Function(x As Author) x.Name = search)
            If toDelete IsNot Nothing Then
                _db.Entry(toDelete).State = EntityState.Deleted
            End If
            Await _db.SaveChangesAsync()
        End Function

        Public Function GetCount() As Integer Implements IAuthorRepository.GetCount
            Return _db.Authors.Count
        End Function
    End Class

End Namespace
