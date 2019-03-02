Namespace Global.MyBooks.Models

    Public Class Author
        Inherits DBObject
        Implements IEquatable(Of Author)

        Public Property Name As String

        Public Sub New()
            Name = ""
        End Sub

        Public Sub New(text As String)
            Name = text
        End Sub

        Public Function IEquatable_Equals(other As Author) As Boolean Implements IEquatable(Of Author).Equals
            Return Name.Equals(other.Name)
        End Function

        Public Function Clone() As Author
            Return DirectCast(MemberwiseClone(), Author)
        End Function

    End Class

End Namespace
