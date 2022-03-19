using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using LibraryProject.Models;


namespace LibraryProject.Controllers
{
    public class BookController : Controller
    {
        IBookStorage bookStorage;

        public BookController(IBookStorage bookStorage) : base()
        {
            this.bookStorage = bookStorage;
        }

        // GET: Book
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetBookList()
        {
            return Json(new List<IBookProperty>());
        }

        [HttpPut]
        public JsonResult RegisterBook(string title, string author, string description, int quantity)
        {
            IBookProperty bookProperty = bookStorage.RegisterBook(title, author, description);
            IEnumerable<IBook> books = bookStorage.AddBook(bookProperty.Id, quantity);
            
            return Json(books);
        }

        [HttpPost]
        public JsonResult UpdateDescription(int id, string newDescription)
        {
            int newLength = bookStorage.UpdateDescription(id, newDescription);
            return Json(newLength);
        }
    }
}