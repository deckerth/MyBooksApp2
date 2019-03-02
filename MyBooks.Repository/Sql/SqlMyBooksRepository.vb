Imports Microsoft.EntityFrameworkCore

Namespace Global.MyBooks.Repository.Sql

    Public Class SqlMyBooksRepository
        Implements IMyBooksRepository

        Private ReadOnly _dbOptions As DbContextOptions(Of MyBooksContext)

        Public Sub New(dbOptionsBuilder As DbContextOptionsBuilder(Of MyBooksContext))
            _dbOptions = dbOptionsBuilder.Options
            Using db As New MyBooksContext(_dbOptions)
                db.Database.EnsureCreated()
            End Using
        End Sub

        Public ReadOnly Property Books As IBookRepository Implements IMyBooksRepository.Books
            Get
                Return New SqlBookRepository(New MyBooksContext(_dbOptions))
            End Get
        End Property

        Public ReadOnly Property Authors As IAuthorRepository Implements IMyBooksRepository.Authors
            Get
                Return New SqlAuthorRepository(New MyBooksContext(_dbOptions))
            End Get
        End Property

        Public ReadOnly Property Keywords As IKeywordRepository Implements IMyBooksRepository.Keywords
            Get
                Return New SqlKeywordRepository(New MyBooksContext(_dbOptions))
            End Get
        End Property

        Public ReadOnly Property Storages As IStorageRepository Implements IMyBooksRepository.Storages
            Get
                Return New SqlStorageRepository(New MyBooksContext(_dbOptions))
            End Get
        End Property

        Public ReadOnly Property ImportExportService As IImportExportService Implements IMyBooksRepository.ImportExportService
            Get
                Return New SqlExportImportService(Me)
            End Get
        End Property
    End Class

End Namespace
