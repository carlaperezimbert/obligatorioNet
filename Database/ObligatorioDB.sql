-- ============================================================
-- Script de creación de ObligatorioDB + datos de prueba
-- Compatible con SQL Server LocalDB
-- ============================================================

IF DB_ID('ObligatorioDB') IS NULL
BEGIN
    CREATE DATABASE ObligatorioDB;
END
GO

USE ObligatorioDB;
GO

-- ---------- Tablas ----------

IF OBJECT_ID('Clientes', 'U') IS NULL
CREATE TABLE Clientes (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    RazonSocial  NVARCHAR(200) NOT NULL,
    RUT          NVARCHAR(20) NOT NULL UNIQUE,
    Direccion    NVARCHAR(300) NOT NULL
);
GO

IF OBJECT_ID('Proyectos', 'U') IS NULL
CREATE TABLE Proyectos (
    Id                   INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId            INT NOT NULL,
    Nombre               NVARCHAR(200) NOT NULL,
    Tipo                 INT NOT NULL,          -- 0=PorHora, 1=MontoMensual, 2=PrecioFijo
    FechaInicio          DATETIME NOT NULL,
    PresupuestoInicial   DECIMAL(18,2) NOT NULL,
    FechaFinPlanificada  DATETIME NOT NULL,
    FechaFin             DATETIME NULL,
    Estado               INT NOT NULL,          -- 0=Planificado, 1=EnCurso, 2=Finalizado, 3=Cancelado
    CONSTRAINT FK_Proyectos_Cliente FOREIGN KEY (ClienteId)
        REFERENCES Clientes(Id)
);
GO

IF OBJECT_ID('FacturasVenta', 'U') IS NULL
CREATE TABLE FacturasVenta (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    NroFactura  NVARCHAR(20) NOT NULL UNIQUE,
    Fecha       DATETIME NOT NULL,
    ClienteId   INT NOT NULL,
    ProyectoId  INT NULL,
    SubTotal    DECIMAL(18,2) NOT NULL,
    IVA         DECIMAL(18,2) NOT NULL,
    MontoTotal  DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_FacturasVenta_Cliente FOREIGN KEY (ClienteId)
        REFERENCES Clientes(Id),
    CONSTRAINT FK_FacturasVenta_Proyecto FOREIGN KEY (ProyectoId)
        REFERENCES Proyectos(Id)
);
GO

IF OBJECT_ID('DetallesFacturaVenta', 'U') IS NULL
CREATE TABLE DetallesFacturaVenta (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    FacturaVentaId   INT NOT NULL,
    Descripcion      NVARCHAR(200) NOT NULL,
    Monto            DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_Detalles_FacturaVenta FOREIGN KEY (FacturaVentaId)
        REFERENCES FacturasVenta(Id) ON DELETE CASCADE
);
GO

IF OBJECT_ID('FacturasCompra', 'U') IS NULL
CREATE TABLE FacturasCompra (
    Id                   INT IDENTITY(1,1) PRIMARY KEY,
    NroFactura           NVARCHAR(20) NOT NULL UNIQUE,
    Fecha                DATETIME NOT NULL,
    ProveedorRazonSocial NVARCHAR(200) NOT NULL,
    ProveedorRUT         NVARCHAR(20) NOT NULL,
    ProveedorDireccion   NVARCHAR(300) NOT NULL,
    SubTotal             DECIMAL(18,2) NOT NULL,
    IVA                  DECIMAL(18,2) NOT NULL,
    MontoTotal           DECIMAL(18,2) NOT NULL
);
GO

IF OBJECT_ID('LiquidacionesImpuestos', 'U') IS NULL
CREATE TABLE LiquidacionesImpuestos (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    Mes              INT NOT NULL,
    Anio             INT NOT NULL,
    FechaCalculo     DATETIME NOT NULL,
    TotalFacturado   DECIMAL(18,2) NOT NULL,
    TotalIvaVentas   DECIMAL(18,2) NOT NULL,
    TotalIvaCompras  DECIMAL(18,2) NOT NULL,
    MontoIva         DECIMAL(18,2) NOT NULL,
    MontoIrae        DECIMAL(18,2) NOT NULL,
    CONSTRAINT UQ_Liquidacion_Periodo UNIQUE (Mes, Anio)
);
GO

-- ---------- Datos de prueba ----------

