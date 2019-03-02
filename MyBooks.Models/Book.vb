Imports MARC

Namespace Global.MyBooks.Models

    Public Class Book
        Inherits DBObject
        Implements IEquatable(Of Book)

        Enum MediaType
            Book
            EBook
            AudioBook
            Undefined
        End Enum

        Public Sub New()
        End Sub

        Private Shared _newBookId As Guid = Guid.NewGuid()
        Public Shared ReadOnly Property NewBookId As Guid
            Get
                Return _newBookId
            End Get
        End Property

        Private Shared _allMediaTypes As List(Of MediaType)
        Public Shared ReadOnly Property AllMediaTypes As List(Of MediaType)
            Get
                If _allMediaTypes Is Nothing Then
                    _allMediaTypes = New List(Of MediaType)
                    _allMediaTypes.Add(MediaType.Book)
                    _allMediaTypes.Add(MediaType.EBook)
                    _allMediaTypes.Add(MediaType.AudioBook)
                End If
                Return _allMediaTypes
            End Get
        End Property

        Public Property Title As String = ""
        Public Property OriginalTitle As String = ""
        Public Property Authors As String = ""
        Public Property Keywords As String = ""
        Public Property Medium As MediaType
        Public Property Storage As String = ""
        'Public Property BorrowedDate As System.Nullable(Of DateTime)
        Public Property BorrowedDate As String = ""
        Public Property BorrowedTo As String = ""
        Public Property Published As String = ""
        Public Property OCLCNo As String = ""
        Public Property DLCNo As String = ""
        Public Property ISBN As String = ""
        Public Property NBACN As String = "" 'National Bibliographic Agency Control Number
        Public Property Url As String = ""
        Public Property GoogleBooksUrl As String = ""

        Public Function IEquatable_Equals(other As Book) As Boolean Implements IEquatable(Of Book).Equals
            Return Title.Equals(other.Title) AndAlso
                   OriginalTitle.Equals(other.OriginalTitle) AndAlso
                   Authors.Equals(other.Authors) AndAlso
                   Keywords.Equals(other.Keywords) AndAlso
                   Medium.Equals(other.Medium) AndAlso
                   Storage.Equals(other.Storage) AndAlso
                   BorrowedTo.Equals(other.BorrowedTo) AndAlso
                   BorrowedTo.Equals(other.BorrowedDate) AndAlso
                   Published.Equals(other.Published)
        End Function

        Public Function Clone() As Book
            Return DirectCast(MemberwiseClone(), Book)
        End Function

    End Class

End Namespace
