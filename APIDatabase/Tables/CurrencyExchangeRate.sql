CREATE TABLE [dbo].[CurrencyExchangeRate]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[CurrencyId] UNIQUEIDENTIFIER NOT NULL,
	[Ask] DECIMAL(7,4) NOT NULL,
	[Bid] DECIMAL(7,4) NOT NULL,
	[ExchangeDate] DATE NOT NULL,
	[CreateDate] DATETIME NOT NULL,
	CONSTRAINT FK_Child_Parent FOREIGN KEY ([CurrencyId]) REFERENCES Currency([Id]),
	CONSTRAINT UQ_ExchangeDaily UNIQUE ([CurrencyId], [ExchangeDate])
)
