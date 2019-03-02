Namespace Global.MyBooks.App.ValueConverters

    Public Class DateToDateTimeOffsetConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert

            Dim result As DateTimeOffset
            If value IsNot Nothing And TypeOf value Is DateTime Then
                Try
                    result = New DateTimeOffset(value)
                Catch ex As Exception
                End Try
            End If
            Return result

        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack

            Dim result As DateTime
            If value IsNot Nothing AndAlso TypeOf value Is DateTimeOffset Then
                result = value.DateTime
            End If
            Return result

        End Function
    End Class

End Namespace
