-- Consulta general de factura
Select  f.*
	  , p.nAgenteID
	  , z.nAgenteID
	From dbtools.dbo.FacturaElectronica f
				Left Outer Join Produccion.dbo.Polizas	p on f.nPoliza = p.nPoliza 
														 and f.nRecibo <> 0
				Left Outer Join Cobranza.dbo.Pagos		s on s.nControlID = f.id_Factura 
														 and f.Tabla = 'NC'
				Left Outer Join Produccion.dbo.Polizas  z on z.nPoliza = s.nPoliza
		Where Fecha_Emision >= '01/03/2015'


-- Key de factura
Select *  From dbtools.dbo.catKeyFactura





Declare @lRFC		VarChar(20)
	  , @lKeyWs		nVarChar(200)
	  , @lLinkWs	nVarChar(200)


-- Clave de conexión al WS
Select Top 1 @lKeyWs = cKey From dbtools.dbo.catKeyFactura

-- RFC Empresa
Select @lRFC = 'ASE941124NN4'

-- Link del Web Service
Select @lLinkWs = 'http://192.168.11.248/xsamanager/downloadCfdWebView?'


-- http://192.168.11.248/xsamanager/downloadCfdWebView?serie=MEXFP&folio=1010444&tipo=PDF&rfc=ASE941124NN4&key=9b333d4207d7de7dae4f87b6627d821f


-- Facturas por agente, fecha de emisión de la póliza
Select  p.nPoliza
      , p.dfExpedicion
	  , p.nAgenteID
	  , p.nClienteID
	  , p.cNombreA
	  , Serie
	  , Folio
	  , Fecha_Emision
	  , @lLinkWs + 'serie=' + Convert(VarChar(20), Serie) + '&folio=' + Convert(VarChar(20), Folio) + '&tipo=PDF&rfc=' + @lRFC + '&key=' + @lKeyWs As LnkPDF
	  , @lLinkWs + 'serie=' + Convert(VarChar(20), Serie) + '&folio=' + Convert(VarChar(20), Folio) + '&tipo=XML&rfc=' + @lRFC + '&key=' + @lKeyWs As LnkXML
	From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
		Where dfExpedicion >= '01/03/2015' and dfExpedicion <= '31/03/2015'
	and nagenteid =   1


Select Top 20 *
	From Produccion.dbo.Polizas

Select *
	From Produccion.dbo.Polizas
		Where cnombrea like 'arevalo%'

Select Top 20 *
	From Produccion.[dbo].[catClientes]

Select Top 20 *
	From dbtools.dbo.FacturaElectronica

Select *
	From Produccion.[dbo].[catClientes]
		Where cRazSoc like 'plaf%'


Select *
	From Produccion.[dbo].[catClientes]
		Where nClienteID = 110194

		http://192.168.11.248/xsamanager/downloadCfdWebView?serie=MEXFP&folio=934429&tipo=PDF&rfc=ASE941124NN4&key=9b333d4207d7de7dae4f87b6627d821f

		ReportViewer.aspx?REPORT=C100&AGENTE=1&P=11100273653

Declare @lTotalCount int
-- Ojo, preguntar a david porque este stored procedure esta
-- cableado solo para 2015
EXECUTE [dbo].[sp_ConsultaFacturasPaginado]
	  @pFechaIni		= ''
	, @pFechaFin		= ''
	, @pAgenteID		= 1
	, @pPolizaID		= 0
	, @pClientID		= 0
	, @PageNo		    = 1
	, @RecordsPerPage   = 10
	, @TotalCount		= @lTotalCount Output

Select @lTotalCount


ReportViewer.aspx?REPORT=" + rptN + "&INICIO=" + fIni + "&FIN=" + fFin + "&AGENTE=" + agen + "&L=2


http://192.168.11.248/xsamanager/downloadCfdWebView?serie=MEXFP&folio=942666&tipo=PDF&rfc=ASE941124NN4&key=9b333d4207d7de7dae4f87b6627d821f
http://192.168.11.248/xsamanager/downloadCfdWebView?serie=MEXFP&folio=942666&tipo=XML&rfc=ASE941124NN4&key=9b333d4207d7de7dae4f87b6627d821f
ReportViewer.aspx?REPORT=C100&P=11100273653
ReportViewer.aspx?REPORT=PSOB&P=11100273653
ReportViewer.aspx?REPORT=PSOB&P=11300264858

Select *
	From Produccion.dbo.PolVehiculos
		Where npoliza = 11123004040


Declare @lTotalCount int
EXECUTE [dbo].[sp_CotizacionesAgentePaginado]
	  @pAgenteID		= 1
	, @PageNo		    = 1
	, @RecordsPerPage   = 10
	, @TotalCount		= @lTotalCount Output

