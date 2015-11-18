

--COTIZACIONES DEL MES
SELECT        COUNT(nCotizaID) AS Expr1
FROM            Produccion.dbo.Cotizacion
WHERE        (dFRegistro >= '01/07/2015') AND (dFRegistro <= '31/07/2015') AND (nAgenteID = 1)

--COTIZACIONES DE AÑO
SELECT        COUNT(nCotizaID) AS Expr1
FROM            Produccion.dbo.Cotizacion
WHERE        (dFRegistro >= '01/01/2015') AND (dFRegistro <= '31/07/2015') AND (nAgenteID = 1)

--PRIMA NETA EMITIDA DEL MES
select sum(mprimneta) from produccion.dbo.polizas where dfexpedicion >= '01/07/2015' and dfexpedicion <= '31/07/2015' 
and nrenovacion = 0 and nagenteid = 1


--PRIMA NETA EMITIDA EN EL AÑO
select sum(mprimneta) from produccion.dbo.polizas where dfexpedicion >= '01/01/2015' and dfexpedicion <= '31/07/2015' 
and nrenovacion = 0 and nagenteid = 1


--RENOVACIONES DEL MES
SELECT        COUNT(npoliza) AS Expr1
FROM            Produccion.dbo.renovaciones
where dfinivig >='01/07/2015' and dfinivig <= '31/07/2015' and nagenteid = 1

--RENOVACIONES EN EL AÑO
SELECT        COUNT(npoliza) AS Expr1
FROM            Produccion.dbo.renovaciones
where dfinivig >='01/01/2015' and dfinivig <= '31/07/2015' and nagenteid = 1

--PRIMA NETA EMITIDA EN EL MES
select sum(mprimneta) from produccion.dbo.polizas where dfexpedicion >= '01/07/2015' and dfexpedicion <= '31/07/2015' 
and nrenovacion > 0 and nagenteid = 1


--PRIMA NETA EMITIDA EN EL AÑO
select sum(mprimneta) from produccion.dbo.polizas where dfexpedicion >= '01/07/2015' and dfexpedicion <= '31/07/2015' 
and nrenovacion > 0 and nagenteid = 1


--ULTIMO PAGO DE COMISIONES
select max(dfaplica) from cobranza.dbo.comisionagte where nagenteid =1 and cestatus = 'P'


