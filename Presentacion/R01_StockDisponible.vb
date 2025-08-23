Imports Logica.AccesoLogica
Imports DevComponents.DotNetBar
Imports DevComponents.DotNetBar.Controls
Imports Janus.Windows.GridEX
Imports System.IO

Public Class R01_StockDisponible
    Dim _inter As Integer = 0

    Dim RutaGlobal As String = gs_CarpetaRaiz
#Region "Variables Globales"

    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem

#End Region

#Region "Eventos"

    Private Sub My_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        P_prInicio()
    End Sub

    Private Sub MBtGenerar_Click(sender As Object, e As EventArgs) Handles MBtGenerar.Click
        P_prCargarReporte()
    End Sub

    Private Sub MBtSalir_Click(sender As Object, e As EventArgs) Handles MBtSalir.Click
        Me.Close()
        _modulo.Select()
        '_tab.Close()
    End Sub

#End Region

#Region "Metodos"

    Private Sub P_prInicio()
        'Abrir conexion
        If (Not gb_ConexionAbierta) Then
            L_prAbrirConexion()
        End If

        Me.Text = "S T O C K   D I S P O N I B L E".ToUpper
        'Me.WindowState = FormWindowState.Maximized
        MCrReporte.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
        _prCargarComboLibreriaDeposito(cbAlmacen)
        P_prArmarComboProveedor()
        If gs_MostrarSucursal = 1 Then
            lbDepositoOrigen.Visible = True
            cbAlmacen.Visible = True
        Else
            lbDepositoOrigen.Visible = False
            cbAlmacen.Visible = False
        End If
    End Sub

    Private Sub P_prArmarComboProveedor()
        Dim DtP As DataTable
        DtP = L_fnObtenerProveedor()
        DtP.Rows.Add(0, "TODOS")

        g_prArmarCombo(cbProveedor, DtP, 60, 200, "COD", "PROVEEDOR")
        cbProveedor.SelectedIndex = Convert.ToInt32(DtP.Rows.Count - 1)

    End Sub
    Private Sub _prCargarComboLibreriaDeposito(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_fnMovimientoListarSucursales()
        'dt.Rows.Add(-1, "Todos")
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("aanumi").Width = 60
            .DropDownList.Columns("aanumi").Caption = "COD"
            .DropDownList.Columns.Add("aabdes").Width = 500
            .DropDownList.Columns("aabdes").Caption = "SUCURSAL"
            .ValueMember = "aanumi"
            .DisplayMember = "aabdes"
            .DataSource = dt
            .Refresh()
        End With

        'mCombo.Value = 1
        If (dt.Rows.Count > 0) Then
            mCombo.SelectedIndex = 0
        End If
    End Sub

    Private Sub P_prCargarReporte()
        Dim _dt As New DataTable
        If swTipo.Value Then
            If rbUnidades.Checked Then
                Dim objrep As New R_StockDisponible()
                '_dt = L_VistaStockDisponible()
                _dt = L_fnStockDisponible(cbAlmacen.Value, cbProveedor.Value)
                objrep.SetDataSource(_dt)
                objrep.SetParameterValue("proveedor", cbProveedor.Text)
                MCrReporte.ReportSource = objrep

                grExcel.DataSource = _dt
                grExcel.RetrieveStructure()
                grExcel.AlternatingColors = True
                With grExcel.RootTable.Columns("canumi")
                    .Width = 90
                    .Visible = True
                    .Caption = "COD. PROD."
                End With
                With grExcel.RootTable.Columns("cadesc")
                    .Width = 90
                    .Visible = True
                    .Caption = "PRODUCTO"
                End With
                With grExcel.RootTable.Columns("categoria")
                    .Width = 90
                    .Visible = True
                    .Caption = "CATEGORIA"
                End With
                With grExcel.RootTable.Columns("cedesc")
                    .Width = 90
                    .Visible = False
                    .Caption = "UBICACION"
                End With
                With grExcel.RootTable.Columns("stockSinPedido")
                    .Width = 90
                    .Visible = True
                    .Caption = "STOCK"
                End With
                With grExcel.RootTable.Columns("stockSoloPedidos")
                    .Width = 90
                    .Visible = True
                    .Caption = "PEDIDOS"
                End With
                With grExcel.RootTable.Columns("stockFinal")
                    .Width = 90
                    .Visible = True
                    .Caption = "DISPONIBLE"
                End With
            Else
                Dim objrep1 As New R_StockDisponibleCajas()

                _dt = L_fnStockDisponibleCajas(cbAlmacen.Value, cbProveedor.Value)
                objrep1.SetDataSource(_dt)
                objrep1.SetParameterValue("proveedor", cbProveedor.Text)
                MCrReporte.ReportSource = objrep1

                grExcel.DataSource = _dt
                grExcel.RetrieveStructure()
                grExcel.AlternatingColors = True
                With grExcel.RootTable.Columns("canumi")
                    .Width = 90
                    .Visible = True
                    .Caption = "COD. PROD."
                End With
                With grExcel.RootTable.Columns("cadesc")
                    .Width = 90
                    .Visible = True
                    .Caption = "PRODUCTO"
                End With
                With grExcel.RootTable.Columns("categoria")
                    .Width = 90
                    .Visible = True
                    .Caption = "CATEGORIA"
                End With
                With grExcel.RootTable.Columns("cedesc")
                    .Width = 90
                    .Visible = False
                    .Caption = "UBICACION"
                End With
                With grExcel.RootTable.Columns("stockSinPedido")
                    .Width = 90
                    .Visible = True
                    .Caption = "STOCK CJ."
                End With
                With grExcel.RootTable.Columns("stockSinPedidoResiduo")
                    .Width = 90
                    .Visible = True
                    .Caption = "STOCK UN."
                End With
                With grExcel.RootTable.Columns("stockSoloPedidos")
                    .Width = 90
                    .Visible = True
                    .Caption = "PEDIDOS CJ."
                End With
                With grExcel.RootTable.Columns("stockSoloPedidosResiduo")
                    .Width = 90
                    .Visible = True
                    .Caption = "PEDIDOS UN."
                End With

                With grExcel.RootTable.Columns("stockFinal")
                    .Width = 90
                    .Visible = True
                    .Caption = "DISPONIBLE CJ."
                End With
                With grExcel.RootTable.Columns("stockFinalResiduo")
                    .Width = 90
                    .Visible = True
                    .Caption = "DISPONIBLE UN."
                End With
            End If

        Else
            Dim objrep As New R_StockDisponiblesSinAgrupacion()
            '_dt = L_VistaStockDisponible()
            _dt = L_fnStockDisponible(cbAlmacen.Value, cbProveedor.Value)
            objrep.SetDataSource(_dt)
            MCrReporte.ReportSource = objrep
        End If
    End Sub

#End Region
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        _inter = _inter + 1
        If _inter = 1 Then
            Me.WindowState = FormWindowState.Normal

        Else
            Me.Opacity = 100
            Timer1.Enabled = False
        End If
        'Me.Opacity = 100
        'Timer1.Enabled = False
    End Sub

    Private Sub btnExportar_Click(sender As Object, e As EventArgs) Handles btnExportar.Click
        _prCrearCarpetaReportes()
        Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
        If (P_ExportarExcel(RutaGlobal + "\Reporte\Reporte Productos")) Then
            ToastNotification.Show(Me, "EXPORTACIÓN DE LISTA DE PRODUCTOS EXITOSA..!!!",
                                       img, 2000,
                                       eToastGlowColor.Green,
                                       eToastPosition.BottomCenter)
        Else
            ToastNotification.Show(Me, "FALLO AL EXPORTACIÓN DE LISTA DE PRODUCTOS..!!!",
                                       My.Resources.WARNING, 2000,
                                       eToastGlowColor.Red,
                                       eToastPosition.BottomLeft)
        End If
    End Sub

    Public Function P_ExportarExcel(_ruta As String) As Boolean
        Dim _ubicacion As String
        'Dim _directorio As New FolderBrowserDialog

        If (1 = 1) Then 'If(_directorio.ShowDialog = Windows.Forms.DialogResult.OK) Then
            '_ubicacion = _directorio.SelectedPath
            _ubicacion = _ruta
            Try
                Dim _stream As Stream
                Dim _escritor As StreamWriter
                Dim _fila As Integer = grExcel.GetRows.Length
                Dim _columna As Integer = grExcel.RootTable.Columns.Count
                Dim _archivo As String = _ubicacion & "\ReporteProductoDetalle_" & Now.Date.Day &
                    "." & Now.Date.Month & "." & Now.Date.Year & "_" & Now.Hour & "." & Now.Minute & "." & Now.Second & ".csv"
                Dim _linea As String = ""
                Dim _linea1 As String = ";;"
                Dim _filadata = 0, columndata As Int32 = 0
                File.Delete(_archivo)
                _stream = File.OpenWrite(_archivo)
                _escritor = New StreamWriter(_stream, System.Text.Encoding.UTF8)

                For Each _col As GridEXColumn In grExcel.RootTable.Columns
                    If (_col.Visible) Then
                        _linea = _linea & _col.Caption & ";"
                    End If
                Next

                _linea1 = Mid(CStr(_linea1), 1, _linea1.Length - 1)
                _escritor.WriteLine(_linea1)
                _linea = Mid(CStr(_linea), 1, _linea.Length - 1)
                _escritor.WriteLine(_linea)
                _linea = Nothing

                'Pbx_Precios.Visible = True
                'Pbx_Precios.Minimum = 1
                'Pbx_Precios.Maximum = Dgv_Precios.RowCount
                'Pbx_Precios.Value = 1

                For Each _fil As GridEXRow In grExcel.GetRows
                    For Each _col As GridEXColumn In grExcel.RootTable.Columns
                        If (_col.Visible) Then
                            Dim data As String = CStr(_fil.Cells(_col.Key).Value)
                            data = data.Replace(vbLf, "")
                            data = data.Replace(vbCr, "")
                            data = data.Replace(";", ",")
                            _linea = _linea & data & ";"
                        End If
                    Next
                    _linea = Mid(CStr(_linea), 1, _linea.Length - 1)
                    _escritor.WriteLine(_linea)
                    _linea = Nothing
                    'Pbx_Precios.Value += 1
                Next
                _escritor.Close()
                'Pbx_Precios.Visible = False
                Try
                    Dim ef = New Efecto
                    ef._archivo = _archivo

                    ef.tipo = 1
                    ef.Context = "Su archivo ha sido Guardado en la ruta: " + _archivo + vbLf + "DESEA ABRIR EL ARCHIVO?"
                    ef.Header = "PREGUNTA"
                    ef.ShowDialog()
                    Dim bandera As Boolean = False
                    bandera = ef.band
                    If (bandera = True) Then
                        Process.Start(_archivo)
                    End If

                    'If (MessageBox.Show("Su archivo ha sido Guardado en la ruta: " + _archivo + vbLf + "DESEA ABRIR EL ARCHIVO?", "PREGUNTA", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes) Then
                    '    Process.Start(_archivo)
                    'End If
                    Return True
                Catch ex As Exception
                    MsgBox(ex.Message)
                    Return False
                End Try
            Catch ex As Exception
                MsgBox(ex.Message)
                Return False
            End Try
        End If
        Return False
    End Function

    Private Sub _prCrearCarpetaReportes()
        Dim rutaDestino As String = RutaGlobal + "\Reporte\Reporte Productos\"

        If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte Productos\") = False Then
            If System.IO.Directory.Exists(RutaGlobal + "\Reporte") = False Then
                System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte")
                If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte Productos") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte\Reporte Productos")
                End If
            Else
                If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte Productos") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte\Reporte Productos")

                End If
            End If
        End If
    End Sub

    Private Sub grExcel_FormattingRow(sender As Object, e As RowLoadEventArgs) Handles grExcel.FormattingRow

    End Sub
End Class