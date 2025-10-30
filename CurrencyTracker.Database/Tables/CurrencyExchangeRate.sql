CREATE TABLE [dbo].[CurrencyExchangeRate]
(
	[Id] UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
	[BaseCurrencyId] UNIQUEIDENTIFIER NOT NULL,
	[TargetCurrencyId] UNIQUEIDENTIFIER NOT NULL, 
	[Ask] DECIMAL(7,4) NOT NULL,
	[Bid] DECIMAL(7,4) NOT NULL,
	[ExchangeDate] DATE NOT NULL,
	[CreateDate] DATETIME NOT NULL,

    CONSTRAINT FK_BaseCurrency_Currency FOREIGN KEY ([BaseCurrencyId]) REFERENCES Currency([Id]),
	CONSTRAINT FK_TargetCurrency_Currency FOREIGN KEY ([TargetCurrencyId]) REFERENCES Currency([Id]),
	CONSTRAINT CHK_BaseTarget CHECK ([BaseCurrencyId] <> [TargetCurrencyId]),
	CONSTRAINT UQ_ExchangeDaily UNIQUE ([BaseCurrencyId],[TargetCurrencyId], [ExchangeDate])
)
