
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/20/2017 21:11:42
-- Generated from EDMX file: E:\Users\Ryan\My Docs\Programming\C#\RUT-Project-Working\ORM\UXProjectModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [uxprojectmodel];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_TestResultParticipant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TestResults] DROP CONSTRAINT [FK_TestResultParticipant];
GO
IF OBJECT_ID(N'[dbo].[FK_TestSubject]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Participants] DROP CONSTRAINT [FK_TestSubject];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskTaskResult]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskResults] DROP CONSTRAINT [FK_TaskTaskResult];
GO
IF OBJECT_ID(N'[dbo].[FK_TestResultTaskResult]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskResults] DROP CONSTRAINT [FK_TestResultTaskResult];
GO
IF OBJECT_ID(N'[dbo].[FK_TestTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tasks] DROP CONSTRAINT [FK_TestTask];
GO
IF OBJECT_ID(N'[dbo].[FK_TestTestResult]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TestResults] DROP CONSTRAINT [FK_TestTestResult];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Participants]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Participants];
GO
IF OBJECT_ID(N'[dbo].[TaskResults]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TaskResults];
GO
IF OBJECT_ID(N'[dbo].[Tasks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tasks];
GO
IF OBJECT_ID(N'[dbo].[TestResults]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TestResults];
GO
IF OBJECT_ID(N'[dbo].[Tests]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tests];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Participants'
CREATE TABLE [dbo].[Participants] (
    [ParticipantId] int IDENTITY(1,1) NOT NULL,
    [TestId] int  NOT NULL,
    [Password] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'TaskResults'
CREATE TABLE [dbo].[TaskResults] (
    [TaskResultId] int IDENTITY(1,1) NOT NULL,
    [TaskId] int  NOT NULL,
    [TestResultId] int  NOT NULL,
    [URL] nvarchar(max)  NOT NULL,
    [Abandoned] bit  NOT NULL,
    [TimeStarted] time  NOT NULL,
    [TimeTaken] time  NOT NULL,
    [NavigationPath] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Tasks'
CREATE TABLE [dbo].[Tasks] (
    [TaskId] int IDENTITY(1,1) NOT NULL,
    [TestId] int  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [CorrectURL] nvarchar(max)  NOT NULL,
    [ComparisonType] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'TestResults'
CREATE TABLE [dbo].[TestResults] (
    [TestResultId] int IDENTITY(1,1) NOT NULL,
    [TestId] int  NOT NULL,
    [RecordingFilepath] nvarchar(max)  NOT NULL,
    [ParticipantId] int  NOT NULL
);
GO

-- Creating table 'Tests'
CREATE TABLE [dbo].[Tests] (
    [TestId] int IDENTITY(1,1) NOT NULL,
    [URL] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ParticipantId] in table 'Participants'
ALTER TABLE [dbo].[Participants]
ADD CONSTRAINT [PK_Participants]
    PRIMARY KEY CLUSTERED ([ParticipantId] ASC);
GO

-- Creating primary key on [TaskResultId] in table 'TaskResults'
ALTER TABLE [dbo].[TaskResults]
ADD CONSTRAINT [PK_TaskResults]
    PRIMARY KEY CLUSTERED ([TaskResultId] ASC);
GO

-- Creating primary key on [TaskId] in table 'Tasks'
ALTER TABLE [dbo].[Tasks]
ADD CONSTRAINT [PK_Tasks]
    PRIMARY KEY CLUSTERED ([TaskId] ASC);
GO

-- Creating primary key on [TestResultId] in table 'TestResults'
ALTER TABLE [dbo].[TestResults]
ADD CONSTRAINT [PK_TestResults]
    PRIMARY KEY CLUSTERED ([TestResultId] ASC);
GO

-- Creating primary key on [TestId] in table 'Tests'
ALTER TABLE [dbo].[Tests]
ADD CONSTRAINT [PK_Tests]
    PRIMARY KEY CLUSTERED ([TestId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ParticipantId] in table 'TestResults'
ALTER TABLE [dbo].[TestResults]
ADD CONSTRAINT [FK_TestResultParticipant]
    FOREIGN KEY ([ParticipantId])
    REFERENCES [dbo].[Participants]
        ([ParticipantId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TestResultParticipant'
CREATE INDEX [IX_FK_TestResultParticipant]
ON [dbo].[TestResults]
    ([ParticipantId]);
GO

-- Creating foreign key on [TestId] in table 'Participants'
ALTER TABLE [dbo].[Participants]
ADD CONSTRAINT [FK_TestSubject]
    FOREIGN KEY ([TestId])
    REFERENCES [dbo].[Tests]
        ([TestId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TestSubject'
CREATE INDEX [IX_FK_TestSubject]
ON [dbo].[Participants]
    ([TestId]);
GO

-- Creating foreign key on [TaskId] in table 'TaskResults'
ALTER TABLE [dbo].[TaskResults]
ADD CONSTRAINT [FK_TaskTaskResult]
    FOREIGN KEY ([TaskId])
    REFERENCES [dbo].[Tasks]
        ([TaskId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskTaskResult'
CREATE INDEX [IX_FK_TaskTaskResult]
ON [dbo].[TaskResults]
    ([TaskId]);
GO

-- Creating foreign key on [TestResultId] in table 'TaskResults'
ALTER TABLE [dbo].[TaskResults]
ADD CONSTRAINT [FK_TestResultTaskResult]
    FOREIGN KEY ([TestResultId])
    REFERENCES [dbo].[TestResults]
        ([TestResultId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TestResultTaskResult'
CREATE INDEX [IX_FK_TestResultTaskResult]
ON [dbo].[TaskResults]
    ([TestResultId]);
GO

-- Creating foreign key on [TestId] in table 'Tasks'
ALTER TABLE [dbo].[Tasks]
ADD CONSTRAINT [FK_TestTask]
    FOREIGN KEY ([TestId])
    REFERENCES [dbo].[Tests]
        ([TestId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TestTask'
CREATE INDEX [IX_FK_TestTask]
ON [dbo].[Tasks]
    ([TestId]);
GO

-- Creating foreign key on [TestId] in table 'TestResults'
ALTER TABLE [dbo].[TestResults]
ADD CONSTRAINT [FK_TestTestResult]
    FOREIGN KEY ([TestId])
    REFERENCES [dbo].[Tests]
        ([TestId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TestTestResult'
CREATE INDEX [IX_FK_TestTestResult]
ON [dbo].[TestResults]
    ([TestId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------