CREATE TABLE [dbo].[Login] (
    [Login Id]   INT        NOT NULL,
    [Brugernavn] CHAR (20)  NOT NULL,
    [Kodeord]    CHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([Login Id] ASC)
);

