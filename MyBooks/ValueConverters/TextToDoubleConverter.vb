Namespace Global.MyBooks.App.ValueConverters

    Public Class TextToDoubleConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
            ' Text to double
            Dim result As Double = Nothing

            If value IsNot Nothing Then
                Dim str = DirectCast(value, String)

                If Not String.IsNullOrWhiteSpace(str) AndAlso
                   Double.TryParse(str, result) AndAlso result = 0 Then
                    result = Nothing
                End If
            End If

            Return result

        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
            ' Double to string

            Dim result As String = ""

            If value IsNot Nothing Then
                Dim d = DirectCast(value, Double)
                If d > 0 Then
                    result = d.ToString()
                End If
            End If

            Return result
        End Function
    End Class

End Namespace
