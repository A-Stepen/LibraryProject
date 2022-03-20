using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using LibraryProject.Controllers;
using LibraryProject.Models;


namespace LibraryProjectTests
{
    class BookStorageMock : IBookStorage
    {
        public class BookPropertieMock : IBookProperty
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

        public class BookMock : IBook
        {
            public int Id { get; private set; }
            public int PropertyId { get; private set; }
            public BookState State { get; private set; }

            public BookMock(int id, int propertyId, BookState state)
            {
                Id = id;
                PropertyId = propertyId;
                State = state;
            }
        }

        public readonly Dictionary<int, IBookProperty> bookProperties = new Dictionary<int, IBookProperty>();
        public readonly Dictionary<int, IBook> books = new Dictionary<int, IBook>();

        public IEnumerable<IBook> AddBook(int propertyId, int quantity)
        {
            Assert.IsTrue(bookProperties.ContainsKey(propertyId));

            int id = 0;
            if (books.Count > 0)
                id = books.Keys.Max() + 1;

            List<IBook> newBooks = new List<IBook>();
            for (int i = 0; i < quantity; ++i, ++id)
            {
                BookMock book = new BookMock(id, propertyId, BookState.Available);
                newBooks.Add(book);
                books.Add(id, book);
            }

            return newBooks;
        }

        public IBookProperty RegisterBook(string title, string author, string description)
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
            IBookProperty bookProperty = bookProperties[propertyId];
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


    [TestClass]
    public class BookControllerTest
    {
        BookController bookController;
        BookStorageMock bookStorageMock;

        [TestInitialize]
        public void PrepareTest()
        {
            bookStorageMock = new BookStorageMock();
            bookController = new BookController(bookStorageMock);
        }


        [TestMethod]
        public void GetEmptyList()
        {
            JsonResult result = bookController.GetBookList();
            List<IBookProperty> bookProperties = result.Data as List<IBookProperty>;

            Assert.IsNotNull(bookProperties);
            Assert.AreEqual(0, bookProperties.Count);
        }

        [TestMethod]
        public void RegisterNewBook()
        {
            string title = "Test Title";
            string author = "T. Test";
            string description = "Test text about testing";
            int quantity = 3;

            JsonResult result = bookController.RegisterBook(title, author, description, quantity);

            var addedBooks = result.Data as IEnumerable<IBook>;
            Assert.IsNotNull(addedBooks);
            Assert.AreEqual(quantity, addedBooks.Count());
            Assert.IsTrue(addedBooks.All((b) => (b.Id >= 0) && (b.PropertyId >= 0)));

            int newPropertyId = addedBooks.First().PropertyId;

            IBookProperty bookProperty = bookStorageMock.bookProperties[newPropertyId];
            Assert.AreEqual(title, bookProperty.Title);
            Assert.AreEqual(author, bookProperty.Author);
            Assert.AreEqual(description, bookProperty.Description);

            Assert.AreEqual(quantity, bookStorageMock.books.Count());
        }


        [TestMethod]
        public void UpdateDescription()
        {
            List<string[]> fillingData = new List<string[]>()
            {
                new string[] { "Test Title", "T. Test", "Test text about testing" },
                new string[] { "Book #2", "A. Author", "Second book in test." },
                new string[] { "Book #3", "A. Author", "Fifth book in test." },
                new string[] { "Book #4", "A. Author", "Fourth book in test." },
            };

            for (int i = 0; i < fillingData.Count; ++i)
            {
                string[] row = fillingData[i];
                bookStorageMock.bookProperties.Add(i, new BookStorageMock.BookPropertieMock(i, row[0], row[1], row[2]));
            }

            int updateId = 2;
            string newDescription = "Fixed description.";

            JsonResult result = bookController.UpdateDescription(updateId, newDescription);
            Assert.IsInstanceOfType(result.Data, typeof(int));
            if (result.Data is int length)
            {
                Assert.AreEqual(newDescription.Length, length);
            }

            for (int i = 0; i < fillingData.Count(); ++i)
            {
                string[] targetRow = fillingData[i];
                IBookProperty actualRow = bookStorageMock.bookProperties[i];

                Assert.AreEqual(targetRow[0], actualRow.Title);
                Assert.AreEqual(targetRow[1], actualRow.Author);

                string targetDescription = i == updateId ? newDescription : targetRow[2];
                Assert.AreEqual(targetDescription, actualRow.Description);
            }
        }
    }
}
