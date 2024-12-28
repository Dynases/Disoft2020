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
Public Class F0_Despacho
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


#Region "Eventos"
    Private Sub frmBillingDispatch_Load(sender As Object, e As EventArgs) Handles MyBase.Load

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



            CargarcComboCamion()
            ConfigForm()
            CargarZona()
            CargarChoferes()
            tbFecha.Value = DateTime.Today
            tbHoraS.Value = DateAndTime.TimeOfDay
            tbHoraL.Value = DateAndTime.TimeOfDay

            Tb_FechaHasta.Value = DateTime.Today
            _cargaCompleta = True
            cbEstado.SelectedIndex = 0
            CargarHojaRuta()
            InHabilitar()
            CargarVistasSalidas()

            MSuperTabControlPrincipal.SelectedTabIndex = 0
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
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

        dt = L_prGeneralCamiones()

        panel.DataSource = dt

        'Dim columna As PanelGridColumn = panelGrid1.Columns(0)

        '' Cambiar el nombre de la columna
        'columna.HeaderText = "Nuevo Nombre"

        '' Refrescar el PanelGrid si es necesario
        'panelGrid1.Refresh()
    End Sub

    Private Sub CargarHojaRuta()
        Dim dt As DataTable = CargarHojaRutaPendientesxDespacho()
        grBuscador.BoundMode = Janus.Data.BoundMode.Bound
        grBuscador.DataSource = dt
        grBuscador.RetrieveStructure()
        With grBuscador.RootTable.Columns("trnumi")
            .Width = 80
            .Caption = "CODIGO"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = True
        End With
        With grBuscador.RootTable.Columns("tssalida")
            .Width = 150
            .Caption = "SALIDA"
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
            .Caption = "CAMION"
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
        With grBuscador.RootTable.Columns("tshdespacho")
            .Width = 200
            .Caption = "FECHA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .Visible = False
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
        'cbZona.ReadOnly = False
        'cbRepartidor.ReadOnly = False
        'tbFecha.Enabled = True
        'cbCamion.ReadOnly = False

        MBtNuevo.Enabled = False
        MBtModificar.Enabled = False
        MBtGrabar.Enabled = True

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
    End Sub
    Private Sub InHabilitar()
        cbZona.ReadOnly = True
        cbRepartidor.ReadOnly = True
        tbFecha.Enabled = False
        cbCamion.ReadOnly = True

        MBtNuevo.Enabled = True
        MBtModificar.Enabled = True
        MBtGrabar.Enabled = False

        tbHoraS.Enabled = False
        tbHoraL.Enabled = False
        tbHoraS2.Enabled = False
        tbHoraL2.Enabled = False
        tbHoraS3.Enabled = False
        tbHoraL3.Enabled = False
        tbHoraS4.Enabled = False
        tbHoraL4.Enabled = False

        'tbPesTot.Visible = False
        tbPesTot2.Visible = False
        'LabelX12.Visible = False
        LabelX14.Visible = False
        LabelX25.Visible = False
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
            Me.Text = "DESPACHO"
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

            cbRepartidor.SelectedIndex = 0
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
        Dim dt As DataTable
        If cod = 1 Then
            dt = TraerPedidosPendientes2(dt1, cbCamion.Value, 1)
        Else
            dt = TraerPedidosPendientes()
        End If


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
        With grilla.RootTable.Columns("hora")
            .Caption = "HORA"
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
        With grilla.RootTable.Columns("oanumi")
            .Caption = "HORA"
            .Width = 50
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        If cod = 0 Then
            With grilla.RootTable.Columns("blanca")
                .Caption = "BLAN"
                .CellStyle.BackColor = Color.LightCoral
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("negra")
                .Caption = "NEGR"
                .Width = 65
                .CellStyle.BackColor = Color.LightCoral
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("azul")
                .Caption = "AZUL"
                .Width = 65
                .CellStyle.BackColor = Color.LightCoral
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("segunda")
                .Caption = "SEGUNDA"
                .Width = 65
                .CellStyle.BackColor = Color.LightCoral
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("bb")
                .Caption = "BB"
                .CellStyle.BackColor = Color.LightCoral
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("g1")
                .Caption = "G1"
                .CellStyle.BackColor = Color.LightCoral
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("g2")
                .Caption = "G2"
                .CellStyle.BackColor = Color.LightCoral
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("g3")
                .Caption = "G3"
                .CellStyle.BackColor = Color.LightCoral
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("g4")
                .Caption = "G4"
                .CellStyle.BackColor = Color.LightCoral
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("g5")
                .Caption = "G5"
                .CellStyle.BackColor = Color.LightCoral
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("filete")
                .Caption = "FIL"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("filetillo")
                .Caption = "FILT"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("molida")
                .Caption = "MOL"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("maltrato")
                .Caption = "MALT"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("molidacor")
                .Caption = "MOLC"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("fpierna")
                .Caption = "FPIER"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("piernasola")
                .Caption = "PIERS"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("muslo")
                .Caption = "MUSL"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("alasegunda")
                .Caption = "ALASG"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("alapicada")
                .Caption = "ALAP"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("huesocostilla")
                .Caption = "COST"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("pulmon")
                .Caption = "PULM"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("puntala")
                .Caption = "P.ALA"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("cabeza")
                .Caption = "CAB"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("pechuga")
                .Caption = "PECH"
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals("0.00", "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum

            End With
            With grilla.RootTable.Columns("pierna")
                .Caption = "PIER"
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals("0.00", "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum

            End With
            With grilla.RootTable.Columns("ala")
                .Caption = "ALA"
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("cazuela")
                .Caption = "CAZ."
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("cuello")
                .Caption = "CUE:"
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("menudo")
                .Caption = "MEN."
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("higado")
                .Caption = "HIG."
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("alalast")
                .Caption = "ALAX"
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("filetepiel")
                .Caption = "P/FIL."
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = False
                .FormatMode = GridEXFormatCondition.Equals(0.00, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With

        Else
            With grilla.RootTable.Columns("blanca")
                .Caption = "BLAN"
                .CellStyle.BackColor = Color.LightCoral
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("negra")
                .Caption = "NEGR"
                .Width = 65
                .CellStyle.BackColor = Color.LightCoral
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("azul")
                .Caption = "AZUL"
                .Width = 65
                .CellStyle.BackColor = Color.LightCoral
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("segunda")
                .Caption = "SEG."
                .Width = 65
                .CellStyle.BackColor = Color.LightCoral
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("filete")
                .Caption = "FIL"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("filetillo")
                .Caption = "FILT"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("molida")
                .Caption = "MOL"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("maltrato")
                .Caption = "MALT"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("molidacor")
                .Caption = "MOLC"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("fpierna")
                .Caption = "FPIER"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("piernasola")
                .Caption = "PIERS"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("muslo")
                .Caption = "MUSL"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("alasegunda")
                .Caption = "ALASG"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("alapicada")
                .Caption = "ALAP"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("huesocostilla")
                .Caption = "COST"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("pulmon")
                .Caption = "PULM"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("puntala")
                .Caption = "P.ALA"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("cabeza")
                .Caption = "CAB"
                .CellStyle.BackColor = Color.LightBlue
                .Width = 65
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("pechuga")
                .Caption = "PECH"
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals("0.00", "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum

            End With
            With grilla.RootTable.Columns("pierna")
                .Caption = "PIER"
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals("0.00", "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum

            End With
            With grilla.RootTable.Columns("ala")
                .Caption = "ALA"
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("cazuela")
                .Caption = "CAZ."
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("cuello")
                .Caption = "CUE:"
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("menudo")
                .Caption = "MEN."
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("higado")
                .Caption = "HIG."
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("alalast")
                .Caption = "ALAX"
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With
            With grilla.RootTable.Columns("filetepiel")
                .Caption = "P/FIL."
                .Width = 65
                .CellStyle.BackColor = Color.LightBlue
                .HeaderStyle.BackColor = Color.Green
                .FormatString = "0.00"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatMode = GridEXFormatCondition.Equals(0.00, "")
                .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
            End With

        End If

        With grilla.RootTable.Columns("obs")
            .Caption = "OBSERVACION"
            .Width = 200
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("KG")
            .Caption = "KG"
            .Width = 200
            .FormatString = "0.00"
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
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

    End Sub

    Private Sub CargarHojaPedidos(grilla As GridEX, zona As Integer)
        Dim dt As DataTable = TraerHojaRuta(zona)

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
            .Caption = "NRO"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("tssalida")
            .Caption = "HORA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grilla.RootTable.Columns("tshoras")
            .Caption = "HORA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grilla.RootTable.Columns("tshoral")
            .Caption = "HORA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grilla.RootTable.Columns("tspeso")
            .Caption = "HORA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grilla.RootTable.Columns("totrasl")
            .Caption = "HORA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grilla.RootTable.Columns("torecorr")
            .Caption = "HORA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grilla.RootTable.Columns("tohora")
            .Caption = "HORA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
        End With
        With grilla.RootTable.Columns("totentre")
            .Caption = "HORA"
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
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
        With grilla.RootTable.Columns("oanumi")
            .Caption = "HORA"
            .Width = 50
            .HeaderStyle.BackColor = Color.Green
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        Dim dtCab1 As DataTable = TraerCabecera()
        For i = 0 To dtCab1.Rows.Count - 1 Step 1
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
        '    .CellStyle.BackColor = Color.LightCoral
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
        '    .CellStyle.BackColor = Color.LightCoral
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
        '    .CellStyle.BackColor = Color.LightCoral
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
        '    .CellStyle.BackColor = Color.LightCoral
        '    .HeaderStyle.BackColor = Color.Green
        '    .FormatString = "0"
        '    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        '    .Visible = True
        '    .FormatMode = GridEXFormatCondition.Equals(0, "")
        '    .AggregateFunction = Janus.Windows.GridEX.AggregateFunction.Sum
        'End With
        'With grilla.RootTable.Columns("g5")
        '    .Caption = "G%"
        '    .Width = 65
        '    .CellStyle.BackColor = Color.LightCoral
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
        ocultarVacios(grilla)
    End Sub




    ' Manejador de eventos para el clic en las pestañas

    Private Sub ocultarVacios(grilla As GridEX)
        If CType(grilla.DataSource, DataTable).Rows.Count > 0 Then
            For i = 9 To CType(grilla.DataSource, DataTable).Columns.Count - 3 Step 1
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

    Private Sub grPedidos_FormattingRow(sender As Object, e As RowLoadEventArgs)


    End Sub

    Private Sub grPedidos_RowCheckStateChanging(sender As Object, e As RowCheckStateChangingEventArgs)

    End Sub

    Private Sub grPedidos_RowCheckStateChanged(sender As Object, e As RowCheckStateChangeEventArgs)
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

        CargarEstilosTab()


        'MSuperTabControlPrincipal.Enabled = False
    End Sub

    Private Sub Limpiar()
        tbCodigo.Clear()
        cbZona.SelectedIndex = 0
        cbRepartidor.SelectedIndex = 0
        tbFecha.Value = Date.Now.ToString("dd/MM/yyyy")
        tbPesSel.Text = "0.00"
        tbPesTot.Text = "0.00"
        tbHoraS.Value = DateAndTime.TimeOfDay
        tbHoraL.Value = DateAndTime.TimeOfDay

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
    End Sub




    Private Sub grPrecioPolloEntero_ColumnHeaderClick(sender As Object, e As ColumnActionEventArgs)

    End Sub



    Private Sub LabelX27_Click(sender As Object, e As EventArgs) Handles LabelX27.Click

    End Sub

    Private Sub MBtGrabar_Click(sender As Object, e As EventArgs) Handles MBtGrabar.Click
        Dim numi As String = ""
        Dim numiConciliacion As String = ""
        Dim resconciliacion As Boolean = L_prMovimientoChoferGrabar(numiConciliacion, tbFecha.Value.ToString("yyyy/MM/dd"), 10, "", cbRepartidor.Value, 0)
        If (resconciliacion = False) Then
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "El Movimiento no pudo ser insertado".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            Return
        Else
            Dim tabla As DataTable = L_prMovimientoChoferNoExisteConciliacion(cbRepartidor.Value) ''Aqui obtengo el numi de la TI0022 
            Dim productos As DataTable = TraerProductosSalida(CType(grPedidos.PrimaryGrid.DataSource, DataTable), cbRepartidor.Value)
            Dim dtaux As DataTable = productos.Clone
            dtaux.Clear()
            For Each fila As DataRow In productos.Rows
                Dim newFila As DataRow = fila
                dtaux.ImportRow(newFila)
            Next
            dtaux.Columns.RemoveAt(0)
            Dim res As Boolean = L_prMovimientoChoferGrabarSalida(numi, tbFecha.Value.ToString("yyyy/MM/dd"), 9, "", cbRepartidor.Value, tabla.Rows(0).Item("ieid"), dtaux, tbFecha.Value.ToString("dd/MM/yyyy"), 1, productos.Rows(0).Item("cbAlmacen"))
            If res Then
                Dim dt As DataTable = L_BuscarIdPedido(cbRepartidor.Value, tbFecha.Value.ToString("dd/MM/yyyy"), tabla.Rows(0).Item("ibid"))
                If dt.Rows.Count > 0 Then
                    For i = 0 To dt.Rows.Count - 1
                        'Grabar Estado 6 de Movimiento Salida en la TO001D
                        L_GrabarTO001D(dt.Rows(i).Item("idpedido"), "6", "Movimiento Salida")
                    Next
                End If

                cambiarestadoSalida(CInt(tbCodigo.Text), CType(grPedidos.PrimaryGrid.DataSource, DataTable).Rows(0).Item("tssalida"), numiConciliacion, tbHoraS.Value.ToString("HH:mm"))
                Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
                ToastNotification.Show(Me, "Código de Movimiento ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper,
                                              img, 2000,
                                              eToastGlowColor.Green,
                                              eToastPosition.TopCenter
                                              )
                MBtSalir.PerformClick()
            Else
                Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
                ToastNotification.Show(Me, "El Movimiento no pudo ser insertado".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            End If
        End If


    End Sub

    Private Sub GrabarNuevoRegistro()
        'llenarvacios(grPedidos)
        'Dim resultadoFilas() As DataRow = CType(grPedidos.DataSource, DataTable).Select("check1 = True")
        'Dim dt As DataTable = CType(grPedidos.DataSource, DataTable).Clone()
        'dt.Clear()
        'For Each fila As DataRow In resultadoFilas
        '    dt.ImportRow(fila)
        'Next
        ''Dim dt As DataTable = CType(grPedidos.DataSource, DataTable)
        'dt.Columns.RemoveAt(0)
        'Dim cod As Integer
        'If Not IsNumeric(tbCodigo.Text) Then
        '    cod = 0
        'Else
        '    cod = CInt(tbCodigo.Text)
        'End If

        'Dim res As Boolean = GrabarHojaRuta(cod, cbCamion.Value, cbRepartidor.Value, tbFecha.Value.ToString("dd/MM/yyyy"), tbHoraS.Value.ToString("HH:mm"), tbHoraL.Value.ToString("HH:mm"),
        '                                    CDbl(tbPesSel.Text), dt)

        'If res Then
        '    Limpiar()
        '    CargarHojaRuta()
        '    InHabilitar()

        'End If
    End Sub

    Private Sub ModificarRegistro()

    End Sub

    Private Sub llenarvacios(grilla As GridEX)

        For i = 0 To CType(grilla.DataSource, DataTable).Rows.Count - 1 Step 1
            For j = 7 To CType(grilla.DataSource, DataTable).Columns.Count - 1 Step 1
                If Not IsNumeric(CType(grilla.DataSource, DataTable).Rows(i).Item(j)) Then
                    CType(grilla.DataSource, DataTable).Rows(i).Item(j) = 0.00
                End If
            Next
        Next
    End Sub

    Private Sub llenarvaciosdatatable(ByRef dt As DataTable)

        For i = 0 To dt.Rows.Count - 1 Step 1
            For j = 7 To dt.Columns.Count - 1 Step 1
                If Not IsNumeric(dt.Rows(i).Item(j)) Then
                    dt.Rows(i).Item(j) = 0.00
                End If
            Next
        Next
    End Sub

    Private Sub MBtModificar_Click(sender As Object, e As EventArgs) Handles MBtModificar.Click
        Dim estado As Boolean = True
        grPedidos.PrimaryGrid.EnsureVisible(True)
        grPedidos.Refresh()

        For Each filaHija As GridRow In grPedidos.PrimaryGrid.Rows
            If filaHija.CellStyles.Default.Background.Color1 <> Color.LightGreen Then
                estado = False
                Exit For
            End If
        Next

        If estado = False Then
            ToastNotification.Show(Me, "No se han cargado todos los productos".ToUpper,
                                       My.Resources.WARNING,
                                       5 * 1000,
                                       eToastGlowColor.Red,
                                       eToastPosition.MiddleCenter)
        Else
            Habilitar()
            MBtGrabar.PerformClick()
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

            Dim dt As DataTable = TraerSalidasPendientes(CInt(.GetValue("trnumi")), CInt(.GetValue("tssalida")))
            'Dim resultadoFilas() As DataRow = dt.Select("tssalida = 1")
            'Dim dt1 As DataTable = dt.Clone()
            'dt1.Clear()
            'For Each fila As DataRow In resultadoFilas
            '    dt1.ImportRow(fila)
            'Next
            If dt.Rows.Count > 0 Then
                CargarGrillas(grPedidos, dt)
                CargarSubFilas()
                If .GetValue("tshdespacho") = "" Then
                    tbHoraS.Value = dt.Rows(0).Item("tshoras")
                Else
                    tbHoraS.Value = .GetValue("tshdespacho")
                End If

                'tbHoraL.Value = dt.Rows(0).Item("tshoral")
                total = IIf(IsDBNull(dt.Compute("Sum(tokg)", "toclie>0")), 0, dt.Compute("Sum(tokg)", "toclie>0"))
                tbPesSel.Text = total.ToString
                Grilla1 = 2
                Dim salida As Integer = dt.Rows(0).Item("tssalida")
                Select Case salida
                    Case 1
                        tbSalida1.Text = "PRIMERA SALIDA"
                    Case 2
                        tbSalida1.Text = "SEGUNDA SALIDA"
                    Case 3
                        tbSalida1.Text = "TERCER SALIDA"
                    Case 4
                        tbSalida1.Text = "CUARTA SALIDA"

                End Select
            Else
                CargarGrillas(grPedidos, dt)
                Grilla1 = 1
            End If







        End With

        CargarVistasSalidas()
        CargarEstilosTab()

        Dim pagina As String = (grBuscador.Row + 1).ToString + "/" + (CType(grBuscador.DataSource, DataTable).Rows.Count).ToString
        MLbPaginacion.Text = pagina
    End Sub

    Private Sub CargarSubFilas()
        For Each filas As GridRow In grPedidos.PrimaryGrid.Rows
            AddSubRow(filas)
        Next
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
    Private Sub cambiarNombres()


        Dim panel As GridPanel = grPedidos.PrimaryGrid
        'grilla.PrimaryGrid.BoundMode = Janus.Data.BoundMode.Bound

        Dim columns As GridColumnCollection = panel.Columns

        columns("tssalida").Visible = False
        columns("tssalida").HeaderText = "pRUEBA"

        columns("tshoras").Visible = False
        columns("tshoras").HeaderText = "pRUEBA"

        columns("tshoral").Visible = False
        columns("tshoral").HeaderText = "pRUEBA"

        columns("tohora").Visible = True
        columns("tohora").HeaderText = "LLEGADA"

        columns("totentre").Visible = True
        columns("totentre").HeaderText = "ENTREGA"

        columns("tooanumi").Visible = False
        columns("tooanumi").HeaderText = "pRUEBA"

        columns("ccdirec").Visible = True
        columns("ccdirec").HeaderText = "ZONA"

        columns("toclie").Visible = True
        columns("toclie").HeaderText = "COD. CLIENTE"
        columns("toclie").CellStyles.Default.Alignment = Style.Alignment.MiddleCenter


        columns("ccdesc").Visible = True
        columns("ccdesc").HeaderText = "CLIENTE"

        columns("cctelf1").Visible = True
        columns("cctelf1").HeaderText = "TELEFONO"

        columns("cbdesc").Visible = True
        columns("cbdesc").HeaderText = "PROMOTOR"

        columns("oaobs").Visible = True
        columns("oaobs").HeaderText = "OBSERVACION"

        columns("tokg").Visible = True
        columns("tokg").HeaderText = "PESO"
        columns("tokg").CellStyles.Default.Alignment = Style.Alignment.BottomRight


        panel.ShowCheckBox = True
    End Sub

    Private Sub AddSubRow(fila As GridRow)
        Dim dt1 As DataTable
        'dt1 = TraerDetalleSalida(fila.Cells(7).Value)
        If fila.Rows.Count = 0 Then
            Dim panel As New GridPanel()
            fila.ShowCheckBox = False
            panel.CheckBoxes = True
            panel.ShowCheckBox = True
            panel.ShowTreeButtons = True
            panel.ShowTreeLines = False
            panel.ShowRowGridIndex = True

            panel.RowHeaderWidth = 40
            panel.DefaultRowHeight = 0
            panel.ColumnHeader.RowHeight = 30

            'panel.DefaultVisualStyles.CellStyles.Default.Alignment = Alignment.MiddleCenter
            'panel.DefaultVisualStyles.ColumnHeaderStyles.Default.Alignment = Alignment.MiddleCenter

            Dim dt As DataTable
            'If cbConci.Checked = True Then
            dt = TraerDetalleSalida(fila.Cells("tooanumi").Value)
            'Else
            'dt = TraerKPI21detalle(crow.Cells("CODIGO").Value, tbFechaI.Value.ToString("dd/MM/yyyy"), tbFechaF.Value.ToString("dd/MM/yyyy"))
            'End If

            panel.DataSource = dt

            fila.Rows.Add(panel)

            panel.EnsureVisible(True)

            Dim columns As GridColumnCollection = panel.Columns

            columns("oanumi").Visible = False
            columns("oanumi").HeaderText = "PEDIDO"



            columns("obcprod").Visible = True
            columns("obcprod").HeaderText = "COD. PRODUCTO"

            columns("cadesc").Visible = True
            columns("cadesc").HeaderText = "PRODUCTO"

            columns("obpcant").Visible = True
            columns("obpcant").HeaderText = "CANTIDAD"
            columns("obpcant").CellStyles.Default.Alignment = Style.Alignment.MiddleRight

            columns("cedesc").Visible = True
            columns("cedesc").HeaderText = "UNIDAD"
            'columns("cedesc").CellStyles.Default.Alignment = Style.Alignment.MiddleRight

            columns("obpbase").Visible = True
            columns("obpbase").HeaderText = "PRECIO"
            columns("obpbase").CellStyles.Default.Alignment = Style.Alignment.MiddleRight


            grPedidos.Refresh()

            Dim rowToSelect As GridRow = grPedidos.PrimaryGrid.Rows(fila.Index)

            ' Seleccionar la fila utilizando el método SetSelected
            grPedidos.PrimaryGrid.SetActiveRow(rowToSelect)
        End If
    End Sub
    Private Sub CargarGrillas(grilla As SuperGridControl, dt As DataTable)


        ' Definir las columnas

        'Dim dtaux As DataTable = dt.Clone
        'dtaux.Clear()

        'For i = 0 To dt.Rows.Count - 1 Step 1

        '    Dim dataRow As DataRow = dt.Rows(i)

        '    ' Crear un nuevo GridRow
        '    Dim gridRow As New GridRow()

        '    ' Asignar los valores de las celdas del DataTable al GridRow
        '    For j As Integer = 0 To dataRow.ItemArray.Length - 1
        '        ' Crear una nueva celda y establecer su valor
        '        Dim cell As New GridCell(dataRow(j).ToString())

        '        ' Agregar la celda al GridRow
        '        gridRow.Cells.Add(cell)
        '    Next
        '    AddSubRow(gridRow)
        '    dtaux.Rows.Add(gridRow)
        '    'grilla.PrimaryGrid.Rows.Add(gridRow)
        '    'grilla.Refresh()

        'Next
        grilla.PrimaryGrid.DataSource = dt

        grilla.PrimaryGrid.CheckBoxes = False
        grilla.PrimaryGrid.ShowCheckBox = True
        grilla.PrimaryGrid.ShowTreeButtons = False
        grilla.PrimaryGrid.ShowTreeLines = False
        grilla.PrimaryGrid.ShowRowGridIndex = True

        grilla.PrimaryGrid.EnsureVisible(True)
        cambiarNombres()



    End Sub

    Private Sub CREARNUEVASALIDAToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CREARNUEVASALIDAToolStripMenuItem.Click
        tbSalida2.Visible = True
        SuperTabControl1.SelectedTab = tbSalida2

    End Sub

    Private Sub MBtSalir_Click(sender As Object, e As EventArgs) Handles MBtSalir.Click
        If MBtGrabar.Enabled = True Then
            Limpiar()
            InHabilitar()
            If CType(grBuscador.DataSource, DataTable).Rows.Count > 0 Then
                LLenarDatos(grBuscador.Row)
                'TraerPendientes()
                CargarVistasSalidas()
            End If
            MSuperTabControlPrincipal.Enabled = True
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



    Private Sub MBtImprimir_Click(sender As Object, e As EventArgs) Handles MBtImprimir.Click

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

    Private Sub btCargarHoja_Click(sender As Object, e As EventArgs)
        ImprimirHojaRutaxChofer()
    End Sub

    Private Sub ImprimirHojaRutaxChofer()
        Dim detalle As String
        Dim salida As Integer
        If SuperTabControl1.SelectedTab Is tbSalida1 Then
            salida = 1
            detalle = "PRIMERA SALIDA"
        ElseIf SuperTabControl1.SelectedTab Is tbSalida2 Then
            salida = 2
            detalle = "SEGUNDA SALIDA"
        ElseIf SuperTabControl1.SelectedTab Is tbSalida3 Then
            salida = 3
            detalle = "TERCER SALIDA"
        ElseIf SuperTabControl1.SelectedTab Is tbSalida4 Then
            salida = 4
            detalle = "CUARTA SALIDA"
        End If
        Dim dt As DataTable = ImprimirSalida(CInt(tbCodigo.Text), salida)
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If
        detalle = detalle + "    ZONA: " + cbZona.Text + "    CHOFER: " + cbRepartidor.Text
        P_Global.Visualizador = New Visualizador

        Dim objrep As New R_HojaRutaxChofer
        objrep.SetDataSource(dt)
        objrep.SetParameterValue("detalle", detalle)
        objrep.SetParameterValue("fecha", tbFecha.Value.ToString("dd/MM/yyyy"))

        P_Global.Visualizador.CRV1.ReportSource = objrep
        P_Global.Visualizador.Show()
        P_Global.Visualizador.BringToFront()

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

    Private Sub TableLayoutPanel1_Paint(sender As Object, e As PaintEventArgs)

    End Sub








    Private Sub DoubleInput2_ValueChanged(sender As Object, e As EventArgs) Handles DoubleInput2.ValueChanged

    End Sub

    Private Sub LabelX34_Click(sender As Object, e As EventArgs) Handles tbPesSal.Click

    End Sub






    Private Sub tbPesSel_TextChanged(sender As Object, e As EventArgs) Handles tbPesSel.TextChanged
        If IsNumeric(tbPesTot.Text) And IsNumeric(tbPesSel.Text) Then
            tbPesSal.Text = (CDbl(tbPesTot.Text) - CDbl(tbPesSel.Text)).ToString
        End If
    End Sub

    Private Sub btRenombrar_Click(sender As Object, e As EventArgs) Handles btRenombrar.Click
        cambiarNombres()
    End Sub

    Private Sub grPedidos_AfterCheck(sender As Object, e As GridAfterCheckEventArgs) Handles grPedidos.AfterCheck
        Dim crow As GridRow = TryCast(e.Item, GridRow)

        ' If the check state is going from unchecked to
        ' checked, then add anew sub panel under the the
        ' row that was checked.
        If crow.Cells.Count = 13 Then
            Dim rowToActivate As GridRow = grPedidos.PrimaryGrid.Rows(crow.Index)
            If rowToActivate IsNot Nothing AndAlso rowToActivate.IsSelectable Then
                ' Selecciona la fila deseada y establece su estado activo
                rowToActivate.IsSelected = True
            End If
            If crow IsNot Nothing AndAlso crow.Checked = True Then
                Dim panel As New GridPanel()
                crow.ShowCheckBox = False
                panel.CheckBoxes = True
                panel.ShowCheckBox = False
                panel.ShowTreeButtons = True
                panel.ShowTreeLines = False
                panel.ShowRowGridIndex = True

                panel.RowHeaderWidth = 40
                panel.DefaultRowHeight = 0
                panel.ColumnHeader.RowHeight = 30

                'panel.DefaultVisualStyles.CellStyles.Default.Alignment = Alignment.MiddleCenter
                'panel.DefaultVisualStyles.ColumnHeaderStyles.Default.Alignment = Alignment.MiddleCenter

                Dim dt As DataTable
                'If cbConci.Checked = True Then
                dt = TraerDetalleSalida(crow.Cells("tooanumi").Value)
                'Else
                'dt = TraerKPI21detalle(crow.Cells("CODIGO").Value, tbFechaI.Value.ToString("dd/MM/yyyy"), tbFechaF.Value.ToString("dd/MM/yyyy"))
                'End If

                panel.DataSource = dt

                crow.Rows.Add(panel)

                ' Since we are always adding the row to the end
                ' of the list, lets make sure it is visible on the screen

                panel.EnsureVisible(True)

                Dim columns As GridColumnCollection = panel.Columns

                columns("obcprod").Visible = True
                columns("obcprod").HeaderText = "COD. PRODUCTO"

                columns("cadesc").Visible = True
                columns("cadesc").HeaderText = "PRODUCTO"

                columns("obpcant").Visible = True
                columns("obpcant").HeaderText = "CANTIDAD"
                columns("obpcant").CellStyles.Default.Alignment = Style.Alignment.BottomRight

                columns("obpbase").Visible = True
                columns("obpbase").HeaderText = "PRECIO"
                columns("obpbase").CellStyles.Default.Alignment = Style.Alignment.BottomRight


                grPedidos.Refresh()

                'For Each r As GridRow In panel.Rows
                '    If r.Cells("ESTADO").Value = "ENTREGADO" Then
                '        r.Cells("ESTADO").CellStyles.Default.Background.Color2 = Color.Green

                '    ElseIf r.Cells("ESTADO").Value <> "ENTREGADO" Then
                '        r.Cells("ESTADO").CellStyles.Default.Background.Color2 = Color.Red
                '    End If

                'Next


            End If
        ElseIf crow.Cells.Count = 6 Then
            crow.ShowCheckBox = True
            If crow IsNot Nothing AndAlso crow.Checked = True Then
                crow.CellStyles.Default.Background.Color2 = Color.LightGreen
                crow.CellStyles.Default.Background.Color1 = Color.LightGreen
            Else
                crow.CellStyles.Default.Background.Color2 = Color.White
                crow.CellStyles.Default.Background.Color1 = Color.White
            End If
            Dim est As Boolean = True
            For Each column As GridRow In crow.GridPanel.Rows
                If column.Checked = False Then
                    est = False
                    Exit For
                End If
            Next column


            'grPedidos.PrimaryGrid.SetActiveRow(crow.Parent)
            Dim selectedRow As GridRow
            For Each fila As GridRow In grPedidos.PrimaryGrid.Rows
                If fila.Cells("tooanumi").Value = crow.Cells("oanumi").Value Then
                    selectedRow = fila
                    Exit For
                End If
            Next
            If est = True Then
                'grPedidos.PrimaryGrid.
                selectedRow.CellStyles.Default.Background.Color1 = Color.LightGreen
                selectedRow.CellStyles.Default.Background.Color2 = Color.LightGreen

                grPedidos.Refresh()
            Else
                selectedRow.CellStyles.Default.Background.Color1 = Color.White
                selectedRow.CellStyles.Default.Background.Color2 = Color.White

                grPedidos.Refresh()
            End If
        End If
    End Sub

    Private Sub grPedidos_DoubleClick(sender As Object, e As EventArgs) Handles grPedidos.DoubleClick
        Dim fila As GridRow = grPedidos.PrimaryGrid.ActiveRow
        AddSubRow(fila)
    End Sub

    Private Sub cbCamion_ValueChanged(sender As Object, e As EventArgs) Handles cbCamion.ValueChanged

    End Sub

    Private Sub tbPesTot_TextChanged(sender As Object, e As EventArgs) Handles tbPesTot.TextChanged
        If IsNumeric(tbPesTot.Text) And IsNumeric(tbPesSel.Text) Then
            tbPesSal.Text = (CDbl(tbPesTot.Text) - CDbl(tbPesSel.Text)).ToString
        End If
    End Sub

    Private Sub grPedidos_EditingCell(sender As Object, e As EditingCellEventArgs)


    End Sub
End Class
