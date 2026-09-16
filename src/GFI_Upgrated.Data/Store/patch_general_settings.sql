-- Script to create and seed Z_MasterGeneralSettings table matching exact DB schema
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Z_MasterGeneralSettings')
BEGIN
    CREATE TABLE [dbo].[Z_MasterGeneralSettings](
        [ConfigID] [bigint] NOT NULL,
        [ConfigKey] [nvarchar](300) NOT NULL,
        [ConfigValue] [nvarchar](300) NOT NULL,
        CONSTRAINT [PK_Z_MasterGeneralSettings] PRIMARY KEY CLUSTERED ([ConfigID] DESC)
    );
END
GO

-- Seed default setting keys if they do not already exist
IF NOT EXISTS (SELECT 1 FROM [dbo].[Z_MasterGeneralSettings] WHERE [ConfigKey] = 'CompanyName')
    INSERT INTO [dbo].[Z_MasterGeneralSettings] ([ConfigID], [ConfigKey], [ConfigValue])
    VALUES ((SELECT ISNULL(MAX([ConfigID]), 0) + 1 FROM [dbo].[Z_MasterGeneralSettings]), N'CompanyName', N'GFI Nuvotrace');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Z_MasterGeneralSettings] WHERE [ConfigKey] = 'CompanyAddress')
    INSERT INTO [dbo].[Z_MasterGeneralSettings] ([ConfigID], [ConfigKey], [ConfigValue])
    VALUES ((SELECT ISNULL(MAX([ConfigID]), 0) + 1 FROM [dbo].[Z_MasterGeneralSettings]), N'CompanyAddress', N'');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Z_MasterGeneralSettings] WHERE [ConfigKey] = 'CompanyLogo')
    INSERT INTO [dbo].[Z_MasterGeneralSettings] ([ConfigID], [ConfigKey], [ConfigValue])
    VALUES ((SELECT ISNULL(MAX([ConfigID]), 0) + 1 FROM [dbo].[Z_MasterGeneralSettings]), N'CompanyLogo', N'/assets/img/branding/nuvotrace-horizontal-logo.png');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Z_MasterGeneralSettings] WHERE [ConfigKey] = 'TopbarLogo')
    INSERT INTO [dbo].[Z_MasterGeneralSettings] ([ConfigID], [ConfigKey], [ConfigValue])
    VALUES ((SELECT ISNULL(MAX([ConfigID]), 0) + 1 FROM [dbo].[Z_MasterGeneralSettings]), N'TopbarLogo', N'/assets/img/branding/nuvotrace-horizontal-logo.png');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Z_MasterGeneralSettings] WHERE [ConfigKey] = 'LoginPageLogo')
    INSERT INTO [dbo].[Z_MasterGeneralSettings] ([ConfigID], [ConfigKey], [ConfigValue])
    VALUES ((SELECT ISNULL(MAX([ConfigID]), 0) + 1 FROM [dbo].[Z_MasterGeneralSettings]), N'LoginPageLogo', N'/assets/img/branding/nuvotrace-custom-logo.png');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Z_MasterGeneralSettings] WHERE [ConfigKey] = 'DefaultCurrency')
    INSERT INTO [dbo].[Z_MasterGeneralSettings] ([ConfigID], [ConfigKey], [ConfigValue])
    VALUES ((SELECT ISNULL(MAX([ConfigID]), 0) + 1 FROM [dbo].[Z_MasterGeneralSettings]), N'DefaultCurrency', N'SRD');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Z_MasterGeneralSettings] WHERE [ConfigKey] = 'DateFormat')
    INSERT INTO [dbo].[Z_MasterGeneralSettings] ([ConfigID], [ConfigKey], [ConfigValue])
    VALUES ((SELECT ISNULL(MAX([ConfigID]), 0) + 1 FROM [dbo].[Z_MasterGeneralSettings]), N'DateFormat', N'MM/DD/YYYY');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Z_MasterGeneralSettings] WHERE [ConfigKey] = 'DecimalDigits')
    INSERT INTO [dbo].[Z_MasterGeneralSettings] ([ConfigID], [ConfigKey], [ConfigValue])
    VALUES ((SELECT ISNULL(MAX([ConfigID]), 0) + 1 FROM [dbo].[Z_MasterGeneralSettings]), N'DecimalDigits', N'2');
GO
