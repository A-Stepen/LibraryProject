using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;

using LibraryProject.Controllers;
using LibraryProject.Models;


namespace LibraryProjectTests
{
    class ClientStorageMock : IClientStorage
    {
        class Client : IClient
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

        Dictionary<int, IClient> clients = new Dictionary<int, IClient>();

        public IClient AddClient(string name, string surname, string phoneNumber)
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

        public IClient FindById(int id)
        {
            if (clients.TryGetValue(id, out IClient client))
                return client;
            return null;
        }


    }

    [TestClass]
    public class ClientControllerTest
    {
        ClientController clientController;

        [TestInitialize]
        public void PrepareTest()
        {
            clientController = new ClientController(new ClientStorageMock());
        }

        [TestMethod]
        public void EmptyListClients()
        {
            JsonResult json = clientController.List();
            List<IClient> data = json.Data as List<IClient>;

            Assert.IsNotNull(data);
            Assert.AreEqual(0, data.Count);
        }

        [TestMethod]
        public void AddClient()
        {
            string clientName = "TestName";
            string clientSurname = "TestSurname";
            string clientPhone = "+79999999999";

            JsonResult result = clientController.Add(clientName, clientSurname, clientPhone);
            IClient data = result.Data as IClient;

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Id >= 0);
        }

        [TestMethod]
        public void GetJustAddedClient()
        {
            string clientName = "TestName";
            string clientSurname = "TestSurname";
            string clientPhone = "+79999999999";

            JsonResult result = clientController.Add(clientName, clientSurname, clientPhone);
            IClient client = result.Data as IClient;

            Assert.IsNotNull(client);
            int id = client.Id;
            Assert.IsTrue(id >= 0);

            result = clientController.GetById(id);
            IClient clientData = result.Data as IClient;

            Assert.IsNotNull(clientData);
            Assert.AreEqual(clientName, clientData.Name);
            Assert.AreEqual(clientSurname, clientData.Surname);
            Assert.AreEqual(clientPhone, clientData.PhoneNumber);
        }
    }
}
