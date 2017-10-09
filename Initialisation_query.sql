-- REMOVE ALL FROM TABLES
DELETE FROM dbo.Participants;
DELETE FROM dbo.TaskResults;
DELETE FROM dbo.TestResults;
DELETE FROM dbo.Tasks;
DELETE FROM dbo.Tests;
--RESET AUTO INCREMENT IDS
DBCC CHECKIDENT ('[Participants]', RESEED, 0);
DBCC CHECKIDENT ('[TaskResults]', RESEED, 0);
DBCC CHECKIDENT ('[Tasks]', RESEED, 0);
DBCC CHECKIDENT ('[TestResults]', RESEED, 0);
DBCC CHECKIDENT ('[Tests]', RESEED, 0);

INSERT INTO dbo.Tests VALUES ('http://www.nhs.uk');
INSERT INTO dbo.Tasks VALUES (1, 'Find the address of your nearest hospital.');
INSERT INTO dbo.Tasks VALUES (1, 'You are looking after someone who has chickenpox and they have a low fever. Find out if they should take paracetamol or ibuprofen.');
INSERT INTO dbo.Tasks VALUES (1, 'Sign up to the NHS newsletter.');
INSERT INTO dbo.Tasks VALUES (1, 'Find out where you can renew your European Health Insurance card.');
INSERT INTO dbo.Participants VALUES (1, 'Eval1'); -- Accounts for user evaluation
INSERT INTO dbo.Participants VALUES (1, 'Eval2'); 
INSERT INTO dbo.Participants VALUES (1, 'Eval3'); 
INSERT INTO dbo.Participants VALUES (1, 'Eval4'); 
INSERT INTO dbo.Participants VALUES (1, 'Eval5'); 
INSERT INTO dbo.Participants VALUES (1, 'Eval6'); 
INSERT INTO dbo.Participants VALUES (1, 'Test1'); -- Test 1
INSERT INTO dbo.Participants VALUES (1, 'Test2'); -- Test 2
INSERT INTO dbo.Participants VALUES (1, 'Demo1'); -- Demo 1
INSERT INTO dbo.Participants VALUES (1, 'Demo2'); -- Demo 2

INSERT INTO dbo.Tests VALUES ('http://www.google.co.uk');
INSERT INTO dbo.Tasks VALUES (2, 'Navigate to the first link that appears when searching for "Java".');
INSERT INTO dbo.Tasks VALUES (2, 'Find the address of the Google HQ.');
INSERT INTO dbo.Tasks VALUES (2, 'Find an image of an iPhone 7.');
INSERT INTO dbo.Participants VALUES (2, 'RG-02');
INSERT INTO dbo.Participants VALUES (2, 'ML-02');

