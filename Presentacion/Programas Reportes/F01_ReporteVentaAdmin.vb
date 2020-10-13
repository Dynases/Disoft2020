Imports Logica.AccesoLogica
Imports DevComponents.DotNetBar
Imports DevComponents.DotNetBar.Controls

Imports Janus.Windows.GridEX
Imports System.IO
Public Class F01_ReporteVentaAdmin
    Private Sub P_prInicio()
        'Abrir conexion dsds
        If (Not gb_ConexionAbierta) Then
            L_prAbrirConexion()
        End If

        Me.WindowState = FormWindowState.Maximized
        tbFechaI.Value = Now.Date
        tbFechaF.Value = Now.Date
        P_prArmarCombos()
    End Sub
    Private Sub P_prArmarCombos()
        P_prArmarComboProveedor()
        P_prArmarComboCategoria()
        P_prCargarComboLibreria(cbMarca, 102)
        P_prCargarComboLibreria(cbAtributo, 103)
        P_prArmarComboDescripcion(cbDescripcion)
        P_prArmarComboClientes()
        P_prArmarComboVendedores()
        P_prArmarComboRepartidor()
        P_prArmarComboProducto()

    End Sub
    Private Sub P_prArmarComboProducto()
        Dim DtP As DataTable
        DtP = L_fnObtenerProductos()
        DtP.Rows.Add(0, "TODOS")

        g_prArmarCombo(cbProducto, DtP, 60, 200, "COD", "PRODUCTOS")
        cbProducto.SelectedIndex = Convert.ToInt32(DtP.Rows.Count - 1)

    End Sub
    Private Sub P_prArmarComboProveedor()
        Dim DtP As DataTable
        DtP = L_fnObtenerProveedor()
        DtP.Rows.Add(0, "TODOS")

        g_prArmarCombo(cbProveedor, DtP, 60, 200, "COD", "PROVEEDOR")
        cbProveedor.SelectedIndex = Convert.ToInt32(DtP.Rows.Count - 1)

    End Sub

    Private Sub P_prArmarComboClientes()
        Dim DtP As DataTable
        DtP = L_fnObtenerClientes()
        DtP.Rows.Add(0, "TODOS")

        g_prArmarCombo(cbClientes, DtP, 60, 200, "COD", "CLIENTES")
        cbClientes.SelectedIndex = Convert.ToInt32(DtP.Rows.Count - 1)

    End Sub
    Private Sub P_prArmarComboRepartidor()
        Dim DtP As DataTable
        DtP = L_fnObtenerPersonal(3)
        DtP.Rows.Add(0, "TODOS")

        g_prArmarCombo(cbRepartidor, DtP, 60, 200, "COD", "REPARTIDORES")
        cbRepartidor.SelectedIndex = Convert.ToInt32(DtP.Rows.Count - 1)

    End Sub
    Private Sub P_prArmarComboVendedores()
        Dim DtP As DataTable
        DtP = L_fnObtenerPersonal(1)
        DtP.Rows.Add(0, "TODOS")

        g_prArmarCombo(cbVendedor, DtP, 60, 200, "COD", "VENDEDORES")
        cbVendedor.SelectedIndex = Convert.ToInt32(DtP.Rows.Count - 1)

    End Sub

    Private Sub P_prArmarComboCategoria()
        Dim Dt As DataTable
        Dt = L_fnObtenerCategoria()
        Dt.Rows.Add(0, "TODOS")
        g_prArmarCombo(cbCategoria, Dt, 60, 200, "Código", "Categoría")
        cbCategoria.SelectedIndex = Convert.ToInt32(Dt.Rows.Count - 1)

    End Sub

    Private Sub P_prCargarComboLibreria(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo, cod1 As String)
        Dim dt As New DataTable
        dt = L_prLibreriaProductoGeneral(cod1)
        dt.Rows.Add(0, "TODOS")
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("cenum").Width = 70
            .DropDownList.Columns("cenum").Caption = "COD"
            .DropDownList.Columns.Add("cedesc").Width = 200
            .DropDownList.Columns("cedesc").Caption = "DESCRIPCION"
            .ValueMember = "cenum"
            .DisplayMember = "cedesc"
            .DataSource = dt
            .Refresh()
        End With
        mCombo.SelectedIndex = Convert.ToInt32(dt.Rows.Count - 1)
    End Sub
    Private Sub P_prArmarComboDescripcion(cbDescripcion As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim Dt As DataTable
        Dt = L_fnObtenerDescripcion()
        Dt.Rows.Add("TODOS")
        With cbDescripcion
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("cadesc2").Width = 200
            .DropDownList.Columns("cadesc2").Caption = "DESCRIPCION"
            .ValueMember = "cadesc2"
            .DisplayMember = "cadesc2"
            .DataSource = Dt
            .Refresh()
        End With
        cbDescripcion.SelectedIndex = Convert.ToInt32(Dt.Rows.Count - 1)
    End Sub
    Private Sub F01_ReporteVentaFact_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "Ventas Facturadas"
        tbFechaI.Value = Now.Date
        tbFechaF.Value = Now.Date
        P_prInicio()

    End Sub
    Public Sub ConstruirTabla(ByRef _dt As DataTable)
        Dim table As DataTable = _dt.Copy
        Dim ventaGeneral, ventaProveedor, ventaCategoria, ventaMarca, ventaAtributo, ventaProducto As DataTable
        table = _dt.Copy
        table.Rows.Clear()
        Dim fechaDesde As DateTime = tbFechaI.Value.ToString("yyyy/MM/dd")
        Dim fechaHasta As DateTime = tbFechaF.Value.ToString("yyyy/MM/dd")
        Dim tipoFila As Integer = 0
        ventaGeneral = L_fnReporteVentaGeneral(cbProveedor.Value, cbCategoria.Value, cbMarca.Value, cbAtributo.Value,
                                               cbDescripcion.Value, fechaDesde, fechaHasta)
        'SELECCION DE GRUPOS
        'GENERAL
        If ventaGeneral.Rows.Count > 0 Then
            insertarFila(table, 0, "", "TOTAL GENERAL", ventaGeneral.Rows(0).Item("caja"), ventaGeneral.Rows(0).Item("Importe"),
                         ventaGeneral.Rows(0).Item("cantidadCliente"), ventaGeneral.Rows(0).Item("Porcentaje"))

            'PROVEEDORES
            ventaProveedor = L_fnReporteVentaProveedor(cbProveedor.Value, cbCategoria.Value, cbMarca.Value, cbAtributo.Value,
                                                       cbDescripcion.Value, fechaDesde, fechaHasta)
            For Each fila As DataRow In ventaProveedor.Rows

                insertarFila(table, 1, "", fila("proveedor"), fila("caja"), fila("Importe"), fila("cantidadCliente"), fila("Porcentaje"))
                'CATEGORIA
                ventaCategoria = L_fnReporteVentaCategoria(fila("idProveedor"), 0, 0, 0, "TODOS", fechaDesde, fechaHasta)

                For Each categ As DataRow In ventaCategoria.Rows
                    insertarFila(table, 2, "", fila("categoria"), fila("caja"), fila("Importe"), fila("cantidadCliente"), fila("Porcentaje"))
                    'MARCA
                    ventaMarca = L_fnReporteVentaMarca(categ("idProveedor"), categ("idCategoria"), 0, 0, "TODOS", fechaDesde, fechaHasta)

                    For Each marca As DataRow In ventaMarca.Rows
                        insertarFila(table, 3, "", fila("categoria"), fila("caja"), fila("Importe"), fila("cantidadCliente"), fila("Porcentaje"))
                        'ATRIBUTO
                        ventaAtributo = L_fnReporteVentaAtributo(marca("idProveedor"), marca("idCategoria"), marca("idmarca"), 0, "TODOS", fechaDesde, fechaHasta)

                        For Each atributo As DataRow In ventaAtributo.Rows
                            insertarFila(table, 4, "", fila("categoria"), fila("caja"), fila("Importe"), fila("cantidadCliente"), fila("Porcentaje"))
                            'ATRIBUTO
                            ventaProducto = L_fnReporteVentaProducto(atributo("idProveedor"), atributo("idCategoria"), atributo("idmarca"), atributo("idatributo"), "TODOS", fechaDesde, fechaHasta)
                            For Each producto As DataRow In ventaProducto.Rows
                                'PRODUCTO
                                insertarFila(table, 5, producto("IdProducto"), producto("categoria"), producto("caja"), producto("Importe"), producto("cantidadCliente"), producto("Porcentaje"))
                            Next
                        Next
                    Next
                Next
            Next
        End If

        _dt = table.Copy
    End Sub
    Public Sub insertarFila(ByRef _dt As DataTable, tipoFila As Integer, codigo As String, descripcion As String, caja As Decimal, importe As Decimal, numCliente As Decimal, porcentaje As Decimal)
        Select Case tipoFila
            Case 0
                descripcion = descripcion
            Case 1
                descripcion = "  " + descripcion
            Case 2
                descripcion = "    " + descripcion
            Case 3
                descripcion = "      " + descripcion
            Case 4
                descripcion = "        " + descripcion
            Case 5
                descripcion = "          " + descripcion
        End Select
        _dt.Rows.Add(codigo, descripcion, caja, importe, numCliente, porcentaje, tipoFila)
    End Sub
    Public Sub Filtrar(ByRef _dt As DataTable)
        Dim table As DataTable = _dt.Copy

        If (cbClientes.Value <> 0) Then
            table = _dt.Copy
            table.Rows.Clear()
            For i As Integer = 0 To _dt.Rows.Count - 1 Step 1
                If (_dt.Rows(i).Item("ClienteId") = cbClientes.Value) Then
                    table.ImportRow(_dt.Rows(i))

                End If

            Next
            _dt = table.Copy
        End If
        If (cbVendedor.Value <> 0) Then
            table = _dt.Copy
            table.Rows.Clear()
            For i As Integer = 0 To _dt.Rows.Count - 1 Step 1
                If (_dt.Rows(i).Item("CodigoVendedor") = cbVendedor.Value) Then
                    table.ImportRow(_dt.Rows(i))

                End If

            Next
            _dt = table.Copy
        End If
        If (cbRepartidor.Value <> 0) Then
            table = _dt.Copy
            table.Rows.Clear()
            For i As Integer = 0 To _dt.Rows.Count - 1 Step 1
                If (_dt.Rows(i).Item("RepartidorId") = cbRepartidor.Value) Then
                    table.ImportRow(_dt.Rows(i))

                End If

            Next
            _dt = table.Copy
        End If
        If (cbProducto.Value <> 0) Then
            table = _dt.Copy
            table.Rows.Clear()
            For i As Integer = 0 To _dt.Rows.Count - 1 Step 1
                If (_dt.Rows(i).Item("CodProducto") = cbProducto.Value) Then
                    table.ImportRow(_dt.Rows(i))

                End If

            Next
            _dt = table.Copy
        End If
    End Sub
    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        Dim dt As DataTable = L_fnReporteFacturados(cbProveedor.Value, cbCategoria.Value, cbMarca.Value, cbAtributo.Value, cbDescripcion.Value, tbFechaI.Value.ToString("yyyy/MM/dd"), tbFechaF.Value.ToString("yyyy/MM/dd"))
        ConstruirTabla(dt)
        Filtrar(dt)
        If (dt.Rows.Count > 0) Then
            grDatos.DataSource = dt
            grDatos.RetrieveStructure()
            grDatos.AlternatingColors = True

            With grDatos.RootTable.Columns("Codigo")
                .Caption = "Codigo"
                .Width = 120
                .FormatString = ""
                .Visible = True
            End With
            With grDatos.RootTable.Columns("Descripcion")
                .Caption = "Empresa/Categoria/Marca/Atributo/Producto"
                .FormatString = ""
                .Width = 250
                .Visible = True
            End With
            With grDatos.RootTable.Columns("Cajas")
                .Caption = "Cajas"
                .FormatString = "0.00"
                .Width = 120
                .Visible = True
            End With

            With grDatos.RootTable.Columns("Importe")
                .Caption = "Importe"
                .FormatString = "0.00"
                .Width = 120
                .Visible = True
            End With
            With grDatos.RootTable.Columns("NroClientes")
                .Caption = "NroClientes"
                .FormatString = "0.00"
                .Width = 120
                .Visible = True
            End With
            With grDatos.RootTable.Columns("Porcentaje")
                .Caption = "Porcentaje"
                .FormatString = "0.00"
                .Width = 120
                .Visible = True
            End With
            With grDatos
                .DefaultFilterRowComparison = FilterConditionOperator.Contains
                .FilterMode = FilterMode.Automatic
                .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
                .GroupByBoxVisible = False
                'diseño de la grilla
                .VisualStyle = VisualStyle.Office2007
            End With
        Else
            If (Not IsNothing(grDatos) And Not IsNothing(grDatos.DataSource)) Then

                CType(grDatos.DataSource, DataTable).Rows.Clear()

            End If

            ToastNotification.Show(Me, "NO HAY DATOS PARA LOS PARAMETROS SELECCIONADOS..!!!",
                                    My.Resources.INFORMATION, 2000,
                                    eToastGlowColor.Blue,
                                    eToastPosition.TopCenter)
        End If

    End Sub
End Class