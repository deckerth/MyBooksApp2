Imports MyBooks.Models
Imports MyBooks.OCLCClassify

Namespace Global.MyBooks.Repository.Libraries

    Public Class OCLCClassifyConverter

        Private Shared _current As OCLCClassifyConverter
        Public Shared ReadOnly Property Current As OCLCClassifyConverter
            Get
                If _current Is Nothing Then
                    _current = New OCLCClassifyConverter
                End If
                Return _current
            End Get
        End Property

        Public Function GetBookFromWork(work As Work) As Book

            Dim book As New Book With {
            .Authors = work.Author,
            .Title = work.Title,
            .Published = work.PublishYear,
            .OCLCNo = work.OCLCNo
            }

            Select Case work.ItemType
                Case "itemtype-book"
                    book.Medium = Book.MediaType.Book
                Case "itemtype-book-digital"
                    book.Medium = Book.MediaType.EBook
                Case Else
                    If work.ItemType IsNot Nothing AndAlso work.ItemType.StartsWith("itemtype-audiobook") Then
                        book.Medium = Book.MediaType.AudioBook
                    Else
                        book.Medium = Book.MediaType.Undefined
                    End If
            End Select

            Return book
        End Function

    End Class

End Namespace
