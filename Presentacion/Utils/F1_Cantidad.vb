Imports DevComponents.DotNetBar.Controls
Imports DevComponents.DotNetBar
Imports Logica.AccesoLogica
Imports Janus.Windows.GridEX
Imports Newtonsoft.Json
Imports Presentacion.UmedidaResp
Imports Presentacion.NitValida
Imports System.IO
Imports System.Net


Public Class F1_Cantidad
    Public nit As String
    Public razonsocial As String
    Public email As String
    Public tipoDoc As Integer
    Public Cantidad As Decimal
    Public cliente As Integer
    Public bandera As Boolean
    Public Nuevo As Boolean = False


    Private Sub F_Cantidad_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        Dim token As String = F01_Producto.ObtToken()
        CodTipoDocumento(token)
        CbTipoDoc.SelectedIndex = 0

        Dim dt As DataTable = TraerUltimoNit(cliente)
        If dt.Rows.Count > 0 Then
            tbNit.Text = dt.Rows(0).Item("sanit").ToString
            tbEmail.Text = dt.Rows(0).Item("sanom2")
            tbRazonSocial.Text = dt.Rows(0).Item("sanom1")
            CbTipoDoc.Value = dt.Rows(0).Item("satipdoc")
        End If
        '_prCargarImpresoras(cbImpresora)
        'tbNit.Value = Cantidad
    End Sub

    'Private Sub _prCargarImpresoras(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
    '    Dim dt As DataTable

    '    dt = L_prCargarImpresoras()
    '    With mCombo
    '        .DropDownList.Columns.Clear()
    '        .DropDownList.Columns.Add("cbnumi").Width = 60
    '        .DropDownList.Columns("cbnumi").Caption = "COD"
    '        .DropDownList.Columns.Add("cbdesc").Width = 500
    '        .DropDownList.Columns("cbdesc").Caption = "IMPRESORA"
    '        .ValueMember = "cbnumi"
    '        .DisplayMember = "cbdesc"
    '        .DataSource = dt
    '        .Refresh()
    '    End With
    'End Sub
    Private Sub tbCantidad_Enter(sender As Object, e As EventArgs)

    End Sub




    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        bandera = False
        Me.Close()
    End Sub

    Private Sub ReflectionLabel1_Click(sender As Object, e As EventArgs) Handles ReflectionLabel1.Click

    End Sub

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint

    End Sub

    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click
        'Cantidad = tbNit.Value
        'Impresora = CbTipoDoc.Value
        Dim dt As DataTable = VerificarNit(tbNit.Text)
        If dt.Rows.Count > 0 Then
            Nuevo = False
        Else
            Nuevo = True
        End If
        If Nuevo = True Then
            Dim token As String = F01_Producto.ObtToken()
            crearCliente(token)
            RegistrarNit(tbNit.Text, tbRazonSocial.Text, tbEmail.Text, CbTipoDoc.Value)
        End If
        nit = tbNit.Text
        razonsocial = tbRazonSocial.Text
        email = tbEmail.Text
        tipoDoc = CbTipoDoc.Value
        bandera = True
        Me.Close()
    End Sub

    '----------------------------facturacion-----------------------

    Public Function CodTipoDocumento(tokenObtenido)

        Dim request = TryCast(System.Net.WebRequest.Create("https://contadores.sige.company/api/invoices/siat/v2/sync-documentos-identidad"), System.Net.HttpWebRequest)

        request.Method = "GET"

        request.ContentType = "application/json"
        request.Headers.Add("authorization", "Bearer " + tokenObtenido)

        request.ContentLength = 0
        Dim responseContent As String
        Using response = TryCast(request.GetResponse(), System.Net.HttpWebResponse)
            Using reader = New System.IO.StreamReader(response.GetResponseStream())
                responseContent = reader.ReadToEnd()
                Dim result = JsonConvert.DeserializeObject(Of Umedida)(responseContent)

                With CbTipoDoc
                    .DropDownList.Columns.Clear()
                    .DropDownList.Columns.Add("codigoClasificador").Width = 70
                    .DropDownList.Columns("codigoClasificador").Caption = "COD"
                    .DropDownList.Columns.Add("descripcion").Width = 500
                    .DropDownList.Columns("descripcion").Caption = "DESCRIPCION"
                    .ValueMember = "codigoClasificador"
                    .DisplayMember = "descripcion"
                    .DataSource = result.data.RespuestaListaParametricas.listaCodigos
                    .Refresh()
                End With
            End Using
        End Using





        'Dim Codigoconn As String
        'Codigoconn = result.meta.code
        'Dim json = JsonConvert.SerializeObject(result)
        'msgBox(json)
        Return ""
    End Function

    Private Sub tbNit_KeyDown(sender As Object, e As KeyEventArgs) Handles tbNit.KeyDown
        If e.KeyData = Keys.Enter Then
            If CbTipoDoc.Value = 5 Then
                Dim token As String = F01_Producto.ObtToken()
                If ValidarNit(token, tbNit.Text) = False Then
                    Dim dt As DataTable = VerificarNit(tbNit.Text)
                    If dt.Rows.Count > 0 Then
                        tbEmail.Text = dt.Rows(0).Item("sanom2")
                        tbNit.Text = dt.Rows(0).Item("sanit")
                        tbRazonSocial.Text = dt.Rows(0).Item("sanom1")
                        'CbTipoDoc.Value = dt.Rows(0).Item("satipdoc")
                    Else
                        Nuevo = True
                        tbEmail.Clear()
                        tbNit.Clear()
                        tbRazonSocial.Clear()

                        'crearCliente(token)

                    End If
                End If
            Else
                Dim dt As DataTable = VerificarNit(tbNit.Text)
                If dt.Rows.Count > 0 Then
                    tbEmail.Text = dt.Rows(0).Item("sanom2")
                    tbNit.Text = dt.Rows(0).Item("sanit")
                    tbRazonSocial.Text = dt.Rows(0).Item("sanom1")
                    'CbTipoDoc.Value = dt.Rows(0).Item("satipdoc")
                Else
                    Nuevo = True
                    Nuevo = True
                    tbEmail.Clear()
                    'tbNit.Clear()
                    tbRazonSocial.Clear()
                    'crearCliente(token)

                End If
            End If

        End If
    End Sub

    Private Sub crearCliente(Token As String)
        Dim request = TryCast(System.Net.WebRequest.Create("https://contadores.sige.company/api/customers"), System.Net.HttpWebRequest)

        request.Method = "POST"

        request.ContentType = "application/json"
        request.Headers.Add("authorization", "Bearer " + Token)

        Using writer As BinaryWriter = New BinaryWriter(request.GetRequestStream())
            Dim byteArray As Byte() = System.Text.Encoding.UTF8.GetBytes("{
              ""code"": """",
              ""group_id"": -1,
              ""store_id"": 0,
              ""first_name"": """ + tbRazonSocial.Text + """,
              ""last_name"": """",
              ""identity_document"": " + tbNit.Text + ",
              ""company"": """",
              ""date_of_birth"": null,
              ""gender"": """",
              ""phone"": """",
              ""mobile"": """",
              ""fax"": """",
              ""email"": """ + tbEmail.Text + """,
              ""website"": """",
              ""address_1"": ""Direccion 01"",
              ""address_2"": """",
              ""zip_code"": """",
              ""city"": """",
              ""country"": ""Bolivia"",
              ""country_code"": ""BO"",
              ""meta"": {
                ""_nit_ruc_nif"": """ + tbNit.Text + """,
                ""_billing_name"": null
              }
            }")
            'request.ContentLength = byteArray.Length
            writer.Write(byteArray)
            writer.Close()
        End Using
        Dim responseContent As String
        Using response = TryCast(request.GetResponse(), System.Net.HttpWebResponse)
            Using reader = New System.IO.StreamReader(response.GetResponseStream())
                responseContent = reader.ReadToEnd()
            End Using
        End Using
    End Sub

    Private Function ValidarNit(token As String, nit As String) As Boolean
        Try
            Dim request = TryCast(System.Net.WebRequest.Create("https://contadores.sige.company/api/invoices/siat/v2/validate-nit?nit=" + nit), System.Net.HttpWebRequest)

            request.Method = "GET"

            request.ContentType = "application/json"
            request.Headers.Add("authorization", "Bearer " + token)

            'request.ContentLength = 0
            Dim responseContent As String
            Using response = TryCast(request.GetResponse(), System.Net.HttpWebResponse)
                Using reader = New System.IO.StreamReader(response.GetResponseStream())
                    responseContent = reader.ReadToEnd()
                    Return True
                End Using
            End Using
        Catch ex As WebException
            If Not ex.Response Is Nothing Then
                Dim data As StreamReader = New StreamReader(ex.Response.GetResponseStream)
                'Al asignar el data.ReadToEnd al string se puede apreciar la respuesta del WebService en la variable str
                Dim str As String = data.ReadToEnd
                str = str.Replace("error", "message")
                Dim result = JsonConvert.DeserializeObject(Of Validar)(str)
                Dim message As String = result.message
                ToastNotification.Show(Me,
                                   message.ToUpper,
                                   My.Resources.WARNING,
                                   2000,
                                   eToastGlowColor.Red,
                                   eToastPosition.TopCenter)
                Return False
            End If


        End Try
    End Function

    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles ButtonX2.Click
        tbRazonSocial.Clear()
        tbNit.Clear()
        tbEmail.Clear()
        CbTipoDoc.SelectedIndex = 0
    End Sub
End Class