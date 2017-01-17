Imports Microsoft.VisualBasic
'Imports umbraco.Core.Persistence
'Imports umbraco.Core.Persistence.DatabaseAnnotations
Imports WSC.PetaPoco
Imports WSC.Extensions
Imports umbraco.NodeExtensions


Namespace WSC
    Public Class Datalayer
#Region "Shared"

        Private Shared _DB As Database
        Private Shared ReadOnly Property DB As Database
            Get
                If HttpContext.Current Is Nothing Then
                    _DB = New Database("AS400")
                    '_DB.KeepConnectionAlive = True
                    umbraco.Core.Logging.LogHelper.Info(Of Datalayer)("Create New CPP.Database connection (No HttpContext")
                    Return _DB
                End If

                _DB = DirectCast(HttpContext.Current.Application("CPP.Database"), Database)
                If _DB Is Nothing Then
                    _DB = New Database("AS400")
                    _DB.KeepConnectionAlive = True
                    HttpContext.Current.Application.Lock()
                    HttpContext.Current.Application.Add("CPP.Database", _DB)
                    HttpContext.Current.Application.UnLock()
                    umbraco.Core.Logging.LogHelper.Info(Of Datalayer)("Create New CPP.Database connection")
                End If

                If _DB Is Nothing Then
                    _DB = New Database("AS400")
                    _DB.KeepConnectionAlive = True
                End If
                Return _DB

            End Get
        End Property

        Public Shared ReadOnly Property LastSQL As String
            Get
                Return DB.LastSQL
            End Get
        End Property

        Public Shared ReadOnly Property LastCommand As String
            Get
                Return DB.LastCommand
            End Get
        End Property

        Public Shared Function TrimSQLFields(Of T)(ignore As IEnumerable(Of String)) As String
            If ignore Is Nothing Then ignore = New List(Of String)
            Dim tmp = GetType(T)
            Dim sql As New List(Of String)
            For Each pi In tmp.GetProperties().Where(Function(x) x.CanWrite)
                Dim c = pi.GetCustomAttributes(GetType(ColumnAttribute), True)
                If c.Length > 0 Then
                    Dim colattr = CType(c(0), ColumnAttribute)
                    If Not String.IsNullOrEmpty(colattr.Name) AndAlso Not ignore.Contains(colattr.Name) Then
                        sql.Add(String.Format("TRIM({0}) AS {0}", colattr.Name))
                    End If
                End If
            Next

            Return String.Join(",", sql.ToArray)
        End Function

        Private Shared Function FormatUrl(ByVal url As String) As String
            Return umbraco.cms.helpers.url.FormatUrl(url.ToLower())
        End Function

        Private Shared Function CleanImagePath(path As String) As String
            If String.IsNullOrEmpty(path) Then Return String.Empty
            path = Regex.Replace(path, "h:[/\\]{1}website", "/media/website", RegexOptions.IgnoreCase).Replace("\", "/")
            'Return HttpContext.Current.Server.UrlPathEncode(path)
            Return HttpUtility.UrlPathEncode(path)
        End Function
#End Region

        Class UPC
            Property Value As String = String.Empty
            Property Width As Integer = 100
            Property Height As Integer = 50

            Sub New(value As String, width As Integer, height As Integer)
                Me.Value = value
                Me.Width = width
                Me.Height = height
            End Sub

            Function Render() As Byte()
                If String.IsNullOrEmpty(Me.Value) Then Return Nothing

                Dim b As New BarcodeLib.Barcode()
                b.IncludeLabel = True
                Dim bi = b.Encode(BarcodeLib.TYPE.UPCA, Me.Value, Me.Width, Me.Height)
                Dim ms As New IO.MemoryStream()
                bi.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg)
                Return ms.ToArray
            End Function

            Public Overrides Function ToString() As String
                'Return MyBase.ToString()
                Dim bytes = Me.Render()
                Dim base64String As String = Convert.ToBase64String(bytes, 0, bytes.Length)
                Return "data:image/jpeg;base64," & base64String
            End Function
        End Class


        <umbraco.Core.Persistence.TableName("tblCart")>
        <umbraco.Core.Persistence.PrimaryKey("ID", autoIncrement:=True)>
        Class Cart
            Const SessionKey As String = "CPP.Cart"

            Public Property ID As Integer = 0
            Public Property AccountNumber As String
            Public Property Note As String = String.Empty

            <umbraco.Core.Persistence.ResultColumn()>
            Public Property Items As New List(Of Item)

            <umbraco.Core.Persistence.Ignore()>
            Public ReadOnly Property Count As Integer
                Get
                    Return Me.Items.Count
                End Get
            End Property

            <umbraco.Core.Persistence.Ignore()>
            Public ReadOnly Property SubTotal As Decimal
                Get
                    Return Me.Items.Sum(Function(x) x.Price * x.Quantity)
                End Get
            End Property

            Private _OrderNumber As String = String.Empty
            <umbraco.Core.Persistence.Ignore()>
            Public ReadOnly Property OrderNumber() As String
                Get
                    Return _OrderNumber
                End Get
            End Property

            Sub New()
            End Sub
            Public Shared Function GetUserAccountNumber() As String
                Dim c = Customer.GetCurrent()
                If c Is Nothing Then Return Nothing
                Dim m = Member.GetCurrent()
                If m Is Nothing Then Return Nothing


                '--Logic for DS customer
                'If XXXXXXX = YYYY Then
                'Return m.AccountNumber
                ' Return m.AccountNumber & m.XXXXXXX
                '  End If


                Return c.AccountNumber
            End Function

            Public Sub Persist()
                '--Save to the DB?
                Dim userAccountNumber = GetUserAccountNumber()
                If Not String.IsNullOrEmpty(userAccountNumber) Then
                    If String.IsNullOrEmpty(Me.AccountNumber) Then
                        Me.AccountNumber = userAccountNumber
                    End If

                    '  Dim c = Customer.GetCurrent()
                    '     If c IsNot Nothing Then
                    '    If String.IsNullOrEmpty(Me.AccountNumber) Then
                    '    Me.AccountNumber = c.AccountNumber
                    'End If

                    umbraco.Core.ApplicationContext.Current.DatabaseContext.Database.Save(Me)
                    For Each i In Me.Items
                        i.CartID = Me.ID
                        umbraco.Core.ApplicationContext.Current.DatabaseContext.Database.Save(i)
                    Next
                End If
            End Sub

            'If ret Is Nothing Then
            ' Dim userAccountNumber = GetUserAccountNumber()
            ' If Not String.IsNullOrEmpty(userAccountNumber) Then
            ' Dim strSQL = umbraco.Core.Persistence.Sql.Builder.Append("SELECT * FROM tblCart LEFT JOIN tblCartItem ON (tblCartItem.CartID=tblCart.ID) WHERE AccountNumber=@0 ORDER BY tblCartItem.ID", userAccountNumber)
            '  Dim temp = umbraco.Core.ApplicationContext.Current.DatabaseContext.Database.Fetch(Of Cart, Item, Cart)(AddressOf New CartItemRelator().MapIt, strSQL).FirstOrDefault()
            ' If temp IsNot Nothing Then
            '                 ret = temp
            ' End If
            ' End If
            ' End If

            ' If ret Is Nothing Then
            '        ret = New Cart()
            '       ret.Items = New List(Of Item)
            'End If

            'Return ret
            'End Function

            Public Sub ClearItems()
                If Not String.IsNullOrEmpty(Me.AccountNumber) Then
                    For Each i In Me.Items
                        umbraco.Core.ApplicationContext.Current.DatabaseContext.Database.Delete(i)
                    Next
                    Me.Items.Clear()
                End If
            End Sub

            Public Sub ClearDB()
                If Not String.IsNullOrEmpty(Me.AccountNumber) Then
                    ClearItems()
                    Me.Note = String.Empty
                    umbraco.Core.ApplicationContext.Current.DatabaseContext.Database.Save(Me)
                End If
            End Sub

            Friend Shared Sub Empty()
                Throw New NotImplementedException()
            End Sub

            Function Submit(cust As Customer) As Boolean
                If Me.Items.Count = 0 Then Return False

                Dim wo As New WebOrder(Me, cust)
                '--Save to the AS400
                If wo.Save() Then
                    Me._OrderNumber = wo.OrderNumber
                    Return True
                End If
                Return False
            End Function

            Public Shared Function GetCurrent() As Cart
                Dim ret As Cart = Nothing
                '--Caching the cart causes issues with multiple users
                'If HttpContext.Current.Session(SessionKey) IsNot Nothing Then
                '    ret = CType(HttpContext.Current.Session(SessionKey), Cart)
                'End If

                If ret Is Nothing Then
                    Dim c = Customer.GetCurrent()
                    If c IsNot Nothing AndAlso Not String.IsNullOrEmpty(c.AccountNumber) Then
                        'Dim temp = umbraco.Core.ApplicationContext.Current.DatabaseContext.Database.FirstOrDefault(Of Cart)("WHERE AccountNumber=@0", c.AccountNumber)
                        Dim strSQL = umbraco.Core.Persistence.Sql.Builder.Append("SELECT * FROM tblCart LEFT JOIN tblCartItem ON (tblCartItem.CartID=tblCart.ID) WHERE AccountNumber=@0 ORDER BY tblCartItem.ID", c.AccountNumber)
                        Dim temp = umbraco.Core.ApplicationContext.Current.DatabaseContext.Database.Fetch(Of Cart, Item, Cart)(AddressOf New CartItemRelator().MapIt, strSQL).FirstOrDefault()
                        If temp IsNot Nothing Then
                            ret = temp
                        End If
                    End If
                End If

                If ret Is Nothing Then
                    ret = New Cart()
                    ret.Items = New List(Of Item)
                End If

                ''--Remove Duplicates that might have been introduced
                'Dim dups = ret.Items.GroupBy(Function(x) x.ItemID).Select(Function(x) x.Count()).Where(Function(x) x > 1)
                'If dups.Count > 0 Then
                '    Dim temp As New List(Of Item)
                '    For Each i In ret.Items
                '        If temp.Exists(Function(x) x.ItemID = i.ItemID) Then
                '            temp.FirstOrDefault(Function(x) x.ItemID = i.ItemID).Quantity += i.Quantity
                '            umbraco.Core.ApplicationContext.Current.DatabaseContext.Database.Delete(i)
                '        Else
                '            temp.Add(i)
                '        End If
                '    Next
                '    ret.Items.Clear()
                '    ret.Items.AddRange(temp)
                '    ret.Persist()
                'End If

                Return ret
            End Function

            Public Shared Function AddItem(item As Cart.Item) As Cart
                Return AddItem(item, True)
            End Function

            Public Shared Function AddItem(item As Cart.Item, update As Boolean) As Cart
                Dim current = GetCurrent()
                '--Check for existing product and increment Quntity rather than add again
                Dim i = current.Items.FirstOrDefault(Function(x) x.ItemID = item.ItemID)
                If i IsNot Nothing AndAlso update Then
                    i.Quantity += item.Quantity
                Else
                    current.Items.Add(item)
                End If
                current.Persist()
                Return current
            End Function

            Public Shared Function AddItem(i As Datalayer.Item, price As Decimal, quantity As Integer) As Cart
                Return AddItem(i, price, quantity, String.Empty)
            End Function

            Public Shared Function AddItem(i As Datalayer.Item, price As Decimal, quantity As Integer, note As String) As Cart
                Return AddItem(i, price, quantity, note, True)
            End Function

            Public Shared Function AddItem(i As Datalayer.Item, price As Decimal, quantity As Integer, note As String, update As Boolean) As Cart
                Dim cartI As New Item(i, price, quantity, note)
                Return AddItem(cartI, update)
            End Function

            Public Shared Function RemoveItem(ID As String) As Cart
                Dim current = GetCurrent()
                Dim i = current.Items.FirstOrDefault(Function(x) x.ItemID = ID)
                If i IsNot Nothing Then
                    umbraco.Core.ApplicationContext.Current.DatabaseContext.Database.Delete(i)
                    current.Items.Remove(i)
                    current.Persist()
                End If

                Return current
            End Function

            Public Shared Sub Clear()
                Clear(False)
            End Sub

            Public Shared Sub Clear(db As Boolean)
                HttpContext.Current.Session.Remove(SessionKey)

                If db Then
                    GetCurrent().ClearDB()
                End If
            End Sub



            Private Class CartItemRelator
                Public Current As Cart
                Public Function MapIt(c As Cart, ci As Item) As Cart
                    If c Is Nothing Then Return Current
                    If Current IsNot Nothing AndAlso Current.ID = ci.CartID Then
                        Current.Items.Add(ci)
                        Return Nothing
                    End If

                    Dim prev = Current
                    Current = c
                    Current.Items = New List(Of Item)
                    If Not String.IsNullOrEmpty(ci.Sku) Then
                        Current.Items.Add(ci)
                    End If

                    Return prev
                End Function
            End Class


            <umbraco.Core.Persistence.TableName("tblCartItem")>
            <umbraco.Core.Persistence.PrimaryKey("ID", autoIncrement:=True)>
            Class Item
                Public Property ID As Integer = 0
                Public Property CartID As Integer = 0
                Public Property Sku As String
                Public Property BarCode As String
                Public Property Name As String
                Public Property Image As String
                Public Property Price As Decimal
                Public Property Quantity As Integer
                Public Property Note As String = String.Empty

                <umbraco.Core.Persistence.Ignore()>
                ReadOnly Property Total As Decimal
                    Get
                        Return Me.Price * Me.Quantity
                    End Get
                End Property

                <umbraco.Core.Persistence.Ignore()>
                ReadOnly Property ItemID As String
                    Get
                        Return Me.Sku.Replace(" ", String.Empty)
                    End Get
                End Property

                Sub New()
                End Sub

                Sub New(i As Datalayer.Item, price As Decimal, quantity As Integer, note As String)
                    Me.Sku = i.Sku
                    'Me.Name = i.Name
                    Me.Name = If(Not String.IsNullOrEmpty(i.DisplayName), i.DisplayName, i.Name)
                    Me.BarCode = i.UPCCode
                    Me.Image = i.Image
                    Me.Price = price
                    Me.Quantity = quantity
                    Me.Note = note
                End Sub
            End Class
        End Class

        <TableName("WEBORDHDR")>
        Class WebOrder
            <Column("CONOWC")> Property CompanyCode As String = "CP"
            <Column("UNIQWC")> Property OrderNumber As String
            <Column("IDNOWC")> Property ID As Integer = 0
            <Column("NAMEWC", Trim:=True)> Property Name As String '--Customer.Name
            <Column("ADDR1WC", Trim:=True)> Property Address1 As String
            <Column("ADDR2WC", Trim:=True)> Property Address2 As String
            <Column("ADDR3WC", Trim:=True)> Property Address3 As String
            <Column("CITYWC", Trim:=True)> Property City As String
            <Column("STATEWC", Trim:=True)> Property State As String
            <Column("ZIPCDWC", Trim:=True)> Property Zip As String
            <Column("CUSNWC", Trim:=True)> Property AccountNumber As String
            <Column("DTORDWC")> Property OrderDate As Decimal = 0 '--CYYMMDD
            <Column("SHPDTWC")> Property ShipDate As Decimal = 0 '--CYYMMDD
            <Column("CUSOWC")> Property CustomerReference As String
            <Column("SLMNWC")> Property Salesman As String
            <Column("RULEWC")> Property UPCRule As String '--From Customer
            <Column("LISTWC")> Property PriceCode As String '--From Customer
            <Column("VALUEWC")> Property Total As Decimal = 0
            <Column("STATWC")> Property StatusCode As String = String.Empty
            <ResultColumn()> Property StatusDescription As String = String.Empty

            <ResultColumn()> Property OrderLines As New List(Of WebOrderDetail)
            '<ResultColumn()> Property Note As WebOrderNote

            Private _OrderNote As WebOrderNote
            <ResultColumn()> Property OrderNote As WebOrderNote
                Get
                    If _OrderNote Is Nothing Then
                        _OrderNote = WebOrderNote.GetForOrder(Me.OrderNumber, 0)
                    End If
                    Return _OrderNote
                End Get
                Set(value As WebOrderNote)
                    _OrderNote = value
                End Set
            End Property

            Sub New()
            End Sub

            Sub New(cust As Customer)
                Me.Name = cust.Name
                Me.Address1 = cust.Address1
                Me.Address2 = cust.Address2
                Me.Address3 = cust.Address3
                Me.City = cust.City
                Me.State = cust.State
                Me.Zip = cust.Zip
                Me.AccountNumber = cust.AccountNumber
                Me.Salesman = cust.Salesman
                Me.UPCRule = cust.UPCRule
                Me.PriceCode = cust.PriceCode
            End Sub

            Sub New(c As Cart, cust As Customer)
                Me.Name = cust.Name
                Me.Address1 = cust.Address1
                Me.Address2 = cust.Address2
                Me.Address3 = cust.Address3
                Me.City = cust.City
                Me.State = cust.State
                Me.Zip = cust.Zip
                Me.AccountNumber = cust.AccountNumber
                Me.Salesman = cust.Salesman
                Me.UPCRule = cust.UPCRule
                Me.PriceCode = cust.PriceCode
                Me.Total = c.SubTotal

                If Not String.IsNullOrEmpty(c.Note) Then
                    Me.OrderNote = New WebOrderNote() With {.Text = c.Note}
                End If

                For Each i In c.Items
                    Dim ol As New WebOrderDetail(i)
                    Me.OrderLines.Add(ol)
                Next
            End Sub


            Function Save() As Boolean
                If Not String.IsNullOrEmpty(Me.OrderNumber) Then Return False

                Me.ID = Member.GetCurrent().ID
                Me.OrderNumber = NextNumber()
                Me.CustomerReference = Me.OrderNumber
                Me.OrderDate = Today.To400Date
                Me.ShipDate = Today.To400Date

                '--Save the header
                DB.Insert(Me)

                '--Save the details
                Dim index As Integer = 0
                For Each ol In Me.OrderLines
                    index += 1
                    ol.LineNumber = index
                    ol.OrderNumber = Me.OrderNumber
                    ol.PriceCode = Me.PriceCode
                    ol.Save()
                Next

                '--Save note if available
                If Me.OrderNote IsNot Nothing AndAlso Not String.IsNullOrEmpty(Me.OrderNote.Text) Then
                    Me.OrderNote.OrderNumber = Me.OrderNumber
                    Me.OrderNote.LineNumber = 0
                    Me.OrderNote.Save()
                End If

                Return True
            End Function

            'Function NextID() As Integer
            '    Return DB.ExecuteScalar(Of Integer)("SELECT IDNOWC + 1 FROM WEBORDHDR ORDER BY IDNOWC DESC FETCH FIRST 1 ROWS ONLY")
            'End Function

            Function NextNumber() As String
                Dim n = DB.ExecuteScalar(Of Integer)("SELECT CAST(SUBSTR(UNIQWC, 3) AS INT) + 1 FROM WEBORDHDR ORDER BY UNIQWC DESC FETCH FIRST 1 ROWS ONLY")
                Return String.Format("WB{0:00000000}", n)
            End Function

            Private Shared Function BaseSQL() As Sql
                Dim pdWO = WSC.PetaPoco.Database.PocoData.ForType(New WSC.Datalayer.WebOrder().GetType)
                Dim pdWD = WSC.PetaPoco.Database.PocoData.ForType(New WSC.Datalayer.WebOrderDetail().GetType)
                Return Sql.Builder.Append("SELECT " & DB.QueryColumnSQL(pdWO)).
                                         Append(", DESC.PRMD15 AS StatusDescription").
                                         Append("," & DB.QueryColumnSQL(pdWD)).
                                         Append("FROM WEBORDHDR").
                                         Append("LEFT JOIN WEBORDDTL ON (WEBORDDTL.UNIQWD = WEBORDHDR.UNIQWC)").
                                         Append("LEFT JOIN DESC ON (DESC.CONO15=WEBORDHDR.CONOWC AND DESC.PSAR15=WEBORDHDR.STATWC AND DESC.PRMT15='$WBP')")
            End Function

            Public Shared Function [Get](orderNumber As String) As WebOrder
                'Dim pdWO = WSC.PetaPoco.Database.PocoData.ForType(New WSC.Datalayer.WebOrder().GetType)
                'Dim pdWD = WSC.PetaPoco.Database.PocoData.ForType(New WSC.Datalayer.WebOrderDetail().GetType)
                Dim strSQL = BaseSQL.Append("WHERE UNIQWC=@0", orderNumber).
                                     Append("ORDER BY WEBORDHDR.UNIQWC")

                Return (DB.Fetch(Of WebOrder, WebOrderDetail, WebOrder)(AddressOf New OrderDetailRelator().MapIt, strSQL)).FirstOrDefault()
                'Return DB.Fetch(Of WebOrder)(strSQL)
            End Function

            Public Shared Function GetForCustomer(c As Customer) As List(Of WebOrder)
                'Dim pdWO = WSC.PetaPoco.Database.PocoData.ForType(New WSC.Datalayer.WebOrder().GetType)
                'Dim pdWD = WSC.PetaPoco.Database.PocoData.ForType(New WSC.Datalayer.WebOrderDetail().GetType)
                Dim strSQL = BaseSQL.Append("WHERE CUSNWC=@0", c.AccountNumber).
                                     Append("ORDER BY WEBORDHDR.UNIQWC")

                Return (DB.Fetch(Of WebOrder, WebOrderDetail, WebOrder)(AddressOf New OrderDetailRelator().MapIt, strSQL))
                'Return DB.Fetch(Of WebOrder)(strSQL)
            End Function

            Public Shared Function GetForMember(m As Member) As List(Of WebOrder)
                'Dim pdWO = WSC.PetaPoco.Database.PocoData.ForType(New WSC.Datalayer.WebOrder().GetType)
                'Dim pdWD = WSC.PetaPoco.Database.PocoData.ForType(New WSC.Datalayer.WebOrderDetail().GetType)
                Dim strSQL = BaseSQL.Append("WHERE IDNOWC=@0", m.ID).
                                     Append("ORDER BY WEBORDHDR.UNIQWC")

                Return (DB.Fetch(Of WebOrder, WebOrderDetail, WebOrder)(AddressOf New OrderDetailRelator().MapIt, strSQL))
                'Return DB.Fetch(Of WebOrder)(strSQL)
            End Function

            Public Shared Function ReOrder(orderNumber As String, c As Cart, m As Member) As Boolean
                Dim wo = [Get](orderNumber)
                If wo Is Nothing Then Return False

                For Each ol In wo.OrderLines
                    'Dim i = Item.GetBySku(ol.ItemNumber)
                    'Dim i = Item.GetByUPC(ol.BarCode)
                    Dim i = Item.GetByUPC(ol.BarCode, m.Customer)
                    If i IsNot Nothing Then
                        Dim p = m.Customer.GetPriceForItem(i.Sku)
                        If p > 0 Then
                            WSC.Datalayer.Cart.AddItem(i, p, ol.Quantity)
                        End If
                    End If
                Next

                If wo.OrderNote IsNot Nothing AndAlso Not String.IsNullOrEmpty(wo.OrderNote.Text) Then
                    c.Note &= wo.OrderNote.Text
                    c.Persist()
                End If

                Return True
            End Function


            Private Class OrderDetailRelator
                Public Current As WebOrder
                Public Function MapIt(wo As WebOrder, wod As WebOrderDetail) As WebOrder
                    If wo Is Nothing Then Return Current
                    If Current IsNot Nothing AndAlso Current.OrderNumber = wo.OrderNumber Then
                        Current.OrderLines.Add(wod)
                        Return Nothing
                    End If

                    Dim prev = Current
                    Current = wo
                    Current.OrderLines = New List(Of WebOrderDetail)
                    Current.OrderLines.Add(wod)

                    Return prev
                End Function
            End Class
        End Class

        <TableName("WEBORDDTL", journaled:=False)>
        Class WebOrderDetail
            <Column("CONOWD")> Property CompanyCode As String = "CP"
            <Column("UNIQWD")> Property OrderNumber As String
            <Column("LINEWD")> Property LineNumber As Decimal = 0
            <Column("ITEMWD", Trim:=True)> Property ItemNumber As String
            <Column("BARCWD", Trim:=True)> Property BarCode As String
            <Column("ALTIWD", Trim:=True)> Property AlternateNumber As String = String.Empty
            <Column("ITMDWD", Trim:=True)> Property Description As String
            <Column("QTYWD")> Property Quantity As Integer = 1
            <Column("UPRCWD")> Property Price As Decimal
            <Column("LISTWD")> Property PriceCode As String '--From Customer
            <Column("DTDRWD")> Property ShipDate As Decimal = 0 '--400 date
            <Column("DCD1WD")> Property DiscountCode1 As String = String.Empty
            <Column("DSC1WD")> Property Discount1 As Decimal = 0
            <Column("DCD2WD")> Property DiscountCode2 As String = String.Empty
            <Column("DSC2WD")> Property Discount2 As Decimal = 0

            <ResultColumn()>
            Property Note As WebOrderNote

            Sub New()
            End Sub

            Sub New(i As Cart.Item)
                Me.ItemNumber = i.Sku
                Me.BarCode = i.BarCode
                Me.Description = i.Name
                Me.Quantity = i.Quantity
                Me.Price = i.Price
                If Not String.IsNullOrEmpty(i.Note) Then
                    Me.Note = New WebOrderNote(i.Note)
                End If
            End Sub

            Function Exists() As Boolean
                Return (DB.ExecuteScalar(Of Integer)("SELECT COUNT(*) FROM WEBORDDTL WHERE UNIQWD=@OrderNumber AND LINEWD=@LineNumber", Me) > 0)
            End Function

            Sub Save()
                If Me.Exists Then
                    '--Should we over write?
                Else
                    DB.Insert(Me)
                End If

                If Me.Note IsNot Nothing AndAlso Not String.IsNullOrEmpty(Me.Note.Text) Then
                    Me.Note.OrderNumber = Me.OrderNumber
                    Me.Note.LineNumber = Me.LineNumber
                    Me.Note.Save()
                End If

            End Sub
        End Class

        <TableName("WEBORDNTE", journaled:=False)>
        Class WebOrderNote
            <Column("CONOWN")> Property CompanyCode As String = "CP"
            <Column("UNIQWN")> Property OrderNumber As String
            <Column("LINEWN")> Property LineNumber As Decimal = 0
            <Column("SEQWN")> Property SequenceNumber As Decimal = 0
            <Column("TEXTWN")> Property Text As String

            Sub New()
            End Sub

            Sub New(txt As String)
                Me.Text = txt
            End Sub

            Sub Save()
                If Me.Text.Length > 70 Then
                    Dim chunks = Me.Text.Chunk(70)
                    Dim seq = 0
                    For Each c In chunks
                        Dim won As New WebOrderNote() With {.OrderNumber = Me.OrderNumber, .LineNumber = Me.LineNumber, .SequenceNumber = seq}
                        won.Text = c
                        won.Save()
                        seq += 1
                    Next
                Else
                    If Me.Exists() Then
                        DB.Execute("UPDATE WEBORDNTE SET TEXTWN=@Text WHERE WHERE UNIQWN=@OrderNumber AND LINEWN=@LineNumber AND SEQWN=@SequenceNumber WITH NONE", Me)
                    Else
                        DB.Insert(Me)
                    End If
                End If
            End Sub

            Function Exists() As Boolean
                Return (DB.ExecuteScalar(Of Integer)("SELECT COUNT(*) FROM WEBORDNTE WHERE UNIQWN=@OrderNumber AND LINEWN=@LineNumber AND SEQWN=@SequenceNumber", Me) > 0)
            End Function


            Public Shared Function GetForOrder(orderNumber As String, orderLine As Integer) As WebOrderNote
                Dim tmp = DB.Fetch(Of WebOrderNote)("WHERE UNIQWN=@0 AND LINEWN=@1 ORDER BY SEQWN", orderNumber, orderLine)
                Dim ret = tmp.FirstOrDefault()
                For Each t In tmp.Skip(1)
                    ret.Text &= t.Text
                Next
                Return ret
            End Function
        End Class

        <TableName("COASTDESC")>
        Class Brand
            <Column()> Public Property ID As String
            <Column()> Public Property Name As String
            <Column()> Public Property ImagePath As String
            <Column()> Public Property BrandOrder As Decimal = 999

            Public ReadOnly Property Image As String
                Get
                    Return CleanImagePath(Me.ImagePath)
                End Get
            End Property

            Public ReadOnly Property NiceUrl As String
                Get
                    Return String.Format("/products/search.aspx?BrandID={0}", Me.ID.ToLower)
                End Get
            End Property

            Public Shared Function All() As List(Of Brand)
                Return AllPaged(0, 0)
            End Function

            Public Shared Function AllPaged(page As Integer, pageSize As Integer) As List(Of Brand)
                Dim strSQL = Sql.Builder.Append("SELECT TRIM(C.PSARC5) AS ID, TRIM(C.CHR4C5) AS Name, TRIM(W.PATHBI) AS ImagePath, NUM1C5 AS BrandOrder").
                                         Append("FROM COASTDESC AS C,").
                                         Append("WEBBRNIMG AS W").
                                         Append("WHERE C.PRMTC5 = 'WEBMKTBRAND'").
                                         Append("AND C.CONOC5 = W.CONOBI").
                                         Append("AND C.PSARC5 = W.BRNDBI")

                If page > 0 AndAlso pageSize > 0 Then
                    page = (page - 1) * pageSize
                    'strSQL.Append("LIMIT " & pageSize & " OFFSET " & page)
                    Return DB.Fetch(Of Brand)(strSQL).Skip(page).Take(pageSize).ToList
                End If

                Return DB.Fetch(Of Brand)(strSQL)
            End Function

            Public Shared Function GetFeatured() As List(Of Brand)
                Dim strSQL = Sql.Builder.Append("SELECT W.SQNOIT, TRIM(W.BRNDIT) AS ID, TRIM(W.BRNNIT) AS Name, TRIM(B.PATHBI) AS ImagePath").
                                         Append("FROM WEBFTRITM AS W").
                                         Append("LEFT JOIN WEBBRNIMG AS B ON (W.CONOIT=B.CONOBI AND W.BRNDIT=B.BRNDBI AND B.ITYPBI='I')").
                                         Append("WHERE BRNDIT <> ''").
                                         Append("ORDER BY W.SQNOIT")
                Return DB.Fetch(Of Brand)(strSQL)
            End Function

        End Class

        <TableName("WEBOEL23")>
        Class Contact
            <Column("EMAL23")> Property Email As String

            Public Shared Function GetForDistributor(preferredDistributor As String) As List(Of Contact)
                Return DB.Fetch(Of Contact)("WHERE ACT223=@0", preferredDistributor)
            End Function

        End Class

        <TableName("CUSTOMERS")>
        Class Customer
            '<Column("ACCNTW1")> Public Property AccountCode As String '--?
            '<Column("ACDSEQW1")> Public Property DeliverySeq As String '--?

            '<Column("RSTS05")> Public Property NotSure As String
            <Column("CUSN05", Trim:=True)> Public Property AccountNumber As String
            '<Column("DSEQ05")> Public Property NotSure As String
            <Column("CONO05")> Public Property CompanyCode As String = "CP"
            <Column("CNAM05", Trim:=True)> Public Property Name As String
            <Column("CAD105", Trim:=True)> Public Property Address1 As String
            <Column("CAD205", Trim:=True)> Public Property Address2 As String
            <Column("CAD305", Trim:=True)> Public Property Address3 As String
            <Column("CAD405", Trim:=True)> Public Property City As String
            <Column("CAD505", Trim:=True)> Public Property State As String
            <Column("POST10", Trim:=True)> Public Property Zip As String
            '<Column("PCD205")> Public Property NotSure As String
            '<Column("POST10")> Public Property NotSure As String
            '<Column("STAD05")> Public Property NotSure As String
            '<Column("PTRM05")> Public Property NotSure As String
            '<Column("CGP105")> Public Property NotSure As String
            '<Column("CGP205")> Public Property NotSure As String
            '<Column("CGP305")> Public Property NotSure As String
            <Column("CGP405", Trim:=True)> Public Property UPCRule As String
            <Column("SLMN05", Trim:=True)> Public Property Salesman As String
            '<Column("REGN05")> Public Property NotSure As String
            '<Column("CRLM05")> Public Property NotSure As String
            '<Column("CRLC05")> Public Property NotSure As String
            '<Column("CSTP05")> Public Property NotSure As String
            '<Column("CNTN05")> Public Property NotSure As String
            '<Column("INCD05")> Public Property NotSure As String
            '<Column("PHON05")> Public Property NotSure As String
            '<Column("SPIN05")> Public Property NotSure As String
            '<Column("ACDT05")> Public Property NotSure As String
            '<Column("CRSP05")> Public Property NotSure As String
            '<Column("CURN05")> Public Property NotSure As String
            '<Column("FAXN05")> Public Property NotSure As String
            '<Column("COCD05")> Public Property NotSure As String
            '<Column("CLSC05")> Public Property NotSure As String
            '<Column("PHN205")> Public Property NotSure As String
            '<Column("CNT205")> Public Property NotSure As String
            '<Column("FAX205")> Public Property NotSure As String
            <Column("LIST20", Trim:=True)> Public Property PriceCode As String
            '<Column("DGRP20")> Public Property NotSure As String
            '<Column("DISC20")> Public Property NotSure As String
            '<Column("LOCD20")> Public Property NotSure As String
            '<Column("PRTY20")> Public Property NotSure As String
            '<Column("SOSG20")> Public Property NotSure As String
            '<Column("DIVN20")> Public Property NotSure As String
            '<Column("SDIV20")> Public Property NotSure As String
            '<Column("REGN20")> Public Property NotSure As String
            '<Column("STTE20")> Public Property NotSure As String
            <Column("TRTY20", Trim:=True)> Public Property RepCode As String
            '<Column("SLMN20")> Public Property NotSure As String
            '<Column("CCLS20")> Public Property NotSure As String
            <Column("CORP20", Trim:=True)> Public Property CompanyAccountNumber As String
            '<Column("BRCH20")> Public Property NotSure As String
            '<Column("TASK20")> Public Property NotSure As String
            '<Column("MINV20")> Public Property NotSure As String
            '<Column("MNOC20")> Public Property NotSure As String
            '<Column("STON20")> Public Property NotSure As String
            '<Column("SDIV5A")> Public Property NotSure As String
            '<Column("SCH25A")> Public Property NotSure As String
            '<Column("MAIL5A")> Public Property NotSure As String
            '<Column("USR15A")> Public Property NotSure As String
            '<Column("USR25A")> Public Property NotSure As String

        '=======================================================
        '
        'Pull in Exclusions list for logged in customers Aug 2016 Per Sean WS
        '
        '
        '=======================================================

            Private _Exclusions As List(Of String)
            <Ignore()>
            Public ReadOnly Property Exclusions As List(Of String)
                Get
                    If _Exclusions Is Nothing Then
                        If Not String.IsNullOrEmpty(Me.AccountNumber) Then
                            _Exclusions = DB.Fetch(Of String)("SELECT PNUMX5 FROM EXCLUSN WHERE CONOX5='CP' AND CUSNX5=@0", Me.AccountNumber)
                        Else
                            Return New List(Of String)
                        End If
                    End If
                    Return _Exclusions
                End Get
            End Property

            Private _ExclusionsStyleGroup As List(Of String)
            <Ignore()>
            Public ReadOnly Property ExclusionsStyleGroup As List(Of String)
                Get
                    If _ExclusionsStyleGroup Is Nothing Then
                        If Not String.IsNullOrEmpty(Me.AccountNumber) Then
                            _ExclusionsStyleGroup = DB.Fetch(Of String)("SELECT STYGEX FROM WEBSTGEXC WHERE CONOEX='CP' AND CUSNEX=@0", Me.AccountNumber)
                        Else
                            Return New List(Of String)
                        End If
                    End If
                    Return _ExclusionsStyleGroup
                End Get
            End Property

            ' Sub GetExlusions()
            'If String.IsNullOrEmpty(Me.AccountNumber) Then Exit Sub
            'Dim strSQL = Sql.Builder.Append("SELECT PNUMX5 FROM EXCLUSN WHERE CONOX5=@0  AND ITEMS.RUL31C=@UPCRule", Me.AccountNumber)
            '   _Exclusions = DB.Fetch(Of String)(strSQL)
            'End Sub




            Private _PriceList As List(Of Price)
            <Ignore()>
            Public ReadOnly Property PriceList As List(Of Price)
                Get
                    If _PriceList Is Nothing Then
                        _PriceList = Price.GetForCustomer(Me)
                    End If
                    Return _PriceList
                End Get
            End Property

            Private _IsDefault As Boolean = False
            <Ignore()>
            ReadOnly Property IsDefault() As Boolean
                Get
                    Return _IsDefault
                End Get
            End Property


            Sub New()
            End Sub

            Sub New(isDeafult As Boolean)
                Me._IsDefault = isDeafult
            End Sub


            Function GetPriceForProduct(productID As String) As String
                If Me.PriceList Is Nothing Or Me.PriceList.Count = 0 Then Return String.Empty

                Dim prices = Me.PriceList.Where(Function(x) x.StyleGroup = productID).ToList
                If prices.Count = 0 Then Return String.Empty

                Dim p As New Price()
                p.Min = prices.Min(Function(x) x.Price)
                p.Max = prices.Max(Function(x) x.Price)
                Return p.ToString

                'If min <> max Then
                '    Return String.Format("{0:C} - {1:C}", min, max)
                'Else
                '    Return String.Format("{0:C}", min)
                'End If
            End Function

            Function GetPriceForItem(itemSku As String) As Decimal
                itemSku = itemSku.Trim()
                Dim p = Me.PriceList.FirstOrDefault(Function(x) x.ItemSku = itemSku)
                'If p Is Nothing Then Return String.Empty
                'Return String.Format("{0:C}", p.Price)
                If p Is Nothing Then Return 0
                Return p.Price
            End Function


            Public Shared Function [Get](accountNumber As String) As Customer
                Return DB.FirstOrDefault(Of Customer)("WHERE CUSN05=@0", accountNumber)
            End Function

            Public Shared Function GetForMember(m As Member) As List(Of Customer)
                'Dim strSQL = Nothing
                'Select Case m.AccountType
                '    Case "REP"
                '        strSQL = Sql.Builder.Append("SELECT * FROM REGREPCUST WHERE REPW1=@0", m.RepCode)
                '        Return DB.Fetch(Of Customer)(strSQL)

                '        'Case "DPR"
                '        '    Return DB.Fetch(Of Customer)("WHERE CORP20=@0 AND CUSN05<>@0", m.AccountNumber)

                '    Case Else
                '        '--Match everyone else base on account number only pick one
                '        Return DB.Fetch(Of Customer)("WHERE CUSN05=@0 FETCH FIRST ROW ONLY", m.AccountNumber)
                'End Select

                If m.Level < 3 AndAlso Not String.IsNullOrEmpty(m.Territory) Then
                    Return DB.Fetch(Of Customer)("SELECT C.* FROM CUSTOMERS AS C INNER JOIN WEBDLRSL1 AS D ON (D.CUSNWB=C.CUSN05) WHERE D.SLSMN=@0", m.Territory)
                End If

                If m.AccountType = "REP" Then
                    Return DB.Fetch(Of Customer)("SELECT * FROM REGREPCUST WHERE REPW1=@0 ORDERBY CNAM05", m.RepCode)
                End If

                '--Match everyone else base on account number only pick one
                Return DB.Fetch(Of Customer)("WHERE CUSN05=@0 FETCH FIRST ROW ONLY", m.AccountNumber)
            End Function

            Public Shared Function GetCurrent() As Customer
                Dim m = Member.GetCurrent()
                If m Is Nothing Then Return Nothing

                Return m.Customer
            End Function

            Public Shared Function GetDefault() As Customer
                Return New Customer(True) With {.UPCRule = "CP"}
            End Function

            Public Shared Function GetForRep(repCode As String) As List(Of Customer)
                Dim strSQL = Sql.Builder.Append("SELECT * FROM CUSTOMERS").
                                         Append("WHERE CUSN05=@0 AND ", repCode)

                Return DB.Fetch(Of Customer)(strSQL)
            End Function
        End Class


        Class EmailMessage
            Inherits System.Net.Mail.MailMessage

            Sub New([from] As String, [to] As String)
                MyBase.New([from], [to])
            End Sub



            Sub Send()
                '--Dev only
                If ConfigurationManager.AppSettings("Environment") = "DEV" Then
                    Me.Body &= "<br /><br /><br />==================================================<br />This is a email from the DEV server<br />=================================================="
                    Me.Body &= String.Format("<br />FROM: {0}", Me.From)
                    Me.Body &= String.Format("<br />TO: {0}", String.Join(",", Me.To))
                    Me.Body &= String.Format("<br />BCC: {0}", String.Join(",", Me.Bcc))
                    Me.Body &= "<br />=================================================="
                    Me.From = New Net.Mail.MailAddress("server@coastalpet.com")
                    Me.To.Clear()
                    Me.Bcc.Clear()
                    Me.To.Add(ConfigurationManager.AppSettings("DevEmail"))
                End If

                If Me.To.Count > 0 Then
                    Dim smtp As New System.Net.Mail.SmtpClient()
                    smtp.Send(Me)
                End If
            End Sub
        End Class

        <TableName("SIGNREGREP")>
        Class SalesRep
            <Column("REPW1", Trim:=True)> Public Property Code As String
            <Column("FSTNAMW1", Trim:=True)> Public Property FirstName As String
            <Column("LSTNAMW1", Trim:=True)> Public Property LastName As String
            <Column("USERW1", Trim:=True)> Public Property Email As String
            <Column("BSSTCDW1", Trim:=True)> Public Property State As String
            '<Column("ACCTYPW1", Trim:=True)> Public Property AccountType As String
            '<Column("PRFDSTW1", Trim:=True)> Public Property PreferredDistributor As String
            '<Column("BSNAMW1", Trim:=True)> Public Property BusinessName As String

            <Ignore()>
            ReadOnly Property Name As String
                Get
                    Return String.Format("{0} {1}", Me.FirstName, Me.LastName)
                End Get
            End Property

            Shared Function All() As List(Of SalesRep)
                Dim ret = DirectCast(WSC.Utilities.Cache.Get(Of SalesRep)(), List(Of SalesRep))
                If ret Is Nothing Then
                    ret = DB.Fetch(Of SalesRep)("")
                    If ret.Count > 0 Then WSC.Utilities.Cache.Set(Of SalesRep)(ret, 1)
                End If
                Return ret
            End Function

            Shared Function [Get](code As String) As SalesRep
                Return DB.FirstOrDefault(Of SalesRep)("WHERE REPW1=@0", code)
            End Function

            Shared Function GetByTerritory(territory As String) As List(Of SalesRep)
                Return DB.Fetch(Of SalesRep)("WHERE SLSMNW1=@0", territory)
            End Function
        End Class

        <TableName("WEBSLSTERR")>
        Class Territory
            <Column("PSAR15", Trim:=True)> Public Property ID As String
            <Column("PRMD15", Trim:=True)> Public Property Name As String

            Shared Function All() As List(Of Territory)
                Dim ret = DirectCast(WSC.Utilities.Cache.Get(Of Territory)(), List(Of Territory))
                If ret Is Nothing Then
                    ret = DB.Fetch(Of Territory)("WHERE CONO15='CP' AND PRMT15='TN'")
                    If ret.Count > 0 Then WSC.Utilities.Cache.Set(Of Territory)(ret, 1)
                End If
                Return ret
            End Function
        End Class


        Class StoreType
            Property Key As String
            Property Value As String

            Public Shared Function All() As List(Of StoreType)
                Dim ret As New List(Of StoreType)
                ret.Add(New StoreType() With {.Key = "D", .Value = "Dealer"})
                ret.Add(New StoreType() With {.Key = "0", .Value = "Multi-location Head-Quarters "})
                ret.Add(New StoreType() With {.Key = "1", .Value = "Multi-location Child"})

                Return ret
            End Function
        End Class



        <TableName("WEBDLRSL1", journaled:=False)>
        <PrimaryKey("IDNOWL", autoIncrement:=False)>
        Class Dealer
            <Column("CONOWB")> Property CompanyCode As String = "CP"
            <Column("IDNOWL")> Property ID As Decimal = -1
            <Column("CUSNWB", Trim:=True)> Property CustomerNumber As String = String.Empty
            <Column("DSEQWB", Trim:=True)> Property CustomerDeliverySequence As String = String.Empty
            <Column("QTR1VD")> Property Q1VisitDate As Decimal = 0
            <Column("QTR2VD")> Property Q2VisitDate As Decimal = 0
            <Column("QTR3VD")> Property Q3VisitDate As Decimal = 0
            <Column("QTR4VD")> Property Q4VisitDate As Decimal = 0

            <Column("QTR1TL")> Property Q1VisitTotal As Integer = 0
            <Column("QTR2TL")> Property Q2VisitTotal As Integer = 0
            <Column("QTR3TL")> Property Q3VisitTotal As Integer = 0
            <Column("QTR4TL")> Property Q4VisitTotal As Integer = 0

            <Column("DLRTV")> Property DealerTV As String = String.Empty '--Y or Empty
            <Column("DPROGR")> Property ProGroomer As String = String.Empty '--Y or Empty
            <Column("DIYGRM")> Property DIYGroomer As String = String.Empty '--Y or Empty
            '<Column("DPUBLS")> Property DealerPublished As String = String.Empty '--Y or Empty
            <Column("DLRAPR")> Property DealerApproved As String = String.Empty '--Y or Empty
            '<Column("DATAPP")> Property DateApproved As String
            <Column("MAILER")> Property Mailer As String = String.Empty '--Y or Empty
            <Column("DWEBST", Trim:=True)> Property WebsiteAddress As String = String.Empty
            <Column("DEMAIL", Trim:=True)> Property EmailAddress As String = String.Empty
            '<Column("DTLEML")> Property DateLastEmailed As Date
            '<Column("TMLEML")> Property TimeLastEmailed As String '--(HH:MM:SS)
            <Column("DTLMOD")> Property LastModified As Date
            '<Column("PREFDL")> Property PreferredDealer As String = String.Empty '--Y or Empty
            '<Column("PREFDD")> Property PreferredDealerDate As Decimal
            <Column("SUBREP", Trim:=True)> Property CompletedByRep As String = String.Empty '--Completed By (Sub) Rep code
            <Column("OWNER", Trim:=True)> Property Owner As String = String.Empty
            <Column("MANAGR", Trim:=True)> Property Manager As String = String.Empty
            <Column("DISTRB", Trim:=True)> Property Distributer As String = String.Empty
            '<Column("DISTSQ")> Property DistributerSequence As String
            '<Column("DATANW")> Property DateAddedNew As Decimal
            <Column("DJRPRIC")> Property PriceCode As String '--(D/J)
            <Column("CNAMDL", Trim:=True)> Property BusinessName As String
            <Column("CAD1DL", Trim:=True)> Property BusinessAddress1 As String = String.Empty
            <Column("CAD2DL", Trim:=True)> Property BusinessAddress2 As String = String.Empty
            <Column("CAD3DL", Trim:=True)> Property BusinessAddress3 As String = String.Empty
            <Column("CAD4DL", Trim:=True)> Property BusinessCity As String = String.Empty
            <Column("CAD5DL", Trim:=True)> Property BusinessState As String = String.Empty
            <Column("ZIPCDL", Trim:=True)> Property BusinessZipCode As String = String.Empty
            <Column("PHONDL", Trim:=True)> Property BusinessPhone As String = String.Empty
            <Column("FAXNDL", Trim:=True)> Property BusinessFax As String = String.Empty
            <Column("COUNTRY", Trim:=True)> Property BusinessCountry As String = String.Empty

            <Column("DLRIMG", Trim:=True)> Property ImagePrimaryPath As String = String.Empty
            <Column("OTIMG1", Trim:=True)> Property Image1Path As String = String.Empty
            <Column("OTIMG2", Trim:=True)> Property Image2Path As String = String.Empty
            <Column("OTIMG3", Trim:=True)> Property Image3Path As String = String.Empty
            <Column("OTIMG4", Trim:=True)> Property Image4Path As String = String.Empty
            <Column("OTIMG5", Trim:=True)> Property Image5Path As String = String.Empty


            <Column("SLINFT", Trim:=True)> Property StoreLinearFootage As String = String.Empty
            <Column("SSQRFT", Trim:=True)> Property StoreSquareFootage As String = String.Empty
            <Column("STRLVL", Trim:=True)> Property StoreLevel As String = String.Empty '-- (A/B/C/D)
            <Column("SLSMN", Trim:=True)> Property SalesTerritory As String = String.Empty '-- (REP)
            <Column("STORTYP", Trim:=True)> Property StoreType As String = String.Empty '-- D=Dealer, 0=Headquarters, 1=Child
            <Column("MULTSTHQ", Trim:=True)> Property StoreHeadquarters As Decimal = 0
            <Column("DTPICLUPD")> Property DateLastPicsUpdate As Decimal '--YYYYMMdd
            <Column("NOOFPICS")> Property NumberOfPics As Integer = 0
            <Column("DATANW")> Property DateAddedNew As Decimal '--YYYYMMdd
            <Column("ACTFLVD")> Property Active As String = String.Empty '-- I or " "

            <ResultColumn()> Property Latitude As Decimal
            <ResultColumn()> Property Longitude As Decimal
            <ResultColumn()> Property Distance As Decimal

            <Ignore()>
            ReadOnly Property ImagePrimary As String
                Get
                    Return CleanImagePath(ImagePrimaryPath)
                End Get
            End Property
            <Ignore()>
            ReadOnly Property Image1 As String
                Get
                    Return CleanImagePath(Image1Path)
                End Get
            End Property
            <Ignore()>
            ReadOnly Property Image2 As String
                Get
                    Return CleanImagePath(Image2Path)
                End Get
            End Property
            <Ignore()>
            ReadOnly Property Image3 As String
                Get
                    Return CleanImagePath(Image3Path)
                End Get
            End Property
            <Ignore()>
            ReadOnly Property Image4 As String
                Get
                    Return CleanImagePath(Image4Path)
                End Get
            End Property
            <Ignore()>
            ReadOnly Property Image5 As String
                Get
                    Return CleanImagePath(Image5Path)
                End Get
            End Property

            Private _CategorySelection As List(Of DealerCategorySelection)
            <Ignore()> ReadOnly Property CategorySelection() As List(Of DealerCategorySelection)
                Get
                    If _CategorySelection Is Nothing AndAlso Me.ID > 0 Then
                        _CategorySelection = DealerCategorySelection.GetForDealer(Me.ID)
                    ElseIf _CategorySelection Is Nothing Then
                        'Return New List(Of DealerCategorySelection)
                        _CategorySelection = New List(Of DealerCategorySelection)
                    End If

                    Return _CategorySelection
                End Get
            End Property

            Private _Children As List(Of Dealer)
            <ResultColumn()> ReadOnly Property Children() As List(Of Dealer)
                Get
                    If _Children Is Nothing AndAlso Me.ID > 0 Then
                        _Children = GetChildren(Me.ID)
                    Else
                        Return New List(Of Dealer)
                    End If
                    Return _Children
                End Get
            End Property

            Private Function CleanImagePath(path As String) As String
                If String.IsNullOrEmpty(path) Then Return String.Empty
                Return String.Format("/media/website/authorizeddealer/{0}", path)
            End Function

            Private Shared Function GetNextId() As Integer
                'Return DB.FirstOrDefault(Of Integer)("Select IDNOWL FROM WEBDLRSL1 ORDER BY IDNOWL DESC FETCH FIRST 1 ROW ONLY") + 1
                Return DB.FirstOrDefault(Of Integer)("Select IDNOWL FROM WEBDLRSL5 ORDER BY IDNOWL DESC FETCH FIRST 1 ROW ONLY") + 1
            End Function


            Sub Save()
                Me.LastModified = Now()
                If Me.ID < 0 Then
                    Me.ID = GetNextId()
                    DB.Insert(Me)
                Else
                    DB.Update(Me)
                End If

                For Each cs In CategorySelection
                    cs.DealerID = Me.ID
                    cs.Save()
                Next
            End Sub

            Public Shared Function All() As List(Of Dealer)
                Dim pd = WSC.PetaPoco.Database.PocoData.ForType(New WSC.Datalayer.Dealer().GetType)
                Dim strSQL = Sql.Builder.Append("SELECT " & DB.QueryColumnSQL(pd)).
                                         Append(", WEBDLRSEL1.LATIDE AS Latitude, WEBDLRSEL1.LONGDE AS Longitude").
                                         Append("FROM WEBDLRSL1").
                                         Append("LEFT JOIN WEBDLRSEL1 ON (WEBDLRSEL1.IDNODE = WEBDLRSL1.IDNOWL)").
                                         Append("WHERE CONOWB='CP'")
                Return DB.Fetch(Of Dealer)(strSQL)
            End Function


            Public Shared Function [Get](id As String) As Dealer
                Return DB.FirstOrDefault(Of Dealer)("WHERE CONOWB='CP' AND IDNOWL=@0", id)
            End Function

            Public Shared Function GetByTerritory(territory As String) As List(Of Dealer)
                Return DB.Fetch(Of Dealer)("WHERE CONOWB='CP' AND SLSMN=@0", territory)
            End Function

            Public Shared Function GetByTerritory(territory As String, storeType As String) As List(Of Dealer)
                'Dim strSQL = Sql.Builder.Append("WHERE SLSMN=@0 AND STORTYP=@1", territory, storeType)
                Dim strSQL = Sql.Builder.Append("WHERE CONOWB='CP' AND SLSMN=@0", territory)

                If String.IsNullOrEmpty(storeType) Then
                    strSQL.Append("AND STORTYP<>'1'")
                Else
                    strSQL.Append("AND STORTYP=@0", storeType)
                End If

                'If storeType = "1" Then
                '    strSQL = Sql.Builder.Append("WHERE STORTYP <> 'D' ORDER BY STORTYP")
                'Else
                '    strSQL = Sql.Builder.Append("WHERE STORTYP=@0", storeType)
                'End If
                Return DB.Fetch(Of Dealer)(strSQL)
            End Function

            Public Shared Function GetByRep(repCode As String) As List(Of Dealer)
                Return DB.Fetch(Of Dealer)("WHERE CONOWB='CP' AND SUBREP=@0", repCode)
            End Function

            Public Shared Function GetByRep(repCode As String, storeType As String) As List(Of Dealer)
                Dim strSQL = Sql.Builder.Append("WHERE CONOWB='CP' AND SUBREP=@0", repCode)

                If String.IsNullOrEmpty(storeType) Then
                    strSQL.Append("AND STORTYP<>'1'")
                Else
                    strSQL.Append("AND STORTYP=@0", storeType)
                End If

                Return DB.Fetch(Of Dealer)(strSQL)
            End Function


            Public Shared Function GetChildren(id As Decimal) As List(Of Dealer)
                'Removed  AND MULTSTHQ=@0 10/12/2016 BK Per Brooke Thornberry
                'Added AND MULTSTHQ=1 10/20/2016 to limit children to logged in rep
                Return DB.Fetch(Of Dealer)("WHERE CONOWB='CP' AND MULTSTHQ=1", id)
            End Function

            Public Shared Function SearchByZIP(zip As String, distance As Integer) As List(Of Dealer)
                Dim location = WSC.Utilities.Geo.Get(zip)
                If location Is Nothing Then Return New List(Of Dealer)
                Dim dealers = Dealer.All().Where(Function(x) x.DealerApproved = "Y" AndAlso x.Active = " " AndAlso location.Distance(x.Latitude, x.Longitude) < distance).ToList()
                For Each d In dealers
                    d.Distance = location.Distance(d.Latitude, d.Longitude)
                Next

                Return dealers.OrderBy(Function(x) x.Distance).ToList
            End Function

        End Class


        <TableName("DISTJ03")>
        Class Distributor
            <Column("CNAM05", trim:=True)> Property Name As String
            <Column("WCUSCITY", trim:=True)> Property City As String
            <Column("WCUSSTAT", trim:=True)> Property StateName As String
            <Column("CNTRYP2", trim:=True)> Property Country As String
            <Column("STATEP2", trim:=True)> Property State As String
            <Column("WEBSITP1", trim:=True)> Property Website As String
            <Column("PHONEP1", trim:=True)> Property Phone As String

            Public Shared Function Search(state As String, region As String) As List(Of Distributor)
                Return DB.Fetch(Of Distributor)("WHERE (STATEP2=@0 OR @0='') AND (CNTRYP2=@1 OR @1='')", state, region)
            End Function

            'Public Shared Function Countries() As List(Of String)
            '    Return DB.Fetch(Of String)("SELECT DISTINCT TRIM(CNTRYP2) FROM DISTJ03 WHERE CONOP1='CP' AND  CNTRYP2<>'USA' AND CNTRYP2<>''")
            'End Function

            'Public Shared Function States() As List(Of String)
            '    'Return DB.Fetch(Of String)("SELECT DISTINCT TRIM(WCUSSTAT) FROM DISTJ03 WHERE CONOP1='CP' AND  CNTRYP2='USA'")
            'End Function

            Public Shared Function Countries() As List(Of KeyValue(Of String, String))
                Dim strSQL = Sql.Builder.Append("SELECT TRIM(PMCD60) AS Key, TRIM(DESC60) AS Value").
                                         Append("FROM WEBCOUNTRY").
                                         Append("WHERE CONO60='CP' AND PMTP60='CCOD' AND PMCD60 IN (SELECT DISTINCT CNTRYP2 FROM DISTJ03 WHERE CONOP1='CP' OR SUBSTRING(CNTRYP2,0,3)<>'US')").
                                         Append("ORDER BY Value")
                Return DB.Fetch(Of KeyValue(Of String, String))(strSQL)
            End Function

            Public Shared Function States() As List(Of KeyValue(Of String, String))
                Dim strSQL = Sql.Builder.Append("SELECT DISTINCT TRIM(D1.PSAR15) AS Key, TRIM(D1.PRMD15) AS Value").
                                         Append("FROM DESC AS D1").
                                         Append("LEFT JOIN DISTJ03 AS D2 ON (D2.CONOP1=D1.CONO15 AND D2.STATEP2=D1.PSAR15)").
                                         Append("WHERE D1.CONO15='CP' AND D1.PRMT15='ST' AND D2.CNTRYP2 IN('USA','CA')").
                                         Append("ORDER BY Value")
                Return DB.Fetch(Of KeyValue(Of String, String))(strSQL)
            End Function

            Public Class KeyValue(Of T1, T2)
                Public Property Key As T1
                Public Property Value As T2
            End Class
        End Class

        <TableName("WEBDLRSLC1", journaled:=False)>
        Class DealerCategorySelection
            <Column("CONOWB")> Property CompanyCode As String = "CP"
            <Column("IDNOWB")> Property DealerID As Integer
            <Column("CUSNDC", Trim:=True)> Property CustomerNumber As String = String.Empty '--Optional
            <Column("DSEQDC")> Property CustomerDeliverySeq As String = String.Empty '--Only if CustomerNumber entered
            <Column("CATTYP")> Property CategoryType As String '--A = Authorize, L = Licensed, O = Other
            <Column("DLRCATSQ")> Property Sequence As Integer = 1
            <Column("CATCOD", Trim:=True)> Property CategoryCode As String
            <Column("ACTFLG")> Property Active As String = " " '-- blank or I
            <Column("DTCANC")> Property CancelDate As Decimal

            Sub New()

            End Sub

            Sub New(c As DealerCategory)
                Me.CategoryType = c.CategoryType
                Me.CategoryCode = c.Code
            End Sub

            Sub Save()
                'Dim cur = [Get](Me.DealerID, Me.CategoryCode)
                If DB.ExecuteScalar(Of Integer)("SELECT COUNT(*) FROM WEBDLRSLC1 WHERE CONOWB='CP' AND IDNOWB=@DealerID AND CATCOD=@CategoryCode", Me) > 0 Then
                    'If cur IsNot Nothing Then
                    'DB.Update(Me)
                    DB.Execute("UPDATE WEBDLRSLC1 SET CUSNDC=@CustomerNumber, DSEQDC=@CustomerDeliverySeq, ACTFLG=@Active, DTCANC=@CancelDate  WHERE CONOWB=@CompanyCode AND IDNOWB=@DealerID AND CATCOD=@CategoryCode WITH NONE", Me)
                Else
                    DB.Insert(Me)
                End If
            End Sub

            'Public Shared Function [Get](dealerId As Integer, categoryCode As String) As DealerCategorySelection
            '    Return DB.FirstOrDefault(Of DealerCategorySelection)("WHERE IDNOWB=@0 AND CATCOD=@1", dealerId, categoryCode)
            'End Function

            Public Shared Function GetForDealer(dealerId As Decimal) As List(Of DealerCategorySelection)
                Return DB.Fetch(Of DealerCategorySelection)("WHERE CONOWB='CP' AND IDNOWB=@0", dealerId)
            End Function


        End Class

        <TableName("WEBDLRCAT1")>
        Class DealerCategory
            <Column("CATCOD", Trim:=True)> Property Code As String
            <Column("CATDSC", Trim:=True)> Property Name As String
            <Column("CATTYP")> Property CategoryType As String 'A = Authorize, L = Licensed, O = Other

            Shared Function All() As List(Of DealerCategory)
                Dim ret = DirectCast(WSC.Utilities.Cache.Get(Of DealerCategory)(), List(Of DealerCategory))
                If ret Is Nothing Then
                    ret = DB.Fetch(Of DealerCategory)("WHERE ACTVFLG=''").OrderBy(Function(x) x.Name).ToList
                    If ret.Count > 0 Then
                        WSC.Utilities.Cache.Set(Of DealerCategory)(ret, 1)
                    End If
                End If
                Return ret
            End Function
        End Class

        Class Feature
            Property Url As String
            Property Image As String
            Property Name As String
            Property Price As String

            Sub New(p As Product)
                Me.Url = p.NiceUrl
                Me.Image = p.Image
                Me.Name = p.Name
                Me.Price = p.Price
            End Sub

            Sub New(b As Brand)
                Me.Url = b.NiceUrl
                Me.Image = b.Image
                Me.Name = b.Name
                Me.Price = String.Empty
            End Sub

            ''' <summary>
            ''' CPBRN – Brand Category Page
            ''' CPCAT – Cat Category Page
            ''' CPDOG – Dog Category Page
            ''' CPFIT – Featured Item Category Page
            ''' CPOTH – Other Category Page
            ''' </summary>
            ''' <param name="type">CPBRN,CPCAT,CPDOG,CPFIT,CPOTH</param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Shared Function [Get](type As String) As List(Of Feature)
                Dim ret As New List(Of Feature)
                If type = "CPBRN" Then
                    For Each b In Brand.GetFeatured().Take(5)
                        ret.Add(New Feature(b))
                    Next
                Else
                    For Each p In Product.GetFeatured(type).Take(6)
                        ret.Add(New Feature(p))
                    Next
                End If
                Return ret
            End Function
        End Class


        <TableName("SIGNONS")>
        <PrimaryKey("IDNOW1")>
        Class Member
            <Column("IDNOW1")> Public Property ID As Integer
            <Column("USERW1", Trim:=True)> Public Property UserName As String
            <Column("PASSWDW1", Trim:=True)> Public Property Password As String
            <Column("FSTNAMW1", Trim:=True)> Public Property FirstName As String
            <Column("LSTNAMW1", Trim:=True)> Public Property LastName As String
            <Column("SLSMNW1", Trim:=True)> Public Property Territory As String
            <Column("REPW1", Trim:=True)> Public Property RepCode As String
            '<Column("REGNLW1", Trim:=True)> Public Property NotSure As String
            '<Column("NATNLW1", Trim:=True)> Public Property NotSure As String
            <Column("CNTACW1", Trim:=True)> Public Property ContactName As String
            <Column("BSNAMW1", Trim:=True)> Public Property BusinessName As String
            <Column("BSADD1W1", Trim:=True)> Public Property Address1 As String
            <Column("BSADD2W1", Trim:=True)> Public Property Address2 As String
            <Column("BSADD3W1", Trim:=True)> Public Property Address3 As String
            <Column("BSCITYW1", Trim:=True)> Public Property City As String
            <Column("BSSTCDW1", Trim:=True)> Public Property State As String
            <Column("BSZIPW1", Trim:=True)> Public Property Zip As String
            '<Column("BSPHNW1", Trim:=True)> Public Property NotSure As String
            '<Column("ACCONOW1", Trim:=True)> Public Property NotSure As String
            <Column("ACCNTW1", Trim:=True)> Public Property AccountNumber As String
            '<Column("ACDSEQW1", Trim:=True)> Public Property NotSure As String
            <Column("ACCTYPW1", Trim:=True)> Public Property AccountType As String '--ADMIN,ASC,DIST,DPR,DS,DSD,INTRN,LIC,MARK,REG,REP
            '<Column("AUTDLRW1", Trim:=True)> Public Property NotSure As String
            '<Column("PRCONOW1", Trim:=True)> Public Property NotSure As String
            <Column("PRFDSTW1", Trim:=True)> Public Property PreferredDistributor As String
            '<Column("PRDSEQW1", Trim:=True)> Public Property NotSure As String
            <Column("PRFDNMW1", Trim:=True)> Public Property PreferredName As String
            '<Column("STATUSW1", Trim:=True)> Public Property NotSure As String
            '<Column("LSTDSTW1", Trim:=True)> Public Property NotSure As String
            '<Column("CREATW1", Trim:=True)> Public Property NotSure As String
            '<Column("LSTUSW1", Trim:=True)> Public Property NotSure As String
            <Column("LEVELW1", Trim:=True)> Public Property Level As String
            <Column("WEBSKNOVR", Trim:=True)> Public Property SkinOverride As String



            <Ignore()> Public Property IsLoggedIn As Boolean = False

            'Private _PriceList As List(Of Price)
            '<Ignore()>
            'Public ReadOnly Property PriceList As List(Of Price)
            '    Get
            '        'If _PriceList Is Nothing Then
            '        '    If Me.Customer IsNot Nothing Then
            '        '        _PriceList = Price.GetForMember(Me.Customer.UPCRule, Me.Customer.PriceCode, Me.Customer.AccountNumber)
            '        '    End If
            '        'End If
            '        Return _PriceList
            '    End Get
            'End Property

            <Ignore()>
            Public ReadOnly Property HasPortalAccess As Boolean
                Get
                    If Not Me.IsLoggedIn Then Return False
                    'Return "Test".Contains(Me.AccountType)
                    Return Not String.IsNullOrEmpty(Me.Level)
                End Get
            End Property

            Private _customer As Customer
            <ResultColumn()>
            Public Property Customer As Customer
                Get
                    If _customer Is Nothing Then
                        '_customer = Customer.GetForRep(Me.RepCode)
                        '_customer.GetExlusions()
                        Me.Customer = Customer.GetDefault()
                    End If
                    Return _customer
                End Get
                Set(value As Customer)
                    If value IsNot Nothing Then
                        _customer = value
                        'If Me.IsLoggedIn Then
                        '    _customer.GetExlusions()
                        '    _customer.GetPrices()
                        'End If

                        '_PriceList = Price.GetForMember(Me.Customer.UPCRule, Me.Customer.PriceCode, Me.Customer.AccountNumber)
                        '_PriceList = Price.GetForMember(Me)
                    End If
                End Set
            End Property

            <Ignore()>
            ReadOnly Property Name As String
                Get
                    Return String.Format("{0} {1}", Me.FirstName, Me.LastName)
                End Get
            End Property


            Private Shared SessionKey As String = "CPP.Member"

            Sub New()
            End Sub

            Sub Persist()
                HttpContext.Current.Session(SessionKey) = Me
            End Sub

            'Public Shared Function All() As List(Of Member)
            '    'Dim ret = DB.Fetch(Of Member)("SELECT * FROM SIGNONS FETCH FIRST 10 ROWS ONLY")
            '    'DB.CloseSharedConnection()
            '    'Return ret
            '    Return DB.Fetch(Of Member)("SELECT * FROM SIGNONS FETCH FIRST 10 ROWS ONLY")
            'End Function

            'Public Shared Function [Get](id As Integer) As Member
            '    Return Nothing
            'End Function

            Public Shared Function [Get](userName As String, password As String) As Member
                'Dim strSQL = Sql.Builder.Append("SELECT * FROM SIGNONS").
                '                         Append("WHERE USERW1=@0 AND PASSWDW1=@1", userName.ToUpper, password)
                'Return DB.FirstOrDefault(Of Member)(strSQL)
                Return DB.FirstOrDefault(Of Member)("WHERE USERW1=@0 AND PASSWDW1=@1", userName.ToUpper, password)
            End Function

            Public Shared Function Exists(userName As String) As Boolean
                'Dim strSQL = Sql.Builder.Append("SELECT COUNT(*) FROM SIGNONS").
                '                         Append("WHERE USERW1=@0", userName.ToUpper)
                'Return (DB.ExecuteScalar(Of Integer)(strSQL) > 0)
                Return (DB.ExecuteScalar(Of Integer)("WHERE USERW1=@0", userName.ToUpper) > 0)
            End Function



            Public Shared Function GetByEmail(email As String) As Member
                'Dim strSQL = Sql.Builder.Append("SELECT * FROM SIGNONS").
                '                         Append("WHERE USERW1=@0 OR USERW1=@1", email, email.ToUpper)
                'Return DB.FirstOrDefault(Of Member)(strSQL)
                Return DB.FirstOrDefault(Of Member)("WHERE USERW1=@0 OR USERW1=@1", email, email.ToUpper)
            End Function

            Public Shared Function GetDefault() As Member
                Dim m As New Member()
                m.Customer = Customer.GetDefault()
                Return m
            End Function

            Public Shared Function GetCurrent() As Member
                If HttpContext.Current.Session(SessionKey) IsNot Nothing Then
                    Return CType(HttpContext.Current.Session(SessionKey), Member)
                End If
                Return GetDefault()
                'Return Nothing
            End Function

            Public Shared Sub Logout()
                HttpContext.Current.Session.Remove(SessionKey)
            End Sub

            Public Shared Function Login(userName As String, password As String) As Boolean
                Dim m = [Get](userName, password)
                If m IsNot Nothing Then
                    m.IsLoggedIn = True
                    m.Persist()
                    Return True
                End If
                'HttpContext.Current.Response.Clear()
                'HttpContext.Current.Response.Write("SQL [" & DB.LastSQL & "]" & vbCrLf)
                'HttpContext.Current.Response.Write("CMD [" & DB.LastCommand & "]" & vbCrLf)
                ''HttpContext.Current.Response.Write("ARG [" & String.Join(",", DB.LastArgs.ToArray) & "]" & vbCrLf)
                'HttpContext.Current.Response.End()
                Return False
            End Function

        End Class

        <TableName("PRICE")>
        Class Price
            <Column("CATNWP", Trim:=True)> Public Property ItemSku As String
            <Column("PRCEWP")> Public Property Price As Decimal
            <Column("STYGMS", Trim:=True)> Public Property StyleGroup As String

            <Ignore()> Public Property Min As Double = 0
            <Ignore()> Public Property Max As Double = 0


            Public Overrides Function ToString() As String
                'Return MyBase.ToString()
                If Me.Min > 0 AndAlso Me.Max > 0 AndAlso Me.Min <> Me.Max Then Return String.Format("{0:C} - {1:C}", Me.Min, Me.Max)
                Return String.Format("{0:C}", Me.Min)
            End Function

            Public Shared Function GetForMember(m As Member) As List(Of Price)
                If Not m.IsLoggedIn Then Return New List(Of Price)
                Return GetForCustomer(m.Customer)
            End Function

            Public Shared Function GetForCustomer(c As Customer) As List(Of Price)
                If String.IsNullOrEmpty(c.PriceCode) Or String.IsNullOrEmpty(c.AccountNumber) Or String.IsNullOrEmpty(c.UPCRule) Then Return New List(Of Price)

                Dim ret As New List(Of Price)
                '--Price at sku level
                'Dim strSQL = Sql.Builder.Append("SELECT I.PNUM35 AS ProductID, MIN(PRCEWP) AS Min, MAX(PRCEWP) AS Max").
                '                         Append("FROM PRICE, ITEMS AS I").
                '                         Append("WHERE I.RUL31C=@", c.UPCRule).
                '                         Append("LISTWP = @0", c.PriceCode).
                'Append("AND I.PNUM35 NOT IN (SELECT E.PNUMX5 FROM EXCLUSN AS E WHERE E.CUSNX5=@0)", c.AccountNumber).
                '                         Append("GROUP BY STYLE, PTYP35")
                Dim strSQL = Sql.Builder()
                strSQL.Append("SELECT TRIM(PRICE.CATNWP) AS CATNWP, PRICE.PRCEWP, TRIM(ITEMS.STYGMS) AS STYGMS")
                strSQL.Append("FROM PRICE")
                strSQL.Append("INNER JOIN ITEMS ON (ITEMS.PNUM35=PRICE.CATNWP AND ITEMS.CONO35=PRICE.CONOWP)")
                'strSQL.Append("WHERE LISTWP=@PriceCode AND ITEMS.RUL31C=@UPCRule AND ITEMS.PNUM35 NOT IN (SELECT E.PNUMX5 FROM EXCLUSN AS E WHERE E.CUSNX5=@PriceCode)", c)
                strSQL.Append("WHERE LISTWP=@PriceCode AND ITEMS.RUL31C=@UPCRule AND ITEMS.PNUM35 NOT IN (SELECT E.PNUMX5 FROM EXCLUSN AS E WHERE E.CUSNX5=@AccountNumber)", c)

                ret.AddRange(DB.Fetch(Of Price)(strSQL))

                ''--Price at product level
                'strSQL = Sql.Builder.Append("SELECT TRIM(STYGMS) AS ProductID, MIN(PRCEWP) AS Min, MAX(PRCEWP) AS Max").
                '                     Append("FROM PRICESTG").
                '                     Append("WHERE LISTWP='DLR' AND STYGMS <> ''").
                '                     Append("GROUP BY STYGMS")
                'ret.AddRange(DB.Fetch(Of Price)(strSQL))

                Return ret
            End Function
        End Class


        Class ProductAttribute
            Public Property ID As String
            Public Property Name As String
        End Class

        Class AnimalType
            Inherits ProductAttribute

            Public Shared Function All() As List(Of AnimalType)
                Dim ret As List(Of AnimalType) = DirectCast(WSC.Utilities.Cache.Get(Of AnimalType)(), List(Of AnimalType))
                If ret Is Nothing Then
                    ret = DB.Fetch(Of AnimalType)("SELECT TRIM(PSARC5) AS ID, TRIM(CHR3C5) AS Name FROM COASTDESC WHERE PRMTC5='WEBANIMAL'")
                    '--Remove CATDG
                    'ret.Remove(ret.FirstOrDefault(Function(x) x.ID = "CATDG"))
                    '--Cache for 1hr
                    WSC.Utilities.Cache.Set(Of AnimalType)(ret, 1)
                End If
                Return ret
            End Function
        End Class

        Class ProductCategory
            Inherits ProductAttribute

            Public Shared Function All() As List(Of ProductCategory)
                Dim ret As List(Of ProductCategory) = DirectCast(WSC.Utilities.Cache.Get(Of ProductCategory)(), List(Of ProductCategory))
                If ret Is Nothing Then
                    Dim strSQL = Sql.Builder.Append("SELECT TRIM(PSARC5) AS ID, TRIM(CHR4C5) AS Name FROM COASTDESC WHERE PRMTC5='WEBPRODCAT'")
                    ret = DB.Fetch(Of ProductCategory)(strSQL)
                    '--Cache for 1hr
                    WSC.Utilities.Cache.Set(Of ProductCategory)(ret, 1)
                End If
                Return ret
            End Function
        End Class

        Class ProductType
            Inherits ProductAttribute

            Public Shared Function All() As List(Of ProductType)
                Dim ret As List(Of ProductType) = DirectCast(WSC.Utilities.Cache.Get(Of ProductType)(), List(Of ProductType))
                If ret Is Nothing Then
                    Dim strSQL = Sql.Builder.Append("SELECT TRIM(PSARC5) AS ID, TRIM(CHR4C5) AS Name FROM COASTDESC WHERE PRMTC5='WEBPRODTYPE'")
                    ret = DB.Fetch(Of ProductType)(strSQL)
                    '--Cache for 1hr
                    WSC.Utilities.Cache.Set(Of ProductType)(ret, 1)
                End If
                Return ret
            End Function
        End Class

        Class ProductMaterial
            Inherits ProductAttribute

            Public Shared Function All() As List(Of ProductMaterial)
                Dim ret As List(Of ProductMaterial) = DirectCast(WSC.Utilities.Cache.Get(Of ProductMaterial)(), List(Of ProductMaterial))
                If ret Is Nothing Then
                    Dim strSQL = Sql.Builder.Append("SELECT TRIM(PSARC5) AS ID, TRIM(CHR4C5) AS Name FROM COASTDESC WHERE PRMTC5='WEBMKTMAT'")
                    ret = DB.Fetch(Of ProductMaterial)(strSQL)
                    '--Cache for 1hr
                    WSC.Utilities.Cache.Set(Of ProductMaterial)(ret, 1)
                End If
                Return ret
            End Function
        End Class

        Class ProductFeature
            Inherits ProductAttribute

            Public Shared Function All() As List(Of ProductFeature)
                Dim ret As List(Of ProductFeature) = DirectCast(WSC.Utilities.Cache.Get(Of ProductFeature)(), List(Of ProductFeature))
                If ret Is Nothing Then
                    Dim strSQL = Sql.Builder.Append("SELECT TRIM(PSARC5) AS ID, TRIM(CHR4C5) AS Name FROM COASTDESC WHERE PRMTC5='WEBFEATURES'")
                    ret = DB.Fetch(Of ProductFeature)(strSQL)
                    '--Cache for 1hr
                    WSC.Utilities.Cache.Set(Of ProductFeature)(ret, 1)
                End If
                Return ret
            End Function
        End Class

        <TableName("ITEMS")>
        Class KitItem
            <Column("PDES35", Trim:=True)> Public Property Name As String
            <Column("PNUM35", Trim:=True)> Public Property Sku As String
            '<Column("SPC135")> Public Property Length As String '--Size of kit top to bottom
            '<Column("SPC335")> Public Property Height As String '--Kit Depth likely not needed
            <Column("SPC235")> Public Property Width As Decimal
            <Column("SPC135")> Public Property Height As Decimal
            <Column("IMAGEPATH", Trim:=True)> Public Property ImagePath As String
            <Column("ImageSequence")> Public Property ImageSequence As String

            Public ReadOnly Property Image As String
                Get
                    Return CleanImagePath(Me.ImagePath)
                End Get
            End Property


            Private Shared Function BaseSQL() As Sql
                Dim trimmedSQL = TrimSQLFields(Of KitItem)({"IMAGEPATH", "ImageSequence"})
                Dim strSQL = Sql.Builder.Append("SELECT " & trimmedSQL & ", TRIM(M.PATHIM) AS IMAGEPATH, M.SQNOIM AS ImageSequence").
                                         Append("FROM ITEMS AS I").
                                         Append("LEFT JOIN WEBITMIMG AS M ON (M.CONOIM=I.CONO35 AND M.PNUMIM=I.PNUM35 AND M.ITYPIM='B')").
                                         Append("WHERE I.PTYP35='K' AND M.PATHIM IS NOT NULL")
                Return strSQL
            End Function


            Public Shared Function All(c As Customer) As List(Of KitItem)
                Dim strSQL = BaseSQL.Append("AND I.RUL31C=@0", c.UPCRule)
                Return DB.Fetch(Of KitItem)(strSQL)
            End Function

            Public Shared Function All() As List(Of KitItem)
                'Dim strSQL = Sql.Builder.Append("SELECT * FROM ITEMS WHERE PTYP35 = 'K' FETCH FIRST 10 ROWS ONLY")
                'Return DB.Fetch(Of Kit)(strSQL)
                Return All(Customer.GetDefault())
            End Function

            Public Shared Function [Get](id As String) As KitItem
                Return [Get](id, Customer.GetDefault())
            End Function

            Public Shared Function [Get](id As String, c As Customer) As KitItem
                Dim strSQL = BaseSQL.Append("AND PNUM35=@0 AND I.RUL31C=@1", id, c.UPCRule)
                Return DB.FirstOrDefault(Of KitItem)(strSQL)
            End Function

            Public Shared Function [Get](id As String, imageSequence As Integer) As KitItem
                Return [Get](id, imageSequence, Customer.GetDefault())
            End Function

            Public Shared Function [Get](id As String, imageSequence As Integer, c As Customer) As KitItem
                Dim strSQL = BaseSQL.Append("AND PNUM35=@0 AND I.RUL31C=@1 AND M.SQNOIM=@2", id, c.UPCRule, imageSequence)
                Return DB.FirstOrDefault(Of KitItem)(strSQL)
            End Function

            'Public Shared Function GetBuIDs(ids As List(Of String)) As Kit
            '    Dim strSQL = BaseSQL.Append("AND PNUM35 IN @0", ids)
            '    Return DB.FirstOrDefault(Of Kit)(strSQL)
            'End Function
        End Class

        '<TableName("KITS")>
        'Class KitComponent
        '    Inherits Item

        '    <Column("NOFF65")> Public Property Quantity As Integer


        '    Public Shared Function GetForKit(kitNumber As String) As List(Of KitComponent)
        '        Dim strSQL = Sql.Builder.Append("SELECT I.*, K.NOFF65")
        '        strSQL.Append("FROM ITEMS AS I")
        '        strSQL.Append("INNER JOIN KITS AS K ON (K.CMPT65=I.PNUM35)")
        '        strSQL.Append("WHERE K.CONO65='CP' AND K.PNUM65=@0 AND RUL31C='CP'", kitNumber)
        '        'Dim temp = DB.Fetch(Of KitData)(strSQL)
        '        'Dim ret = Item.
        '        Return DB.Fetch(Of KitComponent)(strSQL)
        '        Return Nothing
        '    End Function

        '    '<TableName("KITS")>
        '    'Public Class KitData
        '    '    <Column("CMPT65")> Public Property ItemNumber As String
        '    '    <Column("NOFF65")> Public Property Quantity As Integer
        '    'End Class
        'End Class

        <TableName("WEBSTGKACT")>
        <Serializable()>
        Class Kit
            <Column("STYGMS")> Public Property ID As String
            <Column("PRNMMS")> Public Property Name As String
            <Column("WATYMS")> Public Property AnimalType As String
            <Column("WCATMS")> Public Property Category As String
            '<Column("PTYPMS")> Public Property ProductType As String
            <Column("MBRNMS")> Public Property BrandID As String
            '<Column("MMATMS")> Public Property Material As String
            <Column("MKEYMS")> Public Property Keywords As String
            <Column("PATHIM")> Public Property ImagePath As String = String.Empty
            <Column("IALTMS")> Public Property AltTag As String
            <Column("VFLGMS")> Public Property VideoFlag As String
            <Column("CSVFMS")> Public Property SearchTerms As String
            <ResultColumn()> Public Property DescriptionLong As String = String.Empty
            <ResultColumn()> Public Property DescriptionShort As String = String.Empty
            <ResultColumn()> Public Property FeatureDescription As String = String.Empty
            <ResultColumn()> Public Property Application As String = String.Empty
            <ResultColumn()> Public Property MetaTitle As String = String.Empty
            <ResultColumn()> Public Property MetaDescription As String = String.Empty
            <ResultColumn()> Public Property BrandOrder As Decimal = 999


            '<ResultColumn()> Public Property FeatureID As String
            <ResultColumn()> Public Property Price As String = String.Empty

            'Public Property Features As New List(Of String)
            'Public ReadOnly Property FeaturesSearch As String
            '    Get
            '        Return String.Join(" ", Me.Features.ToArray)
            '    End Get
            'End Property

            'Private _Related As List(Of Product)
            'Public ReadOnly Property Related As List(Of Product)
            '    Get
            '        If _Related Is Nothing Then
            '            Dim strSQL = Sql.Builder.Append("SELECT RSTGRI").
            '                                     Append("FROM WEBSTGRLT").
            '                                     Append("WHERE CONORI='CP' AND STYGRI=@0", Me.ID).
            '                                     Append("ORDER BY SEQNRI")
            '            Dim ids = DB.Fetch(Of String)(strSQL).ToArray()
            '            _Related = GetByIDs(ids)
            '            If _Related Is Nothing Then
            '                _Related = New List(Of Product)
            '            End If
            '        End If
            '        Return _Related
            '    End Get
            'End Property


            Private Shared ReadOnly Property TrimmedSQL As String
                Get
                    Return TrimSQLFields(Of Product)({"PATHIM", "BrandOrder"})
                End Get
            End Property

            Private Shared ReadOnly Property FullSQL As String
                Get
                    Dim strSQL = Sql.Builder.Append("SELECT " & TrimmedSQL & ", TRIM(I.PATHIM) AS PATHIM").
                                            Append(",D.NUM1C5 AS BrandOrder").
                                            Append(",TRIM(D1.PDESXD) AS DescriptionShort").
                                            Append(",TRIM(D2.PDESXD) AS DescriptionLong").
                                            Append(",TRIM(D3.PDESXD) AS FeatureDescription").
                                            Append(",TRIM(D4.PDESXD) AS Application").
                                            Append(",TRIM(D5.PDESXD) AS MetaDescription").
                                            Append(",TRIM(D6.PDESXD) AS MetaTitle").
                                            Append("FROM WEBSTGKACT AS W").
                                            Append("LEFT JOIN WEBITMIMG AS I ON (I.CONOIM=W.CONOMS AND I.PNUMIM=W.FIMGMS AND I.ITYPIM='I')").
                                            Append("LEFT JOIN WEBSTGEXD AS D1 ON (D1.CONOXD=W.CONOMS AND D1.STYGXD=W.STYGMS AND D1.DTYPXD='S')").
                                            Append("LEFT JOIN WEBSTGEXD AS D2 ON (D2.CONOXD=W.CONOMS AND D2.STYGXD=W.STYGMS AND D2.DTYPXD='L')").
                                            Append("LEFT JOIN WEBSTGEXD AS D3 ON (D3.CONOXD=W.CONOMS AND D3.STYGXD=W.STYGMS AND D3.DTYPXD='F')").
                                            Append("LEFT JOIN WEBSTGEXD AS D4 ON (D4.CONOXD=W.CONOMS AND D4.STYGXD=W.STYGMS AND D4.DTYPXD='A')").
                                            Append("LEFT JOIN WEBSTGEXD AS D5 ON (D5.CONOXD=W.CONOMS AND D5.STYGXD=W.STYGMS AND D5.DTYPXD='M')").
                                            Append("LEFT JOIN WEBSTGEXD AS D6 ON (D6.CONOXD=W.CONOMS AND D6.STYGXD=W.STYGMS AND D6.DTYPXD='T')").
                                            Append("LEFT JOIN COASTDESC AS D ON (W.MBRNMS=D.PSARC5 AND D.CONOC5='CP' AND D.PRMTC5='WEBMKTBRAND')")






                    'Append(", (SELECT TRIM(PDESXD) FROM WEBSTGEXD WHERE CONOXD=W.CONOMS AND STYGXD=W.STYGMS AND DTYPXD='S') AS SHORTDESCRIPTION").
                    'Append(", (SELECT TRIM(PDESXD) FROM WEBSTGEXD WHERE CONOXD=W.CONOMS AND STYGXD=W.STYGMS AND DTYPXD='L') AS LONGDESCRIPTION").

                    'Append("LEFT JOIN WEBSTGDSC AS D1 ON (D1.CONODS=W.CONOMS AND D1.STYGDS=W.STYGMS AND D1.DTYPDS='F'))").
                    'Append("LEFT JOIN WEBSTGDSC AS D2 ON (D2.CONODS=W.CONOMS AND D2.STYGDS=W.STYGMS AND D2.DTYPDS='A')").
                    'Append("LEFT JOIN WEBSTGDSC AS D3 ON (D3.CONODS=W.CONOMS AND D3.STYGDS=W.STYGMS AND D3.DTYPDS='S')").
                    'Append("LEFT JOIN WEBSTGDSC AS D4 ON (D4.CONODS=W.CONOMS AND D4.STYGDS=W.STYGMS AND D4.DTYPDS='L')").
                    'Append("LEFT JOIN WEBSTGDSC AS D5 ON (D5.CONODS=W.CONOMS AND D5.STYGDS=W.STYGMS AND D5.DTYPDS='M')").
                    'Append("LEFT JOIN WEBSTGDSC AS D6 ON (D6.CONODS=W.CONOMS AND D6.STYGDS=W.STYGMS AND D6.DTYPDS='T')").
                    'Append("LEFT JOIN WEBITMIMG AS I ON (I.CONOIM=W.CONOMS AND I.PNUMIM=W.FIMGMS AND I.ITYPIM='I')")

                    Return strSQL.SQL
                End Get
            End Property

            Public ReadOnly Property HasVideo As Boolean
                Get
                    Return Me.VideoFlag.Equals("Y")
                End Get
            End Property

            Public ReadOnly Property NiceUrl As String
                Get
                    If String.IsNullOrEmpty(Me.ID) Then
                        Return "#"
                    End If
                    Return String.Format("/kits/kit/{0}.aspx", Me.ID.ToLower)
                End Get
            End Property

            Public ReadOnly Property Image As String
                Get
                    'Return String.Format("{0}", Me.ImageName)
                    'If String.IsNullOrEmpty(Me.ImagePath) Then
                    '    Return "#"
                    'End If
                    Return CleanImagePath(Me.ImagePath)
                End Get
            End Property

            Private _Images As List(Of String) = Nothing
            Public ReadOnly Property Images As List(Of String)
                Get
                    If _Images Is Nothing Then
                        '_Images = New List(Of String)
                        Dim strSQL = Sql.Builder.Append("SELECT TRIM(PATHIM) FROM WEBITMIMG3 WHERE CONOIM='CP' AND ITYPIM='L' AND STYGIM=@0 ORDER BY SQNOIM", Me.ID)
                        'For Each s In DB.Fetch(Of String)(strSQL)
                        '   _Images.Add(CleanImagePath(s))
                        'Next
                        _Images = DB.Fetch(Of String)(strSQL).Select(Function(x) CleanImagePath(x)).ToList()
                        '_Images.Add(Me.Image)
                    End If
                    Return _Images
                End Get
            End Property

            Public Shared Function All() As List(Of Kit)
                Dim strSQL = Sql.Builder.Append(Kit.FullSQL)


                'Return DB.Fetch(Of Product)(strSQL)

                'HttpContext.Current.Response.Clear()
                'HttpContext.Current.Response.Write(strSQL.SQL)
                'HttpContext.Current.Response.End()

                'Return (DB.Fetch(Of Product, Feature, Product)(AddressOf New FeatureRelator().MapIt, strSQL))
                Return DB.Fetch(Of Kit)(strSQL)
            End Function


            Public Shared Function GetByIDs(ids As String) As List(Of Kit)
                Return GetByIDs(ids.Split(","))
            End Function

            Public Shared Function GetByIDs(ids As String()) As List(Of Kit)
                If ids.Count = 0 Then Return New List(Of Kit)

                ids = ids.Select(Function(x) x.ToUpper).ToArray
                Dim strSQL = Sql.Builder.Append(Kit.FullSQL).Append("WHERE W.STYGMS IN (@ids)", New With {.ids = ids})
                'Dim ret = DB.Fetch(Of Product, Feature, Product)(AddressOf New FeatureRelator().MapIt, strSQL)
                'Return ret
                Return DB.Fetch(Of Kit)(strSQL)
            End Function

            Public Shared Function [Get](id As String) As Kit
                Dim cacheKey As String = "Kit." & id
                Dim ret = DirectCast(WSC.Utilities.Cache.Get(cacheKey), Kit)
                If ret Is Nothing Then
                    Dim strSQL = Sql.Builder.Append(Kit.FullSQL).Append("WHERE STYGMS=@0", id.ToUpper)
                    'ret = (DB.Fetch(Of Product, Feature, Product)(AddressOf New FeatureRelator().MapIt, strSQL)).FirstOrDefault()
                    ret = DB.FirstOrDefault(Of Kit)(strSQL)
                    WSC.Utilities.Cache.Set(cacheKey, ret, 1)
                End If
                Return ret
            End Function

            'Public Shared Function GetByCategory(category As String) As List(Of Product)
            '    Dim strSQL = Sql.Builder.Append("SELECT * FROM WEBSTGMACT").
            '                            Append("WHERE WCATMS=@0", category)
            '    Return DB.Fetch(Of Product)(strSQL)
            'End Function

            'Public Shared Function GetByAnimal(animalType As String) As List(Of Product)
            '    Dim strSQL = Sql.Builder.Append("SELECT * FROM WEBSTGMACT").
            '                            Append("WHERE WATYMS=@0", animalType)
            '    Return DB.Fetch(Of Product)(strSQL)
            'End Function

            Private Shared Function MergePrice(products As List(Of Product)) As List(Of Product)
                Return products
            End Function

            ' ''' <summary>
            ' ''' CPBRN – Brand Category Page
            ' ''' CPCAT – Cat Category Page
            ' ''' CPDOG – Dog Category Page
            ' ''' CPFIT – Featured Item Category Page
            ' ''' CPOTH – Other Category Page
            ' ''' </summary>
            ' ''' <param name="animalType">CPBRN,CPCAT,CPDOG,CPFIT,CPOTH</param>
            ' ''' <returns></returns>
            ' ''' <remarks></remarks>
            'Public Shared Function GetFeatured(animalType As String) As List(Of Product)
            '    'Dim strSQL = Sql.Builder.Append("SELECT TRIM(W.STYGMS) AS STYGMS, TRIM(W.PRNMMS) AS PRNMMS, TRIM(M.PATHIM) AS PATHIM").
            '    '                         Append("FROM WEBFTRITM AS F").
            '    '                         Append("LEFT JOIN WEBITMIMG AS M ON (M.CONOIM=F.CONOIT AND M.PNUMIM=F.PNUMIT AND M.ITYPIM = 'I')").
            '    '                         Append("LEFT JOIN ITEMS AS I ON (I.CONO35=F.CONOIT AND I.PNUM35=F.PNUMIT AND I.RUL31C='CP')").
            '    '                         Append("LEFT JOIN WEBSTGMACT AS W ON (W.STYGMS = I.STYGMS)").
            '    '                         Append("WHERE W.STYGMS IS NOT NULL AND F.FLOCIT=@0", animalType)
            '    Dim strSQL = Sql.Builder.Append("SELECT TRIM(S.STYGMS) AS STYGMS, TRIM(S.PRNMMS) AS PRNMMS, TRIM(I.PATHIM) AS PATHIM").
            '                             Append("FROM WEBFTRITM AS F").
            '                             Append("LEFT JOIN WEBSTGMACT AS S ON (S.CONOMS=F.CONOIT AND S.STYGMS=F.STYGIT)").
            '                             Append("LEFT JOIN WEBITMIMG AS I ON (I.CONOIM=S.CONOMS AND I.PNUMIM=S.FIMGMS AND I.ITYPIM='I')").
            '                             Append("WHERE S.STYGMS IS NOT NULL AND F.FLOCIT=@0", animalType)
            '    Return DB.Fetch(Of Product)(strSQL)
            'End Function

            'Class Feature
            '    <Column("FeatureID")> Public Property Name As String
            'End Class

            'Private Class FeatureRelator
            '    Public Current As Product
            '    Public Function MapIt(p As Product, pf As Feature) As Product
            '        If p Is Nothing Then Return Current
            '        If Current IsNot Nothing AndAlso Current.ID = p.ID Then
            '            Current.Features.Add(pf.Name)
            '            Return Nothing
            '        End If

            '        Dim prev = Current
            '        Current = p
            '        Current.Features = New List(Of String)
            '        Current.Features.Add(pf.Name)

            '        Return prev
            '    End Function
            'End Class

            'Public Class Component
            '    Public Property Sku As String
            '    Public Property Quantity As Integer
            'End Class

        End Class


        <TableName("WEBSTGMACT")>
        <Serializable()>
        Class Product
            <Column("STYGMS")> Public Property ID As String
            <Column("PRNMMS")> Public Property Name As String
            <Column("WATYMS")> Public Property AnimalType As String
            <Column("WCATMS")> Public Property Category As String
            <Column("PTYPMS")> Public Property ProductType As String
            <Column("MBRNMS")> Public Property BrandID As String
            <Column("MMATMS")> Public Property Material As String
            <Column("MKEYMS")> Public Property Keywords As String
            <Column("PATHIM")> Public Property ImagePath As String = String.Empty
            <Column("IALTMS")> Public Property AltTag As String
            <Column("VFLGMS")> Public Property VideoFlag As String
            <Column("CSVFMS")> Public Property SearchTerms As String
            <ResultColumn()> Public Property DescriptionLong As String = String.Empty
            <ResultColumn()> Public Property DescriptionShort As String = String.Empty
            <ResultColumn()> Public Property FeatureDescription As String = String.Empty
            <ResultColumn()> Public Property Application As String = String.Empty
            <ResultColumn()> Public Property MetaTitle As String = String.Empty
            <ResultColumn()> Public Property MetaDescription As String = String.Empty
            <ResultColumn()> Public Property BrandOrder As Decimal = 999


            '<ResultColumn()> Public Property FeatureID As String
            <ResultColumn()> Public Property Price As String = String.Empty

            Public Property Features As New List(Of String)
            Public ReadOnly Property FeaturesSearch As String
                Get
                    Return String.Join(" ", Me.Features.ToArray)
                End Get
            End Property

            Private _Related As List(Of Product)
            Public ReadOnly Property Related As List(Of Product)
                Get
                    If _Related Is Nothing Then
                        Dim strSQL = Sql.Builder.Append("SELECT RSTGRI").
                                                 Append("FROM WEBSTGRLT").
                                                 Append("WHERE CONORI='CP' AND STYGRI=@0", Me.ID).
                                                 Append("ORDER BY SEQNRI")
                        Dim ids = DB.Fetch(Of String)(strSQL).ToArray()
                        _Related = GetByIDs(ids)
                        If _Related Is Nothing Then
                            _Related = New List(Of Product)
                        End If
                    End If
                    Return _Related
                End Get
            End Property


            Private Shared ReadOnly Property TrimmedSQL As String
                Get
                    Return TrimSQLFields(Of Product)({"PATHIM", "BrandOrder"})
                End Get
            End Property

            Private Shared ReadOnly Property FullSQL As String
                Get
                    Dim strSQL = Sql.Builder.Append("SELECT " & TrimmedSQL & ", TRIM(I.PATHIM) AS PATHIM").
                                            Append(",D.NUM1C5 AS BrandOrder").
                                            Append(",TRIM(D1.PDESXD) AS DescriptionShort").
                                            Append(",TRIM(D2.PDESXD) AS DescriptionLong").
                                            Append(",TRIM(D3.PDESXD) AS FeatureDescription").
                                            Append(",TRIM(D4.PDESXD) AS Application").
                                            Append(",TRIM(D5.PDESXD) AS MetaDescription").
                                            Append(",TRIM(D6.PDESXD) AS MetaTitle").
                                            Append(",TRIM(F.SFTRTR) AS FeatureID").
                                            Append("FROM WEBSTGMACT AS W").
                                            Append("LEFT JOIN WEBSTGFTR AS F ON (W.CONOMS=F.CONOTR AND W.STYGMS=F.STYGTR)").
                                            Append("LEFT JOIN WEBITMIMG AS I ON (I.CONOIM=W.CONOMS AND I.PNUMIM=W.FIMGMS AND I.ITYPIM='I')").
                                            Append("LEFT JOIN WEBSTGEXD AS D1 ON (D1.CONOXD=W.CONOMS AND D1.STYGXD=W.STYGMS AND D1.DTYPXD='S')").
                                            Append("LEFT JOIN WEBSTGEXD AS D2 ON (D2.CONOXD=W.CONOMS AND D2.STYGXD=W.STYGMS AND D2.DTYPXD='L')").
                                            Append("LEFT JOIN WEBSTGEXD AS D3 ON (D3.CONOXD=W.CONOMS AND D3.STYGXD=W.STYGMS AND D3.DTYPXD='F')").
                                            Append("LEFT JOIN WEBSTGEXD AS D4 ON (D4.CONOXD=W.CONOMS AND D4.STYGXD=W.STYGMS AND D4.DTYPXD='A')").
                                            Append("LEFT JOIN WEBSTGEXD AS D5 ON (D5.CONOXD=W.CONOMS AND D5.STYGXD=W.STYGMS AND D5.DTYPXD='M')").
                                            Append("LEFT JOIN WEBSTGEXD AS D6 ON (D6.CONOXD=W.CONOMS AND D6.STYGXD=W.STYGMS AND D6.DTYPXD='T')").
                                            Append("LEFT JOIN COASTDESC AS D ON (W.MBRNMS=D.PSARC5 AND D.CONOC5='CP' AND D.PRMTC5='WEBMKTBRAND')")






                    'Append(", (SELECT TRIM(PDESXD) FROM WEBSTGEXD WHERE CONOXD=W.CONOMS AND STYGXD=W.STYGMS AND DTYPXD='S') AS SHORTDESCRIPTION").
                    'Append(", (SELECT TRIM(PDESXD) FROM WEBSTGEXD WHERE CONOXD=W.CONOMS AND STYGXD=W.STYGMS AND DTYPXD='L') AS LONGDESCRIPTION").

                    'Append("LEFT JOIN WEBSTGDSC AS D1 ON (D1.CONODS=W.CONOMS AND D1.STYGDS=W.STYGMS AND D1.DTYPDS='F'))").
                    'Append("LEFT JOIN WEBSTGDSC AS D2 ON (D2.CONODS=W.CONOMS AND D2.STYGDS=W.STYGMS AND D2.DTYPDS='A')").
                    'Append("LEFT JOIN WEBSTGDSC AS D3 ON (D3.CONODS=W.CONOMS AND D3.STYGDS=W.STYGMS AND D3.DTYPDS='S')").
                    'Append("LEFT JOIN WEBSTGDSC AS D4 ON (D4.CONODS=W.CONOMS AND D4.STYGDS=W.STYGMS AND D4.DTYPDS='L')").
                    'Append("LEFT JOIN WEBSTGDSC AS D5 ON (D5.CONODS=W.CONOMS AND D5.STYGDS=W.STYGMS AND D5.DTYPDS='M')").
                    'Append("LEFT JOIN WEBSTGDSC AS D6 ON (D6.CONODS=W.CONOMS AND D6.STYGDS=W.STYGMS AND D6.DTYPDS='T')").
                    'Append("LEFT JOIN WEBITMIMG AS I ON (I.CONOIM=W.CONOMS AND I.PNUMIM=W.FIMGMS AND I.ITYPIM='I')")

                    Return strSQL.SQL
                End Get
            End Property

            Public ReadOnly Property HasVideo As Boolean
                Get
                    Return Me.VideoFlag.Equals("Y")
                End Get
            End Property

            Public ReadOnly Property NiceUrl As String
                Get
                    If String.IsNullOrEmpty(Me.ID) Then
                        Return "#"
                    End If
                    Return String.Format("/products/product/{0}.aspx", Me.ID.ToLower)
                End Get
            End Property

            Public ReadOnly Property Image As String
                Get
                    'Return String.Format("{0}", Me.ImageName)
                    'If String.IsNullOrEmpty(Me.ImagePath) Then
                    '    Return "#"
                    'End If
                    Return CleanImagePath(Me.ImagePath)
                End Get
            End Property

            Private _Images As List(Of String) = Nothing
            Public ReadOnly Property Images As List(Of String)
                Get
                    If _Images Is Nothing Then
                        '_Images = New List(Of String)
                        Dim strSQL = Sql.Builder.Append("SELECT TRIM(PATHIM) FROM WEBITMIMG3 WHERE CONOIM='CP' AND ITYPIM='L' AND STYGIM=@0 ORDER BY SQNOIM", Me.ID)
                        'For Each s In DB.Fetch(Of String)(strSQL)
                        '   _Images.Add(CleanImagePath(s))
                        'Next
                        _Images = DB.Fetch(Of String)(strSQL).Select(Function(x) CleanImagePath(x)).ToList()
                        '_Images.Add(Me.Image)
                    End If
                    Return _Images
                End Get
            End Property


            Public Shared Function All() As List(Of Product)
                Dim strSQL = Sql.Builder.Append(Product.FullSQL)


                'Return DB.Fetch(Of Product)(strSQL)

                'HttpContext.Current.Response.Clear()
                'HttpContext.Current.Response.Write(strSQL.SQL)
                'HttpContext.Current.Response.End()

                Return (DB.Fetch(Of Product, Feature, Product)(AddressOf New FeatureRelator().MapIt, strSQL))

            End Function


            Public Shared Function GetByIDs(ids As String) As List(Of Product)
                Return GetByIDs(ids.Split(","))
            End Function

            Public Shared Function GetByIDs(ids As String()) As List(Of Product)
                If ids.Count = 0 Then Return New List(Of Product)

                ids = ids.Select(Function(x) x.ToUpper).ToArray
                Dim strSQL = Sql.Builder.Append(Product.FullSQL).Append("WHERE W.STYGMS IN (@ids)", New With {.ids = ids})
                Dim ret = DB.Fetch(Of Product, Feature, Product)(AddressOf New FeatureRelator().MapIt, strSQL)
                Return ret
            End Function

            Public Shared Function [Get](id As String) As Product
                Dim cacheKey As String = "Product." & id
                Dim ret = DirectCast(WSC.Utilities.Cache.Get(cacheKey), Product)
                If ret Is Nothing Then
                    Dim strSQL = Sql.Builder.Append(Product.FullSQL).Append("WHERE STYGMS=@0", id.ToUpper)
                    ret = (DB.Fetch(Of Product, Feature, Product)(AddressOf New FeatureRelator().MapIt, strSQL)).FirstOrDefault()
                    WSC.Utilities.Cache.Set(cacheKey, ret, 1)
                End If
                Return ret
            End Function

            'Public Shared Function GetByCategory(category As String) As List(Of Product)
            '    Dim strSQL = Sql.Builder.Append("SELECT * FROM WEBSTGMACT").
            '                            Append("WHERE WCATMS=@0", category)
            '    Return DB.Fetch(Of Product)(strSQL)
            'End Function

            'Public Shared Function GetByAnimal(animalType As String) As List(Of Product)
            '    Dim strSQL = Sql.Builder.Append("SELECT * FROM WEBSTGMACT").
            '                            Append("WHERE WATYMS=@0", animalType)
            '    Return DB.Fetch(Of Product)(strSQL)
            'End Function

            Private Shared Function MergePrice(products As List(Of Product)) As List(Of Product)
                Return products
            End Function

            ''' <summary>
            ''' CPBRN – Brand Category Page
            ''' CPCAT – Cat Category Page
            ''' CPDOG – Dog Category Page
            ''' CPFIT – Featured Item Category Page
            ''' CPOTH – Other Category Page
            ''' </summary>
            ''' <param name="animalType">CPBRN,CPCAT,CPDOG,CPFIT,CPOTH</param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Shared Function GetFeatured(animalType As String) As List(Of Product)
                'Dim strSQL = Sql.Builder.Append("SELECT TRIM(W.STYGMS) AS STYGMS, TRIM(W.PRNMMS) AS PRNMMS, TRIM(M.PATHIM) AS PATHIM").
                '                         Append("FROM WEBFTRITM AS F").
                '                         Append("LEFT JOIN WEBITMIMG AS M ON (M.CONOIM=F.CONOIT AND M.PNUMIM=F.PNUMIT AND M.ITYPIM = 'I')").
                '                         Append("LEFT JOIN ITEMS AS I ON (I.CONO35=F.CONOIT AND I.PNUM35=F.PNUMIT AND I.RUL31C='CP')").
                '                         Append("LEFT JOIN WEBSTGMACT AS W ON (W.STYGMS = I.STYGMS)").
                '                         Append("WHERE W.STYGMS IS NOT NULL AND F.FLOCIT=@0", animalType)
                Dim strSQL = Sql.Builder.Append("SELECT TRIM(S.STYGMS) AS STYGMS, TRIM(S.PRNMMS) AS PRNMMS, TRIM(I.PATHIM) AS PATHIM").
                                         Append("FROM WEBFTRITM AS F").
                                         Append("LEFT JOIN WEBSTGMACT AS S ON (S.CONOMS=F.CONOIT AND S.STYGMS=F.STYGIT)").
                                         Append("LEFT JOIN WEBITMIMG AS I ON (I.CONOIM=S.CONOMS AND I.PNUMIM=S.FIMGMS AND I.ITYPIM='I')").
                                         Append("WHERE S.STYGMS IS NOT NULL AND F.FLOCIT=@0", animalType)
                Return DB.Fetch(Of Product)(strSQL)
            End Function

            Class Feature
                <Column("FeatureID")> Public Property Name As String
            End Class

            Private Class FeatureRelator
                Public Current As Product
                Public Function MapIt(p As Product, pf As Feature) As Product
                    If p Is Nothing Then Return Current
                    If Current IsNot Nothing AndAlso Current.ID = p.ID Then
                        Current.Features.Add(pf.Name)
                        Return Nothing
                    End If

                    Dim prev = Current
                    Current = p
                    Current.Features = New List(Of String)
                    Current.Features.Add(pf.Name)

                    Return prev
                End Function
            End Class

        End Class

        <TableName("ITEMS")>
        <Serializable()>
        Class Item
            <Column("PDES35")> Public Property Name As String
            '<Column("PDES35")> Public Property Description As String
            <Column("IMAGEPATH")> Public Property ImagePath As String
            <Column("PNUM35")> Public Property Sku As String
            <Column("STYGMS")> Public Property StyleGroup As String
            <Column("STYLE", Trim:=True)> Public Property StyleID As String
            '<Column("STYLEIMAGE")> Public Property StyleImagePath As String
            <Column("ALTCLR")> Public Property ColorID As String
            <Column("COLORNAME")> Public Property ColorName As String
            <Column("COLORIMAGEPATH")> Public Property ColorImagePath As String
            <Column("SIZE")> Public Property SizeID As String
            <Column("SSIZE")> Public Property SizeName As String
            <Column("SMISC1")> Public Property SizeOrder As String
            <Column("UPCCODE")> Public Property UPCCode As String
            <Column("ARSB1C")> Public Property UPCSub As String
            '<Column("")> Public Property PriceLow As String
            '<Column("")> Public Property PriceHigh As String
            '<Column("")> Public Property Style As String
            <Column("TEXTID")> Public Property DisplayName As String
            Public Property Price As Decimal = 0
            <ResultColumn()> Public Property Quantity As Integer = 0




            Public ReadOnly Property Image As String
                Get
                    Return CleanImagePath(Me.ImagePath)
                End Get
            End Property

            Public ReadOnly Property ColorImage As String
                Get
                    Return CleanImagePath(Me.ColorImagePath)
                End Get
            End Property

            'Public ReadOnly Property Url As String
            '    Get
            '        Return ""
            '    End Get
            'End Property

            ReadOnly Property UPC As UPC
                Get
                    Return New UPC(Me.UPCCode, 300, 100)
                End Get
            End Property


            'Public Function GetImage(type As String) As String
            '    Dim path As String = String.Format("{0}//", type, Me.Style.PadRight(9, "_"))
            '    Return path
            'End Function

            'Public Shared Function All() As List(Of Item)
            '    'Dim strSQL = Sql.Builder.Append("")
            '    'Return DB.Fetch
            '    Return New List(Of Item)
            'End Function

            Private Shared Function BaseSQL() As Sql
                Return BaseSQL(String.Empty)
            End Function

            Private Shared Function BaseSQL(extraSelect As String) As Sql
                Dim trimmedSQL = TrimSQLFields(Of Item)({"PNUM35", "UPCCODE", "COLORIMAGEPATH", "IMAGEPATH", "COLORNAME", "TEXTID"})
                'Return Sql.Builder.Append("SELECT PNUM35," & trimmedSQL & ",TRIM(COMI1C)||TRIM(ARSB1C)||TRIM(CHDT1C) AS UPCCode, ").
                '                   Append("(SELECT TRIM(CHR5C5) FROM COASTDESC AS D1 WHERE D1.CONOC5=I.CONO35 AND D1.PSARC5=I.ALTCLR AND D1.PRMTC5='WEBCOLORSWATCH') AS COLORNAME,").
                '                   Append("(SELECT TRIM(CHR4C5) FROM COASTDESC AS D WHERE D.CONOC5=I.CONO35 AND D.PSARC5=I.COLOR AND D.PRMTC5='WEBCOLORSWATCH') AS COLORIMAGEPATH,").
                '                   Append("(SELECT TRIM(PATHIM) FROM WEBITMIMG WHERE CONOIM=I.CONO35 AND PNUMIM=I.PNUM35 AND ITYPIM='I') AS IMAGEPATH").
                '                   Append("FROM ITEMS AS I")
                Return Sql.Builder.Append("SELECT PNUM35," & trimmedSQL & ",TRIM(COMI1C)||TRIM(ARSB1C)||TRIM(CHDT1C) AS UPCCode, TRIM(CHR5C5) AS COLORNAME, TRIM(CHR4C5)  AS COLORIMAGEPATH").
                                   Append(", (SELECT TRIM(PATHIM) FROM WEBITMIMG WHERE CONOIM=I.CONO35 AND PNUMIM=I.PNUM35 AND ITYPIM='I') AS IMAGEPATH").
                                   Append(", WD.TEXTID").
                                   Append(extraSelect).
                                   Append("FROM ITEMS AS I").
                                   Append("LEFT JOIN COASTDESC AS D ON (D.CONOC5=I.CONO35 AND D.PSARC5=I.ALTCLR AND D.PRMTC5='WEBCOLORSWATCH')").
                                   Append("LEFT JOIN WEBITMDSC AS WD ON (WD.CONOID=I.CONO35 AND WD.PNUMID=I.STYLE AND WD.TYPEID='S')")
            End Function

            Public Shared Function GetByUPC(upc As String, c As Customer) As Item
                If upc.Length > 11 Then upc = upc.Substring(0, 11)
                Dim strSQL = BaseSQL.Append("WHERE CONCAT(TRIM(COMI1C),TRIM(ARSB1C))=@upc AND I.RUL31C=@UPCRule", New With {.upc = upc, .UPCRule = c.UPCRule})
                Return DB.FirstOrDefault(Of Item)(strSQL)
            End Function

            'Public Shared Function GetByUPC(upc As String) As Item
            '    If upc.Length > 11 Then upc = upc.Substring(0, 11)
            '    Dim strSQL = BaseSQL.Append("WHERE CONCAT(TRIM(COMI1C), TRIM(ARSB1C))=@0", upc)
            '    Return DB.FirstOrDefault(Of Item)(strSQL)
            'End Function

            Public Shared Function GetBySku(sku As String) As Item
                Dim strSQL = BaseSQL.Append("WHERE I.PNUM35=@0", sku)
                Return DB.FirstOrDefault(Of Item)(strSQL)
            End Function

            Public Shared Function GetBySkus(skus As List(Of String), c As Customer) As List(Of Item)
                Dim strSQL = BaseSQL.Append("WHERE I.PNUM35 IN @skus AND I.RUL31C=@UPCRule", New With {.skus = skus, .UPCRule = c.UPCRule})
                Return DB.Fetch(Of Item)(strSQL)
            End Function

            Public Shared Function GetByStyleGroup(styleGroup As String, c As Customer) As List(Of Item)
                Dim cacheKey As String = String.Format("Items.{0}.{1}", styleGroup, c.UPCRule)
                Dim ret = DirectCast(WSC.Utilities.Cache.Get(cacheKey), List(Of Item))
                If ret Is Nothing OrElse ret.Count = 0 Then
                    Dim strSQL = BaseSQL.Append("WHERE I.STYGMS = @0", styleGroup.ToUpper.PadRight(5, " ")).
                                         Append("AND I.RUL31C=@0", c.UPCRule).
                                         Append("ORDER BY I.STYLE, I.COLOR, I.SIZE")
                    ret = DB.Fetch(Of Item)(strSQL)
                    If ret.Count > 0 Then
                        WSC.Utilities.Cache.Set(cacheKey, ret, 1)
                    End If
                End If
                Return ret
            End Function

            Public Shared Function GetByStyle(style As String, c As Customer) As List(Of Item)
                'Dim cacheKey As String = String.Format("Items.{0}.{1}", style, c.UPCRule)
                'Dim ret = DirectCast(WSC.Utilities.Cache.Get(cacheKey), List(Of Item))
                Dim ret As List(Of Item) = Nothing
                If ret Is Nothing OrElse ret.Count = 0 Then
                    Dim strSQL = BaseSQL.Append("WHERE I.STYLE = @0", style.ToUpper.PadRight(9, " ")).
                                         Append("AND I.RUL31C=@0", c.UPCRule).
                                         Append("ORDER BY I.STYLE, I.COLOR, I.SIZE")
                    ret = DB.Fetch(Of Item)(strSQL)
                    If ret.Count > 0 Then
                        'WSC.Utilities.Cache.Set(cacheKey, ret, 1)
                    End If
                End If
                Return ret
            End Function

            Public Shared Function GetForKit(kitNumber As String, c As Customer) As List(Of Item)
                'Dim strSQL = Sql.Builder.Append("SELECT CMPT65, NOFF65")
                'strSQL.Append("FROM KITS")
                'strSQL.Append("WHERE CONO65='CP' AND PNUM65=@0", Me.ID)
                'Dim components = DB.Fetch(Of Component)(strSQL)

                '_Items = Item.GetBySkus(components.Select(Function(x) x.Sku).ToList, Customer.GetCurrent())
                ''--Add Quantity
                'For Each i In _Items
                '    Dim c = components.FirstOrDefault(Function(x) x.Sku = i.Sku)
                '    If c IsNot Nothing Then
                '        i.Quantity = c.Quantity
                '    End If
                'Next
                Dim strSQL = BaseSQL(",K.NOFF65 AS Quantity")
                strSQL.Append("INNER JOIN KITS AS K ON (K.CMPT65=I.PNUM35 AND K.CONO65='CP')")
                strSQL.Append("WHERE K.PNUM65=@0 AND I.RUL31C=@1 ", kitNumber, c.UPCRule)
                Return DB.Fetch(Of Item)(strSQL)
            End Function

            'Public Shared Function GetBySku(sku As String) As List(Of Item)
            '    Dim strSQL = Sql.Builder.Append("SELECT * FROM WEBFTRITM, ITEMS").
            '        Append("WHERE ITEMS.PNUM35 = WEBFTRITM.PNUMIT")

            '    If sku.Contains(",") Then
            '        For Each s In sku.Split(",")
            '            strSQL.Append("AND PNUM35=@0", s)
            '        Next
            '    Else
            '        strSQL.Append("AND PNUM35=@0", sku)
            '    End If

            '    Return DB.Fetch(Of Item)(strSQL)
            'End Function


            Shared Function VerifyUPCPrefix(upcRule As String, prefix As String) As Boolean
                Return (DB.Single(Of Integer)("SELECT COUNT(*) FROM ITEMS WHERE RUL31C=@0 AND TRIM(COMI1C)=@1", upcRule, prefix) > 0)
            End Function

            Shared Function VerifyUPC(upcRule As String, upc As String) As Boolean
                Return (DB.Single(Of Integer)("SELECT COUNT(*) FROM ITEMS WHERE RUL31C=@0 AND CONCAT(TRIM(COMI1C), TRIM(ARSB1C))=@1", upcRule, upc) > 0)
            End Function


        End Class




        <TableName("WEBSTGREV", journaled:=False)>
        Class Review
            <Column("CONORV")> Public Property CompanyCode As String = "CP"
            <Column("RVIDRV")> Public Property ID As Integer = -1
            <Column("TITLRV", Trim:=True)> Public Property Title As String
            <Column("AUTHRV", Trim:=True)> Public Property Author As String
            <Column("RATERV")> Public Property Rating As Integer
            <Column("RDESRV", Trim:=True)> Public Property Description As String
            <Column("TMSTRV")> Public Property CreateDate As DateTime
            <Column("STYGRV")> Public Property ProductCode As String
            <Column("EMALRV", Trim:=True)> Public Property Email As String
            <Column("STATRV")> Public Property Status As String = "W" '-- W - Waiting Approval, A - Approved, N - Not Approved

            Private Shared Function GetNextId() As Integer
                Return DB.FirstOrDefault(Of Integer)("Select RVIDRV FROM WEBSTGREV ORDER BY RVIDRV DESC FETCH FIRST 1 ROW ONLY") + 1
            End Function

            Sub Save()
                If Me.ID < 0 Then
                    Me.CreateDate = Now
                    Me.ID = GetNextId()
                    DB.Insert(Me)
                Else
                    DB.Update(Me)
                End If
            End Sub

            Public Shared Function [Get](id As Integer) As Review
                Return DB.FirstOrDefault(Of Review)("WHERE RVIDRV=@0", id)
                Return Nothing
            End Function

            Public Shared Function GetForProduct(productCode As String) As List(Of Review)
                Return DB.Fetch(Of Review)("WHERE STYGRV=@0 AND STATRV='A'", productCode)
                'Return New List(Of Review)
            End Function
        End Class



        <TableName("WEBBAWHDR", journaled:=False)>
        <PrimaryKey("PROJHD", autoIncrement:=False)>
        Class BuildAWall
            <Column("CONOHD")> Property CompanyCode As String = "CP"
            <Column("PROJHD")> Property ID As Decimal = -1
            <Column("PRJNHD", Trim:=True)> Property Name As String
            <Column("CUSNHD", Trim:=True)> Property AccountNumber As String = String.Empty
            <Column("TOTSHD")> Property SectionCount As Decimal = 0
            <Column("USERHD", Trim:=True)> Property UserID As String
            <Column("STATHD")> Property Status As String = String.Empty
            '<Column("CHANGEDATE")> Property ChangeDate As Date '--Combine DATEHD and TIMEHD
            '<Column("")> Property ChangeDate

            Private _Items As List(Of Item)
            <Ignore()>
            Public ReadOnly Property Items() As List(Of Item)
                Get
                    'Return _Items
                    If _Items Is Nothing AndAlso Me.ID > 0 Then
                        'Dim strSQL = Sql.Builder.Append("")
                        '_Items = DB.Fetch(Of Item)(strSQL)
                        _Items = Item.GetGorProject(Me.ID)
                    ElseIf Me.ID < 0 Then
                        _Items = New List(Of Item)
                    End If
                    Return _Items
                End Get
            End Property

            Sub Save()
                'Dim strSQL = String.Empty
                If Me.Items.Count > 0 Then
                    Me.SectionCount = Me.Items.Max(Function(x) x.SectionNumber)
                End If


                If Me.ID < 0 Then
                    Me.ID = GetNextId()
                    'strSQL = "INSERT INTO WEBBAWHDR (CONOHD,PROJHD,PRJNHD,CUSNHD,TOTSHD,USERHD,STATHD,TMSTHD) " &
                    '         "VALUES (@CompanyNumber,@ID,@Name,@AccountNumber,@SectionCount,@UserID,@Status, current timestamp) " &
                    '         "WITH NONE"
                    DB.Insert(Me)
                Else
                    'strSQL = "UPDATE WEBBAWHDR SET CONOHD=@CompanyNumber, PRJNHD=@Name, CUSNHD=@AccountNumber, TOTSHD=@SectionCount, USERHD=@UserID, STATHD=@Status, TMSTHD= current timestamp  WHERE PROJHD=@ID WITH NONE;"
                    DB.Update(Me)
                End If

                Item.DeleteForProject(Me.ID)

                For Each i In Me.Items.Distinct()
                    i.ProjectID = Me.ID
                    i.Save()
                Next
            End Sub

            Sub Delete()
                Item.DeleteForProject(Me.ID)
                DB.Execute("DELETE FROM WEBBAWHDR WHERE PROJHD=@0 WITH NONE", Me.ID)
            End Sub

            Sub AddItem(i As Item)
                i.ProjectID = Me.ID
                If Me.Items Is Nothing Then Me._Items = New List(Of Item)
                Dim existing = Me.Items.FirstOrDefault(Function(x) x.SectionNumber = i.SectionNumber AndAlso x.KitNumber.Equals(i.KitNumber))
                If existing IsNot Nothing Then
                    existing.X = i.X
                    existing.Y = i.Y
                Else
                    Me._Items.Add(i)
                End If
            End Sub

            Sub DeleteItem(i As Item)
                i.Delete()
                Me._Items.Remove(i)
            End Sub


            Private Shared Function GetNextId() As Integer
                Return DB.FirstOrDefault(Of Integer)("Select PROJHD FROM WEBBAWHDR ORDER BY PROJHD DESC FETCH FIRST 1 ROW ONLY") + 1
            End Function

            Shared Function [Get](id As Integer) As BuildAWall
                Return DB.FirstOrDefault(Of BuildAWall)("WHERE PROJHD=@0", id)
            End Function

            Shared Function GetForUser(userID As String) As List(Of BuildAWall)
                Return DB.Fetch(Of BuildAWall)("WHERE USERHD=@0", userID)
            End Function

            <TableName("WEBBAWDTL", journaled:=False)>
            <PrimaryKey("CONODT,PROJDT,SECTDT,KITNDT")>
            Public Class Item
                <Column("CONODT")> Property CompnayNumber As String = "CP"
                <Column("PROJDT")> Property ProjectID As Integer
                <Column("SECTDT")> Property SectionNumber As Integer
                <Column("KITNDT", Trim:=True)> Property KitNumber As String
                <Column("SQNODT")> Property SequenceNumber As Integer
                <Column("XCORDT")> Property X As Integer
                <Column("YCORDT")> Property Y As Integer

                Private _KitItem As KitItem
                <Ignore()>
                ReadOnly Property KitItem As KitItem
                    Get
                        If (_KitItem Is Nothing AndAlso Not String.IsNullOrEmpty(KitNumber)) Then
                            _KitItem = Datalayer.KitItem.Get(KitNumber)
                        End If
                        Return _KitItem
                    End Get
                End Property

                Sub Save()
                    DB.Insert(Me)
                End Sub

                Sub Delete()
                    'DB.Delete(Of Item)("WHERE PROJDT=@ProjectID AND SECTDT=@SectionNumber AND KITNDT=@KitNumber", Me)
                    DB.Execute("DELETE FROM WEBBAWDTL WHERE PROJDT=@ProjectID AND SECTDT=@SectionNumber AND KITNDT=@KitNumber WITH NONE", Me)
                End Sub

                Shared Sub DeleteForProject(projectID As Integer)
                    If projectID < 0 Then Exit Sub
                    'DB.Delete(Of Item)("WHERE PROJDT=@0", projectID)
                    DB.Execute("DELETE FROM WEBBAWDTL WHERE PROJDT=@0 WITH NONE", projectID)
                End Sub

                Shared Function GetGorProject(projectID As Integer) As List(Of Item)
                    If projectID < 0 Then Return Nothing
                    Return DB.Fetch(Of Item)("WHERE PROJDT=@0", projectID)
                End Function


            End Class
        End Class


        Class PortalPage
            Private Shared XMLPath As String = "/app_data/PortalPages.xml"

            Property Name As String
            Property Url As String
            Property ControlName As String
            'Property Rewrite As String
            Property Access As New List(Of String)
            Property IsHidden As Boolean = False
            Property Pages As New List(Of PortalPage)

            Sub New(n As System.Xml.XmlNode)
                If n Is Nothing Then Exit Sub

                Me.Name = n.Attributes("name").Value
                Me.Url = n.Attributes("url").Value
                'Me.Rewrite = n.Attributes("rewrite").Value
                Me.ControlName = n.Attributes("control").Value
                Me.Access.AddRange(n.Attributes("access").Value.Split(","))
                Dim hidden = n.Attributes("hidden")
                If hidden IsNot Nothing Then
                    Me.IsHidden = hidden.Value
                End If
            End Sub

            Function HasAccess(m As Member) As Boolean
                If Not m.HasPortalAccess Then Return False
                Return Me.Access.Contains(m.Level) AndAlso Not IsHidden()
            End Function

            Shared Function All() As List(Of PortalPage)
                Dim ret As List(Of PortalPage) = DirectCast(WSC.Utilities.Cache.Get(Of PortalPage)(), List(Of PortalPage))
                If ret Is Nothing Then
                    Dim doc = umbraco.Core.XmlHelper.OpenAsXmlDocument(XMLPath)
                    ret = New List(Of PortalPage)
                    For Each n As System.Xml.XmlNode In doc.DocumentElement.SelectNodes("page")
                        Dim p As New PortalPage(n)
                        Dim children = n.SelectNodes("page")
                        If children.Count > 0 Then
                            For Each cn In children
                                p.Pages.Add(New PortalPage(cn))
                            Next
                        End If
                        ret.Add(p)
                    Next
                    'HttpContext.Current.Cache.Insert(CacheKey, ret, New CacheDependency(umbraco.Core.IO.IOHelper.MapPath(XMLPath)))
                    WSC.Utilities.Cache.Set(Of PortalPage)(ret, XMLPath)
                End If
                Return ret
            End Function


            Shared Function GetByUrl(url As String) As PortalPage
                Dim pages = All()
                For Each p In pages
                    If p.Url = url Then Return p

                    For Each cp In p.Pages
                        If cp.Url = url Then Return cp
                    Next
                Next
                Return Nothing
            End Function

        End Class

        <TableName("WEBITMVID3")>
        Public Class Video
            <Column("CONOVI")> Public Property CompanyNumber As String = "CP"
            <Column("STYGVI")> Public Property StyleGroupCode As String
            <Column("STYLVI")> Public Property StyleCode As String
            <Column("PNUMVI")> Public Property ItemNumber As String
            <Column("VTYPVI")> Public Property VideoType As String = "L" '--L only option
            <Column("SQNOVI")> Public Property SequenceNumber As Decimal
            <Column("TITLVI")> Public Property Title As String
            <Column("STITVI")> Public Property SubTitle As String
            <Column("URLPVI")> Public Property Url As String


            Private _YouTube As WSC.DataType.YouTube.Data
            <Ignore()>
            Public ReadOnly Property YouTube As WSC.DataType.YouTube.Data
                Get
                    If _YouTube Is Nothing Then
                        _YouTube = New WSC.DataType.YouTube.Data(Me.Url)
                    End If
                    Return _YouTube
                End Get
            End Property

            'Public Shared Function GetForStyle(styleCode As String) As List(Of Video)
            '    Return DB.Fetch(Of Video)("WHERE STYLVI=@0 AND VTYPVI='L' AND URLPVI<>'' ORDER BY SQNOVI ASC", styleCode)
            'End Function

            'Public Shared Function GetForItem(itemNumber As String) As List(Of Video)
            '    Return DB.Fetch(Of Video)("WHERE PNUMVI=@0 AND VTYPVI='L' AND URLPVI<>'' ORDER BY SQNOVI ASC", itemNumber)
            'End Function

            Public Shared Function GetForProduct(id As String) As List(Of Video)
                Return DB.Fetch(Of Video)("WHERE STYGVI=@0 AND VTYPVI='L' AND URLPVI<>'' ORDER BY SQNOVI ASC", id)
            End Function


        End Class

        <TableName("WEBASCL01")>
        Public Class AccountServiceCoordinator
            <Column("CONOAS", trim:=True)> Public Property CompanyNumber As String
            <Column("EMPIAS", trim:=True)> Public Property EmployeeNumber As String
            <Column("FNAMAS", trim:=True)> Public Property FirstName As String
            <Column("LNAMAS", trim:=True)> Public Property LastName As String
            <Column("TERRAS", trim:=True)> Public Property TerritoryAdditionalDescription As String '--?
            <Column("PATHAS", trim:=True)> Public Property ImagePath As String
            <Column("INACAS", trim:=True)> Public Property InactiveFlag As String '==(' '=Active, I=Inactive, P=Pending)
            <Column("STERAS", trim:=True)> Public Property SalesTerritoryCode As String
            <Column("EMAILAS", trim:=True)> Public Property EmailAddress As String

            Public Shared Function All() As List(Of AccountServiceCoordinator)
                Return DB.Fetch(Of AccountServiceCoordinator)("WHERE CONOAS='CP'")
            End Function

            Public Shared Function GetForTerritory(salesTerritoryCode As String) As List(Of AccountServiceCoordinator)
                Return DB.Fetch(Of AccountServiceCoordinator)("WHERE CONOAS='CP' AND STERAS=@0", salesTerritoryCode)
            End Function
        End Class



    End Class

    Public Class Settings
        Public Shared Function [Get](key As String, defaultValue As String) As String
            Return [Get](Of String)(key, defaultValue)
        End Function
        Public Shared Function [Get](Of T)(key As String, defaultValue As T) As T
            Dim n = umbraco.uQuery.GetNodesByType("Settings").FirstOrDefault()
            If n Is Nothing Then Return defaultValue

            Return n.GetProperty(Of T)(key)
        End Function

    End Class




End Namespace
