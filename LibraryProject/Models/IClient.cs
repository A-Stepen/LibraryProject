namespace LibraryProject.Models
{

    public interface IClient
    {
        int Id { get; }
        string Name { get; }
        string Surname { get; }
        string PhoneNumber { get; }
    }
}