<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class F0_PreciosAlterno2
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(F0_PreciosAlterno2))
        Me.PanelDatos = New System.Windows.Forms.Panel()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.grprecio = New Janus.Windows.GridEX.GridEX()
        Me.grPolloCrudo = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.grPrecioPolloEntero = New Janus.Windows.GridEX.GridEX()
        Me.GroupPanel4 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.grPP = New Janus.Windows.GridEX.GridEX()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.msModulos = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.SELECCIONARTODOSDELToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
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
        Me.PanelDatos.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.GroupPanel1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.grprecio, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grPolloCrudo.SuspendLayout()
        Me.Panel2.SuspendLayout()
        CType(Me.grPrecioPolloEntero, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupPanel4.SuspendLayout()
        Me.Panel5.SuspendLayout()
        CType(Me.grPP, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.msModulos.SuspendLayout()
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
        Me.MSuperTabControlPrincipal.Margin = New System.Windows.Forms.Padding(2)
        Me.MSuperTabControlPrincipal.Size = New System.Drawing.Size(803, 348)
        Me.MSuperTabControlPrincipal.Controls.SetChildIndex(Me.MSuperTabControlPanelRegistro, 0)
        '
        'MSuperTabControlPanelRegistro
        '
        Me.MSuperTabControlPanelRegistro.Controls.Add(Me.PanelDatos)
        Me.MSuperTabControlPanelRegistro.Margin = New System.Windows.Forms.Padding(2)
        Me.MSuperTabControlPanelRegistro.Size = New System.Drawing.Size(761, 348)
        Me.MSuperTabControlPanelRegistro.Controls.SetChildIndex(Me.MPnUsuario, 0)
        Me.MSuperTabControlPanelRegistro.Controls.SetChildIndex(Me.PanelDatos, 0)
        '
        'MPnSuperior
        '
        Me.MPnSuperior.Margin = New System.Windows.Forms.Padding(2)
        Me.MPnSuperior.Size = New System.Drawing.Size(803, 70)
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
        Me.MPnInferior.Location = New System.Drawing.Point(0, 418)
        Me.MPnInferior.Margin = New System.Windows.Forms.Padding(2)
        Me.MPnInferior.Size = New System.Drawing.Size(803, 36)
        Me.MPnInferior.Style.Alignment = System.Drawing.StringAlignment.Center
        Me.MPnInferior.Style.BackColor1.Color = System.Drawing.Color.Gray
        Me.MPnInferior.Style.BackColor2.Color = System.Drawing.SystemColors.ActiveCaption
        Me.MPnInferior.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine
        Me.MPnInferior.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.MPnInferior.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.MPnInferior.Style.GradientAngle = 90
        '
        'MPanelToolBarUsuario
        '
        Me.MPanelToolBarUsuario.Location = New System.Drawing.Point(603, 0)
        Me.MPanelToolBarUsuario.Margin = New System.Windows.Forms.Padding(2)
        '
        'MTbUsuario
        '
        Me.MTbUsuario.Margin = New System.Windows.Forms.Padding(2)
        Me.MTbUsuario.ReadOnly = True
        Me.MTbUsuario.Size = New System.Drawing.Size(135, 23)
        Me.MTbUsuario.Text = "DEFAULT"
        '
        'MPanelToolBarNavegacion
        '
        Me.MPanelToolBarNavegacion.Visible = False
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
        Me.MBtNuevo.Visible = False
        '
        'MPanelToolBarImprimir
        '
        Me.MPanelToolBarImprimir.Location = New System.Drawing.Point(588, 0)
        Me.MPanelToolBarImprimir.Margin = New System.Windows.Forms.Padding(2)
        Me.MPanelToolBarImprimir.Size = New System.Drawing.Size(215, 70)
        '
        'MBtImprimir
        '
        Me.MBtImprimir.Location = New System.Drawing.Point(143, 0)
        Me.MBtImprimir.Margin = New System.Windows.Forms.Padding(2)
        Me.MBtImprimir.Text = "Excel"
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
        Me.MLbPaginacion.Text = ""
        '
        'MRlAccion
        '
        '
        '
        '
        Me.MRlAccion.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        'PanelDatos
        '
        Me.PanelDatos.BackColor = System.Drawing.Color.White
        Me.PanelDatos.Controls.Add(Me.TableLayoutPanel2)
        Me.PanelDatos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDatos.Location = New System.Drawing.Point(0, 0)
        Me.PanelDatos.Margin = New System.Windows.Forms.Padding(2)
        Me.PanelDatos.Name = "PanelDatos"
        Me.PanelDatos.Size = New System.Drawing.Size(761, 348)
        Me.PanelDatos.TabIndex = 29
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.AutoSize = True
        Me.TableLayoutPanel2.BackColor = System.Drawing.Color.Azure
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.GroupPanel1, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.grPolloCrudo, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.GroupPanel4, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.PictureBox1, 1, 1)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 2
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(761, 348)
        Me.TableLayoutPanel2.TabIndex = 5
        '
        'GroupPanel1
        '
        Me.GroupPanel1.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel1.Controls.Add(Me.Panel1)
        Me.GroupPanel1.DisabledBackColor = System.Drawing.Color.Empty
        Me.GroupPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel1.Font = New System.Drawing.Font("Georgia", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupPanel1.Location = New System.Drawing.Point(3, 90)
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(222, 255)
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
        Me.GroupPanel1.TabIndex = 4
        Me.GroupPanel1.Text = "PT"
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.White
        Me.Panel1.Controls.Add(Me.grprecio)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(216, 232)
        Me.Panel1.TabIndex = 0
        '
        'grprecio
        '
        Me.grprecio.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grprecio.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grprecio.HeaderFormatStyle.Font = New System.Drawing.Font("Georgia", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grprecio.Location = New System.Drawing.Point(0, 0)
        Me.grprecio.Name = "grprecio"
        Me.grprecio.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grprecio.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grprecio.Size = New System.Drawing.Size(216, 232)
        Me.grprecio.TabIndex = 0
        Me.grprecio.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'grPolloCrudo
        '
        Me.grPolloCrudo.CanvasColor = System.Drawing.SystemColors.Control
        Me.grPolloCrudo.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.grPolloCrudo.Controls.Add(Me.Panel2)
        Me.grPolloCrudo.DisabledBackColor = System.Drawing.Color.Empty
        Me.grPolloCrudo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grPolloCrudo.Font = New System.Drawing.Font("Georgia", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPolloCrudo.Location = New System.Drawing.Point(231, 3)
        Me.grPolloCrudo.Name = "grPolloCrudo"
        Me.grPolloCrudo.Size = New System.Drawing.Size(527, 81)
        '
        '
        '
        Me.grPolloCrudo.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2
        Me.grPolloCrudo.Style.BackColorGradientAngle = 90
        Me.grPolloCrudo.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground
        Me.grPolloCrudo.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.grPolloCrudo.Style.BorderBottomWidth = 1
        Me.grPolloCrudo.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.grPolloCrudo.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.grPolloCrudo.Style.BorderLeftWidth = 1
        Me.grPolloCrudo.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.grPolloCrudo.Style.BorderRightWidth = 1
        Me.grPolloCrudo.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.grPolloCrudo.Style.BorderTopWidth = 1
        Me.grPolloCrudo.Style.CornerDiameter = 4
        Me.grPolloCrudo.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.grPolloCrudo.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.grPolloCrudo.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.grPolloCrudo.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        '
        '
        '
        Me.grPolloCrudo.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.grPolloCrudo.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.grPolloCrudo.TabIndex = 3
        Me.grPolloCrudo.Text = "PP"
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.White
        Me.Panel2.Controls.Add(Me.grPrecioPolloEntero)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(521, 58)
        Me.Panel2.TabIndex = 0
        '
        'grPrecioPolloEntero
        '
        Me.grPrecioPolloEntero.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grPrecioPolloEntero.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPrecioPolloEntero.HeaderFormatStyle.Font = New System.Drawing.Font("Georgia", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPrecioPolloEntero.Location = New System.Drawing.Point(0, 0)
        Me.grPrecioPolloEntero.Name = "grPrecioPolloEntero"
        Me.grPrecioPolloEntero.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grPrecioPolloEntero.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grPrecioPolloEntero.Size = New System.Drawing.Size(521, 58)
        Me.grPrecioPolloEntero.TabIndex = 0
        Me.grPrecioPolloEntero.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'GroupPanel4
        '
        Me.GroupPanel4.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel4.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel4.Controls.Add(Me.Panel5)
        Me.GroupPanel4.DisabledBackColor = System.Drawing.Color.Empty
        Me.GroupPanel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel4.Font = New System.Drawing.Font("Georgia", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupPanel4.Location = New System.Drawing.Point(3, 3)
        Me.GroupPanel4.Name = "GroupPanel4"
        Me.GroupPanel4.Size = New System.Drawing.Size(222, 81)
        '
        '
        '
        Me.GroupPanel4.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2
        Me.GroupPanel4.Style.BackColorGradientAngle = 90
        Me.GroupPanel4.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground
        Me.GroupPanel4.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel4.Style.BorderBottomWidth = 1
        Me.GroupPanel4.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel4.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel4.Style.BorderLeftWidth = 1
        Me.GroupPanel4.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel4.Style.BorderRightWidth = 1
        Me.GroupPanel4.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel4.Style.BorderTopWidth = 1
        Me.GroupPanel4.Style.CornerDiameter = 4
        Me.GroupPanel4.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel4.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel4.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.GroupPanel4.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        '
        '
        '
        Me.GroupPanel4.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.GroupPanel4.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.GroupPanel4.TabIndex = 2
        Me.GroupPanel4.Text = "CAJA CERRADA"
        '
        'Panel5
        '
        Me.Panel5.BackColor = System.Drawing.Color.White
        Me.Panel5.Controls.Add(Me.grPP)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel5.Location = New System.Drawing.Point(0, 0)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(216, 58)
        Me.Panel5.TabIndex = 0
        '
        'grPP
        '
        Me.grPP.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grPP.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPP.HeaderFormatStyle.Font = New System.Drawing.Font("Georgia", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPP.Location = New System.Drawing.Point(0, 0)
        Me.grPP.Name = "grPP"
        Me.grPP.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grPP.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grPP.Size = New System.Drawing.Size(216, 58)
        Me.grPP.TabIndex = 0
        Me.grPP.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'PictureBox1
        '
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(231, 90)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(527, 255)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 5
        Me.PictureBox1.TabStop = False
        '
        'msModulos
        '
        Me.msModulos.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.msModulos.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SELECCIONARTODOSDELToolStripMenuItem})
        Me.msModulos.Name = "msModulos"
        Me.msModulos.Size = New System.Drawing.Size(193, 26)
        '
        'SELECCIONARTODOSDELToolStripMenuItem
        '
        Me.SELECCIONARTODOSDELToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.SELECCIONARTODOSDELToolStripMenuItem.Name = "SELECCIONARTODOSDELToolStripMenuItem"
        Me.SELECCIONARTODOSDELToolStripMenuItem.Size = New System.Drawing.Size(192, 22)
        Me.SELECCIONARTODOSDELToolStripMenuItem.Text = "ELIMINAR CATEGORIA"
        '
        'F0_PreciosAlterno2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(803, 454)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "F0_PreciosAlterno2"
        Me.Text = "F0_PreciosAlterno"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
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
        Me.PanelDatos.ResumeLayout(False)
        Me.PanelDatos.PerformLayout()
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.GroupPanel1.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        CType(Me.grprecio, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grPolloCrudo.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        CType(Me.grPrecioPolloEntero, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupPanel4.ResumeLayout(False)
        Me.Panel5.ResumeLayout(False)
        CType(Me.grPP, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.msModulos.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelDatos As Panel
    Friend WithEvents msModulos As ContextMenuStrip
    Friend WithEvents SELECCIONARTODOSDELToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TableLayoutPanel2 As TableLayoutPanel
    Friend WithEvents GroupPanel4 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents Panel5 As Panel
    Friend WithEvents grprecio As Janus.Windows.GridEX.GridEX
    Friend WithEvents grPolloCrudo As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents Panel2 As Panel
    Friend WithEvents grPrecioPolloEntero As Janus.Windows.GridEX.GridEX
    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents Panel1 As Panel
    Friend WithEvents grPP As Janus.Windows.GridEX.GridEX
    Friend WithEvents PictureBox1 As PictureBox
End Class
