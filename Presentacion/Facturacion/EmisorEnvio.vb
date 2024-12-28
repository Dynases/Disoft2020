Public Class EmisorEnvio
    Public Class Detalle
        Public Property product_id As String
        Public Property product_code As String
        Public Property product_name As String
        Public Property price As Integer
        Public Property quantity As Double
        Public Property total As Double
        Public Property unidad_medida As Double
        Public Property codigo_producto_sin As Double
        Public Property codigo_actividad As String
        Public Property discount As Double
    End Class

    Public Class Emisor
        Public Property customer_id As Integer
        Public Property customer As String
        Public Property nit_ruc_nif As String
        Public Property subtotal As Double
        Public Property total_tax As Double
        Public Property discount As Double
        Public Property total As Double
        Public Property invoice_date_time As String
        Public Property monto_giftcard As Double
        Public Property currency_code As String
        Public Property codigo_sucursal As Integer
        Public Property punto_venta As Integer
        Public Property codigo_documento_sector As Integer
        Public Property tipo_documento_identidad As Integer
        Public Property codigo_metodo_pago As Integer
        Public Property codigo_moneda As Integer
        Public Property numero_tarjeta As Integer
        Public Property tipo_cambio As Integer
        Public Property tipo_factura_documento As Integer
        'Public Property email As String
        'Public Property actividadEconomica As Integer
        Public Property detalles As Detalle()
    End Class
End Class