Select @lTotalCount


Select top 1 *
			From [Produccion].[dbo].[Cotizacion]

Select Top 10 *
	From [Produccion].[dbo].[Cotizacion] C Inner Join [Produccion].[dbo].[catTelefonos]  T ON C.[nCotizaID] = T.[nCotizaID]
                Where C.dfRegistro like '%2015%'


execute [dbo].[sp_Sel_AñosCotiza]

execute sp_Sel_SubMarcas 1

SELECT noficinaID, * FROM catAgentes where nAgenteId = 255

SELECT top 10 * FROM catAgentes where noficinaid=5
select * from catoficinas --where noficinaid=5
execute sp_Sel_Campañas

SELECT distinct noficinaID
	FROM catAgentes


Select top 10 *
			From [Produccion].[dbo].[Cotizacion]
				Where nagenteID = 0
			order by 1 desc

Select * from cotizacion where ncotizaid in (1170178, 1169978, 1170274)
Select * from vehiculos where ncotizaid in (1170178, 1169978)

select * from catmarcas
select * from catmodelos

Select * from cotizacion where capaternoc = 'prueba'

Select top 10 * from cotizacion order by ncotizaid desc

begin tran
rollback



INSERT INTO [dbo].[Cotizacion]([cAPaternoC],[cAMaternoC],[cNombresC],[cTituloC],[cSexoC],[cNombreA],[dFCotiza],[nNumCond],[nNumVehi],[cCondRest],[nMeses],[nFormaPago],[cAsistVje],[cTpoPago],[cRFC],[nAgenteID],[nCampID],[dFRegistro],[cRecomienda],[cResponsa],[cEMail],[cEstatus],[nClienteID],[cPeriodo],[cTipo],[cMasAutos],[cNumTarjeta],[dFFinVigTarj],[cUsuario],[cObserv],[nCveObs],[cCuentaCheques],[cCveBanco],[cNomTitular],[nOficinaID],[cRespAnt],[dfRespAnt],[nControl],[cCober100],[cCodigoNegro],[cEmailOtro],[cEntPol],[cEntComen],[cEstatusAnt],[cCIE],[dfVigChequera],[cFormaPago],[cEra],[cTersa],[nSolicitudId],[nSolicitudId_Ant],[nComCedida],[mComCedida],[mComRestante],[ncontrolnuevo],[dfContraEntrega],[dfAutProcar],[cFinanciamiento],[cNombreFinanciamiento],[cEndosoCancela],[nControlMaximo],[cAP],[nReferenciaWeb],[cultimousuario])VALUES('prueba','web','nueva','sr','F','PRUEBA WEB NUEVA','03/06/2015',1,1,'S',12,0, null ,'1','',0,388,'03/06/2015 10:02', '' ,'','','P',0,'062015','','N','NET', null ,'0','',0,'','','','1', null , null ,0,'S','','','N','-03/06/2015-0','', null , null ,'CH','N','S',null,null,0,0,0,0,null,null,'N','','',null,'N',null,'0')

Select @@IDENTITY


INSERT INTO [dbo].[Conducts]([nCotizaID],[nContCond],[cNombre],[dFNacimien],[cSexo],[cEdoCivil],[cExtencRC])VALUES(1169977,1,'PRUEBA WEB NUEVA','27/01/2010','M','S','N')


INSERT INTO [dbo].[Vehiculos]([nCotizaID],[nContVehi],[cCP],[nSubRamo],[nModeloID],[nAño],[cGaragCasa],[cGaragTrab],[cUsoTrab],[nDedudDM],[nDedudRT],[mCobertGM],[mCobertRC],[cDefAseLeg],[cSumAsegF],[nCondAsig],[mSumAEqEsp],[mValorAuto],[cTpoEspAut],[nPuertas],[nCilindros],[nTransmi],[cNumSerie],[cPlacas],[cTpoAlarma],[cSUVA],[cVehSus],[cApoyo100],[cCober100],[cDesEqEsp],[cNCI],[cEBC],[nPagina],[cTersa],[mCobertRCCat],[cValidaSerie],[cAutorizaSerie],[cAsistVje],[cVIN],[cExcenDedu])VALUES(1169977,1,'2','R',1478,2013,'S','S','N',5,10,100000,500000,'S','S',1,0,125000,'auto nuevo',0,0,0,'','','A','S','N','N','N','','','',null,'S',0,'S','','0','','N')


