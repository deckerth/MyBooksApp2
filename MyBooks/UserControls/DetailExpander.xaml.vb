' Die Elementvorlage "Benutzersteuerelement" wird unter https://go.microsoft.com/fwlink/?LinkId=234236 dokumentiert.

Namespace Global.MyBooks.App.UserControls

    Public NotInheritable Class DetailExpander
        Inherits UserControl

        Public Sub New()
            InitializeComponent()
        End Sub

        Public Property Pane As UIElement
            Get
                Return DirectCast(GetValue(PaneProperty), UIElement)
            End Get
            Set(value As UIElement)
                SetValue(PaneProperty, value)
            End Set
        End Property


        '<summary>
        'Dependency property as the backing store for Pane. This enables animation, styling, binding, etc.
        '</summary>
        Public Shared ReadOnly Property PaneProperty As DependencyProperty =
            DependencyProperty.Register("Pane", GetType(UIElement), GetType(DetailExpander), New PropertyMetadata(Nothing))

        Public Property HeaderContent As UIElement
            Get
                Return DirectCast(GetValue(HeaderContentProperty), UIElement)
            End Get
            Set(value As UIElement)
                SetValue(HeaderContentProperty, value)
            End Set
        End Property

        '<summary>
        'Dependency property as the backing store for HeaderContent. This enables animation, sytling, binding, etc.
        '</summary>
        Public Shared ReadOnly Property HeaderContentProperty As DependencyProperty =
            DependencyProperty.Register("HeaderContent", GetType(UIElement), GetType(DetailExpander), New PropertyMetadata(Nothing))

        Public Property Label As String
            Get
                Return DirectCast(GetValue(LabelProperty), String)
            End Get
            Set(value As String)
                SetValue(LabelProperty, value)
            End Set
        End Property

        '<summary>
        'Dependency property as the backing store for Label. This enables animation, sytling, binding, etc.
        '</summary>
        Public Shared ReadOnly Property LabelProperty As DependencyProperty =
            DependencyProperty.Register("Label", GetType(String), GetType(DetailExpander), New PropertyMetadata(Nothing))

        Public Property LabelStyle As Style
            Get
                Return DirectCast(GetValue(LabelStyleProperty), Style)
            End Get
            Set(value As Style)
                SetValue(LabelStyleProperty, value)
            End Set
        End Property

        '<summary>
        'Dependency property as the backing store for LabelStyleProperty. This enables animation, sytling, binding, etc.
        '</summary>
        Public Shared ReadOnly Property LabelStyleProperty As DependencyProperty =
            DependencyProperty.Register("LabelStyle", GetType(Style), GetType(DetailExpander), New PropertyMetadata(Nothing))

        Private Sub ToggleButton_Checked(sender As Object, e As RoutedEventArgs)
            Dim button As ToggleButton = DirectCast(sender, ToggleButton)
            button.Style = TryCast(Resources("ExpanderButtonOpenStyle"), Style)
        End Sub

        Private Sub ToggleButton_UnChecked(sender As Object, e As RoutedEventArgs)
            Dim button As ToggleButton = DirectCast(sender, ToggleButton)
            button.Style = TryCast(Resources("ExpanderButtonClosedStyle"), Style)
        End Sub

        Private Sub Header_Tapped(sender As Object, e As Windows.UI.Xaml.Input.TappedRoutedEventArgs)
            Toggle.IsChecked = Not Toggle.IsChecked
        End Sub

    End Class

    ' Converts a nullable bool to bool

    Class NullableBooleanConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
            Return DirectCast(value, Boolean)
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
            Return DirectCast(value, Boolean)
        End Function
    End Class
End Namespace