-- Clientes (RUT de 12 dígitos, como exige la validación de la lógica)
IF NOT EXISTS (SELECT 1 FROM Clientes)
BEGIN
    INSERT INTO Clientes (RazonSocial, RUT, Direccion) VALUES
    ('Estudio Contable García & Asociados', '210987654321', 'Colonia 1234, Montevideo'),
    ('Inmobiliaria del Plata S.A.',        '211234567890', 'Av. 18 de Julio 2450, Montevideo'),
    ('Restaurante La Esquina',             '212345678901', 'Mercado del Puerto, Local 15, Montevideo'),
    ('Clínica Dental Sonrisa',             '213456789012', 'Bv. Artigas 780, Paysandú');
END
GO

-- Proyectos
IF NOT EXISTS (SELECT 1 FROM Proyectos)
BEGIN
    INSERT INTO Proyectos (ClienteId, Nombre, Tipo, FechaInicio, PresupuestoInicial, FechaFinPlanificada, FechaFin, Estado) VALUES
    (1, 'Liquidación impuestos 2025',      1, '2025-01-10', 48000.00, '2025-12-20', '2025-12-18', 2),
    (2, 'Web corporativa inmobiliaria',    2, '2025-03-01', 120000.00, '2025-08-31', NULL,         1),
    (3, 'Sistema de reservas',             0, '2025-06-15', 96000.00,  '2025-11-30', NULL,         1),
    (4, 'Mantenimiento software dental',   1, '2025-09-01', 36000.00,  '2026-08-31', NULL,         0);
END
GO

-- Facturas de venta del mes actual (para probar liquidación)
DECLARE @ClienteId INT = 1;
DECLARE @ProyectoId INT = NULL;

IF NOT EXISTS (SELECT 1 FROM FacturasVenta WHERE NroFactura = 'V-0001')
BEGIN
    INSERT INTO FacturasVenta (NroFactura, Fecha, ClienteId, ProyectoId, SubTotal, IVA, MontoTotal)
    VALUES ('V-0001', DATEADD(DAY, -20, GETDATE()), @ClienteId, @ProyectoId, 10000.00, 2200.00, 12200.00);

    INSERT INTO DetallesFacturaVenta (FacturaVentaId, Descripcion, Monto)
    SELECT SCOPE_IDENTITY(), 'Honorarios mensuales - noviembre', 10000.00;
END
GO

IF NOT EXISTS (SELECT 1 FROM FacturasVenta WHERE NroFactura = 'V-0002')
BEGIN
    DECLARE @FacturaVentaId INT;

    INSERT INTO FacturasVenta (NroFactura, Fecha, ClienteId, ProyectoId, SubTotal, IVA, MontoTotal)
    VALUES ('V-0002', DATEADD(DAY, -10, GETDATE()), 2, 2, 25000.00, 5500.00, 30500.00);
    SET @FacturaVentaId = SCOPE_IDENTITY();

    INSERT INTO DetallesFacturaVenta (FacturaVentaId, Descripcion, Monto) VALUES
    (@FacturaVentaId, 'Desarrollo web - etapa 1', 18000.00),
    (@FacturaVentaId, 'Hosting y dominio anual', 7000.00);
END
GO

-- Facturas de compra del mes actual
IF NOT EXISTS (SELECT 1 FROM FacturasCompra WHERE NroFactura = 'C-0001')
BEGIN
    INSERT INTO FacturasCompra (NroFactura, Fecha, ProveedorRazonSocial, ProveedorRUT, ProveedorDireccion, SubTotal, IVA, MontoTotal)
    VALUES ('C-0001', DATEADD(DAY, -15, GETDATE()), 'Proveedor de Software SA', '219876543210',
            'Av. Italia 1500, Montevideo', 5000.00, 1100.00, 6100.00);
END
GO

IF NOT EXISTS (SELECT 1 FROM FacturasCompra WHERE NroFactura = 'C-0002')
BEGIN
    INSERT INTO FacturasCompra (NroFactura, Fecha, ProveedorRazonSocial, ProveedorRUT, ProveedorDireccion, SubTotal, IVA, MontoTotal)
    VALUES ('C-0002', DATEADD(DAY, -5, GETDATE()), 'Ofimática Central', '201234567893',
            '25 de Mayo 450, Montevideo', 3000.00, 660.00, 3660.00);
END
GO

-- Liquidación de prueba de un mes anterior (octubre 2026)
IF NOT EXISTS (SELECT 1 FROM LiquidacionesImpuestos WHERE Mes = 10 AND Anio = 2026)
BEGIN
    INSERT INTO LiquidacionesImpuestos (Mes, Anio, FechaCalculo, TotalFacturado, TotalIvaVentas, TotalIvaCompras, MontoIva, MontoIrae)
    VALUES (10, 2026, '2026-10-31 23:59:00', 70000.00, 15400.00, 3300.00, 12100.00, 8400.00);
END
GO

PRINT 'Base ObligatorioDB creada con datos de prueba.';
GO
