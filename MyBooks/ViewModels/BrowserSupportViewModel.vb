Imports MyBooks.App.Commands
Imports MyBooks.Models

Namespace Global.MyBooks.App.ViewModels

    Public Class BrowserSupportViewModel
        Inherits BindableBase

        Public Property DisplayGoogleBooksCommand As RelayCommand
        Public Property DisplayStandardLibCommand As RelayCommand

        Private _bibItemGoogleBooksUri As Uri
        Public Property BibItemGoogleBooksUri As Uri
            Get
                Return _bibItemGoogleBooksUri
            End Get
            Set(value As Uri)
                SetProperty(Of Uri)(_bibItemGoogleBooksUri, value)
                _isGoogleBooksItemAvailable = (_bibItemGoogleBooksUri IsNot Nothing)
            End Set
        End Property

        Private _bibItemLibraryUri As Uri
        Public Property BibItemLibraryUri As Uri
            Get
                Return _bibItemLibraryUri
            End Get
            Set(value As Uri)
                SetProperty(Of Uri)(_bibItemLibraryUri, value)
            End Set
        End Property

        Public Sub New()
            DisplayGoogleBooksCommand = New RelayCommand(AddressOf OnDisplayGoogleBooksCommand)
            DisplayStandardLibCommand = New RelayCommand(AddressOf OnDisplayStandardLibCommand)
            InitializePaneCommands()
        End Sub

        Public Sub New(book As Book)
            Me.New()
            Try
                BibItemLibraryUri = New Uri(book.Url)
            Catch ex As Exception
            End Try
            Try
                BibItemGoogleBooksUri = New Uri(book.GoogleBooksUrl)
            Catch ex As Exception
            End Try
        End Sub

        Public Sub CloneTo(other As BrowserSupportViewModel)
            other.BibItemGoogleBooksUri = Me.BibItemGoogleBooksUri
            other.BibItemLibraryUri = Me.BibItemLibraryUri
            other.BibItemUri = Me.BibItemUri
            other.CurrentDisplayIsStandardLib = Me.CurrentDisplayIsStandardLib
        End Sub

        Private _bibItemUriIsValid As Boolean
        Public ReadOnly Property BibItemUriIsValid As Boolean
            Get
                Return _bibItemUriIsValid
            End Get
        End Property

        Private _bibItemUri As Uri = New Uri("http://www.dnb.de")
        Public Property BibItemUri As Uri
            Get
                Return _bibItemUri
            End Get
            Set(value As Uri)
                If value Is Nothing Then
                    _bibItemUriIsValid = False
                Else
                    _bibItemUriIsValid = True
                    SetProperty(Of Uri)(_bibItemUri, value)
                End If
            End Set
        End Property

        Private _isGoogleBooksItemAvailable As Boolean
        Public Property IsGoogleBooksItemAvailable As Boolean
            Get
                Return _isGoogleBooksItemAvailable
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_isGoogleBooksItemAvailable, value, "IsGoogleBooksItemAvailable")
            End Set
        End Property

        Private _currentDisplayIsStandardLib As Boolean
        Public Property CurrentDisplayIsStandardLib As Boolean
            Get
                Return _currentDisplayIsStandardLib
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_currentDisplayIsStandardLib, value, "CurrentDisplayIsStandardLib")
            End Set
        End Property

        Private _canGoBack As Boolean
        Public Property CanGoBack As Boolean
            Get
                Return _canGoBack
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_canGoBack, value)
            End Set
        End Property

        Private _canGoForward As Boolean
        Public Property CanGoForward As Boolean
            Get
                Return _canGoForward
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_canGoForward, value)
            End Set
        End Property

        Public Sub OnDisplayGoogleBooksCommand()
            BibItemUri = _bibItemGoogleBooksUri
            CurrentDisplayIsStandardLib = False
        End Sub

        Public Sub OnDisplayStandardLibCommand()
            BibItemUri = _bibItemLibraryUri
            CurrentDisplayIsStandardLib = True
        End Sub

#Region "BrowserPane"

        Private _browserPaneOpen As Boolean = False
        Public Property BrowserPaneOpen As Boolean
            Get
                Return _browserPaneOpen
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_browserPaneOpen, value)
            End Set
        End Property

        Private Sub InitializePaneCommands()
            OpenBrowserPane = New RelayCommand(AddressOf OnOpenBrowserPane)
            CloseBrowserPane = New RelayCommand(AddressOf OnCloseBrowserPane)
        End Sub

        Public Property CloseBrowserPane As RelayCommand
        Public Property OpenBrowserPane As RelayCommand

        Private Sub OnOpenBrowserPane()
            BrowserPaneOpen = True
        End Sub

        Private Sub OnCloseBrowserPane()
            BrowserPaneOpen = False
        End Sub

#End Region

    End Class

End Namespace
