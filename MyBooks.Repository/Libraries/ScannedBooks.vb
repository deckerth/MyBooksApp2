Imports System.IO
Imports MyBooks.Models

Namespace Global.MyBooks.Repository.Libraries

    Public Class ScannedBooks

        Private _iSBNCodes As New List(Of String)
        Public ReadOnly Property ISBNCodes As List(Of String)
            Get
                Return _iSBNCodes
            End Get
        End Property

        Public Sub ImportScannedBooks(source As String)
            Dim xmlSource As XDocument = XDocument.Parse(source)
            Scan(xmlSource)
        End Sub

        Private Sub Scan(source As XDocument)

            '<?xml version="1.0" encoding="UTF-8" standalone="true"?>
            '<scannedBooks>
            '  <book title="The little stranger" isbn="9783404167678"/>
            '  <book title="" isbn="9138912171929"/>
            '  <book title="Weber's Classics die besten Originalrezepte der Grill-Pioniere" isbn="9783833837784"/>
            '  <book title="William Shakespeare's Star Wars trilogy" isbn="9781594747915"/>
            '</scannedBooks>

            _iSBNCodes.Clear()

            For Each scannedBooks As XElement In source.Elements().Where(Function(e) e.Name.LocalName = "scannedBooks")
                For Each scannedBook As XElement In scannedBooks.Elements().Where(Function(e) e.Name.LocalName = "book")
                    For Each e In scannedBook.Attributes()
                        Select Case e.Name.LocalName
                            Case "isbn"
                                _iSBNCodes.Add(e.Value.ToString())
                        End Select
                    Next
                Next
            Next

        End Sub

    End Class

End Namespace
