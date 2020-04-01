USE MASTER
GO

IF NOT EXISTS (
    SELECT [name]
    FROM sys.databases
    WHERE [name] = N'BangazonAPI'
)
CREATE DATABASE BangazonAPI
GO

USE BangazonAPI
GO

DROP TABLE IF EXISTS OrderProduct;
DROP TABLE IF EXISTS [Order];
DROP TABLE IF EXISTS UserPaymentType;
DROP TABLE IF EXISTS EmployeeTraining;
DROP TABLE IF EXISTS OrderProduct;
DROP TABLE IF EXISTS TrainingProgram;
DROP TABLE IF EXISTS Employee;
DROP TABLE IF EXISTS Department;
DROP TABLE IF EXISTS Computer;
DROP TABLE IF EXISTS Product;
DROP TABLE IF EXISTS ProductType;
DROP TABLE IF EXISTS Customer;
DROP TABLE IF EXISTS PaymentType;

CREATE TABLE Department (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(55) NOT NULL,
	Budget 	INTEGER NOT NULL
);

CREATE TABLE Computer (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	PurchaseDate DATETIME NOT NULL,
	DecomissionDate DATETIME,
	Make VARCHAR(55) NOT NULL,
	Model VARCHAR(55) NOT NULL
);

CREATE TABLE Employee (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	FirstName VARCHAR(55) NOT NULL,
	LastName VARCHAR(55) NOT NULL,
	DepartmentId INTEGER NOT NULL,
	Email VARCHAR(55) NOT NULL,
	IsSupervisor BIT NOT NULL DEFAULT(0),
	ComputerId INTEGER NOT NULL,
    CONSTRAINT FK_EmployeeDepartment FOREIGN KEY(DepartmentId) REFERENCES Department(Id),
	CONSTRAINT FK_EmployeeComputer FOREIGN KEY (ComputerId) REFERENCES Computer(Id)
);

CREATE TABLE TrainingProgram (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(255) NOT NULL,
	StartDate DATETIME NOT NULL,
	EndDate DATETIME NOT NULL,
	MaxAttendees INTEGER NOT NULL
);

CREATE TABLE EmployeeTraining (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	EmployeeId INTEGER NOT NULL,
	TrainingProgramId INTEGER NOT NULL,
    CONSTRAINT FK_EmployeeTraining_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_EmployeeTraining_Training FOREIGN KEY(TrainingProgramId) REFERENCES TrainingProgram(Id)
);

CREATE TABLE ProductType (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(55) NOT NULL
);

CREATE TABLE Customer (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	FirstName VARCHAR(55) NOT NULL,
	LastName VARCHAR(55) NOT NULL,
	CreatedDate DATETIME NOT NULL,
	Active BIT NOT NULL DEFAULT(1),
	[Address] VARCHAR(255) NOT NULL,
	City VARCHAR(55) NOT NULL,
	[State] VARCHAR(55) NOT NULL,
	Email VARCHAR(55) NOT NULL,
	Phone VARCHAR(55) NOT NULL
);

CREATE TABLE Product (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	DateAdded DATETIME NOT NULL,
	ProductTypeId INTEGER NOT NULL,
	CustomerId INTEGER NOT NULL,
	Price MONEY NOT NULL,
	Title VARCHAR(255) NOT NULL,
	[Description] VARCHAR(255) NOT NULL,
    CONSTRAINT FK_Product_ProductType FOREIGN KEY(ProductTypeId) REFERENCES ProductType(Id),
    CONSTRAINT FK_Product_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);

CREATE TABLE PaymentType (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(55) NOT NULL,
	Active BIT NOT NULL DEFAULT(1)
);

CREATE TABLE UserPaymentType (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	AcctNumber VARCHAR(55) NOT NULL,
	Active BIT NOT NULL DEFAULT(1),
	CustomerId INTEGER NOT NULL,
	PaymentTypeId INTEGER NOT NULL,
    CONSTRAINT FK_UserPaymentType_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id),
	CONSTRAINT FK_UserPaymentType_PaymentType FOREIGN KEY(PaymentTypeId) REFERENCES PaymentType(Id)
);

