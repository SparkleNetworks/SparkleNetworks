
CREATE TABLE [dbo].[Contacts] (
    [IsDisplayed] BIT NULL,
    [UserId]      INT NOT NULL,
    [ContactId]   INT NOT NULL,

    CONSTRAINT [PK_Friends] PRIMARY KEY CLUSTERED ([UserId] ASC, [ContactId] ASC),
    CONSTRAINT [FK_Contacts_Contact] FOREIGN KEY ([ContactId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_Contacts_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);

