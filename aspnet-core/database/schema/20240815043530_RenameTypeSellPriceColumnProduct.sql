BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AppProducts]') AND [c].[name] = N'SellPrice');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [AppProducts] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [AppProducts] ALTER COLUMN [SellPrice] float NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240815043530_RenameTypeSellPriceColumnProduct', N'6.0.5');
GO

COMMIT;
GO

