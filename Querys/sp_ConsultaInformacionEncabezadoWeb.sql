USE [Produccion]
GO
/****** Object:  StoredProcedure [dbo].[sp_ConsultaInformacionEncabezadoWeb]    Script Date: 10/08/2015 12:20:41 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/********************************************************************************************************
 Elaborado por	:	---
 Fecha			: 	30/07/2015
 Proceso		: 	Reporte de Consulta Información de Encabezado Web
 Descripción	: 	Portal Web
					@pAgenteID		: Id del Agente

*********************************************************************************************************
    Analista	Fecha		Proceso
*********************************************************************************************************/

CREATE PROCEDURE [dbo].[sp_ConsultaInformacionEncabezadoWeb]
  @pAgenteID		INT		      = 0
AS

SET NOCOUNT ON

DECLARE @NumCotizacionesMes			INT = 0
	  , @NumCotizacionesAño			INT = 0
	  , @MontoPolPrimNetaMes		DECIMAL = 0
	  , @MontoPolPrimNetaAño		DECIMAL = 0
	  , @NumRenovacionesMes			INT = 0
      , @NumRenovacionesAño			INT = 0
      , @MontoRenPolPrimNetaMes		DECIMAL = 0	
      , @MontoREnPolPrimNetaAño		DECIMAL = 0
	  , @NumEndososMes				INT = 0
      , @NumEndososAño				INT = 0
      , @MontoEndososMes			DECIMAL = 0	
      , @MontoEndososAño			DECIMAL = 0

	  , @UltimoPago					VARCHAR(10)
	  , @FechaIniMes				VARCHAR(8)
	  , @FechaFinMes				VARCHAR(8)
	  , @FechaIniAño				VARCHAR(8)
	  , @FechaFinAño				VARCHAR(8)
