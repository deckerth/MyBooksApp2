Imports System.IO
Imports MyBooks.Models

Namespace Global.MyBooks.Repository

    Public Interface IImportExportService

        Enum ImportOptions
            AddBooks
            ReplaceBooks
        End Enum

        Function ImportAsync(InputStream As Stream, ImportOption As ImportOptions) As Task(Of UpdateCounters)
        Function ExportAsync(OutputStream As Stream) As Task

        Sub ImportAudibleBooksAsync(InputStream As Stream, ByRef importedBooks As List(Of Book))

    End Interface

End Namespace
