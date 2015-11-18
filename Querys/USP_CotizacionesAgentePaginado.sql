SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

/********************************************************************************************************
 Elaborado por	:	---
 Fecha			: 	17/04/2015
 Proceso		: 	Consulta de Cotizaciones via Web
 Descripción	: 	Portal Web
					@pAgenteID		: Id del Agente
					@PageNo		    : Pagina a consultar
					@RecordsPerPage : Numero de registros a devolver

*********************************************************************************************************
    Analista	Fecha		Proceso
*********************************************************************************************************/

ALTER PROCEDURE [dbo].[sp_CotizacionesAgentePaginado]
  @pAgenteID		int		      = 0
, @pPeriodoID		VarChar(4)
, @PageNo		    int
, @RecordsPerPage   int
, @TotalCount		int Output

As

Set Nocount ON

Begin
	If (@pAgenteID = 0)
	Begin
		-- Total registros consultados
		Select @TotalCount = Count(*)
			From [Produccion].[dbo].[Cotizacion]
                Where dfRegistro like '%' + @pPeriodoID + '%'
		
		-- Datos por página
		Select C.[nCotizaID]
             , C.[cNombreA]								As Cliente
             , Convert(Char(10), C.[dFCotiza], 103)		As InicioVigencia
             , RTRIM(T.[cTel])							As Telefono
             , C.[cObserv]								As Observaciones
             , Convert(Char(10), C.[dFRegistro], 103)	As Emision
             , C.[cAPaternoC]
             , C.[cAMaternoC]
             , C.[cNombresC]
             , C.[cTituloC]
             , C.[cSexoC]
             , C.[nNumCond]
             , C.[nNumVehi]
             , C.[cCondRest]
             , C.[nMeses]
             , C.[nFormaPago]
             , C.[cAsistVje]
             , C.[cTpoPago]
             , C.[cRFC]
             , C.[nAgenteID]
             , C.[nCampID]
             , C.[cRecomienda]
             , C.[cResponsa]
             , C.[cEMail]
             , C.[cEstatus]
             , C.[nClienteID]
             , C.[cPeriodo]
             , C.[cTipo]
             , C.[cMasAutos]
             , C.[cNumTarjeta]
             , C.[dFFinVigTarj]
             , C.[cUsuario]
             , C.[nCveObs]
             , C.[cCuentaCheques]
             , C.[cCveBanco]
             , C.[cNomTitular]
             , C.[nOficinaID]
             , C.[cRespAnt]
             , C.[dfRespAnt]
             , C.[nControl]
             , C.[cCober100]
             , C.[cCodigoNegro]
             , C.[cEmailOtro]
             , C.[cEntPol]
             , C.[cEntComen]
             , C.[cEstatusAnt]
             , C.[cCIE]
             , C.[dfVigChequera]
             , C.[cVendida]
             , C.[cFormaPago]
             , C.[cEra]
             , C.[cTersa]
             , C.[nSolicitudId]
             , C.[nSolicitudId_Ant]
             , C.[nComCedida]
             , C.[mComCedida]
             , C.[mComRestante]
             , C.[ncontrolnuevo]
             , C.[dfContraEntrega]
             , C.[dfAutProcar]
             , C.[cFinanciamiento]
             , C.[cNombreFinanciamiento]
             , C.[cEndosoCancela]
             , C.[nControlMaximo]
             , C.[cAP]
             , C.[nReferenciaWeb]
			From [Produccion].[dbo].[Cotizacion] C Inner Join [Produccion].[dbo].[catTelefonos]  T ON C.[nCotizaID] = T.[nCotizaID]
                Where C.dfRegistro Like '%' + @pPeriodoID + '%'
				  Order By C.nCotizaID Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY
	End
	Else
	Begin
		-- Total registros consultados
		Select @TotalCount = Count(*)
			From [Produccion].[dbo].[Cotizacion]
                Where dfRegistro like '%' + @pPeriodoID + '%'
				  and nAgenteId = @pAgenteID

		-- Datos por página
		Select C.[nCotizaID]
             , C.[cNombreA]								As Cliente
             , Convert(Char(10), C.[dFCotiza], 103)		As InicioVigencia
             , RTRIM(T.[cTel])							As Telefono
             , C.[cObserv]								As Observaciones
             , Convert(Char(10), C.[dFRegistro], 103)	As Emision
             , C.[cAPaternoC]
             , C.[cAMaternoC]
             , C.[cNombresC]
             , C.[cTituloC]
             , C.[cSexoC]
             , C.[nNumCond]
             , C.[nNumVehi]
             , C.[cCondRest]
             , C.[nMeses]
             , C.[nFormaPago]
             , C.[cAsistVje]
             , C.[cTpoPago]
             , C.[cRFC]
             , C.[nAgenteID]
             , C.[nCampID]
             , C.[cRecomienda]
             , C.[cResponsa]
             , C.[cEMail]
             , C.[cEstatus]
             , C.[nClienteID]
             , C.[cPeriodo]
             , C.[cTipo]
             , C.[cMasAutos]
             , C.[cNumTarjeta]
             , C.[dFFinVigTarj]
             , C.[cUsuario]
             , C.[nCveObs]
             , C.[cCuentaCheques]
             , C.[cCveBanco]
             , C.[cNomTitular]
             , C.[nOficinaID]
             , C.[cRespAnt]
             , C.[dfRespAnt]
             , C.[nControl]
             , C.[cCober100]
             , C.[cCodigoNegro]
             , C.[cEmailOtro]
             , C.[cEntPol]
             , C.[cEntComen]
             , C.[cEstatusAnt]
             , C.[cCIE]
             , C.[dfVigChequera]
             , C.[cVendida]
             , C.[cFormaPago]
             , C.[cEra]
             , C.[cTersa]
             , C.[nSolicitudId]
             , C.[nSolicitudId_Ant]
             , C.[nComCedida]
             , C.[mComCedida]
             , C.[mComRestante]
             , C.[ncontrolnuevo]
             , C.[dfContraEntrega]
             , C.[dfAutProcar]
             , C.[cFinanciamiento]
             , C.[cNombreFinanciamiento]
             , C.[cEndosoCancela]
             , C.[nControlMaximo]
             , C.[cAP]
             , C.[nReferenciaWeb]
			From [Produccion].[dbo].[Cotizacion] C Inner Join [Produccion].[dbo].[catTelefonos]  T ON C.[nCotizaID] = T.[nCotizaID]
                Where C.dfRegistro Like '%' + @pPeriodoID + '%'
				  and C.nAgenteID  = @pAgenteID
				  Order By C.nCotizaID Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY
	End
End
GO