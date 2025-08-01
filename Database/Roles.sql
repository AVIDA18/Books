-- USERS
--CREATE TABLE Users (
--    UserId INT PRIMARY KEY IDENTITY,
--    Username NVARCHAR(100) NOT NULL,
--    PasswordHash NVARCHAR(255) NOT NULL
--);

-- ROLES
CREATE TABLE Roles (
    RoleId INT PRIMARY KEY IDENTITY,
    RoleName NVARCHAR(100) UNIQUE NOT NULL
);

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