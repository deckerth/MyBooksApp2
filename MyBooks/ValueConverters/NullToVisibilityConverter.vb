Namespace Global.MyBooks.App.ValueConverters

    Public Class NullToVisibilityConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
            Dim visibility As Visibility = Visibility.Visible

            If value Is Nothing Then
                visibility = Visibility.Collapsed
            End If

            If parameter IsNot Nothing Then
                Dim invertResult As Boolean
                Boolean.TryParse(parameter.ToString, invertResult)
                If invertResult = True Then
                    If visibility = Visibility.Visible Then
                        visibility = Visibility.Collapsed
                    Else
                        visibility = Visibility.Visible
                    End If
                End If
            End If
            Return visibility
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class

End Namespace
