Namespace Global.MyBooks.Models

    Public Class Keyword
        Inherits DBObject
        Implements IEquatable(Of Keyword)

        Public Property Name As String

        Public Sub New()
            Name = ""
        End Sub

        Public Sub New(text As String)
            Name = text
        End Sub

        Public Function IEquatable_Equals(other As Keyword) As Boolean Implements IEquatable(Of Keyword).Equals
            Return Name.Equals(other.Name)
        End Function

        Public Function Clone() As Keyword
            Return DirectCast(MemberwiseClone(), Keyword)
        End Function

    End Class

End Namespace
