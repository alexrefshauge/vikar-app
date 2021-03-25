Strong password:
    aVn9vC7c

[] Opdater database, køn - område - login
[] Datahandler klasse
[] Færdiggør Browse
[] Færdiggør Loginside/signup(✔️)
[] implementer data på hjemmeside - ([x]profil og []browse)

sql tables:

CREATE TABLE [dbo].[Kommentar] (
    [Kommentar Id] INT        IDENTITY (1, 1) NOT NULL,
    [Bruger Id]    INT        NOT NULL,
    [Kommentar]    CHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([Kommentar Id] ASC)
);