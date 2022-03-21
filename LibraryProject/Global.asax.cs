using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using LibraryProject.Controllers;
using LibraryProject.Models;

namespace LibraryProject
{


    public class MvcApplication : HttpApplication
    {
        ClientSqlStorage clientStorage;
        BookSqlStorage bookStorage;

        protected void Application_Start()
        {
            string path = Environment.GetEnvironmentVariable("ConnDataPath");
            string connString = path != null ? System.IO.File.ReadAllText(path) : "Host=192.168.1.39;Port=5434;Username=USER;Password=USER;";

            clientStorage = new ClientSqlStorage(connString);
            bookStorage = new BookSqlStorage(connString);

            List<string[]> initialBooks = new List<string[]>()
            {
                new string[] {"Test vol.1", "T. Test", "1st test book."},
                new string[] {"Test vol.2", "T. Test", "2nd test book."},
                new string[] {"Science fiction", "I. Azimov", "Not testing science fiction book."},
                new string[] {"Test vol.3", "T. Test", "3rd test book."},
                new string[] {"Science fiction storybook", "I. Azimov", "Another science fiction book."},
                new string[] {"Test vol.4", "T. Test", "4th test book."},
                new string[] {"Programming for dummy", "D. Dumm", "Very useful book."},
            };

            /*foreach (string[] data in initialBooks)
            {
                var res = bookStorage.RegisterBook(data[0], data[1], data[2]);
                bookStorage.AddBook(res.Id, 3);
            }*/
            

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Dictionary<Type, Func<IController>> injection = new Dictionary<Type, Func<IController>>()
            {
                { typeof(ClientController), () => new ClientController(clientStorage) },
                { typeof(BookController), () => new BookController(bookStorage) },
                { typeof(OperationController), () => new OperationController(bookStorage) },
            };
            ControllerBuilder.Current.SetControllerFactory(new DefaultControllerFactory(new InjectableControllerActivator(injection)));
        }
    }
}
