﻿CREATE PROC FI_SP_ConsBeneficiarioPorCliente
	@IDCLIENTE BIGINT
AS
BEGIN
	SELECT NOME, CPF, IDCLIENTE, ID FROM BENEFICIARIOS WITH(NOLOCK) WHERE @IDCLIENTE = @IDCLIENTE
END