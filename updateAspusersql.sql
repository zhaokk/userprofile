alter table AspNetUsers
add firstName varchar(255) default 'test' not null,
	lastName varchar(255) default 'test' not null,
	phoneNum int,
	email varchar(255),
	picture image,
	country nvarchar(255) default 'australia'  not null,
	postcode int default 2500 not null,
	street varchar(255) default 'crown'  not null,
	streetNumber int default '59'  not null,
	state varchar(5) default 'NSW'  not null,
	dob date default CURRENT_TIMESTAMP  not null ;


	alter table aspnetusers add constraint ASPUSER_AUS_STATE check (state in ('nsw', 'act', 'qld', 'vic', 'nt', 'sa', 'wa', 'tas'))