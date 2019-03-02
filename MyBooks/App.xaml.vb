Imports System.Resources
Imports Microsoft.EntityFrameworkCore
Imports MyBooks.App.TelerikStrings
Imports MyBooks.App.Views
Imports MyBooks.Repository
Imports MyBooks.Repository.Sql
Imports Telerik.UI.Xaml.Controls.Grid
Imports Telerik.UI.Xaml.Controls.Input
Imports Windows.ApplicationModel.Resources
Imports Windows.Globalization
Imports Windows.Storage
Imports Windows.UI.Xaml.Media.Animation

Namespace Global.MyBooks.App

    NotInheritable Class App
        Inherits Application

        Public Shared Texts As ResourceLoader = New Windows.ApplicationModel.Resources.ResourceLoader()
        Public Const ApplicationThemeLight As Integer = 0
        Public Const ApplicationThemeDark As Integer = 1
        Public Shared SelectedApplicationTheme As Integer = 0

        ' <summary>
        ' Pipeline for interacting with backend service Or database.
        ' </summary>
        Public Shared Property Repository As IMyBooksRepository

        Public Sub New()

            Dim settings = Windows.Storage.ApplicationData.Current.LocalSettings

            SelectedApplicationTheme = settings.Values("ApplicationTheme")

            If SelectedApplicationTheme = ApplicationThemeDark Then
                RequestedTheme = ApplicationTheme.Dark
            Else
                RequestedTheme = ApplicationTheme.Light
            End If

            ' Dieser Aufruf ist für den Designer erforderlich.
            InitializeComponent()

            ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        End Sub

        ''' <summary>
        ''' Wird aufgerufen, wenn die Anwendung durch den Endbenutzer normal gestartet wird. Weitere Einstiegspunkte
        ''' werden verwendet, wenn die Anwendung zum Öffnen einer bestimmten Datei, zum Anzeigen
        ''' von Suchergebnissen usw. gestartet wird.
        ''' </summary>
        ''' <param name="e">Details über Startanforderung und -prozess.</param>
        Protected Overrides Sub OnLaunched(e As Windows.ApplicationModel.Activation.LaunchActivatedEventArgs)

            UseSqlite()

            ' Prepare the app shell and window content.
            Dim shell As AppShell = TryCast(Window.Current.Content, AppShell)
            If shell Is Nothing Then
                shell = New AppShell
                shell.Language = ApplicationLanguages.Languages(0)
                Window.Current.Content = shell
            End If

            If shell.AppFrame.Content Is Nothing Then
                ' When the navigation stack isn't restored, navigate to the first page
                ' suppressing the initial entrance animation.

                shell.AppFrame.Navigate(GetType(BookListPage), e.Arguments,
                    New SuppressNavigationTransitionInfo())
            End If

            GridLocalizationManager.Instance.StringLoader = New TelerikStringLoader()
            InputLocalizationManager.Instance.StringLoader = New TelerikStringLoader()

            Window.Current.Activate()
        End Sub

        ' <summary>
        ' Configures the app to use the Sqlite data source.
        ' </summary>
        Public Shared Sub UseSqlite()
            Dim databasePath As String = ApplicationData.Current.LocalFolder.Path + "\MyBooks.db"
            Dim dbOptions = New DbContextOptionsBuilder(Of MyBooksContext)().UseSqlite(
            "Data Source=" + databasePath)
            Repository = New SqlMyBooksRepository(dbOptions)
        End Sub

        ''' <summary>
        ''' Wird aufgerufen, wenn die Navigation auf eine bestimmte Seite fehlschlägt
        ''' </summary>
        ''' <param name="sender">Der Rahmen, bei dem die Navigation fehlgeschlagen ist</param>
        ''' <param name="e">Details über den Navigationsfehler</param>
        Private Sub OnNavigationFailed(sender As Object, e As NavigationFailedEventArgs)
            Throw New Exception("Failed to load Page " + e.SourcePageType.FullName)
        End Sub

        ''' <summary>
        ''' Wird aufgerufen, wenn die Ausführung der Anwendung angehalten wird.  Der Anwendungszustand wird gespeichert,
        ''' ohne zu wissen, ob die Anwendung beendet oder fortgesetzt wird und die Speicherinhalte dabei
        ''' unbeschädigt bleiben.
        ''' </summary>
        ''' <param name="sender">Die Quelle der Anhalteanforderung.</param>
        ''' <param name="e">Details zur Anhalteanforderung.</param>
        Private Sub OnSuspending(sender As Object, e As SuspendingEventArgs) Handles Me.Suspending
            Dim deferral As SuspendingDeferral = e.SuspendingOperation.GetDeferral()
            ' TODO: Anwendungszustand speichern und alle Hintergrundaktivitäten beenden
            deferral.Complete()
        End Sub

    End Class

End Namespace
