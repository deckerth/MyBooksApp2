Imports System.IO
Imports System.Net
Imports System.Text
Imports MyBooks.Models
Imports MyBooks.Models.Book
Imports MyBooks.OCLCClassify

Namespace Global.MyBooks.Repository.Libraries

    Public Class OnlineComputerLibraryCenter
        Inherits LibraryAccessBase

        Public Sub New()

            LibraryName = "Online Computer Library Center (OCLC)"

            serviceURIStr_A_T = "http://classify.oclc.org/classify2/Classify?author=AUTHOR&title=TITLE&maxRecs=HITS&summary=true"
            serviceURIStr_A = "http://classify.oclc.org/classify2/Classify?author=AUTHOR&maxRecs=HITS&summary=true"
            serviceURIStr_T = "http://classify.oclc.org/classify2/Classify?title=TITLE&maxRecs=HITS&summary=true"

        End Sub

        Public Overrides Function GetLink(item As Book) As Uri

            If item Is Nothing OrElse item.OCLCNo Is Nothing OrElse item.OCLCNo.Length = 0 OrElse item.Title Is Nothing Then
                Return Nothing
            End If

            Dim transformedTitle As String
            transformedTitle = item.Title.ToLowerInvariant.Replace(" ", "-")

            'http://www.worldcat.org/title/harry-potter-and-the-chamber-of-secrets/oclc/978302357&referer=brief_results

            Return New Uri("http://www.worldcat.org/title/" + transformedTitle + "/oclc/" + item.OCLCNo + "&referer=brief_results")

        End Function

        Protected Overrides Sub ExecuteRequest(requestURI As Uri)

            BookItems.Clear()
            AudioItems.Clear()

            If requestURI Is Nothing Then
                Return
            End If

            Dim request As WebRequest = WebRequest.Create(requestURI)

            ' Get the response.
            Dim response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
            ' Get the stream containing content returned by the server.
            Dim stream As Stream = response.GetResponseStream()

            Dim source As String
            Using sr As StreamReader = New StreamReader(stream, Encoding.UTF8)
                source = sr.ReadToEnd()
            End Using

            Dim target = New FileOCLCXml(source)

            Dim enumerator As IEnumerator = target.GetEnumerator()
            enumerator.Reset()
            While enumerator.MoveNext()
                Dim current = DirectCast(enumerator.Current, Work)
                Dim bi As Book = OCLCClassifyConverter.Current.GetBookFromWork(current)
                If Not String.IsNullOrEmpty(searchISBN) And String.IsNullOrEmpty(bi.ISBN) Then
                    bi.ISBN = searchISBN
                End If
                If bi.Medium = MediaType.Book Or bi.Medium = MediaType.EBook Then
                    BookItems.Add(bi)
                ElseIf bi.Medium = MediaType.AudioBook Then
                    AudioItems.Add(bi)
                End If
            End While

            NoOfEntries = target.NumberOfEntries
            NoOfPages = 1
        End Sub

    End Class

End Namespace
