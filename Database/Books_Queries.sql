-------------------------DATE: 2081-12-30--------------------------------

GO
CREATE TABLE Users
(
UserId int NOT NULL IDENTITY(1,1) CONSTRAINT Users_pk PRIMARY KEY,
UserName varchar(30) NOT NULL,
Password varchar(500) NOT NULL,
RoleType int NOT NULL, --Role type 1 is admin, 2 is Member and 3 is normal user
Status bit NULL DEFAULT(1)
);

CREATE UNIQUE INDEX Users_UN_ui
ON Users(Username);
GO

GO
CREATE PROCEDURE sp_GetUserDataByUserName
@UserName varchar(30)
AS
BEGIN
	SELECT * FROM Users WHERE UserName = @UserName;
END
GO

GO
CREATE TABLE Products
(
ProductId int NOT NULL IDENTITY(1,1) CONSTRAINT Products_pk PRIMARY KEY,
ParentProductId int NULL CONSTRAINT Products_Products_fk REFERENCES Products(ProductId),
ProductName nvarchar(150) NOT NULL,
Description nvarchar(max) NULL,
Price money NULL,
OnDiscount bit NULL DEFAULT(0),
DiscountPrice money NULL,
IsParentProduct bit NULL DEFAULT(0),
Status bit NULL DEFAULT(1)
);
GO

GO
CREATE TABLE ProductCart
(
ProductCartId int NOT NULL IDENTITY(1,1) CONSTRAINT Cart_pk PRIMARY KEY,
ProductId int NOT NULL CONSTRAINT ProductCart_Products_fk REFERENCES Products(ProductId),
IsInCart bit NULL DEFAULT(0),
IsWish bit NULL DEFAULT(0),
IsPurchased bit NULL DEFAULT(0),
DiscountCode varchar NULL
);
GO

GO
CREATE TABLE Message
(
MessageId int NOT NULL IDENTITY(1,1) CONSTRAINT Message_pk PRIMARY KEY,
UserIds varchar(max) NULL,
Message varchar(256) NOT NULL,
RoleType int NULL,
Status int NULL DEFAULT(1),
EntryDate date NULL DEFAULT(GETDATE())
);
GO