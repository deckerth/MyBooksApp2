Namespace Global.MyBooks.Repository.Libraries

    Public Class LibraryRegistry

        Private Shared _current As LibraryRegistry
        Public Shared ReadOnly Property Current As LibraryRegistry
            Get
                If _current Is Nothing Then
                    _current = New LibraryRegistry
                End If
                Return _current
            End Get
        End Property

        Private _libraries As New List(Of ILibraryAccess)
        Public ReadOnly Property Libraries As List(Of ILibraryAccess)
            Get
                Return _libraries
            End Get
        End Property

        Private _iSBNQueryLibrary As ILibraryAccess
        Public ReadOnly Property ISBNQueryLibrary As ILibraryAccess
            Get
                If _iSBNQueryLibrary Is Nothing Then
                    _iSBNQueryLibrary = New OCLCIsbnAccess
                End If
                Return _iSBNQueryLibrary
            End Get
        End Property

        Private isbnAccess As OCLCIsbnAccess = New OCLCIsbnAccess




        Public Function GetISBNQueryLibrary() As ILibraryAccess
            Return isbnAccess
        End Function

        Public Sub New()
            _libraries.Add(New DeutscheNationalbibliothek)
            _libraries.Add(New BibliotheksverbundBayern)
            _libraries.Add(New OnlineComputerLibraryCenter)
            _libraries.Add(New LibraryOfCongress)
        End Sub


    End Class

End Namespace
