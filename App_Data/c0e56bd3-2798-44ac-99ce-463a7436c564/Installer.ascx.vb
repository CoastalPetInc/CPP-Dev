Imports System.Xml

Partial Class WSC_Installer
    Inherits System.Web.UI.UserControl
    Dim webConfig As XmlDocument
    Dim modified As Boolean = False

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim FULL_PATH As String = HttpContext.Current.Server.MapPath("/web.config")
        '--Keep current indentions format
        webConfig = umbraco.xmlHelper.OpenAsXmlDocument(FULL_PATH)
        webConfig.PreserveWhitespace = True

        AddUrlRewrite()

        AddHttpModule("SSLSwitcher", "WSC.SSLSwitcher, WSC.SSLSwitcher")
        AddHttpModule("_301Redirect", "WSC._301Redirect, WSC.301Redirect")

        AddCustomError()
        AddImageResizer()

        If modified Then
            Try
                '--Save the Rewrite config file with the new rewerite rule
                webConfig.Save(FULL_PATH)
            Catch ex As Exception
                '--Log error message
                'Dim message As String = "Error at execute AddHttpModule package action: " + e.Message
            End Try
        End If
    End Sub

    Sub CreateMemberConfig()
        Dim mg = umbraco.cms.businesslogic.member.MemberGroup.MakeNew("Member", umbraco.BusinessLogic.User.GetCurrent())
        mg.Save()
    End Sub

    Sub AddCustomError()
        Dim newRuleXML As String = "<httpErrors>" & _
                                        "<remove statusCode='404' subStatusCode='-1' />" & _
                                        "<error statusCode='404' prefixLanguageFilePath='' path='/404-file-not-found.aspx' responseMode='ExecuteURL' />" & _
                                   "</httpErrors>"

        '--Select root node in the web.config file for insert new nodes
        Dim webServerNode As XmlNode = webConfig.SelectSingleNode("//configuration/system.webServer")
        If webServerNode Is Nothing Then Exit Sub

        If webServerNode.SelectSingleNode("httpErrors") Is Nothing Then
            Dim xmlData As New XmlDocument()
            xmlData.LoadXml(newRuleXML)
            Dim node As XmlElement = webConfig.ImportNode(xmlData.SelectSingleNode("//httpErrors"), True)
            webServerNode.AppendChild(node)
            modified = True
        End If
    End Sub

    Sub AddUrlRewrite()
        Const NAMESPACEURI As String = "http://www.urlrewriting.net/schemas/config/2006/07"
        Dim filePath As String = System.Web.HttpContext.Current.Server.MapPath("/config/UrlRewriting.config")

        Dim newRuleXML As String = "<root>" & _
                                        "<add name='Admin' redirect='Application' redirectMode='Permanent' rewriteUrlParameter='ExcludeFromClientQueryString' virtualUrl='~/admin[/]?$' destinationUrl='~/umbraco/umbraco.aspx' ignoreCase='true' />" & _
                                        "<add name='Sitemap' virtualUrl='^~/sitemap.xml' rewriteUrlParameter='ExcludeFromClientQueryString' destinationUrl='~/sitemapxml.aspx' ignoreCase='true' />" & _
                                   "</root>"

        Dim xmlData As New XmlDocument()
        xmlData.LoadXml(newRuleXML)

        '--Open the URL Rewrite config file
        Dim rewriteFile As XmlDocument = umbraco.xmlHelper.OpenAsXmlDocument(filePath)
        rewriteFile.PreserveWhitespace = True

        '--Initialize a namespace Manager that adds the namespaces to the umbracoSettingsFile Xml Document 
        Dim xmlnsManager As New System.Xml.XmlNamespaceManager(rewriteFile.NameTable)
        xmlnsManager.AddNamespace("urlrewritingnet", NAMESPACEURI)
        xmlnsManager.AddNamespace("", NAMESPACEURI)

        '--Select rewrites node in the config file and append the importNode
        Dim rewriteRootNode As XmlNode = rewriteFile.SelectSingleNode("//urlrewritingnet:rewrites", xmlnsManager)

        '--Add ForceWWW as comment
        rewriteRootNode.AppendChild(rewriteFile.CreateComment("<add name='forceWWW' redirect='Domain' redirectMode='Permanent' rewrite='Domain' virtualUrl='^(http[s]?://){1}(?!www\.)([^.]+\.[^.]+)$' rewriteUrlParameter='ExcludeFromClientQueryString' destinationUrl='$1www.$2' ignoreCase='true' />"))
        'rewriteRootNode.AppendChild(rewriteFile.CreateComment("<add name='forceWWW' redirect='Domain' redirectMode='Permanent' rewrite='Domain' virtualUrl='^(http[s]?://){1}(?!www\.)(.+)$' rewriteUrlParameter='ExcludeFromClientQueryString' destinationUrl='$1www.$2' ignoreCase='true' />"))
        'rewriteRootNode.AppendChild(rewriteFile.CreateComment("<add name='forceWWW' redirect='Domain' redirectMode='Permanent' rewrite='Domain' virtualUrl='^(http[s]?://){1}([^/.]+).([^/.]+)$' rewriteUrlParameter='ExcludeFromClientQueryString' destinationUrl='$1www.$2.$3' ignoreCase='true' />"))

        '--Select RewriteRuleNode(s) from the supplied xmlData
        For Each rewriteRuleNode As XmlNode In xmlData.SelectNodes("//add")
            '--Create a new Rewrite rule node 
            Dim newRewriteRuleNode As XmlNode = DirectCast(rewriteFile.CreateElement("add", NAMESPACEURI), XmlNode)
            '--Copy all attributes from the package xml to the new Rewrite rule node
            For Each att As XmlAttribute In rewriteRuleNode.Attributes
                Dim attribute As XmlAttribute = rewriteFile.CreateAttribute(att.Name)
                attribute.Value = att.Value
                newRewriteRuleNode.Attributes.Append(attribute)
            Next
            '--Append the new rewrite rule node to the Rewrite config file
            rewriteRootNode.AppendChild(newRewriteRuleNode)
        Next

        '--Save the Rewrite config file with the new rewerite rule
        rewriteFile.Save(filePath)
    End Sub

    Sub AddHttpModule(ByVal name As String, ByVal type As String)
        '--Select root node in the web.config file for insert new nodes
        '--First try IIS-7
        Dim rootNode As XmlNode = webConfig.SelectSingleNode("//configuration/system.webServer/modules")
        '--Try IIS-6
        If rootNode Is Nothing Then
            rootNode = webConfig.SelectSingleNode("//configuration/system.web/httpModules")
        End If

        '--Check if rootNode exists
        If rootNode Is Nothing Then Exit Sub

        '--Set insert node default true
        Dim insertNode As Boolean = True
        '--Look for existing nodes with same name like the new node
        If rootNode.HasChildNodes Then
            '--Look for existing nodeType nodes
            Dim node As XmlNode = rootNode.SelectSingleNode(String.Format("//add[@name = '{0}']", name))
            '--If name already exists 
            If node IsNot Nothing Then
                '--Cancel insert node operation
                insertNode = False
            End If
        End If
        '--Check for insert flag
        If insertNode Then
            '--Create new node with attributes
            Dim newNode As XmlNode = webConfig.CreateElement("add")
            Dim a As XmlAttribute
            a = webConfig.CreateAttribute("name")
            a.Value = name
            newNode.Attributes.Append(a)
            a = webConfig.CreateAttribute("type")
            a.Value = type
            newNode.Attributes.Append(a)
            '--Add new node 
            rootNode.AppendChild(newNode)
            '--Mark document modified
            modified = True
        End If
    End Sub

    Sub AddImageResizer()
        AddHttpModule("ImageResizingModule", "ImageResizer.InterceptModule")

        Dim configSections As XmlNode = webConfig.SelectSingleNode("//configuration/configSections")
        If configSections Is Nothing Then Exit Sub

        Dim xmlData As New XmlDocument()

        xmlData.LoadXml("<section name=""resizer"" type=""ImageResizer.ResizerSection,ImageResizer""  requirePermission=""false""  />")
        Dim node As XmlElement = webConfig.ImportNode(xmlData.DocumentElement, True)
        configSections.AppendChild(node)

        Dim resizerXML As String = "<resizer>" & _
                                    "<!-- Unless you (a) use Integrated mode, or (b) map all reqeusts to ASP.NET, " & _
                                    "     you'll need to add .ashx to your image URLs: image.jpg.ashx?width=200&height=20 -->" & _
                                    "    <pipeline fakeExtensions='.ashx' />" & _
                                    "    <plugins>" & _
                                    "        <!--<add name='PrettyGifs' />-->" & _
                                    "        <add name='MvcRoutingShim' />" & _
                                    "        <add name='SimpleCache' />" & _
                                    "    </plugins>" & _
                                    "	 <simplecache debug='false' disabled='false' defaultDirectory='/media/cache/' useSourceDirectory='true' />" & _
                                    "</resizer>"

        xmlData.LoadXml(resizerXML)
        webConfig.DocumentElement.AppendChild(webConfig.ImportNode(xmlData.DocumentElement, True))

        modified = True
    End Sub

End Class
