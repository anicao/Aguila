USE [Produccion]
GO
/****** Object:  StoredProcedure [dbo].[sp_SelConsultaPoliza]    Script Date: 06/03/2015 09:31:02 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_SelConsultaPoliza]
@nPoliza numeric(18, 0) = 0, 
@PolAP  VARCHAR(30) = ''

AS
SET NOCOUNT ON

DECLARE
@CONSECUTIVO NUMERIC(18),
@MAXENDOSO INT

--DATOS DE POLIZA*******************************************************************************************************************************
DECLARE
@POLIZAS TABLE(
	nPoliza						numeric(12, 0),
	nOficinaID					int,
	nRamo						numeric,
	nSubramo					numeric(1, 0),
	nRenovacion					numeric(2, 0),
	nConsecutivo				numeric(6, 0),
	nClienteID					numeric(8, 0),
	cNombreA					char(80),
	cRFC						char(13),
	nSolicitudID				numeric(10, 0),
	dFIniVig					smalldatetime,
	dFFinVig					smalldatetime,
	nUtlrec						int,
	dFSigrec					smalldatetime,
	nFormaPago					int,
	nAgenteID					int,
	nEstatusID					int,
	dFEstatus					smalldatetime,
	dFExpedicion				smalldatetime,
	mPrimNeta					money,
	mDchosPol					money,
	mRecPF						money,
	nIVA						numeric(2, 0),
	mAP							money,
	nComAgente					money,
	nNumVehi					int,
	nNumCond					int,
	dFUltPago					smalldatetime,
	dFSigPago					smalldatetime,
	cCondRest					char(1),
	nCampID						int,
	cAsistVje					char(1),
	cTpoPago					char(2),
	cPeriodo					char(6),
	nControl					numeric(18, 2),
	cObserv						varchar(800),
	cAPaternoC					char(25),
	cAMaternoC					char(25),
	cNombreC					char(25),
	cTituloC					char(8),
	cSexoC						char(1),
	cRecomienda					char(15),
	nReferidos					numeric(2, 0),
	nRefUsados					numeric(2, 0),
	cUsuario					char(10),
	cImpresa					char(1),
	cEMail						varchar(100),
	cSeguim						char(1),
	cResponsa					char(3),
	nNumUsrID					numeric(6, 0),
	mMontoCupon					money,
	cCuponImpreso				char(1),
	cMasAutos					char(1),
	cNumTarjeta					char(20),
	dFFinVigTarj				smalldatetime,
	cObstarj					varchar(150),
	nCveObs						int,
	nCuponAsig					numeric(18, 0),
	nSiniestro					numeric(18, 0),
	cCuentaCheques				char(20),
	cCveBanco					char(3),
	cNomTitular					varchar(60),
	cCober100					char(1),
	cCodigoNegro				varchar(5),
	cEmailOtro					varchar(100),
	cEntPol						char(1),
	cEntComen					varchar(80),
	dfVigChequera				datetime,
	cFormaPago					varchar(2),
	cEra						char(1),
	nRenovacionERA				numeric(18, 0),
	dFFinVigERA					smalldatetime,
	cTersa						char(1),
	nComCedida					numeric(18, 0),
	mComCedida					money,
	mComRestante				money,
	nControlNuevo				numeric(18, 0),
	dfContraEntrega				datetime,
	dfAutProcar					datetime,
	cFinanciamiento				char(1),
	cNombreFinanciamiento		varchar(200),
	cEndosoCancela				char(1),
	nControlMaximo				int,
	nPolAP						varchar(50),
	nReferenciaWeb				numeric(18, 0),
	nPolTurista					varchar(50),
	nFolioNationalUnity			varchar(800)
 )

 --DATOS DE PRIMAS DE POLIZA **********************************************************************************************************
 DECLARE
 @PRIMPOLIZAS TABLE (
    nPoliza numeric(12, 0), 
	nEndoso numeric(3, 0),
	mPrimRCBI money,
	mPrimProliber money,
	mPrimDM money,
	mPrimRT money,
	mPrimEE money,
	mPrimAV money,
	mPrimGM money,
	mPrimRCPD money,
	mPrimExtRC money,
	mPrimVehSus money,
	mPrimSegObli money
	)

--DATOS DE DIRECCIONES DE POLIZA*******************************************************************************
DECLARE
@DIRSPOLIZA TABLE(
    nPoliza numeric(12, 0), 
	nEndoso numeric(12, 0), 
	nTipoDirID int,
	cDireccion varchar(150),
	nExterior varchar(35),
	nInterior varchar(35),
	cColonia varchar(60),
	cPoblacion varchar(40),
	nEstadoID int,
	nCP char(5),
	cTel varchar(40),
	cOficina varchar(20),
	cCelular varchar(20),
	cFax varchar(40),
	cOtro varchar(20),
	cRequerida char(1),
	cFiscal char(1)
	)

--DATOS DE CONDUCTORES DE POLIZA *******************************************************************************************
DECLARE
@POLCONDU TABLE(	
	nPoliza numeric(12, 0), 
	nEndoso numeric(12, 0), 
	nContCond int,
	cNombre char(40),
	dFNacimien smalldatetime,
	cSexo char(1),
	cEdoCivil char(1),
	cExtencRC char(1),
	mPrimExtRCCONDU money,
	cEstatus char(1)
	)    

--DATOS DE VEHICULOS DE POLIZA******************************************************************************************************
DECLARE
@POLVEHICULOS TABLE(	
	nPoliza numeric(12, 0), 
	nEndoso numeric(12, 0), 
	nContVehi int,
	cCP char(5),
	nSubRamo char(2),
	nModeloID int,
	nAño int,
	cGaragCasa char(1),
	cGaragTrab char(1),
	cUsoTrab char(1),
	nDedudDM int,
	nDedudRT int,
	mCobertGM money,
	mCobertRC money,
	cDefAseLeg char(1),
	cSumAsegF char(1),
	nCondAsig int,
	mSumAEqEsp money,
	mValorAuto money,
	cTpoEspAut varchar(250),
	nPuertas int,
	nCilindros int,
	nTransmi int,
	cNumSerie varchar(30),
	cPlacas varchar(20),
	cTpoAlarma char(2),
	cSUVA char(1),
	dFIniVig smalldatetime,
	dFFinVig smalldatetime,
	cEstatus char(1),
	cVehSus char(1),
	cDescEqEsp varchar(400),
	cApoyo100 char(1),
	cCober100 char(1),
	cInciso int,
	cNCI varchar(8),
	cEBC varchar(50),
	nPagina numeric(18, 0),
	cTersa char(1),
	mCobertRCCat money,
	cValidaSerie char(1),
	cAutorizaSerie char(30),
	cAsistVje char(10),
	cVin varchar(50),
	cExcenDedu char(1)
	)

--DATOS DE PRIMAS DE VEHICULO DE POLIZA *******************************************************************************
DECLARE
@PRIMPOLVEHI TABLE(	
	nPoliza numeric(12, 0), 
	nEndoso numeric(3, 0),
	nContVehi int,
	mPrimRCBI money,
	mPrimProliber money,
	mPrimDM money,
	mPrimRT money,
	mPrimEE money,
	mPrimAV money,
	mPrimGM money,
	mPrimRCPD money,
	mPrimExtRC money,
    mPrimVehSus money,
	mPrimSegObli money
	)

--DATOS DE BENEFICIARIOS DE POLIZA*******************************************************************************
DECLARE
@POLBENEFICIARIOS TABLE(
    nPoliza numeric(12, 0), 
	nEndoso numeric(12, 0), 
	nBenCont int,
	cNombre varchar(100),
	cApellidoP varchar(200),
	cApellidoM varchar(200),
	ParentescoID int,
	nPorcentaje int,
	cDomicilio varchar(400),
	dFechaNac smalldatetime,
	Descripcion varchar(50)
	)

--DATOS GENERALES DE TELEFONOS****************************************************************************	
DECLARE
@CATTELEFONOS TABLE(
	nConsecutivoid int,
	nCotizaID decimal(18, 0),	
	nPoliza numeric(12, 0),
	nReprocesoID decimal(18, 0),
	cLada varchar(5),
	cTel varchar(20),
	cOficina varchar(20),
	cExtension varchar(20),
	cCelular varchar(20),
	cFax varchar(20),
	cRadioID varchar(20),
	cRadio varchar(20),
	cOtro1 varchar(20),
	cTpo1 char(1),
	cHistorial varchar(4000),
	cUsuario varchar(10)
)

--COMENTARIOS VITALICIOS DE LA POLIZA********************************************************************************************************
DECLARE
@COMENTARIOSV TABLE(
    ComentID int,	
	nPolizaV nchar(10),
	Descripcion varchar(850),
	Cambios varchar(850)
	)

--DATOS DE PROSPECTOS DE LA POLIZA***************************************************************************************
DECLARE
@PolProspectos TABLE(	
	nPoliza numeric(12, 0), 
	nConsPros int,
	cTipoModel char(30),
	cCiaAnteri char(30),
	dFTermVig smalldatetime
	)

--DATOS DE DESGLOSE DE PAGOS DE LA POLIZA******************************************************************************************************
DECLARE
@DESGLOSEPAGOSPOLIZA TABLE(
    ID int,
	nPoliza decimal(18, 0) ,
	nEndoso int,
	nFormaPagoID int,
	cNoTarjeta nvarchar(20),
	dfVigTarjeta datetime,
	cCodNegro nvarchar(15),
	mMonto money,
	mMontoSig money,
	cUsuario nvarchar(10),
	dfAlta datetime,
	cTitular varchar(100),
	cBanco varchar(100),
	cTpoTarjeta varchar(5),
	cDescBanco varchar(60),
	cDescrip varchar(50)
)


--DATOS DE CUPONES**********************************************************************************************************
DECLARE
@CUPONES TABLE(
	nPoliza numeric(12, 0),	
	nSolicitudID numeric(10, 0),
	mMontoCupon money,	
	dFIniVig smalldatetime,			
	dFFinVig smalldatetime,				
	nCuponAsig numeric(18, 0),
	nSiniestro numeric(18, 0)	
 )

--DATOS DE SINIESTROS**********************************************************************************************************
DECLARE
@SINIESTROS TABLE(
	nSiniestroID NUMERIC(7, 0),	
	dFSiniestro SMALLDATETIME,
	mImporte MONEY,
	cDescrip VARCHAR(60),
	cInforme VARCHAR(250),
	cConductor CHAR(40),	
	cTpoEspAut VARCHAR(100)
	)

--DATOS DE HISTORIAL*******************************************************************************************************************************
DECLARE
@HISTORIAL TABLE(
	nPoliza NUMERIC(12, 0),	
	cNombreA CHAR(80),	
	nSolicitudID NUMERIC(10, 0),
	dFIniVig SMALLDATETIME,
	dFFinVig SMALLDATETIME,	
	cNombre CHAR(60),
	cDescrip CHAR(18),
	dFExpedicion SMALLDATETIME,
	mPrimNeta MONEY,
	mDchosPol MONEY,
	mTotal MONEY
 )

--DATOS DE PRINCIPAL*******************************************************************************************************************************
 DECLARE
  @PRINCIPAL TABLE(
	nPoliza NUMERIC(12, 0),
	nEndoso NUMERIC(3, 0),
	dFExpedicion SMALLDATETIME,
	nSolicitudID NUMERIC(10, 0),
	dFIniVig SMALLDATETIME,
	dFFinVig smalldatetime,
	cImpreso CHAR(1),
	cTpoMov CHAR(25),
	mPrimNeta MONEY,
	mDchosPol MONEY,
	mRecPF MONEY,
	nIVA NUMERIC,
	nComAgente NUMERIC(2, 0),
	cDesc VARCHAR(50),
	cDetalle VARCHAR(300),
	cDetalle1 VARCHAR(300),
	cDetalle2 VARCHAR(300),
	mAp MONEY
)


IF EXISTS(SELECT * FROM polizas WHERE npoliza = @nPoliza
          UNION
          SELECT * FROM polizas WHERE npolap = @PolAP )
BEGIN

SET @nPoliza = (SELECT NPOLIZA FROM polizas WHERE npoliza = @nPoliza
          UNION
          SELECT NPOLIZA FROM polizas WHERE npolap = @PolAP )

SET @CONSECUTIVO = CONVERT(INT,SUBSTRING(CONVERT(VARCHAR,@nPoliza),6,6))


--DATOS DE POLIZA***************************************************************************************************************************
INSERT INTO @POLIZAS
SELECT *
     , DBO.SelNationalUnity(@nPoliza)
	 FROM POLIZAS
		WHERE NPOLIZA = @nPoliza

--DATOS DE PRIMAS DE POLIZA *****************************************************************************************************************
INSERT INTO @PRIMPOLIZAS (nPoliza,mPrimRCBI,mPrimProliber,mPrimDM,mPrimRT,mPrimEE,mPrimAV,mPrimGM,mPrimRCPD,mPrimExtRC,mPrimVehSus,mPrimSegObli)
	SELECT * FROM PRIMPOLIZAS WHERE NPOLIZA = @nPoliza

--DATOS GENERALES DE TELEFONOS****************************************************************************************************************	
INSERT INTO @CATTELEFONOS
	SELECT * FROM CATTELEFONOS WHERE NPOLIZA = @CONSECUTIVO

--COMENTARIOS VITALICIOS DE LA POLIZA*********************************************************************************************************
INSERT INTO @COMENTARIOSV
	SELECT * FROM COMENTARIOSV WHERE NPOLIZA = CONVERT(NVARCHAR,@CONSECUTIVO)

--DATOS DE PROSPECTOS DE LA POLIZA*************************************************************************************************************
INSERT INTO @PolProspectos
	SELECT * FROM PolProspectos WHERE NPOLIZA = @nPoliza

--DATOS DE DESGLOSE DE PAGOS DE LA POLIZA******************************************************************************************************
INSERT INTO @DESGLOSEPAGOSPOLIZA
	SELECT R.*,C.cDescBanco,F.cDescrip                
		FROM DesglosePagosPoliza  R INNER JOIN catFormaPago F ON R.nFormaPagoID = F.nFormaPagoID
		LEFT JOIN catBancos C ON CONVERT(NUMERIC,CASE WHEN R.cBanco= '' THEN NULL ELSE R.cBanco END) =CONVERT(NUMERIC, C.cBancoID) 
			WHERE npoliza = @nPoliza AND nendoso = 0  

--DATOS DE RECIBOS******************************************************************************************************************************
--INSERT INTO @RECIBOS

--DATOS DE CUPONES******************************************************************************************************************************
INSERT INTO @CUPONES 
	SELECT  nPoliza
		  , nSolicitudID
		  , mMontoCupon
		  , dFIniVig
		  , DATEADD(YEAR,2,dFIniVig)
		  , nCuponAsig
		  , nSiniestro
		FROM produccion.dbo.polizas
			WHERE nConsecutivo = CONVERT(NVARCHAR,@CONSECUTIVO) AND mmontocupon > 0 AND nsubramo = 1 ORDER BY npoliza

--DATOS DE SINIESTROS************************************************************************************** 
INSERT INTO @SINIESTROS
	SELECT S.nSiniestroID
	     , S.dFSiniestro
		 , S.mImporte
		 , C.cDescrip
		 , S.cInforme
		 , A.cConductor
		 , V.cTpoEspAut
		from Siniestros.dbo.Siniestros S 
				INNER JOIN Siniestros.dbo.Avisos A  ON A.nAvisoID = S.nAvisoID 
				INNER JOIN Siniestros.dbo.Vehisiniestrado V ON V.nAvisoID = A.nAvisoID AND V.Npoliza = A.Npoliza 
				INNER JOIN Siniestros.dbo.catCausas C on S.nCausaID = C.nCausaID
			WHERE A.nPoliza = @nPoliza
 
--DATOS DE HISTORIAL*******************************************************************************************************************************
INSERT INTO @HISTORIAL
	SELECT
	 Polizas.nPoliza AS Poliza,       
	 Polizas.cNombreA AS Asegurado,       
	 Polizas.nSolicitudID AS Solicitud,       
	 Polizas.dFIniVig AS Inicia,       
	 Polizas.dFFinVig AS Termina,       
	 catAgentes.cNombre AS Agente,       
	 catEstatusPol.cDescrip AS Estatus,       
	 dFExpedicion AS Registrada,       
	 Polizas.mPrimNeta AS Prima,       
	 Polizas.mDchosPol AS Derechos,      
	 (((Polizas.mPrimNeta * (1 + Polizas.mRecPF/100)) + Polizas.mDchosPol) * (1 + Polizas.nIva/100)) + ISNULL(Polizas.mAP,0) AS Total      
	 FROM   Polizas INNER JOIN catAgentes ON Polizas.nAgenteID = catAgentes.nAgenteID
	 INNER JOIN   catEstatusPol ON Polizas.nEstatusID = catEstatusPol.nEstatusID 
		 WHERE Polizas.nConsecutivo = CONVERT(NVARCHAR,@CONSECUTIVO)     
 
 --DATOS DE PRINCIPAL*******************************************************************************************************************************
INSERT INTO @PRINCIPAL
	SELECT  nPoliza
		  , nEndoso
		  , dFExpedicion
		  , nSolicitudID
		  , dFIniVig
		  , dFFinVig
		  , cImpreso
		  , CASE cTpoMov WHEN 'A' THEN 'Aumento'
						 WHEN 'B' THEN 'Sin Afectacion'
						 WHEN 'D' THEN 'Baja o Devolucion'
			END AS cTpoMov
		  , mPrimNeta
		  , mDchosPol
		  , (mPrimNeta ) * (mRecPF / 100) AS mRecPF
		  , (mPrimNeta + mDchosPol + (mPrimNeta ) * (mRecPF / 100)) * (nIVA / 100) AS nIVA
		  , nComAgente
		  , cDesc
		  , SUBSTRING(cObserv,1,200) AS cDetalle
		  , SUBSTRING(cObserv,201,400) AS cDetalle1
		  , SUBSTRING(cObserv,401,600) AS cDetalle2
		  , map       
		   FROM Endosos INNER JOIN catTpoAfectacion ON catTpoAfectacion.nTipoAfectID = Endosos.nTipoAfectID
				WHERE  NPOLIZA = @nPoliza   

IF EXISTS(SELECT * FROM ENDOSOS WHERE NPOLIZA = @nPoliza)
BEGIN

	    SET @MAXENDOSO = (SELECT MAX(NENDOSO) FROM ENDOSOS WHERE NPOLIZA = @nPoliza)
	
        --ACTUALIZANDO DATOS DE ASEGURADO (POLIZA + ENDOSOS)
        UPDATE @POLIZAS 
			SET P.nAgenteID = ENDOSOS.nAgenteID,
			P.cAPaternoC = ENDOSOS.cAPaternoC,
			P.cAMaternoC = ENDOSOS.cAMaternoC,
			P.cAsistVje = ENDOSOS.cAsistVje,
			P.nClienteID = ENDOSOS.nClienteID,
			P.cCondRest = ENDOSOS.cCondRest,
			P.cNombreA = ENDOSOS.cNombreA,
			P.cNombreC  = ENDOSOS.cNombreC,
			P.nNumVehi = ENDOSOS.nNumVehi,
			P.nNumCond = ENDOSOS.nNumCond,
			P.cObserv = ENDOSOS.cObserv,
			P.cResponsa = ENDOSOS.cResponsa,
			P.cRFC = ENDOSOS.cRFC,
			P.cSexoC = ENDOSOS.cSexoC,
			P.cEMail = ENDOSOS.EMail,
			P.cTituloC = ENDOSOS.cTituloC,
			P.cCober100 = ENDOSOS.cCober100,
			P.cTersa = ENDOSOS.cTersa,
			P.cFinanciamiento = ENDOSOS.cFinanciamiento,
			P.cNombreFinanciamiento = ENDOSOS.cNombreFinanciamiento,
			P.cEndosoCancela = ENDOSOS.cEndosoCancela,
			P.nReferenciaWeb = ENDOSOS.nReferenciaWeb,
			P.mRecPF = CASE WHEN P.mRecPF > ENDOSOS.mRecPF THEN P.mRecPF ELSE ENDOSOS.mRecPF END,
			P.mPrimNeta = P.mPrimNeta + ENDOSOS.mPrimNeta,
			P.mDchosPol = P.mDchosPol + ENDOSOS.mDchosPol
        FROM @POLIZAS P INNER JOIN ENDOSOS ON P.NPOLIZA = ENDOSOS.NPOLIZA
			WHERE ENDOSOS.NPOLIZA = @nPoliza AND ENDOSOS.NENDOSO = @MAXENDOSO
    
		--PRIMAS DE POLIZAS + PRIMAS DE ENDOSOS**********************************************************************************************************
		UPDATE @PRIMPOLIZAS 
			SET P.mPrimRCBI = P.mPrimRCBI + PRIMENDOSOS.mPrimRCBI,
			P.mPrimProliber  = P.mPrimProliber + PRIMENDOSOS.mPrimProliber,
			P.mPrimDM = P.mPrimDM + PRIMENDOSOS.mPrimDM,
			P.mPrimRT = P.mPrimRT + PRIMENDOSOS.mPrimRT,
			P.mPrimEE = P.mPrimEE + PRIMENDOSOS.mPrimEE,
			P.mPrimAV = P.mPrimAV + PRIMENDOSOS.mPrimAV,
			P.mPrimGM = P.mPrimGM + PRIMENDOSOS.mPrimGM,
			P.mPrimRCPD = P.mPrimRCPD + PRIMENDOSOS.mPrimRCPD,
			P.mPrimExtRC = P.mPrimExtRC + PRIMENDOSOS.mPrimExtRC,
			P.mPrimVehSus = P.mPrimVehSus + PRIMENDOSOS.mPrimVehSus,
			P.mPrimSegObli = P.mPrimSegObli + ISNULL(PRIMENDOSOS.mPrimSegObli,0)
        FROM @PRIMPOLIZAS P INNER JOIN PRIMENDOSOS ON P.NPOLIZA = PRIMENDOSOS.NPOLIZA 
			WHERE PRIMENDOSOS.NPOLIZA = @nPoliza AND PRIMENDOSOS.NENDOSO = @MAXENDOSO
		
		
		--DATOS DE PRIMAS DE VEHICULO DE POLIZA *******************************************************************************
        
        INSERT INTO @PRIMPOLVEHI 
			SELECT * FROM PRIMENDVEHI WHERE NPOLIZA = @nPoliza AND NENDOSO = @MAXENDOSO
       		
	    --DATOS DE DIRECCIONES DE POLIZA*****************************************************************************************************************
	    INSERT INTO @DIRSPOLIZA
	    SELECT * FROM DIRSENDOSO WHERE NPOLIZA = @nPoliza AND NENDOSO = @MAXENDOSO
			    
	    --DATOS DE CONDUCTORES DE POLIZA *************************************************************************************************************
	    INSERT INTO @POLCONDU 
			SELECT *
				FROM ENDCONDU
					WHERE NPOLIZA = @nPoliza 
					  AND NENDOSO = @MAXENDOSO 
					  And cEstatus = 'A'
	    
	    --DATOS DE VEHICULOS DE POLIZA****************************************************************************************************************
	    INSERT INTO @POLVEHICULOS 
	    SELECT * FROM ENDVEHICULOS WHERE NPOLIZA = @nPoliza AND NENDOSO = @MAXENDOSO
		
END
ELSE
BEGIN
        --DATOS DE PIMAS DE VEHICULO DE POLIZA *******************************************************************************************************
        INSERT INTO @PRIMPOLVEHI (nPoliza,nContVehi,mPrimRCBI,mPrimProliber,mPrimDM,mPrimRT,mPrimEE, mPrimAV,mPrimGM,mPrimRCPD,mPrimExtRC,mPrimVehSus,mPrimSegObli)
			SELECT * FROM PRIMPOLVEHI WHERE NPOLIZA = @nPoliza

		--DATOS DE DIRECCIONES DE POLIZA**************************************************************************************************************
		INSERT INTO @DIRSPOLIZA (nPoliza, nTipoDirID, cDireccion, nExterior, nInterior, cColonia, cPoblacion, nEstadoID, nCP, cTel, cOficina, cCelular, cFax,cOtro, cRequerida, cFiscal)
			SELECT * FROM DIRSPOLIZA WHERE NPOLIZA = @nPoliza
		
		--DATOS DE CONDUCTORES DE POLIZA *************************************************************************************************************
		INSERT INTO @POLCONDU (nPoliza, nContCond, cNombre, dFNacimien, cSexo, cEdoCivil, cExtencRC, mPrimExtRCCONDU)
			SELECT *
				FROM POLCONDU
					WHERE NPOLIZA = @nPoliza

		--DATOS DE VEHICULOS DE POLIZA****************************************************************************************************************
		INSERT INTO @POLVEHICULOS
			( nPoliza, nContVehi, cCP, nSubRamo, nModeloID, nAño, cGaragCasa, cGaragTrab, cUsoTrab, nDedudDM, nDedudRT, mCobertGM, mCobertRC, cDefAseLeg
			, cSumAsegF, nCondAsig, mSumAEqEsp, mValorAuto, cTpoEspAut, nPuertas, nCilindros, nTransmi, cNumSerie, cPlacas, cTpoAlarma, cSUVA, dFIniVig
			, dFFinVig, cEstatus, cVehSus, cDescEqEsp, cApoyo100, cCober100, cNCI, cEBC, nPagina, cTersa, mCobertRCCat, cValidaSerie, cAutorizaSerie
			, cAsistVje, cVin, cExcenDedu)
			SELECT * 
				FROM POLVEHICULOS
					WHERE NPOLIZA = @nPoliza
END


--DATOS DE BENEFICIARIOS DE POLIZA************************************************************************************************************
IF EXISTS (SELECT * FROM endbeneficiarios WHERE nPoliza = @nPoliza)
BEGIN
		INSERT INTO @POLBENEFICIARIOS 
			SELECT e.*
				 , C.Descripcion
				FROM endbeneficiarios e INNER JOIN catparentesco C ON e.ParentescoID = C.Parentesco
					WHERE NPOLIZA = @nPoliza AND nendoso = (SELECT MAX(nendoso)
																FROM endbeneficiarios
																	WHERE npoliza = @nPoliza)
END
ELSE
BEGIN
		INSERT INTO @POLBENEFICIARIOS 
			SELECT  nPoliza
				  , 0 as nEndoso
				  , nBenCont
				  , cNombre
				  , cApellidoP
				  , cApellidoM
				  , e.ParentescoID
				  , nPorcentaje
				  , cDomicilio
				  , dFechaNac
				  ,  C.Descripcion 
				FROM polbeneficiarios e INNER JOIN catparentesco C ON e.ParentescoID = C.Parentesco
					WHERE NPOLIZA = @nPoliza
END
END --EXISTE POLIZA*************************************************************************************************************************

SELECT *
	 , (mPrimNeta + mDchosPol) + (mPrimNeta * (mRecPF / 100))					As mSubTotal
	 , ((mPrimNeta + mDchosPol) + (mPrimNeta * (mRecPF / 100))) * (nIVA / 100)	As mIVA
	 FROM @POLIZAS				AS POLIZAS

SELECT * FROM @PRIMPOLIZAS			AS PRIMPOLIZAS
SELECT * FROM @POLVEHICULOS			AS POLVEHICULOS
SELECT * FROM @PRIMPOLVEHI			AS PRIMPOLVEHI

-- Recibos
SELECT  r.nReciboID
	  , r.nPoliza
	  , r.nRecibo
	  , CASE cTipo When 'P' Then 'Poliza'
				   When 'A' Then 'Endoso - Incremento'
				   When 'D' Then 'Endoso - Disminucion o Baja'
				   When 'B' Then 'Endoso - Sin Afectacion'
		End As cTipo
	  , r.nEndoso
	  , dFIniVig
	  , dFFinVig
	  , nEstatus
	  , Case nEstatus When 0 Then '0 - Pendiente de Pago'
					  When 1 Then '1 - '
					  When 2 Then '2 - Pagado'
					  When 3 Then '3 - '
		End As sEstatus
	  , ((mPrimNeta + mDchosPol + mRecPF +mAP+ r.mIVA)- mDescuento) as mPrimTotal
	  , mPrimNeta
	  , mDchosPol
	  , mRecPF
	  , mDescuento
	  , r.mIVA
	  , mComAgente
	  , dFEstatus
	  , ISNULL(mAP,0) mAP
	  , r.nImpreso
	FROM Recibos   r    
		WHERE r.nPoliza = @nPoliza 


SELECT * FROM @DIRSPOLIZA			AS DIRSPOLIZA
SELECT * FROM @POLCONDU				AS POLCONDU
SELECT * FROM @CATTELEFONOS			AS CATTELEFONOS
SELECT * FROM @COMENTARIOSV			AS COMENTARIOSV
SELECT * FROM @PolProspectos		AS POLPROSPECTOS
SELECT * FROM @DESGLOSEPAGOSPOLIZA	AS DESGLOSEPAGOSPOLIZA
SELECT * from @POLBENEFICIARIOS		AS POLBENEFICIARIOS
SELECT * FROM @CUPONES				AS CUPONES
SELECT * FROM @SINIESTROS			AS SINIESTROS
SELECT * FROM @HISTORIAL			AS HISTORIAL 
SELECT * FROM @PRINCIPAL			AS PRINCIPAL

-- Oficina
SELECT O.*
	FROM POLIZAS P Inner Join [dbo].[catOficinas] O On (P.nOficinaId = O.nOficinaId)
		WHERE NPOLIZA = @nPoliza

-- Agente
SELECT A.*
	FROM POLIZAS P Inner Join [dbo].[catAgentes]  A On (P.nAgenteId  = A.nAgenteId)
		WHERE NPOLIZA = @nPoliza

-- Campaña
SELECT C.*
	FROM POLIZAS P Inner Join [dbo].[catCamps]    C On (P.nCampId    = C.nCampId)
		WHERE NPOLIZA = @nPoliza

-- Forma de Pago
SELECT F.*
	FROM POLIZAS P Inner Join [dbo].[catPago]	   F On (P.nFormaPago = F.nFormaPagoId)
		WHERE NPOLIZA = @nPoliza

-- Tipo de Pago
SELECT T.*
	FROM POLIZAS P Inner Join [dbo].[catTpoPago]   T On (P.cFormaPago = T.cTpoPago)
		WHERE NPOLIZA = @nPoliza

-- Cliente
SELECT C.*
	FROM POLIZAS P Inner Join [dbo].[catClientes]  C On (P.nClienteID = C.nClienteID)
		WHERE NPOLIZA = @nPoliza

-- Facturas Electronicas  
Select ID_Factura
	 , Serie	
	 , Folio
	 , Estatus_Factura
	 , nRecibo
	 , nEndoso
	From dbtools.dbo.FacturaElectronica
		where nPoliza = @nPoliza

-- Pagos
SELECT  p.nControlID
	  , p.mMonto
	  , p.dFPago
	  , F.cdescrip as cTpoPgo
	  , p.cBanco
	  , p.nCuenta
	  , p.cReferencia
	  , p.cAplicado
	  , p.dFVencimiento
	  , p.cComentarios
	  , isnull(p.cCodNegro,'') as cCodNegro
	  , e.Serie
	  , e.folio
	  , p.nRecibo
	  , p.nEndoso
  FROM cobranza.dbo.Pagos p
				Inner Join cobranza.dbo.catformapgo				F on F.cFmaPgo = p.cFmaPgo
				Left outer join dbtools.dbo.facturaelectronica	E on p.ncontrolid = e.npoliza 
	WHERE p.nPoliza = @nPoliza


-- Comisiones
Select nComBase + nComNva + nComRen AS Porcentaje
	 , mComBase + mComNva + mComRen AS ValorCom
	 , mImporte
	 , nRecibo
	 , nEndoso
	From cobranza.dbo.comisionagte
		Where nPoliza = @nPoliza