using MyContactBook.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyContactBook.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult AllContacts()
        {
            var contactList = ContactDbRepository.ListOfContactModel();
            return View(contactList);
        }

        [HttpGet]
        public ActionResult Add()
        {
            //Fetch country and states data for dropdown list
            var allCountries = new List<Country>();
            var allStates = new List<State>();

            using (var dbContext = new ContactDbContext())
            {
                allCountries = dbContext.Countries.OrderBy(c => c.CountryName).ToList();
                // No need to fetch the states now as we dont know which country will user select here 
            }

            ViewData["Countries"] = new SelectList(allCountries, "CountryId", "CountryName");
            ViewData["States"] = new SelectList(allStates, "StateId", "StateName");

            return View();
        }

        // Here we add one more method to get the states of respective country from jquery code
        public JsonResult GetStates(int countryId)
        {
            // we need to off the lazy loading in the constructor of our ContactDbContext class otherwise our ajax call wont work
            using (var dbContext = new ContactDbContext())
            {
                var states = (from state in dbContext.States
                              where state.CountryId.Equals(countryId)
                              orderby state.StateName
                              select state).ToList();
                return new JsonResult { Data = states, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Contact contact, HttpPostedFileBase file)
        {

            #region step#1 : Fetch Countries and States

            var allCountries = new List<Country>();
            var allStates = new List<State>();
            using (var dbContext = new ContactDbContext())
            {
                allCountries = dbContext.Countries.OrderBy(c => c.CountryName).ToList();
                if (contact.ContactId > 0)
                {
                    // Fetching all the states whose countryId == contact.CountryId (i.e all the states of respective selected country)
                    allStates = dbContext.States.Where(s => s.CountryId.Equals(contact.CountryId)).OrderBy(s => s.StateName).ToList();
                }
            }
            ViewData["Countries"] = new SelectList(allCountries, "CountryId", "CountryName", contact.CountryId);
            ViewData["States"] = new SelectList(allStates, "StateId", "StateName", contact.StateId);
            #endregion

            #region step#2 : Validate the file if selected

            if (file != null)
            {
                if (file.ContentLength > (512 * 1000))  // if file size > then 512KB
                {
                    ModelState.AddModelError("FileErrorMessage", "File size must be within 512KB");
                }

                bool isFileTypeValid = false;

                isFileTypeValid = ContactDbRepository.ValidateFileType(file);

                if (isFileTypeValid == false)
                {
                    ModelState.AddModelError("FileErrorMessage", "only .png , .gif, .jpeg , .jpg file types are allowed");
                }
            }
            #endregion

            #region step#3 : Validate Model and Save the data into the database

            if (ModelState.IsValid)
            {

                if (file != null)
                {
                    string targetPath = Server.MapPath("~/Content/Images");
                    ContactDbRepository.SaveImageFile(contact, file, targetPath);
                }

                ContactDbRepository.AddContact(contact); // adding and saving contact data into database

                return RedirectToAction("AllContacts", "Home");
            }
            #endregion

            return View(contact);
        }

        [HttpGet]
        public ActionResult View(int id) //details 
        {
            // before this we have used view model , now we need to extend the contact class to add countryName & stateName fields.. 
            Contact selecedContact = null;
            selecedContact = ContactDbRepository.GetContact(id);
            return View(selecedContact);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Contact contact = null;
            contact = ContactDbRepository.GetContact(id);  // a private method which we have created previously

            if (contact == null)
            {
                return HttpNotFound("Contact Not Found");
            }

            //fetching States and Countries for dropdownlist
            var allCounteries = new List<Country>();
            var allStates = new List<State>();
            using (var dbContext = new ContactDbContext())
            {
                allCounteries = dbContext.Countries.OrderBy(c => c.CountryName).ToList();
                allStates = dbContext.States.Where(s => s.CountryId.Equals(contact.CountryId)).OrderBy(s => s.StateName).ToList();
            }
            ViewBag.Countries = new SelectList(allCounteries, "CountryId", "CountryName", contact.CountryId);
            ViewBag.States = new SelectList(allStates, "StateId", "StateName", contact.StateId);

            return View(contact);
        }

        [HttpPost]
        public ActionResult Edit(Contact contact, HttpPostedFileBase file)
        {
            #region//fetching States and Countries for dropdownlist
            var allCounteries = new List<Country>();
            var allStates = new List<State>();
            using (var dbContext = new ContactDbContext())
            {
                allCounteries = dbContext.Countries.OrderBy(c => c.CountryName).ToList();
                if (allCounteries.Count > 0)
                {
                    allStates = dbContext.States.Where(s => s.CountryId.Equals(contact.CountryId)).OrderBy(s => s.StateName).ToList();
                }

            }
            ViewBag.Countries = new SelectList(allCounteries, "CountryId", "CountryName", contact.CountryId);
            ViewBag.States = new SelectList(allStates, "StateId", "StateName", contact.StateId);

            #endregion

            #region// Validate File
            if (file != null)
            {
                if (file.ContentLength > (512 * 1000)) //if file size > 512KB
                {
                    ModelState.AddModelError("FileErrorMessage", "File size must be within a range of 512KB");
                }

                bool isFileTypeValid = false;

                isFileTypeValid = ContactDbRepository.ValidateFileType(file);

                if (isFileTypeValid == false)
                {
                    ModelState.AddModelError("FileErrorMessage", "Only .png, .gif, .jpg , .jpeg Types are allow");
                }
            }
            #endregion

            #region //Validate Model and Save changes into the database
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string targetPath = Server.MapPath("~/Content/Images");
                    ContactDbRepository.SaveImageFile(contact, file, targetPath);
                }

                ContactDbRepository.UpdateContact(contact, file);

                return RedirectToAction("AllContacts", "Home");
            }
            else
            {
                return View(contact);
            }
            #endregion
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Contact contact = null;
            contact = ContactDbRepository.GetContact(id);
            if (contact != null)
            {
                return View(contact);
            }
            else
            {
                return HttpNotFound("Contact Not Found");
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(int id)
        {
            using (var dbContext = new ContactDbContext())
            {
                var contact = dbContext.Contacts.Where(c => c.ContactId == id).FirstOrDefault();

                if (contact != null)
                {
                    dbContext.Contacts.Remove(contact);
                    dbContext.SaveChanges();

                    return RedirectToAction("AllContacts", "Home");
                }
                else
                {
                    return HttpNotFound("Contact Not Found");
                }
            }
        }

     }
}