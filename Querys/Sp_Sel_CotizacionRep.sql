USE [Produccion]
GO
/****** Object:  StoredProcedure [dbo].[sp_Sel_CotizacionRep]    Script Date: 14/07/2015 04:30:25 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[sp_Sel_CotizacionRep]    
  
@cotizacion varchar (10)  
  
AS  

Declare @img	VarBinary(max)
 
SELECT  
    Cotizacion.nCotizaID,  
    Cotizacion.cAPaternoC,  
    Cotizacion.cAMaternoC,  
    Cotizacion.cNombresC,  
    Cotizacion.cCondRest,  
    Cotizacion.nMeses,  
    Cotizacion.nFormaPago,  
    Cotizacion.cTpoPago,  
    Cotizacion.nAgenteID,  
    Cotizacion.cResponsa,  
    catResposa.cNomResp,  
    catResposa.cTelefono,  
    Cotizacion.cera,  
    Cotizacion.ncontrol,
    cotizacion.cap ,
	catTelefonos.cTel, 
	catTelefonos.cFax,
	Cotizacion.cPeriodo,
	Cotizacion.dfCotiza
FROM  
Produccion.dbo.Cotizacion Cotizacion 
INNER JOIN Produccion.dbo.catResposa catResposa ON Cotizacion.cResponsa = catResposa.cResponsa  
Left outer  JOIN Produccion.dbo.catTelefonos catTelefonos   ON Cotizacion.nCotizaID = catTelefonos.nCotizaID   
where Cotizacion.nCotizaID = @cotizacion

/*
    (Produccion.dbo.DirsCotizacion DirsCotizacion   
FULL OUTER JOIN Produccion.dbo.Cotizacion Cotizacion   
ON DirsCotizacion.nCotizaID = Cotizacion.nCotizaID)   
*/
