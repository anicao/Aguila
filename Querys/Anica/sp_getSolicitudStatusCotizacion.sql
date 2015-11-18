create procedure sp_getSolicitudStatusCotizacion (
  @nsolicitudId numeric 
)as
begin
select CAST(co.ncotizaID AS BIGINT)[cotizacioId],
     co.nSolicitudID [solicitudId],
     CAST(po.nPoliza AS BIGINT) [polizaId],
     so.cstafinal [statusFinal],
     CAST(SUM(pag.mMonto) AS DECIMAL(10,2)) [montoPagado]
  from cotizacion co 
  inner join polizas po on co.nSolicitudID=po.nSolicitudID
  inner join solicitudes so on so.nsolicitudId=po.nSolicitudID
  inner join Cobranza.dbo.Pagos pag  on pag.nSolicitudID=po.nSolicitudID
  where so.nsolicitudid=@nsolicitudId
  group by  co.ncotizaID,
     co.nSolicitudID,
     po.nPoliza,
     so.cstafinal
end