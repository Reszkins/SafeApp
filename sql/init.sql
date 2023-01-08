CREATE TABLE Accounts
(
  Id INT GENERATED ALWAYS AS IDENTITY,
  UserName VARCHAR(100),
  PasswordHash BYTEA,
  PasswordSalt BYTEA
);

CREATE TABLE Notes
(
  Id INT GENERATED ALWAYS AS IDENTITY,
  Title VARCHAR(200),
  Content VARCHAR(2000),
  Public INT,
  OwnerId INT,
  Encrypted INT,
  PasswordHash BYTEA
);

create table LoginAttempts
(
  UserName VARCHAR(100),
  TimeStamp TIMESTAMP
);