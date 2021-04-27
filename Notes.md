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

gammel
"metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=System.Data.SqlClient;provider connection string='data source=&quot;localhost, 1433&quot;;initial catalog=vikar-db;user id=sa;password=&lt;#Password1&gt;;MultipleActiveResultSets=True;App=EntityFramework'""

ny
"data source=localhost, 1433; initial catalog=vikar-db; user id=sa; password=<#Password1>;"