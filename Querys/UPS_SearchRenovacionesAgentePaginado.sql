USE [Produccion]
GO
/****** Object:  StoredProcedure [dbo].[sp_RenovacionesAgentePaginadoPeriodoMes]    Script Date: 17/07/2015 05:58:55 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/********************************************************************************************************
 Elaborado por	:	---
  Fecha			: 	17/04/2015
 Proceso		: 	Busqueda de Renovaciones via Web Año y Mes
 Descripción	: 	Portal Web
					@pAgenteID		: Id del Agente
					@PageNo		    : Pagina a consultar
					@RecordsPerPage : Numero de registros a devolver
					@@pTextSearch   : Texto a buscar

*********************************************************************************************************
    Analista	  Fecha			Cambio
*********************************************************************************************************/

CREATE PROCEDURE [dbo].[sp_SearchRenovacionesAgentePaginado]
  @pAgenteID		int		     = 0
, @PageNo		    int
, @RecordsPerPage   int
, @pTextSearch		VarChar(100) = ''
, @TotalCount		int Output

As

Set Nocount ON
Begin
	If (@pAgenteID = 0)
	Begin
		-- Total registros consultados
		Select @TotalCount = Count(*)
			From [Produccion].[dbo].[Renovaciones]
				Where ((Convert(VarChar(15), nPoliza) + cNombreA) Like '%' + @pTextSearch +'%')
		
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
				Where ((Convert(VarChar(15), R.nPoliza) + R.cNombreA) Like '%' + @pTextSearch +'%')
				  Order By R.nPoliza Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY
	End
	Else
	Begin
		-- Total registros consultados
		Select @TotalCount = Count(*)
			From [Produccion].[dbo].[Renovaciones]
				Where nAgenteId = @pAgenteID
				And ((Convert(VarChar(15), nPoliza) + cNombreA) Like '%' + @pTextSearch +'%')

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
				Where R.nAgenteId = @pAgenteID
				And ((Convert(VarChar(15), R.nPoliza) + R.cNombreA) Like '%' + @pTextSearch +'%')
				  Order By R.nPoliza Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY
	End
End
