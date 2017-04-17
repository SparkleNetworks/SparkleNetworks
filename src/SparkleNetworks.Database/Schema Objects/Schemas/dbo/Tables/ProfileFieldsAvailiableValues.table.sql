CREATE TABLE [dbo].[ProfileFieldsAvailiableValues]
(
    [Id]               INT             NOT NULL,
    [ProfileFieldId]   INT             NOT NULL,
    [Value]            NVARCHAR(200)   NOT NULL,

    CONSTRAINT [PK_ProfileFieldsAvailiableValues] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProfileFieldsAvailiableValues_ProfileFields] FOREIGN KEY ([ProfileFieldId]) REFERENCES [dbo].[ProfileFields] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [UC_ProfileFieldsAvailiableValues_Value] UNIQUE ([Value]),
);