BEGIN
	
	SELECT @FechaIniMes = CONVERT(VARCHAR,DATEADD(MONTH, DATEDIFF(MONTH, 0,GETDATE()), 0),112)
	SELECT @FechaFinMes = CONVERT(VARCHAR,EOMONTH(GETDATE(),0),112)
	SELECT @FechaIniAño = CONVERT(VARCHAR,DATEADD(YY, DATEDIFF(YY,0,GETDATE()), 0),112)
	SELECT @FechaFinAño = CONVERT(VARCHAR,YEAR(GETDATE())) + '12' + '31'

	
	--COTIZACIONES DEL MES
	SELECT			@NumCotizacionesMes = COUNT(nCotizaID)
	FROM            Produccion.dbo.Cotizacion	
	WHERE			(CONVERT(VARCHAR,dFRegistro,112) >= @FechaIniMes) 
	AND				(CONVERT(VARCHAR,dFRegistro,112) <= @FechaFinMes) 
	AND				(nAgenteID = @pAgenteID)


	--COTIZACIONES DE AÑO
	SELECT			@NumCotizacionesAño = COUNT(nCotizaID)
	FROM            Produccion.dbo.Cotizacion
	WHERE			(CONVERT(VARCHAR,dFRegistro,112) >= @FechaIniAño) 
	AND				(CONVERT(VARCHAR,dFRegistro,112) <= @FechaFinAño) 
	AND				(nAgenteID = @pAgenteID)

	
	--PRIMA NETA EMITIDA DEL MES
	SELECT			@MontoPolPrimNetaMes = ISNULL(SUM(mprimneta),0) 
	FROM			produccion.dbo.polizas 
	WHERE			(CONVERT(VARCHAR,dfexpedicion,112) >= @FechaIniMes) 
	AND				(CONVERT(VARCHAR,dfexpedicion,112) <= @FechaFinMes) 
	AND				nrenovacion = 0 
	AND				nagenteid = @pAgenteID


	--PRIMA NETA EMITIDA EN EL AÑO
	SELECT			@MontoPolPrimNetaAño = ISNULL(SUM(mprimneta),0) 
	FROM			produccion.dbo.polizas 
	WHERE			(CONVERT(VARCHAR,dfexpedicion,112) >= @FechaIniAño) 
	AND			    (CONVERT(VARCHAR,dfexpedicion,112) <= @FechaFinAño)
	AND				nrenovacion = 0 
	AND				nagenteid = @pAgenteID


	--RENOVACIONES DEL MES
	SELECT			@NumRenovacionesMes = COUNT(npoliza)
	FROM            Produccion.dbo.renovaciones
	WHERE			(CONVERT(VARCHAR,dfinivig,112) >= @FechaIniMes) 
	AND				(CONVERT(VARCHAR,dfinivig,112) <= @FechaFinMes) 
	AND				nagenteid = @pAgenteID


	--RENOVACIONES EN EL AÑO
	SELECT			@NumRenovacionesAño = COUNT(npoliza) 
	FROM            Produccion.dbo.renovaciones
	WHERE			(CONVERT(VARCHAR,dfinivig,112) >= @FechaIniAño) 
	AND				(CONVERT(VARCHAR,dfinivig,112) <= @FechaFinAño) 
	AND				nagenteid = @pAgenteID



	--PRIMA NETA EMITIDA EN EL MES
	SELECT			@MontoRenPolPrimNetaMes = ISNULL(sum(mprimneta),0) 
	from			produccion.dbo.polizas 
	WHERE			(CONVERT(VARCHAR,dfexpedicion,112) >= @FechaIniMes)
	AND				(CONVERT(VARCHAR,dfexpedicion,112) <= @FechaFinMes)
	AND				nrenovacion > 0 
	AND				nagenteid = @pAgenteID


	--PRIMA NETA EMITIDA EN EL AÑO
	SELECT			@MontoREnPolPrimNetaAño = ISNULL(sum(mprimneta),0) 
	FROM			produccion.dbo.polizas 
	WHERE			(CONVERT(VARCHAR,dfexpedicion,112) >= @FechaIniAño)
	AND				(CONVERT(VARCHAR,dfexpedicion,112) <= @FechaFinAño) 
	AND				nrenovacion > 0 
	AND				nagenteid = @pAgenteID


	-- ENDOSOS DEL MES
	SELECT			@NumEndososMes = IsNull(COUNT(nEndoso), 0)
	FROM            Produccion.dbo.endosos
	WHERE			(CONVERT(VARCHAR,dfexpedicion,112) >= @FechaIniMes) 
	AND				(CONVERT(VARCHAR,dfexpedicion,112) <= @FechaFinMes) 
	AND				nagenteid = @pAgenteID


	-- ENDOSOS EN EL AÑO
	SELECT			@NumEndososAño = IsNull(COUNT(nEndoso), 0)
	FROM            Produccion.dbo.endosos
	WHERE			(CONVERT(VARCHAR,dfexpedicion,112) >= @FechaIniAño) 
	AND				(CONVERT(VARCHAR,dfexpedicion,112) <= @FechaFinAño) 
	AND				nagenteid = @pAgenteID

	-- MONTO ENDOSOS DEL MES
	SELECT			@MontoEndososMes = IsNull(Sum(mPrimNeta), 0)
	FROM            Produccion.dbo.endosos
	WHERE			(CONVERT(VARCHAR,dfexpedicion,112) >= @FechaIniMes) 
	AND				(CONVERT(VARCHAR,dfexpedicion,112) <= @FechaFinMes) 
	AND				nagenteid = @pAgenteID

	-- MONTO ENDOSOS EN EL AÑO
	SELECT			@MontoEndososAño = IsNull(Sum(mPrimNeta), 0)
	FROM            Produccion.dbo.endosos
	WHERE			(CONVERT(VARCHAR,dfexpedicion,112) >= @FechaIniAño) 
	AND				(CONVERT(VARCHAR,dfexpedicion,112) <= @FechaFinAño) 
	AND				nagenteid = @pAgenteID

	--ULTIMO PAGO DE COMISIONES
	SELECT			@UltimoPago = CONVERT(VARCHAR,MAX(dfaplica),103) 
	FROM			cobranza.dbo.comisionagte 
	WHERE			nagenteid = @pAgenteID 
	AND				cestatus = 'P'

	SELECT	@NumCotizacionesMes			AS NumCotizacionesMes
		  , @NumCotizacionesAño			AS NumCotizacionesAño	
		  , @MontoPolPrimNetaMes		AS MontoPolPrimNetaMes	
		  , @MontoPolPrimNetaAño		AS MontoPolPrimNetaAño
		  , @NumRenovacionesMes			AS NumRenovacionesMes	
		  , @NumRenovacionesAño			AS NumRenovacionesAño	
		  , @MontoRenPolPrimNetaMes		AS MontoRenPolPrimNetaMes	
		  , @MontoREnPolPrimNetaAño		AS MontoRenPolPrimNetaAño
		  , @NumEndososMes				AS NumEndososMes
		  , @NumEndososAño				AS NumEndososAño
		  , @MontoEndososMes			AS MontoEndososMes
		  , @MontoEndososAño			AS MontoEndososAño
		  , @UltimoPago					AS UltimoPago
END
