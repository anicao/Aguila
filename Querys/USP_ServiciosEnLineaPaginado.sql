USE [Produccion]
GO
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

/********************************************************************************************************
 Elaborado por	:	---
 Fecha			: 	03/09/2015
 Proceso		: 	Servicios en Linea
 Descripción	: 	Portal Web
					@pPolizaID		: Número de Poliza
					@pSolicitudID	: Número de Solicitud
					@PageNo		    : Número de pagina a consultar
					@RecordsPerPage : Número de registros por página
					@TotalCount     : Cantidad de registros totales

*********************************************************************************************************
    Analista	Fecha		Proceso
*********************************************************************************************************/

ALTER PROCEDURE [dbo].[sp_ServiciosEnLineaPaginado]
  @pPolizaID		numeric	      = 0
, @pSolicitudID		numeric	      = 0
, @PageNo		    int
, @RecordsPerPage   int
, @TotalCount		int Output

As

Set Nocount ON

Declare @lRFC		VarChar(20)
	  , @lKeyWs		nVarChar(200)
	  , @lLinkWs	nVarChar(200)

Begin
	-- Clave de conexión al WS
	Select Top 1 @lKeyWs  = cKey
		       , @lRFC    = cRFC
			   , @lLinkWs = urlWSFactura
		From dbtools.dbo.catKeyFactura

	-- Total registros consultados
	Select @TotalCount = Count(*)
		From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
			Where p.nPoliza	     = @pPolizaID
			  and p.nSolicitudID = @pSolicitudID

	-- Datos por página
	Select  p.nPoliza
			, Convert(Char(10), p.dfExpedicion, 103) As dfExpedicion
			, p.nAgenteID
			, p.nClienteID
			, p.cNombreA
			, f.nEndoso
			, f.nRecibo
			, Serie
			, Folio
			, Convert(Char(10), Fecha_Emision, 103) As Fecha_Emision
			, @lLinkWs + 'serie=' + Convert(VarChar(20), Serie) + '&folio=' + Convert(VarChar(20), Folio) + '&tipo=PDF&rfc=' + @lRFC + '&key=' + @lKeyWs As LnkPDF
			, @lLinkWs + 'serie=' + Convert(VarChar(20), Serie) + '&folio=' + Convert(VarChar(20), Folio) + '&tipo=XML&rfc=' + @lRFC + '&key=' + @lKeyWs As LnkXML
			, Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S' and f.nRecibo = 1)
					Then 'ReportViewer.aspx?REPORT=C100&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza))) + '&E=' + Convert(VarChar(10), f.nEndoso)
					Else Space(100) End																As Lnk100
			, Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S' and f.nRecibo = 1)
					Then Space(1)
					Else 'disabled' End																As Css100
			, Case When f.nRecibo = 1
					Then 'ReportViewer.aspx?REPORT=PSOB&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza)))
					Else Space(100) End																As LnkSOB
		From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
			Where p.nPoliza	     = @pPolizaID
			  and p.nSolicitudID = @pSolicitudID
			Order By f.nRecibo Asc
				OFFSET (@PageNo-1) * @RecordsPerPage ROWS
					FETCH NEXT @RecordsPerPage ROWS ONLY		
End
