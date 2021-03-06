USE [BDDiconCF]
GO
/****** Object:  StoredProcedure [dbo].[sp_Mam_Asiento]    Script Date: 09/12/2019 06:57:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--drop procedure sp_Mam_TS006
ALTER PROCEDURE [dbo].[sp_Mam_Asiento] (@tipo int,@seuact nvarchar(10)='',@categoria int=-1,@canumi int=-1,
@cuenta nvarchar(20)='',@descripcion nvarchar(200)='',@empresa int=-1,@sector int=-1,@vcnumi int=-1,@servicio int=-1,@fechaI date=null,
@fechaF date=null,@sucursal int=-1,@Estado int=-1, @tventa int=-1,@modulo int=-1,@factura int=-1,@Id int=-1)
AS
BEGIN
	DECLARE @newHora nvarchar(5)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))

	DECLARE @newFecha date
	set @newFecha=GETDATE()
IF @tipo=10 --MOSTRAR CUENTAS
	BEGIN
		BEGIN TRY	
  
  select isnull(cuenta .canumi ,-1) as canumi,isnull(cuenta .cacta,0) as nro,isnull(cuenta .cadesc,'') as cadesc ,b.Porcentaje  as chporcen,b.Debe as  chdebe ,b.Haber  as chhaber,cast(null as decimal (18,2)) as tc
   ,cast(null as decimal (18,2)) as debe,cast(null as decimal (18,2)) as haber,cast(null as decimal (18,2)) as debesus
   ,cast(null as decimal (18,2)) as habersus,cast(null as int) as variable,cast(null as int) as linea
  from Plantilla  as a 
  inner join DetallePlantilla  as b on a.Id  =b.PlantillaId   
  left join TC001 as cuenta on cuenta.canumi =b.CuentaId 
  where a.Id =@Id 
 

		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END
IF @tipo=12 --BUSCAR PADRE
	BEGIN
		BEGIN TRY	
select padre.canumi ,padre .cacta as nro ,padre .cadesc ,0 as chporcen,0 as chdebe ,0 as chhaber ,cast(null as decimal (18,2)) as
 debe,cast(null as decimal (18,2)) as haber,cast(null as int) as variable,cast(null as int) as linea
  from TC001 as cuenta,TC001 as padre,TC0071 as b where cuenta.canumi =b.chnumitc1 
  and cuenta .capadre =padre.canumi  
  and cuenta.canumi =@canumi 
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END

IF @tipo=11 --SUMAR TOTAL DE VENTAS POR CATEGORIA
	BEGIN
		BEGIN TRY	
  select  Isnull(Sum(b.vdtotdesc),0) as total
  from TV002 as a inner join TV0021 as b on a.vcnumi =b.vdvc2numi 
  and a.vcsector =@categoria and a.vcest =0
  and a.vcfdoc >=@fechaI and a.vcfdoc <=@fechaF 
  and a.vcidcore <=0
  and a.vcalm =@sucursal
  and (ISNULL((select top 1 sefactu from TS006 where senumiserv=b.vdserv),1))=1 --aumentado por mi
  inner join TFV001 as factura on factura.fvanumi =a.vcnumi and factura.fvaest in (1,4)
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END

IF @tipo=110101 --SUMAR TOTAL DE VENTAS POR CATEGORIA para totales de ventas y tome en cuenta las ventas por recibo
	BEGIN
		BEGIN TRY	
  select  Isnull(Sum(b.vdtotdesc),0) as total
  from TV002 as a inner join TV0021 as b on a.vcnumi =b.vdvc2numi 
  and a.vcsector =@categoria and a.vcest =0
  and a.vcfdoc >=@fechaI and a.vcfdoc <=@fechaF 
  and a.vcidcore <=0
  and a.vcalm =@sucursal and
  IIF(exists(select x.fvanumi from TFV001 x where a.vcnumi=x.fvanumi) ,(select top 1 y.fvaest from TFV001 y where a.vcnumi=y.fvanumi),1) in (1,4)
  
  -- and (ISNULL((select top 1 sefactu from TS006 where senumiserv=b.vdserv),1))=1 --aumentado por mi
  --inner join TFV001 as factura on factura.fvanumi =a.vcnumi and factura.fvaest =1


 ---- UNION
 ---- select  Isnull(Sum(b.vdtotdesc),0) as total
 ---- from TV002 as a inner join TV0021 as b on a.vcnumi =b.vdvc2numi 
 ---- and a.vcsector =@categoria and a.vcest =0
 ---- and a.vcfdoc >=@fechaI and a.vcfdoc <=@fechaF 
 ---- and a.vcidcore <=0
 ---- and a.vcalm =@sucursal and
 ---- a.vcnumi not in(select x.fvanumi from TFV001 as x )
 ------ and (ISNULL((select top 1 sefactu from TS006 where senumiserv=b.vdserv),1))=1 --aumentado por mi
 ------ inner join TFV001 as factura on factura.fvanumi =a.vcnumi and factura.fvaest =1
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END

IF @tipo=13  -------Lavadero  Total Servicio
	BEGIN
		BEGIN TRY	
 select   Isnull(Sum(b.vdtotdesc),0)  as total
  from TV002 as a inner join TV0021 as b on a.vcnumi =b.vdvc2numi 
  and a.vcsector =@sector and b.vdserv >0
    and a.vcfdoc >=@fechaI and a.vcfdoc <=@fechaF 
  and a.vcidcore <=0 and a.vcalm =@sucursal 
  --inner join TFV001 as factura on factura.fvanumi =a.vcnumi and factura.fvaest =1 --esto para que tome en cuenta a los recibos
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END
IF @tipo=14  -------Lavadero  Total Productos
	BEGIN
		BEGIN TRY	
  select   Isnull(Sum(b.vdtotdesc),0) as total 
  from TV002 as a inner join TV0021 as b on a.vcnumi =b.vdvc2numi 
  and a.vcsector =3 and b.vdprod >0 
 inner join TFV001 as factura on factura.fvanumi =a.vcnumi and factura.fvaest in (1,4)
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END
IF @tipo=15  -------Listar Cuentas Servicios Lavadero
	BEGIN
		BEGIN TRY	
    select distinct hijo.canumi , a.senrocuenta ,a.seref ,a.seest 
from TS006 as a inner join DBDies.dbo.TCE004 as servicios on a.senumiserv =servicios .ednumi 
and servicios .edtipo =3
inner join TC001 as hijo on hijo .cacta =a.senrocuenta 
inner join TC001 as padre on padre.canumi =hijo.capadre 
and padre.cacta =@cuenta 
and padre.caemp =@empresa   ---Emrpresa
union

select distinct hijo.canumi,a.senrocuenta ,a.seref ,a.seest 
from TS006 as a inner join DBDies.dbo.TCL003  as servicios on a.senumiserv  =servicios .ldnumi  
and a.seest =2
inner join TC001 as hijo on hijo .cacta =a.senrocuenta 
inner join TC001 as padre on padre.canumi =hijo.capadre 
and padre .cacta =@cuenta 
and padre.caemp =@empresa ---Empresa
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END

IF @tipo=16  -------Total Por Una Cuenta
	BEGIN
		BEGIN TRY	
    select isnull(sum (b.vdtotdesc ),0) as total,isnull(sum(b.vdcmin),0) as cantidad
   from TV002 as a inner join TV0021 as b on a.vcnumi =b.vdvc2numi and b.vdserv >0 
   inner join TS006 as cuenta on cuenta .senumiserv =b.vdserv 
   and cuenta.senrocuenta =@cuenta and cuenta.seref =@descripcion and cuenta .seest =1
   and a.vcfdoc >=@fechaI and a.vcfdoc <=@fechaF 
    and a.vcidcore <=0 and a.vcalm =@sucursal and a.vcsector >0
	inner join TFV001 as factura on factura.fvanumi =a.vcnumi and factura.fvaest in (1,4)
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END

IF @tipo=17 
	BEGIN
		BEGIN TRY	
   select distinct padre.canumi,padre.caemp ,padre.cacta ,padre.cadesc ,padre.caniv ,padre .camon ,padre .catipo ,padre .capadre 
from TS006 as a inner join DBDies.dbo.TCE004 as servicios on a.senumiserv =servicios .ednumi 
and servicios .edtipo =3
inner join TC001 as hijo on hijo .cacta =a.senrocuenta 
inner join TC001 as padre on padre.canumi =hijo.capadre 
and padre.caemp =@empresa  ---Emrpresa
union

select distinct padre.canumi,padre.caemp ,padre.cacta ,padre.cadesc ,padre.caniv ,padre .camon ,padre .catipo ,padre .capadre 
from TS006 as a inner join DBDies.dbo.TCL003  as servicios on a.senumiserv  =servicios .ldnumi  
and a.seest =2
inner join TC001 as hijo on hijo .cacta =a.senrocuenta 
inner join TC001 as padre on padre.canumi =hijo.capadre 
and padre.caemp =@empresa ---Empresa

order by padre.canumi asc

 
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END


IF @tipo=19  -------Obtener Nombre de la Cuenta
	BEGIN
		BEGIN TRY	
  select   a.cadesc 
  from TC001 as a where a.cacta =@cuenta
 
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END


IF @tipo=26  -------Obtener Cuenta Diferencia  de Cambio
	BEGIN
		BEGIN TRY	
    select hijo.canumi ,hijo.cacta,hijo.cadesc 
	from TC001 as hijo where hijo.canumi =@cuenta 
 union 
 select padre.canumi ,padre.cacta ,padre .cadesc 
 from TC001 as padre inner join TC001 as hijo 
 on hijo.capadre =padre.canumi 
 and hijo.canumi =@cuenta
 order by canumi asc
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END

IF @tipo=27 --SUMAR TOTAL DE VENTAS POR CATEGORIA
	BEGIN
		BEGIN TRY	
  select distinct  a.vcnumi 
  from TV002 as a inner join TV0021 as b on a.vcnumi =b.vdvc2numi 
  and a.vcsector =@categoria and a.vcest =0
  and a.vcfdoc >=@fechaI and a.vcfdoc <=@fechaF 
   and a.vcidcore <=0
   and a.vcalm =@sucursal 
   inner join TFV001 as factura on factura.fvanumi =a.vcnumi and factura.fvaest in (1, 4)
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END

IF @tipo=29  -------Total Por Una Cuenta
	BEGIN
		BEGIN TRY	
    select isnull(sum (b.vdtotdesc ),0) as total,isnull(sum(b.vdcmin),0) as cantidad
   from TV002 as a inner join TV0021 as b on a.vcnumi =b.vdvc2numi and b.vdserv >0 and a.vcfactanul=1
   inner join TS006 as cuenta on cuenta .senumiserv =b.vdserv 
   and cuenta.senrocuenta =@cuenta and cuenta.seref =@descripcion and cuenta .seest =1
   and a.vcfdoc >=@fechaI and a.vcfdoc <=@fechaF 
    and a.vcidcore <=0 and a.vcalm =@sucursal and a.vcsector >0
	and a.vcsector=@sector
	--inner join TFV001 as factura on factura.fvanumi =a.vcnumi and factura.fvaest =1--comentado para que tome en cuenta las venta con recibo solamente
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END



IF @tipo=30 --SUMAR TOTAL DE VENTAS POR CATEGORIA CLIENTES POR COOBRAR
	BEGIN
		BEGIN TRY	
  select  Isnull(Sum(b.vdtotdesc),0) as total 
  from TV002 as a inner join TV0021 as b on a.vcnumi =b.vdvc2numi 
  and a.vcsector =@categoria and a.vcest =0
  and a.vcfdoc >=@fechaI and a.vcfdoc <=@fechaF 
  and a.vcidcore <=0
  and a.vcalm =@sucursal 
  and a.vctipo =0
  inner join TFV001 as factura on factura.fvanumi =a.vcnumi and factura.fvaest in (1,4)
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END

IF @tipo=31 --SUMAR TOTAL DE VENTAS POR CATEGORIA CLIENTES POR COOBRAR
	BEGIN
		BEGIN TRY	
  select clienteCobrar .cjnumi ,clienteCobrar .cjnombre , Isnull(Sum(b.vdtotdesc),0) as total 
  from TV002 as a inner join TV0021 as b on a.vcnumi =b.vdvc2numi 
  and a.vcsector =@categoria and a.vcest =0
  and a.vcfdoc >=@fechaI and a.vcfdoc <=@fechaF 
  and a.vcidcore <=0
  and a.vcalm =@sucursal 
  and a.vctipo =0
  inner join TC009 as clienteCobrar on clienteCobrar .cjnumi=a.vcclietc9 
  inner join TFV001 as factura on factura.fvanumi =a.vcnumi and factura.fvaest in (1,4)
  group  by clienteCobrar .cjnumi ,clienteCobrar .cjnombre 
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END
IF @tipo=32 --SUMAR TOTAL DE VENTAS POR CATEGORIA 
	BEGIN
		BEGIN TRY	
  select  Isnull(Sum(b.vdtotdesc),0) as total,isnull(sum(b.vdcmin),0) as cantidad
  from TV002 as a inner join TV0021 as b on a.vcnumi =b.vdvc2numi 
  and a.vcsector =@categoria and a.vcest =0
  and a.vcfdoc >=@fechaI and a.vcfdoc <=@fechaF 
  and a.vcidcore <=0
  and a.vcalm =@sucursal 
  and a.vctipo = @tventa  --Tipo de Venta
  and IIF(exists(select x.fvanumi from TFV001 x where a.vcnumi=x.fvanumi) ,(select top 1 y.fvaest from TFV001 y where a.vcnumi=y.fvanumi),1) in (1,4)

  --inner join TFV001 as factura on factura.fvanumi =a.vcnumi and factura.fvaest =1 --comentado para que tome en cuenta recibos
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END

IF @tipo=322 --SUMAR TOTAL DE VENTAS POR CATEGORIA 
	BEGIN
		BEGIN TRY	
    select  d.canumi, c.cacuenta, c.canombre,  Isnull(Sum(b.vdtotdesc),0) as total,isnull(sum(b.vdcmin),0) as cantidad
  from TV002 as a inner join TV0021 as b on a.vcnumi =b.vdvc2numi 
  inner join dbdies.dbo.ba001 c on a.vcbanco = c.canumi
  inner join TC001 d on c.cacuenta = d.cacta
  and a.vcsector =@categoria and a.vcest =0
  and a.vcfdoc >=@fechai and a.vcfdoc <= @fechaf 
  and a.vcidcore <=0
  and a.vcalm = @sucursal
  and a.vctipo = @tventa  --Tipo de Venta
  and 1=IIF(exists(select x.fvanumi from TFV001 x where a.vcnumi=x.fvanumi) ,(select top 1 y.fvaest from TFV001 y where a.vcnumi=y.fvanumi),1)
  group by d.canumi, c.cacuenta, c.canombre
  --inner join TFV001 as factura on factura.fvanumi =a.vcnumi and factura.fvaest =1 --comentado para que tome en cuenta recibos
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END

IF @tipo=33 --NumiCuentaPorCobrar
	BEGIN
		BEGIN TRY	
select a.NumiCuenta as cuenta,cuenta .cacta as nro,cuenta .cadesc as descripcion,
padre.canumi as cuentapadre,padre.cacta as nropadre,padre.cadesc as descripcionpadre
from SY000 as a 
inner join TC001 as cuenta on cuenta.canumi =a.NumiCuenta 
inner join TC001 as padre on padre.canumi =cuenta .capadre 
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH

END


		IF @tipo=35 --preguntar si es un servicio
	BEGIN
		BEGIN TRY	
			
			select Plantilla .Id ,Plantilla .Descripcion ,Plantilla .Tipo ,Plantilla .Factura
			from Plantilla where id=@Id 
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END


	
			IF @tipo=36 --TOTAL COMPRAS CON FACTURA
	BEGIN
		BEGIN TRY	
			
			
			select isnull(Sum(transacciones .aamontototal ),0)as total
			from TPA001 as transacciones where transacciones .aatipo =@modulo  and transacciones .aaemision =@factura  and transacciones .aaestado in (1,2)
			and transacciones .aanumiasiento =0 and transacciones .aafecha >=@fechaI  and transacciones .aafecha <=@fechaF
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END
		IF @tipo=37 --Obtener Detalle Plantilla por ID
	BEGIN
		BEGIN TRY	
	
		select *
		from DetallePlantilla where PlantillaId =@Id and CuentaId =@cuenta 
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END

		IF @tipo=38 --TOTAL Ventas Contado
	BEGIN
		BEGIN TRY	
			
			
			select isnull(sum(venta.tatotal) ,0)as total
			from TPA001 as transacciones,DBDinoM.dbo.TV001 as venta where transacciones .aatipo =@modulo  and transacciones .aaemision =@factura  and transacciones .aaestado in (1,2)
			and transacciones .aanumiasiento =0 and transacciones .aafecha >=@fechaI  and transacciones .aafecha <=@fechaF
			and venta.tanumi=transacciones .aanumipadre and venta.tatven =1
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END
		IF @tipo=39 --TOTAL Ventas Credito
	BEGIN
		BEGIN TRY	
			
			
			select isnull(sum(venta.tatotal) ,0)as total
			from TPA001 as transacciones,DBDinoM.dbo.TV001 as venta where transacciones .aatipo =@modulo  and transacciones .aaemision =@factura  and transacciones .aaestado in (1,2)
			and transacciones .aanumiasiento =0 and transacciones .aafecha >=@fechaI  and transacciones .aafecha <=@fechaF
			and venta.tanumi=transacciones .aanumipadre and venta.tatven =0
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END
	IF @tipo=40 --TOTAL Ventas Costos
	BEGIN
		BEGIN TRY	
			
			
select isnull(sum (compra.total ),0) as total
from (
			select isnull((
			select sum(detalle.tbptot2 )
			from DBDinoM.dbo.TV0011 as detalle where detalle .tbtv1numi =venta.tanumi  ) ,0)as total
			from TPA001 as transacciones,DBDinoM.dbo.TV001 as venta where transacciones .aatipo =@modulo  and transacciones .aaemision =@factura  and transacciones .aaestado in (1,2)
			and transacciones .aanumiasiento =0 and transacciones .aafecha >=@fechaI  and transacciones .aafecha <=@fechaF
			and venta.tanumi=transacciones .aanumipadre ) as compra
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END

		IF @tipo=41 --Obtener Pagos de Cuentas por Pagar
	BEGIN
		BEGIN TRY	
			
			
select proveedor.ydnumi,proveedor.yddesc as proveedor,cast(pagos.tdnrorecibo as nvarchar (50)) as nroDocumento,pagos .tdmonto  
from DBDinoM .dbo.TC0012 as credito
inner join DBDinoM .dbo.TY004 as proveedor
on proveedor .ydnumi =credito.tcty4prov 
inner join DBDinoM .dbo.TC00121 as pagos on pagos.tdtc12numi =credito .tcnumi
  inner join DBDinoM .dbo.TC0013 as cabezera on cabezera .tenumi =pagos.tdtc13numi 
  and cabezera .tenumi in (select aanumipadre  from TPA001 as transaccion where transaccion .aatipo =3 and transaccion .aanumiasiento =0
  and transaccion .aafecha >=@fechaI and transaccion .aafecha <=@fechaF )
where pagos.tdfechaPago >=@fechaI and pagos.tdfechaPago <=@fechaF  
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END

		IF @tipo=42 --Obtener Pagos de Cuentas por Pagar de CAJa
	BEGIN
		BEGIN TRY	
			
			
select proveedor.yddesc as proveedor,cast(pagos.tdnrorecibo  as nvarchar (50)) as nroDocumento,pagos .tdmonto  
from DBDinoM .dbo.TC0012 as credito
inner join DBDinoM .dbo.TY004 as proveedor
on proveedor .ydnumi =credito.tcty4prov 
inner join DBDinoM .dbo.TC00121 as pagos on pagos.tdtc12numi =credito .tcnumi
  inner join DBDinoM .dbo.TC0013 as cabezera on cabezera .tenumi =pagos.tdtc13numi 
  and cabezera .tenumi in (select aanumipadre  from TPA001 as transaccion where transaccion .aatipo =3 and transaccion .aanumiasiento =0
  and transaccion .aafecha >=@fechaI and transaccion .aafecha <=@fechaF )
where pagos.tdfechaPago >=@fechaI and pagos.tdfechaPago <=@fechaF  
and pagos.tdty3banco =1
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END

		IF @tipo=43 --Listar Bancos
	BEGIN
		BEGIN TRY	
			
  select distinct isnull(cuenta .canumi ,0) as canumi,isnull(cuenta .cacta,0) as nro,isnull(cuenta .cadesc,'') as cadesc ,b.Porcentaje  as chporcen,b.Debe as  chdebe ,b.Haber  as chhaber,cast(null as decimal (18,2)) as tc
   ,cast(null as decimal (18,2)) as debe,cast(null as decimal (18,2)) as haber,cast(null as decimal (18,2)) as debesus
   ,cast(null as decimal (18,2)) as habersus,cast(null as int) as variable,cast(null as int) as linea
  from Plantilla  as a 
  inner join DetallePlantilla  as b on a.Id  =b.PlantillaId   
  inner join BA001 as banco on banco.caestado =1
  inner  join TC001 as cuenta on cuenta.canumi =banco.catc001numi
  inner join DBDinoM .dbo.TC00121 as pagos on pagos.tdty3banco =banco .canumi  
  inner join DBDinoM .dbo.TC0013 as cabezera on cabezera .tenumi =pagos.tdtc13numi 
  and cabezera .tenumi in (select aanumipadre  from TPA001 as transaccion where transaccion .aatipo =3 and transaccion .aanumiasiento =0
  and transaccion .aafecha >=@fechaI and transaccion .aafecha <=@fechaF )
  where b.CuentaId  =-1 and a.Id =@Id  and pagos.tdfechaPago >=@fechaI and pagos.tdfechaPago <=@fechaF  
  and banco.canumi <>1

		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END

			IF @tipo=44 --Listar Pagos por BAncos
	BEGIN
		BEGIN TRY	
			
select proveedor.yddesc as proveedor,cast(pagos.tdnrorecibo  as nvarchar (50)) as nroDocumento,pagos .tdmonto
from DBDinoM .dbo.TC0012 as credito
inner join DBDinoM .dbo.TY004 as proveedor
on proveedor .ydnumi =credito.tcty4prov 
inner join DBDinoM .dbo.TC00121 as pagos on pagos.tdtc12numi =credito .tcnumi
inner join BA001 as banco on banco.canumi =pagos.tdty3banco 
  inner join DBDinoM .dbo.TC0013 as cabezera on cabezera .tenumi =pagos.tdtc13numi 
  and cabezera .tenumi in (select aanumipadre  from TPA001 as transaccion where transaccion .aatipo =3 and transaccion .aanumiasiento =0
  and transaccion .aafecha >=@fechaI and transaccion .aafecha <=@fechaF )
  where  pagos.tdfechaPago >=@fechaI and pagos.tdfechaPago <=@fechaF and banco.catc001numi =@canumi 
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END

			IF @tipo=45 --Obtener Pagos de Cuentas por Pagar
	BEGIN
		BEGIN TRY	
			
select  proveedor.ydnumi,proveedor.yddesc as cliente,cast(pagos.tdnrorecibo   as nvarchar (50)) as nroDocumento,pagos .tdmonto  
from DBDinoM .dbo.TV0012 as credito
inner join DBDinoM .dbo.TY004 as proveedor
on proveedor .ydnumi =credito.tcty4clie  
inner join DBDinoM .dbo.TV00121 as pagos on pagos.tdtv12numi  =credito .tcnumi
  inner join DBDinoM .dbo.TV0013 as cabezera on cabezera .tenumi =pagos.tdtv13numi
  and cabezera.tenumi  in (select aanumipadre  from TPA001 as transaccion where transaccion .aatipo =4 and transaccion .aanumiasiento =0
  and transaccion .aafecha >=@fechaI and transaccion .aafecha <=@fechaF ) 
where pagos.tdfechaPago >=@fechaI and pagos.tdfechaPago <=@fechaF  

		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END
		IF @tipo=46 --Obtener Pagos de Cuentas por Pagar de CAJa
	BEGIN
		BEGIN TRY	

select  cliente.yddesc as cliente,cast(pagos.tdnrorecibo  as nvarchar (50)) as nroDocumento,pagos .tdmonto  
from DBDinoM .dbo.TV0012  as credito
inner join DBDinoM .dbo.TY004 as cliente
on cliente .ydnumi =credito.tcty4clie  
inner join DBDinoM .dbo.TV00121  as pagos on pagos.tdtv12numi  =credito .tcnumi
  inner join DBDinoM .dbo.TV0013 as cabezera on cabezera .tenumi =pagos.tdtv13numi
  and cabezera.tenumi  in (select aanumipadre  from TPA001 as transaccion where transaccion .aatipo =4 and transaccion .aanumiasiento =0
  and transaccion .aafecha >=@fechaI and transaccion .aafecha <=@fechaF )  
where pagos.tdfechaPago >=@fechaI and pagos.tdfechaPago <=@fechaF
and pagos.tdty3banco =1
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END

	
		IF @tipo=47 --Listar Bancos
	BEGIN
		BEGIN TRY	
			

select  isnull(cuenta .canumi ,0) as canumi,isnull(cuenta .cacta,0) as nro,isnull(cuenta .cadesc,'') as cadesc ,b.Porcentaje  as chporcen,b.Debe as  chdebe ,b.Haber  as chhaber,cast(null as decimal (18,2)) as tc
   ,cast(null as decimal (18,2)) as debe,cast(null as decimal (18,2)) as haber,cast(null as decimal (18,2)) as debesus
   ,cast(null as decimal (18,2)) as habersus,cast(null as int) as variable,cast(null as int) as linea
  from Plantilla  as a 
  inner join DetallePlantilla  as b on a.Id  =b.PlantillaId   
  inner join BA001 as banco on banco.caestado =1
  inner  join TC001 as cuenta on cuenta.canumi =banco.catc001numi
  inner join DBDinoM .dbo.TV00121  as pagos on pagos.tdty3banco =banco .canumi 
  inner join DBDinoM .dbo.TV0013 as cabezera on cabezera .tenumi =pagos.tdtv13numi
  and cabezera.tenumi  in (select aanumipadre  from TPA001 as transaccion where transaccion .aatipo =4 and transaccion .aanumiasiento =0
  and transaccion .aafecha >=@fechaI and transaccion .aafecha <=@fechaF ) 
  where b.CuentaId  =-1 and a.Id =@Id  and pagos.tdfechaPago >=@fechaI and pagos.tdfechaPago <=@fechaF  
  and banco.canumi <>1
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END

	IF @tipo=48 --Listar Pagos por BAncos
	BEGIN
		BEGIN TRY	
select  proveedor.yddesc as cliente,cast(pagos.tdnrorecibo   as nvarchar (50)) as nroDocumento,pagos .tdmonto
from DBDinoM .dbo.TV0012  as credito
inner join DBDinoM .dbo.TY004 as proveedor
on proveedor .ydnumi =credito.tcty4clie  
inner join DBDinoM .dbo.TV00121  as pagos on pagos.tdtv12numi  =credito .tcnumi
inner join BA001 as banco on banco.canumi =pagos.tdty3banco 
inner join DBDinoM .dbo.TV0013 as cabezera on cabezera .tenumi =pagos.tdtv13numi
  and cabezera.tenumi  in (select aanumipadre  from TPA001 as transaccion where transaccion .aatipo =4 and transaccion .aanumiasiento =0) 
  where  pagos.tdfechaPago >=@fechaI and pagos.tdfechaPago <=@fechaF and banco.catc001numi =@canumi
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END
	IF @tipo=49 --Listar Clientes Credito
	BEGIN
		BEGIN TRY	

select cliente.ydnumi,cliente.yddesc as cliente,cast(factura.fvanfac as nvarchar(20)) as nroDocumento,venta.tatotal as monto
from DBDinoM .dbo.TV001 as venta 
inner join DBDinoM .dbo.TY004 as cliente
on cliente.ydnumi =venta.taclpr 
inner join DBDinoM .dbo.TFV001 as factura on factura .fvanumi =venta.tanumi 
where venta.tafdoc >=@fechaI and venta.tafdoc <=@fechaF and venta.tatven=0 and venta.taest=1

		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END

		IF @tipo=50 --Listar Clientes Credito
	BEGIN
		BEGIN TRY	

select proveedor.cmnumi as  ydnumi,proveedor .cmdesc  as proveedor,cast(factura.fcanfac as nvarchar(20)) as nroDocumento,compra.caatotal  as monto
from BDDistBHF_CF .dbo.TCA001  as compra
inner join BDDistBHF_CF .dbo.TC010  as proveedor
on proveedor .cmnumi  =compra.caaprov  
inner join TFC001 as factura on factura.fcanumito11 =compra.caanumi  
where compra.caafdoc  >=@fechaI and compra.caafdoc <=@fechaF and compra.caatven  =0 and compra .caaconsigna  =0  and compra.caaretenc  =0
and compra.caaemision =1 and compra.caaest  =1
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END

		IF @tipo=51 --Listar Clientes Credito
	BEGIN
		BEGIN TRY	

select proveedor.cmnumi  as ydnumi,proveedor .cmdesc  as proveedor,cast(compra .caanumi  as nvarchar(20)) as nroDocumento,compra.caatotal as monto
from BDDistBHF_CF .dbo.TCA001  as compra
inner join BDDistBHF_CF .dbo.TC010  as proveedor
on proveedor .cmnumi  =compra.caaprov  
where compra.caafdoc >=@fechaI and compra.caafdoc <=@fechaF and compra.caatven =0 and compra .caaconsigna =0  and compra.caaretenc =0
and compra.caaemision=0 and compra.caaest=1
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END


		IF @tipo=52 --Listar Clientes Credito
	BEGIN
		BEGIN TRY	

select proveedor .cmdesc  as proveedor,cast(compra .caanumi  as nvarchar(20)) as nroDocumento,compra.caatotal  as monto
from BDDistBHF_CF .dbo.TCA001  as compra
inner join BDDistBHF_CF .dbo.TC010  as proveedor
on proveedor .cmnumi  =compra.caaprov 
where compra.caafdoc >=@fechaI and compra.caafdoc <=@fechaF and compra.caatven =0 and compra .caaconsigna =0  and compra.caaretenc =1
and compra.caaemision=0 and compra.caaest =1
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END

			IF @tipo=53 --Listar Clientes Credito  para la TC009
	BEGIN
		BEGIN TRY	

select cast(proveedor .cmnumi as nvarchar (20)) as cjci,proveedor .cmdesc  as cjnombre,2 as cjtipo,0 as cjnumiTc001
from BDDistBHF_CF .dbo.TCA001  as compra
inner join BDDistBHF_CF .dbo.TC010  as proveedor
on proveedor .cmnumi  =compra.caaprov 
inner join TFC001 as factura on factura.fcanumito11 =compra.caanumi 
where compra.caafdoc >=@fechaI and compra.caafdoc <=@fechaF and compra.caatven =0 and compra .caaconsigna =0  and compra.caaretenc =0
and compra.caaemision=1 and compra.caaest =1
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END

		IF @tipo=54 --Listar Clientes Credito
	BEGIN
		BEGIN TRY	

select cast(proveedor .cmnumi as nvarchar (20)) as cjci,proveedor .cmdesc  as cjnombre,2 as cjtipo,0 as cjnumiTc001
from BDDistBHF_CF .dbo.TCA001  as compra
inner join BDDistBHF_CF .dbo.TC010  as proveedor
on proveedor .cmnumi  =compra.caaprov  
where compra.caafdoc >=@fechaI and compra.caafdoc <=@fechaF and compra.caatven =0 and compra .caaconsigna =0  and compra.caaretenc =0
and compra.caaemision=0 and compra.caaest=1
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END

	IF @tipo=55 --Listar Clientes Credito
	BEGIN
		BEGIN TRY	

select cast(cliente .ccnumi as nvarchar (20)) as cjci,cliente.ccdesc as cjnombre,1 as cjtipo,0 as cjnumiTc001
from BDDistBHF_CF .dbo.TV001 as venta 
inner join BDDistBHF_CF.dbo.TC004 as cliente
on cliente.ccnumi  =venta.taclpr 
inner join BDDistBHF_CF.dbo.TFV001 as factura on factura .fvanumi =venta.tanumi 
where venta.tafdoc >=@fechaI and venta.tafdoc <=@fechaF and venta.tatven=0 and venta.taest=1

		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END

			IF @tipo=56
	BEGIN
		BEGIN TRY	
			
			
select cast(proveedor .ydnumi as nvarchar (20)) as cjci,proveedor .yddesc as cjnombre,2 as cjtipo,0 as cjnumiTc001  
from DBDinoM .dbo.TC0012 as credito
inner join DBDinoM .dbo.TY004 as proveedor
on proveedor .ydnumi =credito.tcty4prov 
inner join DBDinoM .dbo.TC00121 as pagos on pagos.tdtc12numi =credito .tcnumi
  inner join DBDinoM .dbo.TC0013 as cabezera on cabezera .tenumi =pagos.tdtc13numi 
  and cabezera .tenumi in (select aanumipadre  from TPA001 as transaccion where transaccion .aatipo =3 and transaccion .aanumiasiento =0
  and transaccion .aafecha >=@fechaI and transaccion .aafecha <=@fechaF )
where pagos.tdfechaPago >=@fechaI and pagos.tdfechaPago <=@fechaF  
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END

			IF @tipo=58 --Obtener Pagos de Cuentas por Pagar
	BEGIN
		BEGIN TRY	
			
select   cast(proveedor .ydnumi as nvarchar (20)) as cjci,proveedor.yddesc as cliente,1 as cjtipo,0 as cjnumiTc001  
from DBDinoM .dbo.TV0012 as credito
inner join DBDinoM .dbo.TY004 as proveedor
on proveedor .ydnumi =credito.tcty4clie  
inner join DBDinoM .dbo.TV00121 as pagos on pagos.tdtv12numi  =credito .tcnumi
  inner join DBDinoM .dbo.TV0013 as cabezera on cabezera .tenumi =pagos.tdtv13numi
  and cabezera.tenumi  in (select aanumipadre  from TPA001 as transaccion where transaccion .aatipo =4 and transaccion .aanumiasiento =0
  and transaccion .aafecha >=@fechaI and transaccion .aafecha <=@fechaF ) 
where pagos.tdfechaPago >=@fechaI and pagos.tdfechaPago <=@fechaF  

		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END

				IF @tipo=58 --Obtener Pagos de Cuentas por Pagar
	BEGIN
		BEGIN TRY	
			
select  cast(proveedor .ydnumi as nvarchar (20)) as cjci,proveedor.yddesc as cliente,1 as cjtipo,0 as cjnumiTc001 
from DBDinoM .dbo.TV0012 as credito
inner join DBDinoM .dbo.TY004 as proveedor
on proveedor .ydnumi =credito.tcty4clie  
inner join DBDinoM .dbo.TV00121 as pagos on pagos.tdtv12numi  =credito .tcnumi
  inner join DBDinoM .dbo.TV0013 as cabezera on cabezera .tenumi =pagos.tdtv13numi
  and cabezera.tenumi  in (select aanumipadre  from TPA001 as transaccion where transaccion .aatipo =4 and transaccion .aanumiasiento =0
  and transaccion .aafecha >=@fechaI and transaccion .aafecha <=@fechaF ) 
where pagos.tdfechaPago >=@fechaI and pagos.tdfechaPago <=@fechaF  

		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@seuact)
		END CATCH		

	END
End






