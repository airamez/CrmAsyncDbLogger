SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Logs](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Level] [nvarchar](20) NOT NULL,
	[Correlation_ID] [nvarchar](36) NULL,
	[Machine_Name] [nvarchar](250) NULL,
	[User] [nvarchar](150) NULL,
	[Class] [nvarchar](250) NULL,
	[Method] [nvarchar](250) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[Data] [nvarchar](max) NULL,
	[Exception] [nvarchar](max) NULL
 CONSTRAINT [pk_logs] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Primary Key (Identity)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs', @level2type=N'COLUMN',@level2name=N'LogId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Log entry time' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs', @level2type=N'COLUMN',@level2name=N'Date'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Log entry level' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs', @level2type=N'COLUMN',@level2name=N'Level'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Log entry correlation ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs', @level2type=N'COLUMN',@level2name=N'Correlation_ID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Caller machine name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs', @level2type=N'COLUMN',@level2name=N'Machine_Name'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Caller user name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs', @level2type=N'COLUMN',@level2name=N'User'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Caller class name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs', @level2type=N'COLUMN',@level2name=N'Class'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Caller method name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs', @level2type=N'COLUMN',@level2name=N'Method'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Log entry message' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs', @level2type=N'COLUMN',@level2name=N'Message'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Log entry data (JSON)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs', @level2type=N'COLUMN',@level2name=N'Data'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Log entry exception. Plus inner exceptions and stack trace' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Logs', @level2type=N'COLUMN',@level2name=N'Exception'
GO

CREATE procedure [dbo].[InsertLog] 
(
	@date datetime,
	@level nvarchar(10),
	@correlation_id nvarchar(50),
	@machine_name nvarchar(250),
	@user nvarchar(150),
	@class nvarchar(250),
	@method nvarchar(150),
	@message nvarchar(max),
	@data nvarchar(max),
	@exception nvarchar(max)
)
as

insert into dbo.Logs
(
	[Date],
	[Level],
	Correlation_ID,
	Machine_Name,
	[User],
	Class,
	Method,
	[Message],
	[Data],
	[exception]

)
values
(
	@date,
	@level,
	@correlation_id,
	@machine_name,
	@user,
	@class,
	@method,
	@message,
	@data,
	@exception
)
GO
