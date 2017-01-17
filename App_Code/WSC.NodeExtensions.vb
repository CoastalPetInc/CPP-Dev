Imports System.Runtime.CompilerServices
Imports umbraco.NodeFactory

Public Module NodeExtensions
    <Extension()>
    Public Function DisplayName(n As Node) As String
        Dim p As [Property] = n.Properties("pageNavigationName")
        If p IsNot Nothing AndAlso Not String.IsNullOrEmpty(p.Value) Then
            Return p.Value
        End If
        Return n.Name
    End Function

    <Extension()>
    Public Function IsHidden(ByVal n As Node) As Boolean
        Dim p As [Property] = n.GetProperty("umbracoNaviHide")
        If p IsNot Nothing Then
            Return p.Value = "1"
        End If
        Return False
    End Function
End Module
