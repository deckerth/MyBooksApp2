Imports System.Xml

Namespace Global.MyBooks.OCLCClassify

    Public Class FileOCLCXml
        Implements IEnumerator, IEnumerable

        Public Shared ReadOnly Property OCLCNamespace As XNamespace = "http://classify.oclc.org"

        ' source containing New records
        Private _rawSource As List(Of XElement)
        Private _position As Integer = -1

        ' Gets the raw source.
        ' <value>The raw source.</value>
        Public ReadOnly Property RawSource As List(Of XElement)
            Get
                Return _rawSource
            End Get
        End Property

        Private _numberOfEntries As Integer
        Public ReadOnly Property NumberOfEntries As Integer
            Get
                Return _numberOfEntries
            End Get
        End Property

        Private _workCount As Integer
        Public ReadOnly Property WorkCount As Integer
            Get
                Return _workCount
            End Get
        End Property

        Private _maxRecs As Integer
        Public ReadOnly Property MaxRecs As Integer
            Get
                Return _maxRecs
            End Get
        End Property

        Private _nextRecordPosition As Integer
        Public ReadOnly Property NextRecordPosition As Integer
            Get
                Return _nextRecordPosition
            End Get
        End Property

        Default Public ReadOnly Property Item(index As Integer) As Work
            Get
                Return Decode(index)
            End Get
        End Property

        Public ReadOnly Property Count
            Get
                Return RawSource.Count
            End Get
        End Property

        Public Sub New(source As XDocument)
            Me.New()
            Add(source)
        End Sub

        Public Sub New(source As String)
            Me.New(XDocument.Parse(source))
        End Sub

        Public Sub New()
            _rawSource = New List(Of XElement)
        End Sub

        ' Imports the XML records from a file.
        Public Sub ImportOCLCXml(file As String)
            Dim xmlSource As XDocument = XDocument.Load(file)
            Add(xmlSource)
        End Sub

        Public ReadOnly Property Current As Object Implements IEnumerator.Current
            Get
                Return Item(_position)
            End Get
        End Property

        Public Sub Reset() Implements IEnumerator.Reset
            _position = -1
        End Sub

        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            _position = _position + 1
            Return _position < _numberOfEntries
        End Function

        Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return Me
        End Function

        Public Function Add(source As XDocument) As Integer
            Dim addCount As Integer

            '<classify xmlns="http://classify.oclc.org">
            '	<response code="4"/>
            '		<!--Classify is a product of OCLC Online Computer Library Center: http://classify.oclc.org-->
            '	<workCount>3</workCount>
            '	<start>0</start>
            '	<maxRecs>25</maxRecs>
            '	<orderBy>thold desc</orderBy>
            '	<input type="title/author">everything+is+miscellaneous|Weinberger%2C+David</input>
            '	<works>
            '		<work title="Everything is miscellaneous : the power of the new digital disorder" wi="122291427" schemes="DDC LCC" owi="198186770" lyr="2007" itemtype="itemtype-book" hyr="2013" holdings="2054" format="Book" editions="23" author="Weinberger, David, 1950-"/>
            '		<work title="Everything is miscellaneous the power of the new digital disorder" wi="943348356" schemes="DDC" owi="3902701440" lyr="2015" itemtype="itemtype-compfile" hyr="2015" holdings="1" format="Computer file" editions="1" author="Weinberger, David, 1950-"/>
            '		<work title="Luan shi yi zhong xing shang ji : shu wei xing wei gai xie de xiao fei xi guan = Everything is miscellaneous : the power of the new digital disorder" wi="232552408" schemes="DDC" owi="3769108008" lyr="2007" itemtype="itemtype-book" hyr="2007" holdings="1" format="Book" editions="1" author="Weinberger, David, 1950-"/>
            '	</works>
            '</classify>

            For Each classifyResult As XElement In source.Elements().Where(Function(e) e.Name.LocalName = "classify")
                For Each workCount As XElement In classifyResult.Elements().Where(Function(e) e.Name.LocalName = "workCount")
                    _workCount = Convert.ToInt32(workCount.FirstNode.ToString())
                Next

                For Each max As XElement In classifyResult.Elements().Where(Function(e) e.Name.LocalName = "maxRecs")
                    _maxRecs = Convert.ToInt32(max.FirstNode.ToString())
                Next

                For Each works As XElement In classifyResult.Elements().Where(Function(e) e.Name.LocalName = "works")
                    For Each work As XElement In works.Elements().Where(Function(e) e.Name.LocalName = "work")
                        _rawSource.Add(work)
                        addCount = addCount + 1
                    Next
                Next

                For Each editions As XElement In classifyResult.Elements().Where(Function(e) e.Name.LocalName = "editions")
                    For Each edition As XElement In editions.Elements().Where(Function(e) e.Name.LocalName = "edition")
                        _rawSource.Add(edition)
                        addCount = addCount + 1
                    Next
                Next

                For Each work As XElement In classifyResult.Elements().Where(Function(e) e.Name.LocalName = "work")
                        _rawSource.Add(work)
                        addCount = addCount + 1
                    Next
                Next

                _numberOfEntries = addCount

            Return addCount
        End Function

        Public Function Add(source As String) As Integer
            Return Add(XDocument.Parse(text:=source))
        End Function

        Private Function Decode(index As Integer) As Work

            Dim workXml As XElement = RawSource(index)
            Dim work As New Work

            '		<work title="Everything is miscellaneous : the power of the new digital disorder" 
            '             wi="122291427" -> OCLCNumber 
            '             schemes="DDC LCC" 
            '             owi="198186770" 
            '             lyr="2007" 
            '             itemtype="itemtype-book" 
            '             hyr="2013" 
            '             holdings="2054" 
            '             format="Book" 
            '             editions="23" 
            '             author="Weinberger, David, 1950-"/>

            For Each e In workXml.Attributes()
                Select Case e.Name.LocalName
                    Case "wi", "owi", "oclc"
                        work.OCLCNo = e.Value.ToString()
                    Case "lyr"
                        work.PublishYear = e.Value.ToString()
                    Case "itemtype"
                        work.ItemType = e.Value.ToString()
                    Case "title"
                        work.Title = e.Value.ToString()
                    Case "author"
                        work.FullAuthor = e.Value.ToString()
                        work.Author = ""
                        ' author="Preston, Douglas J. | Child, Lincoln [Illustrator; Author] | Murillo, Eduardo G. [Translator]"/>
                        For Each author In work.FullAuthor.Split("|")
                            Dim roleIndex As Integer = author.IndexOf("[")
                            Dim nameLength As Integer
                            If roleIndex >= 0 Then
                                nameLength = roleIndex
                            Else
                                nameLength = author.Length
                            End If
                            Dim words As String() = author.Substring(0, nameLength).Split(",")
                            Dim authorName As String
                            If words.Count >= 2 Then
                                authorName = words(1) + " " + words(0)
                            Else
                                authorName = author
                            End If
                            If work.Author.Length > 0 Then
                                work.Author = work.Author + ", "
                            End If
                            work.Author = work.Author + authorName
                            If roleIndex >= 0 Then
                                work.Author = work.Author + " " + author.Substring(roleIndex)
                            End If
                        Next
                End Select
            Next

            Return work
        End Function

    End Class

End Namespace
