
IF DB_ID(N'VeterinariaDB') IS NULL
    CREATE DATABASE VeterinariaDB;
GO

USE VeterinariaDB;
GO

IF OBJECT_ID(N'dbo.Mascotas', N'U')     IS NOT NULL DROP TABLE dbo.Mascotas;
IF OBJECT_ID(N'dbo.Razas', N'U')        IS NOT NULL DROP TABLE dbo.Razas;
IF OBJECT_ID(N'dbo.Especies', N'U')     IS NOT NULL DROP TABLE dbo.Especies;
IF OBJECT_ID(N'dbo.Propietarios', N'U') IS NOT NULL DROP TABLE dbo.Propietarios;
GO


CREATE TABLE dbo.Propietarios (
    Id          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Propietarios PRIMARY KEY,
    Nombre      VARCHAR(50)  NOT NULL,
    Apellido    VARCHAR(50)  NOT NULL,
    Telefono    VARCHAR(20)  NULL,
    Email       VARCHAR(100) NULL,
    Direccion   VARCHAR(150) NULL
);
GO

CREATE TABLE dbo.Especies (
    Id      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Especies PRIMARY KEY,
    Nombre  VARCHAR(50) NOT NULL
);
GO

CREATE TABLE dbo.Razas (
    Id          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Razas PRIMARY KEY,
    IdEspecie   INT         NOT NULL,
    Nombre      VARCHAR(50) NOT NULL,
    CONSTRAINT FK_Razas_Especies_IdEspecie
        FOREIGN KEY (IdEspecie) REFERENCES dbo.Especies (Id)
);
GO

CREATE TABLE dbo.Mascotas (
    Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Mascotas PRIMARY KEY,
    Nombre          VARCHAR(50)  NOT NULL,
    IdPropietario   INT          NOT NULL,
    IdRaza          INT          NOT NULL,
    FechaNacimiento DATE         NULL,
    Peso            DECIMAL(5,2) NULL,
    RutaFoto        VARCHAR(255) NULL,
    CONSTRAINT FK_Mascotas_Propietarios_IdPropietario
        FOREIGN KEY (IdPropietario) REFERENCES dbo.Propietarios (Id),
    CONSTRAINT FK_Mascotas_Razas_IdRaza
        FOREIGN KEY (IdRaza) REFERENCES dbo.Razas (Id)
);
GO

CREATE INDEX IX_Razas_IdEspecie        ON dbo.Razas (IdEspecie);
CREATE INDEX IX_Mascotas_IdPropietario ON dbo.Mascotas (IdPropietario);
CREATE INDEX IX_Mascotas_IdRaza        ON dbo.Mascotas (IdRaza);
GO



SET IDENTITY_INSERT dbo.Especies ON;
INSERT INTO dbo.Especies (Id, Nombre) VALUES
    (1, 'Canino'),
    (2, 'Felino'),
    (3, 'Ave');
SET IDENTITY_INSERT dbo.Especies OFF;
GO

SET IDENTITY_INSERT dbo.Razas ON;
INSERT INTO dbo.Razas (Id, IdEspecie, Nombre) VALUES
    (1, 1, 'Labrador Retriever'),
    (2, 1, 'Pastor Alemán'),
    (3, 1, 'Bulldog Francés'),
    (4, 1, 'Criollo'),
    (5, 2, 'Siamés'),
    (6, 2, 'Persa'),
    (7, 2, 'Angora'),
    (8, 3, 'Periquito'),
    (9, 3, 'Canario');
SET IDENTITY_INSERT dbo.Razas OFF;
GO



