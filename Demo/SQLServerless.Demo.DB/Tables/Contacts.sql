﻿CREATE TABLE [dbo].[Contacts]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [FirstName] NVARCHAR(100) NOT NULL, 
    [LastName] NVARCHAR(100) NOT NULL, 
    [Email] NVARCHAR(255) NULL, 
    [BirthDate] DATETIME2 NULL, 
    [Height] INT NULL
)
