-- Migration: Add Description + UserTypeID columns to Type.Roles and seed Security.Pages
-- Run against FFI_DEV database on (localdb)\MSSQLLocalDB

-- 1. Add Description column to Type.Roles (if not exists)
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[Type].[Roles]') AND name = 'Description'
)
BEGIN
    ALTER TABLE [Type].[Roles] ADD [Description] NVARCHAR(500) NULL;
    PRINT 'Added Description column to [Type].[Roles]';
END
GO

-- 2. Add UserTypeID column to Type.Roles (if not exists)
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[Type].[Roles]') AND name = 'UserTypeID'
)
BEGIN
    ALTER TABLE [Type].[Roles] ADD [UserTypeID] INT NULL;
    PRINT 'Added UserTypeID column to [Type].[Roles]';
END
GO

-- 3. Seed Security.Pages if table is empty
IF NOT EXISTS (SELECT 1 FROM [Security].[Pages])
BEGIN
    INSERT INTO [Security].[Pages] (DisplayName, SourcePageName, PageOrder) VALUES
    ('Main Dashboard', 'MainDashboard', 1),
    ('Association Profile', 'AssociationProfile', 2),
    ('Insurance Policies', 'InsurancePolicies', 3),
    ('COPE Questionnaire', 'COPEQuestionnaire', 4),
    ('COPE Admin Config', 'COPEAdminConfig', 5),
    ('Reports', 'Reports', 6),
    ('User Management', 'UserManagement', 7),
    ('System Settings', 'SystemSettings', 8);
    PRINT 'Seeded 8 modules into [Security].[Pages]';
END
ELSE
BEGIN
    PRINT '[Security].[Pages] already has data - skipping seed';
END
GO

-- 4. Link existing roles to UserTypes (where names match)
UPDATE r
SET r.UserTypeID = ut.UserTypeID
FROM [Type].[Roles] r
INNER JOIN [Type].[Users] ut ON ut.Name = r.Name
WHERE r.UserTypeID IS NULL;
PRINT 'Linked existing roles to matching UserTypes';
GO
