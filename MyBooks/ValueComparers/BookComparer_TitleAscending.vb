Imports MyBooks.Models

Namespace Global.MyBooks.App.ValueComparers

    Public Class BookComparer_TitleAscending
        Implements IComparer(Of Book)

        Public Function Compare(ByVal x As Book, ByVal y As Book) As Integer Implements IComparer(Of Book).Compare

            If x Is Nothing Then
                If y Is Nothing Then
                    ' If x is Nothing and y is Nothing, they're
                    ' equal. 
                    Return 0
                Else
                    ' If x is Nothing and y is not Nothing, y
                    ' is greater. 
                    Return -1
                End If
            Else
                ' If x is not Nothing...
                '
                If y Is Nothing Then
                    ' ...and y is Nothing, x is greater.
                    Return 1
                Else
                    ' ...and y is not Nothing, compare the string
                    Return String.Compare(x.Title, y.Title, StringComparison.CurrentCultureIgnoreCase)
                End If
            End If
        End Function

    End Class

End Namespace
