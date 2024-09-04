Public Class EmisorResp
    Public Class micrositios
        Public Property descarga As String
        Public Property cliente As String
    End Class

    Public Class Data
        Public Property details As String
        Public Property codigoEstado As Integer
        Public Property codigoRecepcion As String
        Public Property cuf As String
        Public Property codigoSector As String
        Public Property sucursal As Integer
        Public Property puntoVenta As Integer
        Public Property dataXml As String
        Public Property tipoEmision As Integer
    End Class

    Public Class RespEmisor
        ' Public Property response As String
        Public Property [error] As String
        Public Property errores As String()
        Public Property error_cod As String()

        Public Property error_details As String()
        Public Property external_reference As String
        Public Property requiere_fec As String
        Public Property observaciones As String
        Public Property tfc_generacion_tipo As Integer
        Public Property rta As String
        Public Property cae As String
        Public Property vencimiento_cae As String
        Public Property vencimiento_pago As String
        Public Property comprobante_nro As String
        Public Property comprobante_tipo As String
        Public Property afip_codigo_barras As String
        Public Property afip_qr As String
        Public Property micrositios As micrositios
        Public Property envio_x_mail As String
        Public Property envio_x_mail_direcciones As String
        Public Property comprobante_pdf_url As String
    End Class
End Class

