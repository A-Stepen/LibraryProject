using System;
using System.Text;
using System.Collections.Generic;

using Npgsql;


namespace LibraryProject.Models
{
    public class BookSqlStorage : IBookStorage
    {
        class BookPropertie : IBookProperty
        {
            public int Id { get; protected set; }

            public string Title { get; protected set; }

            public string Author { get; protected set; }

            public string Description { get; protected set; }

            public BookPropertie(int id, string title, string author, string description)
            {
                Id = id;
                Title = title;
                Author = author;
                Description = description;
            }
        }

        class Book : IBook
        {
            public int Id { get; protected set; }

            public int PropertyId { get; protected set; }

            public BookState State { get; protected set; }

            public Book(int id, int propertyId, BookState state)
            {
                Id = id;
                PropertyId = propertyId;
                State = state;
            }
        }

        const string registerBookCmd = "INSERT INTO BookProperties(title, author, description) VALUES($1, $2, $3) RETURNING ID;";
        const string addBookItemTemplate = "INSERT INTO Books(status, propertie_id) VALUES";
        const string addBookItemPostfix = "RETURNING id;";

        const string filterBookCmd = "SELECT * FROM BookProperties WHERE (title LIKE $1) AND (author LIKE $2);";

        string connString;

        public BookSqlStorage(string connString)
        {
            this.connString = connString;
        }

        public IEnumerable<IBook> AddBook(int propertyId, int quantity)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(addBookItemTemplate);

            for (int i = 0; i < quantity; ++i)
            {
                if (i > 0)
                    sb.Append(',');
                sb.Append("($1, $");
                sb.Append((2 + i).ToString());
                sb.Append(')');
            }
            sb.Append(addBookItemPostfix);

            var conn = new NpgsqlConnection(connString);
            conn.Open();

            string addBookCmd = sb.ToString();
            using (var cmd = new NpgsqlCommand(addBookCmd, conn))
            {
                cmd.Parameters.AddWithValue((int)BookState.Available);
                for (int i = 0; i < quantity; ++i)
                {
                    cmd.Parameters.AddWithValue(propertyId);
                }

                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    List<IBook> result = new List<IBook>();

                    while (reader.IsOnRow)
                    {
                        int id = (int)reader["id"];
                        result.Add(new Book(id, propertyId, (int)BookState.Available));
                        reader.Read();
                    }
                    return result;
                }
            }
        }

        public List<IBookProperty> FilterBook(string titleFilter, string authorFilter)
        {
            titleFilter = titleFilter != null && titleFilter.Length > 0 ? string.Format("%{0}%", titleFilter.Escape()) : "%";
            authorFilter = authorFilter != null && authorFilter.Length > 0 ? string.Format("%{0}%", authorFilter.Escape()) : "%";

            if (titleFilter.Length == 0 && authorFilter.Length == 0)
                return new List<IBookProperty>();


            var conn = new NpgsqlConnection(connString);
            conn.Open();

            using (var cmd = new NpgsqlCommand(filterBookCmd, conn))
            {
                cmd.Parameters.AddWithValue(titleFilter);
                cmd.Parameters.AddWithValue(authorFilter);

                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    List<IBookProperty> result = new List<IBookProperty>();

                    while (reader.IsOnRow)
                    {
                        int id = (int)reader["id"];
                        string title = (string)reader["title"];
                        string author = (string)reader["author"];
                        string description = (string)reader["description"];
                        result.Add(new BookPropertie(id, title, author, description));
                        reader.Read();
                    }
                    return result;
                }
            }
        }

        public IBookProperty GetPropertyById(int id)
        {
            throw new System.NotImplementedException();
        }

        public IBookProperty RegisterBook(string title, string author, string description)
        {
            title = title.Escape();
            author = author.Escape();
            description = description.Escape();

            var conn = new NpgsqlConnection(connString);
            conn.Open();

            using (var cmd = new NpgsqlCommand(registerBookCmd, conn))
            {
                cmd.Parameters.AddWithValue(title);
                cmd.Parameters.AddWithValue(author);
                cmd.Parameters.AddWithValue(description);
                object result = cmd.ExecuteScalar();
                return new BookPropertie((int)result, title, author, description);
            }
        }

        public int UpdateDescription(int propertyId, string description)
        {
            throw new System.NotImplementedException();
        }
    }
}