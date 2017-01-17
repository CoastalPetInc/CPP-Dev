Imports Microsoft.VisualBasic
Imports umbraco.Core
Imports umbraco.Core.Services

Namespace WSC
    Public Class Events
        Implements IApplicationEventHandler

        Public Sub OnApplicationInitialized(umbracoApplication As UmbracoApplicationBase, applicationContext As ApplicationContext) Implements IApplicationEventHandler.OnApplicationInitialized
        End Sub

        Public Sub OnApplicationStarted(umbracoApplication As UmbracoApplicationBase, applicationContext As ApplicationContext) Implements IApplicationEventHandler.OnApplicationStarted
        End Sub

        Public Sub OnApplicationStarting(umbracoApplication As UmbracoApplicationBase, applicationContext As ApplicationContext) Implements IApplicationEventHandler.OnApplicationStarting
            AddHandler ContentService.Saving, AddressOf Content_Saving
            AddHandler Global.Examine.ExamineManager.Instance.IndexProviderCollection("ExternalIndexer").GatheringNodeData, AddressOf GatheringNodeData
        End Sub

        Private Sub GatheringNodeData(sender As Object, e As Global.Examine.IndexingNodeDataEventArgs)
            Dim path = e.Fields("path")
            '--First add rootID
            e.Fields.Add("rootID", path.Split(",")(1))
            '--Remove commas to make it searchable
            path = path.Replace(",", " ")
            e.Fields.Add("searchPath", path)

            'If Not e.Fields.ContainsKey("umbracoNaviHide") Then
            '    e.Fields.Add("umbracoNaviHide", 0)
            'End If
        End Sub

        Private Sub Content_Saving(sender As IContentService, e As umbraco.Core.Events.SaveEventArgs(Of Models.IContent))
            For Each c In e.SavedEntities
                Dim versions = sender.GetVersions(c.Id)
                '--If first save
                If versions.Count = 0 Then
                    '--Default values
                    If c.HasProperty("metaPageTitle") Then
                        c.SetValue("metaPageTitle", c.Name)
                    End If
                End If
            Next
        End Sub



    End Class
End Namespace

