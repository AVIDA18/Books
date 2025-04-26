-------------------------DATE: 2081-12-30--------------------------------
USE master

GO
drop database Booksdb
GO

GO
CREATE DATABASE Booksdb;
GO

GO
USE Booksdb;
GO

----Tables and stored procedures--------
GO
CREATE TABLE Users
(
UserId int NOT NULL IDENTITY(1,1) CONSTRAINT Users_pk PRIMARY KEY,
UserName varchar(30) NOT NULL,
FullName varchar(50) NOT NULL,
Password varchar(500) NOT NULL,
RoleType int NOT NULL, --Role type 1 is admin, 2 is Member and 3 is normal user
Status bit NULL DEFAULT(1)
);

CREATE UNIQUE INDEX Users_UN_ui
ON Users(Username);
GO

GO
CREATE PROCEDURE sp_RegisterUser
	@UserId int = NULL,
    @UserName varchar(30) = NULL,
    @FullName varchar(50) = NULL,
    @Password varchar(500) = NULL,
    @RoleType int = NULL,
	@Status bit = 0
AS
BEGIN
	IF @UserId IS NULL
	BEGIN
	    IF EXISTS (SELECT 1 FROM Users WHERE UserName = @UserName)
		BEGIN
			RAISERROR('Username already exists. Please choose a different one.', 16, 1);
			RETURN;
		END

		INSERT INTO Users (UserName, FullName, Password, RoleType, Status)
		VALUES (@UserName, @FullName, @Password, 3, 1)

		PRINT 'User created successfully.';
	END
	ELSE
	BEGIN
		UPDATE Users SET
			RoleType = @RoleType,
			Status = @Status
		WHERE UserId = @UserId
	END
END
GO

GO
CREATE PROCEDURE sp_GetUserDataByUserName
@UserName varchar(30)
AS
BEGIN
	SELECT * FROM Users WHERE UserName = @UserName AND Status = 1;
END
GO

GO
CREATE PROCEDURE sp_GetAllUsers
    @RoleType INT = NULL,
    @Status BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Users
    WHERE 
        (
            (@RoleType IS NULL AND RoleType <> 1) OR 
            (@RoleType IS NOT NULL AND RoleType = @RoleType)
        )
        AND (@Status IS NULL OR Status = @Status);
END
GO


GO
CREATE PROCEDURE sp_ChangePassword
@Password varchar(500),
@UserId int
AS
BEGIN
	UPDATE Users SET
		Password = @Password
	WHERE UserId = @UserId
END
GO

GO
CREATE TABLE Products
(
ProductId int NOT NULL IDENTITY(1,1) CONSTRAINT Products_pk PRIMARY KEY,
ParentProductId int NULL CONSTRAINT Products_Products_fk REFERENCES Products(ProductId),
ProductName nvarchar(150) NOT NULL,
Image varbinary(max) NULL,
Description nvarchar(max) NULL,
Price money NULL,
OnDiscount bit NULL DEFAULT(0),
DiscountPrice money NULL,
IsParentProduct bit NULL DEFAULT(0),
Status bit NULL DEFAULT(1)
);
GO

GO
CREATE VIEW vw_Products AS
SELECT 
    p.ProductId,
    p.ParentProductId,
    p.ProductName,
    p.Image,
    p.Description,
    p.Price,
    p.OnDiscount,
    p.DiscountPrice,
    p.IsParentProduct,
    p.Status,
    pp.ProductName AS ParentProductName
FROM Products p
LEFT JOIN Products pp ON p.ParentProductId = pp.ProductId;
GO


