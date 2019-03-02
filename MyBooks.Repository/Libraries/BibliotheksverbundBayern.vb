Imports MyBooks.Models

Namespace Global.MyBooks.Repository.Libraries

    Public Class BibliotheksverbundBayern
        Inherits LibraryAccessBase

        Public Sub New()

            LibraryName = "Bibliotheksverbund Bayern"

            serviceURIStr_A_T = "http://bvbr.bib-bvb.de:5661/bvb01sru?version=1.1&recordSchema=marcxml&operation=searchRetrieve&query=marcxml.creator=AUTHOR%20and%20marcxml.title=TITLE&maximumRecords=HITS"
            serviceURIStr_A = "http://bvbr.bib-bvb.de:5661/bvb01sru?version=1.1&recordSchema=marcxml&operation=searchRetrieve&query=marcxml.creator=AUTHOR&maximumRecords=HITS"
            serviceURIStr_T = "http://bvbr.bib-bvb.de:5661/bvb01sru?version=1.1&recordSchema=marcxml&operation=searchRetrieve&query=marcxml.title=TITLE&maximumRecords=HITS"

        End Sub

        Public Overrides Function GetLink(item As Book) As Uri

            If item Is Nothing OrElse item.OCLCNo Is Nothing OrElse item.OCLCNo.Length = 0 Then
                Return Nothing
            End If

            Return New Uri("https://opacplus.bsb-muenchen.de/metaopac/search?oclcno=" + item.OCLCNo)

        End Function

    End Class

End Namespace
