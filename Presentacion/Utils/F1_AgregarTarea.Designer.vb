<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class F1_AgregarTarea
    Inherits System.Windows.Forms.Form

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
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ReflectionLabel1 = New DevComponents.DotNetBar.Controls.ReflectionLabel()
        Me.btnAgregar = New DevComponents.DotNetBar.ButtonX()
        Me.ButtonX1 = New DevComponents.DotNetBar.ButtonX()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.tbDesc = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.tbHoraE = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.tbHora = New DevComponents.Editors.DateTimeAdv.DateTimeInput()
        Me.tbDirec = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.tbDireccion = New System.Windows.Forms.Label()
        Me.tbObse = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.labelx121 = New System.Windows.Forms.Label()
        Me.Panel1.SuspendLayout()
        CType(Me.tbHora, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.DodgerBlue
        Me.Panel1.Controls.Add(Me.ReflectionLabel1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(2)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(492, 55)
        Me.Panel1.TabIndex = 10
        '
        'ReflectionLabel1
        '
        Me.ReflectionLabel1.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.ReflectionLabel1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.ReflectionLabel1.Font = New System.Drawing.Font("Calibri", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ReflectionLabel1.ForeColor = System.Drawing.Color.White
        Me.ReflectionLabel1.Location = New System.Drawing.Point(9, 10)
        Me.ReflectionLabel1.Margin = New System.Windows.Forms.Padding(2)
        Me.ReflectionLabel1.Name = "ReflectionLabel1"
        Me.ReflectionLabel1.Size = New System.Drawing.Size(278, 43)
        Me.ReflectionLabel1.TabIndex = 5
        Me.ReflectionLabel1.Text = "AGREGAR TAREA"
        '
        'btnAgregar
        '
        Me.btnAgregar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnAgregar.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground
        Me.btnAgregar.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAgregar.Image = Global.Presentacion.My.Resources.Resources.checked
        Me.btnAgregar.ImageFixedSize = New System.Drawing.Size(30, 30)
        Me.btnAgregar.Location = New System.Drawing.Point(261, 310)
        Me.btnAgregar.Margin = New System.Windows.Forms.Padding(2)
        Me.btnAgregar.Name = "btnAgregar"
        Me.btnAgregar.Size = New System.Drawing.Size(96, 42)
        Me.btnAgregar.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.btnAgregar.TabIndex = 373
        Me.btnAgregar.Text = "Confirmar"
        '
        'ButtonX1
        '
        Me.ButtonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground
        Me.ButtonX1.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX1.Image = Global.Presentacion.My.Resources.Resources.cancel
        Me.ButtonX1.ImageFixedSize = New System.Drawing.Size(30, 30)
        Me.ButtonX1.Location = New System.Drawing.Point(130, 310)
        Me.ButtonX1.Margin = New System.Windows.Forms.Padding(2)
        Me.ButtonX1.Name = "ButtonX1"
        Me.ButtonX1.Size = New System.Drawing.Size(96, 42)
        Me.ButtonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.ButtonX1.TabIndex = 374
        Me.ButtonX1.Text = "Salir"
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Calibri", 14.2!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.Color.Navy
        Me.Label1.Location = New System.Drawing.Point(29, 67)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(139, 37)
        Me.Label1.TabIndex = 375
        Me.Label1.Text = "Descripción:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Calibri", 14.2!, System.Drawing.FontStyle.Bold)
        Me.Label2.ForeColor = System.Drawing.Color.Navy
        Me.Label2.Location = New System.Drawing.Point(29, 121)
        Me.Label2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(154, 37)
        Me.Label2.TabIndex = 378
        Me.Label2.Text = "Hora de Llegada:"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Calibri", 14.2!, System.Drawing.FontStyle.Bold)
        Me.Label3.ForeColor = System.Drawing.Color.Navy
        Me.Label3.Location = New System.Drawing.Point(238, 121)
        Me.Label3.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(177, 37)
        Me.Label3.TabIndex = 380
        Me.Label3.Text = "Tiempo de Entrega:"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label3.Visible = False
        '
        'tbDesc
        '
        '
        '
        '
        Me.tbDesc.Border.Class = "TextBoxBorder"
        Me.tbDesc.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbDesc.Location = New System.Drawing.Point(33, 98)
        Me.tbDesc.Name = "tbDesc"
        Me.tbDesc.PreventEnterBeep = True
        Me.tbDesc.Size = New System.Drawing.Size(417, 20)
        Me.tbDesc.TabIndex = 381
        '
        'tbHoraE
        '
        '
        '
        '
        Me.tbHoraE.Border.Class = "TextBoxBorder"
        Me.tbHoraE.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbHoraE.Location = New System.Drawing.Point(242, 161)
        Me.tbHoraE.Name = "tbHoraE"
        Me.tbHoraE.PreventEnterBeep = True
        Me.tbHoraE.Size = New System.Drawing.Size(208, 20)
        Me.tbHoraE.TabIndex = 383
        Me.tbHoraE.Visible = False
        '
        'tbHora
        '
        '
        '
        '
        Me.tbHora.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbHora.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbHora.ButtonDropDown.Shortcut = DevComponents.DotNetBar.eShortcut.AltDown
        Me.tbHora.ButtonDropDown.Visible = True
        Me.tbHora.Format = DevComponents.Editors.eDateTimePickerFormat.ShortTime
        Me.tbHora.IsPopupCalendarOpen = False
        Me.tbHora.Location = New System.Drawing.Point(33, 161)
        '
        '
        '
        '
        '
        '
        Me.tbHora.MonthCalendar.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbHora.MonthCalendar.CalendarDimensions = New System.Drawing.Size(1, 1)
        Me.tbHora.MonthCalendar.ClearButtonVisible = True
        '
        '
        '
        Me.tbHora.MonthCalendar.CommandsBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2
        Me.tbHora.MonthCalendar.CommandsBackgroundStyle.BackColorGradientAngle = 90
        Me.tbHora.MonthCalendar.CommandsBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground
        Me.tbHora.MonthCalendar.CommandsBackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.tbHora.MonthCalendar.CommandsBackgroundStyle.BorderTopColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder
        Me.tbHora.MonthCalendar.CommandsBackgroundStyle.BorderTopWidth = 1
        Me.tbHora.MonthCalendar.CommandsBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbHora.MonthCalendar.DisplayMonth = New Date(2024, 7, 1, 0, 0, 0, 0)
        Me.tbHora.MonthCalendar.FirstDayOfWeek = System.DayOfWeek.Monday
        '
        '
        '
        Me.tbHora.MonthCalendar.NavigationBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2
        Me.tbHora.MonthCalendar.NavigationBackgroundStyle.BackColorGradientAngle = 90
        Me.tbHora.MonthCalendar.NavigationBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground
        Me.tbHora.MonthCalendar.NavigationBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbHora.MonthCalendar.TodayButtonVisible = True
        Me.tbHora.MonthCalendar.Visible = False
        Me.tbHora.Name = "tbHora"
        Me.tbHora.Size = New System.Drawing.Size(150, 20)
        Me.tbHora.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.tbHora.TabIndex = 384
        '
        'tbDirec
        '
        '
        '
        '
        Me.tbDirec.Border.Class = "TextBoxBorder"
        Me.tbDirec.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbDirec.Location = New System.Drawing.Point(33, 225)
        Me.tbDirec.Name = "tbDirec"
        Me.tbDirec.PreventEnterBeep = True
        Me.tbDirec.Size = New System.Drawing.Size(417, 20)
        Me.tbDirec.TabIndex = 386
        '
        'tbDireccion
        '
        Me.tbDireccion.Font = New System.Drawing.Font("Calibri", 14.2!, System.Drawing.FontStyle.Bold)
        Me.tbDireccion.ForeColor = System.Drawing.Color.Navy
        Me.tbDireccion.Location = New System.Drawing.Point(29, 194)
        Me.tbDireccion.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.tbDireccion.Name = "tbDireccion"
        Me.tbDireccion.Size = New System.Drawing.Size(139, 37)
        Me.tbDireccion.TabIndex = 385
        Me.tbDireccion.Text = "Dirección:"
        Me.tbDireccion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tbObse
        '
        '
        '
        '
        Me.tbObse.Border.Class = "TextBoxBorder"
        Me.tbObse.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbObse.Location = New System.Drawing.Point(33, 279)
        Me.tbObse.Name = "tbObse"
        Me.tbObse.PreventEnterBeep = True
        Me.tbObse.Size = New System.Drawing.Size(417, 20)
        Me.tbObse.TabIndex = 388
        '
        'labelx121
        '
        Me.labelx121.Font = New System.Drawing.Font("Calibri", 14.2!, System.Drawing.FontStyle.Bold)
        Me.labelx121.ForeColor = System.Drawing.Color.Navy
        Me.labelx121.Location = New System.Drawing.Point(29, 248)
        Me.labelx121.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.labelx121.Name = "labelx121"
        Me.labelx121.Size = New System.Drawing.Size(139, 37)
        Me.labelx121.TabIndex = 387
        Me.labelx121.Text = "Observación:"
        Me.labelx121.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'F1_AgregarTarea
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(492, 363)
        Me.Controls.Add(Me.tbObse)
        Me.Controls.Add(Me.labelx121)
        Me.Controls.Add(Me.tbDirec)
        Me.Controls.Add(Me.tbDireccion)
        Me.Controls.Add(Me.tbHora)
        Me.Controls.Add(Me.tbHoraE)
        Me.Controls.Add(Me.tbDesc)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ButtonX1)
        Me.Controls.Add(Me.btnAgregar)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "F1_AgregarTarea"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "F1_Cantidad"
        Me.Panel1.ResumeLayout(False)
        CType(Me.tbHora, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents ReflectionLabel1 As DevComponents.DotNetBar.Controls.ReflectionLabel
    Friend WithEvents btnAgregar As DevComponents.DotNetBar.ButtonX
    Friend WithEvents ButtonX1 As DevComponents.DotNetBar.ButtonX
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents tbDesc As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents tbHoraE As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents tbHora As DevComponents.Editors.DateTimeAdv.DateTimeInput
    Friend WithEvents tbDirec As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents tbDireccion As Label
    Friend WithEvents tbObse As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents labelx121 As Label
End Class
