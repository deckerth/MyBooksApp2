Namespace Global.MyBooks.Repository.Libraries

    Public Class OCLCIsbnAccess
        Inherits OnlineComputerLibraryCenter

        Protected Overrides Function GetInitialQueryURI() As Uri

            'Dim serviceURI As New Uri("http://classify.oclc.org/classify2/Classify?isbn=9781542047906")

            If searchISBN Is Nothing Then
                Return Nothing
            End If

            serviceURIStr = "http://classify.oclc.org/classify2/Classify?isbn=" + searchISBN

            Return New Uri(serviceURIStr)

        End Function

    End Class

End Namespace
