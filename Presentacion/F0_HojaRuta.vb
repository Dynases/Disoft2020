Imports DevComponents.DotNetBar
Imports DevComponents.DotNetBar.Controls
Imports ENTITY
Imports Janus.Windows.GridEX
Imports LOGIC
Imports UTILITIES
Imports Facturacion
Imports Logica.AccesoLogica
Imports System.Drawing.Printing
Imports System.IO
Imports CrystalDecisions.Shared
Imports System.Net
Imports System.Reflection
Imports System.ComponentModel
Imports Newtonsoft.Json
Imports Presentacion.RespPDF
Imports Presentacion.RespFactura
Imports PdfiumViewer
Imports iTextSharp.text.pdf
Imports iTextSharp.text
Imports Janus.Windows.GridEX.GridEXColumn
Imports CrystalDecisions.CrystalReports.Engine
Imports Microsoft.Reporting.WinForms
Imports DevComponents.DotNetBar.SuperGrid
Imports DevComponents.DotNetBar.SuperGrid.GridPanel
Imports DevComponents.DotNetBar.SuperGrid.SuperGrid
Imports System.Drawing.Drawing2D
Imports System.Globalization

Public Class F0_HojaRuta
    Dim _inter As Integer = 0
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem

    Private _cargaCompleta = False

    Public nit As String
    Public razonsocial As String
    Public email As String
    Public tipoDoc As Integer

    Public fact As Integer = 0

    Dim Nuevo As Boolean = False
    Dim Modificar As Boolean = False
    Dim Grilla1 As Integer = 0
    Dim Grilla2 As Integer = 0
    Dim Grilla3 As Integer = 0
    Dim Grilla4 As Integer = 0

    Dim Descripcion, HoraL, HoraE As String

    Public direccion, observacion As String

    Dim dtCab As DataTable = TraerCabecera()


