using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using LibraryProject.Controllers;
using LibraryProject.Models;

namespace LibraryProject
{
    class BookStorageMock : Models.IBookStorage
    {
        public class BookPropertieMock : Models.IBookProperty
        {
            public int Id { get; private set; }
            public string Title { get; private set; }
            public string Author { get; private set; }
            public string Description { get; private set; }

            public BookPropertieMock(int id, string title, string author, string description)
            {
                Id = id;
                Title = title;
                Author = author;
                Description = description;
            }
        }

        public class BookMock : Models.IBook
        {
            public int Id { get; private set; }
            public int PropertyId { get; private set; }
            public Models.BookState State { get; private set; }

            public BookMock(int id, int propertyId, Models.BookState state)
            {
                Id = id;
                PropertyId = propertyId;
                State = state;
            }
        }

        public readonly Dictionary<int, Models.IBookProperty> bookProperties = new Dictionary<int, Models.IBookProperty>();
        public readonly Dictionary<int, Models.IBook> books = new Dictionary<int, Models.IBook>();

        public IEnumerable<Models.IBook> AddBook(int propertyId, int quantity)
        {
            int id = 0;
            if (books.Count > 0)
                id = books.Keys.Max() + 1;

            List<Models.IBook> newBooks = new List<Models.IBook>();
            for (int i = 0; i < quantity; ++i, ++id)
            {
                BookMock book = new BookMock(id, propertyId, Models.BookState.Available);
                newBooks.Add(book);
                books.Add(id, book);
            }

            return newBooks;
        }

        public Models.IBookProperty RegisterBook(string title, string author, string description)
        {
            int id = 0;
            if (bookProperties.Count > 0)
                id = bookProperties.Keys.Max() + 1;

            BookPropertieMock bookProperie = new BookPropertieMock(id, title, author, description);
            bookProperties.Add(id, bookProperie);

            return bookProperie;
        }

        public int UpdateDescription(int propertyId, string description)
        {
            Models.IBookProperty bookProperty = bookProperties[propertyId];
            bookProperty = new BookPropertieMock(propertyId, bookProperty.Title, bookProperty.Author, description);
            bookProperties[propertyId] = bookProperty;
            return description.Length;
        }

        public List<IBookProperty> FilterBook(string titleFilter, string authorFilter)
        {
            if (titleFilter == null && authorFilter == null)
                return new List<IBookProperty>();

            titleFilter = titleFilter == null ? string.Empty : titleFilter;
            authorFilter = authorFilter == null ? string.Empty : authorFilter;
            var result = from book in bookProperties.Values where (book.Author.Contains(authorFilter) && book.Title.Contains(titleFilter)) select book;
            return result.ToList();
        }

        public IBookProperty GetById(int id)
        {
            return bookProperties[id];
        }
    }

    class ClientStorageMock : Models.IClientStorage
    {
        class Client : Models.IClient
        {
            public int Id { get; private set; }
            public string Name { get; private set; }
            public string Surname { get; private set; }
            public string PhoneNumber { get; private set; }

            public Client(int id, string name, string surname, string phoneNumber)
            {
                Id = id;
                Name = name;
                Surname = surname;
                PhoneNumber = phoneNumber;
            }
        }

        Dictionary<int, Models.IClient> clients = new Dictionary<int, Models.IClient>();

        public List<Models.IClient> List()
        {
            return clients.Values.ToList();
        }

        public Models.IClient AddClient(string name, string surname, string phoneNumber)
        {
            int newId = 0;
            if (clients.Count > 0)
            {
                newId = clients.Keys.Max() + 1;
            }

            Client client = new Client(newId, name, surname, phoneNumber);
            clients.Add(newId, client);
            return client;
        }

        public Models.IClient FindById(int id)
        {
            if (clients.TryGetValue(id, out Models.IClient client))
                return client;
            return null;
        }


    }


    public class MvcApplication : HttpApplication
    {
        ClientStorageMock clientStorageMock = new ClientStorageMock();
        BookStorageMock bookStorageMock = new BookStorageMock();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Dictionary<Type, Func<IController>> injection = new Dictionary<Type, Func<IController>>()
            {
                { typeof(ClientController), () => new ClientController(clientStorageMock) },
                { typeof(BookController), () => new BookController(bookStorageMock) }
            };
            ControllerBuilder.Current.SetControllerFactory(new DefaultControllerFactory(new InjectableControllerActivator(injection)));
        }
    }
}
