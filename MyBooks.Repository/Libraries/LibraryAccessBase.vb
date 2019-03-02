Imports System.IO
Imports System.Net
Imports System.Text
Imports MARC
Imports MyBooks.Models
Imports MyBooks.Models.Book

Namespace Global.MyBooks.Repository.Libraries

    Public Class LibraryAccessBase
        Implements ILibraryAccess

        Protected DirectAccessUri As String
        Protected serviceURIStr_A_T As String
        Protected serviceURIStr_A As String
        Protected serviceURIStr_T As String

        Public Shared Current As ILibraryAccess

        Public Property LibraryName As String Implements ILibraryAccess.LibraryName

        Public Property BookItems As New List(Of Book) Implements ILibraryAccess.BookItems

        Public Property AudioItems As New List(Of Book) Implements ILibraryAccess.AudioItems

        Public Property NoOfEntries As Integer Implements ILibraryAccess.NoOfEntries

        Public Property NoOfPages As Integer Implements ILibraryAccess.NoOfPages

        Public Property CurrentPage As Integer Implements ILibraryAccess.CurrentPage

        Public ReadOnly Property HasNextPage As Boolean Implements ILibraryAccess.HasNextPage
            Get
                Return CurrentPage < NoOfPages
            End Get
        End Property

        Public ReadOnly Property HasPreviousPage As Boolean Implements ILibraryAccess.HasPreviousPage
            Get
                Return CurrentPage > 1
            End Get
        End Property

        Protected searchAuthor As String
        Protected searchTitle As String
        Protected searchISBN As String
        Private maxNoOfHits As Integer
        Private maxNoOfHitsStr As String
        Private startRecord As Integer = 0
        Protected serviceURIStr As String

        Public Sub New()
            Current = Me
        End Sub

        Public Sub SetSearchParameters(Optional author As String = "", Optional title As String = "", Optional isbn As String = "") Implements ILibraryAccess.SetSearchParameters

            If author IsNot Nothing AndAlso author.Length > 0 Then
                searchAuthor = Web.HttpUtility.UrlPathEncode(author)
            Else
                searchAuthor = Nothing
            End If

            If title IsNot Nothing AndAlso title.Length > 0 Then
                searchTitle = System.Web.HttpUtility.UrlEncode(title)
            Else
                searchTitle = Nothing
            End If

            If isbn IsNot Nothing AndAlso isbn.Length > 0 Then
                searchISBN = System.Web.HttpUtility.UrlEncode(isbn)
            Else
                searchISBN = Nothing
            End If

        End Sub

        Protected Overridable Function GetInitialQueryURI() As Uri

            'Dim serviceURI As New Uri("http://bvbr.bib-bvb.de:5661/bvb01sru?version=1.1&recordSchema=marcxml&operation=searchRetrieve&query=marcxml.creator=%22biagi,%20dario%22&maximumRecords=5")
            'Dim serviceURI As New Uri("https://services.dnb.de/sru/accessToken~728b3077d2fc28f6bd8fee2105b5ca/dnb?version=1.1&operation=searchRetrieve&query=per%3DDouglas%20Preston&recordSchema=MARC21-xml&maximumRecords=100")

            If searchTitle Is Nothing And searchAuthor Is Nothing Then
                Return Nothing
            End If

            If searchTitle Is Nothing Then
                serviceURIStr = serviceURIStr_A.Replace("AUTHOR", searchAuthor)
            ElseIf searchAuthor Is Nothing Then
                serviceURIStr = serviceURIStr_T.Replace("TITLE", searchTitle)
            Else
                serviceURIStr = serviceURIStr_A_T.Replace("AUTHOR", searchAuthor)
                serviceURIStr = serviceURIStr.Replace("TITLE", searchTitle)
            End If
            serviceURIStr = serviceURIStr.Replace("HITS", maxNoOfHits)

            Return New Uri(serviceURIStr)

        End Function

        Protected Function GetQueryURIForNextPage() As Uri

            If startRecord < NoOfEntries Then
                startRecord = startRecord + maxNoOfHits
                CurrentPage = CurrentPage + 1
                Return New Uri(serviceURIStr + "&startRecord=" + Convert.ToString(startRecord))
            Else
                Return Nothing
            End If

        End Function

        Protected Function GetQueryURIForPrevPage() As Uri

            If startRecord > 1 Then
                startRecord = startRecord - maxNoOfHits
                If startRecord < 1 Then
                    startRecord = 1
                End If
                CurrentPage = CurrentPage - 1
                If startRecord > 1 Then
                    Return New Uri(serviceURIStr + "&startRecord=" + Convert.ToString(startRecord))
                Else
                    Return New Uri(serviceURIStr)
                End If
            Else
                Return Nothing
            End If

        End Function

        Private Function ExecuteRequestForSinglePage(requestURI As Uri) As FileMARCXml

            If requestURI Is Nothing Then
                Return Nothing
            End If

            Dim request As WebRequest = WebRequest.Create(requestURI)

            ' Get the response.
            Dim response As HttpWebResponse
            Try
                response = CType(request.GetResponse(), HttpWebResponse)
            Catch ex As Exception
                Return Nothing
            End Try

            ' Get the stream containing content returned by the server.
            Dim stream As Stream = response.GetResponseStream()

            Dim source As String
            Using sr As StreamReader = New StreamReader(stream, Encoding.UTF8)
                source = sr.ReadToEnd()
            End Using

            Dim target As FileMARCXml = New FileMARCXml From {source}

            Dim enumerator As IEnumerator = target.GetEnumerator()
            enumerator.Reset()
            While enumerator.MoveNext()
                Dim current = DirectCast(enumerator.Current, Record)
                Dim bi As Book = MARCConverter.Current.GetBookFromRecord(current)
                If bi.Medium = MediaType.Book Then
                    BookItems.Add(bi)
                ElseIf bi.Medium = MediaType.AudioBook Then
                    AudioItems.Add(bi)
                End If
            End While

            NoOfEntries = target.NumberOfEntries
            NoOfPages = NoOfEntries \ maxNoOfHits
            If NoOfEntries Mod maxNoOfHits > 0 Then
                NoOfPages = NoOfPages + 1
            End If

            Return target
        End Function

        Protected Overridable Sub ExecuteRequest(requestURI As Uri)

            BookItems.Clear()
            AudioItems.Clear()

            If requestURI Is Nothing Then
                Return
            End If

            Dim currentUri As Uri = requestURI
            Dim currentStart = startRecord
            Dim collectedEntries = 0

            ' Some libraries have limits on the maximum number of hits. 
            ' If fewer results are returned execute further queries.
            Do
                Dim target As FileMARCXml = ExecuteRequestForSinglePage(currentUri)

                If target Is Nothing Then
                    Exit Do
                End If

                Dim entriesOnPage = target.NextRecordPosition - currentStart

                If entriesOnPage <= 0 Then
                    Exit Do ' The are no more results to expect
                End If

                collectedEntries = collectedEntries + entriesOnPage

                If collectedEntries > maxNoOfHits Then
                    ' More entries read than requested => correct start for next search
                    startRecord = startRecord + (collectedEntries - maxNoOfHits)
                    If startRecord > NoOfEntries And CurrentPage < NoOfPages Then
                        NoOfPages = CurrentPage
                    End If
                    Exit Do
                ElseIf collectedEntries = maxNoOfHits Then
                    Exit Do
                End If

                currentStart = currentStart + entriesOnPage
                currentUri = New Uri(serviceURIStr + "&startRecord=" + Convert.ToString(currentStart))
            Loop

        End Sub

        Public Overridable Sub ExecuteQuery(maxNumberOfHits As Integer) Implements ILibraryAccess.ExecuteQuery

            startRecord = 1
            CurrentPage = 1
            NoOfPages = 0
            NoOfEntries = 0

            If maxNumberOfHits > 0 Then
                maxNoOfHitsStr = Convert.ToString(maxNumberOfHits)
                maxNoOfHits = maxNumberOfHits
            Else
                maxNoOfHitsStr = "10"
                maxNoOfHits = 10
            End If

            ExecuteRequest(GetInitialQueryURI())

        End Sub

        Public Sub ReadNextPage() Implements ILibraryAccess.ReadNextPage

            ExecuteRequest(GetQueryURIForNextPage())

        End Sub

        Public Sub ReadPreviousPage() Implements ILibraryAccess.ReadPreviousPage

            ExecuteRequest(GetQueryURIForPrevPage())

        End Sub

        Public Overridable Function GetLink(item As Book) As Uri Implements ILibraryAccess.GetLink

            If item Is Nothing OrElse item.NBACN Is Nothing OrElse item.NBACN.Length = 0 Then
                Return Nothing
            End If

            Return New Uri(DirectAccessUri + item.NBACN)

        End Function

        Class IntWrapper
            Public Property Value As Integer
        End Class

        Public Function ExecuteQueryAsync(maxNumberOfHits As Integer) As Task Implements ILibraryAccess.ExecuteQueryAsync

            Return Task.Run(Sub()
                                ExecuteQuery(maxNumberOfHits)
                            End Sub)
        End Function

        Public Function ReadNextPageAsync() As Task Implements ILibraryAccess.ReadNextPageAsync

            Return Task.Run(Sub()
                                ReadNextPage()
                            End Sub)
        End Function

        Public Function ReadPreviousPageAsync() As Task Implements ILibraryAccess.ReadPreviousPageAsync

            Return Task.Run(Sub()
                                ReadPreviousPage()
                            End Sub)
        End Function

        Public Function GetGoogleBooksLink(item As Book) As Uri Implements ILibraryAccess.GetGoogleBooksLink

            If item.DLCNo Is Nothing OrElse item.DLCNo.Length = 0 Then
                If item.OCLCNo Is Nothing OrElse item.OCLCNo.Length = 0 Then
                    Return Nothing
                Else
                    Return New Uri("https://books.google.de/books?vid=OCLC" + item.OCLCNo)
                End If
            Else
                Return New Uri("https://books.google.de/books?vid=LCCN" + item.DLCNo)
            End If

        End Function

    End Class

End Namespace