CREATE TABLE [Order] (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	CustomerId INTEGER NOT NULL,
	UserPaymentTypeId INTEGER,
    CONSTRAINT FK_Order_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id),
    CONSTRAINT FK_Order_Payment FOREIGN KEY(UserPaymentTypeId) REFERENCES UserPaymentType(Id)
);

CREATE TABLE OrderProduct (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	OrderId INTEGER NOT NULL,
	ProductId INTEGER NOT NULL,
    CONSTRAINT FK_OrderProduct_Product FOREIGN KEY(ProductId) REFERENCES Product(Id),
    CONSTRAINT FK_OrderProduct_Order FOREIGN KEY(OrderId) REFERENCES [Order](Id)
);

INSERT INTO Department([Name],Budget) VALUES ('Marketing',115000);
INSERT INTO Department([Name],Budget) VALUES ('Engineering',197200);
INSERT INTO Department([Name],Budget) VALUES ('Accounting',90000);
INSERT INTO Department([Name],Budget) VALUES ('Legal',123000);
INSERT INTO Department([Name],Budget) VALUES ('Human Resources',103000);
INSERT INTO Department([Name],Budget) VALUES ('Customer Service',100000);

INSERT INTO Computer(PurchaseDate,DecomissionDate,Make,Model) VALUES ('2016-01-01T23:28:56.782Z',NULL,'Apple','Macbook Pro');
INSERT INTO Computer(PurchaseDate,DecomissionDate,Make,Model) VALUES ('2018-02-09T23:28:56.782Z',NULL,'Apple','Macbook Air');
INSERT INTO Computer(PurchaseDate,DecomissionDate,Make,Model) VALUES ('2019-05-05T23:28:56.782Z',NULL,'Microsoft','Suface Pro');
INSERT INTO Computer(PurchaseDate,DecomissionDate,Make,Model) VALUES ('2014-01-01T23:28:56.782Z','2017-03-01T23:28:56.782Z','Lenovo','Thinkpad X1');
INSERT INTO Computer(PurchaseDate,DecomissionDate,Make,Model) VALUES ('2016-01-01T23:28:56.782Z',NULL,'Apple','Macbook Pro');
INSERT INTO Computer(PurchaseDate,DecomissionDate,Make,Model) VALUES ('2016-01-01T23:28:56.782Z',NULL,'System 76','Oryx Pro');
INSERT INTO Computer(PurchaseDate,DecomissionDate,Make,Model) VALUES ('2016-01-01T23:28:56.782Z',NULL,'System 76','Gazelle');
INSERT INTO Computer(PurchaseDate,DecomissionDate,Make,Model) VALUES ('2016-01-01T23:28:56.782Z',NULL,'System 76','Oryx Pro Lite');
INSERT INTO Computer(PurchaseDate,DecomissionDate,Make,Model) VALUES ('2016-01-01T00:00:00.000Z','1970-01-01T00:00:00.000Z','System 76','Oryx Pro Lite');

INSERT INTO Employee(FirstName,LastName,DepartmentId,IsSupervisor,ComputerId,Email) VALUES ('Adam','Sheaffer',1,0,4,'iamtheboss@bangazon.com');
INSERT INTO Employee(FirstName,LastName,DepartmentId,IsSupervisor,ComputerId,Email) VALUES ('Madi','Peper',2,1,3,'everythingisawesome@bangzon.com');
INSERT INTO Employee(FirstName,LastName,DepartmentId,IsSupervisor,ComputerId,Email) VALUES ('Andy','Collins',3,1,2,'losinghorn@bangazon.com');
INSERT INTO Employee(FirstName,LastName,DepartmentId,IsSupervisor,ComputerId,Email) VALUES ('Coach','Steve',4,1,1,'coach@bangazon.com');

