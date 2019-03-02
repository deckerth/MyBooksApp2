Imports Windows.UI.Xaml.Markup

Namespace Global.MyBooks.App.ValueConverters

    Public Class StringToGeometryConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert

            If value Is Nothing Then
                Return Nothing
            End If

            Dim xaml As String = "<Path " + "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" + "<Path.Data>" + DirectCast(value, String) + "</Path.Data></Path>"
            Dim path = XamlReader.Load(xaml)
            Dim geometry As Geometry = path.Data
            ' Detach the PathGeometry from the Path (important!)
            path.Data = Nothing
            Return geometry

        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class

End Namespace
