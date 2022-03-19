using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using LibraryProject.Controllers;


namespace LibraryProject
{
    class ClientStorageMock : Models.IClientStorage
    {
        class Client : Models.IClient
        {
            public int Id { get; private set; }
            public string Name { get; private set; }
            public string Surname { get; private set; }
            public string PhoneNumber { get; private set; }

            public Client(int id, string name, string surname, string phoneNumber)
            {
                Id = id;
                Name = name;
                Surname = surname;
                PhoneNumber = phoneNumber;
            }
        }

        Dictionary<int, Models.IClient> clients = new Dictionary<int, Models.IClient>();

        public List<Models.IClient> List()
        {
            return clients.Values.ToList();
        }

        public Models.IClient AddClient(string name, string surname, string phoneNumber)
        {
            int newId = 0;
            if (clients.Count > 0)
            {
                newId = clients.Keys.Max() + 1;
            }

            Client client = new Client(newId, name, surname, phoneNumber);
            clients.Add(newId, client);
            return client;
        }

        public Models.IClient FindById(int id)
        {
            if (clients.TryGetValue(id, out Models.IClient client))
                return client;
            return null;
        }


    }


    public class MvcApplication : HttpApplication
    {
        ClientStorageMock clientStorageMock = new ClientStorageMock();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Dictionary<Type, Func<IController>> injection = new Dictionary<Type, Func<IController>>()
            {
                { typeof(ClientController), () => new ClientController(clientStorageMock) }
            };
            ControllerBuilder.Current.SetControllerFactory(new DefaultControllerFactory(new InjectableControllerActivator(injection)));
        }
    }
}
