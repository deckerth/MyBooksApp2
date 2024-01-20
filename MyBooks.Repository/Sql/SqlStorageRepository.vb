Imports MyBooks.Models
Imports Microsoft.EntityFrameworkCore
Imports MyBooks.ContextProvider

Namespace Global.MyBooks.Repository.Sql

    Public Class SqlStorageRepository
        Implements IStorageRepository

        Private ReadOnly _db As MyBooksContext

        Public Sub New(db As MyBooksContext)
            _db = db
        End Sub

        Public Async Function GetAsync() As Task(Of IEnumerable(Of Storage)) Implements IStorageRepository.GetAsync
            Return Await _db.Storages.AsNoTracking().ToListAsync()
        End Function

        Public Async Function GetAsync(search As String) As Task(Of IEnumerable(Of Storage)) Implements IStorageRepository.GetAsync

            Dim parameters As String() = search.Split(" ")
            Return Await _db.Storages.Where(
                Function(x As Storage) parameters.Any(Function(y As String) x.Name.StartsWith(y, comparisonType:=StringComparison.InvariantCultureIgnoreCase))
                ).OrderByDescending(
                Function(x As Storage) parameters.Count(Function(y As String) x.Name.StartsWith(y, comparisonType:=StringComparison.InvariantCultureIgnoreCase))
                ).AsNoTracking().ToListAsync()

        End Function

        Public Async Function GetAsync(id As Guid) As Task(Of Storage) Implements IStorageRepository.GetAsync
            Return Await _db.Storages.AsNoTracking().FirstOrDefaultAsync(Function(x As Storage) x.Id = id)
        End Function

        Public Async Function GetExactAsync(search As String) As Task(Of Storage) Implements IStorageRepository.GetExactAsync
            Return Await _db.Storages.AsNoTracking().FirstOrDefaultAsync(Function(x As Storage) x.Name = search)
        End Function

        Public Async Function Insert(storage As Storage) As Task Implements IStorageRepository.Insert
            If Await GetExactAsync(storage.Name) Is Nothing Then
                Await _db.Storages.AddAsync(storage)
                Await _db.SaveChangesAsync()
            End If
        End Function

        Public Async Function SetStorages(storages As List(Of Storage)) As Task
            Await ClearAsync()
            _db.Storages.AddRange(storages)
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function AddStorages(items As List(Of Storage)) As Task
            _db.StartMassUpdate()
            For Each i In items
                Await Insert(i)
            Next
            Await _db.EndMassUpdateModeAsync()
        End Function

        Public Async Function ClearAsync() As Task Implements IStorageRepository.ClearAsync
            For Each b In _db.Storages
                _db.Entry(b).State = EntityState.Deleted
            Next
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function DeleteAsyncExact(search As String) As Task Implements IStorageRepository.DeleteAsyncExact
            Dim toDelete = Await _db.Storages.AsNoTracking().FirstOrDefaultAsync(Function(x As Storage) x.Name = search)
            If toDelete IsNot Nothing Then
                _db.Entry(toDelete).State = EntityState.Deleted
            End If
            Await _db.SaveChangesAsync()
        End Function
    End Class

End Namespace
