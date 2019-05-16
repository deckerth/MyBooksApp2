Imports MyBooks.App.Commands
Imports Telerik.Data.Core

Namespace Global.MyBooks.App.UserControls

    Public NotInheritable Class TextFilterControl
        Inherits BaseFilterControl

        Public Property ViewModel As TextFilter

        Private filterDescriptor As DelegateFilterDescriptor

        Public Sub New(filter As DelegateFilterDescriptor, referredProperty As String)
            InitializeComponent()
            PropertyName = referredProperty
            If filter Is Nothing Then
                Select Case PropertyName
                    Case "Authors"
                        filterDescriptor = New DelegateFilterDescriptor With {.Filter = New AuthorFilter()}
                    Case "Storage"
                        filterDescriptor = New DelegateFilterDescriptor With {.Filter = New StorageFilter()}
                    Case "Keywords"
                        filterDescriptor = New DelegateFilterDescriptor With {.Filter = New KeywordsFilter()}

                End Select
            Else
                filterDescriptor = filter
            End If
            ViewModel = DirectCast(filterDescriptor.Filter, TextFilter)

        End Sub

        Protected Overrides Sub Initialize()
            ' This method Is called just before the control Is vizualized.
            ' Here you can put additional logic that will be executed
            ' before the initialization of the control. 
        End Sub

        Public Overrides Function BuildDescriptor() As FilterDescriptorBase
            Return filterDescriptor
        End Function

    End Class

End Namespace
