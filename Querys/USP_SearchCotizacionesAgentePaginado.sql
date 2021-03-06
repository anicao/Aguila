USE [Produccion]
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

/********************************************************************************************************
 Elaborado por	:	---
 Fecha			: 	17/04/2015
 Proceso		: 	Busqueda de Cotizaciones via Web Año y Mes
 Descripción	: 	Portal Web
					@pAgenteID		: Id del Agente
					@PageNo		    : Pagina a consultar
					@RecordsPerPage : Numero de registros a devolver
					@@pTextSearch   : Texto a buscar

*********************************************************************************************************
    Analista	  Fecha			Cambio
*********************************************************************************************************/

CREATE PROCEDURE [dbo].[sp_SearchCotizacionesAgentePaginado]
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
			From [Produccion].[dbo].[Cotizacion]
				Where ((Convert(VarChar(15), nCotizaID) + cNombreA) Like '%' + @pTextSearch +'%')
		
		-- Datos por página
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
				Where ((Convert(VarChar(15), C.nCotizaID) + C.cNombreA) Like '%' + @pTextSearch +'%')
					Order By C.nCotizaID Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY
	End
	Else
	Begin
		-- Total registros consultados
		Select @TotalCount = Count(*)
			From [Produccion].[dbo].[Cotizacion]
				Where nAgenteId = @pAgenteID
				And ((Convert(VarChar(15), nCotizaID) + cNombreA) Like '%' + @pTextSearch +'%')

		-- Datos por página
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
				Where C.nAgenteId = @pAgenteID
				And ((Convert(VarChar(15), C.nCotizaID) + C.cNombreA) Like '%' + @pTextSearch +'%')
					Order By C.nCotizaID Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY
	End
End
