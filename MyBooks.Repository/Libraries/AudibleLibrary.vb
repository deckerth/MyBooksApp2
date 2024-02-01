Imports MyBooks.Models

Namespace Global.MyBooks.Repository.Libraries

    Public Class AudibleLibrary
        Inherits LibraryAccessBase

        Public Sub New()
            LibraryName = "Audible"
        End Sub



        Public Overrides Function GetLink(item As Book) As Uri

            If item Is Nothing OrElse item.Url Is Nothing Then
                Return Nothing
            End If

            Return New Uri(item.Url)

        End Function

    End Class

End Namespace
