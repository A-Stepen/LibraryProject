CREATE TABLE Clients(ID SERIAL PRIMARY KEY, name varchar(64), surname varchar(64), phone varchar(12));
CREATE TABLE BookProperties(ID SERIAL PRIMARY KEY, title varchar(255), author varchar(255), description text);
CREATE TABLE Books(ID SERIAL PRIMARY KEY, propertie_id integer REFERENCES BookProperties(ID), status integer CHECK (status >=0 AND status <= 2));
CREATE TABLE Operations(ID SERIAL PRIMARY KEY, client_id integer REFERENCES Clients(ID), book_id integer REFERENCES Books(ID));