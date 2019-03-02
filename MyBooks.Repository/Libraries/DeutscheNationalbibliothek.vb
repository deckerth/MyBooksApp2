Namespace Global.MyBooks.Repository.Libraries

    Public Class DeutscheNationalbibliothek
        Inherits LibraryAccessBase

        Public Sub New()

            LibraryName = "Deutsche Nationalbibliothek"

            DirectAccessUri = "http://d-nb.info/"
            serviceURIStr_A_T = "https://services.dnb.de/sru/accessToken~728b3077d2fc28f6bd8fee2105b5ca/dnb?version=1.1&operation=searchRetrieve&query=per%3DAUTHOR%20and%20tit%3DTITLE&recordSchema=MARC21-xml&maximumRecords=HITS"
            serviceURIStr_A = "https://services.dnb.de/sru/accessToken~728b3077d2fc28f6bd8fee2105b5ca/dnb?version=1.1&operation=searchRetrieve&query=per%3DAUTHOR&recordSchema=MARC21-xml&maximumRecords=HITS"
            serviceURIStr_T = "https://services.dnb.de/sru/accessToken~728b3077d2fc28f6bd8fee2105b5ca/dnb?version=1.1&operation=searchRetrieve&query=tit%3DTITLE&recordSchema=MARC21-xml&maximumRecords=HITS"

        End Sub

    End Class

End Namespace


