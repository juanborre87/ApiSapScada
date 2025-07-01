-- ==============================================
-- Script limpio y legible para SAPSCADA Database
-- Fecha: 2025-06-30
-- ==============================================
USE [SAPSCADA2]
GO

CREATE TABLE dbo.CommStatus (
    Id TINYINT NOT NULL PRIMARY KEY,
    Description NVARCHAR(50) NOT NULL
);

CREATE TABLE dbo.ProcessOrderStatus (
    Id TINYINT NOT NULL PRIMARY KEY,
    Description NVARCHAR(50) NOT NULL
);

CREATE TABLE dbo.Product (
    Id BIGINT IDENTITY(1,1),
    Product NVARCHAR(50) NOT NULL PRIMARY KEY,
    ProductDescription NVARCHAR(255),
    ProductType NVARCHAR(100)
);

CREATE TABLE dbo.ProcessOrder (
	Id BIGINT IDENTITY(1,1) PRIMARY KEY,
	ManufacturingOrder NVARCHAR(50) NOT NULL UNIQUE,
    ManufacturingOrderCategory NVARCHAR(50),
    ManufacturingOrderType NVARCHAR(50),
    OrderLongText NVARCHAR(MAX),
    ManufacturingOrderImportance INT,
    MfgOrderCreationDateTime DATETIME,
    LastChangeDateTime DATETIME,
    Material NVARCHAR(50),
    StorageLocation NVARCHAR(50),
    GoodsRecipientName NVARCHAR(50),
    UnloadingPointName NVARCHAR(50),
    ProductionPlant NVARCHAR(50),
    Plant NVARCHAR(50),
    ProductionSupervisor NVARCHAR(50),
    ProductionVersion NVARCHAR(50),
    MfgOrderPlannedStartDateTime DATETIME,
    MfgOrderPlannedEndDateTime DATETIME,
    MfgOrderScheduledStartDateTime DATETIME,
    MfgOrderScheduledEndDateTime DATETIME,
    MfgOrderActualReleaseDateTime DATETIME,
    ProductionUnit NVARCHAR(50),
    ProductionUnitISOCode NVARCHAR(50),
    ProductionUnitSAPCode NVARCHAR(50),
    TotalQuantity REAL,
    MfgOrderPlannedScrapQty REAL,
    MfgOrderConfirmedYieldQty REAL,
    Status TINYINT,
    CommStatus TINYINT NOT NULL DEFAULT(0),
    InterfaceTimestamp DATETIME,
    CONSTRAINT FK_ProcessOrder_CommStatus 
		FOREIGN KEY (CommStatus) REFERENCES dbo.CommStatus(Id),
    CONSTRAINT FK_ProcessOrder_Status 
		FOREIGN KEY (Status) REFERENCES dbo.ProcessOrderStatus(Id),
    CONSTRAINT FK_ProcessOrder_Product 
		FOREIGN KEY (Material) REFERENCES dbo.Product(Product)
);

CREATE TABLE dbo.ProcessOrderComponent (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ManufacturingOrder NVARCHAR(50) NOT NULL,
    Material NVARCHAR(50),
    Reservation NVARCHAR(50),
    ReservationItem NVARCHAR(50),
    MatlCompRequirementDateTime DATETIME,
    StorageLocation NVARCHAR(50),
    Batch NVARCHAR(50),
    GoodsMovementType NVARCHAR(50),
    GoodsRecipientName NVARCHAR(50),
    UnloadingPointName NVARCHAR(50),
    EntryUnit NVARCHAR(50),
    EntryUnitISOCode NVARCHAR(50),
    EntryUnitSAPCode NVARCHAR(50),
    GoodsMovementEntryQty REAL,
    LastChangeDateTime DATETIME,
    CONSTRAINT FK_ProcessOrderComponent_ProcessOrder 
		FOREIGN KEY (ManufacturingOrder) REFERENCES dbo.ProcessOrder(ManufacturingOrder),
    CONSTRAINT FK_ProcessOrderComponent_Product 
		FOREIGN KEY (Material) REFERENCES dbo.Product(Product)
);

CREATE TABLE dbo.ProcessOrderConfirmation (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    OrderId NVARCHAR(50) NOT NULL,
    ConfirmationText NVARCHAR(MAX),
    FinalConfirmationType NVARCHAR(50),
    IsFinalConfirmation BIT,
    ConfirmationEntryDateTime DATETIME,
    EnteredByUser NVARCHAR(255),
    Plant NVARCHAR(50),
    WorkCenter NVARCHAR(50),
    Personnel NVARCHAR(50),
    PostingDate DATETIME,
    ConfirmationUnit NVARCHAR(50),
    ConfirmationUnitISOCode NVARCHAR(50),
    ConfirmationUnitSAPCode NVARCHAR(50),
    ConfirmationYieldQuantity REAL,
    ConfirmationScrapQuantity REAL,
    VarianceReasonCode NVARCHAR(50),
    Batch NVARCHAR(50),
    Expiration DATETIME,
    SAPResponse TINYINT,
    CommStatus TINYINT NOT NULL DEFAULT(0),
    InterfaceTimestamp DATETIME,
    CONSTRAINT FK_ProcessOrderConfirmation_CommStatus 
		FOREIGN KEY (CommStatus) REFERENCES dbo.CommStatus(Id),
    CONSTRAINT FK_ProcessOrderConfirmation_ProcessOrder 
		FOREIGN KEY (OrderId) REFERENCES dbo.ProcessOrder(ManufacturingOrder)
);

CREATE TABLE dbo.ProcessOrderConfirmationMaterialMovement (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ProcessOrderConfirmationId BIGINT NOT NULL,
    ProcessOrderComponentId BIGINT NOT NULL,
    EntryUnit NVARCHAR(50),
    EntryUnitISOCode NVARCHAR(50),
    EntryUnitSAPCode NVARCHAR(50),
    QuantityInEntryUnit REAL,
    GoodsMovementDateTime DATETIME,
    InterfaceTimestamp DATETIME,
    CONSTRAINT FK_ProcessOrderConfirmationMaterialMovement_ProcessOrderConfirmation 
		FOREIGN KEY (ProcessOrderConfirmationId) REFERENCES dbo.ProcessOrderConfirmation(Id),
    CONSTRAINT FK_ProcessOrderConfirmationMaterialMovement_ProcessOrderComponent 
		FOREIGN KEY (ProcessOrderComponentId) REFERENCES dbo.ProcessOrderComponent(Id)
);

INSERT [dbo].[CommStatus] ([Id], [Description]) VALUES (0, N'NotReady')
INSERT [dbo].[CommStatus] ([Id], [Description]) VALUES (1, N'ReadyToBeTransferred')
INSERT [dbo].[CommStatus] ([Id], [Description]) VALUES (2, N'TransferredSuccessfully')
INSERT [dbo].[CommStatus] ([Id], [Description]) VALUES (3, N'TransferredWithWarnings')
GO
INSERT [dbo].[ProcessOrderStatus] ([Id], [Description]) VALUES (1, N'created')
INSERT [dbo].[ProcessOrderStatus] ([Id], [Description]) VALUES (2, N'released')
INSERT [dbo].[ProcessOrderStatus] ([Id], [Description]) VALUES (3, N'delivered')
INSERT [dbo].[ProcessOrderStatus] ([Id], [Description]) VALUES (4, N'locked')
INSERT [dbo].[ProcessOrderStatus] ([Id], [Description]) VALUES (5, N'cancelled')
INSERT [dbo].[ProcessOrderStatus] ([Id], [Description]) VALUES (6, N'closed')
GO