INSERT INTO [dbo].[catTelefonos]([nCotizaID],[nPoliza],[nReprocesoID],[cLada],[cTel],[cOficina],[cExtension],[cCelular],[cFax],[cRadioID],[cRadio],[cOtro1],[cTpo1],[cHistorial],[cUsuario]) VALUES(1169977,null,null,'','234234','','','','','','','','','','0')

INSERT INTO [dbo].[Complement]([nCotizaID],[dFProxLlam],[cCompañia],[cInteres],[cGusto],[cTelefono],[cObserv],[precio_aguila],[precio_comp],[cObservGrales],[cObservProcar],[mPrimNeta])VALUES( 1169977 ,'02/08/2015','0','XX','OTR','','','0','0','','','0')



Select * from dbo.catCiasAnt 0

------------------------------------------

INSERT INTO [dbo].[Vehiculos]([nCotizaID],[nContVehi],[cCP],[nSubRamo],[nModeloID],[nAño],[cGaragCasa],[cGaragTrab],[cUsoTrab],[nDedudDM],[nDedudRT],[mCobertGM],[mCobertRC],[cDefAseLeg],[cSumAsegF],[nCondAsig],[mSumAEqEsp],[mValorAuto],[cTpoEspAut],[nPuertas],[nCilindros],[nTransmi],[cNumSerie],[cPlacas],[cTpoAlarma],[cSUVA],[cVehSus],[cApoyo100],[cCober100],[cDesEqEsp],[cNCI],[cEBC],[nPagina],[cTersa],[mCobertRCCat],[cValidaSerie],[cAutorizaSerie],[cAsistVje],[cVIN],[cExcenDedu])VALUES(@IDCotizacion,1,'2','R',1478,2002,'S','S','N',5,10,100000,500000,'S','N',1,0,94000,'ESCALADE 4X2 AUT',0,0,0,'','','A','N','N','N','N','','','',null,'S',0,'N','','R','','N')

INSERT INTO [dbo].[catTelefonos]([nCotizaID],[nPoliza],[nReprocesoID],[cLada],[cTel],[cOficina],[cExtension],[cCelular],[cFax],[cRadioID],[cRadio],[cOtro1],[cTpo1],[cHistorial],[cUsuario]) VALUES(@IDCotizacion,null,null,'','','','','','','','','','','','mromero')

INSERT INTO [dbo].[Complement]([nCotizaID],[dFProxLlam],[cCompañia],[cInteres],[cGusto],[cTelefono],[cObserv],[precio_aguila],[precio_comp],[cObservGrales],[cObservProcar],[mPrimNeta])VALUES( @IDCotizacion ,null,'0','COB','AHO','1','DAS','35644','0','','','30128')



sELECT * FROM CATCLIENTES
sELECT * FROM CATpromocion

/* para saber los datos del responsable de la cotización */
SELECT * FROM catresposa C inner join login.dbo.catusuarios U on c.cUsuarioSistema = U.cUsuario


Select C.dfRegistro, C.cEmail, C.cResponsa, R.cNomResp, R.cTelefono, IsNull(U.cPuesto, '') As cPuesto, IsNull(U.cExtension, '') As cExtension, IsNull(U.cCorreo, '') As cCorreo
	From Produccion.dbo.Cotizacion C Left Join Produccion.dbo.CatResposa R On C.cResponsa		= R.cResponsa
									 Left Join Login.dbo.CatUsuarios	 U On R.cUsuarioSistema = U.cUsuario
		Where C.nCotizaID = 1178225



Select * From Produccion.dbo.Cotizacion Where nCotizaID = 1178223
update Produccion.dbo.Cotizacion Set cEmail = 'ezambrano@elaguila.com.mx', cResponsa = 'EPL' Where nCotizaID = 1178225


Select * From catEBC order by nguia

Select top 10 * from vehiculos  where cEBC != '' order by ncotizaid desc

execute sp_Sel_ModAño '062015'


Select * From [Login].[dbo].[catUsuarios] Where cUsuario = 'ezambrano'



execute Sp_Sel_Territorio


Select top 10 * 
	From [Login].[dbo].[catMenus]
		Order BY 1 Desc

Select * 
	From [Login].[dbo].[catModulos]
		Order BY 1 Desc
		
Select top 10 * 
	From [Login].[dbo].[catUsuarios]
		Order BY 1 Desc
		
Select top 10 * 
	From [Produccion].[dbo].[Vehiculos]
		Order By 1 Desc


--------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------
Begin

Select top 10 * 
	From [Produccion].[dbo].[Cotizacion]
		Order By 1 Desc


Select top 10 * 
	From [Produccion].[dbo].[Conducts]
		Order By 1 Desc


Begin Tran

