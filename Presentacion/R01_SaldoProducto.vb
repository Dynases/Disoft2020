Imports Logica.AccesoLogica
Imports DevComponents.DotNetBar
Imports DevComponents.DotNetBar.Controls
Imports UTILITIES
Imports Janus.Windows.GridEX
Imports System.IO

Public Class R01_SaldoProducto
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
        _prCargarComboLibreriaDeposito(cbAlmacen)
        If gs_MostrarSucursal = 1 Then
            lbDepositoOrigen.Visible = True
            cbAlmacen.Visible = True
        Else
            lbDepositoOrigen.Visible = False
            cbAlmacen.Visible = False
        End If
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

        Me.Text = "S A L D O   D E   P R O D U C T O".ToUpper
        'Me.WindowState = FormWindowState.Maximized
        MCrReporte.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None

    End Sub

    Private Sub P_prCargarReporte()
        Dim _dt As New DataTable
        Dim saldoMayorCero As Boolean = chkSaldoMayorCero.Checked
        Dim condicionMayorCero As String = IIf(chkSaldoMayorCero.Checked, " AND iacant > 0 ", "")
        If swTipo.Value Then
            Dim objrep As New R_StockActual()
            If (cbAlmacen.Value >= 0) Then
                _dt = L_VistaStockActual("cenum=0 and iaalm=" + Str(cbAlmacen.Value) + " " + condicionMayorCero)
                objrep.SetDataSource(_dt)
                MCrReporte.ReportSource = objrep
            Else
                _dt = L_VistaStockActual("cenum=0" + " " + condicionMayorCero)
                objrep.SetDataSource(_dt)
                MCrReporte.ReportSource = objrep
            End If


        Else
            Dim objrep As New R_StockActualSinAgrupacion()

            If (cbAlmacen.Value >= 0) Then
                _dt = L_VistaStockActual("cenum=0 and iaalm=" + Str(cbAlmacen.Value) + " " + condicionMayorCero)
                objrep.SetDataSource(_dt)
                MCrReporte.ReportSource = objrep
            Else
                _dt = L_VistaStockActual("cenum=0" + " " + condicionMayorCero)
                objrep.SetDataSource(_dt)
                MCrReporte.ReportSource = objrep
            End If

        End If
        _dt.Columns.Add("Cajas", GetType(Integer))
        _dt.Columns.Add("Unidades", GetType(Integer))

        ' Recorrer las filas y calcular los valores
        For Each row As DataRow In _dt.Rows
            Dim stock As Integer = Convert.ToInt32(row("iacant"))
            Dim conversion As Integer = Convert.ToInt32(row("caconv"))

            ' Evitar división por cero
            If conversion > 0 Then
                row("Cajas") = stock \ conversion     ' División entera
                row("Unidades") = stock Mod conversion ' Residuo
            Else
                row("Cajas") = 0
                row("Unidades") = stock
            End If
        Next

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
        With grExcel.RootTable.Columns("caconv")
            .Width = 90
            .Visible = True
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
        With grExcel.RootTable.Columns("cadesc2")
            .Width = 90
            .Visible = False
            .Caption = "CATEGORIA"
        End With
        With grExcel.RootTable.Columns("Cajas")
            .Width = 90
            .Visible = True
            .Caption = "CAJAS"
        End With
        With grExcel.RootTable.Columns("Unidades")
            .Width = 90
            .Visible = True
            .Caption = "UNIDADES"
        End With

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
End Class