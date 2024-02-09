Imports Logica.AccesoLogica
Imports Janus.Windows.GridEX
Imports Janus.Data
Imports DevComponents.DotNetBar
Imports DevComponents.DotNetBar.Controls


Public Class F01_Presupuesto
#Region "Variables globales locales"
    Dim Modificar As Boolean = False
    Dim row As Integer
#End Region
    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBox1.Enter

    End Sub

    Private Sub GroupPanelPersonal_Click(sender As Object, e As EventArgs) Handles GroupPanelPersonal.Click

    End Sub

    Private Sub CargarVendedores()
        Dim dt As DataTable = L_prVentasGraficaListarVendedores()

        grVendedor.BoundMode = Janus.Data.BoundMode.Bound
        grVendedor.DataSource = dt
        grVendedor.RetrieveStructure()

        'dar formato a las columnas
        With grVendedor.RootTable.Columns("estado")
            .Caption = "Check"
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = False
        End With
        With grVendedor.RootTable.Columns("cbnumi")
            .Caption = "Check"
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = False
        End With
        With grVendedor.RootTable.Columns("ydcod")
            .Caption = "Check"
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = False
        End With
        With grVendedor.RootTable.Columns("vendedor")
            .Caption = "VENDEDOR"
            .Width = 50

            .Visible = True
        End With

        'Habilitar Filtradores
        With grVendedor
            .GroupByBoxVisible = False
            .ColumnAutoResize = True
            '.FilterRowFormatStyle.BackColor = Color.Blue
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            '.FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            'Diseño de la tabla
            .VisualStyle = VisualStyle.Office2007
            .SelectionMode = SelectionMode.MultipleSelection
            .AlternatingColors = True
        End With
    End Sub

    Private Sub CargarComboMes(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = TListarMeses()
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("cenum").Width = 60
            .DropDownList.Columns("cenum").Caption = "COD"
            .DropDownList.Columns.Add("cedesc").Width = 200
            .DropDownList.Columns("cedesc").Caption = "MES"
            .ValueMember = "cenum"
            .DisplayMember = "cedesc"
            .DataSource = dt
            .Refresh()
        End With
    End Sub

    Private Sub CargarGrilla()
        Dim dt As DataTable = L_fnObtenerCategoria()
        dt.Columns.Add("Monto", Type.GetType("System.Double"))

        grdetalle.BoundMode = Janus.Data.BoundMode.Bound
        grdetalle.DataSource = dt
        grdetalle.RetrieveStructure()

        'dar formato a las columnas
        With grdetalle.RootTable.Columns("cod")
            .Caption = "Check"
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("desc")
            .Caption = "CATEGORIA"
            .Width = 50

            .Visible = True
        End With
        With grdetalle.RootTable.Columns("Monto")
            .Caption = "MONTO"
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
        End With


        'Habilitar Filtradores
        With grdetalle
            .GroupByBoxVisible = False
            .ColumnAutoResize = True
            '.FilterRowFormatStyle.BackColor = Color.Blue
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            '.FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            'Diseño de la tabla
            .VisualStyle = VisualStyle.Office2007
            .SelectionMode = SelectionMode.MultipleSelection
            .AlternatingColors = True
        End With
    End Sub
    Private Sub iniciarTodo()
        MSuperTabControlPrincipal.SelectedTabIndex = 0
        CargarComboMes(cbMes)
        CargarVendedores()
        'CargarGrilla()
        CargarPresupuestos()
        InHabilitar()
    End Sub

    Private Sub CargarPresupuestos()
        Dim dt As DataTable = CargarPresupuestoTodos()

        grPresupuesto.BoundMode = Janus.Data.BoundMode.Bound
        grPresupuesto.DataSource = dt
        grPresupuesto.RetrieveStructure()

        'dar formato a las columnas
        With grPresupuesto.RootTable.Columns("psnumi")
            .Caption = "CODIGO"
            .Width = 50
            '.CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = True
        End With
        With grPresupuesto.RootTable.Columns("psidven")
            .Caption = "CATEGORIA"
            .Width = 50

            .Visible = False
        End With
        With grPresupuesto.RootTable.Columns("cbdesc")
            .Caption = "VENDEDOR"
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
        End With
        With grPresupuesto.RootTable.Columns("psmes")
            .Caption = "MES"
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
        End With
        With grPresupuesto.RootTable.Columns("cedesc")
            .Caption = "MES"
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
        End With
        With grPresupuesto.RootTable.Columns("psaño")
            .Caption = "AÑO"
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
        End With

        'Habilitar Filtradores
        With grPresupuesto
            .GroupByBoxVisible = False
            .ColumnAutoResize = True
            '.FilterRowFormatStyle.BackColor = Color.Blue
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            '.FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            'Diseño de la tabla
            .VisualStyle = VisualStyle.Office2007
            .SelectionMode = SelectionMode.MultipleSelection
            .AlternatingColors = True

        End With
    End Sub
    Private Sub CargarDetallePresupuestos(cod As Integer)
        Dim dt As DataTable = CargarDetallePresupuesto(cod)

        grdetalle.BoundMode = Janus.Data.BoundMode.Bound
        grdetalle.DataSource = dt
        grdetalle.RetrieveStructure()

        'dar formato a las columnas
        With grdetalle.RootTable.Columns("pbnumi")
            .Caption = "CODIGO"
            .Width = 50
            '.CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("pbpsnumi")
            .Caption = "CATEGORIA"
            .Width = 50

            .Visible = False
        End With
        With grdetalle.RootTable.Columns("pbcat")
            .Caption = "VENDEDOR"
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("cadesc")
            .Caption = "CATEGORIA"
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grdetalle.RootTable.Columns("pbtot")
            .Caption = "Monto"
            .Width = 50
            .FormatString = "0.00"
            .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
        End With

        'Habilitar Filtradores
        With grdetalle
            .GroupByBoxVisible = False
            .ColumnAutoResize = True
            '.FilterRowFormatStyle.BackColor = Color.Blue
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            '.FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            'Diseño de la tabla
            .VisualStyle = VisualStyle.Office2007
            .SelectionMode = SelectionMode.MultipleSelection
            .AlternatingColors = True


            .TotalRow = Janus.Windows.GridEX.InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowPosition = TotalRowPosition.BottomFixed
        End With
    End Sub
    Private Sub F01_Presupuesto_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        iniciarTodo()
    End Sub

    Private Sub Habilitar()
        If Modificar = False Then
            cbMes.Enabled = True
            tbAño.ReadOnly = False
        End If
        MBtNuevo.Enabled = False
        MBtModificar.Enabled = False
        MBtGrabar.Enabled = True
    End Sub
    Private Sub InHabilitar()
        cbMes.Enabled = False
        tbAño.ReadOnly = True

        MBtNuevo.Enabled = True
        MBtModificar.Enabled = True
        MBtGrabar.Enabled = False
    End Sub
    Private Sub Limpiar()
        tbId.Clear()
        cbMes.Value = 1
        tbAño.Clear()
        CargarDetallePresupuestos(-1)
    End Sub
    Private Function ValidarCampos() As Boolean
        If cbMes.SelectedIndex < 0 Then
            Return False
        End If
        If tbAño.Text = String.Empty Then
            Return False
        End If
        Return True
    End Function
    Private Sub MBtGrabar_Click(sender As Object, e As EventArgs) Handles MBtGrabar.Click
        If ValidarCampos() = False Then
            Exit Sub
        End If
        If Modificar = False Then
            GuardarNuevo()
        Else
            GuardarModificado()
        End If

    End Sub

    Private Sub GuardarNuevo()
        Dim fecha As String = Date.Now.ToString("dd/MM/yyyy")
        Dim hora As String = DateAndTime.Now.ToString("HH:mm")

        Dim res As Boolean = GrabarPresupuesto(grVendedor.GetValue("cbnumi"), cbMes.Value, CInt(tbAño.Text), fecha, hora, CType(grdetalle.DataSource, DataTable))
        If res Then
            ToastNotification.Show(Me, "El registro " + tbId.Text + ", Se guardo correctamente",
                                  My.Resources.OK, 3000,
                                  eToastGlowColor.Blue, eToastPosition.TopCenter)

            InHabilitar()
            Limpiar()
            CargarPresupuestos()
        Else

            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "No se pudo realizar el registro ", img, 2000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If
    End Sub

    Private Sub GuardarModificado()

        Dim res As Boolean = ModificarPresupuesto(CInt(tbId.Text), CType(grdetalle.DataSource, DataTable))
        If res Then
            ToastNotification.Show(Me, "El registro " + tbId.Text + ", Se modifico correctamente",
                                  My.Resources.OK, 3000,
                                  eToastGlowColor.Blue, eToastPosition.TopCenter)

            InHabilitar()
            Limpiar()
            CargarPresupuestos()
        Else

            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "No se pudo modificar el registro ", img, 2000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If
    End Sub

    Private Sub grVendedor_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grVendedor.EditingCell
        e.Cancel = True
    End Sub

    Private Sub grVendedor_FormattingRow(e As RowLoadEventArgs)


        'e.Row.Cells("VENDEDOR").FormatStyle.BackColor = Color.Blue

    End Sub
    Private Sub grVendedor_SelectionChanged(sender As Object, e As EventArgs) Handles grVendedor.SelectionChanged
        If MBtGrabar.Enabled = True Then
            CargarDetallePresupuestos(-1)
        End If
    End Sub

    Private Sub MBtModificar_Click(sender As Object, e As EventArgs) Handles MBtModificar.Click
        Modificar = True
        Habilitar()
        'Limpiar()

    End Sub

    Private Sub MBtNuevo_Click(sender As Object, e As EventArgs) Handles MBtNuevo.Click
        Habilitar()
        Limpiar()
    End Sub

    Private Sub MBtSalir_Click(sender As Object, e As EventArgs) Handles MBtSalir.Click
        If MBtNuevo.Enabled = False Then
            InHabilitar()
            CargarPresupuestos()
        Else
            Me.Close()
        End If
    End Sub
    Private Sub MostrarRegistro(row As Integer)
        With grPresupuesto
            tbId.Text = .GetValue("psnumi")
            cbMes.Value = .GetValue("psmes")
            tbAño.Text = .GetValue("psaño")
            CargarDetallePresupuestos(.GetValue("psnumi"))
        End With


    End Sub
    Private Sub grPresupuesto_SelectionChanged(sender As Object, e As EventArgs) Handles grPresupuesto.SelectionChanged
        ' grPresupuesto.Row = row
        If (grPresupuesto.RowCount >= 0 And grPresupuesto.Row >= 0) Then
            MostrarRegistro(grPresupuesto.Row)
        End If
    End Sub

    Private Sub grPresupuesto_KeyDown(sender As Object, e As KeyEventArgs) Handles grPresupuesto.KeyDown

        If e.KeyData = Keys.Enter Then
            MSuperTabControlPrincipal.SelectedTabIndex = 0
            grdetalle.Focus()
            '    'row = grPresupuesto.Row

        End If

        'grPresupuesto.Row = row
    End Sub

    Private Sub grPresupuesto_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grPresupuesto.EditingCell
        e.Cancel = True
    End Sub

    Private Sub MBtImprimir_Click(sender As Object, e As EventArgs) Handles MBtImprimir.Click

        Dim dt As DataTable = CopiarDetallePresupuesto(grVendedor.GetValue("cbnumi"))
        If dt.Rows.Count > 0 Then
            grdetalle.BoundMode = Janus.Data.BoundMode.Bound
            grdetalle.DataSource = dt
            grdetalle.RetrieveStructure()

            'dar formato a las columnas
            With grdetalle.RootTable.Columns("pbnumi")
                .Caption = "CODIGO"
                .Width = 50
                '.CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
                .Visible = False
            End With
            With grdetalle.RootTable.Columns("pbpsnumi")
                .Caption = "CATEGORIA"
                .Width = 50

                .Visible = False
            End With
            With grdetalle.RootTable.Columns("pbcat")
                .Caption = "VENDEDOR"
                .Width = 50
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
            End With
            With grdetalle.RootTable.Columns("cadesc")
                .Caption = "CATEGORIA"
                .Width = 50
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = True
            End With
            With grdetalle.RootTable.Columns("pbtot")
                .Caption = "Monto"
                .Width = 50
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
            End With

            'Habilitar Filtradores
            With grdetalle
                .GroupByBoxVisible = False
                .ColumnAutoResize = True
                '.FilterRowFormatStyle.BackColor = Color.Blue
                .DefaultFilterRowComparison = FilterConditionOperator.Contains
                '.FilterMode = FilterMode.Automatic
                .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
                'Diseño de la tabla
                .VisualStyle = VisualStyle.Office2007
                .SelectionMode = SelectionMode.MultipleSelection
                .AlternatingColors = True
            End With
        End If
    End Sub
End Class