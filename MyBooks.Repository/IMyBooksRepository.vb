Namespace Global.MyBooks.Repository

    Public Interface IMyBooksRepository

        ReadOnly Property Books As IBookRepository
        ReadOnly Property Keywords As IKeywordRepository
        ReadOnly Property Storages As IStorageRepository
        ReadOnly Property Authors As IAuthorRepository
        ReadOnly Property ImportExportService As IImportExportService

        Sub StartMassUpdate()
        Function EndMassUpdateAsync() As Task

    End Interface

End Namespace
