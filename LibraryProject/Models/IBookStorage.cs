using System;
using System.Collections.Generic;


namespace LibraryProject.Models
{
    public interface IBookStorage
    {
        IBookProperty RegisterBook(string title, string author, string description);
        IEnumerable<IBook> AddBook(int propertyId, int quantity);
        int UpdateDescription(int propertyId, string description);
        List<IBookProperty> FilterBook(string titleFilter, string authorFilter);

        IBookProperty GetPropertyById(int id);
        Tuple<IBookProperty, int, int> GetAvailableCountPropertyId(int propertyId);
        int BorrowBook(int clientId, int bookId);
        void ReturnBook(int id);
        List<IBookBorrowing> FilterBorrows(string nameFilter, string surnameFilter, string phoneFilter, string titleFilter, string authorFilter);
        IBookBorrowing GetBorrowById(int id);
    }
}