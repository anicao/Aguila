INSERT INTO CotizacionesCoberturas VALUES (1178225, 3 , 'Cobertura 100' , 4 , 'Mensual' ,1291 , '11 de $1,291' , 15626)



Select IsNull(E.Subject, '') 
	, IsNull(E.BodyMail, '') 
    , IsNull(E.Firma, '' )  
From Produccion.dbo.Email E
Where E.Proceso = 'Web'

Select IsNull(E.Subject, '')       , IsNull(E.BodyMail, '')      , IsNull(E.Firma, '' )      From Produccion.dbo.Email E        Where E.Proceso = Web

SELECT * FROM [dbo].[CoberturasPortal]
SELECT * FROM [dbo].[CotizacionesPortal]
SELECT * FROM [dbo].[DesglosePagosCotiza]
SELECT * FROM [dbo].[DesReciboCotiza]

SELECT * FROM [dbo].[CotizacionesCoberturas]


SELECT * FROM [dbo].[CotizacionesPortal]
WHERE idCotizacion = '1178225'

exec sp_Sel_CotizacionRep '1178225'

exec sp_Sel_FormasPagoRep '1185666'


Select IsNull(CP.Subject, '') AS Subject       
	 , IsNull(CP.BodyMail, '') AS BodyMail      
	 , IsNull(CP.Firma, '' )  AS Firma     
From Produccion.dbo.CorreosPortal CP        
Where CP.Proceso = 'Web'

Update CorreosPortal
Set BodyMail = 'Envío de la Cotización Número #NoCotizacion# 
Generada por #FirmaUsuario# 
Teléfono #TelFirma# 
Se envío al correo #MailCliente# '


SELECT * FROM [dbo].[CoberturasPortal]
SELECT * FROM [dbo].[CotizacionesPortal]
SELECT * FROM [dbo].[DesglosePagosCotiza]
SELECT * FROM [dbo].[DesReciboCotiza]

SELECT * FROM [dbo].[CotizacionesCoberturas]


SELECT * FROM [dbo].[CotizacionesPortal]
WHERE idCotizacion = '1178225'

exec sp_Sel_CotizacionRep '1178225'

exec sp_Sel_FormasPagoRep '1185666'


Select IsNull(CP.Subject, 'Envío Cotización') AS Subject       
	 , IsNull(CP.BodyMail, '') AS BodyMail      
	 , IsNull(CP.Firma, '' )  AS Firma     
From Produccion.dbo.CorreosPortal CP        
Where CP.Proceso = 'W'
And Activo = 1

Update CorreosPortal
Set BodyMail = 'Envío de la Cotización Número #NoCotizacion# 
Generada por #FirmaUsuario# 
Teléfono #TelFirma# 
Se envío al correo #MailCliente# '

Select * from CorreosPortal

