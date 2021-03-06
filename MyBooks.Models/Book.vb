﻿'Imports MyBooks.MARC

Namespace Global.MyBooks.Models

    Public Class Book
        Inherits DBObject
        Implements IEquatable(Of Book)

        Enum MediaType
            Book
            EBook
            AudioBook
            KeepValue
            DeleteValue
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
        Public Property ASIN As String = "" 'Amazon article number
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

        Public Function GetAuthorList(useFirstNameLastNameFormat As Boolean) As List(Of String)
            Dim separators As Char()
            Dim result = New List(Of String)
            If useFirstNameLastNameFormat Then
                separators = New [Char]() {"/"c, ";"c, ","c}
            Else
                separators = New [Char]() {"/"c, ";"c}
            End If
            Dim list = Authors.Split(separators)
            If list.Count > 0 Then
                For Each a In list
                    If a IsNot Nothing Then
                        result.Add(a.Trim())
                    End If
                Next
            End If
            Return result
        End Function

        Public Function GetKeywordList() As List(Of String)
            Dim result = New List(Of String)
            Dim list = Keywords.Split(New [Char]() {"/"c, ";"c, ","c, CChar(vbTab)})
            If list.Count > 0 Then
                For Each k In list
                    If k IsNot Nothing Then
                        result.Add(k.Trim())
                    End If
                Next
            End If
            Return result
        End Function

        Private Function GetKeywordSeparator() As Char
            If Keywords.Contains(";"c) Then
                Return ";"c
            End If
            If Keywords.Contains("/"c) Then
                Return "/"c
            End If
            If Keywords.Contains(CChar(vbTab)) Then
                Return CChar(vbTab)
            End If
            Return ","c
        End Function

        Private Function MergeKeywords(otherBook As Book) As Boolean
            Dim changed As Boolean = False

            If Not String.IsNullOrEmpty(otherBook.Keywords) Then
                If String.IsNullOrEmpty(Keywords) Then
                    Keywords = otherBook.Keywords
                    changed = True
                ElseIf Not Keywords.Equals(otherBook.Keywords) Then
                    Dim keyworkList = GetKeywordList()
                    Dim otherList = otherBook.GetKeywordList()
                    Dim sep = GetKeywordSeparator()
                    For Each w In otherList
                        If keyworkList.Select(Of String)(Function(x As String) x.Equals(w)) Is Nothing Then
                            Keywords = Keywords + sep + " " + w
                            changed = True
                        End If
                    Next
                End If
            End If
            Return changed

        End Function

        'Public Property Title As String = ""
        'Public Property OriginalTitle As String = ""
        'Public Property Authors As String = ""
        'Public Property Keywords As String = ""
        'Public Property Medium As MediaType
        'Public Property Storage As String = ""
        'Public Property BorrowedDate As String = ""
        'Public Property BorrowedTo As String = ""
        'Public Property Published As String = ""
        'Public Property OCLCNo As String = ""
        'Public Property DLCNo As String = ""
        'Public Property ISBN As String = ""
        'Public Property NBACN As String = "" 'National Bibliographic Agency Control Number
        'Public Property ASIN As String = "" 'Amazon article number
        'Public Property Url As String = ""
        'Public Property GoogleBooksUrl As String = ""

        Private Function UpdateString(ByRef source As String, value As String) As Boolean
            Dim changed As Boolean = False

            If (String.IsNullOrEmpty(source) And Not String.IsNullOrEmpty(value)) OrElse
               (Not String.IsNullOrEmpty(source) And String.IsNullOrEmpty(value)) OrElse
               (Not source.Equals(value)) Then
                source = value
                changed = True
            End If

            Return changed
        End Function

        Public Function UpdateFrom(anotherBook As Book) As Boolean
            Dim changed As Boolean = False

            changed = UpdateString(Title, anotherBook.Title)
            changed = changed Or UpdateString(OriginalTitle, anotherBook.OriginalTitle)
            changed = changed Or UpdateString(Authors, anotherBook.Authors)
            changed = changed Or MergeKeywords(anotherBook)

            If Medium <> anotherBook.Medium Then
                Medium = anotherBook.Medium
                changed = True
            End If

            changed = changed Or UpdateString(Storage, anotherBook.Storage)
            changed = changed Or UpdateString(BorrowedDate, anotherBook.BorrowedDate)
            changed = changed Or UpdateString(BorrowedTo, anotherBook.BorrowedTo)
            changed = changed Or UpdateString(Published, anotherBook.Published)
            changed = changed Or UpdateString(OCLCNo, anotherBook.OCLCNo)
            changed = changed Or UpdateString(DLCNo, anotherBook.DLCNo)
            changed = changed Or UpdateString(ISBN, anotherBook.ISBN)
            changed = changed Or UpdateString(NBACN, anotherBook.NBACN)
            changed = changed Or UpdateString(ASIN, anotherBook.ASIN)
            changed = changed Or UpdateString(Url, anotherBook.Url)
            changed = changed Or UpdateString(GoogleBooksUrl, anotherBook.GoogleBooksUrl)

            Return changed
        End Function

    End Class

End Namespace
