-- USERS
--CREATE TABLE Users (
--    UserId INT PRIMARY KEY IDENTITY,
--    Username NVARCHAR(100) NOT NULL,
--    PasswordHash NVARCHAR(255) NOT NULL
--);

GO
use Productdb
GO

-- ROLES
CREATE TABLE Roles (
    RoleId INT PRIMARY KEY IDENTITY,
    RoleName NVARCHAR(100) UNIQUE NOT NULL
);

GO
INSERT INTO Roles(RoleName)
VALUES ('admin'),('member'),('user')
GO

-- USER ROLES
CREATE TABLE UserRoles (
    UserId INT,
    RoleId INT,
    PRIMARY KEY(UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);

-- PERMISSIONS
CREATE TABLE Permissions (
    PermissionId INT PRIMARY KEY IDENTITY,
    PermissionName NVARCHAR(100) UNIQUE NOT NULL
);

GO
INSERT INTO Permissions(PermissionName)
VALUES ('assignRole'),('assignPermission')
GO

-- ROLE PERMISSIONS
CREATE TABLE RolePermissions (
    RoleId INT,
    PermissionId INT,
    PRIMARY KEY(RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId),
    FOREIGN KEY (PermissionId) REFERENCES Permissions(PermissionId)
);

-------------Stored Procedures---------------------
GO
CREATE PROCEDURE sp_AssignRoleToUser
    @UserId INT,
    @RoleId INT
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM UserRoles WHERE UserId = @UserId AND RoleId = @RoleId
    )
    BEGIN
        INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)
    END
END
GO

GO
INSERT INTO UserRoles (UserId, RoleId)
VALUES (1, 1)
GO

GO
CREATE PROCEDURE sp_AssignPermissionToRole
    @RoleId INT,
    @PermissionId INT
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM RolePermissions WHERE RoleId = @RoleId AND PermissionId = @PermissionId
    )
    BEGIN
        INSERT INTO RolePermissions (RoleId, PermissionId) VALUES (@RoleId, @PermissionId)
    END
END
GO

GO
INSERT INTO RolePermissions(RoleId, PermissionId)
VALUES (1, 1), (1, 2)
GO

GO
CREATE PROCEDURE sp_UserHasPermission
    @UserId INT,
    @PermissionName NVARCHAR(100)
AS
BEGIN
    SELECT 1
    FROM UserRoles ur
    JOIN RolePermissions rp ON ur.RoleId = rp.RoleId
    JOIN Permissions p ON rp.PermissionId = p.PermissionId
    WHERE ur.UserId = @UserId AND p.PermissionName = @PermissionName
END
GO

----what actually happens here in permissions is
---we have to add roles and permissions first then
----we define what roles can accesses which permissions 
----then the permission names are set in the policy in api.
---------------How it traces in code----------------------
----now permissions name(in api)-->permissionId and RoleId --> userId and roleId