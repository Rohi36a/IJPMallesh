USE [IJP]
GO

/****** Object:  Table [dbo].[IJPDetails]    Script Date: 9/18/2018 11:24:31 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[IJPDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Job] [nvarchar](500) NULL,
	[Experience] [decimal](18, 0) NULL,
	[LastDate] [date] NULL,
	[ApplicationReceived] [date] NULL,
	[Quantity] [int] NULL,
	[StatusId] [int] NOT NULL,
 CONSTRAINT [PK_IJPDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[IJPDetails]  WITH CHECK ADD  CONSTRAINT [FK_IJPDetails_Status] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Status] ([Id])
GO

ALTER TABLE [dbo].[IJPDetails] CHECK CONSTRAINT [FK_IJPDetails_Status]
GO


