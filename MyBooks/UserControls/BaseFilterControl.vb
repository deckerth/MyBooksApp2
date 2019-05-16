Imports Telerik.Data.Core
Imports Telerik.UI.Xaml.Controls.Grid.Primitives

Namespace Global.MyBooks.App.UserControls

    Public MustInherit Class BaseFilterControl
        Inherits UserControl
        Implements IFilterControl

        Public Property PropertyName As String

        Public Property AssociatedDescriptor As FilterDescriptorBase Implements IFilterControl.AssociatedDescriptor

        Public Property IsFirst As Boolean Implements IFilterControl.IsFirst

        Public MustOverride Function BuildDescriptor() As FilterDescriptorBase Implements IFilterControl.BuildDescriptor

        Protected MustOverride Sub Initialize()

    End Class

End Namespace
