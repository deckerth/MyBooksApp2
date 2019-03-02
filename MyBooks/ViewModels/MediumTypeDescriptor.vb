Imports MyBooks.App.ValueConverters
Imports MyBooks.Models

Namespace Global.MyBooks.App.ViewModels

    Public Class MediumTypeDescriptor

        Public Property Type As Book.MediaType
        Public Property Name As String

        Public Sub New()
        End Sub

        Public Sub New(aType As Book.MediaType)
            Dim mediumType = New MediumTypeToStringConverter()

            Type = aType
            Name = mediumType.Convert(Type, GetType(String), Nothing, "")
        End Sub

    End Class

End Namespace
