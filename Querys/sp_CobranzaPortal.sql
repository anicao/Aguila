SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

/********************************************************************************************************
 Elaborado por	:	---
 Fecha			: 	31/03/2015
 Proceso		: 	Reporte de Cobranzas via Web
 Descripción	: 	Cobranzas
*********************************************************************************************************
    Analista	Fecha		Proceso
*********************************************************************************************************/

ALTER PROCEDURE [dbo].[sp_CobranzaPortal]
  @pFechaIni		Char(10)	  = null
, @pFechaFin		Char(10)	  = null
, @pAgenteID		int		      = 0

As

Set Nocount ON

Begin
	If (@pAgenteID = 0)
	Begin
		SELECT DISTINCT t2.nagenteid
					  , t1.npoliza
					  , t1.nendoso
					  , t1.nrecibo
					  , t2.cnombrea
					  , t1.dfInivig
					  , t1.dfFinvig
					  , (t1.mprimneta + t1.mdchospol + t1.mrecpf + t1.miva + t1.map) AS monto
					  , 'estatus' = CASE WHEN t1.nestatus = 2 THEN 'Pagado'
										 WHEN t1.nestatus = 3 THEN 'Cancelado'
										 ELSE 'No Pagado'	  END
					  , isnull(cs.cDescripcion, '')									 AS seguimiento
					  , t2.cera
					  , t2.cformapago
					  , Case When IsNull(cs.cSegui, '') = 'EPR'		-- En Proceso
							 Then dbo.GeneraReferenciaCIE(t2.nSolicitudID, t1.dfInivig)
							 Else Space(15)
							 End													 As ReferenciaDeposito
			FROM Produccion.dbo.Recibos t1
							Inner Join		Produccion.dbo.Polizas	t2 ON t1.npoliza = t2.npoliza 
																	  And t2.nagenteid <> 0 
																	  And t2.nestatusid NOT IN (2, 3)
							Left Outer Join Seguimiento.dbo.Cobranza c ON c.npoliza = t1.npoliza
																	  And c.nendoso = t1.nendoso
																	  And c.nrecibo = t1.nrecibo 
							Left Outer Join Seguimiento.dbo.catSeguimiento cs ON c.cSegui = cs.cSegui
				WHERE (t1.dfInivig >= @pFechaIni AND t1.dfInivig <= @pFechaFin)
					ORDER BY dfinivig
	End
	Else
	Begin
		SELECT DISTINCT t2.nagenteid
					  , t1.npoliza
					  , t1.nendoso
					  , t1.nrecibo
					  , t2.cnombrea
					  , t1.dfInivig
					  , t1.dfFinvig
					  , (t1.mprimneta + t1.mdchospol + t1.mrecpf + t1.miva + t1.map) AS monto
					  , 'estatus' = CASE WHEN t1.nestatus = 2 THEN 'Pagado'
										 WHEN t1.nestatus = 3 THEN 'Cancelado'
										 ELSE 'No Pagado'	  END
					  , isnull(cs.cDescripcion, '')									 AS seguimiento
					  , t2.cera
					  , t2.cformapago
					  , Case When IsNull(cs.cSegui, '') = 'EPR'		-- En Proceso
							 Then dbo.GeneraReferenciaCIE(t2.nSolicitudID, t1.dfInivig)
							 Else Space(15)
							 End													 As ReferenciaDeposito
			FROM Produccion.dbo.Recibos t1
							Inner Join		Produccion.dbo.Polizas	t2 ON t1.npoliza = t2.npoliza 
																	  And t2.nagenteid <> 0 
																	  And t2.nestatusid NOT IN (2, 3)
							Left Outer Join Seguimiento.dbo.Cobranza c ON c.npoliza = t1.npoliza
																	  And c.nendoso = t1.nendoso
																	  And c.nrecibo = t1.nrecibo 
							Left Outer Join Seguimiento.dbo.catSeguimiento cs ON c.cSegui = cs.cSegui
				WHERE t2.nagenteid = @pAgenteID
				 and (t1.dfInivig >= @pFechaIni AND t1.dfInivig <= @pFechaFin)
					ORDER BY dfinivig
	End
End
GO