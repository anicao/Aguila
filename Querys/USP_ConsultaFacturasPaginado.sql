SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

/********************************************************************************************************
 Elaborado por	:	---
 Fecha			: 	08/04/2015
 Proceso		: 	Reporte de Consulta de Facturas via Web
 Descripción	: 	Portal Web
					@pFechaIni		: Fecha Inicio
					@pFechaFin		: Fecha Final
					@pAgenteID		: Id del Agente
					@pPolizaID		: Numero de Poliza
					@pClientID		: Id del Cliente
					@PageNo		    : Pagina a consultar
					@RecordsPerPage   : Numero de registros a devolver

*********************************************************************************************************
    Analista	Fecha		Proceso
*********************************************************************************************************/

ALTER PROCEDURE [dbo].[sp_ConsultaFacturasPaginado]
  @pFechaIni		Char(10)	  = null
, @pFechaFin		Char(10)	  = null
, @pAgenteID		int		      = 0
, @pPolizaID		numeric	      = 0
, @pClientID		int		      = 0
, @PageNo		    int
, @RecordsPerPage   int
, @TotalCount		int Output

As

Set Nocount ON

Declare @lRFC		VarChar(20)
	  , @lKeyWs		nVarChar(200)
	  , @lLinkWs	nVarChar(200)
	  , @lBits		Char(3)

