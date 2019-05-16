Imports Microsoft.Toolkit.Uwp.Helpers

Namespace Global.MyBooks.App.ViewModels

    Public Class AppShellViewModel
        Inherits BindableBase

        Private _navigationAllowed As Boolean
        Public Property NavigationAllowed As Boolean
            Get
                Return _navigationAllowed
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_navigationAllowed, value)
            End Set
        End Property

        Public Async Function SetNavigationAllowedAsync(value As Boolean) As Task
            Await DispatcherHelper.ExecuteOnUIThreadAsync(AppShell.Current.CoreView, Sub() NavigationAllowed = value)
        End Function

    End Class

End Namespace
