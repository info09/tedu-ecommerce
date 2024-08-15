BEGIN TRANSACTION;
GO

EXEC sp_rename N'[AppProducts].[Visiblity]', N'Visibility', N'COLUMN';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240815024855_UpdateVisibilitySpellColumns', N'6.0.5');
GO

COMMIT;
GO

