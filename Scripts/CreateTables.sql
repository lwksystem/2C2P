/****** Object:  Table [dbo].[Currency]    Script Date: 11/28/2021 4:03:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Currency](
	[CurrencyCode] [char](3) NOT NULL,
	[CurrencyName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED 
(
	[CurrencyCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Transactions]    Script Date: 11/28/2021 4:03:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transactions](
	[TransactionId] [varchar](50) NOT NULL,
	[TransactionAmount] [decimal](18, 2) NOT NULL,
	[CurrencyCode] [char](3) NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
	[FileType] [char](3) NOT NULL,
	[InputStatus] [varchar](10) NOT NULL,
	[OutputStatus] [char](1) NOT NULL,
 CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[TransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Currency] ([CurrencyCode], [CurrencyName]) VALUES (N'EUR', N'Euro')
GO
INSERT [dbo].[Currency] ([CurrencyCode], [CurrencyName]) VALUES (N'MYR', N'Malaysia Ringgit')
GO
INSERT [dbo].[Currency] ([CurrencyCode], [CurrencyName]) VALUES (N'THB', N'Thai Baht')
GO
INSERT [dbo].[Currency] ([CurrencyCode], [CurrencyName]) VALUES (N'USD', N'United States Dollars')
GO
ALTER TABLE [dbo].[Currency] ADD  CONSTRAINT [DF_Currency_CurrencyCode]  DEFAULT ('') FOR [CurrencyCode]
GO
ALTER TABLE [dbo].[Currency] ADD  CONSTRAINT [DF_Currency_CurrencyName]  DEFAULT ('') FOR [CurrencyName]
GO
ALTER TABLE [dbo].[Transactions] ADD  CONSTRAINT [DF_Transactions_TransId]  DEFAULT ('') FOR [TransactionId]
GO
ALTER TABLE [dbo].[Transactions] ADD  CONSTRAINT [DF_Transactions_TransAmt]  DEFAULT ((0)) FOR [TransactionAmount]
GO
ALTER TABLE [dbo].[Transactions] ADD  CONSTRAINT [DF_Transactions_CurrCode]  DEFAULT ('') FOR [CurrencyCode]
GO
ALTER TABLE [dbo].[Transactions] ADD  CONSTRAINT [DF_Transactions_FileType]  DEFAULT ('') FOR [FileType]
GO
ALTER TABLE [dbo].[Transactions] ADD  CONSTRAINT [DF_Transactions_Status]  DEFAULT ('') FOR [InputStatus]
GO
ALTER TABLE [dbo].[Transactions] ADD  CONSTRAINT [DF_Transactions_OutputStatus]  DEFAULT ('') FOR [OutputStatus]
GO
