Namespace Global.MyBooks.App.ValueConverters

    Public Class BooleanInverterConverter
        Implements IValueConverter

        ' <summary>
        ' Inverts a Boolean value.
        ' </summary>
        Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert

            If targetType.Equals(GetType(Boolean)) Then
                Return Not DirectCast(value, Boolean)
            Else
                Throw New ArgumentException("Unsuported type: " + targetType.FullName)
            End If

        End Function

        ' <summary>
        ' Inverts a boolean value back to its original value.
        ' </summary>
        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack

            If targetType.Equals(GetType(Boolean)) Then
                Return Not DirectCast(value, Boolean)
            Else
                Throw New ArgumentException("Unsuported type: " + targetType.FullName)
            End If

        End Function
    End Class

End Namespace
