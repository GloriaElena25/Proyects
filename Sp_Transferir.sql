CREATE PROCEDURE sp_Transferir
    @TelefonoOrigen NVARCHAR(15),
    @TelefonoDestino NVARCHAR(15),
    @Monto DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @IdOrigen INT, @IdDestino INT, @SaldoOrigen DECIMAL(18,2);

        SELECT @IdOrigen = IdUsuario, @SaldoOrigen = Saldo
        FROM Usuarios WHERE Telefono = @TelefonoOrigen;

        IF @IdOrigen IS NULL
        BEGIN
            RAISERROR('Usuario origen no encontrado.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF @SaldoOrigen < @Monto
        BEGIN
            RAISERROR('Saldo insuficiente.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        SELECT @IdDestino = IdUsuario
        FROM Usuarios WHERE Telefono = @TelefonoDestino;

        IF @IdDestino IS NULL
        BEGIN
            RAISERROR('Usuario destino no encontrado.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        UPDATE Usuarios SET Saldo = Saldo - @Monto WHERE IdUsuario = @IdOrigen;
        UPDATE Usuarios SET Saldo = Saldo + @Monto WHERE IdUsuario = @IdDestino;

        INSERT INTO Transacciones (IdOrigen, IdDestino, Monto)
        VALUES (@IdOrigen, @IdDestino, @Monto);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END;
