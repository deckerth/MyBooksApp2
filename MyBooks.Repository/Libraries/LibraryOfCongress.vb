Imports MyBooks.Models

Namespace Global.MyBooks.Repository.Libraries

    Public Class LibraryOfCongress
        Inherits LibraryAccessBase

        Public Sub New()

            LibraryName = "Library of Congress"

            serviceURIStr_A_T = "http://lx2.loc.gov:210/lcdb??version=1.1&recordSchema=marcxml&operation=searchRetrieve&query=dc.creator=AUTHOR%20and%20dc.title=TITLE&maximumRecords=HITS"
            serviceURIStr_A = "http://lx2.loc.gov:210/lcdb??version=1.1&recordSchema=marcxml&operation=searchRetrieve&query=dc.creator=AUTHOR&maximumRecords=HITS"
            serviceURIStr_T = "http://lx2.loc.gov:210/lcdb??version=1.1&recordSchema=marcxml&operation=searchRetrieve&query=dc.title=TITLE&maximumRecords=HITS"

        End Sub

        Public Overrides Function GetLink(item As Book) As Uri

            If item Is Nothing OrElse item.DLCNo Is Nothing OrElse item.DLCNo.Length = 0 Then
                Return Nothing
            End If

            Return New Uri("https://lccn.loc.gov/" + item.DLCNo)

        End Function

    End Class

End Namespace
