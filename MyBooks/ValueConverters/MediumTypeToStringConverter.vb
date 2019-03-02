Imports MyBooks.Models

Namespace Global.MyBooks.App.ValueConverters

    Public Class MediumTypeToStringConverter
        Implements IValueConverter

        ' <summary>
        ' Modifies the source data before passing it to the target for display in the UI.
        ' </summary>
        ' <param name="value">The source data being passed to the target.</param>
        ' <param name="targetType">The type of the target property, as a type reference (System.Type for Microsoft .NET, a TypeName helper struct for Visual C++ component extensions (C++/CX)).</param>
        ' <param name="parameter">An optional parameter to be used in the converter logic.</param>
        ' <param name="language">The language of the conversion.</param>
        ' <returns>The value to be passed to the target dependency property.</returns>
        Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
            Dim result As String = ""
            If value IsNot Nothing Then
                Dim type As Book.MediaType = DirectCast(value, Book.MediaType)
                Select Case type
                    Case Book.MediaType.Book
                        result = App.Texts.GetString("Book")
                    Case Book.MediaType.EBook
                        result = App.Texts.GetString("EBook")
                    Case Book.MediaType.AudioBook
                        result = App.Texts.GetString("AudioBook")
                End Select
            End If

            Return result
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
            Dim result As Book.MediaType = Book.MediaType.Undefined
            If value IsNot Nothing Then
                If TypeOf value Is Book.MediaType Then
                    Return value
                Else
                    Dim type As String = DirectCast(value, String)
                    If type.Equals(App.Texts.GetString("Book")) Then
                        result = Book.MediaType.Book
                    ElseIf type.Equals(App.Texts.GetString("EBook")) Then
                        result = Book.MediaType.EBook
                    ElseIf type.Equals(App.Texts.GetString("AudioBook")) Then
                        result = Book.MediaType.AudioBook
                    End If
                End If
            End If
            Return result
        End Function
    End Class

End Namespace
