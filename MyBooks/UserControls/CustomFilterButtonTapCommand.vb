Imports Telerik.Data.Core
Imports Telerik.UI.Xaml.Controls.Grid.Commands

Namespace Global.MyBooks.App.UserControls

    Public Class CustomFilterButtonTapCommand
        Inherits DataGridCommand

        Public Sub New()
            Me.Id = CommandId.FilterButtonTap
        End Sub

        Public Overrides Function CanExecute(parameter As Object) As Boolean
            Return True
        End Function

        Public Overrides Sub Execute(parameter As Object)
            Dim context = DirectCast(parameter, FilterButtonTapContext)

            If ColumnMarker.GetRequiresCustomFiltering(context.Column) Then
                Dim firstFilter As FilterDescriptorBase = Nothing
                Dim secondFilter As FilterDescriptorBase = Nothing
                If context.AssociatedDescriptor IsNot Nothing Then
                    If TypeOf (context.AssociatedDescriptor) Is CompositeFilterDescriptor Then
                        firstFilter = DirectCast(context.AssociatedDescriptor, CompositeFilterDescriptor).Descriptors.Item(0)
                        secondFilter = DirectCast(context.AssociatedDescriptor, CompositeFilterDescriptor).Descriptors.Item(1)
                    Else
                        firstFilter = DirectCast(context.AssociatedDescriptor, DelegateFilterDescriptor)
                    End If
                End If

                Dim columnName = ColumnMarker.GetColumnName(context.Column)
                If columnName.Equals("BorrowedTo") Or columnName.Equals("BorrowedDate") Then
                    context.FirstFilterControl = New BorrowedFilterControl(firstFilter) With {.DataContext = firstFilter, .PropertyName = columnName}
                Else
                    context.FirstFilterControl = New TextChoiceFilterControl(firstFilter, columnName) With {.DataContext = firstFilter}
                End If
                context.SecondFilterControl = Nothing
                'context.FirstFilterControl = New TextFilterControl(firstFilter, ColumnMarker.GetColumnName(context.Column)) With {.DataContext = firstFilter}
                'context.SecondFilterControl = New TextFilterControl(secondFilter, ColumnMarker.GetColumnName(context.Column)) With {.DataContext = secondFilter}
            End If

            Me.Owner.CommandService.ExecuteDefaultCommand(CommandId.FilterButtonTap, context)
        End Sub

    End Class

End Namespace
