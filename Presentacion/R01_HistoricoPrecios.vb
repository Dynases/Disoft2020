Imports Logica.AccesoLogica
Imports DevComponents.DotNetBar
Imports DevComponents.DotNetBar.Controls

Public Class R01_HistoricoPrecios
    Dim _Inter As Integer = 0
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
        If (Not gb_ConexionAbierta) Then
            L_prAbrirConexion()
        End If

        Me.Text = "REPORTE VENTAS VENDEDOR"
        MCrReporte.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
        _IniciarComponentes()
    End Sub

    Public Sub _IniciarComponentes()


    End Sub


    Public Sub _prInterpretarDatos(ByRef _dt As DataTable)

        'titulo = "VENDEDOR:"
        'If (CheckTodosVendedor.Checked) Then
        '    _dt = L_prReporteVentasVendedorTodos(tbFechaI.Value.ToString("yyyy/MM/dd"), tbFechaF.Value.ToString("yyyy/MM/dd"), IIf(swTipo.Value = True, 3, 1), cbProveedor.Value)
        '    Return
        'End If
        'If (checkUnaVendedor.Checked) Then
        _dt = L_TraerHistrialPrecios(tbFechaI.Value.ToString("yyyy/MM/dd"), tbFechaF.Value.ToString("yyyy/MM/dd"))
        '    Return
        'End If

    End Sub
    Private Sub _prCargarReporte()
        Dim _dt As New DataTable
        _prInterpretarDatos(_dt)
        If (_dt.Rows.Count > 0) Then

            Dim objrep As New R_HistoricoPrecios
            objrep.SetDataSource(_dt)
            Dim fechaI As String = tbFechaI.Value.ToString("dd/MM/yyyy")
            Dim fechaF As String = tbFechaF.Value.ToString("dd/MM/yyyy")

            objrep.SetParameterValue("fechaI", fechaI)
            objrep.SetParameterValue("fechaF", fechaF)

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
        Me.Close()
        _modulo.Select()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        _Inter = _Inter + 1
        If _Inter = 1 Then
            Me.WindowState = FormWindowState.Normal

        Else
            Me.Opacity = 100
            Timer1.Enabled = False
        End If

    End Sub



    Private Sub tbVendedor_TextChanged(sender As Object, e As EventArgs)

    End Sub
End Class