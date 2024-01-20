Imports System.Text
Imports Windows.Globalization.DateTimeFormatting

Namespace Global.MyBooks.App.ValueConverters

    Public Class StringToDateTimeOffsetConverter
        Implements IValueConverter

        Public Shared Function ToPlainText(input As String) As String
            Dim result As New StringBuilder
            If input Is Nothing Then
                Return Nothing
            End If
            For Each c In input
                Dim ascii = AscW(c)
                If ascii < 32 Or ascii > 127 Then
                    Continue For
                End If
                result.Append(c)
            Next
            Return result.ToString
        End Function

        Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
            ' Convert String to System.Nullable(Of DateTime)

            If value Is Nothing Then
                Return Nothing
            Else
                Dim dt As DateTime
                If DateTime.TryParse(ToPlainText(value), dt) Then
                    Return New DateTimeOffset(dt)
                Else
                    Return Nothing
                End If
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
            Dim result As String = ""

            If value IsNot Nothing Then
                Dim dto As DateTimeOffset = value
                result = DateTimeFormatter.ShortDate.Format(dto)
            End If

            Return result
        End Function
    End Class

End Namespace
