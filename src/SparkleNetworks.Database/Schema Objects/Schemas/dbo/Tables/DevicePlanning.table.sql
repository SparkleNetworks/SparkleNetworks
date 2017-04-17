
CREATE TABLE [dbo].[DevicePlanning]
(
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [DeviceId]            INT			   NOT NULL,
    [LayoutType]		  NVARCHAR (50)    NOT NULL,
    [LayoutData]          NTEXT			   NULL,
    [DateUpdatedUtc]      DATETIME         NOT NULL,
    [DateStartUtc]		  DATETIME         NOT NULL,
    [DateEndUtc]		  DATETIME         NOT NULL,

    CONSTRAINT [PK_DevicePlanning] PRIMARY KEY (Id),
    CONSTRAINT [CC_DevicePlanning_DateStartIsBeforeDateEnd] CHECK  (DateStartUtc < DateEndUtc),
    CONSTRAINT [FK_DevicePlanning_DeviceId] FOREIGN KEY (DeviceId) REFERENCES Devices (Id) ON DELETE NO ACTION ON UPDATE NO ACTION,
)
