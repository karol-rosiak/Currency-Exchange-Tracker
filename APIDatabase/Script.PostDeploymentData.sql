/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF NOT EXISTS (SELECT 1 FROM dbo.Currency WHERE Code = 'PLN')
BEGIN
    INSERT INTO dbo.Currency (Code, Name) VALUES ('PLN', 'Polski Złoty');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Currency WHERE Code = 'USD')
BEGIN
    INSERT INTO dbo.Currency (Code, Name) VALUES ('USD', 'Dolar Amerykański');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Currency WHERE Code = 'EUR')
BEGIN
    INSERT INTO dbo.Currency (Code, Name) VALUES ('EUR', 'Euro');
END