

-- Create the data type
CREATE TYPE RenovacionType AS TABLE 
(
	nPoliza numeric(9),
	nOficinaID  int,
	nRamo numeric(5),
	nSubramo numeric(5),
	nRenovacion numeric(5),
	nConsecutivo numeric(5),
	nClienteID  numeric(5),
	cNombreA char(80),
	cRFC char(13),
	nSolicitudID numeric(9),
	dFIniVig smalldatetime,
	dFFinVig smalldatetime,
	nUtlrec int,
	dFSigrec smalldatetime,
	nFormaPago  int,
	nAgenteID int,
	nEstatusID  int,
	dFEstatus smalldatetime,
	dFExpedicion smalldatetime,
	mPrimNeta money( 8),
	mDchosPol money( 8),
	mRecPF  money( 8),
	nIVA numeric(5),
	mAP money( 8),
	nComAgente  numeric(5),
	nNumVehi int,
	nNumCond int,
	dFUltPago smalldatetime,
	dFSigPago smalldatetime,
	cCondRest char(1),
	nCampID int,
	cAsistVje char(1),
	cTpoPago char(2),
	cPeriodo char(6),
	nControl numeric(9),
	cObserv varchar(250),
	cAPaternoC  char(25),
	cAMaternoC  char(25),
	cNombreC char(25),
	cTituloC char(8),
	cSexoC  char(1),
	cRecomienda char(15),
	nReferidos  numeric(5),
	nRefUsados  numeric(5),
	cUsuario char(10),
	cImpresa char(1),
	cEMail  varchar(100),
	cSeguim char(1),
	cResponsa char(3),
	nNumUsrID numeric(5),
	mMontoCupon money( 8),
	cCuponImpreso char(1),
	cMasAutos char(1),
	cNumTarjeta char(18),
	dfFinVigTarj smalldatetime,
	nMeses  int,
	cObstarj varchar(150),
	nCveObs int,
	cCuentaCheques  char(20),
	cCveBanco char(3),
	cNomTitular varchar(60),
	cCober100 char(1),
	cCodigoNegro varchar(5),
	cEmailOtro  varchar(100),
	cEntPol char(1),
	cEntComen varchar(80),
	cCIE varchar(50),
	dfVigChequera datetime,
	cResponsaAnt char(3),
	dfCambioIni smalldatetime,
	cFormaPago  varchar(2),
	cEra char(1),
	cTersa  char(1),
	nComCedida  numeric(9),
	mComCedida  money( 8),
	mComRestante money( 8),
	nControlnuevo numeric(9),
	dfContraEntrega datetime,
	dfAutProcar datetime,
	cFinanciamiento char(1),
	cNombreFinanciamiento varchar(200),
	cEndosoCancela  char(1),
	nControlMaximo  int,
	nReferenciaWeb  numeric(9)
)
GO