INSERT INTO TrainingProgram([Name],StartDate,EndDate,MaxAttendees) VALUES ('GIS Application','2018-09-25T00:00:00.000Z','2018-10-05T00:00:00.000Z',45);
INSERT INTO TrainingProgram([Name],StartDate,EndDate,MaxAttendees) VALUES ('Business Process','2019-05-25T00:00:00.000Z','2018-05-26T00:00:00.000Z',100);
INSERT INTO TrainingProgram([Name],StartDate,EndDate,MaxAttendees) VALUES ('Spanish 101','2019-09-25T00:00:00.000Z','2019-10-05T00:00:00.000Z',20);
INSERT INTO TrainingProgram([Name],StartDate,EndDate,MaxAttendees) VALUES ('Application Architecture','2020-02-15T00:00:00.000Z','2020-02-28T00:00:00.000Z',15);
INSERT INTO TrainingProgram([Name],StartDate,EndDate,MaxAttendees) VALUES ('Ethical Hacking','2020-02-16T00:00:00.000Z','2020-02-28T00:00:00.000Z',15);

INSERT INTO EmployeeTraining(EmployeeId,TrainingProgramId) VALUES (1,1);
INSERT INTO EmployeeTraining(EmployeeId,TrainingProgramId) VALUES (2,1);
INSERT INTO EmployeeTraining(EmployeeId,TrainingProgramId) VALUES (2,2);
INSERT INTO EmployeeTraining(EmployeeId,TrainingProgramId) VALUES (3,3);
INSERT INTO EmployeeTraining(EmployeeId,TrainingProgramId) VALUES (4,2);

INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (1,'Nathanael','Laverenz','401 Nunya Business Dr','Herman','New York','n.lav@sbcglobal.net','6151237584','2018-09-25T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (1,'Chrissy','Vivian','302 Puppy Way','Nashville','Tennessee','vivi@gmail.com','5782036593','2018-09-25T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (1,'Halli','Storten','404 Outtamai Way','Los Angelos','California','halliday@hotmail.com','2893750183','2018-09-25T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (1,'Godfree','Chase','500 Internal Cir','Topeka','Kansas','eerfdogesahc@gmail.com','1238693029','2018-09-25T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (1,'Willi','Warnes','418 Teapot Way','Seattle','Washington','willi.warnes@yahoo.com','7693025473','2018-09-25T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (1,'Salmon','O''''Nions','100 Continue Blvd','Atlanta','Georgia','salmonstriker@gmail.com','6151237584','2018-09-25T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (1,'Kimble','Peskett','508 Loop Cir','Nashville','Tennessee','peskykimble@hotmail.com','5671234567','2018-09-25T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (0,'Laura','Darner','504 Timeout Way','New York City','New York','laura.d@yahoo.com','1987654321','2018-09-25T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (1,'Gal','Fieri','999 Ranch Drive','Flavortown','New Jersey','zesty_flames88@hotmail.com','14132278989','2020-01-09T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (1,'Sven','Glambus','3203 Lover''''s Lane','Booleanville','Minnesota','glambus.sven@hotmail.com','13239862981','2019-07-03T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (1,'Treldo','Smith','444444446 Paul Rigggggaaaaal Blvd','Rolo Tony','Arkansas','spaghett@hotmail.com','14439865181','2019-07-03T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (1,'Sven','Glambus','3203 Lover','Booleanville','Minnesota','glambus.sven@hotmail.com','19862981','2019-07-03T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (0,'Varnald','Von Fig','308 N Duke St','Durham','North Carolina','swedishmeatballLuvr@hotmail.com','18739362955','2018-08-23T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (0,'Frelldus','Avocado','43 Trout Lane','Bellingham','South Carolina','toast_is_most3357@hotmail.com','14139362955','2018-08-23T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (1,'Helen','Von Fig','308 N Duke St','Durham','North Carolina','swedishmeatballH8r@hotmail.com','14029382955','2018-08-23T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (0,'George','Jones','128 2nd Ave N','Nashville','Tennessee','swedishmeatballLuvr@hotmail.com','12229362955','2018-08-23T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (0,'Horace','Smith','564 S Roxboro St','Durham','North Carolina','golfpal@yahoo.com','14135723987','2018-08-23T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (1,'Jackie','Smith','52 Prospect Court','Northampton','Massachusetts','freejazzfan22@hotmail.com','14134423987','2018-08-23T00:00:00.000Z');
INSERT INTO Customer(Active,FirstName,LastName,[Address],City,[State],Email,Phone, CreatedDate) VALUES (1,'Glanardo','Kiwi','','Durham','North Carolina','golfpal@yahoo.com','14135723987','2018-08-23T00:00:00.000Z');

INSERT INTO ProductType([Name]) VALUES ('Tanks');
INSERT INTO ProductType([Name]) VALUES ('Accessories');
INSERT INTO ProductType([Name]) VALUES ('Stands');
INSERT INTO ProductType([Name]) VALUES ('Filters');
INSERT INTO ProductType([Name]) VALUES ('Pebbles');
INSERT INTO ProductType([Name]) VALUES ('Food');
INSERT INTO ProductType([Name]) VALUES ('Cleaning Supplies');
INSERT INTO ProductType([Name]) VALUES ('Bowls');
INSERT INTO ProductType([Name]) VALUES ('Filters');
INSERT INTO ProductType([Name]) VALUES ('Plants');
INSERT INTO ProductType([Name]) VALUES ('Heaters');

INSERT INTO Product(ProductTypeId,CustomerId,Price,[Description],Title,DateAdded) VALUES (1,3,176.91,'20 Gallon tank with heater and plant accessories','Tank/Aqarium','2019-08-25T00:00:00.000Z');
INSERT INTO Product(ProductTypeId,CustomerId,Price,[Description],Title,DateAdded) VALUES (1,2,362.54,'80 Gallon black acrylic aquarium with stand','Tank/Aquarium','2018-12-25T00:00:00.000Z');
INSERT INTO Product(ProductTypeId,CustomerId,Price,[Description],Title,DateAdded) VALUES (6,1,79.32,'Dry pellet fish food designed to sink. Ideal for most saltwater fish','Pellet Fish Food (24oz)','2018-09-16T00:00:00.000Z');
INSERT INTO Product(ProductTypeId,CustomerId,Price,[Description],Title,DateAdded) VALUES (4,3,110.94,'Canister filter for freshwater and marine environments. Recommended for aquariums up to 100 gallons.','Cascade Canister Filter','2019-12-05T00:00:00.000Z');
INSERT INTO Product(ProductTypeId,CustomerId,Price,[Description],Title,DateAdded) VALUES (5,5,66.57,'5 pound florencent colored gravel','Gravel (5lb bag)','2019-06-14T00:00:00.000Z');
INSERT INTO Product(ProductTypeId,CustomerId,Price,[Description],Title,DateAdded) VALUES (5,3,79.92,'Naturally pollished gravel. Assorted colors','Decorative Pebbles','2019-09-25T00:00:00.000Z');
INSERT INTO Product(ProductTypeId,CustomerId,Price,[Description],Title,DateAdded) VALUES (2,6,29.99,'Assorted pirate themed accessories for small bowl','Pirate Accessories','2019-09-25T00:00:00.000Z');
INSERT INTO Product(ProductTypeId,CustomerId,Price,[Description],Title,DateAdded) VALUES (6,7,40.93,'Dry pellet fish food designed to sink. 1LB bag','Pellet Fish Food','2019-09-25T00:00:00.000Z');
INSERT INTO Product(ProductTypeId,CustomerId,Price,[Description],Title,DateAdded) VALUES (1,5,353.45,'60 Gallon black acrylic aquarium with gold colored stand','Aquarium','2019-09-25T00:00:00.000Z');
INSERT INTO Product(ProductTypeId,CustomerId,Price,[Description],Title,DateAdded) VALUES (10,2,118.28,'Perfect for 10+ gallons aquariums. Create a natural habitat in your aquarium for your fish and invertebrates. Provide natural resting and hiding places for your smaller fish and invertebrates','Aquarium Plant Bundle (5 species)','2019-09-25T00:00:00.000Z');
INSERT INTO Product(ProductTypeId,CustomerId,Price,[Description],Title,DateAdded) VALUES (4,7,72.25,'Indoor & Outdoor use, Great for Bird Baths, lions head & Vase fountains and aquariums alike. NdFeB Magnets, aluminum oxide ceramic impeller shaft and epoxy resin guarantee the pump can work over 20,000 hours.','Submersible Water Pump with 6ft Power Cord','2019-09-25T00:00:00.000Z');
INSERT INTO Product(ProductTypeId,CustomerId,Price,[Description],Title,DateAdded) VALUES (11,5,137.99,'Designed for a 30 - 60 gallon tank. Made with premium quartz and double sealed by black protective guard, this fish tank heater is resistant to explosion and corrosion, and protect fish and other livestocks from punching and scalding','300W Heater with LED Temp Control','2019-09-25T00:00:00.000Z');
INSERT INTO Product(ProductTypeId,CustomerId,Price,[Description],Title,DateAdded) VALUES (2,6,44.14,'10 Peach colored seashells in small and medium sizes','Seashell Accessories','2019-09-25T00:00:00.000Z');
INSERT INTO Product(ProductTypeId,CustomerId,Price,[Description],Title,DateAdded) VALUES (4,6,151.05,'Canister filter for freshwater and marine environments. Recommended for aquariums up to 100 gallons.','Cascade Canister Filter','2019-09-25T00:00:00.000Z');
INSERT INTO Product(ProductTypeId,CustomerId,Price,[Description],Title,DateAdded) VALUES (8,5,67.83,'5 Gallon small round bowl','Round Bowl','2019-09-25T00:00:00.000Z');

INSERT INTO PaymentType([Name],Active) VALUES ('Mastercard',1);
INSERT INTO PaymentType([Name],Active) VALUES ('Visa',1);
INSERT INTO PaymentType([Name],Active) VALUES ('Discover',1);
INSERT INTO PaymentType([Name],Active) VALUES ('American Express',1);
INSERT INTO PaymentType([Name],Active) VALUES ('Diners Club',0)

INSERT INTO UserPaymentType(CustomerId,PaymentTypeId,AcctNumber,Active) VALUES (1,5,'2234 56789 2123',1);
INSERT INTO UserPaymentType(CustomerId,PaymentTypeId,AcctNumber,Active) VALUES (2,2,'1234 5678 9012',1);
INSERT INTO UserPaymentType(CustomerId,PaymentTypeId,AcctNumber,Active) VALUES (1,2,'1234 56789 4123',1);
INSERT INTO UserPaymentType(CustomerId,PaymentTypeId,AcctNumber,Active) VALUES (4,1,'1234 56789 5123',1);
INSERT INTO UserPaymentType(CustomerId,PaymentTypeId,AcctNumber,Active) VALUES (4,3,'1234 56789 5163',1);
INSERT INTO UserPaymentType(CustomerId,PaymentTypeId,AcctNumber,Active) VALUES (3,1,'1234 56789 5123',1);
INSERT INTO UserPaymentType(CustomerId,PaymentTypeId,AcctNumber,Active) VALUES (5,1,'1234 56789 6123',1);
INSERT INTO UserPaymentType(CustomerId,PaymentTypeId,AcctNumber,Active) VALUES (6,3,'1234 56789 7123',1);
INSERT INTO UserPaymentType(CustomerId,PaymentTypeId,AcctNumber,Active) VALUES (7,2,'1234 56789 8123',1);
INSERT INTO UserPaymentType(CustomerId,PaymentTypeId,AcctNumber,Active) VALUES (8,1,'1234 56789 9123',1);
INSERT INTO UserPaymentType(CustomerId,PaymentTypeId,AcctNumber,Active) VALUES (9,2,'1234 56789 0123',1);
INSERT INTO UserPaymentType(CustomerId,PaymentTypeId,AcctNumber,Active) VALUES (10,1,'1234 56789 2223',1);

INSERT INTO [Order](CustomerId,UserPaymentTypeId) VALUES (1,NULL);
INSERT INTO [Order](CustomerId,UserPaymentTypeId) VALUES (2,2);

INSERT INTO OrderProduct(OrderId,ProductId) VALUES (1,2);
INSERT INTO OrderProduct(OrderId,ProductId) VALUES (1,4);
INSERT INTO OrderProduct(OrderId,ProductId) VALUES (1,3);
INSERT INTO OrderProduct(OrderId,ProductId) VALUES (1,1);
INSERT INTO OrderProduct(OrderId,ProductId) VALUES (2,2);
INSERT INTO OrderProduct(OrderId,ProductId) VALUES (2,1);