USE master;

-- Checks if HospitalDB was created, otherwise generate a new one:

IF NOT EXISTS (SELECT NAME FROM master.dbo.sysdatabases WHERE [name] = 'HospitalDB')
BEGIN
    CREATE DATABASE HospitalDB;
END
GO

USE HospitalDB;
GO

-- Checks if a certain table was created, otherwise generate a new one:

IF NOT EXISTS(SELECT NAME FROM SysObjects WHERE Name = 'UserRegister')
BEGIN
    CREATE TABLE  dbo.UserRegister (
        UserID int NOT NULL IDENTITY(1, 1),
        Email VARCHAR(100) NOT NULL,
		Roles VARCHAR(100) NOT NULL,
		Token VARCHAR(1000),
        PasswordHash VARBINARY(MAX) NOT NULL,
		PasswordSalt VARBINARY(MAX) NOT NULL,
        PRIMARY KEY(UserID)
    );

	 INSERT INTO UserRegister(Email, Roles, Token, PasswordHash, PasswordSalt) 
	 VALUES ('admin@hospital.com', 'ADMIN', '', 0xC5477A8864B962B999B8B55A4499163A2A7A2DED85BE4FCEB8AF1920662C214669B918B4D77FDC6BCCCF43867F2121D37E5A81BCB6C25B5070265961A0A1559C, 0xFC5678EAE52E7838B86821F3565A9BB362FAB0F71C5036C2779ABEF357C01D3B6CB9D2BE1D6367C386B30F8225D0D46E7C3304D4DFFD431E9534247E971F34F29169BC45D281C38CB60A91D9FCAAD96BBF45B9F247948B0378CE9EAC18C4931BB58DB9037E31AB257176C754ECA53C7991D7726DD47E1E6C5DD9A0EF603EE87A);
END
GO

-- Table Refresh token:

IF NOT EXISTS(SELECT NAME FROM SysObjects WHERE Name = 'RefreshToken')
BEGIN
    CREATE TABLE  dbo.RefreshToken (
        Id int NOT NULL IDENTITY(1, 1),
        UserID int NOT NULL,
		Token VARCHAR(1000) NOT NULL,
		IsUsed BIT NOT NULL,
		IsRevoked BIT NOT NULL,
		AddedDate DATETIME NOT NULL,
		ExpireDate DATETIME NOT NULL,
        PRIMARY KEY(Id),
		FOREIGN KEY (UserID) REFERENCES UserRegister(UserID)
    );
END
GO

BEGIN
    CREATE TABLE doctor (
        id int NOT NULL IDENTITY(1,1),
        first_name VARCHAR(100) NOT NULL,
        last_name VARCHAR(100) NOT NULL,
        specialty VARCHAR(100) NOT NULL,
        PRIMARY KEY(id)
    );
    
    INSERT INTO doctor(first_name, last_name, specialty) VALUES ('Alexander', 'Fleming', 'Internal Medicine');
END
GO

BEGIN
    CREATE TABLE area (
        id int NOT NULL IDENTITY(1,1),
        area_name VARCHAR(100) NOT NULL,
		PRIMARY KEY(id)
    );

    INSERT INTO area(area_name) VALUES ('Emergencies');
    INSERT INTO area(area_name) VALUES ('X-RAY');
    INSERT INTO area(area_name) VALUES ('Intensive Care Unit');

END
GO

BEGIN
    CREATE TABLE patient (
        id int NOT NULL IDENTITY(1,1),
        doctor_id int NOT NULL,
        area_id int NOT NULL,
        first_name VARCHAR(100) NOT NULL,
        last_name VARCHAR(100) NOT NULL,
        admission_date datetime NOT NULL,
        release_date datetime,
        PRIMARY KEY(id),
        FOREIGN KEY(doctor_id) REFERENCES doctor(id),
        FOREIGN KEY(area_id) REFERENCES area(id)
    );

    INSERT INTO patient(doctor_id, area_id, first_name, last_name, admission_date, release_date) VALUES ('1', '3', 'Andrea', 'Sanchez', '20220424 11:30:09 AM', '20220430 09:00:00 AM');
END
GO

-- Store procedures:

-- Register a user:

IF EXISTS(SELECT NAME FROM SysObjects WHERE Name = 'spRegister')
	DROP PROCEDURE dbo.spRegister
GO

CREATE PROCEDURE dbo.spRegister(@Email VARCHAR(100), @PasswordHash VARBINARY(200), @PasswordSalt VARBINARY(200))
AS
BEGIN

	IF exists(SELECT * FROM	 UserRegister WHERE Email=@Email)
		SELECT 'Email was taken' AS Response

	ELSE
		INSERT INTO UserRegister(Email, Roles, Token, PasswordHash, PasswordSalt)
		VALUES(@Email, 'USER', '', @PasswordHash, @PasswordSalt)
		   
		SELECT 'Registed Successfully' AS Response
