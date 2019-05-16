Imports Telerik.Data.Core

Namespace Global.MyBooks.App.UserControls

    Public NotInheritable Class BorrowedFilterControl
        Inherits BaseFilterControl

        Private filterDescriptor As DelegateFilterDescriptor

        Public Sub New(filter As DelegateFilterDescriptor)
            InitializeComponent()
            If filter Is Nothing Then
                filterDescriptor = New DelegateFilterDescriptor With {.Filter = New BorrowedFilter()}
            Else
                filterDescriptor = filter
            End If
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
