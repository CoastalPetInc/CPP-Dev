Imports Microsoft.VisualBasic
Imports Examine
Imports Examine.LuceneEngine
Imports umbraco.Core.Logging
Imports WSC.Datalayer
Imports umbraco.Core.TypeExtensions
Imports System.IO

Namespace WSC.Examine
    Public Class ProductDataIndexer
        Inherits Global.Examine.LuceneEngine.Providers.LuceneIndexer

        Property DataService As ISimpleDataService
        Property IndexTypes As New List(Of String)

        Private Shared Locker As Object

        Sub New()
            MyBase.New()
            LogHelper.Info(Of ProductDataIndexer)("New()")

            Me.DataService = New ProductDataService()
            Me.IndexTypes.Add("ProductData")

            ProductDataIndexer.Locker = New Object()
        End Sub

        Public Sub New(ByVal indexerData As IIndexCriteria, ByVal workingFolder As DirectoryInfo, ByVal analyzer As Lucene.Net.Analysis.Analyzer, ByVal dataService As ISimpleDataService, ByVal indexTypes As IEnumerable(Of String), ByVal async As Boolean)
            MyBase.New(indexerData, workingFolder, analyzer, async)
            LogHelper.Info(Of ProductDataIndexer)("New(with params)")
            Me.DataService = dataService
            Me.IndexTypes = indexTypes.ToList

            ProductDataIndexer.Locker = New Object()
        End Sub

        Protected Overrides Sub PerformIndexAll(type As String)
            'LogHelper.Info(Of ProductDataIndexer)("PerformIndexAll(" & type & ")")
            Dim allData As IEnumerable(Of SimpleDataSet) = Me.DataService.GetAllData(type)
            Dim nodes As New List(Of XElement)
            For Each ds As SimpleDataSet In allData
                nodes.Add(ds.RowData.ToExamineXml(ds.NodeDefinition.NodeId, ds.NodeDefinition.Type))
            Next
            'Dim nodes As List(Of XElement) = allData.Select(Function(ds) ds.RowData.ToExamineXml(ds.NodeDefinition.NodeId, ds.NodeDefinition.Type)).ToList
            MyBase.AddNodesToIndex(nodes, type)
        End Sub

        Protected Overrides Sub PerformIndexRebuild()
            'LogHelper.Info(Of ProductDataIndexer)("PerformIndexRebuild: " & String.Join(",", Me.IndexTypes.ToArray))
            For Each s In Me.IndexTypes
                Me.IndexAll(s)
            Next
        End Sub


        ''' <summary>
        ''' A type that defines the type of index for each Umbraco field (non user defined fields)
        ''' Alot of standard umbraco fields shouldn't be tokenized or even indexed, just stored into lucene
        ''' for retreival after searching.
        ''' </summary>
        Friend Shared ReadOnly IndexFieldPolicies As New Dictionary(Of String, FieldIndexTypes)() From {}

        Protected Overrides Function GetPolicy(fieldName As String) As FieldIndexTypes
            If IndexFieldPolicies.ContainsKey(fieldName) Then
                'LogHelper.Info(Of ProductDataIndexer)("GetPolicy(" & fieldName & "):" & ret.ToString)
                Return IndexFieldPolicies.Item(fieldName)
            End If
            Return MyBase.GetPolicy(fieldName)
        End Function


        Protected Overrides Function GetIndexerData(indexSet As Global.Examine.LuceneEngine.Config.IndexSet) As Global.Examine.IIndexCriteria
            If (indexSet.IndexUserFields.Count = 0) Then
                'LogHelper.Info(Of ProductDataIndexer)("GetIndexerData")
                SyncLock ProductDataIndexer.Locker
                    For Each str As String In ProductDataService.GetAllUserPropertyNames()
                        Dim field As New Global.Examine.LuceneEngine.Config.IndexField()
                        field.Name = str
                        'If str = "ParentID" Then
                        '    field.Type = "INT"
                        'End If
                        indexSet.IndexUserFields.Add(field)
                    Next
                    'For Each field As IIndexField In svc.GetAllUserPropertyNames()
                    '    indexSet.IndexUserFields.Add(field)
                    'Next
                End SyncLock
            End If

            'LogHelper.Info(Of ProductDataIndexer)("GetIndexerData:" & indexSet.IndexUserFields.Count)

            Return MyBase.GetIndexerData(indexSet)


            'Dim standardFields = indexSet.IndexAttributeFields.Cast(Of IIndexField)().ToList()
            'Dim userFields = indexSet.IndexUserFields.Cast(Of IIndexField)().ToList()
            'Dim includeNodeTypes As New List(Of String)
            'For Each t In indexSet.IncludeNodeTypes
            '    includeNodeTypes.Add(t.Name)
            'Next
            'Dim excludeNodeTypes As New List(Of String)
            'For Each t In indexSet.ExcludeNodeTypes
            '    excludeNodeTypes.Add(t.Name)
            'Next

            'Return New IndexCriteria(standardFields, userFields, includeNodeTypes, excludeNodeTypes, indexSet.IndexParentId)
        End Function

    End Class



    Public Class ProductDataService
        Implements ISimpleDataService

        Public Function GetAllData(indexType As String) As IEnumerable(Of SimpleDataSet) Implements ISimpleDataService.GetAllData
            Dim ret As New List(Of SimpleDataSet)
            Dim sw As New Diagnostics.Stopwatch

            sw.Start()
            Dim products = Product.All()
            sw.Stop()
            LogHelper.Info(Of ProductDataService)("WSC.Examine GetAllData('" & indexType & ":products - From DB') " & sw.ElapsedMilliseconds & "ms")

            sw.Restart()
            ret.AddRange(GetData(Of Product)(products))
            sw.Stop()
            LogHelper.Info(Of ProductDataService)("WSC.Examine GetAllData('" & indexType & ":products - Add To Index') " & sw.ElapsedMilliseconds & "ms")

            sw.Restart()
            Dim kits = Kit.All()
            ret.AddRange(GetData(Of Kit)(kits))
            sw.Stop()
            LogHelper.Info(Of ProductDataService)("WSC.Examine GetAllData('" & indexType & ":kits') " & sw.ElapsedMilliseconds & "ms")

            'sw.Restart()
            'Dim items = Item.All()
            'ret.AddRange(GetData(Of Item)(items))
            'sw.Stop()
            'LogHelper.Info(Of ProductDataService)("WSC.Examine GetAllData('" & indexType & ":items') " & sw.ElapsedMilliseconds & "ms")

            sw.Restart()
            Dim brands = Brand.All()
            ret.AddRange(GetData(Of Brand)(brands))
            sw.Stop()
            LogHelper.Info(Of ProductDataService)("WSC.Examine GetAllData('" & indexType & ":brands') " & sw.ElapsedMilliseconds & "ms")

            sw.Restart()
            Dim index = 1
            For Each sd In ret
                sd.NodeDefinition.NodeId = index
                index += 1
            Next
            sw.Stop()
            LogHelper.Info(Of ProductDataService)("WSC.Examine GetAllData('" & indexType & ":Update NodeID') " & sw.ElapsedMilliseconds & "ms")

            Return ret
        End Function

        Function GetData(Of T)(items As List(Of T)) As List(Of SimpleDataSet)
            Dim myType As Type = GetType(T)
            Dim properties = umbraco.Core.TypeExtensions.GetAllProperties(myType).Where(Function(x) x.CanRead).ToList
            Dim typeName = myType.ToString.Split("+").Last()

            'LogHelper.Info(Of ProductDataService)("GetData(" & typeName & ") - Properties: " & properties.Count & ", Items: " & items.Count)
            'If typeName = "Product" Then
            '    Dim related = properties.FirstOrDefault(Function(x) x.Name = "Related")
            '    If related IsNot Nothing Then properties.Remove(related)

            'End If
            properties.RemoveAll(Function(x) x.Name = "Related")
            properties.RemoveAll(Function(x) x.Name = "Features")
            properties.RemoveAll(Function(x) x.Name = "Images")
            properties.RemoveAll(Function(x) x.Name = "ImagePath")
            properties.RemoveAll(Function(x) x.Name = "VideoFlag")
            properties.RemoveAll(Function(x) x.Name = "HasVideo")

            'LogHelper.Info(Of ProductDataService)("GetData(" & typeName & ") - Properties: " & properties.Count & "[" & String.Join(",", properties.Select(Function(x) x.Name).ToArray) & "], Items: " & items.Count)

            Dim ret As New List(Of SimpleDataSet)

            Try
                For Each i In items
                    Dim sd As New SimpleDataSet()
                    sd.NodeDefinition = New IndexedNode()
                    sd.NodeDefinition.NodeId = 0 '-- this is overwritten before adding to index.
                    sd.NodeDefinition.Type = typeName
                    sd.RowData = New Dictionary(Of String, String)

                    sd.RowData.Add("type", typeName)

                    For Each p In properties
                        Try
                            sd.RowData.Add(p.Name, p.GetValue(i, Nothing).ToString)
                        Catch ex As Exception
                            LogHelper.Error(Of ProductDataService)("GetData (" & typeName & "." & p.Name & ")", ex)
                        End Try
                    Next

                    If sd.RowData.ContainsKey("Name") Then
                        sd.RowData.Add("Name_Search", sd.RowData.Item("Name").ToLower)
                    End If

                    If sd.RowData.ContainsKey("Keywords") Then
                        sd.RowData.Add("Keywords_Search", sd.RowData.Item("Keywords").ToLower)
                    End If

                    '--Remove Commas from search terms
                    If sd.RowData.ContainsKey("SearchTerms") Then
                        sd.RowData.Item("SearchTerms") = sd.RowData.Item("SearchTerms").Replace(",", " ").ToLower
                    End If

                    If sd.RowData.ContainsKey("AnimalType") Then
                        'sd.RowData.Add("AnimalType_Search", sd.RowData.Item("AnimalType").Replace(",", " ").ToLower)
                        sd.RowData.Item("AnimalType") = sd.RowData.Item("AnimalType").Replace("CATDG", "CAT DOG")
                    End If


                    'LogHelper.Info(Of ProductDataService)("GetData().NiceUrl:" & sd.RowData.FirstOrDefault(Function(x) x.Key = "NiceUrl").Value)
                    ret.Add(sd)
                Next
            Catch ex As Exception
                LogHelper.Error(Of ProductDataService)("GetData", ex)
            End Try

            Return ret
        End Function

        Public Shared Function GetAllSystemPropertyNames() As IEnumerable(Of String)
            Return New List(Of String)
        End Function

        Public Shared Function GetAllUserPropertyNames() As IEnumerable(Of String)
            Dim ret As New List(Of String)

            ret.Add("type")
            ret.AddRange(GetAllProperties(GetType(Product)).Where(Function(x) x.CanRead).Select(Function(x) x.Name).ToArray())
            'ret.AddRange(GetAllProperties(GetType(Item)).Where(Function(x) x.CanRead).Select(Function(x) x.Name).ToArray())
            ret.AddRange(GetAllProperties(GetType(Kit)).Where(Function(x) x.CanRead).Select(Function(x) x.Name).ToArray())
            ret.AddRange(GetAllProperties(GetType(Brand)).Where(Function(x) x.CanRead).Select(Function(x) x.Name).ToArray())

            ret.Add("Name_Search")
            ret.Add("Keywords_Search")
            ret.Add("AnimalType_Search")


            '--Remove
            ret.Remove("Related")
            ret.Remove("Features")
            ret.Remove("Images")
            ret.Remove("ImagePath")
            ret.Remove("VideoFlag")
            ret.Remove("HasVideo")


            Return ret.Distinct()
        End Function

    End Class


End Namespace
