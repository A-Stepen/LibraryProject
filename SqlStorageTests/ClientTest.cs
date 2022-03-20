using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace SqlStorageTests
{
    [TestClass]
    public class ClientTest
    {
        [TestMethod]
        public void TestAddClient()
        {
            string connString = "Host=192.168.1.39;Port=5434;Username=USER;Password=USER;";
            LibraryProject.Models.ClientSqlStorage storage = new LibraryProject.Models.ClientSqlStorage(connString);

            LibraryProject.Models.IClient client = storage.AddClient("Qwe", "Asd", "+7999001122");
            Assert.IsNotNull(client);
            Assert.IsTrue(client.Id > 0);
        }
    }
}
