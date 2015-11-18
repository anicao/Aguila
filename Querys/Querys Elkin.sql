
--INSERT INTO [dbo].[Complement]([nCotizaID],[dFProxLlam],[cCompañia],[cInteres],[cGusto],[cTelefono],[cObserv],[precio_aguila],[precio_comp],[cObservGrales],[cObservProcar],[mPrimNeta])VALUES( 1169977 ,'02/08/2015','0','XX','OTR','','','0','0','','','0')



--Select * from dbo.catCiasAnt 0

--------------------------------------------

--INSERT INTO [dbo].[Vehiculos]([nCotizaID],[nContVehi],[cCP],[nSubRamo],[nModeloID],[nAño],[cGaragCasa],[cGaragTrab],[cUsoTrab],[nDedudDM],[nDedudRT],[mCobertGM],[mCobertRC],[cDefAseLeg],[cSumAsegF],[nCondAsig],[mSumAEqEsp],[mValorAuto],[cTpoEspAut],[nPuertas],[nCilindros],[nTransmi],[cNumSerie],[cPlacas],[cTpoAlarma],[cSUVA],[cVehSus],[cApoyo100],[cCober100],[cDesEqEsp],[cNCI],[cEBC],[nPagina],[cTersa],[mCobertRCCat],[cValidaSerie],[cAutorizaSerie],[cAsistVje],[cVIN],[cExcenDedu])VALUES(@IDCotizacion,1,'2','R',1478,2002,'S','S','N',5,10,100000,500000,'S','N',1,0,94000,'ESCALADE 4X2 AUT',0,0,0,'','','A','N','N','N','N','','','',null,'S',0,'N','','R','','N')

--INSERT INTO [dbo].[catTelefonos]([nCotizaID],[nPoliza],[nReprocesoID],[cLada],[cTel],[cOficina],[cExtension],[cCelular],[cFax],[cRadioID],[cRadio],[cOtro1],[cTpo1],[cHistorial],[cUsuario]) VALUES(@IDCotizacion,null,null,'','','','','','','','','','','','mromero')

--INSERT INTO [dbo].[Complement]([nCotizaID],[dFProxLlam],[cCompañia],[cInteres],[cGusto],[cTelefono],[cObserv],[precio_aguila],[precio_comp],[cObservGrales],[cObservProcar],[mPrimNeta])VALUES( @IDCotizacion ,null,'0','COB','AHO','1','DAS','35644','0','','','30128')



--sELECT * FROM CATCLIENTES
--sELECT * FROM CATpromocion

--Select * From catEBC order by nguia

--Select top 10 * from vehiculos  where cEBC != '' order by ncotizaid desc

--execute sp_Sel_ModAño '062015'


--Select * From [Login].[dbo].[catUsuarios] Where cUsuario = 'ezambrano'


--execute Sp_Sel_Territorio

--Select top 10 * from dbo.cotizacion
--Select  * from dbo.catcamps where ncampid=117
--execute Sp_Sel_Campañas
--execute [dbo].[sp_Sel_CampañasTodas]
--Select * from dbo.renovaciones Where 



--SELECT * FROM [dbo].[Cotizacion]
--WHERE nCotizaID = 1170716


--SELECT TOP 1 * FROM [dbo].[Cotizacion]
--ORDER BY nCotizaID DESC


-------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------------

Select top 100 *
		From Produccion.dbo.Polizas
			Where Year(dfIniVig) = 2015
			order by 1 desc


Declare @lTotalCount int

EXECUTE [dbo].[sp_ServiciosEnLineaPaginado]
  @pPolizaID		= 11100276792
, @pSolicitudID		= 1227237
, @PageNo		    = 1
, @RecordsPerPage   = 100
, @TotalCount		= @lTotalCount OUTPUT

Select @lTotalCount


Select * from dbtools.[dbo].[catKeyFactura]

--update dbtools.[dbo].[catKeyFactura] set cRFC = 'ASE941124NN4', urlWSFactura = 'http://200.52.84.164/xsamanager/downloadCfdWebView?'



	Select  p.nPoliza, p.nSolicitudID, Count(*)
		From Produccion.dbo.Polizas p left outer join dbtools.dbo.FacturaElectronica f on p.nPoliza = f.nPoliza
			Group By p.nPoliza, p.nSolicitudID
				Having Count(*) > 3



