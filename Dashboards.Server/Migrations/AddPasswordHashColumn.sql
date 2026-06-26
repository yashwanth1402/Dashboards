-- Migration: Add PasswordHash column for BCrypt password migration
-- Run this against FFI_DEV database

IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[Users].[Information]') 
    AND name = 'PasswordHash'
)
BEGIN
    ALTER TABLE [Users].[Information]
    ADD [PasswordHash] NVARCHAR(200) NULL;
    
    PRINT 'Column PasswordHash added successfully.';
END
ELSE
BEGIN
    PRINT 'Column PasswordHash already exists.';
END
GO
