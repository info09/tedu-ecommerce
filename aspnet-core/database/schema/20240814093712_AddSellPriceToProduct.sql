BEGIN TRANSACTION;
GO

ALTER TABLE [AppProducts] ADD [SellPrice] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240814093712_AddSellPriceToProduct', N'6.0.5');
GO

COMMIT;
GO

