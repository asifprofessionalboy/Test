CREATE TABLE [dbo].[App_UserFormPermission] (
    [UserID]      UNIQUEIDENTIFIER NOT NULL,
    [FormId]      UNIQUEIDENTIFIER NOT NULL,
    [AllowRead]   BIT              CONSTRAINT [DF_App_UserFormPermission_AllowRead] DEFAULT ((0)) NOT NULL,
    [AllowWrite]  BIT              CONSTRAINT [DF_App_UserFormPermission_AllowWrite] DEFAULT ((0)) NOT NULL,
    [AllowDelete] BIT              NULL,
    [AllowAll]    BIT              NULL,
    [AllowModify] BIT              NULL,
    [DownTime]    BIT              DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_App_UserFormPermission] PRIMARY KEY CLUSTERED ([UserID] ASC, [FormId] ASC)
);

CREATE TABLE [dbo].[App_FormDetails] (
    [ID]       UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [FormName] VARCHAR (100)    NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);
