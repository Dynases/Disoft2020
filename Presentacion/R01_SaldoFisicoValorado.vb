Imports Logica.AccesoLogica
Imports DevComponents.DotNetBar
Imports DevComponents.DotNetBar.Controls
Imports Janus.Windows.GridEX
Imports System.IO

Public Class R01_SaldoFisicoValorado

#Region "Variables Globales"

    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem

    Dim RutaGlobal As String = gs_CarpetaRaiz

#End Region

#Region "Eventos"
    Dim _Inter As Integer = 0
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
        'Abrir conexion dsds
        If (Not gb_ConexionAbierta) Then
            L_prAbrirConexion()
        End If
        P_prArmarComboCatCliente()
        Me.Text = "S A L D O   F Í S I C O   V A L O R A D O".ToUpper
        'Me.WindowState = FormWindowState.Maximized
        MCrReporte.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
        _prCargarComboLibreriaDeposito(cbAlmacen)
        _prCargarComboProveedor(cbProveedor)
        If gs_MostrarSucursal = 1 Then
            lbDepositoOrigen.Visible = True
            cbAlmacen.Visible = True
        Else
            lbDepositoOrigen.Visible = False
            cbAlmacen.Visible = False
        End If
    End Sub
    Private Sub P_prArmarComboCatCliente()

        Dim Dt As New DataTable
        Dt = L_CategoriaPrecioGeneral()
        With cbCategoria.DropDownList
            .Columns.Add(Dt.Columns(0).ToString).Width = 50
            .Columns(0).Caption = "Código"

            .Columns.Add(Dt.Columns(1).ToString).Width = 70
            .Columns(1).Caption = "Abreviatura"

            .Columns.Add(Dt.Columns(2).ToString).Width = 120
            .Columns(2).Caption = "Descripción"
        End With

        cbCategoria.ValueMember = Dt.Columns(0).ToString
        cbCategoria.DisplayMember = Dt.Columns(2).ToString
        cbCategoria.DataSource = Dt
        cbCategoria.Refresh()
        cbCategoria.SelectedIndex = 0
    End Sub
    Private Sub P_prCargarReporte()
        Dim _dt As New DataTable
        If swTipo.Value Then
            Dim objrep As New R_SaldosFisicoValorado()
            If (cbAlmacen.Value <> -1 And cbProveedor.Value <> -1) Then
                _dt = L_VistaSaldoFisicoValorado("cenum=0 AND chcatcl=" + cbCategoria.Value.ToString() + " and iaalm=" + Str(cbAlmacen.Value) + " and cmnumi=" + Str(cbProveedor.Value))
                objrep.SetDataSource(_dt)
                objrep.SetParameterValue("TipoPrecio", cbCategoria.Text)
                MCrReporte.ReportSource = objrep
            Else
                If (cbAlmacen.Value >= 0) Then

                    _dt = L_VistaSaldoFisicoValorado("cenum=0 AND chcatcl=" + cbCategoria.Value.ToString() + " and iaalm=" + Str(cbAlmacen.Value))
                    objrep.SetDataSource(_dt)
                    objrep.SetParameterValue("TipoPrecio", cbCategoria.Text)
                    MCrReporte.ReportSource = objrep
                Else
                    _dt = L_VistaSaldoFisicoValorado("cenum=0 AND chcatcl=" + cbCategoria.Value.ToString())
                    objrep.SetDataSource(_dt)
                    objrep.SetParameterValue("TipoPrecio", cbCategoria.Text)
                    MCrReporte.ReportSource = objrep

                End If
            End If
        Else
            Dim objrep As New R_SaldosFisicoValoradoSinAgrupacion()
            If (cbAlmacen.Value <> -1 And cbProveedor.Value <> -1) Then
                _dt = L_VistaSaldoFisicoValorado("cenum=0 AND chcatcl=" + cbCategoria.Value.ToString() + " and iaalm=" + Str(cbAlmacen.Value) + " and cmnumi=" + Str(cbProveedor.Value))
                objrep.SetDataSource(_dt)
                objrep.SetParameterValue("TipoPrecio", cbCategoria.Text)
                MCrReporte.ReportSource = objrep
            Else
                If (cbAlmacen.Value >= 0) Then

                    _dt = L_VistaSaldoFisicoValorado("cenum=0 AND chcatcl=" + cbCategoria.Value.ToString() + " and iaalm=" + Str(cbAlmacen.Value))
                    objrep.SetDataSource(_dt)
                    objrep.SetParameterValue("TipoPrecio", cbCategoria.Text)
                    MCrReporte.ReportSource = objrep
                Else
                    _dt = L_VistaSaldoFisicoValorado("cenum=0 AND chcatcl=" + cbCategoria.Value.ToString())
                    objrep.SetDataSource(_dt)
                    objrep.SetParameterValue("TipoPrecio", cbCategoria.Text)
                    MCrReporte.ReportSource = objrep
                End If
            End If


        End If

        grExcel.DataSource = _dt
        grExcel.RetrieveStructure()
        grExcel.AlternatingColors = True
        With grExcel.RootTable.Columns("canumi")
            .Width = 90
            .Visible = True
            .Caption = "COD. PROD."
        End With
        With grExcel.RootTable.Columns("cacod")
            .Width = 90
            .Visible = True
            .Caption = "COD. FLEX."
        End With
        With grExcel.RootTable.Columns("cadesc")
            .Width = 90
            .Visible = True
            .Caption = "PRODUCTO"
        End With
        With grExcel.RootTable.Columns("cadesc2")
            .Width = 90
            .Visible = False
            .Caption = "PRODUCTO"
        End With
        With grExcel.RootTable.Columns("caest")
            .Width = 90
            .Visible = False
            .Caption = "COD. PROD."
        End With
        With grExcel.RootTable.Columns("iacprod")
            .Width = 90
            .Visible = False
            .Caption = "COD. PROD."
        End With
        With grExcel.RootTable.Columns("iacant")
            .Width = 90
            .Visible = True
            .Caption = "STOCK"
        End With
        With grExcel.RootTable.Columns("chprecio")
            .Width = 90
            .Visible = True
            .Caption = "PRECIO"
        End With
        With grExcel.RootTable.Columns("subtotal")
            .Width = 90
            .Visible = True
            .Caption = "TOTAL"
        End With
        With grExcel.RootTable.Columns("caconv")
            .Width = 90
            .Visible = False
            .Caption = "CONVERSION"
        End With
        With grExcel.RootTable.Columns("cenum")
            .Width = 90
            .Visible = False
            .Caption = "COD. PROD."
        End With
        With grExcel.RootTable.Columns("cedesc")
            .Width = 90
            .Visible = False
            .Caption = "CATEGORIA"
        End With
    End Sub

