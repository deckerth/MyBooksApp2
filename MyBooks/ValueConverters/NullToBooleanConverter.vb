Namespace Global.MyBooks.App.ValueConverters

    Public Class NullToBooleanConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
            Dim result As Boolean = True

            If value Is Nothing Then
                result = False
            End If

            If parameter IsNot Nothing Then
                Dim invertResult As Boolean
                Boolean.TryParse(parameter.ToString, invertResult)
                If invertResult = True Then
                    result = Not result
                End If
            End If
            Return result
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function

    End Class

End Namespace
