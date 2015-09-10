
USE []
/*create a database called RAO_DB */
GO


/*Users of the system */
create table AspNetUsers (
	Id nvarchar(128) NOT NULL,
	ffaNum int UNIQUE NULL,
	UserName varchar(255) UNIQUE NOT NULL,
	PasswordHash nvarchar(max) not null,
	SecurityStamp nvarchar(max) NULL,
	firstName varchar(255) not null,
	lastName varchar(255) not null,
	phoneNum int NULL,
	email varchar(255) not null,
	photoDir nvarchar(max) NULL,
	Discriminator nvarchar(128) NOT NULL,
	country nvarchar(255) not null DEFAULT ('Australia'),
	postcode int not null,
	street varchar(255) not null,
	streetNumber int not null,
	[state] varchar(5) not null,
	dob date not null,
	isAdmin bit DEFAULT ((0)) not null,
	isOrganizer bit DEFAULT ((0)) not null,
	isReferee bit DEFAULT ((0)) not null,
	isPlayer bit DEFAULT ((0)) not null,
	willingToShowPhoneNum bit DEFAULT((0)) NOT NULL,
	willingToShowDOB bit DEFAULT((0)) NOT NULL,
	willingToShowAddress bit DEFAULT((0)) NOT NULL,
	willingToshowEmail bit DEFAULT((0)) NOT NULL,
	constraint USER_AUS_STATE check(state in ('nsw', 'act', 'qld', 'vic', 'nt', 'sa', 'wa', 'tas')),
	constraint USER_AUS_PCODE check(postcode <= 9999 AND postcode >= 0),
	CONSTRAINT PK_dbo_AspNetUsers PRIMARY KEY CLUSTERED(Id),
	CONSTRAINT aspuser_unique UNIQUE NONCLUSTERED(UserName)
);
GO


/*user authentication tables */
create table AspNetRoles(
	Id nvarchar(128) NOT NULL,
	Name nvarchar(max) NOT NULL,
	CONSTRAINT PK_dbo_AspNetRoles PRIMARY KEY CLUSTERED (Id)

);



