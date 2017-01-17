Imports Microsoft.VisualBasic
Imports System.Data
Imports Microsoft.VisualBasic.FileIO

Namespace WSC.Utilities
    Public Class Cache
        Public Shared Sub [Set](cacheKey As String, o As Object, hours As Integer)
            If o Is Nothing Then Exit Sub
            HttpContext.Current.Cache.Insert(cacheKey, o, Nothing, System.Web.Caching.Cache.NoAbsoluteExpiration, New TimeSpan(hours, 0, 0))
        End Sub

        Public Shared Sub [Set](Of T)(o As Object, hours As Integer)
            If o Is Nothing Then Exit Sub
            Dim CacheKey = GetType(T).FullName
            [Set](CacheKey, o, hours)
        End Sub

        Public Shared Sub [Set](Of T)(o As Object, filePath As String)
            Dim CacheKey = GetType(T).FullName
            HttpContext.Current.Cache.Insert(CacheKey, o, New CacheDependency(umbraco.Core.IO.IOHelper.MapPath(filePath)))
        End Sub

        Public Shared Function [Get](CacheKey As String) As Object
            Return HttpContext.Current.Cache(CacheKey)
        End Function

        Public Shared Function [Get](Of T)() As Object
            Dim CacheKey = GetType(T).FullName
            Return [Get](CacheKey)
        End Function
    End Class
    Public Class CSV
        Public Shared Function GetDataTableFromCSVFile(csvFilePath As String) As DataTable
            Return GetDataTableFromCSVFile(IO.File.OpenRead(csvFilePath))
        End Function

        Public Shared Function GetDataTableFromCSVFile(csvFileStream As IO.Stream) As DataTable
            Dim csvData As New DataTable()
            Try
                Using csvReader As New TextFieldParser(csvFileStream)
                    csvReader.SetDelimiters(New String() {","})
                    csvReader.HasFieldsEnclosedInQuotes = True
                    csvReader.TrimWhiteSpace = True

                    Dim colFields As String() = csvReader.ReadFields()

                    For Each column As String In colFields
                        Dim datecolumn As New DataColumn(column)
                        datecolumn.AllowDBNull = True
                        csvData.Columns.Add(datecolumn)
                    Next

                    While Not csvReader.EndOfData
                        Dim fieldData As String() = csvReader.ReadFields()
                        'Making empty value as null
                        For i As Integer = 0 To fieldData.Length - 1
                            If fieldData(i) = "" Then
                                fieldData(i) = Nothing
                            End If
                        Next

                        csvData.Rows.Add(fieldData)
                    End While
                End Using
            Catch ex As Exception
            End Try

            Return csvData
        End Function
    End Class

    Public Class Geo
        Property Latitude As Decimal
        Property Longitude As Decimal
        Property State As String

        Sub New(ByVal lat As Decimal, ByVal lng As Decimal)
            Me.Latitude = lat
            Me.Longitude = lng
        End Sub

        Shared Function [Get](ByVal address As String) As Geo
            Dim url As String = String.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", HttpUtility.UrlEncode(address))
            Try
                Dim result As XElement = XElement.Load(url)
                If result.Descendants("result") IsNot Nothing Then
                    Dim local As XElement = result.Element("result").Element("geometry").Element("location")
                    If local IsNot Nothing Then
                        Dim ret As New Geo(local.Element("lat").Value, local.Element("lng").Value)
                        Dim state As XElement = result.Descendants("address_component").FirstOrDefault(Function(x) x.Elements("type").First().Value = "administrative_area_level_1")
                        If state IsNot Nothing Then
                            ret.State = state.Element("short_name").Value
                        End If
                        Return ret
                    End If
                End If
            Catch ex As Exception
                umbraco.Core.Logging.LogHelper.Info(Of WSC.Utilities.Geo)(address & ":" & ex.Message)
            End Try

            Return Nothing
        End Function

        '--http://www.geodatasource.com/developers/vb-dot-net
        Public Function Distance(ByVal lat As Double, ByVal lng As Double) As Double
            Dim theta As Double = lng - Me.Longitude
            Dim dist As Double = Math.Sin(deg2rad(lat)) * Math.Sin(deg2rad(Me.Latitude)) + Math.Cos(deg2rad(lat)) * Math.Cos(deg2rad(Me.Latitude)) * Math.Cos(deg2rad(theta))
            dist = Math.Acos(dist)
            dist = rad2deg(dist)
            dist = dist * 60 * 1.1515
            Return dist
        End Function

        Private Function deg2rad(ByVal deg As Double) As Double
            Return (deg * Math.PI / 180.0)
        End Function

        Private Function rad2deg(ByVal rad As Double) As Double
            Return rad / Math.PI * 180.0
        End Function
    End Class
End Namespace
