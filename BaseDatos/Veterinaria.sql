-- Script de base de datos: Clinica Veterinaria (SQL Server / LocalDB)
IF DB_ID('VeterinariaDB') IS NULL CREATE DATABASE VeterinariaDB;
GO
USE VeterinariaDB;
GO
IF OBJECT_ID('dbo.Mascota') IS NOT NULL DROP TABLE dbo.Mascota;
IF OBJECT_ID('dbo.Dueno') IS NOT NULL DROP TABLE dbo.Dueno;
GO
CREATE TABLE dbo.Dueno (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      NVARCHAR(100) NOT NULL,
    Dni         NVARCHAR(8)   NOT NULL UNIQUE,
    Telefono    NVARCHAR(15)  NULL,
    Email       NVARCHAR(100) NULL,
    FechaRegistro DATETIME2   NOT NULL DEFAULT SYSDATETIME()
);
CREATE TABLE dbo.Mascota (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      NVARCHAR(50)  NOT NULL,
    Especie     NVARCHAR(30)  NOT NULL,
    Raza        NVARCHAR(50)  NULL,
    Edad        INT           NOT NULL CHECK (Edad BETWEEN 0 AND 40),
    Peso        DECIMAL(6,2)  NOT NULL CHECK (Peso > 0),
    Activo      BIT           NOT NULL DEFAULT 1,
    DuenoId     INT           NOT NULL,
    CONSTRAINT FK_Mascota_Dueno FOREIGN KEY (DuenoId) REFERENCES dbo.Dueno(Id)
);
GO
INSERT INTO dbo.Dueno (Nombre, Dni, Telefono, Email) VALUES
 (N'Carlos Ramirez', '45678912', '987654321', 'carlos@mail.com'),
 (N'Lucia Fernandez','71234567', '912345678', 'lucia@mail.com'),
 (N'Miguel Torres',  '40111222', NULL,        NULL);
INSERT INTO dbo.Mascota (Nombre, Especie, Raza, Edad, Peso, DuenoId) VALUES
 (N'Firulais','Perro', N'Labrador', 5, 28.50, 1),
 (N'Michi',   'Gato',  N'Siames',   3,  4.20, 1),
 (N'Rex',     'Perro', N'Pastor Aleman', 7, 32.00, 2),
 (N'Luna',    'Gato',  NULL,        2,  3.80, 3);
GO