Insert Into CorreosPortal
Values('Cotización #NoCotizacion# del Águila Compañía de Seguros, del seguro de su auto',' Envío de la Cotización Número #NoCotizacion# 
Generada por #FirmaUsuario# 
Teléfono #TelFirma# 
Se envío al correo #MailCliente#', 	'Firma',	'Web',1)



SELECT DATEADD(MONTH, DATEDIFF(MONTH, 0,GETDATE()), 0) AS 'Primer Dia del Mes'

SELECT DATEADD(MM, DATEDIFF(MM,0,GETDATE()), 0) AS 'Primer Dia del Mes'

SELECT CONVERT(VARCHAR(10),DATEADD(dd,-(DAY(GETDATE())-1),GETDATE()),103) AS 'Primer Dia del Mes'



DECLARE @mydate DATETIME
SELECT @mydate = GETDATE()
SELECT CONVERT(VARCHAR(25),DATEADD(dd,-(DAY(@mydate)),@mydate),101) ,
'Último día del mes anterior'
UNION
SELECT CONVERT(VARCHAR(25),DATEADD(dd,-(DAY(@mydate)-1),@mydate),101) AS Date_Value,
'Primer día del mes corriente' AS Date_Type
UNION
SELECT CONVERT(VARCHAR(25),@mydate,101) AS Date_Value, 'Hoy' AS Date_Type
UNION
SELECT CONVERT(VARCHAR(25),DATEADD(dd,-(DAY(DATEADD(mm,1,@mydate))),DATEADD(mm,1,@mydate)),101) ,
'Último día del mes corriente'
UNION
SELECT CONVERT(VARCHAR(25),DATEADD(dd,-(DAY(DATEADD(mm,1,@mydate))-1),DATEADD(mm,1,@mydate)),101) ,
'Primer día del mes siguiente'


	SELECT  CONVERT(VARCHAR,DATEADD(MONTH, DATEDIFF(MONTH, 0,GETDATE()), 0),112)
	SELECT  CONVERT(VARCHAR,EOMONTH(GETDATE(),0),112)
	SELECT CONVERT(VARCHAR,DATEADD(YY, DATEDIFF(YY,0,GETDATE()), 0),112)
	SELECT CONVERT(VARCHAR,YEAR(GETDATE())) + '12' + '31'

	
--COTIZACIONES DEL MES
SELECT        COUNT(nCotizaID) AS CotizacionesMes
FROM            Produccion.dbo.Cotizacion
WHERE        (dFRegistro >= '01/07/2015') AND (dFRegistro <= '31/07/2015') AND (nAgenteID = 1)

--COTIZACIONES DE AÑO
SELECT        COUNT(nCotizaID) AS CotizacionesAño
FROM            Produccion.dbo.Cotizacion
WHERE        (dFRegistro >= '01/01/2015') AND (dFRegistro <= '31/07/2015') AND (nAgenteID = 1)

--PRIMA NETA EMITIDA DEL MES
SELECT ISNULL(SUM(mprimneta),0) AS NuePolPrimNetaMes  FROM produccion.dbo.polizas WHERE dfexpedicion >= '01/07/2015' and dfexpedicion <= '31/07/2015' 
and nrenovacion = 0 and nagenteid = 1


--PRIMA NETA EMITIDA EN EL AÑO
SELECT ISNULL(SUM(mprimneta),0) AS NuePolPrimNetaAño from produccion.dbo.polizas where dfexpedicion >= '01/01/2015' and dfexpedicion <= '31/07/2015' 
and nrenovacion = 0 and nagenteid = 1


--RENOVACIONES DEL MES
SELECT        COUNT(npoliza) AS RenovacionesMes
FROM            Produccion.dbo.renovaciones
where dfinivig >='01/07/2015' and dfinivig <= '31/07/2015' and nagenteid = 1

--RENOVACIONES EN EL AÑO
SELECT        COUNT(npoliza) AS RenovacionesAño
FROM            Produccion.dbo.renovaciones
where dfinivig >='01/01/2015' and dfinivig <= '31/07/2015' and nagenteid = 1

--PRIMA NETA EMITIDA EN EL MES
select ISNULL(sum(mprimneta),0) AS RenPolPrimNetaMes from produccion.dbo.polizas where dfexpedicion >= '01/07/2015' and dfexpedicion <= '31/07/2015' 
and nrenovacion > 0 and nagenteid = 1


--PRIMA NETA EMITIDA EN EL AÑO
select ISNULL(sum(mprimneta),0) as RenPolPrimNetaAño  from produccion.dbo.polizas where dfexpedicion >= '01/01/2015' and dfexpedicion <= '31/07/2015' 
and nrenovacion > 0 and nagenteid = 1


--ULTIMO PAGO DE COMISIONES
select max(dfaplica) from cobranza.dbo.comisionagte where nagenteid =1 and cestatus = 'P'




	EXEC sp_ConsultaInformacionEncabezadoWeb 1


	SELECT			@UltimoPago = CONVERT(VARCHAR,MAX(dfaplica),103)  
	FROM			cobranza.dbo.comisionagte 
	WHERE			nagenteid = @pAgenteID 
	AND				cestatus = 'P'

	SELECT			CONVERT(VARCHAR,MAX(dfaplica),103)  
	FROM			cobranza.dbo.comisionagte 
	WHERE			nagenteid = 1
	AND				cestatus = 'P'


NumCotizacionesMes	NumCotizacionesAño	MontoPolPrimNetaMes	MontoPolPrimNetaAño	NumRenovacionesMes	NumRenovacionesAño	MontoRenPolPrimNetaMes	MontoRenPolPrimNetaAño	UltimoPago
6	50	0	149827	9	69	70027	525456	15/07/2015


SELECT * FROM CorreosPortal

Cotización #NoCotizacion# del Águila Compañía de Seguros, del seguro de su auto



Select * from DesglosePagosCotiza
Where cNoTarjeta is not null
and Year(dfVigTarjeta) > 2015
order by dfVigTarjeta


--nPoliza noCotizacion
--nEndoso 


4555-0063-0035-7132    

Select * from DesglosePagosCotiza
Where nPoliza = 1182695

Select top 10 * From dbo.Cotizacion Where nAgenteID = 1 Order by nCotizaID DESc

 declare @pTextSearch		VarChar(100) = ''
		, @PageNo		    int
		, @RecordsPerPage   int

 Select   @pTextSearch = '1175'
		, @PageNo		    = 1
		, @RecordsPerPage   = 10

		Select C.[nCotizaID]
				, C.[cNombreA]											As Cliente
				, Convert(Char(10), C.[dFCotiza], 103)					As InicioVigencia
				, RTRIM(IsNUll(T.[cTel], ''))							As Telefono
				, IsNUll(O.[cObserv],'')								As Observaciones
				, Convert(Char(10), C.[dFRegistro], 103)				As FechaRegistro
				, IsNull(Convert(Char(10), P.[dfExpedicion], 103), '')	As FechaExpedicion
				, C.[cAPaternoC]
				, C.[cAMaternoC]
				, C.[cNombresC]
				, C.[cTituloC]
				, C.[nSolicitudId]
				, C.[cEstatus]
				, E.[cDescrip]								As DescripcionEstatus
				, Convert(Char(10), P.[dfExpedicion], 103)	As FechaExpedicion
			From [Produccion].[dbo].[Cotizacion] C Left Outer Join [Produccion].[dbo].[catTelefonos]  T ON C.[nCotizaID]	= T.[nCotizaID]
													Left Outer Join [Produccion].[dbo].[Complement]	  O On C.[nCotizaID]	= O.[nCotizaID]
													Left Outer Join [Produccion].[dbo].[CatEstatus]	  E On C.[cEstatus]		= E.[cEstatus]
													Left Outer Join [Produccion].[dbo].[Polizas]		  P On C.[nSolicitudId] = P.[nSolicitudId]
				Where C.nAgenteId = 1
				And ((Convert(VarChar(15), C.nCotizaID) + C.cNombreA) Like '%' + @pTextSearch +'%')
					Order By C.nCotizaID Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY



Select * from catagentes where nagenteid=1
Select * from catagentes where cpasww is not null
update catagentes set cpasww = '1', email = 'angie3611@gmail.com,moni_ck@prodigy.net.mx' where nagenteid=1
Select * from catformapago

select TOP 160 * from vehiculos ORDER BY 1 DESC


select * from cotizacion where ncotizaid in (1196889)
select * from vehiculos where ncotizaid in (1196832)
select * from conducts where ncotizaid in (1196889)
select * from dirscotizacion where ncotizaid in (1196889)
select * from cattelefonos where ncotizaid in (1196889)
select * from complement where ncotizaid in (1196889)
select * from desglosepagoscotiza where npoliza in (1196889)
select * from desglosepagoscotiza where npoliza in (1196889)
--1178129



select top 10 * From [Produccion].[dbo].[Renovaciones] where nagenteid=1 order by 1 desc

Select * from login.dbo.catusuarios where cusuario = 'ezambrano'

--update login.dbo.catusuarios set cpassword = 'Z12345678' where cusuario = 'ezambrano'



 select count(nendoso) from endosos
where dfexpedicion >= '01/01/2015' and dfexpedicion <= '31/12/2015' 
and nagenteid = 1 

select sum(mprimneta) from endosos 
where dfexpedicion >= '01/01/2015' and dfexpedicion <= '31/12/2015' 
and nagenteid = 1 


--[dbo].[sp_ConsultaInformacionEncabezadoWeb] 1


declare @lTotalCount	 int = 0
 EXECUTE [dbo].[sp_RenovacionesAgentePaginadoPeriodoMes]
  @pAgenteID		= 1
, @pPeriodoID		= '2015'
, @pMesPeriodoID	= '07'
, @PageNo		    = 2
, @RecordsPerPage   = 10
, @TotalCount		= @lTotalCount
Select @lTotalCount


select * from [dbo].[CotizacionesCoberturas] 
execute sp_Sel_TipoPago

execute Sp_Sel_Pago

execute sp_Sel_FormaPago

execute Sp_Sel_Estados

sp_helptext Sp_Sel_Pago
Select * From dbo.CatPago
execute Sp_Sel_Territorio


SELECT * FROM CoberturasPortal Where Cotizador = 'Clientes'
SELECT * FROM CoberturasPortal Order By 1, 2

select * from cotizacion where ncotizaid in (1196895)
select * from vehiculos where ncotizaid in (1196895)
select * from conducts where ncotizaid in (1196895)
select * from dirscotizacion where ncotizaid in (1196895)
select * from cattelefonos where ncotizaid in (1196895)
select * from complement where ncotizaid in (1196895)
select * from desglosepagoscotiza where npoliza in (1196895)

execute sp_sel_formaspagorep 1196895

SELECT
	CC.nCotizaID,
	CC.nOrdenCobertura,  
    CC.Cobertura,
	CC.nFormaPago,  
    CC.desFormaPago,  
    CC.PagoIni,  
    CC.PagoSub,  
    CC.Total
FROM  
    Produccion.dbo.CotizacionesCoberturas CC
		Where nCotizaID = 1196895
		order by 1 desc


SELECT
	CC.nCotizaID,
	CC.nOrdenCobertura,  
    CC.Cobertura,
	CC.nFormaPago,  
    CC.desFormaPago,  
    CC.PagoIni,  
    CC.PagoSub,  
    CC.Total
FROM  
    Produccion.dbo.CotizacionesCoberturas CC
		Where nCotizaID = 1196895
		order by 1 desc


Select top 10 * from cobranza.pagos