#Region "Eventos"
    Private Sub frmBillingDispatch_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        grCamiones.TabSelection = TabSelection.CellSameRow
        Init()

    End Sub

    Private Sub cbChoferes_ValueChanged(sender As Object, e As EventArgs) Handles cbChoferes.ValueChanged
        Try
            If (_cargaCompleta) Then
                CargarPedidos()


            End If
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
    End Sub


    Private Function P_fnValidarFactura() As Boolean
        Return True
    End Function

    Private Function P_fnGenerarFactura(numi As String, subtotal As Double, descuento As Double, total As Double, nit As String, Nombre As String, Codcli As String) As Boolean
        Dim res As Boolean = False
        res = P_fnGrabarFacturarTFV001(numi, subtotal, descuento, total, nit, Nombre, Codcli) ' Grabar en la TFV001

        If (res) Then
            'Grabar Estado 5 de Facturado en la TO001D
            L_GrabarTO001D(numi, "5", "Factura")

            If (P_fnValidarFactura()) Then
                'Validar para facturar
                P_prImprimirFacturar(numi, True, True, nit) '_Codigo de a tabla TV001
            Else
                'Volver todo al estada anterior
                ToastNotification.Show(Me, "No es posible facturar!!!".ToUpper,
                                       My.Resources.OK,
                                       5 * 1000,
                                       eToastGlowColor.Red,
                                       eToastPosition.MiddleCenter)
            End If

            If (Not nit.Equals("0")) Then
                L_Grabar_Nit(nit, Nombre, "")
            Else
                L_Grabar_Nit(nit, "S/N", "")
            End If
        End If
        Dim dtfv001 As DataTable = L_fnObtenerTabla("fvanitcli, fvadescli1, fvadescli2, fvaautoriz, fvanfac, fvaccont, fvafec,fvaest", "TFV001", "fvanumi=" + numi + " or fvanumi=" + "-" + numi)
        If dtfv001.Rows.Count = 2 Then
            L_ActualizaNegativosTFV001(numi, "0")
        End If

        Return res
    End Function


    Private Sub P_prImprimirFacturar(numi As String, impFactura As Boolean, grabarPDF As Boolean, nit As String)
        Dim _Fecha, _FechaAl As Date
        Dim _Ds, _Ds1, _Ds2, _Ds3 As New DataSet
        Dim _Autorizacion, _Nit, _Fechainv, _Total, _Key, _Cod_Control, _Hora,
            _Literal, _TotalDecimal, _TotalDecimal2 As String
        Dim I, _NumFac, _numidosif, _TotalCC As Integer
        Dim ice, _Desc, _TotalLi As Decimal
        Dim _VistaPrevia As Integer = 0
        Dim QrFactura1 As String

        _Desc = CDbl(0)
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If

        _Fecha = Now.Date.ToString("dd/MM/yyyy")
        _Hora = Now.Hour.ToString + ":" + Now.Minute.ToString
        _Ds1 = L_Dosificacion("1", "1", _Fecha)

        _Ds = L_Reporte_Factura(numi, numi)
        _Autorizacion = _Ds1.Tables(0).Rows(0).Item("yeautoriz").ToString
        _NumFac = CInt(_Ds1.Tables(0).Rows(0).Item("yenunf")) + 1
        _Nit = _Ds.Tables(0).Rows(0).Item("fvanitcli").ToString
        _Fechainv = Microsoft.VisualBasic.Right(_Fecha.ToShortDateString, 4) +
                    Microsoft.VisualBasic.Right(Microsoft.VisualBasic.Left(_Fecha.ToShortDateString, 5), 2) +
                    Microsoft.VisualBasic.Left(_Fecha.ToShortDateString, 2)
        _Total = _Ds.Tables(0).Rows(0).Item("fvatotal").ToString
        ice = _Ds.Tables(0).Rows(0).Item("fvaimpsi")
        _numidosif = _Ds1.Tables(0).Rows(0).Item("yenumi").ToString
        _Key = _Ds1.Tables(0).Rows(0).Item("yekey")
        _FechaAl = _Ds1.Tables(0).Rows(0).Item("yefal")

        Dim maxNFac As Integer = L_fnObtenerMaxIdTabla("TFV001", "fvanfac", "fvaautoriz = " + _Autorizacion)
        _NumFac = maxNFac + 1

        _TotalCC = Math.Round(CDbl(_Total), MidpointRounding.AwayFromZero)
        _Cod_Control = ControlCode.generateControlCode(_Autorizacion, _NumFac, _Nit, _Fechainv, CStr(_TotalCC), _Key)

        'Literal 
        _TotalLi = _Ds.Tables(0).Rows(0).Item("fvasubtotal") - _Ds.Tables(0).Rows(0).Item("fvadesc")
        _TotalDecimal = _TotalLi - Math.Truncate(_TotalLi)
        _TotalDecimal2 = CDbl(_TotalDecimal) * 100

        'Dim li As String = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(_Total) - CDbl(_TotalDecimal)) + " con " + IIf(_TotalDecimal2.Equals("0"), "00", _TotalDecimal2) + "/100 Bolivianos"
        _Literal = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(_TotalLi) - CDbl(_TotalDecimal)) + " con " + IIf(_TotalDecimal2.Equals("0"), "00", _TotalDecimal2) + "/100 Bolivianos"
        _Ds2 = L_Reporte_Factura_Cia("1")

        L_Modificar_Factura("fvanumi = " + CStr(numi),
                            "",
                            CStr(_NumFac),
                            CStr(_Autorizacion),
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            _Cod_Control,
                            _FechaAl.ToString("yyyy/MM/dd"),
                            "",
                            "",
                            CStr(numi))


        updateTO001C(numi, Str(_NumFac))
        _Ds = L_Reporte_Factura(numi, numi)

        _Ds3 = L_ObtenerRutaImpresora("1") ' Datos de Impresion de Facturación

        For I = 0 To _Ds.Tables(0).Rows.Count - 1
            '_Ds.Tables(0).Rows(I).Item("fvaimgqr") = P_fnImageToByteArray(QrFactura.Image)
        Next
        P_Global.Visualizador = New Visualizador
        Dim objrep As New Factura
        Dim dia, mes, ano As Integer
        Dim Fecliteral, mesl As String
        Fecliteral = _Ds.Tables(0).Rows(0).Item("fvafec").ToString
        dia = Microsoft.VisualBasic.Left(Fecliteral, 2)
        mes = Microsoft.VisualBasic.Mid(Fecliteral, 4, 2)
        ano = Microsoft.VisualBasic.Mid(Fecliteral, 7, 4)
        If mes = 1 Then
            mesl = "Enero"
        End If
        If mes = 2 Then
            mesl = "Febrero"
        End If
        If mes = 3 Then
            mesl = "Marzo"
        End If
        If mes = 4 Then
            mesl = "Abril"
        End If
        If mes = 5 Then
            mesl = "Mayo"
        End If
        If mes = 6 Then
            mesl = "Junio"
        End If
        If mes = 7 Then
            mesl = "Julio"
        End If
        If mes = 8 Then
            mesl = "Agosto"
        End If
        If mes = 9 Then
            mesl = "Septiembre"
        End If
        If mes = 10 Then
            mesl = "Octubre"
        End If
        If mes = 11 Then
            mesl = "Noviembre"
        End If
        If mes = 12 Then
            mesl = "Diciembre"
        End If
        Dim tipoPago = ObtenerTipoDePagoPedido(numi)

        Dim cadena As String = _Ds2.Tables(0).Rows(0).Item("scciu").ToString
        Dim posicion As Integer = cadena.IndexOf("-")
        Dim ciudad As String = cadena.Substring(0, posicion)

        Fecliteral = ciudad + ",  " + dia.ToString + " de " + mesl + " del " + ano.ToString
        objrep.SetDataSource(_Ds.Tables(0))

        objrep.SetParameterValue("Fecliteral", Fecliteral)
        objrep.SetParameterValue("Nota2", _Ds1.Tables(0).Rows(0).Item("yenota2").ToString())
        'objrep.PrintOptions.PrinterName = "L4150 Series(Red) (Copiar 1)"

        objrep.SetParameterValue("Direccionpr", _Ds2.Tables(0).Rows(0).Item("scdir").ToString)
        objrep.SetParameterValue("Literal1", _Literal)
        objrep.SetParameterValue("NroFactura", _NumFac)
        objrep.SetParameterValue("NroAutoriz", _Autorizacion)
        objrep.SetParameterValue("ENombre", _Ds2.Tables(0).Rows(0).Item("scneg").ToString) '?
        objrep.SetParameterValue("ECasaMatriz", _Ds2.Tables(0).Rows(0).Item("scsuc").ToString)
        objrep.SetParameterValue("ECiudadPais", _Ds2.Tables(0).Rows(0).Item("scciu").ToString)
        objrep.SetParameterValue("ESFC", _Ds1.Tables(0).Rows(0).Item("yesfc").ToString)
        objrep.SetParameterValue("ENit", _Ds2.Tables(0).Rows(0).Item("scnit").ToString)
        objrep.SetParameterValue("EActividad", _Ds2.Tables(0).Rows(0).Item("scact").ToString)
        objrep.SetParameterValue("Tipo", "ORIGINAL")
        objrep.SetParameterValue("TipoPago", tipoPago)
        objrep.SetParameterValue("Logo", gb_ubilogo)
        'If imp = 1 Then
        '    objrep.SetParameterValue("Tipo", "ORIGINAL")
        'Else
        '    objrep.SetParameterValue("Tipo", "COPIA")
        'End If
        If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
            P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
            P_Global.Visualizador.ShowDialog() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar
        Else
            Dim pd As New PrintDocument()
            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
            If (Not pd.PrinterSettings.IsValid) Then
                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
            Else
                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                objrep.PrintToPrinter(1, False, 1, 1)
            End If
        End If

        L_Actualiza_Dosificacion(_numidosif, _NumFac, numi)

        If (grabarPDF) Then
            'Copia de Factura en PDF
            If (Not Directory.Exists(gs_CarpetaRaiz + "\Facturas")) Then
                Directory.CreateDirectory(gs_CarpetaRaiz + "\Facturas")
            End If
            objrep.ExportToDisk(ExportFormatType.PortableDocFormat, gs_CarpetaRaiz + "\Facturas\" + CStr(_NumFac) + "_" + CStr(_Autorizacion) + ".pdf")

        End If
        'Dim pd As New PrintDocument()
        'pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
        'If (Not pd.PrinterSettings.IsValid) Then
        '    ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
        '                           My.Resources.WARNING, 5 * 1000,
        '                           eToastGlowColor.Blue, eToastPosition.BottomRight)
        'Else
        '    objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
        '    objrep.PrintToPrinter(1, False, 1, 1)
        'End If
        'objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
        'objrep.PrintToPrinter(1, False, 1, 1)




        ''For I = 0 To _Ds.Tables(0).Rows.Count - 1
        ''    _Ds.Tables(0).Rows(I).Item("fvaimgqr") = P_fnImageToByteArray(QrFactura.Image)
        ''Next
        'If (impFactura) Then
        '    _Ds3 = L_ObtenerRutaImpresora("1") ' Datos de Impresion de Facturación
        '    If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
        '        P_Global.Visualizador = New Visualizador 'Comentar
        '    End If


        '    Dim objrep As Object = Nothing
        '    objrep = New R_FacturaPreImpresa

        '    objrep.SetDataSource(_Ds.Tables(0))
        '    objrep.SetParameterValue("Hora", _Hora)
        '    objrep.SetParameterValue("Literal", _Literal)

        '    P_Global.Visualizador.CRV1.ReportSource = objrep
        '    P_Global.Visualizador.Show()
        '    P_Global.Visualizador.BringToFront()


        '    'If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
        '    '    P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
        '    '    P_Global.Visualizador.ShowDialog() 'Comentar
        '    '    P_Global.Visualizador.BringToFront() 'Comentar
        '    'End If

        '    'Dim pd As New PrintDocument()
        '    'pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
        '    'If (Not pd.PrinterSettings.IsValid) Then
        '    '    ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
        '    '                           My.Resources.WARNING, 5 * 1000,
        '    '                           eToastGlowColor.Blue, eToastPosition.BottomRight)
        '    'Else
        '    '    objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString '"EPSON TM-T20II Receipt5 (1)"
        '    '    objrep.PrintToPrinter(1, False, 1, 1)
        '    'End If

        'If (grabarPDF) Then
        '    'Copia de Factura en PDF
        '    If (Not Directory.Exists(gs_CarpetaRaiz + "\Facturas")) Then
        '        Directory.CreateDirectory(gs_CarpetaRaiz + "\Facturas")
        '    End If
        '    objrep.ExportToDisk(ExportFormatType.PortableDocFormat, gs_CarpetaRaiz + "\Facturas\" + CStr(_NumFac) + "_" + CStr(_Autorizacion) + ".pdf")

        'End If
        'End If

    End Sub
    Private Sub P_ReImprImprimirFacturar(numi As String, impFactura As Boolean, grabarPDF As Boolean, nit As String)
        Dim _Fecha, _FechaAl As Date
        Dim _Ds, _Ds1, _Ds2, _Ds3 As New DataSet
        Dim _Autorizacion, _Nit, _Fechainv, _Total, _Key, _Cod_Control, _Hora,
            _Literal, _TotalDecimal, _TotalDecimal2 As String
        Dim I, _NumFac, _numidosif, _TotalCC As Integer
        Dim ice, _Desc, _TotalLi As Decimal
        Dim _VistaPrevia As Integer = 0
        Dim QrFactura1 As String

        _Desc = CDbl(0)
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If

        _Fecha = Now.Date.ToString("dd/MM/yyyy")
        _Hora = Now.Hour.ToString + ":" + Now.Minute.ToString
        _Ds1 = L_Dosificacion("1", "1", _Fecha)

        _Ds = L_Reporte_Factura(numi, numi)
        _Autorizacion = _Ds1.Tables(0).Rows(0).Item("yeautoriz").ToString
        _NumFac = CInt(_Ds1.Tables(0).Rows(0).Item("yenunf")) + 1
        _Nit = _Ds.Tables(0).Rows(0).Item("fvanitcli").ToString
        _Fechainv = Microsoft.VisualBasic.Right(_Fecha.ToShortDateString, 4) +
                    Microsoft.VisualBasic.Right(Microsoft.VisualBasic.Left(_Fecha.ToShortDateString, 5), 2) +
                    Microsoft.VisualBasic.Left(_Fecha.ToShortDateString, 2)
        _Total = _Ds.Tables(0).Rows(0).Item("fvatotal").ToString
        ice = _Ds.Tables(0).Rows(0).Item("fvaimpsi")
        _numidosif = _Ds1.Tables(0).Rows(0).Item("yenumi").ToString
        _Key = _Ds1.Tables(0).Rows(0).Item("yekey")
        _FechaAl = _Ds1.Tables(0).Rows(0).Item("yefal")

        _NumFac = CInt(_Ds.Tables(0).Rows(0).Item("fvanfac").ToString)
        'Dim maxnfac As Integer = L_fnObtenerMaxIdTabla("tfv001", "fvanfac", "fvaautoriz = " + _Autorizacion)
        '_NumFac = maxnfac + 1

        _TotalCC = Math.Round(CDbl(_Total), MidpointRounding.AwayFromZero)
        _Cod_Control = ControlCode.generateControlCode(_Autorizacion, _NumFac, _Nit, _Fechainv, CStr(_TotalCC), _Key)

        'Literal 
        _TotalLi = _Ds.Tables(0).Rows(0).Item("fvasubtotal") - _Ds.Tables(0).Rows(0).Item("fvadesc")
        _TotalDecimal = _TotalLi - Math.Truncate(_TotalLi)
        _TotalDecimal2 = CDbl(_TotalDecimal) * 100

        'Dim li As String = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(_Total) - CDbl(_TotalDecimal)) + " con " + IIf(_TotalDecimal2.Equals("0"), "00", _TotalDecimal2) + "/100 Bolivianos"
        _Literal = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(_TotalLi) - CDbl(_TotalDecimal)) + " con " + IIf(_TotalDecimal2.Equals("0"), "00", _TotalDecimal2) + "/100 Bolivianos"
        _Ds2 = L_Reporte_Factura_Cia("1")

        L_Modificar_Factura("fvanumi = " + CStr(numi),
                            "",
                            CStr(_NumFac),
                            CStr(_Autorizacion),
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            _Cod_Control,
                            _FechaAl.ToString("yyyy/MM/dd"),
                            "",
                            "",
                            CStr(numi))


        updateTO001C(numi, Str(_NumFac))
        _Ds = L_Reporte_Factura(numi, numi)

        _Ds3 = L_ObtenerRutaImpresora("1") ' Datos de Impresion de Facturación

        For I = 0 To _Ds.Tables(0).Rows.Count - 1
            '_Ds.Tables(0).Rows(I).Item("fvaimgqr") = P_fnImageToByteArray(QrFactura.Image)
        Next
        P_Global.Visualizador = New Visualizador
        Dim objrep As New Factura
        Dim dia, mes, ano As Integer
        Dim Fecliteral, mesl As String
        Fecliteral = _Ds.Tables(0).Rows(0).Item("fvafec").ToString
        dia = Microsoft.VisualBasic.Left(Fecliteral, 2)
        mes = Microsoft.VisualBasic.Mid(Fecliteral, 4, 2)
        ano = Microsoft.VisualBasic.Mid(Fecliteral, 7, 4)
        mesl = ObtenerMesLiberal(mes)

        Dim tipoPago = ObtenerTipoDePagoPedido(numi)

        Dim cadena As String = _Ds2.Tables(0).Rows(0).Item("scciu").ToString
        Dim posicion As Integer = cadena.IndexOf("-")
        Dim ciudad As String = cadena.Substring(0, posicion)

        Fecliteral = ciudad + ",  " + dia.ToString + " de " + mesl + " del " + ano.ToString
        objrep.SetDataSource(_Ds.Tables(0))

        objrep.SetParameterValue("Fecliteral", Fecliteral)
        objrep.SetParameterValue("Nota2", _Ds1.Tables(0).Rows(0).Item("yenota2").ToString())
        'objrep.PrintOptions.PrinterName = "L4150 Series(Red) (Copiar 1)"

        objrep.SetParameterValue("Direccionpr", _Ds2.Tables(0).Rows(0).Item("scdir").ToString)
        objrep.SetParameterValue("Literal1", _Literal)
        objrep.SetParameterValue("NroFactura", _NumFac)
        objrep.SetParameterValue("NroAutoriz", _Autorizacion)
        objrep.SetParameterValue("ENombre", _Ds2.Tables(0).Rows(0).Item("scneg").ToString) '?
        objrep.SetParameterValue("ECasaMatriz", _Ds2.Tables(0).Rows(0).Item("scsuc").ToString)
        objrep.SetParameterValue("ECiudadPais", _Ds2.Tables(0).Rows(0).Item("scciu").ToString)
        objrep.SetParameterValue("ESFC", _Ds1.Tables(0).Rows(0).Item("yesfc").ToString)
        objrep.SetParameterValue("ENit", _Ds2.Tables(0).Rows(0).Item("scnit").ToString)
        objrep.SetParameterValue("EActividad", _Ds2.Tables(0).Rows(0).Item("scact").ToString)
        objrep.SetParameterValue("Tipo", "ORIGINAL")
        objrep.SetParameterValue("TipoPago", tipoPago)
        objrep.SetParameterValue("Logo", gb_ubilogo)
        'If imp = 1 Then
        '    objrep.SetParameterValue("Tipo", "ORIGINAL")
        'Else
        '    objrep.SetParameterValue("Tipo", "COPIA")
        'End If
        If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
            P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
            P_Global.Visualizador.ShowDialog() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar
        Else
            Dim pd As New PrintDocument()
            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
            If (Not pd.PrinterSettings.IsValid) Then
                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
            Else
                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                objrep.PrintToPrinter(1, False, 1, 1)
            End If
        End If
        'If (grabarPDF) Then
        '    'Copia de Factura en PDF
        '    If (Not Directory.Exists(gs_CarpetaRaiz + "\Facturas")) Then
        '        Directory.CreateDirectory(gs_CarpetaRaiz + "\Facturas")
        '    End If
        '    objrep.ExportToDisk(ExportFormatType.PortableDocFormat, gs_CarpetaRaiz + "\Facturas\" + CStr(_NumFac) + "_" + CStr(_Autorizacion) + ".pdf")

        'End If
        L_Actualiza_Dosificacion(_numidosif, _NumFac, numi)
    End Sub

    Public Sub P_prImprimirNotaVenta(idPedido As String, impFactura As Boolean, grabarPDF As Boolean, idChofer As String, nomVendedor As String)
        Dim _Fecha, _FechaAl As Date
        Dim _Ds, _Ds2, _Ds3 As New DataSet
        Dim _Hora, _Literal, _TotalDecimal, _TotalDecimal2 As String
        Dim _NumFac, _numidosif As Integer
        Dim _Desc, _TotalLi As Decimal
        Dim _VistaPrevia As Integer = 0

        _Desc = CDbl(0)

        Dim listResult = New LPedido().ListarDespachoXNotaVentaDeChofer(idChofer, idPedido)
        If (listResult.Count = 0) Then
            Throw New Exception("No hay registros para generar el reporte.")
        End If
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If

        _Fecha = Now.Date.ToString("dd/MM/yyyy")
        _Hora = Now.Hour.ToString + ":" + Now.Minute.ToString

        '_Ds = L_Reporte_Factura(numi, numi)


        'Literal 
        _TotalLi = listResult.Item(0).Total
        _TotalDecimal = _TotalLi - Math.Truncate(_TotalLi)
        _TotalDecimal2 = CDbl(_TotalDecimal) * 100

        _Literal = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(_TotalLi) - CDbl(_TotalDecimal)) + " con " + IIf(_TotalDecimal2.Equals("0"), "00", _TotalDecimal2) + "/100 Bolivianos"
        _Ds2 = L_Reporte_Factura_Cia("1")
        _Ds3 = L_ObtenerRutaImpresora("1") ' Datos de Impresion de Facturación
        Dim objrep As Object = Nothing
        Select Case _Ds3.Tables(0).Rows(0).Item("cbtimp").ToString
            Case "1"
                ReporteNotaVenta2(idPedido, _Ds2, _Ds3, _Literal, listResult)
            Case "2"
                ReporteNotaVenta(idPedido, _Ds2, _Ds3, _Literal, listResult)
            Case "3"
                ReporteNotaVenta3(idPedido, _Ds2, _Ds3, _Literal, listResult)
            Case "4"
                ReporteNotaVenta4(idPedido, _Ds2, _Ds3, _Literal, listResult)
            Case "5"
                ReporteNotaVenta5(idPedido, _Ds2, _Ds3, _Literal, listResult)
            Case "6"
                ReporteNotaVenta6(idPedido, _Ds2, _Ds3, _Literal, listResult)
            Case "7"
                ReporteNotaVenta7(idPedido, _Ds2, _Ds3, _Literal, listResult)
            Case "8"
                ReporteNotaVenta8(idPedido, _Ds2, _Ds3, _Literal, listResult)
            Case "9"
                ReporteNotaVenta9(idPedido, _Ds2, _Ds3, _Literal, listResult)
            Case "10"
                ReporteNotaVenta10(idPedido, _Ds2, _Ds3, _Literal, listResult, nomVendedor)
            Case "11"
                ReporteNotaVenta11(idPedido, _Ds2, _Ds3, _Literal, listResult, nomVendedor)
            Case "12"
                ReporteNotaVenta12(idPedido, _Ds2, _Ds3, _Literal, listResult, nomVendedor)
            Case "13"
                ReporteNotaVenta13(idPedido, _Ds2, _Ds3, _Literal, listResult, nomVendedor)
        End Select
    End Sub

    Private Sub ReporteNotaVenta(idPedido As String, _Ds2 As DataSet, _Ds3 As DataSet, _Literal As String, listResult As List(Of RDespachoNotaVenta))
        P_Global.Visualizador = New Visualizador
        Dim objrep As New NotaVenta
        Dim dia, mes, ano As Integer
        Dim Fecliteral, mesl As String

        Fecliteral = listResult.Item(0).oafdoc
        dia = Microsoft.VisualBasic.Left(Fecliteral, 2)
        mes = Microsoft.VisualBasic.Mid(Fecliteral, 4, 2)
        ano = Microsoft.VisualBasic.Mid(Fecliteral, 7, 4)

        mesl = ObtenerMesLiberal(mes)

        Fecliteral = "Santa Cruz, " + dia.ToString + " de " + mesl + " del " + ano.ToString
        objrep.SetDataSource(listResult)
        objrep.SetParameterValue("Literal", _Literal)
        objrep.SetParameterValue("Fechali", Fecliteral)
        objrep.SetParameterValue("nombreUsuario", P_Global.gs_user)
        objrep.SetParameterValue("Empresa", _Ds2.Tables(0).Rows(0).Item("scneg").ToString)
        objrep.SetParameterValue("Telefono", _Ds2.Tables(0).Rows(0).Item("sctelf").ToString)
        objrep.SetParameterValue("Logo", gb_ubilogo)

        If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
            P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
            P_Global.Visualizador.ShowDialog() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar
        Else
            Dim pd As New PrintDocument()
            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
            If (Not pd.PrinterSettings.IsValid) Then
                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
            Else
                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                objrep.PrintToPrinter(2, False, 1, 1)
            End If
        End If
    End Sub

    Private Shared Function ObtenerMesLiberal(mes As Integer) As String
        Dim mesl As String = ""
        If mes = 1 Then
            mesl = "Enero"
        End If
        If mes = 2 Then
            mesl = "Febrero"
        End If
        If mes = 3 Then
            mesl = "Marzo"
        End If
        If mes = 4 Then
            mesl = "Abril"
        End If
        If mes = 5 Then
            mesl = "Mayo"
        End If
        If mes = 6 Then
            mesl = "Junio"
        End If
        If mes = 7 Then
            mesl = "Julio"
        End If
        If mes = 8 Then
            mesl = "Agosto"
        End If
        If mes = 9 Then
            mesl = "Septiembre"
        End If
        If mes = 10 Then
            mesl = "Octubre"
        End If
        If mes = 11 Then
            mesl = "Noviembre"
        End If
        If mes = 12 Then
            mesl = "Diciembre"
        End If

        Return mesl
    End Function

    Private Sub ReporteNotaVenta2(idPedido As String, _Ds2 As DataSet, _Ds3 As DataSet, _Literal As String, listResult As List(Of RDespachoNotaVenta))
        P_Global.Visualizador = New Visualizador
        Dim objrep As New NotaVenta2
        Dim dia, mes, ano As Integer
        Dim Fecliteral, mesl As String
        'Fecliteral = _Ds.Tables(0).Rows(0).Item("fvafec").ToString
        Fecliteral = listResult.Item(0).oafdoc
        dia = Microsoft.VisualBasic.Left(Fecliteral, 2)
        mes = Microsoft.VisualBasic.Mid(Fecliteral, 4, 2)
        ano = Microsoft.VisualBasic.Mid(Fecliteral, 7, 4)
        mesl = ObtenerMesLiberal(mes)

        Fecliteral = _Ds2.Tables(0).Rows(0).Item("scciu").ToString + " " + dia.ToString + " de " + mesl + " del " + ano.ToString
        objrep.SetDataSource(listResult)
        objrep.SetParameterValue("Literal", _Literal)
        objrep.SetParameterValue("Fechali", Fecliteral)
        objrep.SetParameterValue("nombreUsuario", P_Global.gs_user)
        objrep.SetParameterValue("Empresa", _Ds2.Tables(0).Rows(0).Item("scneg").ToString)

        If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
            P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
            P_Global.Visualizador.ShowDialog() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar
        Else
            Dim pd As New PrintDocument()
            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
            If (Not pd.PrinterSettings.IsValid) Then
                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
            Else
                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                objrep.PrintToPrinter(1, False, 1, 1)
            End If
        End If
    End Sub

    Private Sub ReporteNotaVenta3(idPedido As String, _Ds2 As DataSet, _Ds3 As DataSet, _Literal As String, listResult As List(Of RDespachoNotaVenta))
        P_Global.Visualizador = New Visualizador
        Dim objrep As New NotaVenta3
        Dim dia, mes, ano As Integer
        Dim Fecliteral, mesl As String
        'Fecliteral = _Ds.Tables(0).Rows(0).Item("fvafec").ToString
        Fecliteral = listResult.Item(0).oafdoc
        dia = Microsoft.VisualBasic.Left(Fecliteral, 2)
        mes = Microsoft.VisualBasic.Mid(Fecliteral, 4, 2)
        ano = Microsoft.VisualBasic.Mid(Fecliteral, 7, 4)
        mesl = ObtenerMesLiberal(mes)

        Fecliteral = _Ds2.Tables(0).Rows(0).Item("scciu").ToString + " " + dia.ToString + " de " + mesl + " del " + ano.ToString
        Dim tipoZona As String = L_fnVerificarZona("oanumi =" + idPedido)
        Dim esZonaLaPaz = IIf(tipoZona = "ES LA PAZ", "*", "")
        Dim esZonaElAlto = IIf(tipoZona = "ES EL ALTO", "*", "")


        objrep.Subreports.Item("NotaVenta3.rpt").SetDataSource(listResult)
        objrep.SetDataSource(listResult)
        'objrep.SetParameterValue("Literal", _Literal)
        'objrep.SetParameterValue("Fechali", Fecliteral)

        objrep.SetParameterValue("tipoZonaLaPaz", esZonaLaPaz)
        objrep.SetParameterValue("tipoZonaElAlto", esZonaElAlto)
        objrep.SetParameterValue("Telefono", _Ds2.Tables(0).Rows(0).Item("sctelf").ToString)
        objrep.SetParameterValue("Empresa", _Ds2.Tables(0).Rows(0).Item("scneg").ToString)
        objrep.SetParameterValue("Logo", gb_ubilogo)

        objrep.SetParameterValue("tipoZonaLaPaz", esZonaLaPaz, "NotaVenta3.rpt")
        objrep.SetParameterValue("tipoZonaElAlto", esZonaElAlto, "NotaVenta3.rpt")
        objrep.SetParameterValue("Telefono", _Ds2.Tables(0).Rows(0).Item("sctelf").ToString, "NotaVenta3.rpt")
        objrep.SetParameterValue("Empresa", _Ds2.Tables(0).Rows(0).Item("scneg").ToString, "NotaVenta3.rpt")
        objrep.SetParameterValue("Logo", gb_ubilogo, "NotaVenta3.rpt")
        If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
            P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
            P_Global.Visualizador.ShowDialog() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar
        Else
            Dim pd As New PrintDocument()
            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
            If (Not pd.PrinterSettings.IsValid) Then
                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
            Else
                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                objrep.PrintToPrinter(1, False, 1, 1)
            End If
        End If

    End Sub

    Private Sub ReporteNotaVenta4(idPedido As String, _Ds2 As DataSet, _Ds3 As DataSet, _Literal As String, listResult As List(Of RDespachoNotaVenta))
        P_Global.Visualizador = New Visualizador
        Dim objrep As New NotaVenta4
        Dim dia, mes, ano As Integer
        Dim Fecliteral, mesl As String
        'Fecliteral = _Ds.Tables(0).Rows(0).Item("fvafec").ToString
        Fecliteral = listResult.Item(0).oafdoc
        dia = Microsoft.VisualBasic.Left(Fecliteral, 2)
        mes = Microsoft.VisualBasic.Mid(Fecliteral, 4, 2)
        ano = Microsoft.VisualBasic.Mid(Fecliteral, 7, 4)
        mesl = ObtenerMesLiberal(mes)

        Fecliteral = _Ds2.Tables(0).Rows(0).Item("scciu").ToString + " " + dia.ToString + " de " + mesl + " del " + ano.ToString
        ' objrep.Subreports.Item("NotaVenta4.rpt").SetDataSource(listResult)
        objrep.SetDataSource(listResult)
        objrep.SetParameterValue("Telefono", _Ds2.Tables(0).Rows(0).Item("sctelf").ToString)
        objrep.SetParameterValue("Empresa", _Ds2.Tables(0).Rows(0).Item("scneg").ToString)
        objrep.SetParameterValue("Logo", gb_ubilogo)

        'objrep.SetParameterValue("Telefono", _Ds2.Tables(0).Rows(0).Item("sctelf").ToString, "NotaVenta4.rpt")
        'objrep.SetParameterValue("Empresa", _Ds2.Tables(0).Rows(0).Item("scneg").ToString, "NotaVenta4.rpt")
        'objrep.SetParameterValue("Logo", gb_ubilogo, "NotaVenta4.rpt")

        If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
            P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
            P_Global.Visualizador.ShowDialog() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar
        Else
            Dim pd As New PrintDocument()
            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
            If (Not pd.PrinterSettings.IsValid) Then
                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
            Else
                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                objrep.PrintToPrinter(1, False, 1, 1)
            End If
        End If

    End Sub
    Private Sub ReporteNotaVenta5(idPedido As String, _Ds2 As DataSet, _Ds3 As DataSet, _Literal As String, listResult As List(Of RDespachoNotaVenta))
        P_Global.Visualizador = New Visualizador
        Dim objrep As New NotaVenta5
        Dim dia, mes, ano As Integer
        Dim Fecliteral, mesl As String
        'Fecliteral = _Ds.Tables(0).Rows(0).Item("fvafec").ToString
        Fecliteral = listResult.Item(0).oafdoc
        dia = Microsoft.VisualBasic.Left(Fecliteral, 2)
        mes = Microsoft.VisualBasic.Mid(Fecliteral, 4, 2)
        ano = Microsoft.VisualBasic.Mid(Fecliteral, 7, 4)

        mesl = ObtenerMesLiberal(mes)

        Fecliteral = _Ds2.Tables(0).Rows(0).Item("scciu").ToString + " " + dia.ToString + " de " + mesl + " del " + ano.ToString
        objrep.SetDataSource(listResult)
        objrep.SetParameterValue("Literal", _Literal)
        objrep.SetParameterValue("Fechali", Fecliteral)
        objrep.SetParameterValue("nombreUsuario", P_Global.gs_user)
        objrep.SetParameterValue("Empresa", _Ds2.Tables(0).Rows(0).Item("scneg").ToString)
        objrep.SetParameterValue("Telefono", _Ds2.Tables(0).Rows(0).Item("sctelf").ToString)
        objrep.SetParameterValue("Logo", gb_ubilogo)

        If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
            P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
            P_Global.Visualizador.ShowDialog() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar
        Else
            Dim pd As New PrintDocument()
            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
            If (Not pd.PrinterSettings.IsValid) Then
                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
            Else
                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                objrep.PrintToPrinter(2, False, 1, 1)
            End If
        End If

    End Sub
    Private Sub ReporteNotaVenta7(idPedido As String, _Ds2 As DataSet, _Ds3 As DataSet, _Literal As String, listResult As List(Of RDespachoNotaVenta))
        P_Global.Visualizador = New Visualizador
        Dim objrep As New NotaVenta7
        Dim dia, mes, ano As Integer
        Dim Fecliteral, mesl As String

        Fecliteral = listResult.Item(0).oafdoc
        dia = Microsoft.VisualBasic.Left(Fecliteral, 2)
        mes = Microsoft.VisualBasic.Mid(Fecliteral, 4, 2)
        ano = Microsoft.VisualBasic.Mid(Fecliteral, 7, 4)
        mesl = ObtenerMesLiberal(mes)
        Fecliteral = _Ds2.Tables(0).Rows(0).Item("scciu").ToString + " " + dia.ToString + " de " + mesl + " del " + ano.ToString

        objrep.SetDataSource(listResult)
        objrep.SetParameterValue("Literal", _Literal)
        objrep.SetParameterValue("Fechali", Fecliteral)
        objrep.SetParameterValue("nombreUsuario", P_Global.gs_user)
        objrep.SetParameterValue("Empresa", _Ds2.Tables(0).Rows(0).Item("scneg").ToString)

        If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
            P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
            P_Global.Visualizador.ShowDialog() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar
        Else
            Dim pd As New PrintDocument()
            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
            If (Not pd.PrinterSettings.IsValid) Then
                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
            Else
                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                objrep.PrintToPrinter(2, False, 1, 1)
            End If
        End If
    End Sub
    Private Sub ReporteNotaVenta6(idPedido As String, _Ds2 As DataSet, _Ds3 As DataSet, _Literal As String, listResult As List(Of RDespachoNotaVenta))
        P_Global.Visualizador = New Visualizador
        Dim objrep As New NotaVenta_Ticket
        Dim zona, repartidor, vendedor As String

        Dim tZonaRepartidorVendedor As DataTable = L_fnObtenerZonaRepartidorDistribuidor("oanumi =" + idPedido)
        If tZonaRepartidorVendedor.Rows.Count() > 0 Then
            zona = tZonaRepartidorVendedor.Rows(0).Item("zona").ToString()
            repartidor = tZonaRepartidorVendedor.Rows(0).Item("repartidor").ToString()
            vendedor = tZonaRepartidorVendedor.Rows(0).Item("vendedor").ToString()
        Else
            zona = "--"
            repartidor = "--"
            vendedor = "--"
        End If
        objrep.SetDataSource(listResult)
        objrep.SetParameterValue("zona", zona)
        objrep.SetParameterValue("repartidor", repartidor)
        objrep.SetParameterValue("vendedor", vendedor)
        objrep.SetParameterValue("Logo", gb_ubilogo)

        If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
            P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
            P_Global.Visualizador.ShowDialog() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar
        Else
            Dim pd As New PrintDocument()
            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
            If (Not pd.PrinterSettings.IsValid) Then
                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
            Else
                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                Dim nrocopias As Integer = _Ds3.Tables(0).Rows(0).Item("cbnrocopias")
                objrep.PrintToPrinter(nrocopias, False, 1, 1)
            End If
        End If
    End Sub

    Private Sub ReporteNotaVenta8(idPedido As String, _Ds2 As DataSet, _Ds3 As DataSet, _Literal As String, listResult As List(Of RDespachoNotaVenta))
        P_Global.Visualizador = New Visualizador
        Dim objrep As New NotaVenta8
        Dim dia, mes, ano As Integer
        Dim Fecliteral, mesl As String

        Fecliteral = listResult.Item(0).oafdoc
        dia = Microsoft.VisualBasic.Left(Fecliteral, 2)
        mes = Microsoft.VisualBasic.Mid(Fecliteral, 4, 2)
        ano = Microsoft.VisualBasic.Mid(Fecliteral, 7, 4)
        mesl = ObtenerMesLiberal(mes)

        Fecliteral = _Ds2.Tables(0).Rows(0).Item("scciu").ToString + " " + dia.ToString + " de " + mesl + " del " + ano.ToString
        objrep.SetDataSource(listResult)
        objrep.SetParameterValue("Telefono", _Ds2.Tables(0).Rows(0).Item("sctelf").ToString)
        objrep.SetParameterValue("Empresa", _Ds2.Tables(0).Rows(0).Item("scneg").ToString)
        objrep.SetParameterValue("Logo", gb_ubilogo)

        If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
            P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
            P_Global.Visualizador.ShowDialog() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar
        Else
            Dim pd As New PrintDocument()
            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
            If (Not pd.PrinterSettings.IsValid) Then
                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
            Else
                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                objrep.PrintToPrinter(1, False, 1, 1)
            End If
        End If
    End Sub

    Private Sub ReporteNotaVenta9(idPedido As String, _Ds2 As DataSet, _Ds3 As DataSet, _Literal As String, listResult As List(Of RDespachoNotaVenta))
        P_Global.Visualizador = New Visualizador
        Dim objrep As New NotaVenta9
        Dim dia, mes, ano As Integer
        Dim Fecliteral, mesl As String

        Fecliteral = listResult.Item(0).oafdoc
        dia = Microsoft.VisualBasic.Left(Fecliteral, 2)
        mes = Microsoft.VisualBasic.Mid(Fecliteral, 4, 2)
        ano = Microsoft.VisualBasic.Mid(Fecliteral, 7, 4)
        mesl = ObtenerMesLiberal(mes)

        Fecliteral = _Ds2.Tables(0).Rows(0).Item("scciu").ToString + " " + dia.ToString + " de " + mesl + " del " + ano.ToString
        objrep.SetDataSource(listResult)
        objrep.SetParameterValue("Telefono", _Ds2.Tables(0).Rows(0).Item("sctelf").ToString)
        objrep.SetParameterValue("Empresa", _Ds2.Tables(0).Rows(0).Item("scneg").ToString)
        objrep.SetParameterValue("Logo", gb_ubilogo)

        If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
            P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
            P_Global.Visualizador.ShowDialog() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar
        Else
            Dim pd As New PrintDocument()
            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
            If (Not pd.PrinterSettings.IsValid) Then
                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
            Else
                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                objrep.PrintToPrinter(1, False, 1, 1)
            End If
        End If
    End Sub
    Private Sub ReporteNotaVenta10(idPedido As String, _Ds2 As DataSet, _Ds3 As DataSet, _Literal As String, listResult As List(Of RDespachoNotaVenta), nomVendedor As String)
        P_Global.Visualizador = New Visualizador
        Dim objrep As New NotaVenta10
        Dim dia, mes, ano As Integer
        Dim Fecliteral, mesl As String

        Fecliteral = listResult.Item(0).oafdoc
        dia = Microsoft.VisualBasic.Left(Fecliteral, 2)
        mes = Microsoft.VisualBasic.Mid(Fecliteral, 4, 2)
        ano = Microsoft.VisualBasic.Mid(Fecliteral, 7, 4)
        mesl = ObtenerMesLiberal(mes)

        Fecliteral = _Ds2.Tables(0).Rows(0).Item("scciu").ToString + " " + dia.ToString + " de " + mesl + " del " + ano.ToString
        objrep.SetDataSource(listResult)
        objrep.SetParameterValue("Telefono", _Ds2.Tables(0).Rows(0).Item("sctelf").ToString)
        objrep.SetParameterValue("Direccion", _Ds2.Tables(0).Rows(0).Item("scdir").ToString)
        objrep.SetParameterValue("Ciudad", _Ds2.Tables(0).Rows(0).Item("scciu").ToString)
        objrep.SetParameterValue("Empresa", gs_empresaDescSistema)
        objrep.SetParameterValue("idPedido", idPedido)
        objrep.SetParameterValue("Logo", gb_ubilogo)
        objrep.SetParameterValue("vendedor", nomVendedor)


        If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
            P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
            P_Global.Visualizador.ShowDialog() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar
        Else
            Dim pd As New PrintDocument()
            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
            If (Not pd.PrinterSettings.IsValid) Then
                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
            Else
                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                objrep.PrintToPrinter(1, False, 1, 1)
            End If
        End If
    End Sub
    Private Sub ReporteNotaVenta11(idPedido As String, _Ds2 As DataSet, _Ds3 As DataSet, _Literal As String, listResult As List(Of RDespachoNotaVenta), nomVendedor As String)
        P_Global.Visualizador = New Visualizador
        Dim objrep As New NotaVenta11
        Dim dia, mes, ano As Integer
        Dim Fecliteral, mesl As String

        Fecliteral = listResult.Item(0).oafdoc
        dia = Microsoft.VisualBasic.Left(Fecliteral, 2)
        mes = Microsoft.VisualBasic.Mid(Fecliteral, 4, 2)
        ano = Microsoft.VisualBasic.Mid(Fecliteral, 7, 4)
        mesl = ObtenerMesLiberal(mes)

        Fecliteral = _Ds2.Tables(0).Rows(0).Item("scciu").ToString + " " + dia.ToString + " de " + mesl + " del " + ano.ToString
        objrep.SetDataSource(listResult)
        objrep.SetParameterValue("Telefono", _Ds2.Tables(0).Rows(0).Item("sctelf").ToString)
        objrep.SetParameterValue("Direccion", _Ds2.Tables(0).Rows(0).Item("scdir").ToString)
        objrep.SetParameterValue("Ciudad", _Ds2.Tables(0).Rows(0).Item("scciu").ToString)
        objrep.SetParameterValue("Empresa", gs_empresaDescSistema)
        objrep.SetParameterValue("idPedido", idPedido)
        objrep.SetParameterValue("vendedor", nomVendedor)


        If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
            P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
            P_Global.Visualizador.ShowDialog() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar
        Else
            Dim pd As New PrintDocument()
            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
            If (Not pd.PrinterSettings.IsValid) Then
                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
            Else
                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                objrep.PrintToPrinter(1, False, 1, 1)
            End If
        End If
    End Sub

    Private Sub ReporteNotaVenta12(idPedido As String, _Ds2 As DataSet, _Ds3 As DataSet, _Literal As String, listResult As List(Of RDespachoNotaVenta), nomVendedor As String)
        P_Global.Visualizador = New Visualizador
        Dim objrep As New NotaVenta12
        Dim dia, mes, ano As Integer
        Dim Fecliteral, mesl As String

        Fecliteral = listResult.Item(0).oafdoc
        dia = Microsoft.VisualBasic.Left(Fecliteral, 2)
        mes = Microsoft.VisualBasic.Mid(Fecliteral, 4, 2)
        ano = Microsoft.VisualBasic.Mid(Fecliteral, 7, 4)
        mesl = ObtenerMesLiberal(mes)

        Fecliteral = _Ds2.Tables(0).Rows(0).Item("scciu").ToString + " " + dia.ToString + " de " + mesl + " del " + ano.ToString
        objrep.SetDataSource(listResult)
        objrep.SetParameterValue("Telefono", _Ds2.Tables(0).Rows(0).Item("sctelf").ToString)
        objrep.SetParameterValue("Direccion", _Ds2.Tables(0).Rows(0).Item("scdir").ToString)
        objrep.SetParameterValue("Ciudad", _Ds2.Tables(0).Rows(0).Item("scciu").ToString)
        objrep.SetParameterValue("Empresa", gs_empresaDescSistema)
        objrep.SetParameterValue("idPedido", idPedido)
        objrep.SetParameterValue("Logo", gb_ubilogo)
        objrep.SetParameterValue("vendedor", nomVendedor)


        If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
            P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
            P_Global.Visualizador.ShowDialog() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar
        Else
            Dim pd As New PrintDocument()
            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
            If (Not pd.PrinterSettings.IsValid) Then
                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
            Else
                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                objrep.PrintToPrinter(1, False, 1, 1)
            End If
        End If
    End Sub


    Private Sub ReporteNotaVenta13(idPedido As String, _Ds2 As DataSet, _Ds3 As DataSet, _Literal As String, listResult As List(Of RDespachoNotaVenta), nomVendedor As String)
        P_Global.Visualizador = New Visualizador
        Dim objrep As New NotaVenta13
        Dim dia, mes, ano As Integer
        Dim Fecliteral, mesl As String

        Fecliteral = listResult.Item(0).oafdoc
        dia = Microsoft.VisualBasic.Left(Fecliteral, 2)
        mes = Microsoft.VisualBasic.Mid(Fecliteral, 4, 2)
        ano = Microsoft.VisualBasic.Mid(Fecliteral, 7, 4)
        mesl = ObtenerMesLiberal(mes)

        Fecliteral = _Ds2.Tables(0).Rows(0).Item("scciu").ToString + " " + dia.ToString + " de " + mesl + " del " + ano.ToString
        objrep.SetDataSource(listResult)
        objrep.SetParameterValue("Telefono", _Ds2.Tables(0).Rows(0).Item("sctelf").ToString)
        objrep.SetParameterValue("Direccion", _Ds2.Tables(0).Rows(0).Item("scdir").ToString)
        objrep.SetParameterValue("Ciudad", _Ds2.Tables(0).Rows(0).Item("scciu").ToString)
        objrep.SetParameterValue("Empresa", gs_empresaDescSistema)
        objrep.SetParameterValue("idPedido", idPedido)
        objrep.SetParameterValue("Logo", gb_ubilogo)
        objrep.SetParameterValue("vendedor", nomVendedor)


        If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
            P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
            P_Global.Visualizador.ShowDialog() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar
        Else
            Dim pd As New PrintDocument()
            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
            If (Not pd.PrinterSettings.IsValid) Then
                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
            Else
                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                objrep.PrintToPrinter(1, False, 1, 1)
            End If
        End If
    End Sub

    Public Function P_fnImageToByteArray(ByVal imageIn As Image) As Byte()
        Dim ms As New System.IO.MemoryStream()
        'imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg)
        Return ms.ToArray()
    End Function
    Private Function P_fnGrabarFacturarTFV001(numi As String, subtotal As Double, descuento As Double, total As Double, nit As String, nameCliente As String, Codcli As String) As Boolean
        Dim a As Double = subtotal
        Dim b As Double = CDbl(0) 'Ya esta calculado el 55% del ICE
        Dim c As Double = CDbl("0")
        Dim d As Double = CDbl("0")
        Dim e As Double = a - b - c - d
        Dim f As Double = descuento
        Dim g As Double = e - f
        Dim h As Double = g * (13 / 100)

        Dim res As Boolean = False
        'Grabado de Cabesera Factura
        L_Grabar_Factura(numi,
                        Now.Date.ToString("yyyy/MM/dd"), "0", "0",
                        "1",
                        nit,
                        Codcli,
                       nameCliente,
                        "",
                        CStr(Format(a, "####0.00")),
                        CStr(Format(b, "####0.00")),
                        CStr(Format(c, "####0.00")),
                        CStr(Format(d, "####0.00")),
                        CStr(Format(e, "####0.00")),
                        CStr(Format(f, "####0.00")),
                        CStr(Format(g, "####0.00")),
                        CStr(Format(h, "####0.00")),
                        "",
                        Now.Date.ToString("yyyy/MM/dd"),
                        "''",
                        "0",
                        numi)


        Dim dtDetalle As DataTable = L_prObtenerDetallePedido(numi)
        For i As Integer = 0 To dtDetalle.Rows.Count - 1 Step 1

            L_Grabar_Factura_Detalle(numi.ToString,
                                        dtDetalle.Rows(i).Item("obcprod").ToString,
                                         dtDetalle.Rows(i).Item("producto").ToString,
                                        dtDetalle.Rows(i).Item("obpcant").ToString,
                                        dtDetalle.Rows(i).Item("obpbase").ToString,
                                        numi)

        Next
        Return True
    End Function

    Private Sub btReporteDespachoCliente_Click(sender As Object, e As EventArgs)
        'Try
        '    Dim idChofer = Me.cbChoferes.Value
        '    If (Not IsNumeric(idChofer)) Then
        '        Throw New Exception("Debe seleccionar un chofer.")
        '    End If
        '    If (Convert.ToInt32(idChofer) = ENCombo.ID_SELECCIONAR) Then
        '        Throw New Exception("Debe seleccionar un chofer.")
        '    End If

        '    'Dim listResult = New LPedido().ListarDespachoXClienteDeChofer(idChofer, IIf(cbEstado.SelectedIndex = 0, ENEstadoPedido.DICTADO, ENEstadoPedido.ENTREGADO))
        '    'Dim lista = (From a In listResult
        '    '             Where a.oafdoc >= Tb_Fecha.Value And
        '    '                    a.oafdoc <= Tb_FechaHasta.Value
        '    '             Order By a.oanumi Ascending).ToList
        '    Dim dt As DataTable = ListarDespachoXcLIENTE(idChofer, IIf(cbEstado.SelectedIndex = 0, ENEstadoPedido.DICTADO, ENEstadoPedido.ENTREGADO), Tb_Fecha.Value.ToString("dd/MM/yyyy"), Tb_FechaHasta.Value.ToString("dd/MM/yyyy"))

        '    If (dt.Rows.Count = 0) Then
        '        Throw New Exception("No hay registros para generar el reporte.")
        '    End If

        '    If Not IsNothing(P_Global.Visualizador) Then
        '        P_Global.Visualizador.Close()
        '    End If

        '    P_Global.Visualizador = New Visualizador
        '    Dim objrep As New DespachoXCliente

        '    objrep.SetDataSource(dt)
        '    objrep.SetParameterValue("nroDespacho", String.Empty)
        '    objrep.SetParameterValue("nombreDistribuidor", cbChoferes.Text)
        '    objrep.SetParameterValue("FechaDocumento", Tb_Fecha.Value)
        '    objrep.SetParameterValue("nombreUsuario", P_Global.gs_user)
        '    objrep.SetParameterValue("moneda", gs_Mon)

        '    P_Global.Visualizador.CRV1.ReportSource = objrep
        '    P_Global.Visualizador.Show()
        '    P_Global.Visualizador.BringToFront()
        'Catch ex As Exception
        '    MostrarMensajeError(ex.Message)
        'End Try
    End Sub

    Private Sub btReporteDespachoLinea_Click(sender As Object, e As EventArgs)
        'Try
        '    Dim idChofer = Me.cbChoferes.Value
        '    If (Not IsNumeric(idChofer)) Then
        '        Throw New Exception("Debe seleccionar un chofer.")
        '    End If
        '    If (Convert.ToInt32(idChofer) = ENCombo.ID_SELECCIONAR) Then
        '        Throw New Exception("Debe seleccionar un chofer.")
        '    End If

        '    Dim listResult = New LPedido().ListarDespachoXProductoDeChofer(idChofer, IIf(cbEstado.SelectedIndex = 0, ENEstadoPedido.DICTADO, ENEstadoPedido.ENTREGADO), Tb_Fecha.Value, Tb_FechaHasta.Value)
        '    Dim lista = (From a In listResult
        '                 Group a By a.canumi, a.cadesc, a.categoria Into grupo = Group
        '                 Select New RDespachoXProducto With {
        '                  .canumi = grupo.FirstOrDefault().canumi,
        '                  .cacod = grupo.FirstOrDefault().cacod,
        '                  .cadesc = grupo.FirstOrDefault().cadesc,
        '                  .categoria = grupo.FirstOrDefault().categoria,
        '                  .obpcant = grupo.Sum(Function(item) item.obpcant),
        '                  .Caja = grupo.Sum(Function(item) item.Caja),
        '                  .Unidad = grupo.Sum(Function(item) item.Unidad),
        '                  .Total = grupo.Sum(Function(item) item.Total),
        '                  .Conv = grupo.FirstOrDefault().Conv,
        '                  .Pesokg = grupo.Sum(Function(item) item.Pesokg)
        '                }).ToList()
        '    If (lista.Count = 0) Then
        '        Throw New Exception("No hay registros para generar el reporte.")
        '    End If
        '    Dim empresaId = ObtenerEmpresaHabilitada()
        '    Dim empresaHabilitada As DataTable = ObtenerEmpresaTipoReporte(empresaId, Convert.ToInt32(ENReporte.DESPACHOXPRODUCTO))
        '    For Each fila As DataRow In empresaHabilitada.Rows
        '        Select Case fila.Item("TipoReporte").ToString
        '            Case ENReporteTipo.DESPACHOXPRODUCTO_AgrupadoXCategoria
        '                Dim objrep As New DespachoXProducto
        '                SerParametros(lista, objrep)
        '            Case ENReporteTipo.DESPACHOXPRODUCTO_SinAgrupacion
        '                Dim objrep As New DespachoXProductoSinAgrupacion
        '                SerParametros(lista, objrep)
        '        End Select
        '    Next
        'Catch ex As Exception
        '    MostrarMensajeError(ex.Message)
        'End Try
    End Sub

    Private Sub SerParametros(listResult As List(Of RDespachoXProducto), objrep As Object)
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If
        P_Global.Visualizador = New Visualizador
        objrep.SetDataSource(listResult)
        objrep.SetParameterValue("nroDespacho", String.Empty)
        objrep.SetParameterValue("nombreDistribuidor", cbChoferes.Text)
        objrep.SetParameterValue("FechaDocumento", Tb_Fecha.Value)
        objrep.SetParameterValue("nombreUsuario", P_Global.gs_user)
        P_Global.Visualizador.CRV1.ReportSource = objrep
        P_Global.Visualizador.ShowDialog()
        P_Global.Visualizador.BringToFront()
    End Sub


    Private Sub Tb_Fecha_ValueChanged(sender As Object, e As EventArgs) Handles Tb_Fecha.ValueChanged
        Try
            If (_cargaCompleta) Then
                CargarPedidos()


            End If
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
    End Sub
