Namespace Global.MyBooks.App.ValueConverters

    Public Class IntegerToStringConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert

            Dim result As String = ""

            If value IsNot Nothing Then
                result = value.ToString
            End If

            Return result

        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class

End Namespace
