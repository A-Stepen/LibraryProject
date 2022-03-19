using System;
using System.Collections.Generic;


namespace LibraryProject.Models
{
    public interface IClientStorage
    {
        List<IClient> List();
        IClient AddClient(string name, string surname, string phoneNumber);
        IClient FindById(int id);
    }
}