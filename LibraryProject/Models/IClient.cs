namespace LibraryProject.Models
{
    public interface IClient
    {
        int Id { get; }
        string Name { get; }
        string Surname { get; }
        string PhoneNumber { get; }
    }

    public interface IClientStorage
    {
        IClient AddClient(string name, string surname, string phoneNumber);
        IClient FindById(int id);
    }
}