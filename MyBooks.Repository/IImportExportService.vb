Imports System.IO

Namespace Global.MyBooks.Repository

    Public Interface IImportExportService

        Enum ImportOptions
            AddBooks
            ReplaceBooks
        End Enum

        Function ImportAsync(InputStream As Stream, ImportOption As ImportOptions) As Task
        Function ExportAsync(OutputStream As Stream) As Task

    End Interface

End Namespace
