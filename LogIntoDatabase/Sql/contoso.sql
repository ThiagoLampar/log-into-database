create table Employee(
	Id int primary key identity(1,1),
	FirstName varchar(60),
	LastName varchar(60),
	Age int
);

create table Log(
	Id int primary key identity(1,1),
	Before varchar(3000),
	After varchar(3000),
	Url varchar(150),
	Type varchar(15),
	DateUpdate datetime
);