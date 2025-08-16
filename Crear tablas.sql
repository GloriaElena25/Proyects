CREATE DATABASE BinPrueba;
GO

USE BinPrueba;
GO

CREATE TABLE Usuarios (
    IdUsuario INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(100) NOT NULL,
    Telefono NVARCHAR(15) NOT NULL UNIQUE,
    Saldo DECIMAL(18,2) NOT NULL DEFAULT 0
);

CREATE TABLE Transacciones (
    IdTransaccion INT PRIMARY KEY IDENTITY,
    IdOrigen INT NOT NULL,
    IdDestino INT NOT NULL,
    Monto DECIMAL(18,2) NOT NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (IdOrigen) REFERENCES Usuarios(IdUsuario),
    FOREIGN KEY (IdDestino) REFERENCES Usuarios(IdUsuario)
);

INSERT INTO Usuarios (Nombre, Telefono, Saldo)
VALUES ('Juan Pérez', '88888888', 500),
       ('María López', '77777777', 300),
       ('Carlos Ruiz', '66666666', 1000);
	   ('Gloria Ortega', '85935832', 1000);
	   ('Gloria Ortega Claro', '83538171', 1000);
