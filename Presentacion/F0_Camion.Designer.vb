<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class F0_Camion
    Inherits Modelo.ModeloF01_ca

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.gpDatos = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.tbCapacidad = New DevComponents.Editors.DoubleInput()
        Me.LabelX4 = New DevComponents.DotNetBar.LabelX()
        Me.swEstado = New DevComponents.DotNetBar.Controls.SwitchButton()
        Me.BtAdicionar = New DevComponents.DotNetBar.ButtonX()
        Me.LabelX11 = New DevComponents.DotNetBar.LabelX()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.pbImage = New System.Windows.Forms.PictureBox()
        Me.LabelX3 = New DevComponents.DotNetBar.LabelX()
        Me.LabelX1 = New DevComponents.DotNetBar.LabelX()
        Me.LabelX2 = New DevComponents.DotNetBar.LabelX()
        Me.tbcodigo = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.tbnombre = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.grBuscador = New Janus.Windows.GridEX.GridEX()
        Me.OfdProducto = New System.Windows.Forms.OpenFileDialog()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        CType(Me.MSuperTabControlPrincipal, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MSuperTabControlPrincipal.SuspendLayout()
        Me.MSuperTabControlPanelRegistro.SuspendLayout()
        Me.MPnSuperior.SuspendLayout()
        Me.MPnInferior.SuspendLayout()
        Me.MPanelToolBarUsuario.SuspendLayout()
        Me.MPanelToolBarNavegacion.SuspendLayout()
        Me.MPanelToolBarAccion.SuspendLayout()
        Me.MPanelToolBarImprimir.SuspendLayout()
        CType(Me.MBubbleBarUsuario, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MPnUsuario.SuspendLayout()
        CType(Me.MEP, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpDatos.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.tbCapacidad, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        CType(Me.pbImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupPanel1.SuspendLayout()
        CType(Me.grBuscador, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'MSuperTabControlPrincipal
        '
        '
        '
        '
        '
        '
        '
        Me.MSuperTabControlPrincipal.ControlBox.CloseBox.Name = ""
        '
        '
        '
        Me.MSuperTabControlPrincipal.ControlBox.MenuBox.Name = ""
        Me.MSuperTabControlPrincipal.ControlBox.Name = ""
        Me.MSuperTabControlPrincipal.ControlBox.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.MSuperTabControlPrincipal.ControlBox.MenuBox, Me.MSuperTabControlPrincipal.ControlBox.CloseBox})
        Me.MSuperTabControlPrincipal.Controls.SetChildIndex(Me.MSuperTabControlPanelRegistro, 0)
        '
        'MSuperTabControlPanelRegistro
        '
        Me.MSuperTabControlPanelRegistro.Controls.Add(Me.GroupPanel1)
        Me.MSuperTabControlPanelRegistro.Controls.Add(Me.gpDatos)
        Me.MSuperTabControlPanelRegistro.Margin = New System.Windows.Forms.Padding(2)
        Me.MSuperTabControlPanelRegistro.Size = New System.Drawing.Size(942, 455)
        Me.MSuperTabControlPanelRegistro.Controls.SetChildIndex(Me.MPnUsuario, 0)
        Me.MSuperTabControlPanelRegistro.Controls.SetChildIndex(Me.gpDatos, 0)
        Me.MSuperTabControlPanelRegistro.Controls.SetChildIndex(Me.GroupPanel1, 0)
        '
        'MPnSuperior
        '
        Me.MPnSuperior.Style.Alignment = System.Drawing.StringAlignment.Center
        Me.MPnSuperior.Style.BackColor1.Color = System.Drawing.Color.Gray
        Me.MPnSuperior.Style.BackColor2.Color = System.Drawing.SystemColors.ActiveCaption
        Me.MPnSuperior.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine
        Me.MPnSuperior.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.MPnSuperior.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.MPnSuperior.Style.GradientAngle = 90
        '
        'MPnInferior
        '
        Me.MPnInferior.Style.Alignment = System.Drawing.StringAlignment.Center
        Me.MPnInferior.Style.BackColor1.Color = System.Drawing.Color.Gray
        Me.MPnInferior.Style.BackColor2.Color = System.Drawing.SystemColors.ActiveCaption
        Me.MPnInferior.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine
        Me.MPnInferior.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.MPnInferior.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.MPnInferior.Style.GradientAngle = 90
        '
        'MTbUsuario
        '
        Me.MTbUsuario.Margin = New System.Windows.Forms.Padding(2)
        Me.MTbUsuario.ReadOnly = True
        Me.MTbUsuario.Size = New System.Drawing.Size(135, 23)
        Me.MTbUsuario.Text = "DEFAULT"
        '
        'MBtUltimo
        '
        Me.MBtUltimo.Margin = New System.Windows.Forms.Padding(2)
        '
        'MBtSiguiente
        '
        '
        'MBtAnterior
        '
        '
        'MBtPrimero
        '
        '
        'MBtSalir
        '
        '
        'MBtGrabar
        '
        '
        'MBtEliminar
        '
        Me.MBtEliminar.Visible = False
        '
        'MBtModificar
        '
        '
        'MBtNuevo
        '
        '
        'MBtImprimir
        '
        Me.MBtImprimir.Visible = False
        '
        'MBubbleBarUsuario
        '
        '
        '
        '
        Me.MBubbleBarUsuario.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.MBubbleBarUsuario.ButtonBackAreaStyle.BackColor = System.Drawing.Color.Transparent
        Me.MBubbleBarUsuario.ButtonBackAreaStyle.BorderBottomWidth = 1
        Me.MBubbleBarUsuario.ButtonBackAreaStyle.BorderColor = System.Drawing.Color.FromArgb(CType(CType(180, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer))
        Me.MBubbleBarUsuario.ButtonBackAreaStyle.BorderLeftWidth = 1
        Me.MBubbleBarUsuario.ButtonBackAreaStyle.BorderRightWidth = 1
        Me.MBubbleBarUsuario.ButtonBackAreaStyle.BorderTopWidth = 1
        Me.MBubbleBarUsuario.ButtonBackAreaStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.MBubbleBarUsuario.ButtonBackAreaStyle.PaddingBottom = 3
        Me.MBubbleBarUsuario.ButtonBackAreaStyle.PaddingLeft = 3
        Me.MBubbleBarUsuario.ButtonBackAreaStyle.PaddingRight = 3
        Me.MBubbleBarUsuario.ButtonBackAreaStyle.PaddingTop = 3
        Me.MBubbleBarUsuario.MouseOverTabColors.BorderColor = System.Drawing.SystemColors.Highlight
        Me.MBubbleBarUsuario.SelectedTabColors.BorderColor = System.Drawing.Color.Black
        '
        'MLbPaginacion
        '
        '
        '
        '
        Me.MLbPaginacion.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        'MRlAccion
        '
        '
        '
        '
        Me.MRlAccion.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        'gpDatos
        '
        Me.gpDatos.CanvasColor = System.Drawing.SystemColors.Control
        Me.gpDatos.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.gpDatos.Controls.Add(Me.Panel1)
        Me.gpDatos.DisabledBackColor = System.Drawing.Color.Empty
        Me.gpDatos.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpDatos.Font = New System.Drawing.Font("Georgia", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gpDatos.Location = New System.Drawing.Point(0, 0)
        Me.gpDatos.Margin = New System.Windows.Forms.Padding(2)
        Me.gpDatos.Name = "gpDatos"
        Me.gpDatos.Size = New System.Drawing.Size(942, 254)
        '
        '
        '
        Me.gpDatos.Style.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.gpDatos.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.gpDatos.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.gpDatos.TabIndex = 29
        Me.gpDatos.Text = "DATOS"
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.White
        Me.Panel1.Controls.Add(Me.tbCapacidad)
        Me.Panel1.Controls.Add(Me.LabelX4)
        Me.Panel1.Controls.Add(Me.swEstado)
        Me.Panel1.Controls.Add(Me.BtAdicionar)
        Me.Panel1.Controls.Add(Me.LabelX11)
        Me.Panel1.Controls.Add(Me.Panel2)
        Me.Panel1.Controls.Add(Me.LabelX3)
        Me.Panel1.Controls.Add(Me.LabelX1)
        Me.Panel1.Controls.Add(Me.LabelX2)
        Me.Panel1.Controls.Add(Me.tbcodigo)
        Me.Panel1.Controls.Add(Me.tbnombre)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(2)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(942, 236)
        Me.Panel1.TabIndex = 0
        '
        'tbCapacidad
        '
        '
        '
        '
        Me.tbCapacidad.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbCapacidad.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbCapacidad.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbCapacidad.Increment = 1.0R
        Me.tbCapacidad.Location = New System.Drawing.Point(117, 123)
        Me.tbCapacidad.Name = "tbCapacidad"
        Me.tbCapacidad.ShowUpDown = True
        Me.tbCapacidad.Size = New System.Drawing.Size(179, 23)
        Me.tbCapacidad.TabIndex = 226
        '
        'LabelX4
        '
        Me.LabelX4.AutoSize = True
        '
        '
        '
        Me.LabelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX4.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX4.Location = New System.Drawing.Point(8, 180)
        Me.LabelX4.Name = "LabelX4"
        Me.LabelX4.Size = New System.Drawing.Size(49, 16)
        Me.LabelX4.TabIndex = 225
        Me.LabelX4.Text = "Estado:"
        '
        'swEstado
        '
        '
        '
        '
        Me.swEstado.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.swEstado.Location = New System.Drawing.Point(118, 180)
        Me.swEstado.Name = "swEstado"
        Me.swEstado.OffText = "INACTIVO"
        Me.swEstado.OnText = "ACTIVO"
        Me.swEstado.Size = New System.Drawing.Size(177, 22)
        Me.swEstado.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.swEstado.TabIndex = 224
        Me.swEstado.Value = True
        Me.swEstado.ValueObject = "Y"
        '
        'BtAdicionar
        '
        Me.BtAdicionar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.BtAdicionar.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground
        Me.BtAdicionar.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtAdicionar.Image = Global.Presentacion.My.Resources.Resources.jpg
        Me.BtAdicionar.ImageFixedSize = New System.Drawing.Size(40, 40)
        Me.BtAdicionar.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.BtAdicionar.Location = New System.Drawing.Point(418, 46)
        Me.BtAdicionar.Name = "BtAdicionar"
        Me.BtAdicionar.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(4)
        Me.BtAdicionar.Size = New System.Drawing.Size(75, 61)
        Me.BtAdicionar.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.BtAdicionar.SubItemsExpandWidth = 10
        Me.BtAdicionar.TabIndex = 222
        Me.BtAdicionar.Text = "Adicionar"
        Me.BtAdicionar.TextColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        '
        'LabelX11
        '
        Me.LabelX11.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX11.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX11.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX11.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.LabelX11.Location = New System.Drawing.Point(418, 18)
        Me.LabelX11.Name = "LabelX11"
        Me.LabelX11.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX11.Size = New System.Drawing.Size(79, 23)
        Me.LabelX11.TabIndex = 223
        Me.LabelX11.Text = "Imagen:"
        '
        'Panel2
        '
        Me.Panel2.BackgroundImage = Global.Presentacion.My.Resources.Resources.img2
        Me.Panel2.Controls.Add(Me.pbImage)
        Me.Panel2.Location = New System.Drawing.Point(502, 18)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(2)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Padding = New System.Windows.Forms.Padding(4)
        Me.Panel2.Size = New System.Drawing.Size(225, 162)
        Me.Panel2.TabIndex = 8
        '
        'pbImage
        '
        Me.pbImage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbImage.Image = Global.Presentacion.My.Resources.Resources.I256x256_image_capture
        Me.pbImage.InitialImage = Global.Presentacion.My.Resources.Resources.pantalla1
        Me.pbImage.Location = New System.Drawing.Point(4, 4)
        Me.pbImage.Margin = New System.Windows.Forms.Padding(2)
        Me.pbImage.Name = "pbImage"
        Me.pbImage.Size = New System.Drawing.Size(217, 154)
        Me.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbImage.TabIndex = 0
        Me.pbImage.TabStop = False
        '
        'LabelX3
        '
        Me.LabelX3.AutoSize = True
        '
        '
        '
        Me.LabelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX3.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX3.Location = New System.Drawing.Point(8, 123)
        Me.LabelX3.Name = "LabelX3"
        Me.LabelX3.Size = New System.Drawing.Size(103, 16)
        Me.LabelX3.TabIndex = 6
        Me.LabelX3.Text = "Capacidad(Kg,):"
        '
        'LabelX1
        '
        Me.LabelX1.AutoSize = True
        '
        '
        '
        Me.LabelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX1.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX1.Location = New System.Drawing.Point(9, 27)
        Me.LabelX1.Name = "LabelX1"
        Me.LabelX1.Size = New System.Drawing.Size(50, 16)
        Me.LabelX1.TabIndex = 2
        Me.LabelX1.Text = "Código:"
        '
        'LabelX2
        '
        Me.LabelX2.AutoSize = True
        '
        '
        '
        Me.LabelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX2.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX2.Location = New System.Drawing.Point(8, 72)
        Me.LabelX2.Name = "LabelX2"
        Me.LabelX2.Size = New System.Drawing.Size(58, 16)
        Me.LabelX2.TabIndex = 4
        Me.LabelX2.Text = "Nombre:"
        '
        'tbcodigo
        '
        '
        '
        '
        Me.tbcodigo.Border.Class = "TextBoxBorder"
        Me.tbcodigo.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbcodigo.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbcodigo.Location = New System.Drawing.Point(119, 27)
        Me.tbcodigo.Name = "tbcodigo"
        Me.tbcodigo.PreventEnterBeep = True
        Me.tbcodigo.Size = New System.Drawing.Size(80, 23)
        Me.tbcodigo.TabIndex = 3
        '
        'tbnombre
        '
        '
        '
        '
        Me.tbnombre.Border.Class = "TextBoxBorder"
        Me.tbnombre.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbnombre.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbnombre.Location = New System.Drawing.Point(118, 72)
        Me.tbnombre.MaxLength = 50
        Me.tbnombre.Name = "tbnombre"
        Me.tbnombre.PreventEnterBeep = True
        Me.tbnombre.Size = New System.Drawing.Size(265, 23)
        Me.tbnombre.TabIndex = 5
        '
        'GroupPanel1
        '
        Me.GroupPanel1.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel1.Controls.Add(Me.grBuscador)
        Me.GroupPanel1.DisabledBackColor = System.Drawing.Color.Empty
        Me.GroupPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel1.Font = New System.Drawing.Font("Georgia", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupPanel1.Location = New System.Drawing.Point(0, 254)
        Me.GroupPanel1.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(942, 201)
        '
        '
        '
        Me.GroupPanel1.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2
        Me.GroupPanel1.Style.BackColorGradientAngle = 90
        Me.GroupPanel1.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground
        Me.GroupPanel1.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel1.Style.BorderBottomWidth = 1
        Me.GroupPanel1.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel1.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel1.Style.BorderLeftWidth = 1
        Me.GroupPanel1.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel1.Style.BorderRightWidth = 1
        Me.GroupPanel1.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel1.Style.BorderTopWidth = 1
        Me.GroupPanel1.Style.CornerDiameter = 4
        Me.GroupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel1.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.GroupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        '
        '
        '
        Me.GroupPanel1.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.GroupPanel1.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.GroupPanel1.TabIndex = 30
        Me.GroupPanel1.Text = "BUSQUEDA"
        '
        'grBuscador
        '
        Me.grBuscador.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.grBuscador.BackColor = System.Drawing.Color.WhiteSmoke
        Me.grBuscador.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grBuscador.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grBuscador.HeaderFormatStyle.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grBuscador.Location = New System.Drawing.Point(0, 0)
        Me.grBuscador.Margin = New System.Windows.Forms.Padding(2)
        Me.grBuscador.Name = "grBuscador"
        Me.grBuscador.RowFormatStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grBuscador.Size = New System.Drawing.Size(936, 177)
        Me.grBuscador.TabIndex = 0
        '
        'OfdProducto
        '
        Me.OfdProducto.FileName = "OpenFileDialog1"
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 1000
        '
        'F0_Camion
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(984, 561)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "F0_Camion"
        Me.Opacity = 0.05R
        Me.Text = "F0_Camiones"
        Me.WindowState = System.Windows.Forms.FormWindowState.Minimized
        Me.Controls.SetChildIndex(Me.MPnSuperior, 0)
        Me.Controls.SetChildIndex(Me.MPnInferior, 0)
        Me.Controls.SetChildIndex(Me.MSuperTabControlPrincipal, 0)
        CType(Me.MSuperTabControlPrincipal, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MSuperTabControlPrincipal.ResumeLayout(False)
        Me.MSuperTabControlPanelRegistro.ResumeLayout(False)
        Me.MPnSuperior.ResumeLayout(False)
        Me.MPnInferior.ResumeLayout(False)
        Me.MPanelToolBarUsuario.ResumeLayout(False)
        Me.MPanelToolBarUsuario.PerformLayout()
        Me.MPanelToolBarNavegacion.ResumeLayout(False)
        Me.MPanelToolBarAccion.ResumeLayout(False)
        Me.MPanelToolBarImprimir.ResumeLayout(False)
        CType(Me.MBubbleBarUsuario, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MPnUsuario.ResumeLayout(False)
        Me.MPnUsuario.PerformLayout()
        CType(Me.MEP, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpDatos.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.tbCapacidad, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        CType(Me.pbImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupPanel1.ResumeLayout(False)
        CType(Me.grBuscador, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents gpDatos As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents grBuscador As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel1 As Panel
    Friend WithEvents LabelX1 As DevComponents.DotNetBar.LabelX
    Friend WithEvents LabelX2 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbcodigo As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents tbnombre As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents LabelX3 As DevComponents.DotNetBar.LabelX
    Friend WithEvents Panel2 As Panel
    Friend WithEvents pbImage As PictureBox
    Friend WithEvents BtAdicionar As DevComponents.DotNetBar.ButtonX
    Friend WithEvents LabelX11 As DevComponents.DotNetBar.LabelX
    Friend WithEvents OfdProducto As OpenFileDialog
    Friend WithEvents Timer1 As Timer
    Friend WithEvents LabelX4 As DevComponents.DotNetBar.LabelX
    Friend WithEvents swEstado As DevComponents.DotNetBar.Controls.SwitchButton
    Friend WithEvents tbCapacidad As DevComponents.Editors.DoubleInput
End Class
