Imports Microsoft.VisualBasic
Imports umbraco.NodeFactory
Imports uComponents.Core
Imports uComponents.Core.uQueryExtensions
Imports System.Linq

Namespace WSC
    Public Class UmbracoDefault
        Inherits umbraco.UmbracoDefault

        Protected Overrides Sub OnLoadComplete(e As System.EventArgs)
            If Page.Header IsNot Nothing Then
                Dim n As Node = Node.GetCurrent()
                Dim title As String = GetPropertyRecursive(n, "metaPageTitle")
                If n.NodeTypeAlias = "BlogPost" Then
                    title = n.Name
                End If
                If n.NodeTypeAlias = "Blog" Then
                    title = n.GetProperty(Of String)("blogName")
                End If
                If Not String.IsNullOrEmpty(Page.Title) Then
                    title = Page.Title
                End If

                Dim siteName As String = GetPropertyRecursive(n, "siteName")
                'Dim sep As String = If(String.IsNullOrEmpty(title), String.Empty, " - ")
                'Page.Title = String.Format("{0}{1}{2}", title, sep, siteName)
                If String.IsNullOrEmpty(title) Then title = siteName
                Page.Title = title


                If String.IsNullOrEmpty(Page.MetaDescription) Then
                    Page.MetaDescription = GetPropertyRecursive(n, "metaDescription")
                End If

                If String.IsNullOrEmpty(Page.MetaKeywords) Then
                    Page.MetaKeywords = GetPropertyRecursive(n, "metaKeywords")
                End If

            End If
            MyBase.OnLoadComplete(e)
        End Sub

        Private Function GetPropertyRecursive(n As Node, propertyAlias As String) As String
            Return n.GetAncestorOrSelfNodes().FirstOrDefault(Function(x) Not String.IsNullOrEmpty(x.GetProperty(Of String)(propertyAlias))).GetProperty(Of String)(propertyAlias)
        End Function



    End Class
End Namespace

