Imports Logica.AccesoLogica
Imports DevComponents.Editors
Imports DevComponents.DotNetBar.SuperGrid
Imports System.IO
Imports System.Drawing.Printing
Imports DevComponents.DotNetBar
Imports DevComponents.DotNetBar.Controls
Imports Google.Apis.Auth.OAuth2


Public Class F03_Cabfac
#Region "Variables Globales"
    Dim _inter As Integer = 0
    Dim DtCabfac As DataTable
    Dim DtDetfac As DataTable
    Public _modulo As SideNavItem
    Public _nameButton As String
    Public _tab As SuperTabItem

#End Region
    Private Sub _PIniciarTodo()
        Me.Text = "G E N E R A R    A R C H I V O S   C S V"
        tbFecha.Value = Now.Date

    End Sub

    Private Sub BtnGenerar_Click(sender As Object, e As EventArgs) Handles BtnGenerar.Click
        Dim _ubicacion As String
        DtCabfac = L_prGenerarCabfac(tbFecha.Value)
        DtDetfac = L_prGenerarDetfac(tbFecha.Value)

        If DtCabfac.Rows.Count > 0 Then
            If (Not Directory.Exists(gs_CarpetaRaiz + "\CSV")) Then
                Directory.CreateDirectory(gs_CarpetaRaiz + "\CSV")
            End If

            _ubicacion = gs_CarpetaRaiz + "\CSV\"
            If (DataTableCSVFile(DtCabfac, "cabfac.csv", _ubicacion)) Then
                If (DataTableCSVFile(DtDetfac, "detfac.csv", _ubicacion)) Then
                    ToastNotification.Show(Me, "EXPORTACIÓN DE LOS ARCHIVOS EXITOSO EN LA UBICACION: " + _ubicacion,
                                           My.Resources.OK, 5000,
                                           eToastGlowColor.Green,
                                           eToastPosition.BottomCenter)
                End If

            Else
                ToastNotification.Show(Me, "FALLÓ LA EXPORTACIÓN DE LOS ARCHIVOS..!!!",
                                           My.Resources.WARNING, 4000,
                                           eToastGlowColor.Red,
                                           eToastPosition.BottomCenter)
            End If
        Else
            ToastNotification.Show(Me, "NO HAY REGISTROS PARA GENERAR...!!!",
                                           My.Resources.WARNING, 4000,
                                           eToastGlowColor.Red,
                                           eToastPosition.BottomCenter)
        End If

    End Sub

    Private Function DataTableCSVFile(ByVal dt As DataTable, ByVal sfilename As String, ByVal strFilePath As String) As Boolean
        Try
            Dim _stream As Stream
            Dim _archivo As String = strFilePath & sfilename
            File.Delete(_archivo)
            _stream = File.OpenWrite(_archivo)
            Dim sw As StreamWriter = New StreamWriter(_stream, System.Text.Encoding.UTF8)
            Dim iColCount As Integer = dt.Columns.Count

            'For i As Integer = 0 To iColCount - 1
            '    sw.Write(dt.Columns(i))

            '    If i < iColCount - 1 Then
            '        sw.Write(",")
            '    End If
            'Next

            'sw.Write(sw.NewLine)

            For Each dr As DataRow In dt.Rows

                For i As Integer = 0 To iColCount - 1

                    If Not Convert.IsDBNull(dr(i)) Then
                        sw.Write(dr(i).ToString())
                    End If

                    If i < iColCount - 1 Then
                        sw.Write(",")
                    End If
                Next

                sw.Write(sw.NewLine)
            Next

            sw.Close()

            'Try
            '    If (MessageBox.Show("DESEA ABRIR EL ARCHIVO?", "PREGUNTA", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes) Then
            '        Process.Start(_archivo)
            '    End If
            '    Return True
            'Catch ex As Exception
            '    MsgBox(ex.Message)
            '    Return False
            'End Try
            Return True
        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try
        Return False
    End Function

    Private Sub F03_Cabfac_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _PIniciarTodo()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        _inter = _inter + 1
        If _inter = 1 Then
            Me.WindowState = FormWindowState.Normal
        Else
            Me.Opacity = 100
            Timer1.Enabled = False
        End If
    End Sub

    Private Sub BtnSalir1_Click(sender As Object, e As EventArgs) Handles BtnSalir1.Click
        Me.Close()
    End Sub

End Class