END

-- Login a user:

GO
IF EXISTS(SELECT NAME FROM SysObjects WHERE Name = 'spLogin')
	DROP PROCEDURE dbo.spLogin
GO

CREATE PROCEDURE dbo.spLogin(@Email VARCHAR(100))
AS
BEGIN
	SET NOCOUNT ON

	IF exists(SELECT Email FROM UserRegister WHERE Email=@Email)
		SELECT UserID, Email, Roles, Token, PasswordHash, PasswordSalt, '' FROM UserRegister WHERE Email=@Email
	ELSE 
		SELECT 'Email not found' AS Response

	SET NOCOUNT OFF
END

-- Save Token once the user is login:

GO
IF EXISTS(SELECT NAME FROM SysObjects WHERE Name = 'spSaveToken')
	DROP PROCEDURE dbo.spSaveToken
GO

CREATE PROCEDURE dbo.spSaveToken(@Email VARCHAR(100), @Token VARCHAR(1000))
AS
BEGIN
	SET NOCOUNT ON

		UPDATE UserRegister
		SET Token = @Token
		WHERE Email=@Email

	SET NOCOUNT OFF
END

-- Insert values into Refresh token values:

GO
IF EXISTS(SELECT NAME FROM SysObjects WHERE Name = 'spAddValuesRefreshToken')
	DROP PROCEDURE dbo.spAddValuesRefreshToken
GO

CREATE PROCEDURE dbo.spAddValuesRefreshToken(@IsUsed BIT, @IsRevoked BIT, @UserID int, @AddedDate DATETIME, @ExpireDate DATETIME, @Token VARCHAR(400))
AS
BEGIN
	SET NOCOUNT ON

		INSERT INTO RefreshToken(IsUsed, IsRevoked, UserID, AddedDate, ExpireDate, Token)
		VALUES(@IsUsed, @IsRevoked, @UserID, @AddedDate, @ExpireDate, @Token)

	SET NOCOUNT OFF
END

-- Find the refresh token concurrency:

GO
IF EXISTS(SELECT NAME FROM SysObjects WHERE Name = 'spFindRefreshToken')
	DROP PROCEDURE dbo.spFindRefreshToken
GO

CREATE PROCEDURE dbo.spFindRefreshToken(@Token VARCHAR(1000))
AS
BEGIN
	SET NOCOUNT ON

		IF exists(SELECT Token FROM RefreshToken WHERE Token=@Token)
			SELECT Id, UserID, Token, IsUsed, IsRevoked, AddedDate, ExpireDate FROM RefreshToken WHERE Token=@Token

	SET NOCOUNT OFF
END


-- Update refresh token table:

GO
IF EXISTS(SELECT NAME FROM SysObjects WHERE Name = 'spUpdateRefreshToken')
	DROP PROCEDURE dbo.spUpdateRefreshToken
GO

CREATE PROCEDURE dbo.spUpdateRefreshToken(@Id int, @UserID int, @Token VARCHAR(400), @IsUsed BIT, @IsRevoked BIT, @AddedDate DATETIME, @ExpireDate DATETIME)
AS
BEGIN
	SET NOCOUNT ON

		UPDATE RefreshToken
		SET UserID = @UserID, Token = @Token, IsUsed = @IsUsed, IsRevoked = @IsRevoked, AddedDate = @AddedDate, ExpireDate = @ExpireDate
		WHERE Id = @Id

	SET NOCOUNT OFF
END

-- Save refresh token:

GO
IF EXISTS(SELECT NAME FROM SysObjects WHERE Name = 'spSaveNewToken')
	DROP PROCEDURE dbo.spSaveNewToken
GO

CREATE PROCEDURE dbo.spSaveNewToken(@UserID int, @Token VARCHAR(1000))
AS
BEGIN
	SET NOCOUNT ON

		UPDATE UserRegister
		SET Token = @Token
		WHERE UserID=@UserID

	SET NOCOUNT OFF
END

-- Find user by id:

GO
IF EXISTS(SELECT NAME FROM SysObjects WHERE Name = 'spFindUserById')
	DROP PROCEDURE dbo.spFindUserById
GO

CREATE PROCEDURE dbo.spFindUserById(@UserID int)
AS
BEGIN
	SET NOCOUNT ON

		IF exists(SELECT UserID FROM UserRegister WHERE UserID=@UserID)
			SELECT UserID, Email, Roles, PasswordHash, PasswordSalt, '' FROM UserRegister WHERE UserID=@UserID

	SET NOCOUNT OFF
END