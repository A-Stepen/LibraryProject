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
    }
}