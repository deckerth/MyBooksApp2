Imports MyBooks.App.ViewModels

Namespace Global.MyBooks.App.UserControls

    Public Class ChoiceCheckBox
        Inherits BindableBase

        Private _isChecked As Boolean = False
        Public Property IsChecked As Boolean
            Get
                Return _isChecked
            End Get
            Set(value As Boolean)
                If value <> _isChecked Then
                    SetProperty(Of Boolean)(_isChecked, value)
                End If
            End Set
        End Property

        Private _value As String
        Public Property Value As String
            Get
                Return _value
            End Get
            Set(val As String)
                If Not val.Equals(_value) Then
                    SetProperty(Of String)(_value, val)
                    If String.IsNullOrEmpty(_value) Then
                        DisplayValue = App.Texts.GetString("EmptyEntry")
                    Else
                        DisplayValue = _value
                    End If
                End If
            End Set
        End Property

        Private _displayValue As String
        Public Property DisplayValue As String
            Get
                Return _displayValue
            End Get
            Set(val As String)
                If Not val.Equals(_displayValue) Then
                    SetProperty(Of String)(_displayValue, val)
                End If
            End Set
        End Property

    End Class

End Namespace
