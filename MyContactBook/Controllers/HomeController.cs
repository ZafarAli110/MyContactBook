using MyContactBook.ViewModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyContactBook.Controllers
{
    public class HomeController : Controller
    {


        public ActionResult Index()
        {
            var contactList = new List<ContactModel>();
            using (var dbContext = new ContactDbContext())
            {
                var query = (from tblContact in dbContext.Contacts
                             join tblCountry in dbContext.Countries on tblContact.CountryId equals tblCountry.CountryId
                             join tblState in dbContext.States on tblContact.StateId equals tblState.StateId
                             select new ContactModel
                             {
                                 ContactId = tblContact.ContactId,
                                 FirstName = tblContact.ContactFName,
                                 LastName = tblContact.ContactLName,
                                 EmailAddress = tblContact.EmailAddress,
                                 Contact1 = tblContact.ContactNumber1,
                                 Contact2 = tblContact.ContactNumber2,
                                 Country = tblCountry.CountryName,
                                 State = tblState.StateName,
                                 ImagePath = tblContact.ImagePath
                             }).ToList();

                contactList = query;

                return View(contactList);
            }

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

                string[] allowedFileType = new string[] { "image/png", "image/gif", "image/jpeg", "image/jpg" };
                bool isFileTypeValid = false;

                foreach (var item in allowedFileType)
                {
                    if (file.ContentType == (item.ToString()))
                    {
                        isFileTypeValid = true;
                        break;
                    }
                }

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
                    //string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string fileName = Path.GetFileName(file.FileName);
                    file.SaveAs(Path.Combine(targetPath, fileName));
                    contact.ImagePath = fileName;
                }

                using (var dbContext = new ContactDbContext())
                {
                    // saving contact data into database
                    dbContext.Contacts.Add(contact);
                    dbContext.SaveChanges();
                }

                return RedirectToAction("Index", "Home");
            }
            #endregion

            return View(contact);
        }

        public ActionResult View(int id) //details of particular contact
        {
            // before this we have used view model , now we need to extend the contact class to add countryName & stateName fields.. 
            Contact selecedContact = null;
            selecedContact = GetContact(id);
            return View(selecedContact);
        }

        // here we will create new method to get the details of particular contact as we need this method mutiple times
        private Contact GetContact(int contactId)
        {
            Contact contact = null;
            using (var dbContext = new ContactDbContext())
            {
                var selectedContactFromDatabase = (from tblContacts in dbContext.Contacts
                                                   join tblCountry in dbContext.Countries on tblContacts.CountryId equals tblCountry.CountryId
                                                   join tblStates in dbContext.States on tblContacts.StateId equals tblStates.StateId
                                                   where tblContacts.ContactId.Equals(contactId)
                                                   select new
                                                   {
                                                       tblContacts,
                                                       tblCountry.CountryName,
                                                       tblStates.StateName
                                                   }).FirstOrDefault();

                if (selectedContactFromDatabase != null)
                {
                    contact = selectedContactFromDatabase.tblContacts;
                    contact.CountryName = selectedContactFromDatabase.CountryName;
                    contact.StateName = selectedContactFromDatabase.StateName;
                }

            }

            return contact;
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Contact contact = null;
            contact = GetContact(id);  // a private method which we have created previously

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

                string[] allowedFileType = new string[] { "image/png", "image/jpeg", "image/jpg", "image/gif" };
                bool isFileTypeValid = false;

                foreach (var item in allowedFileType)
                {
                    if (file.ContentType == item.ToString())
                    {
                        isFileTypeValid = true;
                        break;
                    }
                }

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
                    string fileName = Path.GetFileName(file.FileName);
                    file.SaveAs(Path.Combine(targetPath, fileName));
                    contact.ImagePath = fileName;
                }

                using (var dbContext = new ContactDbContext())
                {
                    var selectedContactFromDataBase = dbContext.Contacts.Where(c => c.ContactId.Equals(contact.ContactId)).FirstOrDefault();

                    if (selectedContactFromDataBase != null)
                    {
                        selectedContactFromDataBase.ContactFName = contact.ContactFName;
                        selectedContactFromDataBase.ContactLName = contact.ContactLName;
                        selectedContactFromDataBase.EmailAddress = contact.EmailAddress;
                        selectedContactFromDataBase.ContactNumber1 = contact.ContactNumber1;
                        selectedContactFromDataBase.ContactNumber2 = contact.ContactNumber2;
                        selectedContactFromDataBase.CountryId = contact.CountryId;
                        selectedContactFromDataBase.StateId = contact.StateId;
                        if (file != null)
                        {
                            selectedContactFromDataBase.ImagePath = contact.ImagePath;
                        }

                    }

                    dbContext.SaveChanges();
                }

                return RedirectToAction("Index", "Home");
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
            contact = GetContact(id);
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
                    return RedirectToAction("Index", "Home");
                }

                else
                {
                    return HttpNotFound("Contact Not Found");
                }
            }

        }

    
    public ActionResult WebGridDemo()
    {
        var contactList = new List<ContactModel>();
        using (var dbContext = new ContactDbContext())
        {
            var query = (from tblContact in dbContext.Contacts
                         join tblCountry in dbContext.Countries on tblContact.CountryId equals tblCountry.CountryId
                         join tblState in dbContext.States on tblContact.StateId equals tblState.StateId
                         select new ContactModel
                         {
                             ContactId = tblContact.ContactId,
                             FirstName = tblContact.ContactFName,
                             LastName = tblContact.ContactLName,
                             EmailAddress = tblContact.EmailAddress,
                             Contact1 = tblContact.ContactNumber1,
                             Contact2 = tblContact.ContactNumber2,
                             Country = tblCountry.CountryName,
                             State = tblState.StateName,
                             ImagePath = tblContact.ImagePath
                         }).ToList();
            contactList = query.ToList();
        }

        return View(contactList);
    }

    public ActionResult JqueryDemo()
    {
        return View();
    }


    #region Old logic code which has now no use  
    //private void GeneratingSelectListItemOfCountriesAndStates()
    //{
    //    var dbContext = new ContactDbContext();
    //    var countriesList = dbContext.Countries.OrderBy(c => c.CountryName).ToList();
    //    var statesList = dbContext.States.OrderBy(s => s.StateName).ToList();

    //    #region Old Logic
    //    //List<SelectListItem> countriesSelectList = new List<SelectListItem>();
    //    //List<SelectListItem> statesSelectList = new List<SelectListItem>();
    //    //if (countriesList.Count > 0 && statesList.Count > 0)
    //    //{
    //    //    foreach (var country in countriesList)
    //    //    {
    //    //        SelectListItem s1 = new SelectListItem();

    //    //        s1.Value = country.CountryId.ToString();
    //    //        s1.Text = country.CountryName;

    //    //        countriesSelectList.Add(s1);
    //    //    }
    //    //    foreach (var state in statesList)
    //    //    {
    //    //        SelectListItem s1 = new SelectListItem();

    //    //        s1.Value = state.StateId.ToString();
    //    //        s1.Text = state.StateName;

    //    //        statesSelectList.Add(s1);
    //    //    }
    //    //    ViewData["Countries"] = countriesSelectList;
    //    //    ViewData["States"] = statesSelectList;
    //    //}
    //    #endregion

    //    ViewData["Countries"] = new SelectList(countriesList, "CountryId", "CountryName");
    //    ViewData["States"] = new SelectList(statesList, "StateId", "StateName");
    //}
    #endregion


}
}