USE [Produccion]
GO

/****** Object:  Table [dbo].[CorreosPortal]    Script Date: 29/07/2015 08:58:48 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CorreosPortal](
	[IdEmail] [int] IDENTITY(1,1) NOT NULL,
	[Subject] [nvarchar](600) NOT NULL,
	[BodyMail] [xml] NULL,
	[Firma] [xml] NULL,
	[Proceso] [nvarchar](10) NOT NULL,
	[Activo] [bit] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO