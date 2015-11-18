create procedure sp_logUsuarioWeb (
	@nAgente int,
	@cPassw varchar(30)
)as
begin
	begin try

		SELECT *, 
				case 
					when noficinaid = 1 then 388  
					when noficinaid = 2 then 353   
					when noficinaid = 3 then 366   
					when noficinaid = 4 then 537   
					when noficinaid = 5 then 1026  
					when noficinaid = 6 then 1565 else 388  
			end as campaña from catagentes where nagenteid=@nAgente and cPasww=@cPassw and cEstatus='A'
	end try
	begin catch 
		select ERROR_MESSAGE();
	end catch
end



select * from catagentes  where nagenteid=2362
update catagentes set cEstatus='A' where nagenteid=2362
2362
update catagentes set cpasww='agente1' where nagenteid=1

alter table catagentes
	alter column cpasww varchar(30)

AGENTE1159 agente1159

select * from catagentes

update catagentes set cpasww='a' where nagenteid=4