create table AspNetUserClaims(
	Id int IDENTITY(1,1) NOT NULL,
	ClaimType nvarchar(max) NULL,
	ClaimValue nvarchar(max) NULL,
	User_Id nvarchar(128) NOT NULL,
	CONSTRAINT PK_dbo_AspNetUserClaims PRIMARY KEY CLUSTERED (Id),
	constraint FK_AspNetUserClaims_dbo_User_Id foreign key (User_Id) references AspNetUsers (Id),
);
GO
/****** Object:  Index [IX_User_Id] ******/
CREATE NONCLUSTERED INDEX [IX_User_Id] ON [dbo].[AspNetUserClaims]
(
	[User_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO



create table AspNetUserLogins(
	UserId nvarchar(128) NOT NULL,
	LoginProvider nvarchar(128) NOT NULL,
	ProviderKey nvarchar(128) NOT NULL,
	CONSTRAINT PK_dbo_AspNetUserLogins PRIMARY KEY CLUSTERED (UserId, LoginProvider, ProviderKey),
	constraint FK_AspNetUserLogins_dbo_User_Id foreign key (UserId) references AspNetUsers (Id)
);
GO
/****** Object:  Index [IX_UserId] ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

create table AspNetUserRoles(
	UserId nvarchar(128) NOT NULL,
	RoleId nvarchar(128) NOT NULL,
	CONSTRAINT PK_dbo_AspNetUserRoles PRIMARY KEY CLUSTERED(UserId, RoleId),
	constraint FK_AspNetUserRoles_dbo_Role_Id foreign key (RoleId) references AspNetRoles (Id),
	constraint FK_AspNetUserRoles_dbo_User_Id foreign key (UserId) references AspNetUsers (Id)
);

GO
CREATE NONCLUSTERED INDEX [IX_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


/****** Object:  Index [IX_UserId]    Script Date: 5/08/2015 18:00:38 ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserRoles]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


create table LOCATION(
	locationId int  identity(1,1) NOT NULL,
	name varchar(50) not null,
	price float(53) null,
	street varchar(20) not null,
	streetNum int not null,
	city varchar(25) not null,
	postcode int not null,
	phoneNum int not null,
	country nvarchar(255) not null DEFAULT ('Australia'),
	[state] varchar(50),
	CONSTRAINT PK_dbo_LOCATION PRIMARY KEY CLUSTERED(locationId),
	constraint LOCATION_AUS_PCODE check(postcode <= 9999 AND postcode >= 0),
	constraint LOCATION_AUS_STATE check(state in ('NSW', 'ACT', 'QLD', 'VIC', 'NT', 'SA', 'WA', 'TAS'))
);
GO


create table SPORT(
	name varchar(100) NOT NULL,
	constraint PK_MATCH_QUAL1 primary key CLUSTERED(name)
);
GO

create table TOURNAMENT(
	tournamentId int identity(1,1) NOT NULL,
	name nvarchar(128) not null,
	startDate date NOT NULL,
	organizer varchar(255) NOT NULL,
	ageBracket int not null,
	grade int not null,
	sport varchar(100)NOT NULL,
	constraint fk_TOURNAMENT_SPORT foreign key (sport) references SPORT (name),
	constraint fk_TOURNAMENT_MANAGER foreign key (organizer) references AspNetUsers (UserName),
	CONSTRAINT TOURNAMENT_DATE_CHECK check(startDate > GETDATE()),
	CONSTRAINT PK_dbo_TOURNAMENT PRIMARY KEY CLUSTERED(tournamentId),
);
GO

create table REFEREE(
	refId int NOT NULL,
	distTravel int,
	sport varchar(100) not null,
	prefAge int,
	prefGrade int,
	userId nvarchar(128) not null,	
	maxGames int not null DEFAULT ((4)),
	CONSTRAINT PK_dbo_REFEREE PRIMARY KEY CLUSTERED(refID),
	constraint FK_REFEREE_USER foreign key (userId) references AspNetUsers (Id),
	CONSTRAINT REFEREE_GAMES_CHECK check(maxGames >= 0),
	constraint fk_REFEREE_SPORT foreign key (sport) references SPORT (name)
);
GO

CREATE TABLE WILLINGLOCATIONS(
	refId int not null,
	locationId int not null,
	constraint FK_WILLINGLOCATION_REF foreign key (refId) references REFEREE (refID),
	constraint FK_WILLINGLOCATION_LOCATION foreign key (locationId) references LOCATION (locationId),
	CONSTRAINT PK_dbo_WILLINGLOCATIONS PRIMARY KEY CLUSTERED(refId, locationId)
);

create table TEAM(
	teamId int identity(1,1) NOT NULL,
	name varchar(100) UNIQUE,
	ageBracket int not null,
	grade int not null,
	sport varchar(100) not null,
	managerID varchar(255) not null,
	CONSTRAINT PK_dbo_TEAM PRIMARY KEY CLUSTERED(teamId),
	constraint FK_TEAM_MANAGER foreign key (managerID) references AspNetUsers (UserName),
	constraint fk_TEAM_SPORT foreign key (sport) references SPORT (name)
	--constraint fk_TEAM_TOURNAMENT foreign key (tournament) references TOURNAMENT (tID)
);
GO

create table TEAMINS(
	teamID int not null,
	tournament int not null,
	--sport varchar(100) not null,
	--tournamentDate date not null,
	--	constraint fk_TEAM_TOURNAMENT_DATE foreign key (tournamentDate) references TOURNAMENT (tDate),
	constraint fk_TEAM_TOURNAMENT foreign key (tournament) references TOURNAMENT (tournamentId),
	--constraint fk_TOURNAMENT_SPORT foreign key (sport) references TOURNAMENT (sport),
	constraint FK_TEAMINS_TEAM foreign key (teamID) references TEAM (teamID),
	CONSTRAINT PK_dbo_TEAMINS PRIMARY KEY CLUSTERED(teamID, tournament)
);

create table PLAYER(
	teamId int not null,
	userId nvarchar(128) not null,
	position varchar(20),
	shirtNum int,
	constraint fk_PLAYER_TEAM foreign key (teamId) references TEAM (teamId),
	constraint fk_PLAYER_USERNAME foreign key (userId) references AspNetUsers (Id),
	constraint PK_PLAYER1 primary key CLUSTERED(teamId, userId)
	[name] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

);
GO



create table MATCH(
	matchId int identity(1,1),
	matchDate datetime not null,
	locationId int,
	teamaId int NOT NULL,
	teambId int NOT NULL,
	teamAScore int null DEFAULT ((0)),
	teamBScore int null DEFAULT ((0)),
	winnerId int null, -- 0 for a, 1 for b, 2 for draw.
	tournamentId int NOT NULL,
	matchLength int DEFAULT((90)) not null, --in mins
	CONSTRAINT PK_dbo_MATCH PRIMARY KEY CLUSTERED(matchId),
	constraint fk_MATCH_LOCATION foreign key (locationId) references LOCATION (locationId),
	CONSTRAINT MATCH_DATE_CHECK check(matchDate > GETDATE()),
	constraint fk_MATCH_TEAMA foreign key (teamaId) references TEAM (teamId),
	constraint fk_MATCH_TEAMB foreign key (teambId) references TEAM (teamId),
	constraint fk_MATCH_TOURNAMENT foreign key (tournamentId) references TOURNAMENT (tournamentId)
);
GO

create table INFRACTIONS(
	infractionId int identity(1,1),
	[type] varchar(20) not null,
	status int not null DEFAULT ((0)),
	matchID int not null,
	player nvarchar(128) not null,
	expiryDate date not null,
	CONSTRAINT PK_dbo_INFRACTIONS PRIMARY KEY CLUSTERED(infractionId),
	constraint fk_INFRACTION_MATCH foreign key (matchID) references MATCH (matchId),
	constraint fk_INFRACTION_PLAYER foreign key (player) references AspNetUsers (ffaNum) 
);
GO

create table OFFER(
	offerId int identity(1,1) NOT NULL,
	sport varchar(100) not null,
	matchId int not null,
	refID int not null,
	status int not null DEFAULT ((0)),
	dateOfOffer date not null,
	declinedReason nvarchar(max) null,
	constraint fk_OFFER_MATCH foreign key (matchId) references MATCH (matchId),
	constraint fk_OFFER_refID foreign key (refID) references REFEREE (refID),
	CONSTRAINT OFFER_STATUS_CHECK check(status >= 0 AND status <=5),
	CONSTRAINT PK_dbo_OFFER PRIMARY KEY CLUSTERED(offerId),
	constraint fk_OFFER_SPORT foreign key (sport) references SPORT (name)
);
GO

CREATE TABLE ALTERNATEOFFERS(
	offerId int not null,
	refId int not null,
	constraint fk_ALTERNATEOFFERS_refID foreign key (refId) references REFEREE (refId),
	constraint fk_ALTERNATEOFFERS_offID foreign key (offerId) references OFFER (offerId),
	CONSTRAINT PK_dbo_ALTOFFER PRIMARY KEY CLUSTERED(refId, offerId)
);


GO
create table WEEKLYAVAILABILITY(
	refId int NOT NULL,
	monday int not null DEFAULT ((0)),
	tuesday int not null DEFAULT ((0)),
	wednesday int not null DEFAULT ((0)),
	thursday int not null DEFAULT ((0)),
	friday int not null DEFAULT ((0)),
	saturday int not null DEFAULT ((0)),
	sunday int not null DEFAULT ((0)),
	constraint FK_WEEKLYAVAILABILITY_USER foreign key (refId) references REFEREE (refId),
	CONSTRAINT WEEKLY_MONDAY check(monday >= 0 AND monday <=6),
	CONSTRAINT WEEKLY_tuesday check(tuesday >= 0 AND tuesday <=6),
	CONSTRAINT WEEKLY_wednesday check(wednesday >= 0 AND wednesday <=6),
	CONSTRAINT WEEKLY_thursday check(thursday >= 0 AND thursday <=6),
	CONSTRAINT WEEKLY_friday check(friday >= 0 AND friday <=6),
	CONSTRAINT WEEKLY_saturday check(saturday >= 0 AND saturday <=6),
	CONSTRAINT WEEKLY_sunday check(sunday >= 0 AND sunday <=6),
	CONSTRAINT PK_dbo_WEEKLY PRIMARY KEY CLUSTERED(refId)
);


GO
create table OneOffAVAILABILITY(
	refId int NOT NULL,
	startDate date NOT NULL,
	allDay bit NULL,
	timeOnOrOff bit NOT null, --0 for time on, 1 for time off
	[description] varchar(255) NULL,
	constraint FK_TIMEOFF_USER foreign key (refId) references REFEREE (refId),
	CONSTRAINT OFFER_DATES_CHECK check(endDate > startDate),
	CONSTRAINT OFFER_START_DATE_CHECK check(startDate > GETDATE()),
	CONSTRAINT PK_dbo_TIMEOFF PRIMARY KEY CLUSTERED(refId, startDate)
);
GO

create table [TYPE](
	typeId int identity(1,1) NOT NULL,
	offerId int not null,
	name varchar(255) not null,
	description varchar(255) null,
	constraint FK_TYPE_OFFER foreign key (offerId) references OFFER (offerId),
	CONSTRAINT PK_dbo_TYPE PRIMARY KEY CLUSTERED(typeId)
);
GO


create table QUALIFICATIONS(
	qualificationId int identity(1,1) NOT NULL,
	name varchar(100) not null,
	sport varchar(100) not null,
	description nvarchar(128),
	qualificationLevel int not null, --is the bounds that a level can be up to
	constraint fk_QUAL_SPORT foreign key (sport) references SPORT (name),
	CONSTRAINT PK_dbo_QUALIFICATIONS PRIMARY KEY CLUSTERED(qualificationId)
);
GO

create table USERQUAL(
	qualificationId int,
	refID int,
	qualLevel int,
	constraint fk_USER_QUAL foreign key (qualificationId) references QUALIFICATIONS (qualificationId),
	constraint fk_USER foreign key (refID) references REFEREE (refID),
	constraint PK_USER_QUAL1 primary key CLUSTERED(qualificationId, refID)
);
GO

create table OFFERQUAL(
	qualificationId int,
	offerId int,
	qualLevel int,
	constraint fk_MATCH_ID foreign key (qualificationId) references QUALIFICATIONS (qualificationId),
	constraint fk_OFFER_QUAL foreign key (offerId) references OFFER (offerId),
	constraint PK_OFFERQUAL primary key CLUSTERED(qualificationId, offerId)
);
GO

CREATE TABLE ADMINISTRATION(
	refsCanViewOthers bit not null,
	refsCanSeeOtherGames bit not null,
	durationOfAlgorithm int not null
);
GO

CREATE TABLE [Events](
	Id int NOT NULL,
	owner nvarchar(128) NULL,
	title nvarchar(50) NULL,
	start date NULL,
	[end] date NULL,
	url nvarchar(50) NULL,
	backgroundColor [nvarchar](50) NULL,
	allDay bit NULL,
	CONSTRAINT PK_dbo_events primary key CLUSTERED(Id),
	CONSTRAINT fk_EVENTS_USERS foreign key (owner) references AspNetUsers (Id)
);
GO



--create table scores(-
--	score int not null,
--	team int not null,
--	matchID int not null,
--	tID int not null,
--	constraint PK_dbo_scores primary key CLUSTERED(team, matchID, tID),
--	constraint fk_SCORES_MATCH foreign key (matchID) references MATCH (mID),
--	constraint fk_SCORES_PLAYER foreign key (team) references TEAM (teamID),
--	constraint FK_SCORES_TOURNAMENT foreign key (tID) references TOURNAMENT (tID)
--);
--GO