INSERT INTO [dbo].[Cotizacion]([cAPaternoC],[cAMaternoC],[cNombresC],[cTituloC],[cSexoC],[cNombreA],[dFCotiza],[nNumCond],[nNumVehi],[cCondRest],[nMeses],[nFormaPago],[cAsistVje],[cTpoPago],[cRFC],[nAgenteID],[nCampID],[dFRegistro],[cRecomienda],[cResponsa],[cEMail],[cEstatus],[nClienteID],[cPeriodo],[cTipo],[cMasAutos],[cNumTarjeta],[dFFinVigTarj],[cUsuario],[cObserv],[nCveObs],[cCuentaCheques],[cCveBanco],[cNomTitular],[nOficinaID],[cRespAnt],[dfRespAnt],[nControl],[cCober100],[cCodigoNegro],[cEmailOtro],[cEntPol],[cEntComen],[cEstatusAnt],[cCIE],[dfVigChequera],[cFormaPago],[cEra],[cTersa],[nSolicitudId],[nSolicitudId_Ant],[nComCedida],[mComCedida],[mComRestante],[ncontrolnuevo],[dfContraEntrega],[dfAutProcar],[cFinanciamiento],[cNombreFinanciamiento],[cEndosoCancela],[nControlMaximo],[cAP],[nReferenciaWeb],[cultimousuario])VALUES('aaaaaa','bbbbbb','qqqqqq','lic','M','AAAAAA BBBBBB QQQQQQ','13/07/2015',1,1,'N',12,1, null ,'C','',0,388,'13/07/2015 12:08', '' ,'XX','','P',1,'072015','','N','NET', null ,'0','',0,'','','CLIENTE INDIVIDUAL','1', null , null ,0,'N','','','N','-13/07/2015-0','', null , null ,'CH','S','S',null,null,0,0,0,12,null,null,'N','','',null,'N',null,'0')

Select @@IDENTITY

INSERT INTO [dbo].[Conducts]([nCotizaID],[nContCond],[cNombre],[dFNacimien],[cSexo],[cEdoCivil],[cExtencRC])VALUES(1178221,1,'AAAAAA BBBBBB QQQQQQ','13/06/1982','M','S','N')

INSERT INTO [dbo].[Vehiculos]([nCotizaID],[nContVehi],[cCP],[nSubRamo],[nModeloID],[nAño],[cGaragCasa],[cGaragTrab],[cUsoTrab],[nDedudDM],[nDedudRT],[mCobertGM],[mCobertRC],[cDefAseLeg],[cSumAsegF],[nCondAsig],[mSumAEqEsp],[mValorAuto],[cTpoEspAut],[nPuertas],[nCilindros],[nTransmi],[cNumSerie],[cPlacas],[cTpoAlarma],[cSUVA],[cVehSus],[cApoyo100],[cCober100],[cDesEqEsp],[cNCI],[cEBC],[nPagina],[cTersa],[mCobertRCCat],[cValidaSerie],[cAutorizaSerie],[cAsistVje],[cVIN],[cExcenDedu])VALUES(1178221,1,'03800','R',3296,2014,'S','S','N',5,10,100000,500000,'S','N',1,0,255000,'ACCORD EXL L4 CVT 2.4L NAVY NUEVO',5,4,0,'','AAA-BB1','A','S','N','N','N','','','',null,'S',0,'N','','R','','N')



INSERT INTO [dbo].[catTelefonos]([nCotizaID],[nPoliza],[nReprocesoID],[cLada],[cTel],[cOficina],[cExtension],[cCelular],[cFax],[cRadioID],[cRadio],[cOtro1],[cTpo1],[cHistorial],[cUsuario]) VALUES(1178221,null,null,'','2222','','','','','','','','','','0')


INSERT INTO [dbo].[Complement]([nCotizaID],[dFProxLlam],[cCompañia],[cInteres],[cGusto],[cTelefono],[cObserv],[precio_aguila],[precio_comp],[cObservGrales],[cObservProcar],[mPrimNeta])VALUES( 1178221 ,'11/09/2015','0',' ','OTR','','','14310','0','','','10968')


--Rollback tran


Begin Tran