Begin
	-- Clave de conexión al WS
	Select Top 1 @lKeyWs = cKey From dbtools.dbo.catKeyFactura

	-- RFC Empresa
	Select @lRFC = 'ASE941124NN4'

	-- Link del Web Service
	Select @lLinkWs = 'http://200.52.84.164/xsamanager/downloadCfdWebView?'

	--
	Select @lBits = Case When @pClientID > 0   Then '1' Else '0' End
				  + Case When @pPolizaID > 0   Then '1' Else '0' End
				  + Case When @pFechaIni != '' Then '1' Else '0' End

	-- Facturas por agente
	If (@lBits = '000')
	Begin
		-- Total registros consultados
		Select @TotalCount = Count(*)
			From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
				Where nAgenteID = @pAgenteID

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
			  , Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S')
					 Then 'ReportViewer.aspx?REPORT=C100&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza))) + '&E=' + Convert(VarChar(10), f.nEndoso)
					 Else Space(100) End															As Lnk100
			  , Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S')
					 Then Space(1)
					 Else 'disabled' End															As Css100
			  , 'ReportViewer.aspx?REPORT=PSOB&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza)))	As LnkSOB
			From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
				Where p.nAgenteID = @pAgenteID
				Order By p.nPoliza Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY		
	End

	-- Facturas por agente y fecha de expedición
	If (@lBits = '001')
	Begin
		-- Total registros consultados
		Select @TotalCount = Count(*)
			From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
				Where nAgenteID = @pAgenteID
				  and dfExpedicion >= @pFechaIni and dfExpedicion <= @pFechaFin

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
			  , Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S')
					 Then 'ReportViewer.aspx?REPORT=C100&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza))) + '&E=' + Convert(VarChar(10), f.nEndoso)
					 Else Space(100) End															As Lnk100
			  , Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S')
					 Then Space(1)
					 Else 'disabled' End															As Css100
			  , 'ReportViewer.aspx?REPORT=PSOB&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza)))	As LnkSOB
			From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
				Where p.nAgenteID = @pAgenteID
				  and dfExpedicion >= @pFechaIni and dfExpedicion <= @pFechaFin
				Order By p.nPoliza Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY		
	End

	-- Facturas por agente y poliza
	If (@lBits = '010')
	Begin
		-- Total registros consultados
		Select @TotalCount = Count(*)
			From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
				Where nAgenteID = @pAgenteID
				  and p.nPoliza = @pPolizaID

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
			  , Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S')
					 Then 'ReportViewer.aspx?REPORT=C100&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza))) + '&E=' + Convert(VarChar(10), f.nEndoso)
					 Else Space(100) End															As Lnk100
			  , Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S')
					 Then Space(1)
					 Else 'disabled' End															As Css100
			  , 'ReportViewer.aspx?REPORT=PSOB&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza)))	As LnkSOB
			From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
				Where p.nAgenteID = @pAgenteID
				  and p.nPoliza   = @pPolizaID
				Order By p.nPoliza Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY		
	End

	-- Facturas por agente, fecha de expedición y poliza
	If (@lBits = '011')
	Begin
		-- Total registros consultados
		Select @TotalCount = Count(*)
			From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
				Where nAgenteID = @pAgenteID
				  and dfExpedicion >= @pFechaIni and dfExpedicion <= @pFechaFin
				  and p.nPoliza = @pPolizaID

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
			  , Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S')
					 Then 'ReportViewer.aspx?REPORT=C100&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza))) + '&E=' + Convert(VarChar(10), f.nEndoso)
					 Else Space(100) End															As Lnk100
			  , Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S')
					 Then Space(1)
					 Else 'disabled' End															As Css100
			  , 'ReportViewer.aspx?REPORT=PSOB&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza)))	As LnkSOB
			From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
				Where p.nAgenteID = @pAgenteID
				  and dfExpedicion >= @pFechaIni and dfExpedicion <= @pFechaFin
				  and p.nPoliza   = @pPolizaID
				Order By p.nPoliza Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY		
	End

	-- Facturas por agente y cliente
	If (@lBits = '100')
	Begin
		-- Total registros consultados
		Select @TotalCount = Count(*)
			From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
				Where nAgenteID    = @pAgenteID
				  and p.nClienteID = @pClientID

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
			  , Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S')
					 Then 'ReportViewer.aspx?REPORT=C100&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza))) + '&E=' + Convert(VarChar(10), f.nEndoso)
					 Else Space(100) End															As Lnk100
			  , Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S')
					 Then Space(1)
					 Else 'disabled' End															As Css100
			  , 'ReportViewer.aspx?REPORT=PSOB&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza)))	As LnkSOB
			From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
				Where p.nAgenteID  = @pAgenteID
				  and p.nClienteID = @pClientID
				Order By p.nPoliza Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY		
	End

	-- Facturas por agente, cliente y fecha de expedición
	If (@lBits = '101')
	Begin
		-- Total registros consultados
		Select @TotalCount = Count(*)
			From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
				Where nAgenteID    = @pAgenteID
				  and dfExpedicion >= @pFechaIni and dfExpedicion <= @pFechaFin
				  and p.nClienteID = @pClientID

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
			  , Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S')
					 Then 'ReportViewer.aspx?REPORT=C100&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza))) + '&E=' + Convert(VarChar(10), f.nEndoso)
					 Else Space(100) End															As Lnk100
			  , Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S')
					 Then Space(1)
					 Else 'disabled' End															As Css100
			  , 'ReportViewer.aspx?REPORT=PSOB&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza)))	As LnkSOB
			From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
				Where p.nAgenteID  = @pAgenteID
				  and dfExpedicion >= @pFechaIni and dfExpedicion <= @pFechaFin
				  and p.nClienteID = @pClientID
				Order By p.nPoliza Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY		
	End

	-- Facturas por agente, cliente y poliza
	If (@lBits = '110')
	Begin
		-- Total registros consultados
		Select @TotalCount = Count(*)
			From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
				Where nAgenteID    = @pAgenteID
				  and p.nPoliza    = @pPolizaID
				  and p.nClienteID = @pClientID

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
			  , Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S')
					 Then 'ReportViewer.aspx?REPORT=C100&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza))) + '&E=' + Convert(VarChar(10), f.nEndoso)
					 Else Space(100) End															As Lnk100
			  , Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S')
					 Then Space(1)
					 Else 'disabled' End															As Css100
			  , 'ReportViewer.aspx?REPORT=PSOB&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza)))	As LnkSOB
			From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
				Where p.nAgenteID  = @pAgenteID
				  and p.nPoliza    = @pPolizaID
				  and p.nClienteID = @pClientID
				Order By p.nPoliza Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY		
	End

	-- Facturas por agente, fecha de expedición, cliente y poliza
	If (@lBits = '111')
	Begin
		-- Total registros consultados
		Select @TotalCount = Count(*)
			From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
				Where nAgenteID    = @pAgenteID
				  and dfExpedicion >= @pFechaIni and dfExpedicion <= @pFechaFin
				  and p.nPoliza    = @pPolizaID
				  and p.nClienteID = @pClientID

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
			  , Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S')
					 Then 'ReportViewer.aspx?REPORT=C100&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza))) + '&E=' + Convert(VarChar(10), f.nEndoso)
					 Else Space(100) End															As Lnk100
			  , Case When Exists(Select nPoliza From Produccion.dbo.PolVehiculos Where npoliza = p.nPoliza and cCober100 = 'S')
					 Then Space(1)
					 Else 'disabled' End															As Css100
			  , 'ReportViewer.aspx?REPORT=PSOB&P=' + lTrim(rTrim(Convert(VarChar(50), p.nPoliza)))	As LnkSOB
			From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
				Where p.nAgenteID  = @pAgenteID
				  and dfExpedicion >= @pFechaIni and dfExpedicion <= @pFechaFin
				  and p.nPoliza    = @pPolizaID
				  and p.nClienteID = @pClientID
				Order By p.nPoliza Desc
					OFFSET (@PageNo-1) * @RecordsPerPage ROWS
						FETCH NEXT @RecordsPerPage ROWS ONLY		
	End
End
GO