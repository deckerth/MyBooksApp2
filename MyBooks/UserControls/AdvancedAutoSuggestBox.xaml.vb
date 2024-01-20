' Die Elementvorlage "Benutzersteuerelement" wird unter https://go.microsoft.com/fwlink/?LinkId=234236 dokumentiert.

Namespace Global.MyBooks.App.UserControls

    Public NotInheritable Class AdvancedAutoSuggestBox
        Inherits UserControl

        Public Event DeleteSuggestion(sender As AdvancedAutoSuggestBox, e As AdvancedAutoSuggestBoxDeleteSuggestionArgs)

        Public Event TextChanged(sender As AdvancedAutoSuggestBox, e As AutoSuggestBoxTextChangedEventArgs)

        Public Event SuggestionChosen(sender As AdvancedAutoSuggestBox, args As AdvancedAutoSuggestBoxSuggesttionChosenArgs)

        Protected Property SuggestionList As New ObservableCollection(Of AdvancedAutoSuggestBoxEntryViewModel)

        Public Shared ReadOnly ItemsSourceProperty As DependencyProperty = DependencyProperty.Register("ItemsSource",
                           GetType(Collection(Of String)), GetType(AdvancedAutoSuggestBox), New PropertyMetadata(Nothing))

        Public Property ItemsSource As Collection(Of String)
            Get
                Return DirectCast(GetValue(ItemsSourceProperty), Collection(Of String))
            End Get
            Set(value As Collection(Of String))
                SetValue(ItemsSourceProperty, value)
                SuggestionList.Clear()
                AdvancedAutoSuggestBoxEntryViewModel.InputBoxWidth = Width
                If value IsNot Nothing Then
                    For Each v In value
                        Dim vm As New AdvancedAutoSuggestBoxEntryViewModel(v)
                        AddHandler vm.DeleteEntry, AddressOf OnDeleteEntry
                        SuggestionList.Add(vm)
                    Next
                End If
                If SuggestionList.Count <> 0 Then
                    SuggestionListView.ItemsSource = SuggestionList
                    SuggestionListView.SelectedIndex = -1
                    AutoSuggestBoxFlyout.ShowAt(InputBox, New FlyoutShowOptions With {
                    .Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft
                })
                Else
                    AutoSuggestBoxFlyout.Hide()
                End If
            End Set
        End Property

        Public Shared ReadOnly TextProperty As DependencyProperty = DependencyProperty.Register("Text",
                           GetType(String), GetType(AdvancedAutoSuggestBox), New PropertyMetadata(""))

        Public Property Text As String
            Get
                Return DirectCast(GetValue(TextProperty), String)
            End Get
            Set(value As String)
                SetValue(TextProperty, value)
            End Set
        End Property

        Public Shared ReadOnly HeaderProperty As DependencyProperty = DependencyProperty.Register("Header",
                           GetType(String), GetType(AdvancedAutoSuggestBox), New PropertyMetadata(""))

        Public Property Header As String
            Get
                Return DirectCast(GetValue(HeaderProperty), String)
            End Get
            Set(value As String)
                SetValue(HeaderProperty, value)
            End Set
        End Property

        Private Sub OnDeleteEntry(e As AdvancedAutoSuggestBoxEntryViewModel.DeleteEntryEventArgs)
            RemoveHandler e.entry.DeleteEntry, AddressOf OnDeleteEntry
            SuggestionList.Remove(e.entry)
            RaiseEvent DeleteSuggestion(Me, New AdvancedAutoSuggestBoxDeleteSuggestionArgs With {.SuggestionToDelete = e.entry.Text})
        End Sub

        Private Sub AutoSuggestBoxItem_PointerEntered(sender As Grid, e As PointerRoutedEventArgs)
            If e.Pointer.PointerDeviceType = Windows.Devices.Input.PointerDeviceType.Mouse OrElse e.Pointer.PointerDeviceType = Windows.Devices.Input.PointerDeviceType.Pen Then
                DirectCast(sender.DataContext, AdvancedAutoSuggestBoxEntryViewModel).PointerEntered = True
            End If
        End Sub

        Private Sub AutoSuggestBoxItem_PointerExited(sender As Grid, e As PointerRoutedEventArgs)
            If e.Pointer.PointerDeviceType = Windows.Devices.Input.PointerDeviceType.Mouse OrElse e.Pointer.PointerDeviceType = Windows.Devices.Input.PointerDeviceType.Pen Then
                DirectCast(sender.DataContext, AdvancedAutoSuggestBoxEntryViewModel).PointerEntered = False
            End If
        End Sub

        Private Sub SuggestionList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            AutoSuggestBoxFlyout.Hide()
            If SuggestionListView.SelectedItem IsNot Nothing Then
                InputBox.Text = SuggestionListView.SelectedItem.Text
                RaiseEvent SuggestionChosen(Me, New AdvancedAutoSuggestBoxSuggesttionChosenArgs With {.ChosenSuggestion = SuggestionListView.SelectedItem.Text})
            End If
        End Sub

        Private Sub InputBox_TextChanged(sender As AutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)
            'Since selecting an item will also change the text,
            'only listen to changes caused by user entering text.
            If args.Reason = AutoSuggestionBoxTextChangeReason.UserInput Then
                RaiseEvent TextChanged(Me, args)
            End If
        End Sub

        Private Sub SuggestionListView_GotFocus(sender As Object, e As RoutedEventArgs)
            InputBox.Focus(FocusState.Programmatic)
        End Sub

    End Class

End Namespace

