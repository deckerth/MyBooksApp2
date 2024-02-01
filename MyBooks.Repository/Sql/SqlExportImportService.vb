Imports System.IO
Imports System.Net
Imports System.Text.RegularExpressions
Imports MyBooks.Models
Imports OfficeOpenXml
Imports OfficeOpenXml.FormulaParsing.Excel.Functions.Text
Imports Windows.Globalization.DateTimeFormatting

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

        Private Async Function ImportBooksAsync(worksheet As ExcelWorksheet, ImportOption As IImportExportService.ImportOptions) As Task(Of UpdateCounters)

            Dim rows = worksheet.Dimension.Rows
            Dim books As New List(Of Book)
            Dim counters As UpdateCounters
            For i = 2 To rows
                Dim errorFlag As Boolean = False
                Dim book As New Book
                Dim medium As String = ""
                Try
                    book.Title = worksheet.Cells(i, 2).Value.ToString()
                Catch ex As Exception
                    errorFlag = True
                End Try
                If Not errorFlag Then
                    Try
                        book.Authors = worksheet.Cells(i, 3).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        book.Keywords = worksheet.Cells(i, 4).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        book.Storage = worksheet.Cells(i, 6).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        book.BorrowedDate = worksheet.Cells(i, 7).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        book.BorrowedTo = worksheet.Cells(i, 8).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        book.Published = worksheet.Cells(i, 9).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        book.OCLCNo = worksheet.Cells(i, 10).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        book.ISBN = worksheet.Cells(i, 11).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        book.NBACN = worksheet.Cells(i, 12).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        book.Url = worksheet.Cells(i, 13).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        book.GoogleBooksUrl = worksheet.Cells(i, 14).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        medium = worksheet.Cells(i, 5).Value.ToString()
                    Catch ex As Exception
                    End Try
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
                End If
            Next

            Dim repo As SqlBookRepository = DirectCast(Repository.Books, SqlBookRepository)
            If ImportOption = IImportExportService.ImportOptions.ReplaceBooks Then
                counters = Await repo.SetBooks(books)
            Else
                counters = Await repo.AddBooks(books)
            End If
            Return counters
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

        Public Async Function ImportAsync(InputStream As Stream, ImportOption As IImportExportService.ImportOptions) As Task(Of UpdateCounters) Implements IImportExportService.ImportAsync
            Dim counters As New UpdateCounters

            If InputStream IsNot Nothing Then
                Using package = New ExcelPackage(InputStream)
                    counters = Await ImportBooksAsync(package.Workbook.Worksheets(BooksWorkbook), ImportOption)
                    Await ImportAuthorsAsync(package.Workbook.Worksheets(AuthorsWorkbook), ImportOption)
                    Await ImportKeywordsAsync(package.Workbook.Worksheets(KeywordsWorkbook), ImportOption)
                    Await ImportStoragesAsync(package.Workbook.Worksheets(StoragesWorkbook), ImportOption)
                End Using
            End If
            Return counters
        End Function

        Public Sub ImportAudibleBooksAsync(InputStream As Stream, ByRef importedBooks As List(Of Book)) Implements IImportExportService.ImportAudibleBooksAsync
            If InputStream IsNot Nothing Then
                Using package = New ExcelPackage(InputStream)
                    ImportAudibleBooksFromExcelAsync(package.Workbook.Worksheets(0), importedBooks)
                End Using
            End If
        End Sub

        Private Sub ImportAudibleBooksFromExcelAsync(worksheet As ExcelWorksheet, ByRef books As List(Of Book))

            ' 1 Added	
            ' 2 Cover	
            ' 3 Sample	
            ' 4 Web Player	
            ' 5 Search In Goodreads	
            ' 6 Title	
            ' 7 Title Short
            ' 8 Series	
            ' 9Book Numbers	
            '10 Blurb	
            '11 Authors	
            '12 Narrators	
            '13 Tags	
            '14 Categories	
            '15 Parent Category	
            '16 Child Category	
            '17 Length	
            '18 Progress	
            '19 Release Date	
            '20 Publishers	
            '21 My Rating	
            '22 Rating	
            '23 Ratings	
            '24 Favorite	
            '25 Format	
            '26 Language	
            '27 Whispersync	
            '28 From Plus Catalog	
            '29 Unavailable	
            '30 Archived	
            '31 Downloaded	
            '32 Store Page Changed	
            '33 Store Page Missing	
            '34 ASIN	
            '35 ISBN10	
            '36 ISBN13	
            '37 Summary	
            '38 People Also Bought	
            '39 Subtitle	
            '40 Collection Ids

            Dim rows = worksheet.Dimension.Rows
            books.Clear()
            For i = 2 To rows
                Dim errorFlag As Boolean = False
                Dim book As New Book
                Try
                    'Dim hyperlink = worksheet.Cells(i, 7).Value.ToString() 'Title Short

                    ''=HYPERLINK("https://audible.de/pd/B0857FDF48?ipRedirectOverride=true&overrideBaseCountry=true";"Der &Uuml;bergangsmanager")
                    ''https://regex101.com/
                    ''=HYPERLINK\("[a-zA-Z0-9\:\/\.\?\=\&]*";"([a-zA-Z0-9\:\/\.\?\=\&\;\s]*)"\)

                    'Dim pattern As String = "=HYPERLINK\(""[a-zA-Z0-9\:\/\.\?\=\&]*"";""(?<Title>[a-zA-Z0-9\:\/\.\?\=\&\;\s]*)""\)"

                    'Dim regex = New Regex(pattern)
                    'Dim match = regex.Match(hyperlink)
                    'If match.Success Then
                    '    book.Title = WebUtility.HtmlDecode(match.Groups("Title").Value)
                    'Else
                    '    errorFlag = True
                    'End If
                    book.Title = WebUtility.HtmlDecode(worksheet.Cells(i, 7).Value.ToString()) 'Title Short
                Catch ex As Exception
                    errorFlag = True
                End Try
                If Not errorFlag Then
                    Try
                        book.Authors = worksheet.Cells(i, 11).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        book.Keywords = worksheet.Cells(i, 8).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        book.Storage = "Audible"
                    Catch ex As Exception
                    End Try
                    Try
                        Dim val As Double = worksheet.Cells(i, 19).Value
                        Dim conv = DateTime.FromOADate(val)
                        Dim dto As DateTimeOffset = New DateTimeOffset(conv)
                        book.Published = DateTimeFormatter.ShortDate.Format(dto)
                    Catch ex As Exception
                    End Try
                    Try
                        book.ISBN = worksheet.Cells(i, 36).Value.ToString()
                    Catch ex As Exception
                    End Try
                    Try
                        'input: =HYPERLINK("https://www.goodreads.com/search?q=Andrzej%20Sapkowski%20-%20Die%20Dame%20vom%20See"; IMAGE("https://i.imgur.com/RPJRqNX.png"; 4; 20; 20))
                        Dim pattern As String = "=HYPERLINK\(""(?<Url>[a-zA-Z0-9\:\/\.\?\=\&\%\-]*)"";\s*IMAGE\(""[a-zA-Z0-9\:\/\.\?\=\&\;\s\%\-]*"";\s*[0-9]*;\s*[0-9]*;\s*[0-9]*\)\)"

                        Dim regex = New Regex(pattern)
                        Dim input = worksheet.Cells(i, 5).Value.ToString
                        Dim match = regex.Match(input)
                        If match.Success Then
                            book.Url = match.Groups("Url").Value
                        End If
                    Catch ex As Exception
                    End Try
                    book.Medium = Book.MediaType.AudioBook
                    books.Add(book)
                End If
            Next
        End Sub

    End Class

End Namespace
