CREATE TABLE [dbo].[Kommentar] (
    [Kommentar Id] INT        IDENTITY (1, 1) NOT NULL,
    [Bruger Id]    INT        NOT NULL,
    [Kommentar]    CHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([Kommentar Id] ASC)
);

