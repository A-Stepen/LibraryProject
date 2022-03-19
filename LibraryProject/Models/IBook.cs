using System;
using System.Collections.Generic;


namespace LibraryProject.Models
{
    public interface IBookProperty
    {
        int Id { get; }
        string Title { get; }
        string Author { get; }
        string Description { get; }
    }

    public interface IBookStorage
    {
        IBookProperty RegisterBook(string title, string author, string description);
        IEnumerable<IBook> AddBook(int propertyId, int quantity);
        int UpdateDescription(int propertyId, string description);
    }

    public enum BookState
    {
        Available,
        Given,
        Lost
    }

    public interface IBook
    {
        int Id { get; }
        int PropertyId { get; }
        BookState State { get; }
    }


}