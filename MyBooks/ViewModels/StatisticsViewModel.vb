Namespace Global.MyBooks.App.ViewModels

    Public Class StatisticsViewModel
        Inherits BindableBase

        Private Shared _current As StatisticsViewModel
        Public Shared ReadOnly Property Current As StatisticsViewModel
            Get
                If _current Is Nothing Then
                    _current = New StatisticsViewModel
                End If
                Return _current
            End Get
        End Property

        Private _statusText As String = ""
        Public Property StatusText As String
            Get
                Return _statusText
            End Get
            Set(value As String)
                SetProperty(Of String)(_statusText, value)
            End Set
        End Property

        Private NoOfBooks As Integer = 0
        Private NoOfPrintMedia As Integer = 0
        Private NoOfEbooks As Integer = 0
        Private NoOfAudioBooks As Integer = 0

        Public Sub Reset()
            NoOfAudioBooks = 0
            NoOfPrintMedia = 0
            NoOfAudioBooks = 0
            NoOfEbooks = 0
        End Sub

        Public Sub AddBook(book As Models.Book, Optional automaticRendering As Boolean = True)
            Update(book, 1, automaticRendering)
        End Sub

        Public Sub DeleteBook(book As Models.Book, Optional automaticRendering As Boolean = True)
            Update(book, -1, automaticRendering)
        End Sub

        Private Sub Update(book As Models.Book, delta As Integer, automaticRendering As Boolean)
            NoOfBooks += delta
            Select Case book.Medium
                Case Models.Book.MediaType.AudioBook
                    NoOfAudioBooks += delta
                Case Models.Book.MediaType.EBook
                    NoOfEbooks += delta
                Case Else
                    NoOfPrintMedia += delta
            End Select
            If automaticRendering Then
                RenderState()
            End If
        End Sub

        Public Sub RenderState()
            Dim state = "&1 Einträge: &2 Bücher  |  &3 E-Books  |  &4 Hörbücher  |  &5 Autoren"
            state = state.Replace("&1", NoOfBooks.ToString)
            state = state.Replace("&2", NoOfPrintMedia.ToString)
            state = state.Replace("&3", NoOfEbooks.ToString)
            state = state.Replace("&4", NoOfAudioBooks.ToString)
            state = state.Replace("&5", App.Repository.Authors.GetCount().ToString)
            StatusText = state
        End Sub

    End Class

End Namespace
