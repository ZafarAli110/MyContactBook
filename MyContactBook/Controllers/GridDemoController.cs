using MyContactBook.Repository;
using System.Web.Mvc;

namespace MyContactBook.Controllers
{
    public class GridDemoController : Controller
    {
        //Grid.Mvc
        public ActionResult GridMvcDemo()
        {
            var contactList = ContactDbRepository.ListOfContactModel();

            return View(contactList);
        }

        //WebGrid
        public ActionResult WebGridDemo()
        {
            var contactList = ContactDbRepository.ListOfContactModel();

            return View(contactList);
        }

        public ActionResult JqueryDemo()
        {
            return View();
        }
    }
}