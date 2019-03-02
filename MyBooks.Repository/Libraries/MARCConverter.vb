Imports MARC
Imports MyBooks.Models

Namespace Global.MyBooks.Repository.Libraries

    Public Class MARCConverter

        Private Shared _current As MARCConverter
        Public Shared ReadOnly Property Current As MARCConverter
            Get
                If _current Is Nothing Then
                    _current = New MARCConverter
                End If
                Return _current
            End Get
        End Property

        Public Function GetBookFromRecord(record As Record) As Book

            Dim book As New Book()

            For Each field In record.GetFields("")
                If field.IsDataField() Then
                    Dim dataField As DataField = DirectCast(field, DataField)
                    Select Case field.Tag
                        Case "010" 'Library of Congress Control Number 
                            If String.IsNullOrWhiteSpace(book.DLCNo) Then
                                book.DLCNo = ConcatenateSubFields(dataField, "a").Trim
                            End If
                        Case "016" 'National Bibliographic Agency Control Number
                            book.NBACN = ConcatenateSubFields(dataField, "a")
                        Case "035"
                            Dim v As String = ConcatenateSubFields(dataField, "a")
                            ' <subfield code="a">(OCoLC)641960569</subfield>
                            If v IsNot Nothing AndAlso v.StartsWith("(OCoLC)") Then
                                book.OCLCNo = v.Substring(7)
                            End If

                            If String.IsNullOrWhiteSpace(book.DLCNo) Then
                                v = ConcatenateSubFields(dataField, "9")
                                ' <subfield code="9">(DLC) 75046540</subfield>
                                If v IsNot Nothing AndAlso v.StartsWith("(DLC)") Then
                                    book.DLCNo = v.Substring(5).Trim()
                                End If
                            End If

                        Case "240"
                            book.OriginalTitle = ConcatenateSubFields(dataField, "a")
                        Case "245"
                            book.Title = ConcatenateSubFields(dataField, "a")
                            'SubTitle = ConcatenateSubFields(dataField, "b")
                            book.Authors = ConcatenateSubFields(dataField, "c")
                        'Case "250"
                        '    Version = ConcatenateSubFields(dataField, "a")
                        'Case "264"
                        '    Publisher = ConcatenateSubFields(dataField, "abc")
                        'Case "300"
                        '    Format = ConcatenateSubFields(dataField, "abc")
                        Case "263"
                            Dim yyyymm As String = ConcatenateSubFields(dataField, "a")
                            If yyyymm IsNot Nothing AndAlso yyyymm.Length = 6 Then
                                book.Published = yyyymm.Substring(0, 4)
                            End If
                        Case "264"
                            Dim yyyy As String = ConcatenateSubFields(dataField, "c")
                            If yyyy IsNot Nothing AndAlso yyyy.Length = 4 Then
                                book.Published = yyyy.Substring(0, 4)
                            End If
                        Case "260"
                            Dim yyyy As String = ConcatenateSubFields(dataField, "c")
                            If yyyy IsNot Nothing AndAlso yyyy.Length >= 4 Then
                                book.Published = ""
                                Dim matches = yyyy.Where(Function(c) ("0" <= c AndAlso c <= "9"))
                                For Each match In matches
                                    If book.Published.Length < 4 Then
                                        book.Published = book.Published + match
                                    End If
                                Next
                            End If
                        Case "337"
                            Dim code As String
                            code = ConcatenateSubFields(dataField, "b")
                            If code.Length = 0 OrElse code = "n" Then
                                book.Medium = Book.MediaType.Book
                            ElseIf code = "s" OrElse code = "c" Then
                                book.Medium = Book.MediaType.AudioBook
                            Else
                                book.Medium = Book.MediaType.Undefined
                            End If
                            'Case "338"
                            '    Medium = ConcatenateSubFields(dataField, "a")
                    End Select
                End If
            Next

            Return book
        End Function

        Private Function ConcatenateSubFields(dataField As DataField, tags As String) As String

            Dim MARCText As String = ""

            Dim commaFlag As Boolean = False
            For Each sf In dataField.GetSubfields()
                If tags.Contains(sf.Code) Then
                    If commaFlag Then
                        MARCText = MARCText + ", "
                    End If
                    MARCText = MARCText + sf.Data
                    commaFlag = True
                End If
            Next
            Return MARCText

        End Function

    End Class

End Namespace
