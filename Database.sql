create database db_Auction

use db_Auction

create table [Admin](
ad_id int primary key identity,
ad_email nvarchar(50) not null,
ad_username nvarchar(50) not null unique,
ad_password nvarchar(50) not null,
)

insert into [Admin] values('Admin@gmail.com','Admin','Admin123')

create table Customer(
c_id int primary key identity,
c_firstName nvarchar(50) not null,
c_lastName nvarchar(50) not null,
c_age nvarchar(50) not null,
c_phone bigint not null,
c_gender nvarchar(50) not null,
c_address nvarchar(max) not null,
c_image nvarchar(max) not null,
c_email nvarchar(50) not null,
c_userName nvarchar(50) not null unique,
c_password nvarchar(50) not null,
register_date varchar(50)
)

create table Category(
cat_id int primary key identity,
cat_name nvarchar(50) not null,
cat_image nvarchar(max) not null,
cat_status int not null
)

create table Product(
p_id int primary key identity,
p_name nvarchar(50) not null,
p_price int not null,
p_description nvarchar(max) not null,
p_dateOfCreation nvarchar(max) not null,
p_fk_cat int foreign key references Category(cat_id),
p_start_date nvarchar(50) not null,
p_end_date nvarchar(50) not null,
p_increment bigint not null,
p_featured_product int not null,
p_status int not null
)
select * from Product

create table images(
img_id int primary key identity,
img_text nvarchar(max) not null,
img_fk_product_id int foreign key references Product(p_id),
img_selected int default(0) not null
)

insert into images(img_text) values('/content/images/no-image.png')

create table Cart(
car_id int primary key identity,
car_fk_product_id int not null foreign key references Product(p_id),
car_fk_cus_id int not null foreign key references Customer(c_id),
)

create table [order](
o_id int primary key identity,
o_fk_cus_id int foreign key references Customer(c_id),
o_address varchar(max) not null,
o_phone bigint not null,
o_email varchar(50) not null,
o_zip int not null,
o_total_amout decimal not null,
o_status varchar(50) not null
)

create table order_details(
od_id int primary key identity,
od_fk_product_id int foreign key references Product(p_id),
od_fk_o int foreign key references [order](o_id)
)

create table Bids(
bid_id int primary key identity,
bid_fk_cus int foreign key references Customer(c_id),
bid_amount nvarchar(max) not null,
bid_timeStamp varchar(50) not null,
bid_fk_product int foreign key references Product(p_id)
)

create table Contact(
cont_id int primary key identity,
cont_name nvarchar(50) not null,
cont_email nvarchar(50) not null,
cont_message nvarchar(max) not null,
)

create table Product_View(
av_id int primary key identity,
av_fk_product_id int foreign key references Product(p_id),
av_veiws_count decimal
)

create table Wishlist(
w_id int primary key identity,
w_fk_cus_id int foreign key references Customer(c_id),
w_fk_product_id int foreign key references Product(p_id),
)
select * from [order]
select * from [order_details]