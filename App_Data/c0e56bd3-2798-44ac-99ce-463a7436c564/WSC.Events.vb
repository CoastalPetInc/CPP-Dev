Imports umbraco.Core
Imports umbraco.Core.Services
Imports umbraco.Core.Events
Imports umbraco.Core.Models

Namespace WSC
    Public Class Events
        Implements IApplicationEventHandler

        Public Sub OnApplicationInitialized(umbracoApplication As UmbracoApplicationBase, applicationContext As ApplicationContext) Implements IApplicationEventHandler.OnApplicationInitialized

        End Sub

        Public Sub OnApplicationStarting(umbracoApplication As UmbracoApplicationBase, applicationContext As ApplicationContext) Implements IApplicationEventHandler.OnApplicationStarting

        End Sub

        Public Sub OnApplicationStarted(umbracoApplication As UmbracoApplicationBase, applicationContext As ApplicationContext) Implements IApplicationEventHandler.OnApplicationStarted
            AddHandler ContentService.Saving, AddressOf Content_Saving
        End Sub

        Private Sub Content_Saving(sender As IContentService, e As SaveEventArgs(Of IContent))
            Dim c = e.SavedEntities(0)
            If Not c.HasPublishedVersion Then
                If c.HasProperty("metaPageTitle") Then
                    c.SetValue("metaPageTitle", c.Name)
                End If

                If c.ContentType.Alias = "Job" Then
                    c.SetValue("postDate", Today)
                End If
            End If
        End Sub

    End Class
End Namespace