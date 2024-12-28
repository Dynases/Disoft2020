Imports DevComponents.DotNetBar.Controls
Imports DevComponents.DotNetBar
Imports Logica.AccesoLogica
Imports Janus.Windows.GridEX
Imports Newtonsoft.Json
Imports Presentacion.UmedidaResp
Imports Presentacion.NitValida
Imports System.IO
Imports System.Net


Public Class F1_AgregarTarea
    Public nit As String
    Public razonsocial As String
    Public email As String
    Public tipoDoc As Integer
    Public Cantidad As Decimal
    Public cliente As Integer
    Public bandera As Boolean
    Public Nuevo As Boolean = False
    Public direccion As String
    Public observacion As String

    Private Sub F_Cantidad_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '_prCargarImpresoras(cbImpresora)
        'tbNit.Value = Cantidad
        tbHora.Value = DateAndTime.TimeOfDay
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

        nit = tbDesc.Text
        razonsocial = tbHora.Value.ToString("HH:mm")
        email = "00:00" 'tbHoraE.Text
        direccion = tbDirec.Text
        observacion = tbObse.Text
        bandera = True
        Me.Close()
    End Sub

    '----------------------------facturacion-----------------------



End Class