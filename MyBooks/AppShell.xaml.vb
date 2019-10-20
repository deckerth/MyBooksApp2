Imports Windows.ApplicationModel.Core
Imports Windows.System
Imports Windows.UI.Core
Imports Windows.UI.Xaml.Automation
Imports Windows.UI.Xaml.Controls
Imports MyBooks.App.Navigation
Imports MyBooks.App.Views
Imports Windows.Storage.Pickers
Imports Windows.Storage
Imports Windows.Storage.Provider
Imports Windows.UI.Popups
Imports MyBooks.App.ViewModels
Imports Windows.UI.Xaml.Markup
Imports System.Threading

Namespace Global.MyBooks.App

    Public NotInheritable Class AppShell
        Inherits Page

        Private _isPaddingAdded As Boolean

        Private Shared _AppShell As AppShell
        Public Shared Property Current As AppShell
            Get
                Return _AppShell
            End Get
            Private Set(value As AppShell)
                _AppShell = value
            End Set
        End Property

        Private _coreView As CoreApplicationView
        Public ReadOnly Property CoreView As CoreApplicationView
            Get
                Return _coreView
            End Get
        End Property

        Public Property ViewModel As BrowserSupportViewModel = New BrowserSupportViewModel()

        Public Property QueryResult As LibraryBooksViewModel = New LibraryBooksViewModel()

        Public Property AppViewModel As New AppShellViewModel

        Public Sub New()
            InitializeComponent()
            DataContext = ViewModel
            AddHandler Loaded, AddressOf OnLoadedHandler
        End Sub

        Private Sub OnLoadedHandler(sender As Object, e As RoutedEventArgs)
            Current = Me
            CheckTogglePaneButtonSizeChanged()
            _coreView = CoreApplication.GetCurrentView()
            Dim titleBar = CoreApplication.GetCurrentView().TitleBar
            AddHandler titleBar.IsVisibleChanged, AddressOf TitleBar_IsVisibleChanged

            RootSplitView.RegisterPropertyChangedCallback(SplitView.DisplayModeProperty, Sub(s, a)
                                                                                             'Ensure that we update the reported size of the TogglePaneButton when the SplitView's
                                                                                             'DisplayMode changes.
                                                                                             CheckTogglePaneButtonSizeChanged()
                                                                                         End Sub)
            AddHandler SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf SystemNavigationManager_BackRequested
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible

            'Make sure that the first menu items is selected
            Dim container = DirectCast(NavMenuList.ContainerFromItem(PrimaryMenuItems.ElementAt(0)), ListViewItem)
            If container IsNot Nothing Then
                NavMenuList.SetSelectedItem(DirectCast(container, ListViewItem))
            End If
        End Sub

        Private ImportIconMarkup As String = "F0 M 1,1 h10 v4 h-1 v-3 h-8 v13 h8 v-3 h1 v4 h-10 v-15 M 14,7 h-8 v2 h8 v-2 M 6,7 l2,-2 l-1,-1 l-4,4 l4,4 l1,-1 l-2,-2"
        Private BarcodeMarkup As String = "F0 " &
                                          "M0,0 h2 v19 h-2 v-19 " &
                                          "M4,0 h1 v16 h-1 v-16 " &
                                          "M7,0 h1 v16 h-1 v-16 " &
                                          "M11,0 h1 v16 h-1 v-16 " &
                                          "M14,0 h1 v16 h-1 v-16 " &
                                          "M18,0 h2 v19 h-2 v-19"

        'New NavMenuItem With {.PathIconMarkup = BarcodeMarkup, .Label = App.Texts.GetString("Barcodes"), .DestPage = GetType(BarcodeImportPage), .IsSelected = False}


        Public PrimaryMenuItems As IReadOnlyCollection(Of NavMenuItem) = New ReadOnlyCollection(Of NavMenuItem)(
    {
      New NavMenuItem With {.Symbol = Symbol.Library, .Label = App.Texts.GetString("Shelf"), .DestPage = GetType(BookListPage), .IsSelected = True},
      New NavMenuItem With {.Symbol = Symbol.World, .Label = App.Texts.GetString("LibrarySearch"), .DestPage = GetType(LibrarySearchPage), .IsSelected = False},
      New NavMenuItem With {.Symbol = Symbol.Import, .Label = App.Texts.GetString("DataImport"), .DestPage = GetType(BarcodeImportPage), .IsSelected = False}
    })

        Public Sub AppShell_KeyDown(sender As Object, e As KeyRoutedEventArgs)
            Dim direction As FocusNavigationDirection = FocusNavigationDirection.None
            Select Case e.Key
                Case VirtualKey.Left
                Case VirtualKey.GamepadDPadLeft
                Case VirtualKey.GamepadLeftThumbstickLeft
                Case VirtualKey.NavigationLeft
                    direction = FocusNavigationDirection.Left
                Case VirtualKey.Right
                Case VirtualKey.GamepadDPadRight
                Case VirtualKey.GamepadLeftThumbstickRight
                Case VirtualKey.NavigationRight
                    direction = FocusNavigationDirection.Right

                Case VirtualKey.Up
                Case VirtualKey.GamepadDPadUp
                Case VirtualKey.GamepadLeftThumbstickUp
                Case VirtualKey.NavigationUp
                    direction = FocusNavigationDirection.Up

                Case VirtualKey.Down
                Case VirtualKey.GamepadDPadDown
                Case VirtualKey.GamepadLeftThumbstickDown
                Case VirtualKey.NavigationDown
                    direction = FocusNavigationDirection.Down
            End Select

            If direction <> FocusNavigationDirection.None And
                FocusManager.FindNextFocusableElement(direction) Is GetType(Control) Then
                Dim control As Control = DirectCast(FocusManager.FindNextFocusableElement(direction), Control)
                control.Focus(FocusState.Keyboard)
                e.Handled = True
            End If
        End Sub

        Private Sub SystemNavigationManager_BackRequested(sender As Object, e As BackRequestedEventArgs)
            Dim handled As Boolean = e.Handled
            BackRequested(handled)
            e.Handled = handled
        End Sub

        Private Sub BackRequested(ByRef handled As Boolean)
            ' Get a hold of the current frame so that we can inspect the app back stack.
            If AppFrame() Is Nothing Then
                Return
            End If

            ' Check to see if this Is the top-most page on the app back stack.
            If AppFrame.CanGoBack And Not handled Then
                ' If Not, set the event to handled And go back to the previous page in the app.
                handled = True
                AppFrame.GoBack()
            End If
        End Sub

        ' Navigate to the Page for the selected
        Private Sub NavMenuList_ItemInvoked(sender As Object, listViewItem As ListViewItem)
            For Each i In PrimaryMenuItems
                i.IsSelected = False
            Next

            Dim item = DirectCast(DirectCast(sender, NavMenuListView).ItemFromContainer(listViewItem), NavMenuItem)

            If item IsNot Nothing Then
                item.IsSelected = True
                If item.DestPage IsNot Nothing AndAlso item.DestPage IsNot AppFrame.CurrentSourcePageType Then
                    AppFrame.Navigate(item.DestPage, item.Arguments)
                End If
            End If
        End Sub

        Public Function AppFrame() As Frame
            Return frame
        End Function

        Public Property TogglePaneButtonRect As Rect


        ' Invoked when window title bar visibility changes, such as after loading Or in tablet mode
        ' Ensures correct padding at window top, between title bar And app content
        Private Sub TitleBar_IsVisibleChanged(sender As CoreApplicationViewTitleBar, args As Object)
            If Not _isPaddingAdded And sender.IsVisible() Then
                'add extra padding between window title bar And app content
                Dim extraPadding As Double = DirectCast(App.Current.Resources("DesktopWindowTopPadding"), Double)
                _isPaddingAdded = True
                Dim margin As Thickness = NavMenuList.Margin
                NavMenuList.Margin = New Thickness(margin.Left, margin.Top + extraPadding, margin.Right, margin.Bottom)
                margin = AppFrame.Margin
                AppFrame.Margin = New Thickness(margin.Left, margin.Top + extraPadding, margin.Right, margin.Bottom)
                margin = TogglePaneButton.Margin
                TogglePaneButton.Margin = New Thickness(margin.Left, margin.Top + extraPadding, margin.Right, margin.Bottom)
            End If

        End Sub


        ' Ensures the nav menu reflects reality when navigation Is triggered outside of
        ' the nav menu buttons.
        Private Sub OnNavigatingToPage(sender As Object, e As NavigatingCancelEventArgs)
            If e.NavigationMode = NavigationMode.Back Then
                Dim item = PrimaryMenuItems.Where(Function(p) p.DestPage Is e.SourcePageType).SingleOrDefault()
                If item Is Nothing And AppFrame.BackStackDepth > 0 Then
                    ' In cases where a page drills into sub-pages then we'll highlight the most recent
                    ' navigation menu item that appears in the BackStack
                    For Each entry In AppFrame.BackStack.Reverse()
                        item = PrimaryMenuItems.Where(Function(p) p.DestPage Is entry.SourcePageType).SingleOrDefault()
                        If item IsNot Nothing Then

                        End If
                    Next
                End If
                For Each i In PrimaryMenuItems
                    i.IsSelected = False
                Next

                If item IsNot Nothing Then
                    item.IsSelected = True
                End If

                Dim container = DirectCast(NavMenuList.ContainerFromItem(item), ListViewItem)

                ' While updating the selection state of the item prevent it from taking keyboard focus.  If a
                ' user Is invoking the back button via the keyboard causing the selected nav menu item to change
                ' then focus will remain on the back button.
                If container IsNot Nothing Then
                    container.IsTabStop = False
                End If

                NavMenuList.SetSelectedItem(container)

                If container IsNot Nothing Then
                    container.IsTabStop = True
                End If
            End If
        End Sub

        Public Event TogglePaneButtonRectChanged(shell As AppShell, r As Rect)

        ' Public method to allow pages to open SplitView's pane.
        ' Used for custom app shortcuts Like navigating left from page's left-most item
        Public Sub OpenNavePane()
            TogglePaneButton.IsChecked = True
            NavPaneDivider.Visibility = Visibility.Visible
        End Sub

        ' Hides divider when nav pane is closed.
        Private Sub RootSplitView_PaneClosed(sender As SplitView, args As Object)
            NavPaneDivider.Visibility = Visibility.Collapsed
        End Sub

        ' Callback when the SplitView's Pane is toggled closed.  When the Pane is not visible
        ' then the floating hamburger may be occluding other content in the app unless it Is aware.
        Private Sub TogglePaneButton_Unchecked(sender As Object, e As RoutedEventArgs)
            CheckTogglePaneButtonSizeChanged()
        End Sub

        ' Callback when the SplitView's Pane is toggled opened.
        ' Restores divider's visibility and ensures that margins around the floating hamburger are correctly set.
        Private Sub TogglePaneButton_Checked(sender As Object, e As RoutedEventArgs)
            NavPaneDivider.Visibility = Visibility.Visible
            CheckTogglePaneButtonSizeChanged()
        End Sub

        ' Check for the conditions where the navigation pane does Not occupy the space under the floating
        ' hamburger button And trigger the event.
        Private Sub CheckTogglePaneButtonSizeChanged()
            If RootSplitView.DisplayMode = SplitViewDisplayMode.Inline Or RootSplitView.DisplayMode = SplitViewDisplayMode.Overlay Then
                Dim transform = TogglePaneButton.TransformToVisual(Me)
                Dim rect = transform.TransformBounds(New Rect(0, 0, TogglePaneButton.ActualWidth, TogglePaneButton.ActualHeight))
                TogglePaneButtonRect = rect
            Else
                TogglePaneButtonRect = New Rect()
            End If

            RaiseEvent TogglePaneButtonRectChanged(Me, TogglePaneButtonRect)
        End Sub

        ' Enable accessibility on each nav menu item by setting the AutomationProperties.Name on each container
        ' using the associated Label of each item.
        Private Sub NavMenuItemContainerContentChanging(sender As ListViewBase, args As ContainerContentChangingEventArgs)
            If Not args.InRecycleQueue And args.Item IsNot Nothing AndAlso args.Item Is GetType(NavMenuItem) Then
                args.ItemContainer.SetValue(AutomationProperties.NameProperty, DirectCast(args.Item, NavMenuItem).Label)
            Else
                args.ItemContainer.ClearValue(AutomationProperties.NameProperty)
            End If
        End Sub

        Private Sub WebView_NavigationStarting(sender As WebView, args As WebViewNavigationStartingEventArgs)
            WebPageLoadingProgressRing.IsActive = True
        End Sub

        Private Sub WebView_NavigationCompleted(sender As WebView, args As WebViewNavigationCompletedEventArgs)
            WebPageLoadingProgressRing.IsActive = False
            ViewModel.CanGoBack = WebViewer.CanGoBack
            ViewModel.CanGoForward = WebViewer.CanGoForward
        End Sub

        Private Sub WebPageGoBack_Click(sender As Object, e As RoutedEventArgs)
            WebViewer.GoBack()
        End Sub

        Private Sub WebPageGoForward_Click(sender As Object, e As RoutedEventArgs)
            WebViewer.GoBack()
        End Sub

        Private Sub CopyLinkToClipboard(sender As Object, e As RoutedEventArgs)
            Dim linkDataPackage As New Windows.ApplicationModel.DataTransfer.DataPackage
            linkDataPackage.SetText(WebViewer.Source.OriginalString)
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(linkDataPackage)
        End Sub

        Private Async Sub SettingsButton_Click(sender As Object, e As RoutedEventArgs)
            Dim settings = New Views.SettingsDialog
            Await settings.ShowAsync()
        End Sub

        Private Async Sub OpenInBrowser(sender As Object, e As RoutedEventArgs)
            Await Windows.System.Launcher.LaunchUriAsync(New Uri(WebViewer.Source.OriginalString))
        End Sub
    End Class

End Namespace
