using System;
using System.Collections.Generic;

using Npgsql;


namespace LibraryProject.Models
{

    public class ClientSqlStorage : IClientStorage
    {
        class ClientData : IClient
        {
            public int Id { get; private set; }

            public string Name { get; private set; }

            public string Surname { get; private set; }

            public string PhoneNumber { get; private set; }

            public ClientData(int id, string name, string surname, string phoneNumber)
            {
                Id = id;
                Name = name;
                Surname = surname;
                PhoneNumber = phoneNumber;
            }
        }

        const string addClientCmd = "INSERT INTO Clients(name, surname, phone) VALUES ($1, $2, $3) RETURNING id";
        const string getClientByIdCmd = "SELECT * FROM Clients WHERE id=$1";
        const string getAllClients = "SELECT * FROM Clients";
        const string filterClientCmd = "SELECT * FROM Clients WHERE (name LIKE $1) AND (surname LIKE $2) AND (phone LIKE $3);";

        string connString;

        public ClientSqlStorage(string connString)
        {
            this.connString = connString;
        }

        public IClient AddClient(string name, string surname, string phoneNumber)
        {
            name = name.Escape();
            surname = surname.Escape();
            phoneNumber = phoneNumber.Escape();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(addClientCmd, conn))
                {
                    cmd.Parameters.AddWithValue(name);
                    cmd.Parameters.AddWithValue(surname);
                    cmd.Parameters.AddWithValue(phoneNumber);
                    object result = cmd.ExecuteScalar();
                    return new ClientData((int)result, name, surname, phoneNumber);
                }
            }
        }

        public IClient FindById(int id)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(getClientByIdCmd, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Rows != 1)
                            throw new ArgumentException("Incorrect id");
                        string name = (string)reader["name"];
                        string surname = (string)reader["surname"];
                        string phone = (string)reader["phone"];
                        return new ClientData(id, name, surname, phone);
                    }
                }
            }
        }

        public List<IClient> List()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(getAllClients, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        List<IClient> result = new List<IClient>();

                        while (reader.IsOnRow)
                        {
                            int id = (int)reader["id"];
                            string name = (string)reader["name"];
                            string surname = (string)reader["surname"];
                            string phone = (string)reader["phone"];
                            result.Add(new ClientData(id, name, surname, phone));
                            reader.Read();
                        }
                        return result;
                    }
                }
            }
        }

        public List<IClient> Filter(string nameFilter, string surnameFilter, string phoneFilter)
        {
            nameFilter = nameFilter.PreprocessFilter();
            surnameFilter = surnameFilter.PreprocessFilter();
            phoneFilter = phoneFilter.PreprocessFilter();

            if (nameFilter == "%" && surnameFilter == "%" && phoneFilter == "%")
                return new List<IClient>();


            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(filterClientCmd, conn))
                {
                    cmd.Parameters.AddWithValue(nameFilter);
                    cmd.Parameters.AddWithValue(surnameFilter);
                    cmd.Parameters.AddWithValue(phoneFilter);

                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        List<IClient> result = new List<IClient>();

                        while (reader.IsOnRow)
                        {
                            int id = (int)reader["id"];
                            string title = (string)reader["name"];
                            string surname = (string)reader["surname"];
                            string phone = (string)reader["phone"];
                            result.Add(new ClientData(id, title, surname, phone));
                            reader.Read();
                        }
                        return result;
                    }
                }
            }
        }
    }
}
