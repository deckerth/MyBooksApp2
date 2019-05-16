Namespace Global.MyBooks.App.Views

    Public NotInheritable Class ShowProgress
        Inherits ContentDialog

        Public Property Text As String = ""

        Public Sub New(op As String)
            InitializeComponent()
            Text = op
        End Sub

    End Class

End Namespace
