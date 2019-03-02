Imports System.IO
Imports MyBooks.Models
Imports OfficeOpenXml

Namespace Global.MyBooks.Repository.Sql

    Public Class SqlExportImportService
        Implements IImportExportService

        Private Const BooksWorkbook As String = "Books"
        Private Const AuthorsWorkbook As String = "Authors"
        Private Const KeywordsWorkbook As String = "Keywords"
        Private Const StoragesWorkbook As String = "Storages"

        Private Repository As SqlMyBooksRepository

        Public Sub New(repo As SqlMyBooksRepository)
            Repository = repo
        End Sub

        Private Async Function ImportBooksAsync(worksheet As ExcelWorksheet, ImportOption As IImportExportService.ImportOptions) As Task

            Dim rows = worksheet.Dimension.Rows
            Dim books As New List(Of Book)
            For i = 2 To rows
                Dim book As New Book With {
                    .Title = worksheet.Cells(i, 2).Value.ToString(),
                    .Authors = worksheet.Cells(i, 3).Value.ToString(),
                    .Keywords = worksheet.Cells(i, 4).Value.ToString(),
                    .Storage = worksheet.Cells(i, 6).Value.ToString(),
                    .BorrowedDate = worksheet.Cells(i, 7).Value.ToString(),
                    .BorrowedTo = worksheet.Cells(i, 8).Value.ToString(),
                    .Published = worksheet.Cells(i, 9).Value.ToString(),
                    .OCLCNo = worksheet.Cells(i, 10).Value.ToString(),
                    .ISBN = worksheet.Cells(i, 11).Value.ToString(),
                    .NBACN = worksheet.Cells(i, 12).Value.ToString(),
                    .Url = worksheet.Cells(i, 13).Value.ToString(),
                    .GoogleBooksUrl = worksheet.Cells(i, 14).Value.ToString()
                }
                Dim medium As String = worksheet.Cells(i, 5).Value.ToString()
                Select Case medium
                    Case "Book"
                        book.Medium = Book.MediaType.Book
                    Case "AudioBook"
                        book.Medium = Book.MediaType.AudioBook
                    Case "EBook"
                        book.Medium = Book.MediaType.EBook
                    Case Else
                        book.Medium = Book.MediaType.Undefined
                End Select
                books.Add(book)
            Next

            If ImportOption = IImportExportService.ImportOptions.ReplaceBooks Then
                Await DirectCast(Repository.Books, SqlBookRepository).SetBooks(books)
            Else
                Await DirectCast(Repository.Books, SqlBookRepository).AddBooks(books)
            End If

        End Function

        Private Async Function ExportBooksAsync(package As ExcelPackage) As Task

            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add(BooksWorkbook)

            'Add the headers

            worksheet.Cells(1, 2).Value = "Title"
            worksheet.Cells(1, 3).Value = "Authors"
            worksheet.Cells(1, 4).Value = "Keywords"
            worksheet.Cells(1, 5).Value = "Medium"
            worksheet.Cells(1, 6).Value = "Storage"
            worksheet.Cells(1, 7).Value = "BorrowedDate"
            worksheet.Cells(1, 8).Value = "BorrowedTo"
            worksheet.Cells(1, 9).Value = "Published"
            worksheet.Cells(1, 10).Value = "OCLCNo"
            worksheet.Cells(1, 11).Value = "ISBN"
            worksheet.Cells(1, 12).Value = "NBACN"
            worksheet.Cells(1, 13).Value = "Url"
            worksheet.Cells(1, 14).Value = "GoogleBooks"

            Dim books = Await Repository.Books.GetAsync()

            For i = 1 To books.Count
                Dim book = books(i - 1)
                worksheet.Cells(i + 1, 2).Value = book.Title
                worksheet.Cells(i + 1, 3).Value = book.Authors
                worksheet.Cells(i + 1, 4).Value = book.Keywords
                worksheet.Cells(i + 1, 6).Value = book.Storage
                worksheet.Cells(i + 1, 7).Value = book.BorrowedDate
                worksheet.Cells(i + 1, 8).Value = book.BorrowedTo
                worksheet.Cells(i + 1, 9).Value = book.Published
                worksheet.Cells(i + 1, 10).Value = book.OCLCNo
                worksheet.Cells(i + 1, 11).Value = book.ISBN
                worksheet.Cells(i + 1, 12).Value = book.NBACN
                worksheet.Cells(i + 1, 13).Value = book.Url
                worksheet.Cells(i + 1, 14).Value = book.GoogleBooksUrl

                Select Case book.Medium
                    Case Book.MediaType.Book
                        worksheet.Cells(i + 1, 5).Value = "Book"
                    Case Book.MediaType.AudioBook
                        worksheet.Cells(i + 1, 5).Value = "AudioBook"
                    Case Book.MediaType.EBook
                        worksheet.Cells(i + 1, 5).Value = "EBook"
                    Case Book.MediaType.Undefined
                        worksheet.Cells(i + 1, 5).Value = ""
                End Select
            Next

        End Function

        Private Async Function ImportAuthorsAsync(worksheet As ExcelWorksheet, ImportOption As IImportExportService.ImportOptions) As Task

            Dim rows = worksheet.Dimension.Rows
            Dim items As New List(Of Author)
            For i = 2 To rows
                Dim item As New Author With {
                    .Name = worksheet.Cells(i, 1).Value.ToString()
                }
                items.Add(item)
            Next

            If ImportOption = IImportExportService.ImportOptions.ReplaceBooks Then
                Await DirectCast(Repository.Authors, SqlAuthorRepository).SetAuthors(items)
            Else
                Await DirectCast(Repository.Authors, SqlAuthorRepository).AddAuthors(items)
            End If

        End Function

        Private Async Function ExportAuthorsAsync(package As ExcelPackage) As Task

            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add(AuthorsWorkbook)

            'Add the headers
            worksheet.Cells(1, 1).Value = "Name"

            Dim items = Await Repository.Authors.GetAsync()

            For i = 1 To items.Count
                Dim item = items(i - 1)
                worksheet.Cells(i + 1, 1).Value = item.Name
            Next

        End Function

        Private Async Function ImportKeywordsAsync(worksheet As ExcelWorksheet, ImportOption As IImportExportService.ImportOptions) As Task

            Dim rows = worksheet.Dimension.Rows
            Dim items As New List(Of Keyword)
            For i = 2 To rows
                Dim item As New Keyword With {
                    .Name = worksheet.Cells(i, 1).Value.ToString()
                }
                items.Add(item)
            Next

            If ImportOption = IImportExportService.ImportOptions.ReplaceBooks Then
                Await DirectCast(Repository.Keywords, SqlKeywordRepository).SetKeywords(items)
            Else
                Await DirectCast(Repository.Keywords, SqlKeywordRepository).AddKeywords(items)
            End If

        End Function

        Private Async Function ExportKeywordsAsync(package As ExcelPackage) As Task

            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add(KeywordsWorkbook)

            'Add the headers
            worksheet.Cells(1, 1).Value = "Name"

            Dim items = Await Repository.Keywords.GetAsync()

            For i = 1 To items.Count
                Dim item = items(i - 1)
                worksheet.Cells(i + 1, 1).Value = item.Name
            Next

        End Function

        Private Async Function ImportStoragesAsync(worksheet As ExcelWorksheet, ImportOption As IImportExportService.ImportOptions) As Task

            Dim rows = worksheet.Dimension.Rows
            Dim items As New List(Of Storage)
            For i = 2 To rows
                Dim item As New Storage With {
                    .Name = worksheet.Cells(i, 1).Value.ToString()
                }
                items.Add(item)
            Next

            If ImportOption = IImportExportService.ImportOptions.ReplaceBooks Then
                Await DirectCast(Repository.Storages, SqlStorageRepository).SetStorages(items)
            Else
                Await DirectCast(Repository.Storages, SqlStorageRepository).AddStorages(items)
            End If

        End Function

        Private Async Function ExportStoragesAsync(package As ExcelPackage) As Task

            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add(StoragesWorkbook)

            'Add the headers
            worksheet.Cells(1, 1).Value = "Name"

            Dim items = Await Repository.Storages.GetAsync()

            For i = 1 To items.Count
                Dim item = items(i - 1)
                worksheet.Cells(i + 1, 1).Value = item.Name
            Next

        End Function

        Public Async Function ExportAsync(OutputStream As Stream) As Task Implements IImportExportService.ExportAsync

            If OutputStream IsNot Nothing Then
                Dim package = New ExcelPackage(OutputStream)
                While package.Workbook.Worksheets.Count > 0
                    package.Workbook.Worksheets.Delete(0)
                End While
                Await ExportBooksAsync(package)
                Await ExportAuthorsAsync(package)
                Await ExportKeywordsAsync(package)
                Await ExportStoragesAsync(package)
                package.Save()
            End If

        End Function

        Public Async Function ImportAsync(InputStream As Stream, ImportOption As IImportExportService.ImportOptions) As Task Implements IImportExportService.ImportAsync

            If InputStream IsNot Nothing Then
                Using package = New ExcelPackage(InputStream)
                    Await ImportBooksAsync(package.Workbook.Worksheets(BooksWorkbook), ImportOption)
                    Await ImportAuthorsAsync(package.Workbook.Worksheets(AuthorsWorkbook), ImportOption)
                    Await ImportKeywordsAsync(package.Workbook.Worksheets(KeywordsWorkbook), ImportOption)
                    Await ImportStoragesAsync(package.Workbook.Worksheets(StoragesWorkbook), ImportOption)
                End Using
            End If
        End Function

    End Class

End Namespace
