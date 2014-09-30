USE [DocumentManager]
GO

/****** Object:  Table [dbo].[TWK_Assemblies]    Script Date: 10/14/2012 08:50:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TWK_Assemblies](
	[AssemblyID] [int] IDENTITY(1,1) NOT NULL,
	[TypeName] [varchar](50) NULL,
	[AssemblyPath] [varchar](500) NULL,
	[ClassName] [varchar](100) NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_twk_Assemblies] PRIMARY KEY CLUSTERED 
(
	[AssemblyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

USE [DocumentManager]
GO

/****** Object:  Table [dbo].[TWK_RunProcedures]    Script Date: 10/14/2012 08:50:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TWK_RunProcedures](
	[procedureID] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NULL,
	[source] [varchar](50) NULL,
	[priority] [int] NULL,
	[currentstatus] [varchar](50) NULL,
	[procsXML] [xml] NULL,
	[maxretrys] [int] NULL,
	[IsComplete] [bit] NULL,
	[StartedOn] [datetime] NULL,
	[CompletedOn] [datetime] NULL,
	[ErrorCount] [int] NULL,
	[ProcessingBy] [varchar](50) NULL,
	[IsProcessing] [bit] NULL,
	[HostName] [varchar](50) NULL,
	[IPAddress] [varchar](50) NULL,
	[Comments] [varchar](255) NULL,
	[AddedBy] [varchar](50) NULL,
	[AddedOn] [datetime] NULL,
	[TrackingNumber] [varchar](50) NULL,
	[TrackingNumberType] [int] NULL,
 CONSTRAINT [PK_TWK_RunProcedures] PRIMARY KEY CLUSTERED 
(
	[procedureID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

