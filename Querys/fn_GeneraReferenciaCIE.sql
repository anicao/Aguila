ALTER FUNCTION [dbo].[GeneraReferenciaCIE] (
	@pSolicitudID		int
  ,	@pFechaInicio		SmallDateTime
)
Returns VarChar(15)  AS   

Begin
	Declare @lReferenciaCIE			VarChar(25) = ''
		  , @lnValor				int			= 2
		  , @lsValor				VarChar(25) = ''
		  , @lnDigito				int			= 0
		  , @i						int			= 0

	Select @lReferenciaCIE = Substring(Convert(Char(8), DateAdd(d, 5, @pFechaInicio), 112), 3, 8) + lTrim(rTrim(Convert(Char(15), @pSolicitudID)))

	While (@i < Len(@lReferenciaCIE))
	Begin
		If ((Convert(int, Substring(@lReferenciaCIE, (Len(@lReferenciaCIE) - @i), 1)) * @lnValor) > 9)
		Begin
            Select @lsValor  = Convert(VarChar(15), (Convert(int, Substring(@lReferenciaCIE, (Len(@lReferenciaCIE) - @i), 1)) * @lnValor))
			     , @lnDigito = @lnDigito + Convert(int, Substring(@lsValor, 1, 1)) + Convert(int, Substring(@lsValor, 2, 1))

        End
        Else
        Begin
			Select @lnDigito = @lnDigito + (Convert(int, Substring(@lReferenciaCIE, (Len(@lReferenciaCIE) - @i), 1)) * @lnValor)
        End


        Select @lnValor = Case @lnValor When 2 Then 1 Else 2 End
		Select @i = @i + 1
	End

	-- El resultado de la suma se resta a la decena superior mas próxima
	Select @lsValor = Case Len(@lnDigito) When 2 Then Substring(Convert(VarChar(10), @lnDigito), 1, 1) + '0'
												 Else Substring(Convert(VarChar(10), @lnDigito), 1, 2) + '0'
					  End

	-- Devolver la referencia completa mas el digito verificador
	Select @lReferenciaCIE = @lReferenciaCIE 
						   + Case When Abs((Convert(bigInt, @lsValor) + 10) - @lnDigito) > 9 
								  Then '0'
								  Else Convert(VarChar(10), Abs((Convert(bigInt, @lsValor) + 10) - @lnDigito))
							 End
	Return @lReferenciaCIE
End
GO