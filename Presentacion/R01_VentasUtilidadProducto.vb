Imports Logica.AccesoLogica
Imports DevComponents.DotNetBar
Imports DevComponents.DotNetBar.Controls
Imports Janus.Windows.GridEX
Imports System.IO

Public Class R01_VentasUtilidadProducto
    Dim _inter As Integer = 0
    Dim RutaGlobal As String = gs_CarpetaRaiz
#Region "VARIABLES GLOBALES"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Dim titulo As String = ""
    Public _modulo As SideNavItem
#End Region
#Region "METODOS PRIVADOS"
    Public Sub _prIniciarTodo()
        tbFechaI.Value = Now.Date
        tbFechaF.Value = Now.Date
        'If (Not gb_ConexionAbierta) Then
        '    L_prAbrirConexion()
        'End If
        'Me.WindowState = FormWindowState.Maximized
        L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        Me.Text = "REPORTE PRODUCTOS EN VENTAS"
        MCrReporte.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None

        _prCargarComboProveedor(cbProv)
        _prCargarTipoCliente(cbTipC)

    End Sub

    Private Sub _prCargarTipoCliente(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_fnObtenerTiposdeCliente()
        dt.Rows.Add(-1, "Todos")

        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("cenum").Width = 60
            .DropDownList.Columns("cenum").Caption = "COD"
            .DropDownList.Columns.Add("cedesc").Width = 500
            .DropDownList.Columns("cedesc").Caption = "TIPO DE CLIENTE"
            .ValueMember = "cenum"
            .DisplayMember = "cedesc"
            .DataSource = dt
            .Refresh()
        End With

        'mCombo.Value = -1
        If (dt.Rows.Count > 0) Then
            mCombo.Value = -1
        End If
    End Sub
    Private Sub _prCargarComboProveedor(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_fnObtenerTabla("cmnumi, cmdesc, cmnit", "TC010", "cmest=1")
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
            mCombo.Value = -1
        End If
    End Sub
    Private Sub armarReporte(ByRef dt As DataTable)
        Dim cont As Integer = 0
        Dim mesI As Integer = Month(tbFechaI.Value.ToString("dd/MM/yyyy"))
        Dim mesF As Integer = Month(tbFechaF.Value.ToString("dd/MM/yyyy"))
        Dim anioI As Integer = Year(tbFechaI.Value.ToString("dd/MM/yyyy"))
        Dim anioF As Integer = Year(tbFechaF.Value.ToString("dd/MM/yyyy"))

        'For i = 0 To dt.Rows.Count - 1 Step 1
        For j = 0 To 3 Step 1
            dt = L_prReporteVentasDetalladaxMesProducto3(mesI + cont, anioI, tbFechaI.Value.ToString("dd/MM/yyyy"), tbFechaF.Value.ToString("dd/MM/yyyy"), cbProv.Value, dt, j)
            'Dim dtaux As DataTable = L_prReporteVentasDetalladaxMesProducto(dt.Rows(i).Item("ccnumi"), mesI + cont, anioI, tbFechaI.Value.ToString("dd/MM/yyyy"), tbFechaF.Value.ToString("dd/MM/yyyy"), cbProv.Value)
            'If dtaux.Rows.Count > 0 Then
            '    dt.Rows(i).Item(((j + 1) * 2) + j) = dtaux.Rows(0).Item("total")
            '    dt.Rows(i).Item((((j + 1) * 2) + j) + 1) = dtaux.Rows(0).Item("peso")
            '    dt.Rows(i).Item((((j + 1) * 2) + j) + 2) = dtaux.Rows(0).Item("utilidad")
            'End If
            cont += 1
            If mesI + cont > 12 Then
                mesI = 0
                anioI += 1
            End If
        Next
        ' cont = 0
        'Next
    End Sub

    Private Sub _prCargarReporte()
        Dim _dt As New DataTable
        _dt = L_prReporteVentasDetalladaProducto(tbFechaI.Value.ToString("yyyy/MM/dd"), tbFechaF.Value.ToString("yyyy/MM/dd"), cbProv.Value, cbTipC.Value)
        armarReporte(_dt)
        If (_dt.Rows.Count > 0) Then

            Dim objrep As New R_VentasDetallada12meses
            objrep.SetDataSource(_dt)
            Dim fechaI As String = tbFechaI.Value.ToString("dd/MM/yyyy")
            Dim fechaF As String = tbFechaF.Value.ToString("dd/MM/yyyy")
            objrep.SetParameterValue("usuario", L_Usuario)
            objrep.SetParameterValue("titulo", "REPORTE DE VENTAS POR PRODUCTOS POR MESES")
            objrep.SetParameterValue("fechaI", fechaI)
            objrep.SetParameterValue("fechaF", fechaF)
            objrep.SetParameterValue("mesI", Month(tbFechaI.Value.ToString("dd/MM/yyyy")))
            objrep.SetParameterValue("anioI", Year(tbFechaI.Value.ToString("dd/MM/yyyy")))

            MCrReporte.ReportSource = objrep
            MCrReporte.Show()
            MCrReporte.BringToFront()



        Else
            ToastNotification.Show(Me, "NO HAY DATOS PARA LOS PARAMETROS SELECCIONADOS..!!!",
                                       My.Resources.INFORMATION, 2000,
                                       eToastGlowColor.Blue,
                                       eToastPosition.BottomLeft)
            MCrReporte.ReportSource = Nothing
        End If





    End Sub

    Private Sub MBtGenerar_Click(sender As Object, e As EventArgs) Handles MBtGenerar.Click
        _prCargarReporte()
    End Sub

    Private Sub R01_VentasAtendidas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()

    End Sub
#End Region



    Private Sub MBtSalir_Click(sender As Object, e As EventArgs) Handles MBtSalir.Click
        '_tab.Close()
        _modulo.Select()
        Me.Close()
    End Sub
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

    Private Sub RellenarVacios(ByRef dt As DataTable)
        For i = 0 To dt.Columns.Count - 1 Step 1
            For j = 0 To dt.Rows.Count - 1 Step 1
                If IsDBNull(dt.Rows(j).Item(i)) Then
                    dt.Rows(j).Item(i) = 0
                End If
            Next
        Next
    End Sub
    Private Sub armarReporte2(ByRef dt As DataTable)
        Dim cont As Integer = 0
        Dim mesI As Integer = Month(tbFechaI.Value.ToString("dd/MM/yyyy"))
        Dim mesF As Integer = Month(tbFechaF.Value.ToString("dd/MM/yyyy"))
        Dim anioI As Integer = Year(tbFechaI.Value.ToString("dd/MM/yyyy"))
        Dim anioF As Integer = Year(tbFechaF.Value.ToString("dd/MM/yyyy"))
        Dim diferenciaMeses As Integer = DateDiff("m", tbFechaI.Value.ToString("dd/MM/yyyy"), tbFechaF.Value.ToString("dd/MM/yyyy"))

        For k = 0 To diferenciaMeses Step 1
            dt.Columns.Add("total" + k.ToString)
            dt.Columns.Add("peso" + k.ToString)
            dt.Columns.Add("utilidad" + k.ToString)
            dt.Columns.Add("cajas" + k.ToString)
            dt.Columns.Add("unidades" + k.ToString)
            dt.Columns.Add("cantidadTotal" + k.ToString)
            Dim dtaux As DataTable = L_prReporteVentasDetalladaxMesProducto2(mesI + cont, anioI, tbFechaI.Value.ToString("dd/MM/yyyy"), tbFechaF.Value.ToString("dd/MM/yyyy"), cbProv.Value)
            If dtaux.Rows.Count > 0 Then
                For j = 0 To dtaux.Rows.Count - 1 Step 1
                    For i = 0 To dt.Rows.Count - 1 Step 1

                        If dt.Rows(i).Item("ccnumi") = dtaux.Rows(j).Item("canumi") Then
                            dt.Rows(i).Item(2 + (k) * 6) = dtaux.Rows(j).Item("total")
                            dt.Rows(i).Item((2 + (k) * 6) + 1) = dtaux.Rows(j).Item("peso")
                            dt.Rows(i).Item((2 + (k) * 6) + 2) = dtaux.Rows(j).Item("utilidad")
                            dt.Rows(i).Item((2 + (k) * 6) + 3) = dtaux.Rows(j).Item("cajas")
                            dt.Rows(i).Item((2 + (k) * 6) + 4) = dtaux.Rows(j).Item("unidades")
                            dt.Rows(i).Item((2 + (k) * 6) + 5) = dtaux.Rows(j).Item("cantTotal")


                            Exit For
                        End If

                    Next
                Next

            End If
            cont += 1
            If mesI + cont > 12 Then
                mesI = 0
                anioI += 1
            End If



        Next


    End Sub
    Private Sub btnExportar_Click(sender As Object, e As EventArgs) Handles btnExportar.Click
        Dim _dt As New DataTable
        _dt = L_prReporteVentasDetalladaProducto2(tbFechaI.Value.ToString("yyyy/MM/dd"), tbFechaF.Value.ToString("yyyy/MM/dd"), cbProv.Value, cbTipC.Value)
        armarReporte2(_dt)
        RellenarVacios(_dt)
        grExcel.DataSource = _dt
        grExcel.RetrieveStructure()
        grExcel.AlternatingColors = True

        With grExcel.RootTable.Columns("ccnumi")
            .Width = 90
            .Visible = True
            .Caption = "COD. PROD."
        End With
        With grExcel.RootTable.Columns("ccdesc")
            .Width = 90
            .Visible = True
            .Caption = "PRODUCTO"
        End With
        Dim cont = 1
        For i = 2 To grExcel.RootTable.Columns.Count - 1 Step 1
            If cont = 1 Then
                With grExcel.RootTable.Columns(i)
                    .Width = 90
                    .Visible = True
                    .Caption = "TOTAL"
                End With
            ElseIf cont = 2 Then
                With grExcel.RootTable.Columns(i)
                    .Width = 90
                    .Visible = True
                    .Caption = "PESO"
                End With
            ElseIf cont = 3 Then
                With grExcel.RootTable.Columns(i)
                    .Width = 90
                    .Visible = True
                    .Caption = "UTLIDAD"
                End With
            ElseIf cont = 4 Then
                With grExcel.RootTable.Columns(i)
                    .Width = 90
                    .Visible = True
                    .Caption = "CAJAS"
                End With
            ElseIf cont = 5 Then
                With grExcel.RootTable.Columns(i)
                    .Width = 90
                    .Visible = True
                    .Caption = "UNIDADES"
                End With
            ElseIf cont = 6 Then
                With grExcel.RootTable.Columns(i)
                    .Width = 90
                    .Visible = True
                    .Caption = "CANT. TOTAL"
                End With

            End If
            If cont = 6 Then
                cont = 1
            Else
                cont = cont + 1
            End If
        Next


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
                Dim cont As Integer = 0
                Dim mesI As Integer = Month(tbFechaI.Value.ToString("dd/MM/yyyy"))
                Dim mesF As Integer = Month(tbFechaF.Value.ToString("dd/MM/yyyy"))
                Dim anioI As Integer = Year(tbFechaI.Value.ToString("dd/MM/yyyy"))
                Dim anioF As Integer = Year(tbFechaF.Value.ToString("dd/MM/yyyy"))

                Dim contador As Integer = grExcel.RootTable.Columns.Count - 2
                For i = 2 To grExcel.RootTable.Columns.Count - 1 Step 1
                    If ((i + 1) Mod 6) = 0 Then
                        Dim mesA As String = ObtenerMes(mesI + cont)
                        _linea1 = _linea1 & mesA & " " & anioI.ToString & ";"
                        If cont = 12 Then
                            mesI = 0
                            cont = 0
                            anioI = anioI + 1
                        Else
                            cont = cont + 1
                        End If

                    Else
                        _linea1 = _linea1 & "" & ";"
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

    Private Function ObtenerMes(mes As Integer) As String
        Dim ret As String = ""
        If mes = 1 Then
            ret = "Enero"
        ElseIf mes = 2 Then
            ret = "Febrero"
        ElseIf mes = 3 Then
            ret = "Marzo"
        ElseIf mes = 4 Then
            ret = "Abril"
        ElseIf mes = 5 Then
            ret = "Mayo"
        ElseIf mes = 6 Then
            ret = "Junio"
        ElseIf mes = 7 Then
            ret = "Julio"
        ElseIf mes = 8 Then
            ret = "Agosto"
        ElseIf mes = 9 Then
            ret = "Septiembre"
        ElseIf mes = 10 Then
            ret = "Octubre"
        ElseIf mes = 11 Then
            ret = "Noviembre"
        ElseIf mes = 12 Then
            ret = "Diciembre"
        End If
        Return ret
    End Function
End Class