USE [Produccion]
GO
/****** Object:  StoredProcedure [dbo].[sp_RenovacionesAgentePaginadoPeriodoMes]    Script Date: 11/08/2015 10:21:32 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/********************************************************************************************************
 Elaborado por	:	---
 Fecha			: 	08/06/2015
 Proceso		: 	Consulta de Renovaciones via Web
 Descripción	: 	Portal Web
					@pAgenteID		: Id del Agente
					@pPeriodoID		: Periodo Año
					@pMesPeriodoID	: Periodo Mes
					@PageNo		    : Pagina a consultar
					@RecordsPerPage : Numero de registros a devolver

*********************************************************************************************************
    Analista	   Fecha		Cambio
		EZ		17/07/2015		Se agregaron columnas nuevas: cEstatus, DescripcionEstatus
								Se agregaron joins para extraer otra información
		EZ		11/08/2015		Corrección de error al obtener el total de registros
*********************************************************************************************************/

ALTER PROCEDURE [dbo].[sp_RenovacionesAgentePaginadoPeriodoMes]
  @pAgenteID		int		      = 0
, @pPeriodoID		VarChar(4)
, @pMesPeriodoID	VarChar(2)
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
			From [Produccion].[dbo].[Renovaciones]
				Where Year(dFFinVig) = @pPeriodoID
				And Month(dFFinVig) = @pMesPeriodoID
		
		-- Datos por página
		Select R.[nPoliza]
             , R.[cNombreA]											As Cliente
			 , LTRIM(RTRIM(R.cAPaternoC) + ' ' + RTRIM(R.cAMaternoC) + ' ' + RTRIM(R.cNombreC)) As Nombre
             , Convert(Char(10), R.[dFIniVig], 103)					As InicioVigencia
             , Convert(Char(10), R.[dFFinVig], 103)					As FinVigencia
             , IsNUll(O.[cObserv],'')								As Observaciones
             , IsNull(Convert(Char(10), P.[dfExpedicion], 103), '')	As FechaExpedicion
             , R.[nNumCond]
             , R.[nNumVehi]
             , R.[cCondRest]
             , R.[cRFC]
             , R.[nAgenteID]
             , R.[nCampID]
             , R.[cRecomienda]
             , R.[cResponsa]
             , O.[cEstSeg]											As nEstatusID
			 , E.[cDescripcion]										As DescripcionEstatus
             , R.[nClienteID]
             , R.[cPeriodo]
             , R.[cUsuario]
             , R.[nOficinaID]
			From [Produccion].[dbo].[Renovaciones] R Left Outer Join [Produccion].[dbo].[Seguimiento] O On  R.[nPoliza]		   = O.[nPoliza]
													 Left Outer Join [Produccion].[dbo].[CatEstSeg]	  E On  O.[cEstSeg]        = E.[cEstSeg]
													 Left Outer Join [Produccion].[dbo].[Polizas]	  P On (R.[nPoliza]	       = P.[nPoliza]
																										AND R.[nConsecutivo]   = P.[nConsecutivo]
																										AND (R.[nRenovacion]+1)= P.[nRenovacion])
                Where Year(R.dFFinVig) = @pPeriodoID
				And Month(R.dFFinVig) = @pMesPeriodoID
				  Order By R.nPoliza Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY
	End
	Else
	Begin
		-- Total registros consultados
		Select @TotalCount = Count(*)
			From [Produccion].[dbo].[Renovaciones]
				Where Year(dFFinVig) = @pPeriodoID
				And Month(dFFinVig) = @pMesPeriodoID
				  and nAgenteId = @pAgenteID

		-- Datos por página
		Select R.[nPoliza]
             , R.[cNombreA]											As Cliente
			 , LTRIM(RTRIM(R.cAPaternoC) + ' ' + RTRIM(R.cAMaternoC) + ' ' + RTRIM(R.cNombreC)) as Nombre
             , Convert(Char(10), R.[dFIniVig], 103)					As InicioVigencia
             , Convert(Char(10), R.[dFFinVig], 103)					As FinVigencia
             , IsNUll(O.[cObserv],'')								As Observaciones
             , IsNull(Convert(Char(10), P.[dfExpedicion], 103), '')	As FechaExpedicion
             , R.[nNumCond]
             , R.[nNumVehi]
             , R.[cCondRest]
             , R.[cRFC]
             , R.[nAgenteID]
             , R.[nCampID]
             , R.[cRecomienda]
             , R.[cResponsa]
             , O.[cEstSeg]											As nEstatusID
			 , E.[cDescripcion]										As DescripcionEstatus
             , R.[nClienteID]
             , R.[cPeriodo]
             , R.[cUsuario]
             , R.[nOficinaID]
			From [Produccion].[dbo].[Renovaciones] R Left Outer Join [Produccion].[dbo].[Seguimiento] O On  R.[nPoliza]		   = O.[nPoliza]
													 Left Outer Join [Produccion].[dbo].[CatEstSeg]	  E On  O.[cEstSeg]        = E.[cEstSeg]
													 Left Outer Join [Produccion].[dbo].[Polizas]	  P On (R.[nPoliza]	       = P.[nPoliza]
																										AND R.[nConsecutivo]   = P.[nConsecutivo]
																										AND (R.[nRenovacion]+1)= P.[nRenovacion])
				Where Year(R.dFFinVig) = @pPeriodoID
				And Month(R.dFFinVig) = @pMesPeriodoID
				  and R.nAgenteID  = @pAgenteID
				  Order By R.nPoliza Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY
	End
End
