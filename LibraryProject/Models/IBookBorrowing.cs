namespace LibraryProject.Models
{
    public interface IBookBorrowing
    {
        int Id { get; }
        int ClientId { get; }
        int BookId { get; }
        string ClientName { get; }
        string ClientSurname { get; }
        string ClientPhone { get; }
        string BookTitle { get; }
        string BookAuthor { get; }
    }


}