GO
CREATE PROCEDURE sp_SaveEditProducts
@ProductId int = NULL,
@ParentProductId int = NULL,
@ProductName nvarchar(150),
@Image varbinary(max) = NULL,
@Description nvarchar(max) = NULL,
@Price money = NULL,
@OnDiscount bit = 0,
@DiscountPrice money = NULL,
@IsParentProduct bit = 0,
@Status bit = 1
AS
BEGIN
	IF @ProductId IS NULL
	BEGIN
		INSERT INTO Products (ParentProductId, ProductName, Description, Price, OnDiscount, DiscountPrice, IsParentProduct, Status, Image)
		VALUES (@ParentProductId, @ProductName, @Description, @Price, @OnDiscount, @DiscountPrice, @IsParentProduct, @Status, @Image);
	END
	ELSE
	BEGIN
		UPDATE Products SET
			ParentProductId = @ParentProductId,
			ProductName = @ProductName,
			Image = CASE 
						WHEN @Image IS NOT NULL THEN @Image ELSE Image
					END,
			Description = @Description,
			Price = @Price,
			OnDiscount = @OnDiscount,
			DiscountPrice = @DiscountPrice,
			IsParentProduct = @IsParentProduct,
			Status = @Status
		WHERE ProductId = @ProductId
	END
END
GO

GO
CREATE PROCEDURE sp_SelectAllProductCategories
@Status bit
AS
BEGIN
	IF @Status = 0
	BEGIN
		SELECT ProductId AS ParentProductId, ProductName AS ParentProductName, Status FROM vw_Products WHERE IsParentProduct = 1;
	END
	ELSE
	BEGIN
		SELECT ProductId AS ParentProductId, ProductName AS ParentProductName, Status FROM vw_Products WHERE IsParentProduct = 1 AND Status = @Status;
	END
END
GO

GO
CREATE PROCEDURE sp_SelectAllProductsByProductCategory
    @ParentProductId INT = NULL,
    @Status BIT = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT *
    FROM vw_Products
    WHERE 
        (@ParentProductId IS NULL OR ParentProductId = @ParentProductId)
        AND (@Status IS NULL OR Status = @Status) AND IsParentProduct <> 1
    ORDER BY ProductId
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

GO
CREATE PROCEDURE sp_SearchAndSortProducts
    @SearchTerm NVARCHAR(150) = NULL,
    @ParentProductId INT = NULL,
    @Status BIT = NULL,
    @SortBy NVARCHAR(50) = 'ProductName',
    @SortOrder NVARCHAR(4) = 'ASC',
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    DECLARE @SQL NVARCHAR(MAX);

    SET @SQL = N'
    SELECT 
        ProductId,
        ParentProductId,
        ProductName,
        Image,
        Description,
        Price,
        OnDiscount,
        DiscountPrice,
        IsParentProduct,
        Status,
        ParentProductName
    FROM vw_Products
    WHERE IsParentProduct <> 1 AND 1 = 1';

    IF @SearchTerm IS NOT NULL
        SET @SQL += N' AND (ProductName LIKE ''%' + @SearchTerm + '%'' OR Description LIKE ''%' + @SearchTerm + '%'')';

    IF @ParentProductId IS NOT NULL
        SET @SQL += N' AND ParentProductId = ' + CAST(@ParentProductId AS NVARCHAR);

    IF @Status IS NOT NULL
        SET @SQL += N' AND Status = ' + CAST(@Status AS NVARCHAR);

    IF @SortBy NOT IN ('ProductName', 'Price', 'DiscountPrice')
        SET @SortBy = 'ProductName';

    IF @SortOrder NOT IN ('ASC', 'DESC')
        SET @SortOrder = 'ASC';

    SET @SQL += N' ORDER BY ' + QUOTENAME(@SortBy) + ' ' + @SortOrder;
    SET @SQL += N' OFFSET ' + CAST(@Offset AS NVARCHAR) + ' ROWS FETCH NEXT ' + CAST(@PageSize AS NVARCHAR) + ' ROWS ONLY';

    EXEC sp_executesql @SQL;
END
GO



GO
CREATE TABLE ProductCart
(
ProductCartId int NOT NULL IDENTITY(1,1) CONSTRAINT Cart_pk PRIMARY KEY,
UserId int NOT NULL CONSTRAINT ProductCart_Users_fk REFERENCES Users(UserId),
ProductId int NOT NULL CONSTRAINT ProductCart_Products_fk REFERENCES Products(ProductId),
Quantity int NOT NULL,
IsInCart bit NULL DEFAULT(0),
IsWish bit NULL DEFAULT(0),
ProductPrice money NOT NULL,
DiscountPrice money NULL,
Remarks varchar(500) NULL,
EntryDate dateTime NOT NULL DEFAULT(GETDATE())
);
GO

