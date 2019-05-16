Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Design
Imports MyBooks.ContextProvider

Namespace Global.MyBooks.Repository.Sql

    Public Class SqlMyBooksRepository
        Implements IMyBooksRepository

        Private ReadOnly _dbOptions As DbContextOptions(Of MyBooksContext)
        Private ReadOnly _context As MyBooksContext

        Public Sub New(dbOptionsBuilder As DbContextOptionsBuilder(Of MyBooksContext))
            _dbOptions = dbOptionsBuilder.Options
            Using db As New MyBooksContext(_dbOptions)
                db.Database.Migrate()
            End Using
            _context = New MyBooksContext(_dbOptions)
        End Sub

        Private _books As IBookRepository = Nothing
        Public ReadOnly Property Books As IBookRepository Implements IMyBooksRepository.Books
            Get
                If _books Is Nothing Then
                    _books = New SqlBookRepository(_context)
                End If
                Return _books
            End Get
        End Property

        Private _authors As IAuthorRepository = Nothing
        Public ReadOnly Property Authors As IAuthorRepository Implements IMyBooksRepository.Authors
            Get
                If _authors Is Nothing Then
                    _authors = New SqlAuthorRepository(_context)
                End If
                Return _authors
            End Get
        End Property

        Private _keywords As IKeywordRepository = Nothing
        Public ReadOnly Property Keywords As IKeywordRepository Implements IMyBooksRepository.Keywords
            Get
                If _keywords Is Nothing Then
                    _keywords = New SqlKeywordRepository(_context)
                End If
                Return _keywords
            End Get
        End Property

        Private _storages As IStorageRepository = Nothing
        Public ReadOnly Property Storages As IStorageRepository Implements IMyBooksRepository.Storages
            Get
                If _storages Is Nothing Then
                    _storages = New SqlStorageRepository(_context)
                End If
                Return _storages
            End Get
        End Property

        Public ReadOnly Property ImportExportService As IImportExportService Implements IMyBooksRepository.ImportExportService
            Get
                Return New SqlExportImportService(Me)
            End Get
        End Property

        Public Sub StartMassUpdate() Implements IMyBooksRepository.StartMassUpdate
            _context.StartMassUpdate()
        End Sub

        Public Async Function EndMassUpdateAsync() As Task Implements IMyBooksRepository.EndMassUpdateAsync
            Await _context.EndMassUpdateModeAsync()
        End Function

    End Class

End Namespace