UPDATE [dbo].[Cotizacion]  
 SET [cAPaternoC] = 'aaaaaa' 
	,[cAMaternoC] = 'kkkkkk'
	,[cNombresC] = 'hhhhhh' 
	,[cTituloC] = 'ing' 
	,[cSexoC] = 'F' 
	,[cNombreA] = 'OOOOOO KKKKKK HHHHHH'
	,[dFCotiza] = '13/07/2015'
	,[nNumCond] = 1
	,[nNumVehi] = 1
	,[cCondRest] = 'N'
	,[nMeses] = 12
	,[nFormaPago] = 1
	,[cTpoPago] = 'C'
	,[cRFC] = ''
	,[nAgenteID] = 0 
	,[nCampID] = 388
	,[cRecomienda] = ''
	,[cResponsa] = 'XX' 
	,[cEMail] = ''
	,[cEstatus] = 'P'
	,[nClienteID] = '1'
	,[cPeriodo] = '072015'
	,[cTipo] = 'N'
	,[cMasAutos] = 'N'
	,[cNumTarjeta]='Actualizo NET'
	,[cUsuario] = '0'
	,[cObserv] = '' 
	,[nCveObs] = 0
	,[cCuentaCheques] = null
	,[cCveBanco] = null
	,[cNomTitular] = null
	,[nOficinaID] = 1
	,[nControl] = 0
	,[cCober100] = 'N'
	,[cCodigoNegro] = ''
	,[cEmailOtro] = ''
	,[dfVigChequera] = null
	,[cFormaPago] = 'CH'
	,[cEra] = 'S'
	,[cTersa] = 'S'
	--,[nSolicitudId] = ''
	,[nComCedida] = 0
	,[mComCedida] = 0
	,[mComRestante] = 0
	,[ncontrolnuevo] = 0 
	,[dfAutProcar] =  null
	,[cFinanciamiento] = 'N'
	,[cNombreFinanciamiento] = '' 
	,[cEndosoCancela] = '' 
	,[cAP] = 'N' 
	WHERE nCotizaid=1178224

Rollback tran


execute [dbo].[sp_Sel_CotizacionRep] 1178225


Select * From polizas where nconsecutivo=256358
select * From [Produccion].[dbo].[Renovaciones] where nconsecutivo=256358

Select * From polizas where npoliza=51100259183
select * From [Produccion].[dbo].[Renovaciones] where npoliza=51100259183

select * from complement order by 1 DEsc
select * from catestatus order by 1 DEsc
select * from cotizacion order by 1 DEsc
select top 10 * From [Produccion].[dbo].[Renovaciones]

select * from [dbo].[catEstatusEmisionVtas]
select * from [dbo].[catEstatusEnvioSol]
select * from [dbo].[catEstatusPol]
select * from [dbo].[catEstatusRen]

--select * from [dbo].[catEstSeg]

select * from [dbo].[catEstSegRep]

select * from [Seguimiento].[dbo].[catEstatusRec]
select * from [Seguimiento].[dbo].[catEstatusSeg]

select top 10 * From [dbo].[Seguimiento]


Select * From [dbo].[Cotizacion] Where nCotizaID = 310608
Select * From [dbo].[Vehiculos] Where nCotizaID = 310608

execute [dbo].[sp_CotizacionesAgentePaginadoPeriodoMes]
  @pAgenteID		= 1
, @pPeriodoID		= '2015'
, @pMesPeriodoID	= '07'
, @PageNo		    = 1
, @RecordsPerPage   = 100
, @TotalCount		= 0



execute [dbo].[sp_RenovacionesAgentePaginadoPeriodoMes]
  @pAgenteID		= 0
, @pPeriodoID		= '2015'
, @pMesPeriodoID	= '07'
, @PageNo		    = 1
, @RecordsPerPage   = 25
, @TotalCount		= 0


select IsNUll(Convert(Char(10), null, 103), '')	As FechaExpedicion


--------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------

Select * From [dbo].[Cotizacion]
	Where PATINDEX(nCotizaID, '310') > 0
	
Select * From [dbo].[Cotizacion]
	Where PATINDEX(nCotizaID, '310608') > 0

Select * From [dbo].[Cotizacion]
		Where ((Convert(VarChar(15), nCotizaID) + cNombreA) Like '%OSO%')
	Where nCotizaID like '%310%'


Select @TotalCount = Count(*)
	From [Produccion].[dbo].[Cotizacion]
		Where Year(dfRegistro) = @pPeriodoID
		And Month(dfRegistro) = @pMesPeriodoID
		And (nCotizaID + cNombreA) Like '%' + @pTextSearch +'%')

	nCotizaID = 310608

Select * From [dbo].[Cotizacion] Where nCotizaID = 296749
Select * From [dbo].[Vehiculos] Where nCotizaID = 296749


Select * From [dbo].[Cotizacion] Where nCotizaID = 1175492
Select * From [dbo].[Cotizacion] Where nCotizaID = 296754


Select * From [dbo].[Vehiculos] Where nCotizaID = 1175492
Select * From [dbo].[Vehiculos] Where nCotizaID = 296754

select * from catsubramo
