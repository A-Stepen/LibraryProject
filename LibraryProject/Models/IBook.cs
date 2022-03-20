using System;


namespace LibraryProject.Models
{

    public interface IBook
    {
        int Id { get; }
        int PropertyId { get; }
        BookState State { get; }
    }


}