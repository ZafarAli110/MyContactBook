using MyContactBook.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyContactBook.Controllers
{
    public class ExportController : Controller
    {
        [HttpGet]
        public ActionResult Export()
        {
            var contactList = ContactDbRepository.ListOfContactModel();
            return View(contactList);
        }

        [HttpPost]
        [ActionName("Export")]
        public FileResult ExportData()
        {
            var contactList = ContactDbRepository.ListOfContactModel();
            string dataToBeExport = ContactDbRepository.ExportData(contactList);

            return File(new System.Text.UTF8Encoding().GetBytes(dataToBeExport),
                        "application/vnd.ms-excel", "Contacts.xls");
        }
    }
}