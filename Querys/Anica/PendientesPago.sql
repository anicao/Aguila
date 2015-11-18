
create PROCEDURE [dbo].[pendientesPago_sl_sp] 
  @pPolizaID      NUMERIC = null , 
  @PageNo         INT , 
  @RecordsPerPage INT , 
  @TotalCount     INT output 
AS 
  SET nocount ON 
  BEGIN 
    -- Clave de conexión al WS 
	SELECT  @TotalCount = Count(*) 
		from recibos 
		where npoliza= @pPolizaID and nestatus=0

		select  re.nEndoso [endoso],
				re.nrecibo [recibo],
				re.dFIniVig [vigenciaInicio],
				re.dFIniVig [vigenciaFin],
				re.mprimNeta + re.mdchospol + re.mrecpf + re.miva [total] 
		from recibos re with(nolock) 
		where re.npoliza= @pPolizaID and nestatus=0
			ORDER BY re.dFIniVig asc offset (@PageNo-1) * @RecordsPerPage rows 
			FETCH next @RecordsPerPage rows only 

  END