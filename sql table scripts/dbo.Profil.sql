CREATE TABLE [dbo].[Profil] (
    [Bruger Id]     INT         IDENTITY (1, 1) NOT NULL,
    [Navn]          CHAR (20)   NOT NULL,
    [Efternavn]     CHAR (20)   NOT NULL,
    [Alder]         INT         NOT NULL,
    [Køn]           INT         NOT NULL,
    [Område]        INT         NOT NULL,
    [Bio]           CHAR (255)  NULL,
    [Profilbillede] BINARY (50) NULL,
    [Tlf]           CHAR (8)    NULL,
    [Email]         CHAR (63)   NULL,
    PRIMARY KEY CLUSTERED ([Bruger Id] ASC)
);

