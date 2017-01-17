Imports umbraco.cms.businesslogic.web
Imports umbraco.editorControls
Imports umbraco.controls
Imports System.Xml
Imports umbraco.BusinessLogic
Imports System.Collections.Generic

Partial Class usercontrols_SitemapImport
    Inherits System.Web.UI.UserControl
    Dim p As New ContentPicker()
    Dim ignore As New List(Of String)
    Dim propMap As New Hashtable()
    Dim updateCount As Integer = 0
    Dim errors As New StringBuilder()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        propMap.Add("meta_page_title", "metaPageTitle")
        propMap.Add("meta_decription", "metaDescription")
        propMap.Add("meta_keywords", "metaKeywords")
        propMap.Add("content", "bodyText")

        pagePickerPlaceHolder.Controls.Add(p)
    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        ignore.AddRange(txtIgnore.Text.ToString.Split(","))

        '--Check for file
        If Not uplFile.HasFile Then Exit Sub

        Dim doc As New XmlDocument()
        doc.Load(uplFile.PostedFile.InputStream)

        Dim sb As New StringBuilder()
        Dim parentId As String = p.Value.ToString()
        If String.IsNullOrEmpty(parentId) Then parentId = "-1"
        
        If parentId <> "-1" Then
            Try
                Dim parent As New Document(parentId)
            Catch ex As Exception
                Exit Sub
            End Try
        End If

        Dim xPath As String = txtXPath.Text
        If String.IsNullOrEmpty(xPath) Then xPath = "sitemap/page"

        Dim dtAlias As String
        For Each node As XmlNode In doc.SelectNodes(xPath)
            If node.Attributes("level").Value = "1" Then
                dtAlias = "Homepage"
            Else
                dtAlias = "TextPage"
            End If

            CreateDocument(node, parentId, dtAlias)
        Next

        Dim msg As New Control()
        msg.Controls.Add(New LiteralControl(String.Format("<br /><div class='success'>Added/Updated {0} Nodes</div>", updateCount)))
        If errors.Length > 0 Then
            msg.Controls.Add(New LiteralControl(String.Format("<div class='error'>{0}</div>", errors.ToString())))
        End If

        Me.Controls.AddAt(0, msg)

    End Sub

    Sub CreateDocument(ByVal node As XmlNode, ByVal parentId As Integer, ByVal dtAlias As String)
        Dim dt As DocumentType = DocumentType.GetByAlias(dtAlias)
        Dim u As User = umbraco.BusinessLogic.User.GetCurrent()
        Dim name As String = node.Attributes("name").Value.ToString()

        If ignore.Contains(name) Then Exit Sub

        'Dim d As Document = Document.MakeNew(name, dt, u, parentId)
        Dim d As Document = GetDocument(name, dt, u, parentId)
        
        For Each prop As DictionaryEntry In propMap
            Dim value As String = node.SelectSingleNode(prop.Key).InnerText.ToString()
            If prop.Key = "content" And node.Attributes("type").Value = "Form" Then
                value = String.Format("<pre>{0}</pre>", node.SelectSingleNode(prop.Key).InnerXml.ToString())
            End If

            AddProperty(d, prop.Value, value)
        Next

        d.Save()
        updateCount += 1

        'Response.Write(String.Format("Added ({2}): {1}{0}", vbCrLf, name, dtAlias))
        For Each n As XmlNode In node.SelectNodes("page")
            CreateDocument(n, d.Id, "TextPage")
        Next
    End Sub

    Sub AddProperty(ByRef d As Document, ByVal propertyAlias As String, ByVal value As String)
        Try
            d.getProperty(propertyAlias).Value = value
        Catch ex As Exception
            errors.AppendFormat("Error adding {0} to {1}", propertyAlias, d.Text)
        End Try
    End Sub

    Function GetDocument(ByVal name As String, ByVal dt As DocumentType, ByVal u As User, ByVal parentId As Integer) As Document
        If parentId > 0 And chbUpdate.Checked Then
            Dim parent As New Document(parentId)
            For Each d As Document In parent.Children
                If d.Text.CompareTo(name) = 0 Then
                    Return d
                End If
            Next
            Return Document.MakeNew(name, dt, u, parentId)
        End If

        Return Document.MakeNew(name, dt, u, parentId)
    End Function

End Class