#End Region

#Region "Privado, metodos y funciones"
    Private Sub Init()
        Try

            'L_prJobDuplicados()

            CargarCamiones()
            CargarcComboCamion()
            ConfigForm()
            CargarZona()
            CargarChoferes()
            tbFecha.Value = DateTime.Today
            tbHoraS.Value = DateAndTime.TimeOfDay
            tbHoraL.Value = DateAndTime.TimeOfDay
            Dim cod As Integer = cbCamion.Value
            Tb_FechaHasta.Value = DateTime.Today
            _cargaCompleta = True
            cbEstado.SelectedIndex = 0
            CargarHojaRuta()
            CargarHojaPedidosPendientes(grPendientes, 0, CType(grPedidos4.DataSource, DataTable))
            InHabilitar()
            CargarVistasSalidas()
            MSuperTabControlPrincipal.SelectedTabIndex = 0
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
    End Sub

    Private Sub CargarCamiones()
        Dim panel As GridPanel = grCamiones.PrimaryGrid

        InitPanel2(panel)
    End Sub

    Private Sub InitPanel2(ByVal panel As GridPanel)
        panel.CheckBoxes = False
        panel.ShowCheckBox = False
        panel.ShowTreeButtons = False
        panel.ShowTreeLines = False
        panel.ShowRowGridIndex = False

        panel.RowHeaderWidth = 40
        panel.DefaultRowHeight = 0
        panel.ColumnHeader.RowHeight = 30

        panel.DefaultVisualStyles.CellStyles.Default.Alignment = Alignment.Justified
        panel.DefaultVisualStyles.ColumnHeaderStyles.Default.Alignment = Alignment.Justified

        Dim dt As DataTable

        dt = L_prGeneralEstadoCamiones(Date.Now.ToString("dd/MM/yyyy"))

        panel.DataSource = dt

        grCamiones.PrimaryGrid.EnsureVisible(True)

        Dim panel1 As GridPanel = grCamiones.PrimaryGrid
        'grilla.PrimaryGrid.BoundMode = Janus.Data.BoundMode.Bound

        Dim columns As GridColumnCollection = panel.Columns

        columns("capacidad").Visible = True
        columns("capacidad").Width = 200
        columns("capacidad").HeaderText = "CAPACIDAD TOTAL"

        columns("OCUPADO").Visible = True
        columns("OCUPADO").Width = 300
        columns("OCUPADO").HeaderText = "CAPACIDAD DE CARGA ACTUALIZADA"

        columns("saldo").Visible = True
        columns("saldo").Width = 300
        columns("saldo").HeaderText = "CAPACIDAD DE CARGA DISPONIBLE"

        columns("progreso").Visible = True
        columns("progreso").HeaderText = "PROGRESO"

        columns("progreso").EditorType = GetType(GridProgressBarXEditControl)

        columns("tshoras").Visible = True
        columns("tshoras").HeaderText = "HORA SALIDA"

        columns("tshoral").Visible = True
        columns("tshoral").HeaderText = "HORA LLEGADA"

        columns("estado").Visible = True
        columns("estado").HeaderText = "ESTADO"

        aplicarEstilos()
        'Dim columna As PanelGridColumn = panelGrid1.Columns(0)

        '' Cambiar el nombre de la columna
        'columna.HeaderText = "Nuevo Nombre"

        '' Refrescar el PanelGrid si es necesario
        'panelGrid1.Refresh()
    End Sub

    Private Sub aplicarEstilos()
        For Each fila As GridRow In grCamiones.PrimaryGrid.Rows
            If fila.Cells(5).Value = "EN RUTA" Then
                'fila.CellStyles.Default.Background.Color1 = Color.Yellow
                fila.CellStyles.Default.Background.Color2 = Color.Yellow
            End If
        Next
    End Sub
    Private Sub CargarHojaRuta()
        Dim dt As DataTable = CargarHojaRutaTodos()
        grBuscador.BoundMode = Janus.Data.BoundMode.Bound
        grBuscador.DataSource = dt
        grBuscador.RetrieveStructure()
        With grBuscador.RootTable.Columns("trnumi")
            .Width = 40
            .Caption = "CODIGO"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = True
        End With
        With grBuscador.RootTable.Columns("trzon")
            .Width = 40
            .Caption = "CODIGO"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = False
        End With
        With grBuscador.RootTable.Columns("cedesc")
            .Width = 200
            .Caption = "ZONA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = True
        End With
        With grBuscador.RootTable.Columns("trcodrep")
            .Width = 40
            .Caption = "CODIGO rep"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = False
        End With
        With grBuscador.RootTable.Columns("cbdesc")
            .Width = 200
            .Caption = "CHOFER"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = True
        End With
        With grBuscador.RootTable.Columns("trfdoc")
            .Width = 200
            .Caption = "FECHA"
            .HeaderStyle.BackColor = Color.Green
            .FormatString = "dd/MM/yyyy"
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = True
        End With

        With grBuscador

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

            '.TotalRow = InheritableBoolean.True
            '.TotalRowFormatStyle.BackColor = Color.Gold
            '.TotalRowPosition = TotalRowPosition.BottomFixed
        End With
    End Sub
    Private Sub Habilitar()
        cbZona.ReadOnly = False
        cbRepartidor.ReadOnly = False
        tbFecha.Enabled = True
        cbCamion.ReadOnly = False
        MBtNuevo.Enabled = False
        MBtModificar.Enabled = False
        MBtGrabar.Enabled = True
        btCargarHoja.Enabled = False
        btAddTarea.Enabled = True
        btVisualizar.Enabled = False
        MBtImprimir.Enabled = False
        btCerrarHoja.Enabled = False
        tbHoraS.Enabled = True
        tbHoraL.Enabled = True
        tbHoraS2.Enabled = True
        tbHoraL2.Enabled = True
        tbHoraS3.Enabled = True
        tbHoraL3.Enabled = True
        tbHoraS4.Enabled = True
        tbHoraL4.Enabled = True

        tbPesTot.Visible = True
        tbPesTot2.Visible = True
        LabelX12.Visible = True
        LabelX14.Visible = True
        LabelX25.Visible = True
        LabelX28.Visible = True
        MPanelToolBarNavegacion.Enabled = False

        BUSCADOR.Visible = False
        PENDIENTES.Visible = False


        If grPedidos.DataSource IsNot Nothing Then
            grPedidos.RootTable.Columns("check1").Visible = True
        End If
    End Sub
    Private Sub InHabilitar()
        cbZona.ReadOnly = True
        cbRepartidor.ReadOnly = True
        tbFecha.Enabled = False
        cbCamion.ReadOnly = True
        MBtNuevo.Enabled = True
        MBtModificar.Enabled = True
        MBtGrabar.Enabled = False
        btCargarHoja.Enabled = True
        btAddTarea.Enabled = False
        btVisualizar.Enabled = True
        MBtImprimir.Enabled = True
        btCerrarHoja.Enabled = True
        tbHoraS.Enabled = False
        tbHoraL.Enabled = False
        tbHoraS2.Enabled = False
        tbHoraL2.Enabled = False
        tbHoraS3.Enabled = False
        tbHoraL3.Enabled = False
        tbHoraS4.Enabled = False
        tbHoraL4.Enabled = False


        BUSCADOR.Visible = True
        PENDIENTES.Visible = True

        Dim salida As Integer = 0
        Dim grilla As GridEX
        If SuperTabControl1.SelectedTab Is tbSalida1 Then
            grilla = grPedidos
            salida = 1
        ElseIf SuperTabControl1.SelectedTab Is tbSalida2 Then
            grilla = grPedidos2
            salida = 2
        ElseIf SuperTabControl1.SelectedTab Is tbSalida3 Then
            grilla = grPedidos3
            salida = 3
        ElseIf SuperTabControl1.SelectedTab Is tbSalida4 Then
            grilla = grPedidos4
            salida = 4
        End If

        If grilla.DataSource IsNot Nothing Then
            grilla.RootTable.Columns("check1").Visible = False
        End If

        'tbPesTot.Visible = False
        'tbPesTot2.Visible = False
        'LabelX12.Visible = False
        'LabelX14.Visible = False
        'LabelX25.Visible = False
        'LabelX28.Visible = False
        MPanelToolBarNavegacion.Enabled = True
    End Sub
    Private Sub CargarZona()
        Dim dt As DataTable = L_fnObtenerLibreria("2", " 1= 1 ")
        With cbZona.DropDownList
            .Columns.Clear()

            .Columns.Add("cod").Width = 30
            .Columns("cod").Caption = "Id"
            .Columns("cod").Visible = True

            .Columns.Add("desc").Width = 180
            .Columns("desc").Caption = "Zona"
            .Columns("desc").Visible = True

            .ValueMember = "cod"
            .DisplayMember = "desc"
            .DataSource = dt

            .AlternatingColors = True
            .AllowColumnDrag = False
            .AutomaticSort = False
            .Refresh()
        End With
        cbZona.VisualStyle = VisualStyle.Office2007

        cbZona.SelectedIndex = 0
    End Sub

    Private Sub ConfigForm()
        Try
            Me.Text = "HOJA DE RUTA"
            ' Me.WindowState = FormWindowState.Maximized
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

    Private Sub CargarVistasSalidas()
        If CType(grPedidos2.DataSource, DataTable) IsNot Nothing Then
            If CType(grPedidos2.DataSource, DataTable).Rows.Count > 0 Then
                If Grilla2 = 0 Then
                    Grilla2 = 1
                End If
                tbSalida2.Visible = True
            Else
                tbSalida2.Visible = False
            End If
        End If
        If CType(grPedidos3.DataSource, DataTable) IsNot Nothing Then
            If CType(grPedidos3.DataSource, DataTable).Rows.Count > 0 Then
                If Grilla3 = 0 Then
                    Grilla3 = 1
                End If
                tbSalida3.Visible = True
            Else
                tbSalida3.Visible = False
            End If
        End If
        If CType(grPedidos4.DataSource, DataTable) IsNot Nothing Then
            If CType(grPedidos4.DataSource, DataTable).Rows.Count > 0 Then
                tbSalida4.Visible = True
            Else
                tbSalida4.Visible = False
            End If
        End If
    End Sub
    Private Sub CargarChoferes()
        Try
            Dim listResult As List(Of VCombo) = New LPersonal().ListarRepatidorCombo()

            With cbRepartidor.DropDownList
                .Columns.Clear()

                .Columns.Add("Id").Width = 30
                .Columns("Id").Caption = "Id"
                .Columns("Id").Visible = True

                .Columns.Add("Descripcion").Width = 180
                .Columns("Descripcion").Caption = "Nombre repartidor"
                .Columns("Descripcion").Visible = True

                .ValueMember = "Id"
                .DisplayMember = "Descripcion"
                .DataSource = listResult

                .AlternatingColors = True
                .AllowColumnDrag = False
                .AutomaticSort = False
                .Refresh()
            End With
            cbRepartidor.VisualStyle = VisualStyle.Office2007

            'cbRepartidor.SelectedIndex = 0
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

    Private Sub CargarcComboCamion()
        Try
            Dim dt As DataTable = L_prGeneralCamiones()

            With cbCamion.DropDownList
                .Columns.Add(dt.Columns(0).ToString).Width = 50
                .Columns(0).Caption = "Código"

                .Columns.Add(dt.Columns(1).ToString).Width = 180
                .Columns(1).Caption = "Nombre Camion"


            End With

            cbCamion.ValueMember = dt.Columns(0).ToString
            cbCamion.DisplayMember = dt.Columns(1).ToString
            cbCamion.DataSource = dt
            cbCamion.Refresh()

            cbCamion.VisualStyle = VisualStyle.Office2007

            cbCamion.SelectedIndex = 0
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

    Private Sub CargarPedidos()
        Try
            Dim lista As List(Of VPedido_BillingDispatch) = ObtenerListaPedido()

            '_prCargarIconPagar(lista)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

    Private Function ObtenerListaPedido() As List(Of VPedido_BillingDispatch)
        'Dim idChofer = Me.cbChoferes.Value
        'If (Not IsNumeric(idChofer)) Then
        '    Throw New Exception("Debe seleccionar un chofer.")
        'End If
        'If (Convert.ToInt32(idChofer) = ENCombo.ID_SELECCIONAR) Then
        '    Throw New Exception("Debe seleccionar un chofer.")
        'End If

        'Dim listResult = New LPedido().ListarPedidoAsignadoAChoferFechas(idChofer, IIf(cbEstado.SelectedIndex = 0, ENEstadoPedido.DICTADO, ENEstadoPedido.ENTREGADO), Tb_Fecha.Value, Tb_FechaHasta.Value)
        ''Dim lista = (From a In listResult
        ''             Where a.Fecha >= Tb_Fecha.Value And
        ''                   a.Fecha <= Tb_FechaHasta.Value).ToList
        'Return listResult
    End Function



    'Public Sub _prCargarIconPagar(lista As List(Of VPedido_BillingDispatch))
    '    Dim dt As DataTable = ConvertToDataTable(Of VPedido_BillingDispatch)(lista)
    '    Dim Bin As New MemoryStream
    '    Dim img As New Bitmap(My.Resources.cobro, 60, 28)
    '    img.Save(Bin, Imaging.ImageFormat.Png)
    '    'CType(dgjPedido.DataSource, DataTable).Rows(i).Item("check1") = Bin.GetBuffer

    '    For Each Row As GridEXRow In dgjPedido.GetRows
    '        Row.BeginEdit()
    '        Row.Cells("check1").Value = Bin.GetBuffer
    '        Row.EndEdit()
    '        'dgjPedido.RootTable.Columns("check1").Visible = True
    '        'dgjPedido.RootTable.Columns("check1").CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
    '    Next

    'End Sub

    Public Shared Function ConvertToDataTable(Of T)(ByVal list As IList(Of T)) As DataTable
        Dim td As New DataTable
        Dim entityType As Type = GetType(T)
        Dim properties As PropertyDescriptorCollection = TypeDescriptor.GetProperties(entityType)

        For Each prop As PropertyDescriptor In properties
            td.Columns.Add(prop.Name)
        Next

        For Each item As T In list
            Dim row As DataRow = td.NewRow()

            For Each prop As PropertyDescriptor In properties
                row(prop.Name) = prop.GetValue(item)
            Next

            td.Rows.Add(row)
        Next

        Return td
    End Function

    Private Sub MostrarMensajeError(mensaje As String)
        ToastNotification.Show(Me,
                               mensaje.ToUpper,
                               My.Resources.WARNING,
                               ENMensaje.MEDIANO,
                               eToastGlowColor.Red,
                               eToastPosition.TopCenter)
    End Sub
    Private Sub MostrarMensajeOk(mensaje As String)
        ToastNotification.Show(Me,
                               mensaje.ToUpper,
                               My.Resources.OK,
                               ENMensaje.MEDIANO,
                               eToastGlowColor.Green,
                               eToastPosition.TopCenter)
    End Sub