GO
CREATE TABLE ProductPurchase
(
PurchaseId int NOT NULL IDENTITY(1,1) CONSTRAINT ProductPurchase_pk PRIMARY KEY,
UserId int NOT NULL CONSTRAINT ProductPurchase_Users_fk REFERENCES Users(UserId),
ProductCartIds varchar(100) NOT NULL,
TotalPrice money NOT NULL,
DiscountCode varchar(10) NULL,
DiscountPrice money NULL,
PurchasePrice money NULL,
IsPurchased bit NOT NULL,
PurchaseDate dateTime NOT NULL DEFAULT(GETDATE()),
Remarks varchar(250) NULL
)
GO

GO
CREATE PROCEDURE sp_PurchaseCancelProduct
@PurchaseId int,
@UserId int,
@ProductCartIds varchar(100),
@TotalPrice money,
@DiscountCode varchar(10),
@DiscountPrice money,
@PurchasePrice money,
@IsPurchased bit,
@Remarks varchar(250)
AS
BEGIN
	IF @PurchaseId IS NULL
	BEGIN
		INSERT INTO ProductPurchase (UserId, ProductCartIds, TotalPrice, DiscountCode, DiscountPrice, PurchasePrice, IsPurchased, PurchaseDate, Remarks)
		VALUES (@UserId, @ProductCartIds, @TotalPrice, @DiscountCode, @DiscountPrice, @PurchasePrice, 1, GETDATE(), '');
	END
	ELSE
	BEGIN
		UPDATE ProductPurchase SET
			IsPurchased = 0,
			Remarks = @Remarks
		WHERE PurchaseId = @PurchaseId
	END
END
GO


GO
CREATE TABLE DiscountCodes
(
CodeId INT NOT NULL IDENTITY(1,1) CONSTRAINT DiscountCodes_pk PRIMARY KEY,
Code varchar(10) NOT NULL,
DiscountPercentage int NOT NULL,
ExpiryDate datetime NOT NULL
)
GO

GO
CREATE PROCEDURE sp_SaveEditDiscountCode
@CodeId int,
@Code varchar(10),
@DiscountPercentage int,
@ExpiryDate dateTime
AS
BEGIN
	IF @CodeId IS NULL
	BEGIN
		INSERT INTO DiscountCodes (Code, DiscountPercentage, ExpiryDate)
		VALUES (@Code, @DiscountPercentage, @ExpiryDate);
	END
	ELSE
	BEGIN
		UPDATE DiscountCodes SET
			Code = @Code,
			DiscountPercentage = @DiscountPercentage,
			ExpiryDate = @ExpiryDate
		WHERE CodeId = @CodeId
	END
END
GO

GO
CREATE PROCEDURE sp_SelectDiscountCodes
@Code varchar(10) = null
AS
BEGIN
	IF @Code IS NULL
	BEGIN
		SELECT * FROM DiscountCodes;
	END
	ELSE
	BEGIN
		SELECT * FROM DiscountCodes WHERE Code = @Code;
	END
END
GO


GO
CREATE TABLE Message
(
MessageId int NOT NULL IDENTITY(1,1) CONSTRAINT Message_pk PRIMARY KEY,
UserIds varchar(max) NULL,
Message varchar(256) NOT NULL,
RoleType int NULL,
ExpiryDate datetime NOT NULL,
EntryDate datetime NULL DEFAULT(GETDATE())
);
GO

GO
CREATE PROCEDURE sp_SendMessage
@MessageId int,
@UserIds varchar(max),
@Message varchar(256),
@RoleType int,
@ExpiryDate datetime
AS
BEGIN
	IF @MessageId IS NULL
	BEGIN
		INSERT INTO Message (UserIds, Message, RoleType, ExpiryDate, EntryDate)
		VALUES (@UserIds, @Message, @RoleType, @ExpiryDate, GETDATE())
	END
	ELSE
	BEGIN
		UPDATE Message SET
			UserIds = @UserIds,
			Message = @Message,
			Roletype = @RoleType,
			ExpiryDate = @ExpiryDate
		WHERE MessageId = @MessageId
	END
END
GO