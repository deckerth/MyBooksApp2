Namespace Global.MyBooks.Models

    Public Class Storage
        Inherits DBObject
        Implements IEquatable(Of Storage)

        Public Property Name As String

        Public Sub New()
            Name = ""
        End Sub

        Public Sub New(text As String)
            Name = text
        End Sub

        Public Function IEquatable_Equals(other As Storage) As Boolean Implements IEquatable(Of Storage).Equals
            Return Name.Equals(other.Name)
        End Function

        Public Function Clone() As Storage
            Return DirectCast(MemberwiseClone(), Storage)
        End Function
    End Class

End Namespace