#End Region

#Region "Publico, metodos y funciones"
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

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

    Private Sub btReporteDespachoPedido_Click(sender As Object, e As EventArgs)
        'Try
        '    Dim idChofer = Me.cbChoferes.Value
        '    If (Not IsNumeric(idChofer)) Then
        '        Throw New Exception("Debe seleccionar un chofer.")
        '    End If
        '    If (Convert.ToInt32(idChofer) = ENCombo.ID_SELECCIONAR) Then
        '        Throw New Exception("Debe seleccionar un chofer.")
        '    End If

        '    'Dim listResult = New LPedido().ListarDespachoDetalleXChofer(idChofer, IIf(cbEstado.SelectedIndex = 0, ENEstadoPedido.DICTADO, ENEstadoPedido.ENTREGADO))
        '    'Dim lista = (From a In listResult
        '    '             Where a.oafdoc >= Tb_Fecha.Value And
        '    '                    a.oafdoc <= Tb_FechaHasta.Value).ToList
        '    Dim dt As DataTable = ListarDespachoXChofer(idChofer, IIf(cbEstado.SelectedIndex = 0, ENEstadoPedido.DICTADO, ENEstadoPedido.ENTREGADO), Tb_Fecha.Value.ToString("dd/MM/yyyy"), Tb_FechaHasta.Value.ToString("dd/MM/yyyy"))
        '    If (dt.Rows.Count = 0) Then
        '        Throw New Exception("No hay registros para generar el reporte.")
        '    End If

        '    If Not IsNothing(P_Global.Visualizador) Then
        '        P_Global.Visualizador.Close()
        '    End If

        '    P_Global.Visualizador = New Visualizador
        '    Dim objrep As New R_Ventasdespacho

        '    objrep.SetDataSource(dt)
        '    'objrep.SetParameterValue("nroDespacho", String.Empty)
        '    'objrep.SetParameterValue("nombreDistribuidor", cbChoferes.Text)
        '    'objrep.SetParameterValue("FechaDocumento", Tb_Fecha.Value)
        '    'objrep.SetParameterValue("nombreUsuario", P_Global.gs_user)

        '    P_Global.Visualizador.CRV1.ReportSource = objrep
        '    P_Global.Visualizador.Show()
        '    P_Global.Visualizador.BringToFront()
        'Catch ex As Exception
        '    MostrarMensajeError(ex.Message)
        'End Try
    End Sub

    Private Sub Tb_FechaHasta_ValueChanged(sender As Object, e As EventArgs) Handles Tb_FechaHasta.ValueChanged
        Try
            If (_cargaCompleta) Then
                CargarPedidos()

            End If
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
    End Sub



    Private Sub cbEstado_SelectedValueChanged(sender As Object, e As EventArgs) Handles cbEstado.SelectedValueChanged
        Try
            If cbEstado.SelectedIndex = 1 Then
                btCargarHoja.Enabled = False

            Else
                btCargarHoja.Enabled = True

            End If

            If (_cargaCompleta) Then
                CargarPedidos()


            End If
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
    End Sub



    '------------------ FACTURACION-----------------------------------------------------
    Private Sub TraerPDF(token As String, fact1 As Integer)
        Try
            Dim request = TryCast(System.Net.WebRequest.Create("https://contadores.sige.company/api/invoices/" + fact1.ToString + "/pdf?tpl=rollo"), System.Net.HttpWebRequest)

            request.Method = "GET"

            request.ContentType = "application/json"
            request.Headers.Add("authorization", "Bearer " + token)

            request.ContentLength = 0
            Dim responseContent As String
            Using response = TryCast(request.GetResponse(), System.Net.HttpWebResponse)
                Using reader = New System.IO.StreamReader(response.GetResponseStream())
                    responseContent = reader.ReadToEnd()
                    Dim result = JsonConvert.DeserializeObject(Of PDFResp)(responseContent)
                    LeerPDF(result.data.buffer)
                End Using
            End Using
        Catch ex As WebException
            If Not ex.Response Is Nothing Then
                Dim data As StreamReader = New StreamReader(ex.Response.GetResponseStream)
                'Al asignar el data.ReadToEnd al string se puede apreciar la respuesta del WebService en la variable str
                Dim str As String = data.ReadToEnd

            End If

        End Try
    End Sub

    Private Sub LeerPDF(report As String)
        Dim bytes As Byte() = Convert.FromBase64String(report)



        'Dim ruta As String = "C:\Disoft_Doc\Reporte\Fact" + fact.ToString + ".pdf"
        'Dim Stream As System.IO.FileStream = New FileStream(ruta, FileMode.CreateNew)
        'Dim writer As System.IO.BinaryWriter = New BinaryWriter(Stream)
        'writer.Write(bytes, 0, bytes.Length)
        'writer.Close()


        P_Global.Visualizador2 = New Visualizador2

        Dim tempFile As String = Path.GetTempFileName()
        File.WriteAllBytes(tempFile, bytes)

        ' Cargar el archivo PDF en el control AxAcroPDF


        ' Dim pdfFilePath As String = ruta
        P_Global.Visualizador2.AxAcroPDF1.LoadFile(tempFile) '(pdfFilePath)
        P_Global.Visualizador2.AxAcroPDF1.setZoom(100)
        P_Global.Visualizador2.Show()
        P_Global.Visualizador2.BringToFront()

    End Sub
    Private Sub crearCliente(Token As String)
        Dim request = TryCast(System.Net.WebRequest.Create("https://contadores.sige.company/api/customers"), System.Net.HttpWebRequest)

        request.Method = "POST"

        request.ContentType = "application/json"
        request.Headers.Add("authorization", "Bearer " + Token)

        Using writer As BinaryWriter = New BinaryWriter(request.GetRequestStream())
            Dim byteArray As Byte() = System.Text.Encoding.UTF8.GetBytes("{
              ""code"": ""6"",
              ""group_id"": -1,
              ""store_id"": 0,
              ""first_name"": ""Jose"",
              ""last_name"": ""Callisaya"",
              ""identity_document"": 00000123,
              ""company"": """",
              ""date_of_birth"": null,
              ""gender"": """",
              ""phone"": """",
              ""mobile"": """",
              ""fax"": """",
              ""email"": ""jose@dynasys.com.bo"",
              ""website"": """",
              ""address_1"": ""Direccion 01"",
              ""address_2"": """",
              ""zip_code"": """",
              ""city"": """",
              ""country"": ""Bolivia"",
              ""country_code"": ""BO"",
              ""meta"": {
                ""_nit_ruc_nif"": ""123456789"",
                ""_billing_name"": null
              }
            }")
            'request.ContentLength = byteArray.Length
            writer.Write(byteArray)
            writer.Close()
        End Using
        Dim responseContent As String
        Using response = TryCast(request.GetResponse(), System.Net.HttpWebResponse)
            Using reader = New System.IO.StreamReader(response.GetResponseStream())
                responseContent = reader.ReadToEnd()
            End Using
        End Using
    End Sub

    Private Sub crearFactura(token As String, pedido As Integer)
        Try

            Dim Emenvio = New EmisorEnvio.Emisor()
            ServicePointManager.Expect100Continue = True
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
            Dim request = TryCast(System.Net.WebRequest.Create("https://contadores.sige.company/api/invoices"), System.Net.HttpWebRequest)

            request.Method = "POST"
            Dim token2 As String = "Bearer " + token
            request.ContentType = "application/json"
            request.Headers.Add("authorization", token2)



            Using writer As BinaryWriter = New BinaryWriter(request.GetRequestStream())

                Dim cadena As String = "{
                  ""customer_id"": 1,
                  ""customer"": """ + razonsocial + """,
                  ""nit_ruc_nif"": """ + nit + """,
                  ""subtotal"": 1290.00,
                  ""total_tax"": 0.00,
                  ""discount"": ""0"",
                  ""monto_giftcard"": 0.00,
                  ""total"": 1190.00,
                  ""invoice_date_time"": """",
                  ""currency_code"": """",
                  ""codigo_sucursal"": 0,
                  ""punto_venta"": 0,
                  ""codigo_documento_sector"": 1,
                  ""tipo_documento_identidad"": " + tipoDoc.ToString + ",
                  ""codigo_metodo_pago"": 1,
                  ""codigo_moneda"": 1,
                  ""complemento"": null,
                  ""numero_tarjeta"": null,
                  ""tipo_cambio"": 1,
                  ""tipo_factura_documento"": 1,
                  ""items"": [
                  "

                cadena = cadena + "]}"
                Dim byteArray As Byte()


                byteArray = System.Text.Encoding.UTF8.GetBytes(cadena)
                'request.ContentLength = byteArray.Length
                Dim TxtEncodedValue As String = System.Text.Encoding.UTF8.GetString(byteArray)
                writer.Write(byteArray)
                writer.Close()
            End Using

            'request.d
            Dim responseContent As String
            Using response = TryCast(request.GetResponse(), System.Net.HttpWebResponse)
                Using reader = New System.IO.StreamReader(response.GetResponseStream())
                    responseContent = reader.ReadToEnd()
                    Dim result = JsonConvert.DeserializeObject(Of FactResp)(responseContent)
                    If result.code = 200 Then
                        fact = result.data.invoice_id
                        With result.data
                            Dim fec As String = .invoice_date_time.Substring(0, 10)
                            GrabarTFV001(pedido, fec, .invoice_number, .cuf, .nit_ruc_nif, .customer, .subtotal, .total, .control_code, .cufd, .leyenda, .nit_emisor.ToString, .print_url, .siat_id, .siat_url, .invoice_id)
                        End With
                    Else
                        ToastNotification.Show(Me,
                               result.response.ToUpper,
                               My.Resources.OK,
                               ENMensaje.MEDIANO,
                               eToastGlowColor.Green,
                               eToastPosition.TopCenter)
                    End If
                End Using
            End Using
        Catch ex As WebException
            If Not ex.Response Is Nothing Then
                Dim data As StreamReader = New StreamReader(ex.Response.GetResponseStream)
                'Al asignar el data.ReadToEnd al string se puede apreciar la respuesta del WebService en la variable str
                Dim str As String = data.ReadToEnd

            End If

        End Try
    End Sub

    Private Sub PanelSuperior_Paint(sender As Object, e As PaintEventArgs) Handles PanelSuperior.Paint

    End Sub



    Private Sub CargarHojaPedidosPendientes(grilla As GridEX, cod As Integer, dt1 As DataTable)
        Dim filaModificada As Integer = grCamiones.ActiveRow.Index
        Dim columna As Integer = grCamiones.PrimaryGrid.Columns.Item("CODIGO").ColumnIndex
        Dim columnaPeso As Integer = grCamiones.PrimaryGrid.Columns.Item("capacidad").ColumnIndex
        Dim peso As Integer = grCamiones.GetCell(filaModificada, columna).Value
        Dim salida As Integer = 0
        Dim numi As Integer = 0

        Dim salidaT As DataTable = TraerUltimaSalida(peso, Now.Date.ToString("dd/MM/yyyy"))
        If salidaT.Rows.Count > 0 Then
            salida = salidaT.Rows(0).Item("salida")
            numi = salidaT.Rows(0).Item("numi")
        End If

        If dt1 Is Nothing Then
        Else
            Dim dtAux As DataTable = dtCab.Clone()
            For i = 1 To 51 - dtAux.Rows.Count - 1 Step 1
                Dim nom1 As String = (dtAux.Rows.Count + 2).ToString
                dtAux.Rows.Add(nom1, 0)
            Next

            If dt1.Columns.Count < dtAux.Rows.Count + 17 Then
                Dim j As Integer = 52
                For i = 0 To (dtAux.Rows.Count - dtCab.Rows.Count) - 1 Step 1

                    Dim nom As String = j.ToString
                    dt1.Columns.Add(nom, GetType(Double))
                    dt1.Columns(nom).SetOrdinal(17 + dtCab.Rows.Count)
                    j = j - 1
                Next

            End If
        End If
        Dim dt As DataTable
        If cod = 1 Then
            ''dt1.Columns.RemoveAt(15)
            ''dt1.Columns.RemoveAt(15)

            Dim dtSalida As DataTable = VerificarSalidaExistenteDatos(peso)
            If VerificarSalidaExistente(peso) Then

                Dim ef = New Efecto

                ef.tipo = 1
                ef.Context = "Ya existe una salida de ese camion, ¿desea unir los pedidos pendientes a esa salida?".ToUpper
                ef.Header = "PREGUNTA"
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                If (bandera = True) Then
                    For i = 0 To CType(grBuscador.DataSource, DataTable).Rows.Count - 1 Step 1
                        If CType(grBuscador.DataSource, DataTable).Rows(i).Item("trnumi") = numi Then
                            grBuscador.Row = i
                            Exit For
                        End If
                    Next
                    If (dtSalida.Rows(0).Item("salida")) = 1 Then
                        SuperTabControl1.SelectedTab = tbSalida1
                        grilla = grPedidos
                    ElseIf (dtSalida.Rows(0).Item("salida")) = 2 Then
                        SuperTabControl1.SelectedTab = tbSalida2
                        grilla = grPedidos2
                    ElseIf (dtSalida.Rows(0).Item("salida")) = 3 Then
                        SuperTabControl1.SelectedTab = tbSalida3
                        grilla = grPedidos3
                    Else
                        SuperTabControl1.SelectedTab = tbSalida4
                        grilla = grPedidos4
                    End If

                    MBtModificar.PerformClick()
                    cbCamion.Value = peso

                    dt = TraerPedidosPendientes2(dt1, cbCamion.Value, dtSalida.Rows(0).Item("salida"))
                Else
                    MBtNuevo.PerformClick()
                    If (dtSalida.Rows(0).Item("salida") + 1) = 2 Then
                        tbSalida1.Text = "SEGUNDA SALIDA"
                    ElseIf (dtSalida.Rows(0).Item("salida") + 1) = 3 Then
                        tbSalida1.Text = "TERCERA SALIDA"
                    Else
                        tbSalida1.Text = "CUARTA SALIDA"
                    End If
                    dt = TraerPedidosPendientesNuevo(dt1)
                    cbCamion.Value = peso
                End If

                If salida = 0 Then

                Else

                End If

            Else
                MBtNuevo.PerformClick()
                dt = TraerPedidosPendientesNuevo(dt1)
                cbCamion.Value = peso
            End If

        Else

            dt = TraerPedidosPendientes()

        End If
        calcularRecorrido(dt)

        grilla.BoundMode = Janus.Data.BoundMode.Bound
        grilla.DataSource = dt
        grilla.RetrieveStructure()
        With grilla.RootTable.Columns("check1")
            .Width = 40
            .Caption = ""
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = True
        End With
        With grilla.RootTable.Columns("tonro")
            .Width = 40
            .Caption = "NRO."
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = True
        End With
        With grilla.RootTable.Columns("tssalida")
            .Width = 40
            .Caption = ""
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = False
        End With
        With grilla.RootTable.Columns("tshoras")
            .Width = 40
            .Caption = ""
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = False
        End With
        With grilla.RootTable.Columns("tshoral")
            .Width = 40
            .Caption = ""
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = False
        End With
        With grilla.RootTable.Columns("tspeso")
            .Width = 40
            .Caption = ""
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = False
        End With
        With grilla.RootTable.Columns("tohora")
            .Caption = "HORA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("torecorr")
            .Caption = "RECORRIDO"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("totrasl")
            .Caption = "SALIDA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("totentre")
            .Caption = "ENTREGA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("ccdirec")
            .Caption = "RUTA"
            .Width = 100
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("oaccli")
            .Caption = "COD. CLI"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("ccdesc")
            .Caption = "CLIENTE"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("cctelf1")
            .Caption = "TEL."
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("cbdesc")
            .Caption = "PROMOTOR"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        'With grilla.RootTable.Columns("oanumi")
        '    .Caption = "HORA"
        '    .Width = 50
        '    .HeaderStyle.BackColor = Color.Green
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        '    .Visible = False
        'End With
        'With grilla.RootTable.Columns("oaest")
        '    .Caption = "HORA"
        '    .Width = 50
        '    .HeaderStyle.BackColor = Color.Green
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        '    .Visible = False
        'End With
        With grilla.RootTable.Columns("obnumi")
            .Caption = "HORA"
            .Width = 50
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        If cod = 0 Then
            Dim dtCab1 As DataTable = TraerCabecera()
            For i = 0 To dtCab1.Rows.Count - 1 Step 1
                Dim cod1 As String = dtCab1.Rows(i).Item("canumi")
                With grilla.RootTable.Columns(cod1)
                    .Caption = dtCab.Rows(i).Item("cacampo1")
                    .CellStyle.BackColor = Color.LightCoral
                    .Width = 65
                    .HeaderStyle.BackColor = Color.Green
                    .FormatString = "0"
                    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                    .Visible = False
                    .FormatMode = GridEXFormatCondition.Equals(0, "")
                    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
                End With

            Next


        Else
            Dim dtCab1 As DataTable = TraerCabecera()
            For i = 0 To dtCab1.Rows.Count - 1 Step 1
                Dim cod1 As String = dtCab1.Rows(i).Item("canumi")
                With grilla.RootTable.Columns(cod1)
                    .Caption = dtCab.Rows(i).Item("cacampo1")
                    .CellStyle.BackColor = Color.LightCoral
                    .Width = 65
                    .HeaderStyle.BackColor = Color.Green
                    .FormatString = "0"
                    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                    .Visible = True
                    .FormatMode = GridEXFormatCondition.Equals(0, "")
                    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
                End With

            Next
            With grilla.RootTable.Columns("KG")
                .Caption = "KG"
                .Width = 100
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With

        End If

        With grilla.RootTable.Columns("obs")
            .Caption = "OBSERVACION"
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With

        With grilla.RootTable.Columns("obnumi")
            .Caption = "OBSERVACION"
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grilla.RootTable.Columns("kg")
            .Caption = "KG"
            .Width = 120
            .FormatString = "0.00"
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
        End With



        With grilla

            .GroupByBoxVisible = False
            .FilterRowFormatStyle.BackColor = Color.Blue
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            'Diseño de la tabla
            .VisualStyle = VisualStyle.Office2007
            .SelectionMode = SelectionMode.MultipleSelection
            .AlternatingColors = True
            .RecordNavigator = True

            .AllowColumnDrag = False


            .AllowEdit = False

            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowPosition = TotalRowPosition.BottomFixed
        End With
        For i = 0 To CType(grilla.DataSource, DataTable).Columns.Count - 1 Step 1

            If i < 9 Then
                Dim columnHeader As GridEXColumn = grilla.RootTable.Columns(i)
                columnHeader.HeaderStyle.ForeColor = Color.Black
                grilla.Refresh()
            ElseIf i < 19 Then
                Dim columnHeader As GridEXColumn = grilla.RootTable.Columns(i)
                columnHeader.HeaderStyle.ForeColor = Color.Red
                grilla.Refresh()
            ElseIf i < 42 Then
                Dim columnHeader As GridEXColumn = grilla.RootTable.Columns(i)
                columnHeader.HeaderStyle.ForeColor = Color.Blue
                grilla.Refresh()
            Else
                Dim columnHeader As GridEXColumn = grilla.RootTable.Columns(i)
                columnHeader.HeaderStyle.ForeColor = Color.Black
                grilla.Refresh()
            End If

        Next
        If grilla Is grPedidos Then
            tbPesSel.Text = IIf(IsDBNull(dt.Compute("Sum(kg)", "oaccli>0")), 0, dt.Compute("Sum(kg)", "oaccli>0")).ToString
        ElseIf grilla Is grPendientes Then
            tbSeleccionado.Text = IIf(IsDBNull(dt.Compute("Sum(kg)", "oaccli>0")), 0, dt.Compute("Sum(kg)", "oaccli>0")).ToString
        End If

    End Sub

    Private Sub RevisarFormato(ByRef dt As DataTable)

        For i = 0 To dt.Rows.Count - 1 Step 1
            Dim llegada As String = dt.Rows(i).Item("tohora")
            Dim salida As String = dt.Rows(i).Item("totrasl")
            Dim entrega As String = dt.Rows(i).Item("totentre")

            Dim time As DateTime = DateTime.ParseExact(llegada, "H:mm", System.Globalization.CultureInfo.InvariantCulture)
            Dim time2 As DateTime = DateTime.ParseExact(salida, "H:mm", System.Globalization.CultureInfo.InvariantCulture)
            Dim time3 As DateTime = DateTime.ParseExact(entrega, "H:mm", System.Globalization.CultureInfo.InvariantCulture)

            dt.Rows(i).Item("tohora") = time.ToString("HH:mm")
            dt.Rows(i).Item("totrasl") = time2.ToString("HH:mm")
            dt.Rows(i).Item("totentre") = time3.ToString("HH:mm")
        Next

    End Sub

    Private Sub calcularRecorrido(ByRef dt As DataTable)


        RevisarFormato(dt)

        For i = 0 To dt.Rows.Count - 1 Step 1
            Dim llegada As String = dt.Rows(i).Item("tohora")
            Dim salida As String = dt.Rows(i).Item("totrasl")

            'Dim horaL, horaS, minutoL, minutoS As Integer
            'horaL = llegada.Substring(0, 2)
            'horaS = salida.Substring(0, 2)
            'minutoL = llegada.Substring(3, 2)
            'minutoS = salida.Substring(3, 2)



            ' Convertir las cadenas a objetos DateTime
            Dim tiempoInicio As DateTime = DateTime.ParseExact(salida, "HH:mm", Nothing)
            Dim tiempoFin As DateTime = DateTime.ParseExact(llegada, "HH:mm", Nothing)

            ' Calcular la diferencia
            Dim diferencia As TimeSpan = tiempoFin - tiempoInicio

            ' Extraer la diferencia en horas y minutos
            Dim horas As Integer
            Dim minutos As Integer
            If diferencia.Hours < 0 Then
                horas = 0
            Else

                horas = diferencia.Hours
            End If
            If diferencia.Minutes < 0 Then
                minutos = 0
            Else

                minutos = diferencia.Minutes
            End If


            Dim recorrido As String = horas.ToString("00") + ":" + minutos.ToString("00") + ":00"

            dt.Rows(i).Item("torecorr") = recorrido
        Next
    End Sub


    Private Sub CargarHojaPedidos(grilla As GridEX, zona As Integer)
        Dim dt As DataTable = TraerHojaRuta(zona)



        calcularRecorrido(dt)

        dt.Columns.RemoveAt(17)

        grilla.BoundMode = Janus.Data.BoundMode.Bound
        grilla.DataSource = dt
        grilla.RetrieveStructure()
        With grilla.RootTable.Columns("check1")
            .Width = 40
            .Caption = ""
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = True
        End With
        With grilla.RootTable.Columns("tonro")
            .Width = 40
            .Caption = "NRO."
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = True
        End With
        With grilla.RootTable.Columns("tssalida")
            .Caption = "LLEGADA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grilla.RootTable.Columns("tshoras")
            .Caption = "LLEGADA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grilla.RootTable.Columns("tshoral")
            .Caption = "LLEGADA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grilla.RootTable.Columns("tspeso")
            .Caption = "LLEGADA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grilla.RootTable.Columns("tohora")
            .Caption = "LLEGADA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("torecorr")
            .Caption = "RECORRIDO"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("totrasl")
            .Caption = "SALIDA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With

        With grilla.RootTable.Columns("ccdirec")
            .Caption = "RUTA"
            .Width = 100
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("oaccli")
            .Caption = "COD. CLI"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("ccdesc")
            .Caption = "CLIENTE"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("cctelf1")
            .Caption = "TEL."
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("cbdesc")
            .Caption = "PROMOTOR"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        'With grilla.RootTable.Columns("oanumi")
        '    .Caption = "HORA"
        '    .Width = 50
        '    .HeaderStyle.BackColor = Color.Green
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        '    .Visible = False
        'End With
        'With grilla.RootTable.Columns("oaest")
        '    .Caption = "ESTADO"
        '    .Width = 50
        '    .HeaderStyle.BackColor = Color.Green
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        '    .Visible = False
        'End With
        Dim dtCab1 As DataTable = TraerCabecera()
        For i = 0 To dtCab1.Rows.Count - 1 Step 1
            Dim cod As String = dtCab1.Rows(i).Item("canumi")
            With grilla.RootTable.Columns(cod)
                .Caption = dtCab.Rows(i).Item("cacampo1")
                .CellStyle.BackColor = Color.LightCoral
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With

        Next

        'With grilla.RootTable.Columns("blanca")
        '    .Caption = "BLAN"
        '    .CellStyle.BackColor = Color.LightCoral
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("negra")
        '    .Caption = "NEGR"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightCoral
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("azul")
        '    .Caption = "AZUL"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightCoral
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("segunda")
        '    .Caption = "SEG."
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightCoral
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("bb")
        '    .Caption = "BB."
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightCoral
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("g1")
        '    .Caption = "G1"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightYellow
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("g2")
        '    .Caption = "G2"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightYellow
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("g3")
        '    .Caption = "G3"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightYellow
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("g4")
        '    .Caption = "G4"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightYellow
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("g5")
        '    .Caption = "G5"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightYellow
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("filete")
        '    .Caption = "FIL"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("filetillo")
        '    .Caption = "FILT"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("filedesh")
        '    .Caption = "FIL. DES."
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 75
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("molida")
        '    .Caption = "MOL"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("maltrato")
        '    .Caption = "MALT"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("molidacor")
        '    .Caption = "MOLC"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("fpierna")
        '    .Caption = "FPIER"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("piernasola")
        '    .Caption = "PIERS"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("muslo")
        '    .Caption = "MUSL"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("alasegunda")
        '    .Caption = "ALASG"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("alapicada")
        '    .Caption = "ALAP"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("huesocostilla")
        '    .Caption = "COST"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("pulmon")
        '    .Caption = "PULM"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("puntala")
        '    .Caption = "P.ALA"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("cabeza")
        '    .Caption = "CAB"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("pechuga")
        '    .Caption = "PECH"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals("0.00", "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum

        'End With
        'With grilla.RootTable.Columns("pierna")
        '    .Caption = "PIER"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0.00"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals("0.00", "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum

        'End With
        'With grilla.RootTable.Columns("ala")
        '    .Caption = "ALA"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0.00"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("cazuela")
        '    .Caption = "CAZ."
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0.00"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("cuello")
        '    .Caption = "CUE:"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0.00"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("menudo")
        '    .Caption = "MEN."
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0.00"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("higado")
        '    .Caption = "HIG."
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0.00"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("alalast")
        '    .Caption = "ALAX"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0.00"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("filetepiel")
        '    .Caption = "P/FIL."
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0.00"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0.00, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        With grilla.RootTable.Columns("obs")
            .Caption = "OBSERVACION"
            .Width = 200
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("KG")
            .Caption = "OBSERVACION"
            .Width = 200
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With


        With grilla

            .GroupByBoxVisible = False
            .FilterRowFormatStyle.BackColor = Color.Blue
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            'Diseño de la tabla
            .VisualStyle = VisualStyle.Office2007
            .SelectionMode = SelectionMode.MultipleSelection
            .AlternatingColors = True
            .RecordNavigator = True

            .AllowColumnDrag = False


            .AllowEdit = False

            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowPosition = TotalRowPosition.BottomFixed
        End With
        For i = 0 To CType(grilla.DataSource, DataTable).Columns.Count - 1 Step 1

            If i < 11 Then
                Dim columnHeader As GridEXColumn = grilla.RootTable.Columns(i)
                columnHeader.HeaderStyle.ForeColor = Color.Black
                grilla.Refresh()
            ElseIf i < 21 Then
                Dim columnHeader As GridEXColumn = grilla.RootTable.Columns(i)
                columnHeader.HeaderStyle.ForeColor = Color.Red
                grilla.Refresh()
            ElseIf i < 44 Then
                Dim columnHeader As GridEXColumn = grilla.RootTable.Columns(i)
                columnHeader.HeaderStyle.ForeColor = Color.Blue
                grilla.Refresh()
            Else
                Dim columnHeader As GridEXColumn = grilla.RootTable.Columns(i)
                columnHeader.HeaderStyle.ForeColor = Color.Black
                grilla.Refresh()
            End If

        Next
        ocultarVacios(grilla)
    End Sub




    ' Manejador de eventos para el clic en las pestañas

    Private Sub ocultarVacios(grilla As GridEX)
        If CType(grilla.DataSource, DataTable).Rows.Count > 0 Then
            For i = 16 To CType(grilla.DataSource, DataTable).Columns.Count - 3 Step 1
                Dim valores As Decimal() = CType(grilla.DataSource, DataTable).AsEnumerable().Select(Function(row) Convert.ToDecimal(row(i))).ToArray()

                ' Luego, utilizamos LINQ para calcular la suma de los valores en el array
                Dim suma As Decimal = valores.Sum()

                If suma = 0 Then
                    grilla.RootTable.Columns(i).Visible = False
                Else
                    grilla.RootTable.Columns(i).Visible = True
                End If
            Next
        End If
    End Sub

    Private Sub ocultarVacios2(grilla As GridEX)
        If CType(grilla.DataSource, DataTable).Rows.Count > 0 Then
            For i = 16 To CType(grilla.DataSource, DataTable).Columns.Count - 3 Step 1
                Dim valores As Decimal() = CType(grilla.DataSource, DataTable).AsEnumerable().Select(Function(row) Convert.ToDecimal(row(i))).ToArray()

                ' Luego, utilizamos LINQ para calcular la suma de los valores en el array
                Dim suma As Decimal = valores.Sum()

                If suma = 0 Then
                    grilla.RootTable.Columns(i).Visible = False
                Else
                    grilla.RootTable.Columns(i).Visible = True
                End If
            Next
        End If
    End Sub
    Private Sub TableLayoutPanelPrincipal_Paint(sender As Object, e As PaintEventArgs)

    End Sub

    Private Sub grPedidos_FormattingRow(sender As Object, e As RowLoadEventArgs) Handles grPedidos.FormattingRow

    End Sub

    Private Sub grPedidos_RowCheckStateChanging(sender As Object, e As RowCheckStateChangingEventArgs) Handles grPedidos.RowCheckStateChanging

    End Sub

    Private Sub grPedidos_RowCheckStateChanged(sender As Object, e As RowCheckStateChangeEventArgs) Handles grPedidos.RowCheckStateChanged
        ' Verificar si la fila está marcada como seleccionada
        If e.Row.RowType = Janus.Windows.GridEX.RowType.Record AndAlso e.Row.IsChecked Then
            ' Iterar sobre las celdas de la fila actual
            For Each cell As Janus.Windows.GridEX.GridEXCell In e.Row.Cells
                ' Verificar si la celda contiene un valor numérico igual a 0
                If cell.Value IsNot Nothing AndAlso IsNumeric(cell.Value) AndAlso Convert.ToDouble(cell.Value) = 0 Then
                    ' Establecer el valor de la celda como una cadena vacía
                    cell.Value = ""
                End If
            Next
        End If
    End Sub

    Private Sub TableLayoutPanel2_Paint(sender As Object, e As PaintEventArgs)

    End Sub

    Private Sub MBtNuevo_Click(sender As Object, e As EventArgs) Handles MBtNuevo.Click
        Nuevo = True
        Modificar = False

        cbZona.Value = -1
        Habilitar()
        Limpiar()
        'TraerPendientes()
        CargarEstilosTab()
        'CargarPeso()

        'MSuperTabControlPrincipal.Enabled = False
    End Sub

    Private Sub Limpiar()
        tbCodigo.Clear()
        cbZona.SelectedIndex = 0
        'cbRepartidor.SelectedIndex = 0
        tbFecha.Value = Date.Now.ToString("dd/MM/yyyy")
        tbPesSel.Text = "0.00"
        tbPesTot.Text = "0.00"
        tbHoraS.Value = DateAndTime.TimeOfDay
        tbHoraL.Value = DateAndTime.TimeOfDay
        CargarHojaPedidos(grPedidos, -1)
        CargarHojaPedidos(grPedidos2, -1)
        CargarHojaPedidos(grPedidos3, -1)
        CargarHojaPedidos(grPedidos4, -1)
        tbSalida2.Visible = False
        tbSalida3.Visible = False
        tbSalida4.Visible = False

        Grilla1 = 0
        Grilla2 = 0
        Grilla3 = 0
        Grilla4 = 0

        tbDisponible.Text = "0.00"
        tbSaldoCamion.Text = "0.00"
        tbSeleccionado.Text = "0.00"
    End Sub




    Private Sub grPrecioPolloEntero_ColumnHeaderClick(sender As Object, e As ColumnActionEventArgs)

    End Sub

    Private Sub grPedidos_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grPedidos.CellValueChanged
        If ((e.Column.Key.Equals("check1"))) Then
            Dim bit As Boolean = grPedidos.GetValue("check1")

            If bit Then
                Dim act As Double = CDbl(tbPesSel.Text)

                act = act + grPedidos.GetValue("kg")
                tbPesSel.Text = act.ToString
            Else
                Dim act As Double = CDbl(tbPesSel.Text)

                act = act - grPedidos.GetValue("kg")
                tbPesSel.Text = act.ToString
            End If


        End If
    End Sub

    Private Sub LabelX27_Click(sender As Object, e As EventArgs) Handles LabelX27.Click

    End Sub

    Private Sub MBtGrabar_Click(sender As Object, e As EventArgs) Handles MBtGrabar.Click
        btOrdenar.PerformClick()
        If Nuevo Then
            GrabarNuevoRegistro()
        ElseIf Modificar Then
            ModificarRegistro()
        End If
    End Sub

    Private Sub GrabarNuevoRegistro()
        calcularRecorrido(CType(grPedidos.DataSource, DataTable))
        llenarvacios(grPedidos)
        Dim resultadoFilas() As DataRow = CType(grPedidos.DataSource, DataTable).Select("check1 = True")
        Dim dt As DataTable = CType(grPedidos.DataSource, DataTable).Clone()
        dt.Clear()
        For Each fila As DataRow In resultadoFilas
            dt.ImportRow(fila)
        Next
        'Dim dt As DataTable = CType(grPedidos.DataSource, DataTable)
        dt.Columns.RemoveAt(0)
        'dt.Columns.RemoveAt(15)
        'dt.Columns.RemoveAt(15)
        Dim dtAux As DataTable = dtCab.Clone()



        For i = 1 To 51 - dtAux.Rows.Count - 1 Step 1
            Dim nom1 As String = (dtAux.Rows.Count + 3).ToString
            dtAux.Rows.Add(nom1, 0)
        Next

        If dt.Columns.Count < dtAux.Rows.Count + 17 Then
            Dim j As Integer = 52
            For i = 0 To (dtAux.Rows.Count - dtCab.Rows.Count) - 1 Step 1

                Dim nom As String = j.ToString
                dt.Columns.Add(nom, GetType(Double))
                dt.Columns(nom).SetOrdinal(15 + dtCab.Rows.Count)
                j = j - 1
            Next

        End If

        Dim cod As Integer
        If Not IsNumeric(tbCodigo.Text) Then
            cod = 0
        Else
            cod = CInt(tbCodigo.Text)
        End If
        If dt.Rows.Count > 0 Then
            Dim res As Boolean = GrabarHojaRuta(cod, cbCamion.Value, cbRepartidor.Value, tbFecha.Value.ToString("dd/MM/yyyy"), tbHoraS.Value.ToString("HH:mm"), tbHoraL.Value.ToString("HH:mm"),
                                            CDbl(tbPesTot.Text), dt)

            If res Then
                ToastNotification.Show(Me, "salida grabada con exito".ToUpper,
                                           My.Resources.OK,
                                           3 * 1000,
                                           eToastGlowColor.Red,
                                           eToastPosition.TopCenter)
                Limpiar()
                CargarHojaRuta()
                InHabilitar()
                CargarCamiones()
                CargarHojaPedidosPendientes(grPendientes, 0, CType(grPedidos4.DataSource, DataTable))
            End If
        Else
            ToastNotification.Show(Me, "seleccione al menos un pedido".ToUpper,
                                       My.Resources.WARNING,
                                       3 * 1000,
                                       eToastGlowColor.Red,
                                       eToastPosition.TopCenter)
        End If
    End Sub

    Private Sub ModificarRegistro()
        Dim salida As Integer = 0
        Dim grilla As GridEX
        If SuperTabControl1.SelectedTab Is tbSalida1 Then
            grilla = grPedidos
            salida = 1
        ElseIf SuperTabControl1.SelectedTab Is tbSalida2 Then
            grilla = grPedidos2
            salida = 2
        ElseIf SuperTabControl1.SelectedTab Is tbSalida3 Then
            grilla = grPedidos3
            salida = 3
        ElseIf SuperTabControl1.SelectedTab Is tbSalida4 Then
            grilla = grPedidos4
            salida = 4
        End If
        calcularRecorrido(CType(grilla.DataSource, DataTable))
        llenarvacios(grilla)
        Dim resultadoFilas() As DataRow = CType(grilla.DataSource, DataTable).Select("check1 = True")
        Dim dt As DataTable = CType(grilla.DataSource, DataTable).Clone()
        dt.Clear()
        For Each fila As DataRow In resultadoFilas
            dt.ImportRow(fila)
        Next
        dt.Columns.RemoveAt(0)
        If Nuevo = True Then
            'dt.Columns.RemoveAt(15)
            'dt.Columns.RemoveAt(15)
        End If


        Dim dtAux As DataTable = dtCab.Clone()



        For i = 1 To 51 - dtAux.Rows.Count - 1 Step 1
            Dim nom1 As String = (dtAux.Rows.Count + 3).ToString
            dtAux.Rows.Add(nom1, 0)
        Next

        If (dt.Columns.Count) - dtAux.Rows.Count <> 17 Then
            Dim j As Integer = 52
            For i = 0 To (dtAux.Rows.Count - dtCab.Rows.Count) - 1 Step 1

                Dim nom As String = j.ToString
                dt.Columns.Add(nom, GetType(Double))
                dt.Columns(nom).SetOrdinal(15 + dtCab.Rows.Count)
                j = j - 1
            Next

        End If
        Dim cod As Integer
        If Not IsNumeric(tbCodigo.Text) Then
            cod = 0
        Else
            cod = CInt(tbCodigo.Text)
        End If
        If dt.Rows.Count > 0 Then
            Dim res As Boolean = ModificarHojaRuta(cod, salida, cbCamion.Value, cbRepartidor.Value, tbFecha.Value.ToString("dd/MM/yyyy"), tbHoraS.Value.ToString("HH:mm"), tbHoraL.Value.ToString("HH:mm"),
                                            CDbl(tbPesTot.Text), dt)

            If res Then
                ToastNotification.Show(Me, "salida grabada con exito".ToUpper,
                                       My.Resources.OK,
                                       3 * 1000,
                                       eToastGlowColor.Red,
                                       eToastPosition.TopCenter)
                Limpiar()
                CargarHojaRuta()
                InHabilitar()
                CargarCamiones()
                CargarHojaPedidosPendientes(grPendientes, 0, CType(grPedidos4.DataSource, DataTable))
            End If
        Else
            ToastNotification.Show(Me, "seleccione al menos un pedido".ToUpper,
                                   My.Resources.WARNING,
                                   5 * 1000,
                                   eToastGlowColor.Red,
                                   eToastPosition.TopCenter)
        End If
    End Sub

    Private Sub llenarvacios(grilla As GridEX)

        For i = 0 To CType(grilla.DataSource, DataTable).Rows.Count - 1 Step 1
            For j = 16 To CType(grilla.DataSource, DataTable).Columns.Count - 3 Step 1
                If Not IsNumeric(CType(grilla.DataSource, DataTable).Rows(i).Item(j)) Then
                    CType(grilla.DataSource, DataTable).Rows(i).Item(j) = 0.00
                End If
            Next
        Next
    End Sub

    Private Sub llenarvaciosdatatable(ByRef dt As DataTable)

        For i = 0 To dt.Rows.Count - 1 Step 1
            For j = 16 To dt.Columns.Count - 1 Step 1
                If Not IsNumeric(dt.Rows(i).Item(j)) Then
                    dt.Rows(i).Item(j) = 0.00
                End If
            Next
        Next
    End Sub

    Private Sub MBtModificar_Click(sender As Object, e As EventArgs) Handles MBtModificar.Click
        Dim cont As Integer
        If Grilla2 = 1 Then
            If CType(grPedidos2.DataSource, DataTable).Rows.Count = 0 Then
                CargarHojaPedidos(grPedidos2, cbZona.Value)
                cont = CType(grPedidos2.DataSource, DataTable).Rows.Count
                If cont > 0 Then
                    tbSalida2.Visible = True
                    SuperTabControl1.SelectedTab = tbSalida2
                End If
            End If
        ElseIf Grilla3 = 1 Then
            If CType(grPedidos3.DataSource, DataTable).Rows.Count = 0 Then
                CargarHojaPedidos(grPedidos3, cbZona.Value)
                cont = CType(grPedidos3.DataSource, DataTable).Rows.Count
                If cont > 0 Then
                    tbSalida3.Visible = True
                    SuperTabControl1.SelectedTab = tbSalida3
                End If
            End If
        ElseIf Grilla4 = 1 Then
            If CType(grPedidos4.DataSource, DataTable).Rows.Count = 0 Then
                CargarHojaPedidos(grPedidos4, cbZona.Value)
                cont = CType(grPedidos4.DataSource, DataTable).Rows.Count
                If cont > 0 Then
                    tbSalida4.Visible = True
                    SuperTabControl1.SelectedTab = tbSalida3
                End If
            End If
        End If
        If Grilla2 = 1 Or Grilla3 = 1 Or Grilla4 = 1 Then
            Nuevo = False
            Modificar = True
            Habilitar()
            cbZona.ReadOnly = True
            cbRepartidor.ReadOnly = True
        Else
            ToastNotification.Show(Me, "no tiene pedidos pendientes para crear una nueva salida".ToUpper,
                                       My.Resources.WARNING,
                                       5 * 1000,
                                       eToastGlowColor.Red,
                                       eToastPosition.TopCenter)
        End If

    End Sub

    Private Sub grBuscador_SelectionChanged(sender As Object, e As EventArgs) Handles grBuscador.SelectionChanged
        If (grBuscador.Row > -1) Then
            LLenarDatos(grBuscador.Row)
        End If
    End Sub

    Private Sub LLenarDatos(num As Integer)


        With grBuscador
            Dim total As Double
            Dim total2 As Double
            tbCodigo.Text = .GetValue("trnumi").ToString
            tbFecha.Value = .GetValue("trfdoc")
            cbRepartidor.Value = .GetValue("trcodrep")
            cbCamion.Value = .GetValue("trzon")
            Dim dt As DataTable = TraerSalidas(CInt(.GetValue("trnumi")))
            Dim resultadoFilas() As DataRow = dt.Select("tssalida = 1")
            Dim dt1 As DataTable = dt.Clone()
            dt1.Clear()
            For Each fila As DataRow In resultadoFilas
                dt1.ImportRow(fila)
            Next
            If dt1.Rows.Count > 0 Then
                CargarGrillas(grPedidos, dt1)
                tbHoraS.Value = dt1.Rows(0).Item("tshoras")
                tbHoraL.Value = dt1.Rows(0).Item("tshoral")
                total = IIf(IsDBNull(dt1.Compute("Sum(tokg)", "toclie>0")), 0, dt1.Compute("Sum(tokg)", "toclie>0"))
                tbPesTot.Text = (dt1.Rows(0).Item("tspeso")).ToString
                tbPesSel.Text = total.ToString
                tbPesSal.Text = (dt1.Rows(0).Item("tspeso") - total).ToString
                Grilla1 = 2
            Else
                CargarGrillas(grPedidos, dt1)
                Grilla1 = 1
            End If
            resultadoFilas = dt.Select("tssalida = 2")
            Dim dt2 As DataTable = dt.Clone()
            dt2.Clear()
            For Each fila As DataRow In resultadoFilas
                dt2.ImportRow(fila)
            Next
            If dt2.Rows.Count > 0 Then
                CargarGrillas(grPedidos2, dt2)
                tbHoraS2.Value = dt2.Rows(0).Item("tshoras")
                tbHoraL2.Value = dt2.Rows(0).Item("tshoral")
                total2 = IIf(IsDBNull(dt2.Compute("Sum(tokg)", "toclie>0")), 0, dt2.Compute("Sum(tokg)", "toclie>0"))
                tbPesTot2.Text = tbPesTot.Text
                tbPesSel2.Text = total2.ToString
                Grilla2 = 2
            Else
                CargarGrillas(grPedidos2, dt2)
                Grilla2 = 1
                tbPesTot2.Text = tbPesTot.Text
            End If

            resultadoFilas = dt.Select("tssalida = 3")
            Dim dt3 As DataTable = dt.Clone()
            dt3.Clear()
            For Each fila As DataRow In resultadoFilas
                dt3.ImportRow(fila)
            Next
            If dt3.Rows.Count > 0 Then
                CargarGrillas(grPedidos3, dt3)
                tbHoraS3.Value = dt3.Rows(0).Item("tshoras")
                tbHoraL3.Value = dt3.Rows(0).Item("tshoral")
                total = IIf(IsDBNull(dt3.Compute("Sum(tokg)", "toclie>0")), 0, dt3.Compute("Sum(tokg)", "toclie>0"))
                'tbPesTot.Text = total.ToString
                Grilla3 = 2
            Else
                CargarGrillas(grPedidos3, dt3)
                Grilla3 = 1
            End If

            resultadoFilas = dt.Select("tssalida = 4")
            Dim dt4 As DataTable = dt.Clone()
            dt4.Clear()
            For Each fila As DataRow In resultadoFilas
                dt4.ImportRow(fila)
            Next
            If dt4.Rows.Count > 0 Then
                CargarGrillas(grPedidos4, dt4)
                tbHoraS2.Value = dt4.Rows(0).Item("tshoras")
                tbHoraL2.Value = dt4.Rows(0).Item("tshoral")
                total = IIf(IsDBNull(dt4.Compute("Sum(tokg)", "toclie>0")), 0, dt4.Compute("Sum(tokg)", "toclie>0"))
                'tbPesTot.Text = total.ToString
                Grilla4 = 2
            Else
                CargarGrillas(grPedidos4, dt4)
                Grilla4 = 1
            End If



        End With
        ' TraerPendientes()
        CargarVistasSalidas()
        CargarEstilosTab()
        CargarTitulos()
        'P_prPonerCodicion()

        'CargarPeso2()
        Dim pagina As String = (grBuscador.Row + 1).ToString + "/" + (CType(grBuscador.DataSource, DataTable).Rows.Count).ToString
        MLbPaginacion.Text = pagina
    End Sub

    Private Sub P_prPonerCodicion()
        'poner color a la fila de acuerdo a la condicion 
        Dim fc As GridEXFormatCondition
        If CType(grPedidos.DataSource, DataTable).Rows.Count > 0 Then
            fc = New GridEXFormatCondition(grPedidos.RootTable.Columns("oaest"), ConditionOperator.Equal, 3)
            fc.FormatStyle.BackColor = Color.LightGreen
            fc.FormatStyle.ForeColor = Color.Black

            grPedidos.RootTable.FormatConditions.Add(fc)
        End If
        If CType(grPedidos2.DataSource, DataTable).Rows.Count > 0 Then
            fc = New GridEXFormatCondition(grPedidos2.RootTable.Columns("oaest"), ConditionOperator.Equal, 3)
            fc.FormatStyle.BackColor = Color.LightGreen
            fc.FormatStyle.ForeColor = Color.Black

            grPedidos2.RootTable.FormatConditions.Add(fc)
        End If
        If CType(grPedidos3.DataSource, DataTable).Rows.Count > 0 Then
            fc = New GridEXFormatCondition(grPedidos3.RootTable.Columns("oaest"), ConditionOperator.Equal, 3)
            fc.FormatStyle.BackColor = Color.LightGreen
            fc.FormatStyle.ForeColor = Color.Black
            grPedidos3.RootTable.FormatConditions.Add(fc)
        End If
        If CType(grPedidos.DataSource, DataTable).Rows.Count > 0 Then
            fc = New GridEXFormatCondition(grPedidos4.RootTable.Columns("oaest"), ConditionOperator.Equal, 3)
            fc.FormatStyle.BackColor = Color.LightGreen
            fc.FormatStyle.ForeColor = Color.Black

            grPedidos4.RootTable.FormatConditions.Add(fc)
        End If
    End Sub

    Private Sub CargarTitulos()
        tbSalida1.Text = "PRIMERA SALIDA"
        tbSalida2.Text = "SEGUNDA SALIDA"
        tbSalida3.Text = "TERCERA SALIDA"
        tbSalida4.Text = "CUARTA SALIDA"
    End Sub

    Private Sub CargarEstilosTab()
        Dim colore() As Color = {Color.Lime}
        Dim colore15() As Color = {Color.Green}
        Dim colore2() As Color = {Color.Yellow}
        If Grilla1 = 2 Then
            tbSalida1.TabColor.Default.Normal.Background.Colors = colore
            tbSalida1.TabColor.Default.Selected.Background.Colors = colore
            tbSalida1.TabColor.Default.SelectedMouseOver.Background.Colors = colore

            tbSalida1.TabColor.Default.Normal.Text = Color.Black
            tbSalida1.TabColor.Default.Selected.Text = Color.Black
            tbSalida1.TabColor.Default.SelectedMouseOver.Text = Color.Black
            tbSalida1.TabColor.Default.MouseOver.Background.Colors = colore15
            tbSalida1.TabColor.Default.MouseOver.Text = Color.White



        Else
            tbSalida1.TabColor.Default.Normal.Background.Colors = colore2
            tbSalida1.TabColor.Default.Selected.Background.Colors = colore2
            tbSalida1.TabColor.Default.SelectedMouseOver.Background.Colors = colore2
            tbSalida1.TabColor.Default.Normal.Text = Color.Black
            tbSalida1.TabColor.Default.Selected.Text = Color.Black
            tbSalida1.TabColor.Default.SelectedMouseOver.Text = Color.Black
        End If
        If Grilla2 = 2 Then
            tbSalida2.TabColor.Default.Normal.Background.Colors = colore
            tbSalida2.TabColor.Default.Selected.Background.Colors = colore
            tbSalida2.TabColor.Default.SelectedMouseOver.Background.Colors = colore
            tbSalida2.TabColor.Default.Normal.Text = Color.Black
            tbSalida2.TabColor.Default.Selected.Text = Color.Black
            tbSalida2.TabColor.Default.SelectedMouseOver.Text = Color.Black
            tbSalida2.TabColor.Default.MouseOver.Background.Colors = colore15
            tbSalida2.TabColor.Default.MouseOver.Text = Color.White
        Else
            tbSalida2.TabColor.Default.Normal.Background.Colors = colore2
            tbSalida2.TabColor.Default.Selected.Background.Colors = colore2
            tbSalida2.TabColor.Default.SelectedMouseOver.Background.Colors = colore2
            tbSalida2.TabColor.Default.Normal.Text = Color.Black
            tbSalida2.TabColor.Default.Selected.Text = Color.Black
            tbSalida2.TabColor.Default.SelectedMouseOver.Text = Color.Black
        End If
        If Grilla3 = 2 Then
            tbSalida3.TabColor.Default.Normal.Background.Colors = colore
            tbSalida3.TabColor.Default.Selected.Background.Colors = colore
            tbSalida3.TabColor.Default.SelectedMouseOver.Background.Colors = colore
            tbSalida3.TabColor.Default.Normal.Text = Color.Black
            tbSalida3.TabColor.Default.Selected.Text = Color.Black
            tbSalida3.TabColor.Default.SelectedMouseOver.Text = Color.Black
            tbSalida3.TabColor.Default.MouseOver.Background.Colors = colore15
            tbSalida3.TabColor.Default.MouseOver.Text = Color.White
        Else
            tbSalida3.TabColor.Default.Normal.Background.Colors = colore2
            tbSalida3.TabColor.Default.Selected.Background.Colors = colore2
            tbSalida3.TabColor.Default.SelectedMouseOver.Background.Colors = colore2
            tbSalida3.TabColor.Default.Normal.Text = Color.Black
            tbSalida3.TabColor.Default.Selected.Text = Color.Black
            tbSalida3.TabColor.Default.SelectedMouseOver.Text = Color.Black
        End If
    End Sub
    Private Sub CargarGrillas(grilla As GridEX, dt As DataTable)


        grilla.BoundMode = Janus.Data.BoundMode.Bound
        grilla.DataSource = dt
        grilla.RetrieveStructure()
        With grilla.RootTable.Columns("check1")
            .Width = 40
            .Caption = ""
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = False
        End With
        With grilla.RootTable.Columns("tonro")
            .Width = 40
            .Caption = "NRO."
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = True
        End With
        With grilla.RootTable.Columns("tssalida")
            .Width = 40
            .Caption = ""
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = False
        End With
        With grilla.RootTable.Columns("tshoras")
            .Width = 40
            .Caption = ""
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = False
        End With
        With grilla.RootTable.Columns("tshoral")
            .Width = 40
            .Caption = ""
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = False
        End With
        With grilla.RootTable.Columns("tspeso")
            .Width = 40
            .Caption = ""
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = False
        End With
        With grilla.RootTable.Columns("tohora")
            .Caption = "LLEGADA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("totentre")
            .Caption = "ENTREGA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("totrasl")
            .Caption = "SALIDA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("toRECORR")
            .Caption = "RECORRIDO"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("ccdirec")
            .Caption = "RUTA"
            .Width = 100
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("tooanumi")
            .Caption = "PEDIDO"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grilla.RootTable.Columns("toclie")
            .Caption = "COD. CLI"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("ccdesc")
            .Caption = "CLIENTE"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("cctelf1")
            .Caption = "TEL."
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("cbdesc")
            .Caption = "PROMOTOR"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        Dim dtCab1 As DataTable = TraerCabecera()
        For i = 0 To dtCab.Rows.Count - 1 Step 1
            Dim cod As String = dtCab1.Rows(i).Item("canumi")
            With grilla.RootTable.Columns(cod)
                .Caption = dtCab1.Rows(i).Item("cacampo1")
                .CellStyle.BackColor = Color.LightCoral
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With

        Next
        'With grilla.RootTable.Columns("toblan")
        '    .Caption = "BLAN"
        '    .CellStyle.BackColor = Color.LightCoral
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("toneg")
        '    .Caption = "NEGR"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightCoral
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("toazul")
        '    .Caption = "AZUL"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightCoral
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("tosegun")
        '    .Caption = "SEG."
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightCoral
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("tobb")
        '    .Caption = "BB."
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightCoral
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("gr1")
        '    .Caption = "G1"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightYellow
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("gr2")
        '    .Caption = "G2"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightYellow
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("gr3")
        '    .Caption = "G3"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightYellow
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("gr4")
        '    .Caption = "G4"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightYellow
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("gr5")
        '    .Caption = "G5"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightYellow
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("tofile")
        '    .Caption = "FIL"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("topech")
        '    .Caption = "PECH"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals("0.00", "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum

        'End With
        'With grilla.RootTable.Columns("topier")
        '    .Caption = "PIER"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0.00"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals("0.00", "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum

        'End With
        'With grilla.RootTable.Columns("toala")
        '    .Caption = "ALA"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0.00"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("tocaz")
        '    .Caption = "CAZ."
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0.00"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("tocuell")
        '    .Caption = "CUE:"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0.00"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("tomenu")
        '    .Caption = "MEN."
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0.00"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("tohiga")
        '    .Caption = "HIG."
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0.00"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("toalalast")
        '    .Caption = "ALAX"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0.00"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("tofilepiel")
        '    .Caption = "P/FIL."
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightBlue
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0.00"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0.00, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("tofileti")
        '    .Caption = "FILT"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("tomoli")
        '    .Caption = "MOL"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("tomalt")
        '    .Caption = "MALT"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("tofildesh")
        '    .Caption = "FIL.DESH."
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("tomolcor")
        '    .Caption = "MOLC"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("tofpier")
        '    .Caption = "FPIER"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("topiersola")
        '    .Caption = "PIERS"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("tomuslo")
        '    .Caption = "MUSL"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("toalas")
        '    .Caption = "ALASG"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("toalap")
        '    .Caption = "ALAP"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("tohuescos")
        '    .Caption = "COST"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("topulmon")
        '    .Caption = "PULM"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("topuntala")
        '    .Caption = "P.ALA"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("tocab")
        '    .Caption = "CAB"
        '    .CellStyle.BackColor = Color.LightBlue
        '    .Width = 65
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        With grilla.RootTable.Columns("toobs")
            .Caption = "OBSERVACION"
            .Width = 200
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        'With grilla.RootTable.Columns("oaest")
        '.Caption = "ESTADO"
        '.Width = 200
        '.CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        '.Visible = False
        'End With
        With grilla.RootTable.Columns("tokg")
            .Caption = "OBSERVACION"
            .Width = 200
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With


        With grilla

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

            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowPosition = TotalRowPosition.BottomFixed
        End With
        For i = 0 To CType(grilla.DataSource, DataTable).Columns.Count - 1 Step 1

            If i < 15 Then
                Dim columnHeader As GridEXColumn = grilla.RootTable.Columns(i)
                columnHeader.HeaderStyle.ForeColor = Color.Black
                grilla.Refresh()
            ElseIf i < 20 Then
                Dim columnHeader As GridEXColumn = grilla.RootTable.Columns(i)
                columnHeader.HeaderStyle.ForeColor = Color.Red
                grilla.Refresh()
            ElseIf i < 25 Then
                Dim columnHeader As GridEXColumn = grilla.RootTable.Columns(i)
                columnHeader.HeaderStyle.ForeColor = Color.Black
                grilla.Refresh()
            ElseIf i < 49 Then
                Dim columnHeader As GridEXColumn = grilla.RootTable.Columns(i)
                columnHeader.HeaderStyle.ForeColor = Color.Blue
                grilla.Refresh()
            Else
                Dim columnHeader As GridEXColumn = grilla.RootTable.Columns(i)
                columnHeader.HeaderStyle.ForeColor = Color.Black
                grilla.Refresh()
            End If

        Next
        ocultarVacios2(grilla)
    End Sub

    Private Sub CREARNUEVASALIDAToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CREARNUEVASALIDAToolStripMenuItem.Click
        tbSalida2.Visible = True
        SuperTabControl1.SelectedTab = tbSalida2

    End Sub

    Private Sub MBtSalir_Click(sender As Object, e As EventArgs) Handles MBtSalir.Click
        If MBtGrabar.Enabled = True Then
            Dim ef = New Efecto

            ef.tipo = 2
            ef.Context = "mensaje principal".ToUpper
            ef.Header = "Esta seguro que desea salir?, los cambios se perderán.".ToUpper
            ef.ShowDialog()
            Dim bandera As Boolean = False
            bandera = ef.band
            If (bandera = True) Then
                Dim mensajeError As String = ""
                Dim res As Boolean = True 'L_fnCategoriaEliminar(tbCodigo.Text)
                If res Then
                    Limpiar()
                    InHabilitar()
                    If CType(grBuscador.DataSource, DataTable).Rows.Count > 0 Then
                        LLenarDatos(grBuscador.Row)
                        'TraerPendientes()

                        CargarVistasSalidas()
                    End If
                    CargarHojaPedidosPendientes(grPendientes, 0, CType(grPedidos4.DataSource, DataTable))
                    MSuperTabControlPrincipal.Enabled = True
                Else
                    Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
                    ToastNotification.Show(Me, "EL REGISTRO NO PUDO SER ELIMINADO", img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                End If
            End If

        Else
            Me.Close()
        End If
    End Sub

    Private Sub grPedidos2_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grPedidos2.CellValueChanged
        If ((e.Column.Key.Equals("check1"))) Then
            Dim bit As Boolean = grPedidos2.GetValue("check1")

            If bit Then
                Dim act As Double = CDbl(tbPesSel2.Text)

                act = act + grPedidos2.GetValue("kg")
                tbPesSel2.Text = act.ToString
            Else
                Dim act As Double = CDbl(tbPesSel2.Text)

                act = act - grPedidos2.GetValue("kg")
                tbPesSel2.Text = act.ToString
            End If


        End If


    End Sub

    Private Sub cbZona_ValueChanged(sender As Object, e As EventArgs) Handles cbZona.ValueChanged
        If MBtNuevo.Enabled = False Then

            Dim dt As DataTable = traerRepartidorZona(cbZona.Value)
            If dt.Rows.Count > 0 Then
                cbRepartidor.Value = dt.Rows(0).Item("cbnumi")
            End If
            CargarHojaPedidos(grPedidos, -1)
            CargarHojaPedidos(grPedidos2, -1)
            CargarHojaPedidos(grPedidos3, -1)
            CargarHojaPedidos(grPedidos4, -1)
            TraerPendientes()
        End If
    End Sub

    Private Sub MBtImprimir_Click(sender As Object, e As EventArgs) Handles MBtImprimir.Click
        ImprimirComandas()
    End Sub

    Private Sub ImprimirComandas()
        Dim est1 = 0
        Dim est2 = 0
        Dim salidaS As String
        Dim _Ds3 As New DataSet
        _Ds3 = L_ObtenerRutaImpresora("1")
        Dim cont As Integer = 0
        Dim dt As DataTable
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If

        P_Global.Visualizador = New Visualizador
        Dim objrep As New R_Comanda

        If SuperTabControl1.SelectedTab Is tbSalida1 Then
            salidaS = "PRIMERA SALIDA"
            For i = 0 To CType(grPedidos.DataSource, DataTable).Rows.Count - 1 Step 1
                dt = TraerComanda(CType(grPedidos.DataSource, DataTable).Rows(i).Item("tooanumi"))
                est1 = 0
                est2 = 0
                For Each fila As DataRow In dt.Rows
                    If fila("tipo1") = 1 Then
                        If est1 = 0 Then
                            Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 1")
                            Dim dt1 As DataTable = dt.Clone()
                            dt1.Clear()
                            For Each fila1 As DataRow In resultadoFilas
                                dt1.ImportRow(fila1)
                            Next
                            objrep.SetDataSource(dt1)
                            objrep.SetParameterValue("chofer", cbRepartidor.Text)
                            objrep.SetParameterValue("salida", salidaS)
                            objrep.SetParameterValue("codigo", CType(grPedidos.DataSource, DataTable).Rows(i).Item("tonro"))
                            ' Finalmente, puedes mostrar el informe en el visor de informes
                            'P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
                            'P_Global.Visualizador.ShowDialog() 'Comentar
                            'P_Global.Visualizador.BringToFront()
                            Dim pd As New PrintDocument()
                            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                            If (Not pd.PrinterSettings.IsValid) Then
                                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
                            Else
                                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                                objrep.PrintToPrinter(1, False, 1, 1)
                            End If
                            'objrep.SetParameterValue("chofer" + (cont + 1).ToString, cbRepartidor.Text)
                            'objrep.SetParameterValue("cliente1".ToString, CType(grPedidos.DataSource, DataTable).Rows(i).Item("ccdesc"))
                            cont = cont + 1
                            est1 = 1
                        End If
                    ElseIf fila("tipo1") = 2 Then
                        If est2 = 0 Then
                            Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 2")
                            Dim dt1 As DataTable = dt.Clone()
                            dt1.Clear()
                            For Each fila1 As DataRow In resultadoFilas
                                dt1.ImportRow(fila1)
                            Next
                            objrep.SetDataSource(dt1)
                            objrep.SetParameterValue("chofer", cbRepartidor.Text)
                            objrep.SetParameterValue("salida", salidaS)
                            objrep.SetParameterValue("codigo", CType(grPedidos.DataSource, DataTable).Rows(i).Item("tonro"))
                            ' Finalmente, puedes mostrar el informe en el visor de informes
                            'P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
                            'P_Global.Visualizador.ShowDialog() 'Comentar
                            'P_Global.Visualizador.BringToFront()
                            Dim pd As New PrintDocument()
                            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                            If (Not pd.PrinterSettings.IsValid) Then
                                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
                            Else
                                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                                objrep.PrintToPrinter(1, False, 1, 1)
                            End If
                            'objrep.SetParameterValue("chofer" + (cont + 1).ToString, cbRepartidor.Text)
                            'objrep.SetParameterValue("cliente1".ToString, CType(grPedidos.DataSource, DataTable).Rows(i).Item("ccdesc"))
                            cont = cont + 1
                            est2 = 1
                        End If
                    Else

                        Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 3")
                        Dim dtaux As DataTable = dt.Clone()
                        dtaux.Clear()
                        For Each fila1 As DataRow In resultadoFilas
                            dtaux.ImportRow(fila1)
                        Next
                        objrep.SetDataSource(dtaux)
                        objrep.SetParameterValue("chofer", cbRepartidor.Text)
                        objrep.SetParameterValue("salida", salidaS)
                        objrep.SetParameterValue("codigo", CType(grPedidos.DataSource, DataTable).Rows(i).Item("tonro"))
                        ' Finalmente, puedes mostrar el informe en el visor de informes
                        'P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
                        'P_Global.Visualizador.ShowDialog() 'Comentar
                        'P_Global.Visualizador.BringToFront()
                        Dim pd As New PrintDocument()
                        pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                        If (Not pd.PrinterSettings.IsValid) Then
                            ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                   My.Resources.WARNING, 5 * 1000,
                                   eToastGlowColor.Blue, eToastPosition.BottomRight)
                        Else
                            objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                            objrep.PrintToPrinter(1, False, 1, 1)
                        End If
                        'objrep.SetParameterValue("chofer" + (cont + 1).ToString, cbRepartidor.Text)
                        'crSubreportDocument.SetParameterValue("cliente2", CType(grPedidos.DataSource, DataTable).Rows(i).Item("ccdesc"))

                        cont = cont + 1
                        Exit For
                    End If
                Next
            Next
        ElseIf SuperTabControl1.SelectedTab Is tbSalida2 Then
            salidaS = "SEGUNDA SALIDA"
            For i = 0 To CType(grPedidos2.DataSource, DataTable).Rows.Count - 1 Step 1
                dt = TraerComanda(CType(grPedidos2.DataSource, DataTable).Rows(i).Item("tooanumi"))
                For Each fila As DataRow In dt.Rows
                    If fila("tipo1") = 1 Then
                        If est1 = 0 Then
                            Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 1")
                            Dim dt1 As DataTable = dt.Clone()
                            dt1.Clear()
                            For Each fila1 As DataRow In resultadoFilas
                                dt1.ImportRow(fila1)
                            Next
                            objrep.SetDataSource(dt1)
                            objrep.SetParameterValue("chofer", cbRepartidor.Text)
                            objrep.SetParameterValue("salida", salidaS)
                            objrep.SetParameterValue("codigo", CType(grPedidos.DataSource, DataTable).Rows(i).Item("tonro"))
                            ' Finalmente, puedes mostrar el informe en el visor de informes
                            'P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
                            'P_Global.Visualizador.ShowDialog() 'Comentar
                            'P_Global.Visualizador.BringToFront()
                            Dim pd As New PrintDocument()
                            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                            If (Not pd.PrinterSettings.IsValid) Then
                                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
                            Else
                                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                                objrep.PrintToPrinter(1, False, 1, 1)
                            End If
                            'objrep.SetParameterValue("chofer" + (cont + 1).ToString, cbRepartidor.Text)
                            'objrep.SetParameterValue("cliente1".ToString, CType(grPedidos.DataSource, DataTable).Rows(i).Item("ccdesc"))
                            cont = cont + 1
                            est1 = 1
                        End If
                    ElseIf fila("tipo1") = 2 Then
                        If est2 = 0 Then
                            Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 2")
                            Dim dt1 As DataTable = dt.Clone()
                            dt1.Clear()
                            For Each fila1 As DataRow In resultadoFilas
                                dt1.ImportRow(fila1)
                            Next
                            objrep.SetDataSource(dt1)
                            objrep.SetParameterValue("chofer", cbRepartidor.Text)
                            objrep.SetParameterValue("salida", salidaS)
                            objrep.SetParameterValue("codigo", CType(grPedidos.DataSource, DataTable).Rows(i).Item("tonro"))
                            ' Finalmente, puedes mostrar el informe en el visor de informes
                            'P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
                            'P_Global.Visualizador.ShowDialog() 'Comentar
                            'P_Global.Visualizador.BringToFront()
                            Dim pd As New PrintDocument()
                            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                            If (Not pd.PrinterSettings.IsValid) Then
                                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
                            Else
                                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                                objrep.PrintToPrinter(1, False, 1, 1)
                            End If
                            'objrep.SetParameterValue("chofer" + (cont + 1).ToString, cbRepartidor.Text)
                            'objrep.SetParameterValue("cliente1".ToString, CType(grPedidos.DataSource, DataTable).Rows(i).Item("ccdesc"))
                            cont = cont + 1
                            est2 = 1
                        End If
                    Else

                        Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 3")
                        Dim dtaux As DataTable = dt.Clone()
                        dtaux.Clear()
                        For Each fila1 As DataRow In resultadoFilas
                            dtaux.ImportRow(fila1)
                        Next
                        objrep.SetDataSource(dtaux)
                        objrep.SetParameterValue("chofer", cbRepartidor.Text)
                        objrep.SetParameterValue("salida", salidaS)
                        objrep.SetParameterValue("codigo", CType(grPedidos.DataSource, DataTable).Rows(i).Item("tonro"))
                        ' Finalmente, puedes mostrar el informe en el visor de informes
                        'P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
                        'P_Global.Visualizador.ShowDialog() 'Comentar
                        'P_Global.Visualizador.BringToFront()
                        Dim pd As New PrintDocument()
                        pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                        If (Not pd.PrinterSettings.IsValid) Then
                            ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                   My.Resources.WARNING, 5 * 1000,
                                   eToastGlowColor.Blue, eToastPosition.BottomRight)
                        Else
                            objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                            objrep.PrintToPrinter(1, False, 1, 1)
                        End If
                        'objrep.SetParameterValue("chofer" + (cont + 1).ToString, cbRepartidor.Text)
                        'crSubreportDocument.SetParameterValue("cliente2", CType(grPedidos.DataSource, DataTable).Rows(i).Item("ccdesc"))

                        cont = cont + 1
                        Exit For
                    End If
                Next
            Next


        ElseIf SuperTabControl1.SelectedTab Is tbSalida3 Then
            salidaS = "TERCERA SALIDA"
            For i = 0 To CType(grPedidos3.DataSource, DataTable).Rows.Count - 1 Step 1
                dt = TraerComanda(CType(grPedidos3.DataSource, DataTable).Rows(i).Item("tooanumi"))
                For Each fila As DataRow In dt.Rows
                    If fila("tipo1") = 1 Then
                        If est1 = 0 Then
                            Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 1")
                            Dim dt1 As DataTable = dt.Clone()
                            dt1.Clear()
                            For Each fila1 As DataRow In resultadoFilas
                                dt1.ImportRow(fila1)
                            Next
                            objrep.SetDataSource(dt1)
                            objrep.SetParameterValue("chofer", cbRepartidor.Text)
                            objrep.SetParameterValue("salida", salidaS)
                            objrep.SetParameterValue("codigo", CType(grPedidos.DataSource, DataTable).Rows(i).Item("tonro"))
                            ' Finalmente, puedes mostrar el informe en el visor de informes
                            'P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
                            'P_Global.Visualizador.ShowDialog() 'Comentar
                            'P_Global.Visualizador.BringToFront()
                            Dim pd As New PrintDocument()
                            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                            If (Not pd.PrinterSettings.IsValid) Then
                                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
                            Else
                                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                                objrep.PrintToPrinter(1, False, 1, 1)
                            End If
                            'objrep.SetParameterValue("chofer" + (cont + 1).ToString, cbRepartidor.Text)
                            'objrep.SetParameterValue("cliente1".ToString, CType(grPedidos.DataSource, DataTable).Rows(i).Item("ccdesc"))
                            cont = cont + 1
                            est1 = 1
                        End If
                    ElseIf fila("tipo1") = 2 Then
                        If est2 = 0 Then
                            Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 2")
                            Dim dt1 As DataTable = dt.Clone()
                            dt1.Clear()
                            For Each fila1 As DataRow In resultadoFilas
                                dt1.ImportRow(fila1)
                            Next
                            objrep.SetDataSource(dt1)
                            objrep.SetParameterValue("chofer", cbRepartidor.Text)
                            objrep.SetParameterValue("salida", salidaS)
                            objrep.SetParameterValue("codigo", CType(grPedidos.DataSource, DataTable).Rows(i).Item("tonro"))
                            ' Finalmente, puedes mostrar el informe en el visor de informes
                            'P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
                            'P_Global.Visualizador.ShowDialog() 'Comentar
                            'P_Global.Visualizador.BringToFront()
                            Dim pd As New PrintDocument()
                            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                            If (Not pd.PrinterSettings.IsValid) Then
                                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
                            Else
                                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                                objrep.PrintToPrinter(1, False, 1, 1)
                            End If
                            'objrep.SetParameterValue("chofer" + (cont + 1).ToString, cbRepartidor.Text)
                            'objrep.SetParameterValue("cliente1".ToString, CType(grPedidos.DataSource, DataTable).Rows(i).Item("ccdesc"))
                            cont = cont + 1
                            est2 = 1
                        End If
                    Else

                        Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 3")
                        Dim dtaux As DataTable = dt.Clone()
                        dtaux.Clear()
                        For Each fila1 As DataRow In resultadoFilas
                            dtaux.ImportRow(fila1)
                        Next
                        objrep.SetDataSource(dtaux)
                        objrep.SetParameterValue("chofer", cbRepartidor.Text)
                        objrep.SetParameterValue("salida", salidaS)
                        objrep.SetParameterValue("codigo", CType(grPedidos.DataSource, DataTable).Rows(i).Item("tonro"))
                        ' Finalmente, puedes mostrar el informe en el visor de informes
                        'P_Global.Visualizador.CRV1.ReportSource = objrep 'Comentar
                        'P_Global.Visualizador.ShowDialog() 'Comentar
                        'P_Global.Visualizador.BringToFront()
                        Dim pd As New PrintDocument()
                        pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                        If (Not pd.PrinterSettings.IsValid) Then
                            ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                   My.Resources.WARNING, 5 * 1000,
                                   eToastGlowColor.Blue, eToastPosition.BottomRight)
                        Else
                            objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
                            objrep.PrintToPrinter(1, False, 1, 1)
                        End If
                        'objrep.SetParameterValue("chofer" + (cont + 1).ToString, cbRepartidor.Text)
                        'crSubreportDocument.SetParameterValue("cliente2", CType(grPedidos.DataSource, DataTable).Rows(i).Item("ccdesc"))

                        cont = cont + 1
                        Exit For
                    End If
                Next
            Next

        End If
        'If SuperTabControl1.SelectedTab Is tbSalida1 Then
        '    For i = 0 To CType(grPedidos.DataSource, DataTable).Rows.Count - 1 Step 1
        '        dt = TraerComanda(CType(grPedidos.DataSource, DataTable).Rows(i).Item("tooanumi"))
        '        For Each fila As DataRow In dt.Rows
        '            If fila("tipo1") = 1 Or fila("tipo1") = 2 Then
        '                Dim dt1 As DataTable = dt.Clone()
        '                dt1.Clear()
        '                dt1.ImportRow(fila)
        '                objrep.SetDataSource(dt1)
        '                objrep.SetParameterValue("chofer", cbRepartidor.Text)
        '                ' Finalmente, puedes mostrar el informe en el visor de informes
        '                Dim pd As New PrintDocument()
        '                pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
        '                If (Not pd.PrinterSettings.IsValid) Then
        '                    ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
        '                               My.Resources.WARNING, 5 * 1000,
        '                               eToastGlowColor.Blue, eToastPosition.BottomRight)
        '                Else
        '                    objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
        '                    objrep.PrintToPrinter(1, False, 1, 1)
        '                End If

        '                cont = cont + 1
        '            Else

        '                Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 3")
        '                Dim dtaux As DataTable = dt.Clone()
        '                dtaux.Clear()
        '                For Each fila1 As DataRow In resultadoFilas
        '                    dtaux.ImportRow(fila1)
        '                Next
        '                objrep.SetDataSource(dtaux)
        '                objrep.SetParameterValue("chofer", cbRepartidor.Text)
        '                ' Finalmente, puedes mostrar el informe en el visor de informes
        '                Dim pd As New PrintDocument()
        '                pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
        '                If (Not pd.PrinterSettings.IsValid) Then
        '                    ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
        '                               My.Resources.WARNING, 5 * 1000,
        '                               eToastGlowColor.Blue, eToastPosition.BottomRight)
        '                Else
        '                    objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
        '                    objrep.PrintToPrinter(1, False, 1, 1)
        '                End If
        '                'objrep.SetParameterValue("chofer" + (cont + 1).ToString, cbRepartidor.Text)
        '                'crSubreportDocument.SetParameterValue("cliente2", CType(grPedidos.DataSource, DataTable).Rows(i).Item("ccdesc"))

        '                cont = cont + 1
        '                Exit For
        '            End If
        '        Next
        '    Next
        'ElseIf SuperTabControl1.SelectedTab Is tbSalida2 Then
        '    For i = 0 To CType(grPedidos2.DataSource, DataTable).Rows.Count - 1 Step 1
        '        dt = TraerComanda(CType(grPedidos2.DataSource, DataTable).Rows(i).Item("tooanumi"))
        '        For Each fila As DataRow In dt.Rows
        '            If fila("tipo1") = 1 Or ("tipo1") = 2 Then
        '                Dim dt1 As DataTable = dt.Clone()
        '                dt1.Clear()
        '                dt1.ImportRow(fila)
        '                objrep.SetDataSource(dt1)
        '                objrep.SetParameterValue("chofer", cbRepartidor.Text)
        '                ' Finalmente, puedes mostrar el informe en el visor de informes
        '                Dim pd As New PrintDocument()
        '                pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
        '                If (Not pd.PrinterSettings.IsValid) Then
        '                    ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
        '                               My.Resources.WARNING, 5 * 1000,
        '                               eToastGlowColor.Blue, eToastPosition.BottomRight)
        '                Else
        '                    objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
        '                    objrep.PrintToPrinter(1, False, 1, 1)
        '                End If
        '                cont = cont + 1
        '            Else

        '                Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 3")
        '                Dim dtaux As DataTable = dt.Clone()
        '                dtaux.Clear()
        '                For Each fila1 As DataRow In resultadoFilas
        '                    dtaux.ImportRow(fila1)
        '                Next
        '                objrep.SetDataSource(dtaux)
        '                objrep.SetParameterValue("chofer", cbRepartidor.Text)
        '                ' Finalmente, puedes mostrar el informe en el visor de informes
        '                Dim pd As New PrintDocument()
        '                pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
        '                If (Not pd.PrinterSettings.IsValid) Then
        '                    ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
        '                               My.Resources.WARNING, 5 * 1000,
        '                               eToastGlowColor.Blue, eToastPosition.BottomRight)
        '                Else
        '                    objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
        '                    objrep.PrintToPrinter(1, False, 1, 1)
        '                End If
        '                cont = cont + 1
        '                Exit For
        '            End If
        '        Next
        '    Next


        'ElseIf SuperTabControl1.SelectedTab Is tbSalida3 Then
        '    For i = 0 To CType(grPedidos3.DataSource, DataTable).Rows.Count - 1 Step 1
        '        dt = TraerComanda(CType(grPedidos3.DataSource, DataTable).Rows(i).Item("tooanumi"))
        '        For Each fila As DataRow In dt.Rows
        '            If fila("tipo1") = 1 Or ("tipo1") = 2 Then
        '                Dim dt1 As DataTable = dt.Clone()
        '                dt1.Clear()
        '                dt1.ImportRow(fila)
        '                objrep.SetDataSource(dt1)
        '                objrep.SetParameterValue("chofer", cbRepartidor.Text)
        '                ' Finalmente, puedes mostrar el informe en el visor de informes
        '                Dim pd As New PrintDocument()
        '                pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
        '                If (Not pd.PrinterSettings.IsValid) Then
        '                    ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
        '                               My.Resources.WARNING, 5 * 1000,
        '                               eToastGlowColor.Blue, eToastPosition.BottomRight)
        '                Else
        '                    objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
        '                    objrep.PrintToPrinter(1, False, 1, 1)
        '                End If
        '                cont = cont + 1
        '            Else

        '                Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 3")
        '                Dim dtaux As DataTable = dt.Clone()
        '                dtaux.Clear()
        '                For Each fila1 As DataRow In resultadoFilas
        '                    dtaux.ImportRow(fila1)
        '                Next
        '                objrep.SetDataSource(dtaux)
        '                objrep.SetParameterValue("chofer", cbRepartidor.Text)
        '                ' Finalmente, puedes mostrar el informe en el visor de informes

        '                Dim pd As New PrintDocument()
        '                pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
        '                If (Not pd.PrinterSettings.IsValid) Then
        '                    ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
        '                               My.Resources.WARNING, 5 * 1000,
        '                               eToastGlowColor.Blue, eToastPosition.BottomRight)
        '                Else
        '                    objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
        '                    objrep.PrintToPrinter(1, False, 1, 1)
        '                End If
        '                cont = cont + 1
        '                Exit For
        '            End If
        '        Next
        '    Next

        'End If


        ''objrep.SetParameterValue("chofer", cbRepartidor.Text)
        ' Finalmente, puedes mostrar el informe en el visor de informes
        'P_Global.Visualizador.CRV1.ReportSource = objrep
        'P_Global.Visualizador.ShowDialog()
        'P_Global.Visualizador.BringToFront()
    End Sub

    Private Sub ImprimirHojaRuta()
        Dim cont As Integer = 0
        Dim est1 As Integer = 0
        Dim est2 As Integer = 0
        Dim est3 As Integer = 0
        Dim salidaS As String
        Dim dt As DataTable
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If

        P_Global.Visualizador = New Visualizador
        Dim objrep As New R_HojadeRuta
        If SuperTabControl1.SelectedTab Is tbSalida1 Then
            For i = 0 To CType(grPedidos.DataSource, DataTable).Rows.Count - 1 Step 1
                salidaS = "PRIMERA SALIDA"
                dt = TraerComanda(CType(grPedidos.DataSource, DataTable).Rows(i).Item("tooanumi"))
                est1 = 0
                est2 = 0
                For Each fila As DataRow In dt.Rows
                    If fila("tipo1") = 1 Then
                        If est1 = 0 Then
                            Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 1")
                            Dim dt1 As DataTable = dt.Clone()
                            dt1.Clear()
                            For Each fila1 As DataRow In resultadoFilas
                                dt1.ImportRow(fila1)
                            Next
                            Dim reporte As String = "Subreport" + (cont + 1).ToString
                            Dim reporte1 As String
                            If cont = 0 Then
                                reporte1 = "R_RepComanda.rpt"
                            Else
                                reporte1 = "R_RepComanda.rpt - " + (Format(cont, "00")).ToString
                            End If
                            Dim crSubreportObject As SubreportObject = CType(objrep.ReportDefinition.ReportObjects(reporte), SubreportObject)
                            Dim crSubreportDocument As ReportDocument = crSubreportObject.OpenSubreport(reporte1)
                            crSubreportDocument.SetDataSource(dt1)
                            'objrep.SetParameterValue("chofer" + (cont + 1).ToString, cbRepartidor.Text)
                            'objrep.SetParameterValue("cliente1".ToString, CType(grPedidos.DataSource, DataTable).Rows(i).Item("ccdesc"))
                            cont = cont + 1
                            est1 = 1
                        End If
                    ElseIf fila("tipo1") = 2 Then
                        If est2 = 0 Then
                            Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 2")
                            Dim dt1 As DataTable = dt.Clone()
                            dt1.Clear()
                            For Each fila1 As DataRow In resultadoFilas
                                dt1.ImportRow(fila1)
                            Next
                            Dim reporte As String = "Subreport" + (cont + 1).ToString
                            Dim reporte1 As String
                            If cont = 0 Then
                                reporte1 = "R_RepComanda.rpt"
                            Else
                                reporte1 = "R_RepComanda.rpt - " + (Format(cont, "00")).ToString
                            End If
                            Dim crSubreportObject As SubreportObject = CType(objrep.ReportDefinition.ReportObjects(reporte), SubreportObject)
                            Dim crSubreportDocument As ReportDocument = crSubreportObject.OpenSubreport(reporte1)
                            crSubreportDocument.SetDataSource(dt1)
                            'objrep.SetParameterValue("chofer" + (cont + 1).ToString, cbRepartidor.Text)
                            'objrep.SetParameterValue("cliente1".ToString, CType(grPedidos.DataSource, DataTable).Rows(i).Item("ccdesc"))
                            cont = cont + 1
                            est2 = 1
                        End If
                    Else

                        Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 3")
                        Dim dtaux As DataTable = dt.Clone()
                        dtaux.Clear()
                        For Each fila1 As DataRow In resultadoFilas
                            dtaux.ImportRow(fila1)
                        Next
                        Dim reporte As String = "Subreport" + (cont + 1).ToString
                        Dim reporte1 As String
                        If cont = 0 Then
                            reporte1 = "R_RepComanda.rpt"
                        Else
                            reporte1 = "R_RepComanda.rpt - " + (Format(cont, "00")).ToString
                        End If
                        Dim crSubreportObject As SubreportObject = CType(objrep.ReportDefinition.ReportObjects(reporte), SubreportObject)
                        Dim crSubreportDocument As ReportDocument = crSubreportObject.OpenSubreport(reporte1)
                        crSubreportDocument.SetDataSource(dtaux)
                        'objrep.SetParameterValue("chofer" + (cont + 1).ToString, cbRepartidor.Text)
                        'crSubreportDocument.SetParameterValue("cliente2", CType(grPedidos.DataSource, DataTable).Rows(i).Item("ccdesc"))

                        cont = cont + 1
                        Exit For
                    End If
                Next
            Next
        ElseIf SuperTabControl1.SelectedTab Is tbSalida2 Then
            salidaS = "SEGUNDA SALIDA"
            For i = 0 To CType(grPedidos2.DataSource, DataTable).Rows.Count - 1 Step 1
                dt = TraerComanda(CType(grPedidos2.DataSource, DataTable).Rows(i).Item("tooanumi"))
                For Each fila As DataRow In dt.Rows
                    If fila("tipo1") = 1 Or ("tipo1") = 2 Then
                        Dim dt1 As DataTable = dt.Clone()
                        dt1.Clear()
                        dt1.ImportRow(fila)
                        Dim reporte As String = "Subreport" + (cont + 1).ToString
                        Dim reporte1 As String
                        If cont = 0 Then
                            reporte1 = "R_RepComanda.rpt"
                        Else
                            reporte1 = "R_RepComanda.rpt - " + (Format(cont, "00")).ToString
                        End If
                        Dim crSubreportObject As SubreportObject = CType(objrep.ReportDefinition.ReportObjects(reporte), SubreportObject)
                        Dim crSubreportDocument As ReportDocument = crSubreportObject.OpenSubreport(reporte1)
                        crSubreportDocument.SetDataSource(dt1)
                        cont = cont + 1
                    Else

                        Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 3")
                        Dim dtaux As DataTable = dt.Clone()
                        dtaux.Clear()
                        For Each fila1 As DataRow In resultadoFilas
                            dtaux.ImportRow(fila1)
                        Next
                        Dim reporte As String = "Subreport" + (cont + 1).ToString
                        Dim reporte1 As String
                        If cont = 0 Then
                            reporte1 = "R_RepComanda.rpt"
                        Else
                            reporte1 = "R_RepComanda.rpt - " + (Format(cont, "00")).ToString
                        End If
                        Dim crSubreportObject As SubreportObject = CType(objrep.ReportDefinition.ReportObjects(reporte), SubreportObject)
                        Dim crSubreportDocument As ReportDocument = crSubreportObject.OpenSubreport(reporte1)
                        crSubreportDocument.SetDataSource(dtaux)
                        cont = cont + 1
                        Exit For
                    End If
                Next
            Next


        ElseIf SuperTabControl1.SelectedTab Is tbSalida3 Then
            salidaS = "TERCERA SALIDA"
            For i = 0 To CType(grPedidos3.DataSource, DataTable).Rows.Count - 1 Step 1
                dt = TraerComanda(CType(grPedidos3.DataSource, DataTable).Rows(i).Item("tooanumi"))
                For Each fila As DataRow In dt.Rows
                    If fila("tipo1") = 1 Or ("tipo1") = 2 Then
                        Dim dt1 As DataTable = dt.Clone()
                        dt1.Clear()
                        dt1.ImportRow(fila)
                        Dim reporte As String = "Subreport" + (cont + 1).ToString
                        Dim reporte1 As String
                        If cont = 0 Then
                            reporte1 = "R_RepComanda.rpt"
                        Else
                            reporte1 = "R_RepComanda.rpt - " + (Format(cont, "00")).ToString
                        End If
                        Dim crSubreportObject As SubreportObject = CType(objrep.ReportDefinition.ReportObjects(reporte), SubreportObject)
                        Dim crSubreportDocument As ReportDocument = crSubreportObject.OpenSubreport(reporte1)
                        crSubreportDocument.SetDataSource(dt1)
                        cont = cont + 1
                    Else

                        Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 3")
                        Dim dtaux As DataTable = dt.Clone()
                        dtaux.Clear()
                        For Each fila1 As DataRow In resultadoFilas
                            dtaux.ImportRow(fila1)
                        Next
                        Dim reporte As String = "Subreport" + (cont + 1).ToString
                        Dim reporte1 As String
                        If cont = 0 Then
                            reporte1 = "R_RepComanda.rpt"
                        Else
                            reporte1 = "R_RepComanda.rpt - " + (Format(cont, "00")).ToString
                        End If
                        Dim crSubreportObject As SubreportObject = CType(objrep.ReportDefinition.ReportObjects(reporte), SubreportObject)
                        Dim crSubreportDocument As ReportDocument = crSubreportObject.OpenSubreport(reporte1)
                        crSubreportDocument.SetDataSource(dtaux)
                        cont = cont + 1
                        Exit For
                    End If
                Next
            Next

        End If
        For i = cont + 1 To 35 Step 1
            Dim repor As String = "Subreport" + (i).ToString
            Dim crSubreportObject As CrystalDecisions.CrystalReports.Engine.SubreportObject
            crSubreportObject = objrep.ReportDefinition.ReportObjects(repor)

            ' Establecemos la propiedad Visible del objeto SubreportObject en False para ocultar el subinforme
            crSubreportObject.ObjectFormat.EnableSuppress = True
            ' dtDatosSubInforme es el DataTable que contiene los datos para el subinforme
        Next

        objrep.SetParameterValue("chofer", cbRepartidor.Text)
        objrep.SetParameterValue("salida", salidaS)
        ' Finalmente, puedes mostrar el informe en el visor de informes
        P_Global.Visualizador.CRV1.ReportSource = objrep
        P_Global.Visualizador.Show()
        P_Global.Visualizador.BringToFront()
    End Sub

    Private Sub ImprimirHojaRuta2()
        Dim cont As Integer = 0
        Dim dt As DataTable
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If

        P_Global.Visualizador = New Visualizador
        Dim objrep As New R_ComandaEtiqueta
        If SuperTabControl1.SelectedTab Is tbSalida1 Then
            For i = 0 To CType(grPedidos.DataSource, DataTable).Rows.Count - 1 Step 1
                dt = TraerComanda(CType(grPedidos.DataSource, DataTable).Rows(i).Item("tooanumi"))
                For Each fila As DataRow In dt.Rows
                    If fila("tipo1") = 1 Then
                        Dim dt1 As DataTable = dt.Clone()
                        dt1.Clear()
                        dt1.ImportRow(fila)
                        objrep.SetDataSource(dt1)
                        objrep.SetParameterValue("chofer", cbRepartidor.Text)
                        P_Global.Visualizador.CRV1.ReportSource = objrep
                        P_Global.Visualizador.ShowDialog()
                        P_Global.Visualizador.BringToFront()

                        'objrep.SetParameterValue("cliente1".ToString, CType(grPedidos.DataSource, DataTable).Rows(i).Item("ccdesc"))
                        cont = cont + 1
                    Else

                        Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 2")
                        Dim dtaux As DataTable = dt.Clone()
                        dtaux.Clear()
                        For Each fila1 As DataRow In resultadoFilas
                            dtaux.ImportRow(fila1)
                        Next
                        objrep.SetDataSource(dtaux)
                        objrep.SetParameterValue("chofer", cbRepartidor.Text)
                        P_Global.Visualizador.CRV1.ReportSource = objrep
                        P_Global.Visualizador.ShowDialog()
                        P_Global.Visualizador.BringToFront()

                        'crSubreportDocument.SetParameterValue("cliente2", CType(grPedidos.DataSource, DataTable).Rows(i).Item("ccdesc"))

                        cont = cont + 1
                        Exit For
                    End If
                Next
            Next
        ElseIf SuperTabControl1.SelectedTab Is tbSalida2 Then
            For i = 0 To CType(grPedidos2.DataSource, DataTable).Rows.Count - 1 Step 1
                dt = TraerComanda(CType(grPedidos2.DataSource, DataTable).Rows(i).Item("tooanumi"))
                For Each fila As DataRow In dt.Rows
                    If fila("tipo1") = 1 Then
                        Dim dt1 As DataTable = dt.Clone()
                        dt1.Clear()
                        dt1.ImportRow(fila)
                        objrep.SetDataSource(dt1)
                        objrep.SetParameterValue("chofer", cbRepartidor.Text)
                        P_Global.Visualizador.CRV1.ReportSource = objrep
                        P_Global.Visualizador.ShowDialog()
                        P_Global.Visualizador.BringToFront()
                        cont = cont + 1
                    Else

                        Dim resultadoFilas() As DataRow = dt.Select("tipo1 = 2")
                        Dim dtaux As DataTable = dt.Clone()
                        dtaux.Clear()
                        For Each fila1 As DataRow In resultadoFilas
                            dtaux.ImportRow(fila1)
                        Next
                        objrep.SetDataSource(dtaux)
                        objrep.SetParameterValue("chofer", cbRepartidor.Text)
                        P_Global.Visualizador.CRV1.ReportSource = objrep
                        P_Global.Visualizador.ShowDialog()
                        P_Global.Visualizador.BringToFront()
                        cont = cont + 1
                        Exit For
                    End If
                Next
            Next

        End If

        ' Finalmente, puedes mostrar el informe en el visor de informes

    End Sub

    Private Sub MBtPrimero_Click(sender As Object, e As EventArgs) Handles MBtPrimero.Click
        If (grBuscador.RowCount > 0) Then
            grBuscador.MoveFirst()
        End If
    End Sub

    Private Sub MBtAnterior_Click(sender As Object, e As EventArgs) Handles MBtAnterior.Click
        If (grBuscador.RowCount > 0) Then
            grBuscador.MovePrevious()
        End If
    End Sub

    Private Sub MBtSiguiente_Click(sender As Object, e As EventArgs) Handles MBtSiguiente.Click
        If (grBuscador.RowCount > 0) Then
            grBuscador.MoveNext()
        End If
    End Sub

    Private Sub MBtUltimo_Click(sender As Object, e As EventArgs) Handles MBtUltimo.Click
        If (grBuscador.RowCount > 0) Then
            grBuscador.MoveLast()
        End If
    End Sub

    Private Sub btCargarHoja_Click(sender As Object, e As EventArgs) Handles btCargarHoja.Click
        ImprimirHojaRutaxChofer()
    End Sub

    Private Sub ImprimirHojaRutaxChofer()
        Dim HSalida As String
        Dim HLlegada As String
        Dim detalle As String
        Dim salida As Integer
        If SuperTabControl1.SelectedTab Is tbSalida1 Then
            salida = 1
            detalle = "PRIMERA SALIDA"
            HSalida = tbHoraS.Value.ToString("HH:mm")
            HLlegada = tbHoraL.Value.ToString("HH:mm")
        ElseIf SuperTabControl1.SelectedTab Is tbSalida2 Then
            salida = 2
            detalle = "SEGUNDA SALIDA"
            HSalida = tbHoraS2.Value.ToString("HH:mm")
            HLlegada = tbHoraL2.Value.ToString("HH:mm")
        ElseIf SuperTabControl1.SelectedTab Is tbSalida3 Then
            salida = 3
            detalle = "TERCER SALIDA"
            HSalida = tbHoraS3.Value.ToString("HH:mm")
            HLlegada = tbHoraL3.Value.ToString("HH:mm")
        ElseIf SuperTabControl1.SelectedTab Is tbSalida4 Then
            salida = 4
            detalle = "CUARTA SALIDA"
            HSalida = tbHoraS4.Value.ToString("HH:mm")
            HLlegada = tbHoraL4.Value.ToString("HH:mm")
        End If
        Dim dt As DataTable = ImprimirSalida(CInt(tbCodigo.Text), salida)
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If
        'detalle = detalle + "    ZONA: " + cbZona.Text + "    CHOFER: " + cbRepartidor.Text
        P_Global.Visualizador = New Visualizador
        Dim objrep As New R_HojaRutaxChofer

        Dim dt1 As DataTable = TraerTotales(CInt(tbCodigo.Text), salida)

        Dim crSubreportObject As SubreportObject = CType(objrep.ReportDefinition.ReportObjects("Subreport1"), SubreportObject)
        Dim crSubreportDocument As ReportDocument = crSubreportObject.OpenSubreport("R_RepComandaTotales.rpt")
        crSubreportDocument.SetDataSource(dt1)

        Dim crSubreportObject2 As SubreportObject = CType(objrep.ReportDefinition.ReportObjects("Subreport2"), SubreportObject)
        Dim crSubreportDocument2 As ReportDocument = crSubreportObject2.OpenSubreport("R_RepComandaTotalesG.rpt")
        crSubreportDocument2.SetDataSource(dt1)

        Dim crSubreportObject3 As SubreportObject = CType(objrep.ReportDefinition.ReportObjects("Subreport3"), SubreportObject)
        Dim crSubreportDocument3 As ReportDocument = crSubreportObject3.OpenSubreport("R_RepComandaTotalesP.rpt")
        crSubreportDocument3.SetDataSource(dt1)



        objrep.SetDataSource(dt)
        objrep.SetParameterValue("detalle", detalle)
        objrep.SetParameterValue("zona", cbZona.Text)
        objrep.SetParameterValue("repartidor", cbRepartidor.Text)
        objrep.SetParameterValue("fecha", tbFecha.Value.ToString("dd/MM/yyyy"))
        objrep.SetParameterValue("horaS", HSalida)
        objrep.SetParameterValue("horaL", HLlegada)


        P_Global.Visualizador.CRV1.ReportSource = objrep
        P_Global.Visualizador.Show()
        P_Global.Visualizador.BringToFront()


    End Sub
    Private Sub TraerPendientes()
        Dim grillas As GridEX
        Dim peso As LabelX
        'If CType(grPedidos.DataSource, DataTable) Is Nothing Then
        If CType(grPedidos.DataSource, DataTable).Rows.Count = 0 Then
            grillas = grPedidos
            peso = tbPesTot
            'End If
            'ElseIf CType(grPedidos2.DataSource, DataTable) Is Nothing Then
        ElseIf CType(grPedidos2.DataSource, DataTable).Rows.Count = 0 Then
            grillas = grPedidos2
            peso = tbPesTot2
            'End If
            'ElseIf CType(grPedidos3.DataSource, DataTable) Is Nothing Then
        ElseIf CType(grPedidos3.DataSource, DataTable).Rows.Count = 0 Then
            grillas = grPedidos3
            'End If
            'peso = tbPesTot3
        ElseIf CType(grPedidos4.DataSource, DataTable).Rows.Count = 0 Then

            grillas = grPedidos4
            'peso = tbPesTot4
        End If



        If grillas IsNot Nothing Then

            CargarHojaPedidos(grillas, cbCamion.Value)
            Dim dt As DataTable = CType(grillas.DataSource, DataTable)
            Dim total As Double = IIf(IsDBNull(dt.Compute("Sum(kg)", "oaccli>0")), 0, dt.Compute("Sum(kg)", "oaccli>0"))
            If grillas Is grPedidos Then
                tbPesSel.Text = total.ToString
            ElseIf grillas Is grPedidos2 Then
                tbPesSel2.Text = total.ToString
            ElseIf grillas Is grPedidos3 Then
                tbPesSel3.Text = total.ToString
            Else
                ' tbPesSel4.Text = total.ToString
            End If
            'tbPesSel.Text = total.ToString

            ocultarVacios2(grillas)
        End If
    End Sub

    Private Sub grBuscador_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grBuscador.CellEdited

    End Sub

    Private Sub grBuscador_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grBuscador.EditingCell
        e.Cancel = True
    End Sub

    Private Sub grBuscador_KeyDown(sender As Object, e As KeyEventArgs) Handles grBuscador.KeyDown
        If (e.KeyData = Keys.Enter) Then
            MSuperTabControlPrincipal.SelectedTabIndex = 0
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub grBuscador_DoubleClick(sender As Object, e As EventArgs) Handles grBuscador.DoubleClick
        If (grBuscador.Row > -1) Then
            MSuperTabControlPrincipal.SelectedTabIndex = 0
        End If
    End Sub

    Private Sub TableLayoutPanel1_Paint(sender As Object, e As PaintEventArgs) Handles TableLayoutPanel1.Paint

    End Sub

    Private Sub grPendientes_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grPendientes.CellValueChanged
        If ((e.Column.Key.Equals("check1"))) Then
            Dim bit As Boolean = grPendientes.GetValue("check1")

            If bit Then
                Dim act As Double = CDbl(tbSeleccionado.Text)

                act = act + grPendientes.GetValue("kg")
                tbSeleccionado.Text = act.ToString
            Else
                Dim act As Double = CDbl(tbSeleccionado.Text)

                act = act - grPendientes.GetValue("kg")
                tbSeleccionado.Text = act.ToString
            End If


        End If
        Dim saldo As Double = (CDbl(tbDisponible.Text) - CDbl(tbSeleccionado.Text))
        If saldo < 0 Then
            tbSaldoCamion.ForeColor = Color.Red
            ToastNotification.Show(Me, "se ha superado la capacidad del camion".ToUpper,
                                       My.Resources.WARNING,
                                       5 * 1000,
                                       eToastGlowColor.Red,
                                       eToastPosition.TopCenter)
        Else
            tbSaldoCamion.ForeColor = Color.Black
        End If
        tbSaldoCamion.Text = saldo.ToString("0.00")
    End Sub

    Private Sub grCamiones_SelectionChanged(sender As Object, e As GridEventArgs) Handles grCamiones.SelectionChanged
        Dim filaModificada As Integer = e.GridPanel.ActiveRow.Index
        Dim columna As Integer = e.GridPanel.Columns.Item("saldo").ColumnIndex

        Dim peso As Integer = grCamiones.GetCell(filaModificada, columna).Value
        Dim placa As String = grCamiones.GetCell(filaModificada, e.GridPanel.Columns.Item("NOMBRE").ColumnIndex).Value
        tbDescripcionCamion.Text = "CAMION: " + placa + "   CAPACIDAD: " + peso.ToString

        tbDisponible.Text = peso.ToString("0.00")
        Dim saldo As Double = (peso - CDbl(tbSeleccionado.Text))
        If saldo < 0 Then
            tbSaldoCamion.ForeColor = Color.Red
            ToastNotification.Show(Me, "se ha superado la capacidad del camion".ToUpper,
                                       My.Resources.WARNING,
                                       5 * 1000,
                                       eToastGlowColor.Red,
                                       eToastPosition.TopCenter)
        Else
            tbSaldoCamion.ForeColor = Color.Black
        End If
        tbSaldoCamion.Text = saldo.ToString("0.00")
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        Dim resultadoFilas() As DataRow = CType(grPendientes.DataSource, DataTable).Select("check1 = True")
        Dim dt As DataTable = CType(grPendientes.DataSource, DataTable).Clone()
        dt.Clear()
        For Each fila As DataRow In resultadoFilas
            dt.ImportRow(fila)
        Next
        Dim filaModificada As Integer = grCamiones.ActiveRow.Index
        Dim columna As Integer = grCamiones.PrimaryGrid.Columns.Item("CODIGO").ColumnIndex
        Dim columnaPeso As Integer = grCamiones.PrimaryGrid.Columns.Item("capacidad").ColumnIndex

        Dim peso As Integer = grCamiones.GetCell(filaModificada, columna).Value
        Dim pesoTotal As Integer = grCamiones.GetCell(filaModificada, columnaPeso).Value
        Dim salida As Integer = 0
        Dim numi As Integer = 0




        CargarNuevosPendientes(dt)
        tbPesSal.Text = tbSaldoCamion.Text
        tbPesTot.Text = pesoTotal.ToString
        btOrdenar.PerformClick()
    End Sub

    Private Sub CargarNuevosPendientes(dt As DataTable)
        dt.Columns.RemoveAt(0)
        llenarvaciosdatatable(dt)
        Dim filaModificada As Integer = grCamiones.ActiveRow.Index
        Dim columna As Integer = grCamiones.PrimaryGrid.Columns.Item("CODIGO").ColumnIndex
        Dim salida As Integer = 1
        Dim numi As Integer = 0
        Dim peso As Integer = grCamiones.GetCell(filaModificada, columna).Value
        If (TraerUltimaSalida(peso, Now.Date.ToString("dd/MM/yyyy"))).Rows.Count > 0 Then
            salida = (TraerUltimaSalida(peso, Now.Date.ToString("dd/MM/yyyy"))).Rows(0).Item("salida")
            numi = (TraerUltimaSalida(peso, Now.Date.ToString("dd/MM/yyyy"))).Rows(0).Item("numi")
        End If
        'MBtNuevo.PerformClick()
        Dim grilla As GridEX
        SuperTabControl1.SelectedTab = tbSalida1
        tbSalida1.Visible = True
        SuperTabControl1.SelectedTabIndex = 0
        'If salida + 1 = 1 Then
        grilla = grPedidos

        'ElseIf salida + 1 = 2 Then
        '    grilla = grPedidos2
        '    'SuperTabControl1.SelectedTab = tbSalida2
        '    'SuperTabControl1.SelectedTabIndex = 1
        '    'tbSalida2.Visible = True
        'ElseIf salida + 1 = 3 Then
        '    grilla = grPedidos3
        '    'SuperTabControl1.SelectedTab = tbSalida3
        '    'tbSalida3.Visible = True
        '    'SuperTabControl1.SelectedTabIndex = 2
        'Else
        '    grilla = grPedidos4
        '    'SuperTabControl1.SelectedTab = tbSalida4
        '    'tbSalida4.Visible = True
        '    'SuperTabControl1.SelectedTabIndex = 3
        'End If
        CargarHojaPedidosPendientes(grilla, 1, dt)
        ocultarVacios(grilla)
        'grilla.RootTable.Columns("check1").Visible = False


        MSuperTabControlPrincipal.SelectedTabIndex = 0

    End Sub

    Private Sub DoubleInput2_ValueChanged(sender As Object, e As EventArgs) Handles DoubleInput2.ValueChanged

    End Sub

    Private Sub LabelX34_Click(sender As Object, e As EventArgs) Handles tbPesSal.Click

    End Sub

    Private Sub cbCamion_ValueChanged(sender As Object, e As EventArgs) Handles cbCamion.ValueChanged

        Dim dt As DataTable = traerRepartidorZona(cbCamion.Value)
        If dt.Rows.Count > 0 Then
            cbRepartidor.Value = dt.Rows(0).Item("cbnumi")
        End If
        If MBtNuevo.Enabled = False Then


            CargarHojaPedidos(grPedidos, -1)
            CargarHojaPedidos(grPedidos2, -1)
            CargarHojaPedidos(grPedidos3, -1)
            CargarHojaPedidos(grPedidos4, -1)
            TraerPendientes()
            CargarPeso()

        End If
    End Sub

    Private Sub CargarPeso()
        Dim peso As Double
        For Each fila As GridRow In grCamiones.PrimaryGrid.Rows
            If fila.Cells("CODIGO").Value = cbCamion.Value Then
                peso = fila.Cells("Saldo").Value
            End If
        Next

        tbPesTot.Text = peso.ToString("N2")

    End Sub
    Private Sub CargarPeso2()
        Dim peso As Double
        Dim dt As DataTable = CType(grPedidos.DataSource, DataTable)
        peso = IIf(IsDBNull(dt.Compute("Sum(tokg)", "toclie>0")), 0, dt.Compute("Sum(tokg)", "toclie>0"))
        tbPesTot.Text = peso.ToString

    End Sub

    Private Sub tbPesSel_TextChanged(sender As Object, e As EventArgs) Handles tbPesSel.TextChanged
        If IsNumeric(tbPesTot.Text) And IsNumeric(tbPesSel.Text) Then
            tbPesSal.Text = (CDbl(tbPesTot.Text) - CDbl(tbPesSel.Text)).ToString
        End If
    End Sub

    Private Sub btAddTarea_Click(sender As Object, e As EventArgs) Handles btAddTarea.Click
        If MBtNuevo.Enabled = False Then
            Dim ef = New Efecto
            ef.tipo = 6
            ef.ShowDialog()
            Dim bandera As Boolean = False

            bandera = ef.band
            If (bandera = True) Then
                Descripcion = ef.nit
                HoraL = ef.razonsocial
                HoraE = ef.email
                direccion = ef.Header
                observacion = ef.Context
                AgregarTareaAdministrativa(Descripcion, HoraL, HoraE, direccion, observacion) ', direccion, observacion)
                btOrdenar.PerformClick()
            End If
        End If
    End Sub

    Private Sub AgregarTareaAdministrativa(desc As String, horaL As String, HoraE As String, dir As String, obs As String)

        Dim grilla As GridEX
        If SuperTabControl1.SelectedTab Is tbSalida1 Then
            grilla = grPedidos
        ElseIf SuperTabControl1.SelectedTab Is tbSalida2 Then
            grilla = grPedidos2
        ElseIf SuperTabControl1.SelectedTab Is tbSalida3 Then
            grilla = grPedidos3
        ElseIf SuperTabControl1.SelectedTab Is tbSalida4 Then
            grilla = grPedidos4
        End If
        Dim dte As DataTable = CType(grilla.DataSource, DataTable).Clone()

        Dim nuevaFila2 As DataRow = CType(grilla.DataSource, DataTable).NewRow()
        nuevaFila2(0) = True
        nuevaFila2(1) = 0
        nuevaFila2(2) = CType(grilla.DataSource, DataTable).Rows(0).Item("tssalida")
        nuevaFila2(3) = CType(grilla.DataSource, DataTable).Rows(0).Item("tshoras")
        nuevaFila2(4) = CType(grilla.DataSource, DataTable).Rows(0).Item("tshoral")
        nuevaFila2(5) = CType(grilla.DataSource, DataTable).Rows(0).Item("tspeso")
        nuevaFila2(6) = horaL
        nuevaFila2(7) = "00:00:00"
        nuevaFila2(8) = HoraE
        nuevaFila2(9) = "00:00"
        nuevaFila2(10) = dir
        nuevaFila2(11) = 0
        nuevaFila2(12) = desc
        nuevaFila2(13) = ""
        nuevaFila2(14) = "NINGUNO"
        nuevaFila2(15) = 0

        For j = 16 To dte.Columns.Count - 3 Step 1
            nuevaFila2(j) = 0
        Next

        nuevaFila2(dte.Columns.Count - 2) = obs
        nuevaFila2(dte.Columns.Count - 1) = 0




        Dim aux As Integer = 0
        For i = 0 To CType(grilla.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim hora2 As String = CType(grilla.DataSource, DataTable).Rows(i).Item("tohora")

            Dim resultado As Integer = String.Compare(horaL, hora2)

            If resultado < 0 Then

                Dim nuevaFila As DataRow = CType(grilla.DataSource, DataTable).NewRow()
                nuevaFila(0) = True
                nuevaFila(1) = 0
                nuevaFila(2) = CType(grilla.DataSource, DataTable).Rows(0).Item("tssalida")
                nuevaFila(3) = CType(grilla.DataSource, DataTable).Rows(0).Item("tshoras")
                nuevaFila(4) = CType(grilla.DataSource, DataTable).Rows(0).Item("tshoral")
                nuevaFila(5) = CType(grilla.DataSource, DataTable).Rows(0).Item("tspeso")
                nuevaFila(6) = horaL
                nuevaFila(7) = "00:00:00"
                nuevaFila(8) = HoraE
                nuevaFila(9) = "00:00"
                nuevaFila(10) = dir
                nuevaFila(11) = 0
                nuevaFila(12) = desc
                nuevaFila(13) = ""
                nuevaFila(14) = "NINGUNO"
                nuevaFila(15) = 0
                For j = 16 To CType(grilla.DataSource, DataTable).Columns.Count - 3 Step 1
                    nuevaFila(j) = 0
                Next

                nuevaFila(CType(grilla.DataSource, DataTable).Columns.Count - 2) = obs
                nuevaFila(CType(grilla.DataSource, DataTable).Columns.Count - 1) = 0


                aux = 1



                CType(grilla.DataSource, DataTable).Rows.InsertAt(nuevaFila, i)
                Exit For
            End If

        Next
        If aux = 1 Then

        Else

            CType(grilla.DataSource, DataTable).Rows.Add(nuevaFila2)
            ocultarVacios2(grilla)
        End If
    End Sub

    Private Sub grPedidos2_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grPedidos2.EditingCell
        If SuperTabControl1.SelectedTab Is tbSalida2 Then
            If MBtNuevo.Enabled = False Then
                If e.Column.Index = grPedidos2.RootTable.Columns("torecorr").Index Or e.Column.Index = grPedidos2.RootTable.Columns("totrasl").Index Or e.Column.Index = grPedidos2.RootTable.Columns("tohora").Index Or e.Column.Index = grPedidos2.RootTable.Columns("totentre").Index Or e.Column.Index = grPedidos2.RootTable.Columns("check1").Index Then
                    e.Cancel = False
                Else
                    e.Cancel = True
                End If
            Else
                e.Cancel = True
            End If
        End If


    End Sub

    Private Sub LabelX13_Click(sender As Object, e As EventArgs) Handles LabelX13.Click

    End Sub

    Private Sub PanelEx2_Click(sender As Object, e As EventArgs) Handles PanelEx2.Click

    End Sub

    Private Sub tbPesTot2_TextChanged(sender As Object, e As EventArgs) Handles tbPesTot2.TextChanged
        If IsNumeric(tbPesTot2.Text) And IsNumeric(tbPesSel2.Text) Then
            tbPesSal2.Text = (CDbl(tbPesTot2.Text) - CDbl(tbPesSel2.Text)).ToString
        End If
    End Sub

    Private Sub tbPesSel2_TextChanged(sender As Object, e As EventArgs) Handles tbPesSel2.TextChanged
        If IsNumeric(tbPesTot2.Text) And IsNumeric(tbPesSel2.Text) Then
            tbPesSal2.Text = (CDbl(tbPesTot2.Text) - CDbl(tbPesSel2.Text)).ToString
        End If
    End Sub

    Private Sub GroupPanelDatos_Click(sender As Object, e As EventArgs) Handles GroupPanelDatos.Click

    End Sub

    Private Sub btVisualizar_Click(sender As Object, e As EventArgs) Handles btVisualizar.Click
        ImprimirHojaRuta()
    End Sub

    Private Sub btOrdenar_Click(sender As Object, e As EventArgs) Handles btOrdenar.Click
        Dim salida As Integer = 0
        Dim grilla As GridEX
        If SuperTabControl1.SelectedTab Is tbSalida1 Then
            grilla = grPedidos
            salida = 1
        ElseIf SuperTabControl1.SelectedTab Is tbSalida2 Then
            grilla = grPedidos2
            salida = 2
        ElseIf SuperTabControl1.SelectedTab Is tbSalida3 Then
            grilla = grPedidos3
            salida = 3
        ElseIf SuperTabControl1.SelectedTab Is tbSalida4 Then
            grilla = grPedidos4
            salida = 4
        End If


        CType(grilla.DataSource, DataTable).DefaultView.Sort = "totrasl ASC"

        Dim query = From row In CType(grilla.DataSource, DataTable).AsEnumerable()
                    Order By row.Field(Of String)("totrasl") Ascending
                    Select row

        ' Crear un nuevo DataTable ordenado
        Dim dtOrdenado As DataTable = CType(grilla.DataSource, DataTable).Clone() ' Clona la estructura del DataTable
        For Each row As DataRow In query
            dtOrdenado.ImportRow(row) ' Importa las filas ordenadas
        Next

        ' Mostrar el DataTable ordenado
        For i = 0 To dtOrdenado.Rows.Count - 1 Step 1
            dtOrdenado.Rows(i).Item("tonro") = i + 1
        Next


        grilla.DataSource = dtOrdenado
    End Sub

    Private Sub AnularPedidosNoEntregados(cod As Integer)
        Dim dt As DataTable = TraerSalidas(cod)
        For i = 0 To dt.Rows.Count - 1 Step 1
            L_PedidoCabacera_ModificarActivoPasivo(dt.Rows(i).Item("tooanumi"), "2")
        Next
    End Sub
    Private Sub btCerrarHoja_Click(sender As Object, e As EventArgs) Handles btCerrarHoja.Click
        CerrarHojaRuta(tbCodigo.Text)
        AnularPedidosNoEntregados(tbCodigo.Text)
        Limpiar()
        CargarHojaRuta()
        InHabilitar()
        CargarCamiones()
    End Sub

    Private Sub MSuperTabControlPrincipal_SelectedTabChanged(sender As Object, e As SuperTabStripSelectedTabChangedEventArgs) Handles MSuperTabControlPrincipal.SelectedTabChanged
        If MSuperTabControlPrincipal.SelectedTab.Text = "PENDIENTES" Then
            CargarCamiones()
            ' CargarHojaPedidosPendientes(grPendientes, 0, CType(grPedidos4.DataSource, DataTable))
        End If
    End Sub

    Private Sub tbPesTot_TextChanged(sender As Object, e As EventArgs) Handles tbPesTot.TextChanged
        If IsNumeric(tbPesTot.Text) And IsNumeric(tbPesSel.Text) Then
            tbPesSal.Text = (CDbl(tbPesTot.Text) - CDbl(tbPesSel.Text)).ToString
        End If
    End Sub

    Private Sub grPedidos_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grPedidos.EditingCell

        If SuperTabControl1.SelectedTab Is tbSalida1 Then
            If MBtNuevo.Enabled = False Then
                If e.Column.Index = grPedidos.RootTable.Columns("tohora").Index Or e.Column.Index = grPedidos.RootTable.Columns("totentre").Index Or e.Column.Index = grPedidos.RootTable.Columns("check1").Index Or e.Column.Index = grPedidos.RootTable.Columns("totrasl").Index Then
                    e.Cancel = False
                Else
                    e.Cancel = True
                End If
            Else
                e.Cancel = True
            End If
        End If


    End Sub
End Class