#End Region
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        _Inter = _Inter + 1
        If _Inter = 1 Then
            Me.WindowState = FormWindowState.Normal

        Else
            Me.Opacity = 100
            Timer1.Enabled = False
        End If
        'Me.Opacity = 100
        'Timer1.Enabled = False
    End Sub
    Private Sub _prCargarComboLibreriaDeposito(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_fnMovimientoListarSucursales()
        dt.Rows.Add(-1, "Todos")
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

        'mCombo.Value = -1
        If (dt.Rows.Count > 0) Then
            mCombo.SelectedIndex = 0
        End If
    End Sub

    Private Sub _prCargarComboProveedor(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_fnProveedores()
        dt.Rows.Add(-1, "Todos")
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("cmnumi").Width = 60
            .DropDownList.Columns("cmnumi").Caption = "COD"
            .DropDownList.Columns.Add("cmdesc").Width = 500
            .DropDownList.Columns("cmdesc").Caption = "PROVEEDOR"
            .ValueMember = "cmnumi"
            .DisplayMember = "cmdesc"
            .DataSource = dt
            .Refresh()
        End With

        'mCombo.Value = -1
        If (dt.Rows.Count > 0) Then
            mCombo.SelectedIndex = 0
        End If
    End Sub

    Private Sub LabelX1_Click(sender As Object, e As EventArgs) Handles LabelX1.Click

    End Sub

    Private Sub MultiColumnCombo1_ValueChanged(sender As Object, e As EventArgs) Handles cbProveedor.ValueChanged

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
End Class