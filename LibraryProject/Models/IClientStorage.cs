namespace LibraryProject.Models
{
    public interface IClientStorage
    {
        IClient AddClient(string name, string surname, string phoneNumber);
        IClient FindById(int id);
    }
}