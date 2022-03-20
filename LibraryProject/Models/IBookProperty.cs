namespace LibraryProject.Models
{
    public interface IBookProperty
    {
        int Id { get; }
        string Title { get; }
        string Author { get; }
        string Description { get; }
    }


}