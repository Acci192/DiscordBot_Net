CREATE TABLE [dbo].[Event]
(
	[Id] UNIQUEIDENTIFIER NOT NULL , 
    [Version] INT NOT NULL, 
    PRIMARY KEY ([Id], [Version])
)
