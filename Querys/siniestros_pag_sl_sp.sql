/********************************************************************************************************

*********************************************************************************************************/

CREATE PROCEDURE [dbo].[siniestros_pag_sl_sp] 
  @pPolizaID      NUMERIC = null , 
  @nSiniestroID   int =null,
  @PageNo         INT , 
  @RecordsPerPage INT , 
  @TotalCount     INT output 
AS 
  SET nocount ON 
  BEGIN 
    -- Clave de conexión al WS 
	SELECT  @TotalCount = Count(*) 
		from Siniestros.dbo.Siniestros S 
			inner join Siniestros.dbo.Avisos A  on A.nAvisoID = S.nAvisoID 
			inner Join Siniestros.dbo.Vehisiniestrado V on V.nAvisoID = A.nAvisoID
			and V.Npoliza = A.Npoliza 
			inner join Siniestros.dbo.catCausas C on S.nCausaID = C.nCausaID
			LEFT OUTER JOIN siniestros.dbo.estimaciones E on  S.nSiniestroID = E.nSiniestroID and ((S.nCausaID = 7 and  ltrim(rtrim(E.cRiesgo)) = '2') or 
			(S.nCausaID != 7 and  ltrim(rtrim(E.cRiesgo)) = '1'))
		 where (S.nSiniestroID=@nSiniestroID or @nSiniestroID is null) and
		   (S.nPoliza=@pPolizaID or @pPolizaID is null)

	SELECT 
		S.nSiniestroID
		,S.dFSiniestro
		,S.mImporte
		,C.cDescrip
		,S.cInforme
		,A.cConductor
		,V.cTpoEspAut
		,v.cNumSerie
		,v.cplacas
		,v.cinciso 
		,0 AS RT
		,0 AS DM
		,isnull(E.mPago , 0) as Pago, isnull(E.mSaldo , 0) as Saldo
	from Siniestros.dbo.Siniestros S 
			inner join Siniestros.dbo.Avisos A  on A.nAvisoID = S.nAvisoID 
			inner Join Siniestros.dbo.Vehisiniestrado V on V.nAvisoID = A.nAvisoID
			and V.Npoliza = A.Npoliza 
			inner join Siniestros.dbo.catCausas C on S.nCausaID = C.nCausaID
			LEFT OUTER JOIN siniestros.dbo.estimaciones E on  S.nSiniestroID = E.nSiniestroID and ((S.nCausaID = 7 and  ltrim(rtrim(E.cRiesgo)) = '2') or 
			(S.nCausaID != 7 and  ltrim(rtrim(E.cRiesgo)) = '1'))
	 where (S.nSiniestroID=@nSiniestroID or @nSiniestroID is null) and
		   (S.nPoliza=@pPolizaID or @pPolizaID is null)
    ORDER BY S.dFSiniestro desc offset (@PageNo-1) * @RecordsPerPage rows 
    FETCH next @RecordsPerPage rows only 

  END