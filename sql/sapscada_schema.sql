USE [SAPSCADA]
GO

/****** Object:  Table [dbo].[CommStatus]    Script Date: 30/6/2025 14:19:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CommStatus](
	[StatusId] [tinyint] NOT NULL,
	[StatusDescription] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_CommStatus] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProcessOrder]    Script Date: 30/6/2025 14:19:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessOrder](
	[ManufacturingOrder] [bigint] NOT NULL,
	[ManufacturingOrderCategory] [nvarchar](50) NULL,
	[ManufacturingOrderType] [nvarchar](50) NULL,
	[OrderLongText] [nvarchar](max) NULL,
	[ManufacturingOrderImportance] [int] NULL,
	[MfgOrderCreationDateTime] [datetime] NULL,
	[LastChangeDateTime] [datetime] NULL,
	[Material] [nvarchar](50) NULL,
	[StorageLocation] [nvarchar](50) NULL,
	[GoodsRecipientName] [nvarchar](50) NULL,
	[UnloadingPointName] [nvarchar](50) NULL,
	[ProductionPlant] [nvarchar](50) NULL,
	[Plant] [nvarchar](50) NULL,
	[ProductionSupervisor] [nvarchar](50) NULL,
	[ProductionVersion] [nvarchar](50) NULL,
	[MfgOrderPlannedStartDateTime] [datetime] NULL,
	[MfgOrderPlannedEndDateTime] [datetime] NULL,
	[MfgOrderScheduledStartDateTime] [datetime] NULL,
	[MfgOrderScheduledEndDateTime] [datetime] NULL,
	[MfgOrderActualReleaseDateTime] [datetime] NULL,
	[ProductionUnit] [nvarchar](50) NULL,
	[ProductionUnitISOCode] [nvarchar](50) NULL,
	[ProductionUnitSAPCode] [nvarchar](50) NULL,
	[TotalQuantity] [real] NULL,
	[MfgOrderPlannedScrapQty] [real] NULL,
	[MfgOrderConfirmedYieldQty] [real] NULL,
	[Status] [tinyint] NULL,
	[CommStatus] [tinyint] NOT NULL,
	[InterfaceTimestamp] [datetime] NULL,
 CONSTRAINT [PK_ProcessOrder] PRIMARY KEY CLUSTERED 
(
	[ManufacturingOrder] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProcessOrderComponent]    Script Date: 30/6/2025 14:19:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessOrderComponent](
	[ProcessOrderComponentId] [bigint] NOT NULL,
	[ManufacturingOrder] [bigint] NOT NULL,
	[Material] [nvarchar](50) NULL,
	[Reservation] [bigint] NULL,
	[ReservationItem] [nvarchar](50) NULL,
	[MatlCompRequirementDateTime] [datetime] NULL,
	[StorageLocation] [nvarchar](50) NULL,
	[Batch] [nvarchar](50) NULL,
	[GoodsMovementType] [nvarchar](50) NULL,
	[GoodsRecipientName] [nvarchar](50) NULL,
	[UnloadingPointName] [nvarchar](50) NULL,
	[EntryUnit] [nvarchar](50) NULL,
	[EntryUnitISOCode] [nvarchar](50) NULL,
	[EntryUnitSAPCode] [nvarchar](50) NULL,
	[GoodsMovementEntryQty] [real] NULL,
	[LastChangeDateTime] [datetime] NULL,
 CONSTRAINT [PK_ProcessOrderComponent] PRIMARY KEY CLUSTERED 
(
	[ProcessOrderComponentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProcessOrderConfirmation]    Script Date: 30/6/2025 14:19:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessOrderConfirmation](
	[ProcessOrderConfirmationId] [bigint] NOT NULL,
	[OrderID] [bigint] NOT NULL,
	[ConfirmationText] [nvarchar](max) NULL,
	[FinalConfirmationType] [nvarchar](50) NULL,
	[IsFinalConfirmation] [bit] NULL,
	[ConfirmationEntryDateTime] [datetime] NULL,
	[EnteredByUser] [nvarchar](255) NULL,
	[Plant] [nvarchar](50) NULL,
	[WorkCenter] [nvarchar](50) NULL,
	[Personnel] [nvarchar](50) NULL,
	[PostingDate] [datetime] NULL,
	[ConfirmationUnit] [nvarchar](50) NULL,
	[ConfirmationUnitISOCode] [nvarchar](50) NULL,
	[ConfirmationUnitSAPCode] [nvarchar](50) NULL,
	[ConfirmationYieldQuantity] [real] NULL,
	[ConfirmationScrapQuantity] [real] NULL,
	[VarianceReasonCode] [nvarchar](50) NULL,
	[Batch] [nvarchar](50) NULL,
	[Expiration] [datetime] NULL,
	[SAPResponse] [tinyint] NULL,
	[CommStatus] [tinyint] NOT NULL,
	[InterfaceTimestamp] [datetime] NULL,
 CONSTRAINT [PK_ProcessOrderConfirmation] PRIMARY KEY CLUSTERED 
(
	[ProcessOrderConfirmationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProcessOrderConfirmationMaterialMovement]    Script Date: 30/6/2025 14:19:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessOrderConfirmationMaterialMovement](
	[ProcessOrderConfirmationMaterialMovementId] [bigint] NOT NULL,
	[ProcessOrderConfirmationId] [bigint] NOT NULL,
	[ProcessOrderComponentId] [bigint] NOT NULL,
	[EntryUnit] [nvarchar](50) NULL,
	[EntryUnitISOCode] [nvarchar](50) NULL,
	[EntryUnitSAPCode] [nvarchar](50) NULL,
	[QuantityInEntryUnit] [real] NULL,
	[GoodsMovementDateTime] [datetime] NULL,
	[InterfaceTimestamp] [datetime] NULL,
 CONSTRAINT [PK_ProcessOrderConfirmationMaterialMovement] PRIMARY KEY CLUSTERED 
(
	[ProcessOrderConfirmationMaterialMovementId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProcessOrderStatus]    Script Date: 30/6/2025 14:19:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessOrderStatus](
	[StatusId] [tinyint] NOT NULL,
	[StatusDescription] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ProcessOrderStatus] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 30/6/2025 14:19:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[ProductId] [bigint] NOT NULL,
	[Product] [nvarchar](50) NOT NULL,
	[ProductDescription] [nvarchar](255) NULL,
	[ProductType] [nvarchar](100) NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[Product] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ProcessOrder] ADD  CONSTRAINT [DF_ProcessOrder_CommStatus]  DEFAULT ((0)) FOR [CommStatus]
GO
ALTER TABLE [dbo].[ProcessOrderConfirmation] ADD  CONSTRAINT [DF_ProcessOrderConfirmation_CommStatus]  DEFAULT ((0)) FOR [CommStatus]
GO
ALTER TABLE [dbo].[ProcessOrder]  WITH CHECK ADD  CONSTRAINT [FK_ProcessOrder_CommStatus] FOREIGN KEY([CommStatus])
REFERENCES [dbo].[CommStatus] ([StatusId])
GO
ALTER TABLE [dbo].[ProcessOrder] CHECK CONSTRAINT [FK_ProcessOrder_CommStatus]
GO
ALTER TABLE [dbo].[ProcessOrder]  WITH CHECK ADD  CONSTRAINT [FK_ProcessOrder_ProcessOrderStatus] FOREIGN KEY([Status])
REFERENCES [dbo].[ProcessOrderStatus] ([StatusId])
GO
ALTER TABLE [dbo].[ProcessOrder] CHECK CONSTRAINT [FK_ProcessOrder_ProcessOrderStatus]
GO
ALTER TABLE [dbo].[ProcessOrder]  WITH NOCHECK ADD  CONSTRAINT [FK_ProcessOrder_Product] FOREIGN KEY([Material])
REFERENCES [dbo].[Product] ([Product])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[ProcessOrder] NOCHECK CONSTRAINT [FK_ProcessOrder_Product]
GO
ALTER TABLE [dbo].[ProcessOrderComponent]  WITH CHECK ADD  CONSTRAINT [FK_ProcessOrderComponent_ProcessOrder] FOREIGN KEY([ManufacturingOrder])
REFERENCES [dbo].[ProcessOrder] ([ManufacturingOrder])
GO
ALTER TABLE [dbo].[ProcessOrderComponent] CHECK CONSTRAINT [FK_ProcessOrderComponent_ProcessOrder]
GO
ALTER TABLE [dbo].[ProcessOrderComponent]  WITH NOCHECK ADD  CONSTRAINT [FK_ProcessOrderComponent_Product] FOREIGN KEY([Material])
REFERENCES [dbo].[Product] ([Product])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[ProcessOrderComponent] NOCHECK CONSTRAINT [FK_ProcessOrderComponent_Product]
GO
ALTER TABLE [dbo].[ProcessOrderConfirmation]  WITH CHECK ADD  CONSTRAINT [FK_ProcessOrderConfirmation_CommStatus] FOREIGN KEY([CommStatus])
REFERENCES [dbo].[CommStatus] ([StatusId])
GO
ALTER TABLE [dbo].[ProcessOrderConfirmation] CHECK CONSTRAINT [FK_ProcessOrderConfirmation_CommStatus]
GO
ALTER TABLE [dbo].[ProcessOrderConfirmation]  WITH CHECK ADD  CONSTRAINT [FK_ProcessOrderConfirmation_ProcessOrder] FOREIGN KEY([OrderID])
REFERENCES [dbo].[ProcessOrder] ([ManufacturingOrder])
GO
ALTER TABLE [dbo].[ProcessOrderConfirmation] CHECK CONSTRAINT [FK_ProcessOrderConfirmation_ProcessOrder]
GO
ALTER TABLE [dbo].[ProcessOrderConfirmationMaterialMovement]  WITH CHECK ADD  CONSTRAINT [FK_ProcessOrderConfirmationMaterialMovement_ProcessOrderComponent] FOREIGN KEY([ProcessOrderComponentId])
REFERENCES [dbo].[ProcessOrderComponent] ([ProcessOrderComponentId])
GO
ALTER TABLE [dbo].[ProcessOrderConfirmationMaterialMovement] CHECK CONSTRAINT [FK_ProcessOrderConfirmationMaterialMovement_ProcessOrderComponent]
GO
ALTER TABLE [dbo].[ProcessOrderConfirmationMaterialMovement]  WITH CHECK ADD  CONSTRAINT [FK_ProcessOrderConfirmationMaterialMovement_ProcessOrderConfirmation] FOREIGN KEY([ProcessOrderConfirmationId])
REFERENCES [dbo].[ProcessOrderConfirmation] ([ProcessOrderConfirmationId])
GO
ALTER TABLE [dbo].[ProcessOrderConfirmationMaterialMovement] CHECK CONSTRAINT [FK_ProcessOrderConfirmationMaterialMovement_ProcessOrderConfirmation]
GO

