Imports MyBooks.Models

Namespace Global.MyBooks.Repository.Libraries

    Public Class KindleLibrary
        Inherits LibraryAccessBase

        Public Sub New()
            LibraryName = "Kindle"
        End Sub


        Public Sub ImportKindleLibrary(source As String)
            Dim xmlSource As XDocument = XDocument.Parse(source)
            Scan(xmlSource)
        End Sub

        Private Sub Scan(source As XDocument)

            '<?xml version="1.0" encoding="ISO-8859-1"?>
            '<response>
            '    <sync_time>2019-04-22T10:48:07+0000;softwareVersion:52064;SE:F;SC:F;ST:KS-15000000000002,Periodical-1555930087822,KB-15000000000632,</sync_time>
            '    <cache_metadata>
            '        <version>1</version>
            '    </cache_metadata>
            '    <add_update_list>
            '        <meta_data>
            '            <ASIN>B00G3CS1H8</ASIN>
            '            <title pronunciation="">Genua auf eigene Faust: Halbtagestour für Kreuzfahrer</title>
            '            <authors>
            '                <author pronunciation="">Falkner, Andreas</author>
            '            </authors>
            '            <publishers>
            '                <publisher>BookRix GmbH & Co. KG</publisher>
            '            </publishers>
            '            <publication_date>2013-10-22T00:00:00+0000</publication_date>
            '            <purchase_date>2018-08-08T17:39:15+0000</purchase_date>
            '            <textbook_type/>
            '            <cde_contenttype>EBOK</cde_contenttype>
            '            <content_type>application/x-mobipocket-ebook</content_type>
            '        </meta_data>
            '    </add_update_list>
            '</response>

            BookItems.Clear()

            For Each response As XElement In source.Elements().Where(Function(e) e.Name.LocalName = "response")
                For Each add_update_list As XElement In response.Elements().Where(Function(e) e.Name.LocalName = "add_update_list")
                    For Each meta_data As XElement In add_update_list.Elements().Where(Function(e) e.Name.LocalName = "meta_data")
                        Dim current As New Book()
                        current.Medium = Book.MediaType.EBook
                        For Each authors As XElement In meta_data.Elements().Where(Function(e) e.Name.LocalName = "authors")
                            For Each author As XElement In authors.Elements().Where(Function(e) e.Name.LocalName = "author")
                                If String.IsNullOrEmpty(current.Authors) Then
                                    current.Authors = author.Value
                                Else
                                    current.Authors = current.Authors + "; " + author.Value
                                End If
                            Next
                        Next
                        For Each asin As XElement In meta_data.Elements().Where(Function(e) e.Name.LocalName = "ASIN")
                            current.ASIN = asin.Value
                        Next
                        For Each publication_date As XElement In meta_data.Elements().Where(Function(e) e.Name.LocalName = "publication_date")
                            If Not String.IsNullOrEmpty(publication_date.Value) AndAlso publication_date.Value.Length >= 4 Then
                                current.Published = publication_date.Value.Substring(0, 4)
                            End If
                        Next
                        For Each title As XElement In meta_data.Elements().Where(Function(e) e.Name.LocalName = "title")
                            ' Do not store entries such as: <title pronunciation="">---------------</title>
                            If Not title.Value.StartsWith("---") Then
                                current.Title = title.Value
                                BookItems.Add(current)
                            End If
                        Next
                    Next
                Next
            Next

            NoOfEntries = BookItems.Count
            NoOfPages = 1
            CurrentPage = 1
        End Sub

        Public Overrides Function GetLink(item As Book) As Uri

            If item Is Nothing OrElse item.ASIN Is Nothing OrElse item.ASIN.Length = 0 Then
                Return Nothing
            End If

            Return New Uri("http://www.amazon-asin.com/asincheck/?product_id=" + item.ASIN)

        End Function

    End Class

End Namespace

