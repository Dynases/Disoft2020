
Imports Logica.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports GMap.NET.MapProviders
Imports GMap.NET
Imports GMap.NET.WindowsForms.Markers
Imports GMap.NET.WindowsForms
Imports GMap.NET.WindowsForms.ToolTips
Imports System.Drawing
Imports DevComponents.DotNetBar.Controls

Public Class F0_PreciosAlterno2
    Dim _Inter As Integer = 0

    Dim RutaGlobal As String = gs_CarpetaRaiz
#Region "Variables Globales"
    Dim precio As DataTable
    Public _nameButton As String
    Public _modulo As SideNavItem
    Public _tab As SuperTabItem
#End Region
#Region "MEtodos Privados"
    Private Sub _IniciarTodo()
        'TableLayoutPanel2.SetColumnSpan(grPolloCrudo, 2)
        'Me.WindowState = FormWindowState.Maximized

        cargarTablaPrecios1()
        cargarTablaPrecios2()
        cargarTablaPrecios3()

        _prAsignarPermisos()
        Me.Text = "PRECIOS"
        Dim blah As New Bitmap(New Bitmap(My.Resources.precio), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico


    End Sub

    Private Sub _prAsignarPermisos()

        'Dim dtRolUsu As DataTable = L_prRolDetalleGeneral(gi_userRol, _nameButton)

        'Dim show As Boolean = dtRolUsu.Rows(0).Item("ycshow")
        'Dim add As Boolean = dtRolUsu.Rows(0).Item("ycadd")
        'Dim modif As Boolean = dtRolUsu.Rows(0).Item("ycmod")
        'Dim del As Boolean = dtRolUsu.Rows(0).Item("ycdel")

        'If add = False Then
        '    btnNuevo.Visible = False
        'End If
        'If modif = False Then
        '    btnModificar.Visible = False
        'End If
        'If del = False Then
        '    btnEliminar.Visible = False
        'End If
    End Sub
    Public Sub _prCargarPrecios()
        precio = L_fnListarProductosConPrecios(0)
    End Sub

    Private Sub cargarTablaPrecios1()
        Dim dt As DataTable = TraerPrecioPolloEntero()

        grPrecioPolloEntero.BoundMode = Janus.Data.BoundMode.Bound
        grPrecioPolloEntero.DataSource = dt
        grPrecioPolloEntero.RetrieveStructure()
        With grPrecioPolloEntero.RootTable.Columns("canumi")
            .Caption = "CODIGO" & vbCr & ""
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grPrecioPolloEntero.RootTable.Columns("cadesc")
            .Caption = "PRODUCTO" & vbCr & ""
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grPrecioPolloEntero.RootTable.Columns("chprecio")
            .Caption = "DE 10 A 14"
            .MaxLines = 3
            .FormatString = "0.00"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
        End With
        With grPrecioPolloEntero.RootTable.Columns("precio15")
            .Caption = "DE 15 A 75"
            .FormatString = "0.00"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center

            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
        End With
        With grPrecioPolloEntero.RootTable.Columns("precio75")
            .Caption = "DE 76 A 150"
            .FormatString = "0.00"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
        End With
        With grPrecioPolloEntero.RootTable.Columns("precio150")
            .Caption = "DE 151 A MAS"
            .Width = 50
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .FormatString = "0.00"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
        End With
        With grPrecioPolloEntero.RootTable.Columns("preciocontado")
            .Caption = "CANTIDAD AL CONTADO"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Width = 65
            .HeaderStyle.BackColor = Color.Green
            .FormatString = "0.00"
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True

        End With
        With grPrecioPolloEntero.RootTable.Columns("preciovip")


            .Caption = "VIP"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Width = 65
            .HeaderStyle.BackColor = Color.Green
            .FormatString = "0.00"
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True

        End With


        With grPrecioPolloEntero

            .ColumnAutoResize = True
            .GroupByBoxVisible = False
            '.FilterRowFormatStyle.BackColor = Color.Blue
            '.DefaultFilterRowComparison = FilterConditionOperator.Contains
            '.FilterMode = FilterMode.Automatic
            '.FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            'Diseño de la tabla
            .VisualStyle = VisualStyle.Office2007
            .SelectionMode = SelectionMode.MultipleSelection
            .AlternatingColors = True
            .RecordNavigator = True

            .AllowColumnDrag = False


            .AllowEdit = False
        End With
    End Sub

    Private Sub cargarTablaPrecios2()
        Dim dt As DataTable = TraerPrecioPresas()

        grprecio.BoundMode = Janus.Data.BoundMode.Bound
        grprecio.DataSource = dt
        grprecio.RetrieveStructure()
        With grprecio.RootTable.Columns("canumi")
            .Caption = "CODIGO" & vbCr & ""
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grprecio.RootTable.Columns("cadesc")
            .Caption = "PRODUCTO" & vbCr & ""
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grprecio.RootTable.Columns("chprecio")
            .Caption = "PRECIO"
            .MaxLines = 3
            .FormatString = "0.00"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
        End With


        With grprecio

            .ColumnAutoResize = True
            .GroupByBoxVisible = False
            '.FilterRowFormatStyle.BackColor = Color.Blue
            '.DefaultFilterRowComparison = FilterConditionOperator.Contains
            '.FilterMode = FilterMode.Automatic
            '.FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            'Diseño de la tabla
            .VisualStyle = VisualStyle.Office2007
            .SelectionMode = SelectionMode.MultipleSelection
            .AlternatingColors = True
            .RecordNavigator = True

            .AllowColumnDrag = False


            .AllowEdit = False
        End With
    End Sub

    Private Sub cargarTablaPrecios3()
        Dim dt As DataTable = TraerPrecioPP()

        grPP.BoundMode = Janus.Data.BoundMode.Bound
        grPP.DataSource = dt
        grPP.RetrieveStructure()
        With grPP.RootTable.Columns("canumi")
            .Caption = "CODIGO" & vbCr & ""
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grPP.RootTable.Columns("cadesc")
            .Caption = "PRODUCTO" & vbCr & ""
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grPP.RootTable.Columns("chprecio")
            .Caption = "PRECIO"
            .MaxLines = 3
            .FormatString = "0.00"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
        End With


        With grPP

            .ColumnAutoResize = True
            .GroupByBoxVisible = False
            '.FilterRowFormatStyle.BackColor = Color.Blue
            '.DefaultFilterRowComparison = FilterConditionOperator.Contains
            '.FilterMode = FilterMode.Automatic
            '.FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            'Diseño de la tabla
            .VisualStyle = VisualStyle.Office2007
            .SelectionMode = SelectionMode.MultipleSelection
            .AlternatingColors = True
            .RecordNavigator = True

            .AllowColumnDrag = False


            .AllowEdit = False
        End With
    End Sub
    Public Sub _prCargarTablaPrecios(bandera As Boolean) ''Bandera = true si es que haiq cargar denuevo la tabla de Precio Bandera =false si solo cargar datos al Janus con el precio antepuesto

        Dim productos As DataTable
        grprecio.BoundMode = Janus.Data.BoundMode.Bound
            grprecio.DataSource = productos
            grprecio.RetrieveStructure()


        'a.yfcprod ,a.yfnumi ,a.yfcdprod1,gr3.ycdes3 as Laboratorio,gr4.ycdes3 as Presentacion 
        With grprecio.RootTable.Columns("yfcprod")
                .Caption = "COD"
                .Width = 60
                .Visible = True
            End With
            With grprecio.RootTable.Columns("yfnumi")
                .Caption = "COD PROD"
                .Width = 70
                .Visible = False
            End With
            With grprecio.RootTable.Columns("yfCodProd")
                .Caption = "COD PROD"
                .Width = 70
                .Visible = True
            End With
            With grprecio.RootTable.Columns("cacat")
                .Caption = "Cod P"
                .Width = 70
                .Visible = False
            End With
            With grprecio.RootTable.Columns("yfcdprod1")
                .Caption = "PRODUCTO"
                .Width = 260
                .Visible = True
            End With
            With grprecio.RootTable.Columns("Laboratorio")
                .Caption = "PROCEDENCIA"
                .Width = 150
                .Visible = False
            End With
            With grprecio.RootTable.Columns("Presentacion")
                .Caption = "Presentacion"
                .Width = 120
                .Visible = False
            End With
            'Habilitar Filtradores
            With grprecio
                .GroupByBoxVisible = False
                '.FilterRowFormatStyle.BackColor = Color.Blue
                .DefaultFilterRowComparison = FilterConditionOperator.Contains
                '.FilterMode = FilterMode.Automatic
                .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
                .FilterMode = FilterMode.Automatic
                'Diseño de la tabla
                .VisualStyle = VisualStyle.Office2007
                .SelectionMode = SelectionMode.SingleSelection
                .AlternatingColors = True
            End With

    End Sub

    Private Sub _prInhabiliitar()


        MBtModificar.Enabled = True
        MBtGrabar.Enabled = False
        ' _prCargarTablaPrecios(True)


    End Sub
    Private Sub _prhabilitar()

        MBtGrabar.Enabled = True
    End Sub




#End Region


#Region "MEtodoso Formulario"
    Private Sub F0_Precios_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _IniciarTodo()
        _prInhabiliitar()
        MBtModificar.PerformClick()
    End Sub
    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles MBtModificar.Click
        _prhabilitar()
        MBtModificar.Enabled = False

    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles MBtSalir.Click
        If (MBtModificar.Enabled = False) Then
            _prInhabiliitar()
        Else
            _modulo.Select()
            Me.Close()
        End If
    End Sub

    Private Sub TextBox_KeyDown(sender As Object, e As KeyEventArgs)
        Dim tb As TextBoxX = CType(sender, TextBoxX)
        If tb.Text = String.Empty Then

        Else
            tb.BackColor = Color.White
            MEP.SetError(tb, "")
        End If
    End Sub
    Private Sub grprecio_CellEdited(sender As Object, e As ColumnActionEventArgs)
        If (MBtNuevo.Enabled = False) Then
            'Habilitar solo las columnas de Precio, %, Monto y Observación
            If (e.Column.Index > 1) Then
                Dim data As String = grprecio.GetValue(e.Column.Index - 1).ToString.Trim 'En esta columna obtengo un protocolo que me indica el estado del precio 0= no insertado 1= ya insertado , a la ves con un '-' me indica la posicion de ese dato en el Datatable que envio para grabarlo que esta en 'precio' Ejemplo:1-15 -> estado=1 posicion=15
                Dim estado As String = data.Substring(0, 1).Trim
                Dim pos As String = data.Substring(2, data.Length - 2)
                If (estado = 1 Or estado = 2) Then
                    precio.Rows(pos).Item("estado") = 2
                    precio.Rows(pos).Item("yhprecio") = grprecio.GetValue(e.Column.Index)
                Else
                    If (estado = 0 Or estado = 3) Then
                        precio.Rows(pos).Item("estado") = 3
                        precio.Rows(pos).Item("yhprecio") = grprecio.GetValue(e.Column.Index)
                    End If
                End If


            End If

        End If
    End Sub

    Private Sub grprecio_EditingCell(sender As Object, e As EditingCellEventArgs)

        If MBtGrabar.Enabled = False Then
            e.Cancel = True
            Return
        End If
        If (MBtNuevo.Enabled = False And IsNothing(grprecio.DataSource) = False) Then
            'Deshabilitar la columna de Productos y solo habilitar la de los precios
            If (e.Column.Index = grprecio.RootTable.Columns("yfcdprod1").Index Or
                e.Column.Index = grprecio.RootTable.Columns("yfcprod").Index Or
                e.Column.Index = grprecio.RootTable.Columns("yfnumi").Index Or
                e.Column.Index = grprecio.RootTable.Columns("yfCodProd").Index) Then 'Or e.Column.Index = grprecio.RootTable.Columns("73").Index
                e.Cancel = True
            Else
                e.Cancel = False
            End If
        Else
            e.Cancel = True
        End If
    End Sub
    Private Sub btnGrabar_Click(sender As Object, e As EventArgs) Handles MBtGrabar.Click
        Dim fecha1 As String = Date.Now.ToString("dd/MM/yyyy")
        Dim hora1 As String = DateAndTime.Now.ToString("HH:mm")
        Dim grabar As Boolean = L_fnGrabarPrecios2(CType(grPP.DataSource, DataTable), CType(grPrecioPolloEntero.DataSource, DataTable), CType(grprecio.DataSource, DataTable), fecha1, hora1)
        If (grabar) Then
            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Precios Grabados con Exito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )


            cargarTablaPrecios1()
            cargarTablaPrecios2()
            cargarTablaPrecios3()
            _prInhabiliitar()

        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "Los precios no pudieron ser insertados".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
        End If

    End Sub


