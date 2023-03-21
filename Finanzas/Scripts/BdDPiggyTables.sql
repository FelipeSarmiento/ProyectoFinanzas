create table dbo.Bills
(
    idBill          int identity
        primary key,
    idIcons         int,
    Name            varchar(100) not null,
    Cost            varchar(100),
    idMonth         int,
    idUser          int,
    idCategory      int,
    idPaymentMethod int,
    PaymentDate     varchar(100),
    Paid            varchar(10),
    Recurring       varchar(10)
)
go

create table dbo.Icons
(
    idIcons  int identity
        primary key,
    Code     varchar(20)  not null,
    CodeHTML varchar(100) not null
)
go

create table dbo.Categories
(
    idCategory int identity
        primary key,
    Name       varchar(20) not null,
    idIcon     int default 0
        references dbo.Icons,
    idUser     int
)
go

create table dbo.Months
(
    idMonth   int identity
        primary key,
    Name      varchar(15) not null,
    StartDate varchar(20),
    EndDate   varchar(20)
)
go

create table dbo.PaymentsMethods
(
    idPaymentMethod int identity
        primary key,
    Name            varchar(30) not null,
    idUser          int
)
go

create table dbo.RecurringBills
(
    idRecurringBill int not null,
    idIcon          int,
    Name            varchar(100),
    Cost            int,
    idUser          int,
    idCategory      int,
    idPaymentMethod int,
    FromDate        varchar(100),
    ToDate          varchar(100),
    PaymentDate     varchar(100)
)
go

create unique clustered index RecurringBills_idRecurringBill_uindex
    on dbo.RecurringBills (idRecurringBill)
go

create table dbo.Users
(
    idUser   int identity
        primary key,
    Name     varchar(50)  not null,
    Email    varchar(50)  not null,
    Password varchar(100) not null
)
go

create table dbo.Balance
(
    idBalance int identity
        primary key,
    Income    varchar(50),
    Expenses  varchar(50),
    Remaining varchar(50),
    idUser    int
        references dbo.Users,
    idMonth   int
        references dbo.Months
)
go

