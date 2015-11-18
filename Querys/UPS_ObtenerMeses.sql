USE [Produccion]
GO
/****** Object:  StoredProcedure [dbo].[sp_ObtenerMes]    Script Date: 26/06/2015 07:07:09 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/********************************************************************************************************
 Elaborado por	:	---
 Fecha			: 	26/06/2015
 Proceso		: 	Consulta de Meses para Consulta de Cotizaciones Web
 Descripción	: 	Portal Web

*********************************************************************************************************
    Analista	Fecha		Proceso
*********************************************************************************************************/

ALTER PROCEDURE [dbo].[sp_ObtenerMeses] 
AS
DECLARE @Contador INT

SET @Contador = 0

CREATE TABLE #TabMes
(
	IdMes Int,
	Mes Varchar(50)
)

WHILE @Contador < 12
BEGIN
	INSERT INTO #TabMes
		Values (
			MONTH(DATEADD(M , - @Contador , GETDATE())) , 
			DATENAME(MONTH, DATEADD(M , - @Contador , GETDATE()))
			)
	SET @Contador =  @Contador + 1
	PRINT @Contador
END


SELECT * FROM #TabMes

DROP TABLE #TabMes