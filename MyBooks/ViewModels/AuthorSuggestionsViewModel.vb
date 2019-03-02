Imports System.Text.RegularExpressions
Imports MyBooks.App.Commands

Namespace Global.MyBooks.App.ViewModels

    Public Class AuthorSuggestionsViewModel
        Inherits BindableBase

        Public Enum ConversionMode
            LastNameFirstName
            FirstNameLastName
        End Enum

        Private _originalAuthorsString As String = ""
        Public Property OriginalAuthorsString
            Get
                Return _originalAuthorsString
            End Get
            Set(value)
                SetProperty(Of String)(_originalAuthorsString, value)
            End Set
        End Property

        Private _suggestion As String = ""
        Public Property Suggestion
            Get
                Return _suggestion
            End Get
            Set(value)
                SetProperty(Of String)(_suggestion, value)
            End Set
        End Property

        Private _nameSeparator As String = ""
        Public Property NameSeparator As String
            Get
                Return _nameSeparator
            End Get
            Set(value As String)
                SetProperty(Of String)(_nameSeparator, value)
            End Set
        End Property

        Private _extendedEditMode As Boolean = False
        Public Property ExtendedEditMode As Boolean
            Get
                Return _extendedEditMode
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_extendedEditMode, value)
            End Set
        End Property

        Private _startIndex As Integer = 0
        Private _length As Integer = 0
        Private _conversionMode As ConversionMode = ConversionMode.LastNameFirstName

        Public Property AcceptConvertedNameCommand As RelayCommand
        Public Property OpenExtendedModeCommand As RelayCommand

        Private ReadOnly _bookViewModel As BookDetailPageViewModel

        Public Sub New(viewModel As BookDetailPageViewModel)
            _bookViewModel = viewModel
            AcceptConvertedNameCommand = New RelayCommand(AddressOf OnAcceptConvertedName)
            OpenExtendedModeCommand = New RelayCommand(AddressOf OnOpenExtendedMode)
        End Sub

        Private Sub OnAcceptConvertedName()
            _bookViewModel.Book.Authors = Suggestion
        End Sub

        Private Sub OnOpenExtendedMode()
            ExtendedEditMode = True
        End Sub

        'Douglas Preston ; aus dem amerikanischen Englischen von Jürgen Neubauer
        'Eliane Beaufils/Eva Holling (eds)
        'Douglas Preston, Lincoln Child ; aus dem Amerikanischen von Michael Benthack
        'Yan Wang Preston ; editor: Nadine Barth
        'editors: Tonio Kröger, Laura Preston, Tanja Widmann
        'by Preston M. Mote, Jeffrey S. Neuschatz, Brian H. Bornstein, Stacy A. Wetmore, Kylie N. Key
        'Johann Sebastian Bach ; César Franck ; Felix Mendelssohn-Bartholdy ; Léon Boe͏̈llmann ; Edward Elgar ; Charles-Marie Widor ...
        'Interpr.: Wolfgang Hannes [Trp.]. Bernhard Läubin [Trp.]. English Chamber Orchestra. Simon Preston [Dir.]
        'Frann Preston-Gannon ; Übersetzung Englisch - Deutsch: Charlotte Hammer und Hanne Hammer
        'Eric Wilson/Theresa Preston ; aus dem Englischen von Anna Schnabel

        Public Sub SetAuthors(ByRef authors As String)
            ExtendedEditMode = False
            NameSeparator = ""
            _startIndex = 0
            _length = 0
            OriginalAuthorsString = authors
        End Sub

        Public Sub SetAuthors(ByRef authors As String, StartIndex As Integer, Length As Integer)
            _startIndex = StartIndex
            _length = Length
            OriginalAuthorsString = authors
        End Sub

        Public Sub SetConversionMode(mode As ConversionMode)
            _conversionMode = mode
        End Sub

        Private Sub DetermineStartIndex()

            If _length = 0 Then
                _startIndex = 0
                Dim firstSpace = OriginalAuthorsString.IndexOf(" ")
                If firstSpace < 1 Or firstSpace = OriginalAuthorsString.Length Then
                    Return
                End If
                Dim prefix = OriginalAuthorsString.Substring(0, firstSpace)
                If prefix.Contains(":") Or prefix.Equals("by") Then
                    _startIndex = firstSpace + 1
                End If
            End If
        End Sub

        Private Function GetSeparator(workArea As String, allowComma As Boolean) As Char()
            If NameSeparator.Length = 0 Then
                ' Find primary separator sign ,/;
                If allowComma AndAlso workArea.Contains(",") Then
                    NameSeparator = ","
                ElseIf workArea.Contains("/") Then
                    NameSeparator = "/"
                ElseIf workArea.Contains(";") Then
                    NameSeparator = ";"
                End If
            End If
            Return {NameSeparator}
        End Function

        Private Sub CheckForFullStopAsEndMarker(workArea As String, ByRef endPos As Integer)
            ' Special handling for ".":
            ' "." is an end-marker as well, but titles such as 
            ' "Dr." "Prof." "Ph.D." must be accepted s parts of names

            Dim parts() As String = workArea.Split({"."c}, StringSplitOptions.None)
            If parts.Length > 1 Then
                Dim lastValidPart = 0
                For Each part In parts.SkipLast(1) ' The last part is not followed by a full-stop
                    ' 1.) [Paul Wilson Dr] [Thomas Decker] [This is an additional text]
                    ' 2.) [Paul Wilson Dr] [Thomas Decker]
                    ' 3.) Paul Wilson. First additional sentence. This is an additional text.
                    '                ^endPos                        
                    '     [Paul Wilson] [First additional sentence] [This is an additional text] []

                    If part.EndsWith("Dr") OrElse part.EndsWith("Prof") OrElse
                       part.EndsWith("Ph") OrElse part.EndsWith("Ing") Then
                        lastValidPart = lastValidPart + 1
                        Continue For
                    Else
                        ' Check for abbreviations Artur C. Clarke => [Arthur] [C] [Clarke]
                        If part.Length >= 1 Then
                            Dim lastChar = part.Substring(0, part.Length)
                            If Char.IsLetter(lastChar) And Char.IsUpper(lastChar) Then
                                lastValidPart = lastValidPart + 1
                                Continue For
                            End If
                        End If
                    End If
                    Exit For ' invalid part found or end of sequence
                Next
                If lastValidPart < parts.Length - 1 Then
                    ' Examples above:
                    ' 1.) lastValidPart = 1, parts.Length = 3 => part 2 is not valid, end at second full-stop
                    ' 2.) lastValidPart = 1, parts.Length = 2 => all parts are valid, no end marker
                    ' 3.) lastValidPart = 0, parts.Length = 4 => part1 1,2,3 are not valid, end at first full-stop.
                    Dim endingFullStopNumber = lastValidPart + 1
                    Dim fullStopsFound = 0
                    Dim index = -1
                    Do
                        index = workArea.IndexOf("."c, index + 1)
                        If index >= 0 Then
                            fullStopsFound = fullStopsFound + 1
                            If fullStopsFound = endingFullStopNumber Then
                                endPos = index
                                Exit Do
                            End If
                        Else
                            Exit Do
                        End If
                    Loop
                End If
            End If
        End Sub

        Private Function DetermineLength(workArea As String, separator() As Char, allowComma As Boolean) As String
            Dim endPosSpace As String = ""
            If _length = 0 Then
                Dim endPos = workArea.Length
                Dim endMarkers As String = ";/("
                Dim endMarkerRequiresLeadingSpace As New List(Of Boolean) From {False, True, True}
                If allowComma Then
                    endMarkers = "," + endMarkers
                    endMarkerRequiresLeadingSpace.Insert(0, False)
                End If
                Dim index As Integer = 0
                For Each t In endMarkers.ToList()
                    If separator.Length > 0 AndAlso t <> separator(0) Then
                        Dim pos = workArea.IndexOf(t)
                        If pos >= 0 And pos < endPos Then
                            endPos = pos
                            endPosSpace = If(endMarkerRequiresLeadingSpace(index), " ", "")
                        End If
                    End If
                    index = index + 1
                Next
                If endPos = workArea.Length Then
                    CheckForFullStopAsEndMarker(workArea, endPos)
                End If
                If endPos < workArea.Length Then
                    _length = endPos
                Else
                    endPosSpace = ""
                    _length = workArea.Length
                End If
            End If
            Return endPosSpace
        End Function

        Private Function SplitNameToLastNameFirstName(ByRef name As String) As String
            Dim result = ""
            Dim space() As Char = {" "}

            ' split at space
            Dim parts() As String = name.Split(space, StringSplitOptions.RemoveEmptyEntries)
            Dim validParts() As String = {}
            Dim postfix As String = ""
            If parts.Count < 2 Then 'OrElse parts.Count > 4
                Return name
            End If

            ' Check if each part contains at least a character
            Dim skipMode As Boolean = False
            For Each part In parts
                If Not skipMode Then
                    If Regex.Match(part, "[A-Za-z]").Length > 0 Then
                        ReDim Preserve validParts(validParts.Length)
                        validParts(validParts.Length - 1) = part
                    Else
                        skipMode = True
                    End If
                End If
                If skipMode Then
                    If postfix.Length > 0 Then
                        postfix = postfix + " "
                    End If
                    postfix = postfix + part
                End If
            Next
            parts = validParts
            validParts = {}
            If parts.Count < 2 Then 'OrElse parts.Count > 4
                Return name
            End If

            ' glue name prefixes to the trailings parts, e.g. van, Gogh => van Gogh
            Dim namePrefix As String = " "
            For Each part In parts
                If Char.IsLower(part.First()) Then
                    namePrefix = namePrefix + part + " "
                Else
                    ReDim Preserve validParts(validParts.Length)
                    validParts(validParts.Length - 1) = namePrefix + part
                    namePrefix = " "
                End If
            Next
            parts = validParts
            validParts = {}
            If parts.Count < 2 Then 'OrElse parts.Count > 4
                Return name
            End If

            'Finally: accept name
            result = parts(parts.Length - 1) + ","
            For Each part In parts.SkipLast(1)
                result = result + " " + part
            Next
            result = result + postfix

            Return result
        End Function

        Private Sub ComputeLastNameFirstNameSuggestion()

            Suggestion = OriginalAuthorsString

            ' Trivial cases
            If String.IsNullOrEmpty(OriginalAuthorsString) OrElse Not _originalAuthorsString.Contains(" ") Then
                Return
            End If

            Dim workArea = OriginalAuthorsString
            Dim prefix As String = ""
            Dim postfix As String = ""
            Dim endPosSpace As String = ""

            ' Check for start tokens (xxx:, by)
            DetermineStartIndex()
            prefix = OriginalAuthorsString.Substring(0, _startIndex)
            workArea = OriginalAuthorsString.Substring(_startIndex)

            Dim separator() As Char = GetSeparator(workArea, True)

            endPosSpace = DetermineLength(workArea, separator, True)

            postfix = workArea.Substring(_length)
            workArea = workArea.Substring(0, _length)

            Dim names() As String
            If separator.Length > 0 Then
                names = workArea.Split(separator, StringSplitOptions.RemoveEmptyEntries)
            Else
                names = {workArea}
            End If

            Dim canonicResult As String = ""
            Dim isFirst As Boolean = True
            For Each name In names
                If Not isFirst Then
                    canonicResult = canonicResult + "; "
                Else
                    isFirst = False
                End If
                canonicResult = canonicResult + SplitNameToLastNameFirstName(name)
            Next

            Suggestion = prefix + canonicResult + endPosSpace + postfix
        End Sub

        Private Function SplitNameToFirstNameLastName(name As String) As String
            Dim pos = name.IndexOf(",")
            If pos < 0 Then
                Return name
            End If

            ' Preston, Douglas  => Douglas Preston
            Dim firstName As String = ""
            Dim lastName As String = ""
            If pos < name.Length() - 1 Then
                firstName = name.Substring(pos + 1).Trim() + " "
            End If
            If pos > 0 Then
                lastName = name.Substring(0, pos).Trim()
            End If
            Return firstName + lastName
        End Function

        Private Sub ComputeFirstNameLastNameSuggestion()

            Suggestion = OriginalAuthorsString
            ' Trivial cases
            If String.IsNullOrEmpty(OriginalAuthorsString) OrElse Not _originalAuthorsString.Contains(",") Then
                Return
            End If

            Dim workArea = OriginalAuthorsString
            Dim prefix As String = ""
            Dim postfix As String = ""
            Dim endPosSpace As String = ""

            ' Check for start tokens (xxx:, by)
            DetermineStartIndex()
            prefix = OriginalAuthorsString.Substring(0, _startIndex)
            workArea = OriginalAuthorsString.Substring(_startIndex)

            Dim separator() As Char = GetSeparator(workArea, False)

            endPosSpace = DetermineLength(workArea, separator, False)

            postfix = workArea.Substring(_length)
            workArea = workArea.Substring(0, _length)

            Dim names() As String
            If separator.Length > 0 Then
                names = workArea.Split(separator, StringSplitOptions.RemoveEmptyEntries)
            Else
                names = {workArea}
            End If

            Dim canonicResult As String = ""
            Dim isFirst As Boolean = True
            For Each name In names
                If Not isFirst Then
                    canonicResult = canonicResult + "; "
                Else
                    isFirst = False
                End If
                canonicResult = canonicResult + SplitNameToFirstNameLastName(name)
            Next

            Suggestion = prefix + canonicResult + endPosSpace + postfix
        End Sub

        Public Sub ComputeSuggestion()
            If _conversionMode = ConversionMode.FirstNameLastName Then
                ComputeFirstNameLastNameSuggestion()
            Else
                ComputeLastNameFirstNameSuggestion()
            End If
        End Sub

    End Class

End Namespace
