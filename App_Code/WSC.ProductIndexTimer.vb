Imports Microsoft.VisualBasic
Imports umbraco.Core
Imports umbraco.Web.BaseRest
Imports WSC.Extensions.ExamineExtensions

Namespace WSC.ProductIndexTimer
    Public Class Events
        Implements IApplicationEventHandler

        Public Shared ExpireHour As Integer = 3
        Private ExpireDate As Date

        Public Sub OnApplicationInitialized(umbracoApplication As UmbracoApplicationBase, applicationContext As ApplicationContext) Implements IApplicationEventHandler.OnApplicationInitialized
        End Sub

        Public Sub OnApplicationStarted(umbracoApplication As UmbracoApplicationBase, applicationContext As ApplicationContext) Implements IApplicationEventHandler.OnApplicationStarted
            StartTimer()
            StartUp()
        End Sub

        Public Sub OnApplicationStarting(umbracoApplication As UmbracoApplicationBase, applicationContext As ApplicationContext) Implements IApplicationEventHandler.OnApplicationStarting
        End Sub


        Public Sub StartTimer()
            If HttpContext.Current Is Nothing Then Exit Sub
            If HttpContext.Current.Cache Is Nothing Then Exit Sub

            Me.ExpireDate = Now.Date
            If Now.Hour >= Events.ExpireHour Then Me.ExpireDate = Me.ExpireDate.AddDays(1)
            Me.ExpireDate = Me.ExpireDate.AddHours(Events.ExpireHour)

            umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.Events)("Expire at " & Me.ExpireDate)
            umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.Events)("HttpContext: " & (HttpContext.Current Is Nothing))
            umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.Events)("Cache: " & (HttpContext.Current.Cache Is Nothing))

            HttpContext.Current.Cache.Add("ExamineTimer", "True", Nothing, ExpireDate, Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, AddressOf ExpireCallBack)
        End Sub

        Public Sub StartUp()
            Dim updateHours = DateDiff(DateInterval.Hour, ProductIndex.LastUpdate, Now)
            Dim docCount = ProductIndex.DocumentCount
            umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.Events)("StartUp - Cache Last Updated: " & updateHours)
            umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.Events)("StartUp - DocumentCount: " & docCount)
            If updateHours > 24 Or (docCount = 0 AndAlso updateHours > 1) Then
                umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.Events)("StartUp Rebuild")
                ProductIndex.RebuildRemote()
            End If
        End Sub

        Public Sub ExpireCallBack(k As String, v As Object, r As CacheItemRemovedReason)
            umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.Events)("Cache Removed: " & r)
            If r <> CacheItemRemovedReason.Expired Then Return

            'If HttpContext.Current Is Nothing Then
            '	umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.Events)("Cache Expired: HttpContext.Current")
            '	Exit Sub
            'End If
            '         If HttpContext.Current.Cache Is Nothing Then
            '	umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.Events)("Cache Expired: HttpContext.Current.Cache")
            '	Exit Sub
            'End If

            Dim updateHours = DateDiff(DateInterval.Hour, WSC.ProductIndexTimer.ProductIndex.LastUpdate, Now)
            umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.Events)("Cache Last Updated: " & updateHours)
            If updateHours > 24 Or Now.Hour = Events.ExpireHour Then
                umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.Events)("Cache Expired")
                'ProductIndex.Rebuild()
                ProductIndex.RebuildRemote()
            Else
                umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.Events)("Cache Expired: not the right hour")
            End If

            StartTimer()
        End Sub
    End Class

    Public Class ProductIndex
        Private Shared ReadOnly Property Indexer As Global.Examine.Providers.BaseIndexProvider
            Get
                Return Global.Examine.ExamineManager.Instance.IndexProviderCollection("ProductIndexer")
            End Get
        End Property

        Public Shared Sub RebuildRemote()
            Try
                Dim wc As New Net.WebClient()
                Dim defaultDomain = "http://www.coastalpet.com"
                If ConfigurationManager.AppSettings("Environment") = "DEV" Then
                    defaultDomain = "http://dev.coastalpet.com"
                End If
                Dim result = wc.DownloadString(defaultDomain & "/Base/ProductIndex/Rebuild")
                umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.ProductIndex)(result)
            Catch ex As Exception
                umbraco.Core.Logging.LogHelper.Error(Of WSC.ProductIndexTimer.ProductIndex)("RebuildRemote", ex)
            End Try
        End Sub

        Public Shared Sub Rebuild()
            Indexer.RebuildIndex()
            umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.ProductIndex)("Rebuild Examine ProductIndexer")
        End Sub

        Public Shared Function LastUpdate() As Date
            'Dim path = System.IO.Path.Combine(Global.Examine.LuceneEngine.Config.IndexSets.Instance.Sets("ProductIndexSet").IndexDirectory.ToString, "index")
            'If Not System.IO.Directory.Exists(path) Then Return Date.MinValue
            'Return System.IO.Directory.GetLastWriteTime(path)
            Return Indexer.LastUpdate
        End Function

        Public Shared ReadOnly Property DocumentCount As Integer
            Get
                Return Indexer.GetIndexDocumentCount()
            End Get
        End Property


    End Class

    <RestExtension("ProductIndex")>
    Public Class Base
        <RestExtensionMethod(ReturnXml:=False)>
        Public Shared Function Rebuild() As String
            Dim sw As New Diagnostics.Stopwatch
            sw.Start()
            Try
                ProductIndex.Rebuild()
            Catch ex As Exception
                Return ex.Message
            End Try
            sw.Stop()
            umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.Base)("Manual Rebuild (" & sw.ElapsedMilliseconds & "ms)")
            Return "Index Rebuilt " & ProductIndex.LastUpdate & " (" & sw.ElapsedMilliseconds & "ms)"
        End Function

        <RestExtensionMethod(ReturnXml:=False)>
        Public Shared Function ScheduledRebuild() As String
            Dim updateHours = DateDiff(DateInterval.Hour, WSC.ProductIndexTimer.ProductIndex.LastUpdate, Now)
            'umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.Base)("Cache Last Updated: " & updateHours)
            If updateHours > 24 Or Now.Hour = Events.ExpireHour Then
                Dim sw As New Diagnostics.Stopwatch
                sw.Start()
                ProductIndex.Rebuild()
                sw.Stop()
                Return "Index Rebuilt " & ProductIndex.LastUpdate & " (" & sw.ElapsedMilliseconds & "ms)"
                umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.Base)("ScheduledRebuild (" & sw.ElapsedMilliseconds & "ms)")
            Else
                'umbraco.Core.Logging.LogHelper.Info(Of WSC.ProductIndexTimer.Base)("Cache Expired: not the right hour")
            End If

            Return "not the right hour "
        End Function

    End Class
End Namespace
