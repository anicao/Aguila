CREATE TYPE SeguimientoType AS TABLE 
(
	nPoliza numeric(9),
	dFULlamada smalldatetime,
	dFProxLlamada smalldatetime,
	dHHora smalldatetime,
	nNumLlamadas int,
	cObsUtlLlam varchar(250),
	cObserv varchar(250),
	cObsFact varchar(250),
	cEstSeg char(3),
	cObservGrales varchar(150),
	cObservProcar varchar(150)
)