USE [BDDistBHF_CF]
GO
/****** Object:  StoredProcedure [dbo].[sp_Mam_TV00121Cheque]    Script Date: 06/01/2020 06:20:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--drop procedure sp_Mam_TV00121Cheque
ALTER PROCEDURE [dbo].[sp_Mam_TV00121Cheque] (@tipo int,@tenumi int=-1,@tefdoc date=null,
@tety4vend int=-1,@teobs nvarchar(100)='',@tdnumi int=-1,@teuact nvarchar(10)='',@TV00121 TV00121TypeCheque ReadOnly,
@credito int=-1)

AS
BEGIN
	DECLARE @newHora nvarchar(5)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))

	DECLARE @newFecha date
	set @newFecha=GETDATE()

	IF @tipo=-1 --ELIMINAR REGISTRO
	BEGIN
		BEGIN TRY

		DELETE FROM TV00121  WHERE tdtv13numi =@tenumi 
		delete from TV0013 where tenumi=@tenumi 
			select @tdnumi as newNumi  --Consultar que hace newNumi
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),-1,@newFecha,@newHora,@teuact)
		END CATCH
	END

	IF @tipo=1 --NUEVO REGISTRO
	BEGIN
		BEGIN TRY 

		      set @tenumi=IIF((select COUNT(tenumi) from TV0013)=0,0,(select MAX(tenumi) from TV0013))+1
			  insert into TV0013 values(@tenumi ,@tefdoc,@tety4vend ,@teobs ,@newFecha ,@newHora ,@teuact )
		----INSERTO EL DETALLE
				INSERT INTO TV00121 (tdtv12numi,tdtv13numi  ,tdnrodoc ,tdfechaPago ,tdmonto ,tdnrorecibo,tdty3banco ,
				tdnrocheque ,tdfact ,tdhact ,tduact)

			SELECT td.tdtv12numi ,@tenumi ,td.tdnrodoc ,@newFecha ,td.tdmonto ,td.tdnrorecibo ,td.tdty3banco,
			td.tdnrocheque, @newFecha  ,@newHora  ,@teuact 
			FROM @TV00121 AS td where td.estado =0

			select @tdnumi as newNumi
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@teuact)
		END CATCH
	END

		IF @tipo=2 --NUEVO REGISTRO
	BEGIN
		BEGIN TRY 

			  update TV0013 set tefdoc =@tefdoc ,
			  tety4vend =@tety4vend ,teobs=@teobs,
			  tefact =@newFecha ,tehact =@newHora ,teuact =@teuact  
			  where tenumi =@tenumi 
		----INSERTO EL DETALLE
				INSERT INTO TV00121 (tdtv12numi ,tdtv13numi ,tdnrodoc ,tdfechaPago ,tdmonto ,tdnrorecibo,tdty3banco ,
				tdnrocheque ,tdfact ,tdhact ,tduact)
			SELECT td.tdtv12numi,@tenumi  ,td.tdnrodoc ,@newFecha ,td.tdmonto ,td.tdnrorecibo ,td.tdty3banco,
			td.tdnrocheque, @newFecha  ,@newHora  ,@teuact 
			FROM @TV00121 AS td where td.estado =0

		    UPDATE TV00121 
			SET tdmonto=td.tdmonto ,tdnrorecibo =td.tdnrorecibo ,
			tdty3banco =td.tdty3banco ,tdnrocheque =td.tdnrocheque  
			FROM TV00121  INNER JOIN @TV00121 AS td
			ON TV00121.tdnumi     = td.tdnumi   and td.estado=2;

				--ELIMINO LOS REGISTROS
			DELETE FROM TV00121 WHERE tdnumi   in (SELECT td.tdnumi   FROM @TV00121 AS td WHERE td.estado=-1)

			select @tdnumi as newNumi
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@teuact)
		END CATCH
	END

		IF @tipo=3
	BEGIN
		BEGIN TRY 
		select a.tenumi ,a.tefdoc ,a.tety4vend,vendedor .cbdesc  as vendedor,
		a.teobs ,a.tefact ,a.tehact ,a.teuact  ,Sum(detalle .tdmonto) as total 
		from TV0013 as a
		inner join TC002 as vendedor on vendedor .cbnumi  =a.tety4vend 
		inner join TV00121 as detalle on detalle .tdtv13numi =a.tenumi 
		group by a.tenumi ,a.tefdoc ,a.tety4vend,vendedor .cbdesc ,
		a.teobs ,a.tefact ,a.tehact ,a.teuact  
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@teuact)
		END CATCH
	END
		IF @tipo=4
	BEGIN
		BEGIN TRY 
	select  detalle .tdnumi as numidetalle,cliente.ccdesc  as cliente,Concat(Month(a.oafdoc)  ,'-',Year(a.oafdoc )) NroDoc,'0' as factura,
	a.oanumi  as numiCredito,cobranza .tenumi as numiCobranza
	,a.oanumi as tctv1numi ,a.oaccli  as tcty4clie ,detalle.tdfechaPago,
	(credito.ogcred  -(select Isnull(Sum(detalle.tdmonto ),0) from TV00121 as detalle where detalle .tdtv12numi =a.oanumi  ))
	 as pendiente,detalle .tdmonto  as PagoAc,detalle .tdnrorecibo  as NumeroRecibo,
    banco.cedesc  as DescBanco,detalle .tdty3banco as banco, detalle.tdnrocheque,Cast('' as image)as img ,1 as estado 
	from TO001  as a inner join TC004 as cliente
	on cliente .ccnumi  =a.oaccli  
	inner join TC002  as vendedor on vendedor .cbnumi  =a.oarepa  
	inner join TO001A1  as credito on credito.ognumi  =a.oanumi  
	inner join TV00121 as detalle on detalle .tdtv12numi =a.oanumi  
	inner join TV0013 as cobranza on cobranza .tenumi =detalle .tdtv13numi 
	inner join TC0051  as banco on banco .cecon  =25 and banco .cenum  =detalle .tdty3banco 
	and cobranza.tenumi =@tenumi 
	order by a.oanumi  asc

		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@teuact)
		END CATCH
	END
	IF @tipo=5
	BEGIN
		BEGIN TRY 
	select a.tdnumi ,a.tdtv12numi ,a.tdnrodoc ,a.tdfechaPago ,a.tdmonto ,a.tdnrorecibo ,a.tdfact ,a.tdhact
	,a.tduact,Cast('' as image)as img 
	from TV00121 as a
	where a.tdtv12numi =@credito
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@teuact)
		END CATCH
	END

			IF @tipo=6  ---------LISTAR DEUDAS PENDIENTES
	BEGIN
		BEGIN TRY 
	select a.oanumi as  tcnumi,'Principal' as sucursal,Concat(Month(a.oafdoc ) ,'-',Year(a.oafdoc )) NroDoc,
	'0' as factura
	,a.oanumi as  tctv1numi ,a.oaccli as   tcty4clie , cliente.ccdesc   as cliente,a.oarepa as tcty4vend,
	vendedor.cbdesc  as vendedor,a.oafdoc as tcfdoc ,credito.ogcred  as totalfactura,
	(credito.ogcred  -(select Isnull(Sum(detalle.tdmonto ),0) from TV00121 as detalle where detalle .tdtv12numi =a.oanumi ))as pendiente
	,Cast(0 as decimal(18,2)) as PagoAc,'' as NumeroRecibo
	from TO001 as a inner join TC004 as cliente
	on cliente .ccnumi  =a.oaccli  
	inner join TC002  as vendedor on vendedor .cbnumi  =a.oarepa 
	inner join TO001A1  as credito on credito.ognumi  =a.oanumi   
		where (credito .ogcred  -(select Isnull(Sum(detalle.tdmonto ),0) from TV00121 as detalle where detalle .tdtv12numi =a.oanumi ))>0
	order by a.oanumi asc

		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@teuact)
		END CATCH
	END

	
			IF @tipo=7  ---------LISTAR DEUDAS PENDIENTES
	BEGIN
		BEGIN TRY 
	select a.tdnumi ,a.tdtv12numi ,a.tdtv13numi ,a.tdnrodoc ,a.tdfechaPago ,a.tdmonto ,a.tdnrorecibo ,a.tdty3banco ,
	a.tdnrocheque ,a.tdfact ,a.tdhact ,a.tduact,Cast('' as image) as img,1 as estado
	from TV00121 as a where a.tdnumi =@tdnumi 

		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@teuact)
		END CATCH
	END

		IF @tipo=8  ---------LISTAR DEUDAS PENDIENTES
	BEGIN
		BEGIN TRY 
	
	select cobranza.tenumi as numiCobranza,vendedor .cbnumi  AS numiVendedor,vendedor .cbdesc  as vendedor,a.oaccli as numiCliente
	,cliente.ccdesc  as cliente,FORMAT (cobranza .tefdoc, 'dd-MM-yyyy')  as fechaPago
	,Concat(Month(a.oafdoc ) ,'-',Year(a.oafdoc )) NroDocVenta,'Sucursal Principal' as sucursal,detalle .tdmonto  as importe,
	detalle .tdnrorecibo as nroRecibo,
	banco.cedesc  as banco,detalle .tdnrocheque as NroCheque,
	 IIF
                      ((SELECT Sum(auxdetallepago.tdmonto)
                        FROM      TV00121 AS auxdetallepago
                        WHERE   auxdetallepago.tdtv12numi = a.oanumi) = credito.ogcred, IIF
                      ((SELECT Max(ayuda.tdnumi)
                        FROM      TV00121 ayuda
                        WHERE   ayuda.tdtv12numi = a.oanumi) = detalle.tdnumi, 'CANCELACION TOTAL', 'CANCELACION PARCIAL'), 'CANCELACION PARCIAL') AS observacion,
						cobranza .teobs as glosa
	from TO001 as a inner join TC004 as cliente
	on cliente .ccnumi  =a.oaccli  
	inner join TC002 as vendedor on vendedor .cbnumi  =a.oarepa 
	inner join TO001A1  as credito on credito.ognumi  =a.oanumi 
	inner join TV00121 as detalle on detalle .tdtv12numi =a.oanumi 
	inner join TV0013 as cobranza on cobranza .tenumi =detalle .tdtv13numi 
	inner join TC0051 as banco on banco .cecon  =17 and banco .cenum  =detalle .tdty3banco 
	and cobranza.tenumi =@tenumi
	order by a.oanumi asc

		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@teuact)
		END CATCH
	END
	--	IF @tipo=9 ---------LISTAR DEUDAS PENDIENTES POR CLIENTES
	--BEGIN
	--	BEGIN TRY 
	
	--select a.tcnumi,Concat(venta.taidcore ,'-',Year(a.tcfdoc)) NroDoc,
	--IIF(exists(select factura.fvanumi from TFV001 as factura where factura.fvanumi=a.tctv1numi),
	--(select factura.fvanfac from TFV001 as factura where factura.fvanumi=
	--a.tctv1numi),'0') as factura
	--,a.tctv1numi ,a.tcty4clie , cliente.ydrazonsocial  as cliente,a.tcty4vend,
	--vendedor.yddesc as vendedor,a.tcfdoc ,a.tcfvencre,tctotcre as totalfactura,
	--(tctotcre -(select Isnull(Sum(detalle.tdmonto ),0) from TV00121 as detalle where detalle .tdtv12numi =a.tcnumi ))as pendiente
	--,Cast(0 as decimal(18,2)) as PagoAc,cast(0 as bit) as Pagar
	--from TV0012 as a inner join TY004 as cliente
	--on cliente .ydnumi =a.tcty4clie 
	--inner join TY004 as vendedor on vendedor .ydnumi =a.tcty4vend 
	--inner join TV001 as venta on venta.tanumi =a.tctv1numi 
	--inner join TA001 as sucursal on sucursal.aanumi =venta .taalm 
	--	where (tctotcre -(select Isnull(Sum(detalle.tdmonto ),0) from TV00121 as detalle where detalle .tdtv12numi =a.tcnumi ))>0
	--	and cliente.ydnumi =@tenumi
	--order by a.tcnumi asc

	--	END TRY
	--	BEGIN CATCH
	--		INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
	--			   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@teuact)
	--	END CATCH
	--END
	
	IF @tipo=10 --NUEVO REGISTRO
	BEGIN
		BEGIN TRY 

		      set @tenumi=IIF((select COUNT(tenumi) from TV0013)=0,0,(select MAX(tenumi) from TV0013))+1
			  insert into TV0013 values(@tenumi ,@tefdoc,(select top 1 td.tdty3banco from @TV00121 as td) ,@teobs ,@newFecha ,@newHora ,@teuact )
		----INSERTO EL DETALLE
				INSERT INTO TV00121 (tdtv12numi,tdtv13numi  ,tdnrodoc ,tdfechaPago ,tdmonto ,tdnrorecibo,tdty3banco ,
				tdnrocheque ,tdfact ,tdhact ,tduact)

			SELECT td.tdtv12numi ,@tenumi ,td.tdnrodoc ,@newFecha ,td.tdmonto ,td.tdnrorecibo ,1,
			td.tdnrocheque, @newFecha  ,@newHora  ,@teuact 
			FROM @TV00121 AS td where td.estado =0

			select @tdnumi as newNumi
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@teuact)
		END CATCH
	END

	--	IF @tipo=11 ---------LISTAR DEUDAS PENDIENTES POR VENDEDOR
	--BEGIN
	--	BEGIN TRY 
	
	--select a.tcnumi,Concat(venta.taidcore ,'-',Year(a.tcfdoc)) NroDoc,
	--IIF(exists(select factura.fvanumi from TFV001 as factura where factura.fvanumi=a.tctv1numi),
	--(select factura.fvanfac from TFV001 as factura where factura.fvanumi=
	--a.tctv1numi),'0') as factura
	--,a.tctv1numi ,a.tcty4clie , cliente.ydrazonsocial  as cliente,a.tcty4vend,
	--vendedor.yddesc as vendedor,a.tcfdoc ,a.tcfvencre,tctotcre as totalfactura,
	--(tctotcre -(select Isnull(Sum(detalle.tdmonto ),0) from TV00121 as detalle where detalle .tdtv12numi =a.tcnumi ))as pendiente
	--,Cast(0 as decimal(18,2)) as PagoAc,cast(0 as bit) as Pagar
	--from TV0012 as a inner join TY004 as cliente
	--on cliente .ydnumi =a.tcty4clie 
	--inner join TY004 as vendedor on vendedor .ydnumi =a.tcty4vend 
	--inner join TV001 as venta on venta.tanumi =a.tctv1numi 
	--inner join TA001 as sucursal on sucursal.aanumi =venta .taalm 
	--	where (tctotcre -(select Isnull(Sum(detalle.tdmonto ),0) from TV00121 as detalle where detalle .tdtv12numi =a.tcnumi ))>0
	--	and vendedor.ydnumi  =@tenumi and venta.tafdoc =@tefdoc 
	--order by a.tcnumi asc

	--	END TRY
	--	BEGIN CATCH
	--		INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
	--			   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@teuact)
	--	END CATCH
	--END

	IF @tipo=12 --MOSTlaR LIBRERIA
	BEGIN
		BEGIN TRY
		
			SELECT a.cenum   ,a.cedesc   
			from TC0051 as a 
			where a.cecon   =25
			order by  a.cenum  asc
		END TRY
		BEGIN CATCH
		INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@teuact)
		END CATCH

END
	IF @tipo=13 --MOSTRaR Vendedores
	BEGIN
		BEGIN TRY
	select a.cbnumi  ,a.cbdesc  ,a.cbci,a.cbfnac 
		from TC002 as a 
				order by a.cbnumi asc 
		
		END TRY
		BEGIN CATCH
			INSERT INTO TB001 (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@teuact)
		END CATCH

END

End









