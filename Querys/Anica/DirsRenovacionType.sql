CREATE TYPE DirsRenovacionType AS TABLE 
(
	nPoliza numeric(9),
	nTipoDirID int,
	cDireccion varchar(150),
	nExterior  varchar(35),
	nInterior  varchar(35),
	cColonia varchar(60),
	cPoblacion varchar(40),
	nEstadoID  int,
	nCP char(5),
	cTel1  varchar(40),
	cOficina1  varchar(20),
	cCelular1  varchar(20),
	cFax1  varchar(40),
	cOtro1 varchar(20),
	cRequerida char(1),
	cFiscal char(1)
)