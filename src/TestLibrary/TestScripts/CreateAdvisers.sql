SET IDENTITY_INSERT Adviser ON

INSERT INTO Adviser (Id, FirstName, LastName, Department, CreatedTimestamp, ModifiedTimestamp) VALUES (100, 'Scott', 'Mitchell', 'Business', GETDATE(), GETDATE())
INSERT INTO Adviser (Id, FirstName, LastName, Department, CreatedTimestamp, ModifiedTimestamp) VALUES (101, 'Mike', 'Nash', 'Economics', GETDATE(), GETDATE())

SET IDENTITY_INSERT Adviser OFF