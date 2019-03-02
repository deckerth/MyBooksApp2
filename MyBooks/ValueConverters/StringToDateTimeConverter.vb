Imports Windows.Globalization.DateTimeFormatting

Namespace Global.MyBooks.App.ValueConverters

    Public Class StringToDateTimeConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
            ' Convert String to System.Nullable(Of DateTime)

            If value Is Nothing Then
                Return Nothing
            Else
                Dim result As DateTime
                If DateTime.TryParse(value, result) Then
                    Return result
                Else
                    Return Nothing
                End If
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
            Dim result As String = ""

            If value IsNot Nothing Then
                Dim dto As DateTimeOffset = New DateTimeOffset(value)
                result = DateTimeFormatter.ShortDate.Format(dto)
            End If

            Return result
        End Function
    End Class

End Namespace
