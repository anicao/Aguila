SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

/********************************************************************************************************
 Elaborado por	:	---
 Fecha			: 	08/06/2015
 Proceso		: 	Consulta de Renovaciones via Web
 Descripción	: 	Portal Web
					@pAgenteID		: Id del Agente
					@PageNo		    : Pagina a consultar
					@RecordsPerPage : Numero de registros a devolver

*********************************************************************************************************
    Analista	Fecha		Proceso
*********************************************************************************************************/

CREATE PROCEDURE [dbo].[sp_RenovacionesAgentePaginado]
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
			From [Produccion].[dbo].[Renovaciones]
                Where dFFinVig like '%' + @pPeriodoID + '%'
		
		-- Datos por página
		Select R.[nPoliza]
             , R.[cNombreA]								As Cliente
			 , LTRIM(RTRIM(R.cAPaternoC) + ' ' + RTRIM(R.cAMaternoC) + ' ' + RTRIM(R.cNombreC)) as Nombre
             , Convert(Char(10), R.[dFIniVig], 103)		As InicioVigencia
             , Convert(Char(10), R.[dFFinVig], 103)		As FinVigencia
             , R.[cObserv]								As Observaciones
             , Convert(Char(10), R.[dFExpedicion], 103)	As Emision
             , R.[nNumCond]
             , R.[nNumVehi]
             , R.[cCondRest]
             , R.[cRFC]
             , R.[nAgenteID]
             , R.[nCampID]
             , R.[cRecomienda]
             , R.[cResponsa]
             , R.[nEstatusID]
             , R.[nClienteID]
             , R.[cPeriodo]
             , R.[cUsuario]
             , R.[nOficinaID]
			From [Produccion].[dbo].[Renovaciones] R
                Where R.dFFinVig Like '%' + @pPeriodoID + '%'
				  Order By R.nPoliza Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY
	End
	Else
	Begin
		-- Total registros consultados
		Select @TotalCount = Count(*)
			From [Produccion].[dbo].[Renovaciones]
                Where dFFinVig like '%' + @pPeriodoID + '%'
				  and nAgenteId = @pAgenteID

		-- Datos por página
		Select R.[nPoliza]
             , R.[cNombreA]								As Cliente
			 , LTRIM(RTRIM(R.cAPaternoC) + ' ' + RTRIM(R.cAMaternoC) + ' ' + RTRIM(R.cNombreC)) as Nombre
             , Convert(Char(10), R.[dFIniVig], 103)		As InicioVigencia
             , Convert(Char(10), R.[dFFinVig], 103)		As FinVigencia
             , R.[cObserv]								As Observaciones
             , Convert(Char(10), R.[dFExpedicion], 103)	As Emision
             , R.[nNumCond]
             , R.[nNumVehi]
             , R.[cCondRest]
             , R.[cRFC]
             , R.[nAgenteID]
             , R.[nCampID]
             , R.[cRecomienda]
             , R.[cResponsa]
             , R.[nEstatusID]
             , R.[nClienteID]
             , R.[cPeriodo]
             , R.[cUsuario]
             , R.[nOficinaID]
			From [Produccion].[dbo].[Renovaciones] R
                Where R.dFFinVig Like '%' + @pPeriodoID + '%'
				  and R.nAgenteID  = @pAgenteID
				  Order By R.nPoliza Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY
	End
End
GO