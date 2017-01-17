Imports System.Xml.XPath
Imports umbraco.NodeExtensions
Imports System.Xml

Namespace WSC
    <umbraco.XsltExtension("wsc.library")>
    Public Class Library
        Public Shared Function JsonToXml(json As String) As XPathNodeIterator
            Try
                If json.StartsWith("[") Then
                    '--we'll assume it's an array, in which case we need to add a root
                    json = "{""arrayitem"":" + json + "}"
                End If
                Dim xml = Newtonsoft.Json.JsonConvert.DeserializeXmlNode(json, "json", False)
                Return xml.CreateNavigator().Select("/json")
            Catch ex As Exception
                Dim xd = New XmlDocument()
                xd.LoadXml(String.Format("<error>Could not convert JSON to XML. Error: {0}</error>", ex))
                Return xd.CreateNavigator().Select("/error")
            End Try

        End Function
    End Class

End Namespace