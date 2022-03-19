using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using LibraryProject.Models;


namespace LibraryProject.Controllers
{

    public class ClientController : Controller
    {
        IClientStorage clientStorage;

        public ClientController(IClientStorage clientStorage) : base()
        {
            this.clientStorage = clientStorage;
        }

        // GET: Client
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult List()
        {
            return Json(clientStorage.List());
        }

        [HttpPost]
        public JsonResult Add(string name, string surname, string phoneNumber)
        {
            IClient client = clientStorage.AddClient(name, surname, phoneNumber);
            return Json(client);
        }

        public JsonResult GetById(int id)
        {
            return Json(clientStorage.FindById(id));
        }
    }
}