#End Region
    Private Sub _prCrearCarpetaReportes()

        Dim rutaDestino As String = RutaGlobal + "\Reporte\Reporte Precios\"

        If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte Precios\") = False Then
            If System.IO.Directory.Exists(RutaGlobal + "\Reporte") = False Then
                System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte")
                If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte Precios") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte\Reporte Precios")
                End If
            Else
                If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte Precios") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte\Reporte Precios")

                End If
            End If
        End If
    End Sub

    Private Sub grPP_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grPP.EditingCell
        If MBtModificar.Enabled Then
            e.Cancel = True
        Else
            If (e.Column.Index = grPP.RootTable.Columns("chprecio").Index) Then
                e.Cancel = False
            Else
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub grPrecioPolloEntero_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grPrecioPolloEntero.EditingCell
        If MBtModificar.Enabled Then
            e.Cancel = True
        Else
            If (e.Column.Index = grPrecioPolloEntero.RootTable.Columns("chprecio").Index) Or (e.Column.Index = grPrecioPolloEntero.RootTable.Columns("precio15").Index) Or
                                 (e.Column.Index = grPrecioPolloEntero.RootTable.Columns("precio75").Index) Or
                                 (e.Column.Index = grPrecioPolloEntero.RootTable.Columns("precio150").Index) Or (e.Column.Index = grPrecioPolloEntero.RootTable.Columns("preciocontado").Index) Or
                                 (e.Column.Index = grPrecioPolloEntero.RootTable.Columns("preciovip").Index) Then
                e.Cancel = False
            Else
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub grprecio_EditingCell_1(sender As Object, e As EditingCellEventArgs) Handles grprecio.EditingCell
        If MBtModificar.Enabled Then
            e.Cancel = True
        Else
            If (e.Column.Index = grprecio.RootTable.Columns("chprecio").Index) Then
                e.Cancel = False
            Else
                e.Cancel = True
            End If
        End If
    End Sub
End Class