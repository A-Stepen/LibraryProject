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

        class BookBorrowing : IBookBorrowing
        {
            public int Id { get; private set; }

            public int ClientId { get; private set; }

            public int BookId { get; private set; }

            public string ClientName { get; private set; }

            public string ClientSurname { get; private set; }

            public string ClientPhone { get; private set; }

            public string BookTitle { get; private set; }

            public string BookAuthor { get; private set; }

            public BookBorrowing(int id, int clientId, int bookId,
                string clientName, string clientSurname, string clientPhone,
                string bookTitle, string bookAuthor)
            {
                Id = id;
                ClientId = clientId;
                BookId = bookId;
                ClientName = clientName;
                ClientSurname = clientSurname;
                ClientPhone = clientPhone;
                BookTitle = bookTitle;
                BookAuthor = bookAuthor;
            }
        }

        const string registerBookCmd = "INSERT INTO BookProperties(title, author, description) VALUES($1, $2, $3) RETURNING ID;";
        const string addBookItemTemplate = "INSERT INTO Books(status, propertie_id) VALUES";
        const string addBookItemPostfix = "RETURNING id;";

        const string filterBookCmd = "SELECT * FROM BookProperties WHERE (title LIKE $1) AND (author LIKE $2);";
        const string getBookByIdCmd = "SELECT * FROM BookProperties WHERE id=$1";
        const string updateDescriptionCmd = "UPDATE BookProperties SET description=$2 WHERE id=$1 RETURNING LENGTH(Description);";
        const string countAvailableBooksCmd = "SELECT COUNT(*) FROM Books WHERE (propertie_id=$1) AND (status=0);";
        const string selectAvailableBookIdCmd = "SELECT id FROM Books WHERE (propertie_id=$1) AND (status=0) LIMIT 1;";
        const string getBookStatusCmd = "SELECT status FROM Books WHERE id=$1;";

        const string updateBookStatus = "UPDATE Books SET status=$2 WHERE id=$1;";
        const string addOperation = "INSERT INTO Operations(client_id, book_id) VALUES ($1, $2) RETURNING ID;";
        const string selectBookIdFromOperation = "SELECT book_id FROM Operations WHERE id=$1;";
        const string deleteOperationCmd = "DELETE FROM Operations WHERE id=$1";

        const string selectBorrowCmd = "SELECT Operations.*, BookProperties.title, BookProperties.author, Clients.name, Clients.surname, Clients.phone " +
            "FROM Operations JOIN BookProperties ON (Operations.book_id = BookProperties.id) " +
            "JOIN Clients ON (Operations.client_id = Clients.id) " +
            "WHERE Operations.id=$1";

        const string filterBorrowCmd = "SELECT Operations.*, BookProperties.title, BookProperties.author, Clients.name, Clients.surname, Clients.phone " + 
            "FROM Operations JOIN BookProperties ON (Operations.book_id = BookProperties.id) " +
            "JOIN Clients ON (Operations.client_id = Clients.id) " +
            "WHERE (surname LIKE $1) AND (name LIKE $2) AND (phone LIKE $3) AND " +
            "(title LIKE $4) AND (author LIKE $5);";

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

            using (var conn = new NpgsqlConnection(connString))
            {
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
        }

        public List<IBookProperty> FilterBook(string titleFilter, string authorFilter)
        {
            titleFilter = titleFilter.PreprocessFilter();
            authorFilter = authorFilter.PreprocessFilter();

            if (titleFilter == "%" && authorFilter == "%")
                return new List<IBookProperty>();


            using (var conn = new NpgsqlConnection(connString))
            {
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
        }

        public IBookProperty GetPropertyById(int id)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(getBookByIdCmd, conn))
                {
                    cmd.Parameters.AddWithValue(id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new ArgumentException("Incorrect id");
                        string name = (string)reader["title"];
                        string surname = (string)reader["author"];
                        string phone = (string)reader["description"];
                        return new BookPropertie(id, name, surname, phone);
                    }
                }
            }
        }

        public IBookProperty RegisterBook(string title, string author, string description)
        {
            title = title.Escape();
            author = author.Escape();
            description = description.Escape();

            using (var conn = new NpgsqlConnection(connString))
            {
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
        }

        public int UpdateDescription(int propertyId, string description)
        {
            description = description.Escape();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(updateDescriptionCmd, conn))
                {
                    cmd.Parameters.AddWithValue(propertyId);
                    cmd.Parameters.AddWithValue(description);
                    object result = cmd.ExecuteScalar();
                    return (int)result;
                }
            }
        }

        public Tuple<IBookProperty, int, int> GetAvailableCountPropertyId(int propertyId)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                IBookProperty property;

                using (var cmd = new NpgsqlCommand(getBookByIdCmd, conn))
                {
                    cmd.Parameters.AddWithValue(propertyId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new ArgumentException("Incorrect id");
                        string name = (string)reader["title"];
                        string surname = (string)reader["author"];
                        string phone = (string)reader["description"];
                        property = new BookPropertie(propertyId, name, surname, phone);
                    }
                }

                int availableBooks = 0;
                using (var cmd = new NpgsqlCommand(countAvailableBooksCmd, conn))
                {
                    cmd.Parameters.AddWithValue(propertyId);
                    availableBooks = (int)(long)cmd.ExecuteScalar();
                }

                int availableId = 0;
                if (availableBooks > 0)
                {
                    using (var cmd = new NpgsqlCommand(selectAvailableBookIdCmd, conn))
                    {
                        cmd.Parameters.AddWithValue(propertyId);
                        availableId = (int)cmd.ExecuteScalar();
                    }
                }
                return new Tuple<IBookProperty, int, int>(property, availableId, availableBooks);
            }
        }
    
        public int BorrowBook(int clientId, int bookId)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(getBookStatusCmd, conn))
                {
                    cmd.Parameters.AddWithValue(bookId);
                    if ((BookState)cmd.ExecuteScalar() != BookState.Available)
                    {
                        throw new ArgumentException("Invalid book status.");
                    }
                }
                    

                using (var cmd = new NpgsqlCommand(updateBookStatus, conn))
                {
                    cmd.Parameters.AddWithValue(bookId);
                    cmd.Parameters.AddWithValue((int)BookState.Given);
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new NpgsqlCommand(addOperation, conn))
                {
                    cmd.Parameters.AddWithValue(clientId);
                    cmd.Parameters.AddWithValue(bookId);
                    object result = cmd.ExecuteScalar();
                    return (int)result;
                }
            }
        }

        public void ReturnBook(int id)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                int bookId = 0;
                using (var cmd = new NpgsqlCommand(selectBookIdFromOperation, conn))
                {
                    cmd.Parameters.AddWithValue(id);
                    bookId = (int)cmd.ExecuteScalar();
                }

                using (var cmd = new NpgsqlCommand(deleteOperationCmd, conn))
                {
                    cmd.Parameters.AddWithValue(id);
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new NpgsqlCommand(updateBookStatus, conn))
                {
                    cmd.Parameters.AddWithValue(bookId);
                    cmd.Parameters.AddWithValue((int)BookState.Available);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public IBookBorrowing GetBorrowById(int id)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(selectBorrowCmd, conn))
                {
                    cmd.Parameters.AddWithValue(id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new ArgumentException("Incorrect id");
                        }

                        int clientId = (int)reader["client_id"];
                        int bookId = (int)reader["book_id"];
                        string title = (string)reader["title"];
                        string author = (string)reader["author"];
                        string clientName = (string)reader["name"];
                        string clientSurname = (string)reader["surname"];
                        string clientPhone = (string)reader["phone"];
                        return new BookBorrowing(id, clientId, bookId, clientName, clientSurname, clientPhone, title, author);
                    }
                }
            }
        }

        public List<IBookBorrowing> FilterBorrows(string nameFilter, string surnameFilter, string phoneFilter, string titleFilter, string authorFilter)
        {
            nameFilter = nameFilter.PreprocessFilter();
            surnameFilter = surnameFilter.PreprocessFilter();
            phoneFilter = phoneFilter.PreprocessFilter();
            titleFilter = titleFilter.PreprocessFilter();
            authorFilter = authorFilter.PreprocessFilter();


            if (titleFilter == "%" && authorFilter == "%" &&
                nameFilter == "%" && surnameFilter == "%" && phoneFilter == "%")
                return new List<IBookBorrowing>();


            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(filterBorrowCmd, conn))
                {
                    cmd.Parameters.AddWithValue(surnameFilter);
                    cmd.Parameters.AddWithValue(nameFilter);
                    cmd.Parameters.AddWithValue(phoneFilter);
                    cmd.Parameters.AddWithValue(titleFilter);
                    cmd.Parameters.AddWithValue(authorFilter);

                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        List<IBookBorrowing> result = new List<IBookBorrowing>();

                        while (reader.IsOnRow)
                        {
                            int id = (int)reader["id"];
                            int clientId = (int)reader["client_id"];
                            int bookId = (int)reader["book_id"];
                            string title = (string)reader["title"];
                            string author = (string)reader["author"];
                            string clientName = (string)reader["name"];
                            string clientSurname = (string)reader["surname"];
                            string clientPhone= (string)reader["phone"];
                            result.Add(new BookBorrowing(id, clientId, bookId, clientName, clientSurname, clientPhone, title, author));
                            reader.Read();
                        }
                        return result;
                    }
                }
            }
        }
    }
}