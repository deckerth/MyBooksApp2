Imports Telerik.Core

Namespace Global.MyBooks.App.TelerikStrings

    Public Class TelerikStringLoader
        Implements IStringResourceLoader

        Public Function GetString(key As String) As String Implements IStringResourceLoader.GetString

            Select Case key
                Case "And", "CaseSensitiveMode", "ClearFilter", "Contains", "DoesNotContain",
                     "DoesNotEqualTo", "DragToGroup", "EndsWith", "EqualsTo", "Filter",
                     "IsFalse", "IsGreaterThan", "IsGreaterThanOrEqualTo", "IsLessThan",
                     "IsLessThanOrEqualTo", "IsTrue", "Off", "On", "StartsWith",
                     "DataOperationsButtonFilter", "DataOperationsButtonSort", "DataOperationsButtonGroup",
                     "FilterHeader", "FilterButton", "Or", "Cancel", "DataOperationsButtonUngroup",
                     "LeapYear", "DateSelectorHeader"

                Case Else
                    Return Nothing
            End Select

            Return App.Texts.GetString("Telerik_" + key)

        End Function
    End Class

End Namespace
