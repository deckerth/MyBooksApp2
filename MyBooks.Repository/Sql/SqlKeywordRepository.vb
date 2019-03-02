Imports MyBooks.Models
Imports Microsoft.EntityFrameworkCore

Namespace Global.MyBooks.Repository.Sql

    Public Class SqlKeywordRepository
        Implements IKeywordRepository

        Private ReadOnly _db As MyBooksContext

        Public Sub New(db As MyBooksContext)
            _db = db
        End Sub

        Public Async Function GetAsync() As Task(Of IEnumerable(Of Keyword)) Implements IKeywordRepository.GetAsync
            Return Await _db.Keywords.AsNoTracking().ToListAsync()
        End Function

        Public Async Function GetAsync(search As String) As Task(Of IEnumerable(Of Keyword)) Implements IKeywordRepository.GetAsync

            Dim parameters As String() = search.Split(" ")
            Return Await _db.Keywords.Where(
                Function(x As Keyword) parameters.Any(Function(y As String) x.Name.StartsWith(y, comparisonType:=StringComparison.InvariantCultureIgnoreCase))
                ).OrderByDescending(
                Function(x As Keyword) parameters.Count(Function(y As String) x.Name.StartsWith(y, comparisonType:=StringComparison.InvariantCultureIgnoreCase))
                ).AsNoTracking().ToListAsync()

        End Function

        Public Async Function GetAsync(id As Guid) As Task(Of Keyword) Implements IKeywordRepository.GetAsync
            Return Await _db.Keywords.AsNoTracking().FirstOrDefaultAsync(Function(x As Keyword) x.Id = id)
        End Function

        Public Async Function GetExactAsync(search As String) As Task(Of Keyword) Implements IKeywordRepository.GetExactAsync
            Return Await _db.Keywords.AsNoTracking().FirstOrDefaultAsync(Function(x As Keyword) x.Name = search)
        End Function

        Public Async Function Insert(word As Keyword) As Task Implements IKeywordRepository.Insert
            If Await GetExactAsync(word.Name) Is Nothing Then
                Await _db.Keywords.AddAsync(word)
                Await _db.SaveChangesAsync()
            End If
        End Function

        Public Async Function SetKeywords(keywords As List(Of Keyword)) As Task
            For Each b In _db.Keywords
                _db.Entry(b).State = EntityState.Deleted
            Next
            Await _db.SaveChangesAsync()
            _db.Keywords.AddRange(keywords)
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function AddKeywords(items As List(Of Keyword)) As Task
            Dim saveRequired As Boolean

            For Each i In items
                If Await GetAsync(i.Id) IsNot Nothing Then
                    _db.Entry(i).State = EntityState.Deleted
                    saveRequired = True
                End If
            Next
            If saveRequired Then
                Await _db.SaveChangesAsync()
            End If
            _db.Keywords.AddRange(items)
            Await _db.SaveChangesAsync()
        End Function

    End Class

End Namespace
