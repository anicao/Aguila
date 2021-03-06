USE [Sistemas]
GO
SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_CambioSistemas] 
  @pTipoDocumento	VarChar(20)
, @pID				BigInt
, @pUserName		VarChar(10)
, @pCambio			Int 
, @pMotivo			VarChar(400)
, @pObservaciones	VarChar(400)


AS

Declare @lStrID  VarChar(20)

Begin Tran
	Select 	@lStrID = Convert(VarChar(20), @pID)

	-- Inserto el log
	INSERT INTO [dbo].[Cambios]
			   ( [Fecha]
			   , [Poliza]
			   , [Endoso]
			   , [Recibo]
			   , [Solicitud]
			   , [Siniestro]
			   , [Usuario]
			   , [Cambio]
			   , [Razon]
			   , [Sistemas]
			   , [Observaciones]
			   , [Terminado]
			   , [Aviso]
			   , [nPagoID])
		 VALUES
			   ( GetDate()
			   , Case When @pTipoDocumento = 'POLIZA'	 Then @pID					Else Null End
			   , Case When @pTipoDocumento = 'ENDOSO'	 Then Convert(int, @pID)	Else Null End
			   , Case When @pTipoDocumento = 'RECIBO'	 Then Convert(int, @pID)	Else Null End
			   , Case When @pTipoDocumento = 'SOLICITUD' Then @pID					Else Null End
			   , Case When @pTipoDocumento = 'SINIESTRO' Then @pID					Else Null End
			   , @pUserName
			   , @pCambio
			   , @pMotivo
			   , null
			   , @pObservaciones
			   , null
			   , null
			   , null)
	If @@Error<>0
	Begin
		RaisError('Problemas insertando la auditoria de cambios. Tipo de Documento: %s. ID: %s',16, 1,@pTipoDocumento, @lStrID)
		GoTo On_Error
	End
Commit Tran
Return
------------------------
-- Control de Errores
------------------------
On_Error:
    Rollback Tran
Return
GO

