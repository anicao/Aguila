USE [Produccion]
GO
/****** Object:  StoredProcedure [dbo].[sp_Sel_FormasPagoRep]    Script Date: 29/07/2015 10:48:27 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_Sel_FormasPagoRep]   
@Cotizacion NUMERIC(18) 
  
AS  
  
SELECT
	CC.nCotizaID,
	CC.nOrdenCobertura,  
    CC.Cobertura,
	CC.nFormaPago,  
    CC.desFormaPago,  
    CC.PagoIni,  
    CC.PagoSub,  
    CC.Total
FROM  
    Produccion.dbo.CotizacionesCoberturas CC  
WHERE  
    CC.nCotizaID = @Cotizacion
ORDER BY 	  
	nOrdenCobertura	,nFormaPago