--SELECT P.nPoliza,  isnull( F.cnombrea, P.cnombrea) as asegurado,  E.cDescrip,SUBSTRING(convert(varchar(11),P.npoliza),7,5) as Consecutivo,
--P.dFExpedicion,  p.nNumCond , p.nNumVehi, p.cCondRest, p.cResponsa, p.nCampID, p.nClienteID, p.nPolAP ,p.npolturista 
-- FROM Polizas P WITH(NOLOCK) 
--  inner Join catEstatusPol E WITH(NOLOCK) on P.nEstatusID = E.nEstatusID  
--  left outer join Endosos  F WITH(NOLOCK)  on P.NPOLIZA = F.NPOLIZA and F.nEndoso = 
--   (select max(nEndoso) from Endosos WITH(NOLOCK)  where P.NPOLIZA = NPOLIZA)  
--   WHERE  P.dFExpedicion BETWEEN '01-01-2015' and  '31-12-2015'




----USE AdventureWorks2012
--go
--Create Procedure dbo.Sp_Data_Paging
declare
@PageNo int,
@RecordsPerPage int



Select @PageNo		   = 2
	 , @RecordsPerPage = 100
--AS

DECLARE @cSql VarcHar(max)

select @cSql = '
SELECT  P.nPoliza
	  , Isnull(F.cNombreA, P.cNombreA) As Asegurado
	  , E.cDescrip
	  , SUBSTRING(convert(varchar(11), P.nPoliza),7,5) As Consecutivo
	  , P.dFExpedicion
	  ,  p.nNumCond
	  , p.nNumVehi
	  , p.cCondRest
	  , p.cResponsa
	  , p.nCampID
	  , p.nClienteID
	  , p.nPolAP
	  , p.npolturista 
 FROM Polizas P WITH(NOLOCK) 
		Inner Join catEstatusPol E WITH(NOLOCK) On (P.nEstatusID = E.nEstatusID)
		Left Outer Join Endosos  F WITH(NOLOCK) on (P.NPOLIZA    = F.NPOLIZA 
											    and F.nEndoso    = (Select Max(nEndoso)
																		From Endosos WITH(NOLOCK) 
																			Where P.NPOLIZA = NPOLIZA))
	WHERE P.dFExpedicion BETWEEN ''01-01-2015'' and  ''31-12-2015''
		Order By P.nPoliza
			OFFSET ( ' + Convert(Char(3), @PageNo) + ' -1) * ' + Convert(Char(3), @RecordsPerPage) + ' ROWS
				FETCH NEXT ' + Convert(Char(3), @RecordsPerPage) + 'ROWS ONLY
'

--print @cSQL


execute (@cSql)

GO



--Sp_Data_Paging 1,10 --First Page
GO
--Result


------Create Procedure dbo.USP_POlizasDataPaging
------  @PageNo		   int
------, @RecordsPerPage  int

------AS

------SELECT  P.nPoliza										As 'Poliza No'
------	  , Isnull(F.cNombreA, P.cNombreA)					As 'NombreAsegurado'
------	  , E.cDescrip										As 'Estatus'
------	  , SUBSTRING(convert(varchar(11), P.nPoliza),7,5)	As 'No Consecutivo'
------	  , P.dFExpedicion									As 'Fecha Registro'
------	  , p.nNumCond										As 'Conductores'
------	  , p.nNumVehi										As 'Vehiculos'
------	  , p.cCondRest										As 'Conductor de Bajo Riesgo'
------	  , p.cResponsa										As 'Responsable'
------	  , p.nCampID										As 'Compañia'
------	  , p.nClienteID									As 'Cliente'
------	  , p.nPolAP										As 'Póliza AP'
------	  , p.npolturista									As 'Póliza Turista'
------ FROM Polizas P WITH(NOLOCK) 
------		Inner Join catEstatusPol E WITH(NOLOCK) On (P.nEstatusID = E.nEstatusID)
------		Left Outer Join Endosos  F WITH(NOLOCK) on (P.NPOLIZA    = F.NPOLIZA 
------											    and F.nEndoso    = (Select Max(nEndoso)
------																		From Endosos WITH(NOLOCK) 
------																			Where P.NPOLIZA = NPOLIZA))
------	WHERE P.dFExpedicion BETWEEN '01-01-2015' and  '31-12-2015'
------		Order By P.nPoliza
------			OFFSET (@PageNo-1) * @RecordsPerPage ROWS
------				FETCH NEXT @RecordsPerPage ROWS ONLY

------GO



--------Sp_Data_Paging 1,10 --First