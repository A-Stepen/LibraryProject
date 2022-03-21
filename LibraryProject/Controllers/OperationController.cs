using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryProject.Controllers
{
    public class OperationController : Controller
    {
        Models.IBookStorage bookStorage;

        public OperationController(Models.IBookStorage bookStorage)
        {
            this.bookStorage = bookStorage;
        }

        // GET: Operation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Borrowing()
        {
            return View();
        }

        public ActionResult Returning()
        {
            return View();
        }

        public ActionResult BorrowBook(int clientId, int bookId)
        {
            bookStorage.BorrowBook(clientId, bookId);
            return RedirectToAction("Index", "Operation");
        }

        public ActionResult ReturnBook(int id)
        {
            bookStorage.ReturnBook(id);
            return RedirectToAction("Index", "Operation");
        }

        public JsonResult FilterBorrow(string nameFilter, string surnameFilter, string phoneFilter, string titleFilter, string authorFilter)
        {
            return Json(bookStorage.FilterBorrows(nameFilter, surnameFilter, phoneFilter, titleFilter, authorFilter));
        }

        public JsonResult GetBorrowById(int id)
        {
            return Json(bookStorage.GetBorrowById(id));
        }
    }
}