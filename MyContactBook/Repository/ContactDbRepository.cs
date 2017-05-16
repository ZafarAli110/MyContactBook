using MyContactBook.ViewModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;


namespace MyContactBook.Repository
{
    public static class ContactDbRepository
    {
        public static List<ContactModel> ListOfContactModel()
        {
            var contactList = new List<ContactModel>();
            using (var dbContext = new ContactDbContext())
            {
                var contactsFromDb = (from tblContact in dbContext.Contacts
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

                contactList = contactsFromDb;

                return contactList;

            }
        }

        public static Contact GetContact(int contactId)
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

        public static void AddContact(Contact contact)
        {
            using (var dbContext = new ContactDbContext())
            {
                
                dbContext.Contacts.Add(contact);
                dbContext.SaveChanges();
            }
        }

        public static void UpdateContact(Contact contact, HttpPostedFileBase file)
        {
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
        }

        public static bool ValidateFileType(HttpPostedFileBase file)
        {
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

            return isFileTypeValid;
        }

        public static void SaveImageFile(Contact contact, HttpPostedFileBase file ,string targetPath)
        {
            //string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string fileName = Path.GetFileName(file.FileName);
            file.SaveAs(Path.Combine(targetPath, fileName));
            contact.ImagePath = fileName;
        }

        public static string ExportData(List<ContactModel> contactList)
        {
            var grid = new WebGrid(source: contactList, canPage: false, canSort: false);
            string exportData = @grid.GetHtml(
                                        columns: grid.Columns(
                                        grid.Column("FirstName", header: "First Name"),
                                        grid.Column("LastName", header: "Last Name"),
                                        grid.Column("EmailAddress", header: "Email Id"),
                                        grid.Column("Contact1", header: "Contact No.1"),
                                        grid.Column("Contact2", header: "Contact No.2"),
                                        grid.Column("Country", header: "Country"),
                                        grid.Column("State", header: "State")
                                            )
                                        ).ToHtmlString();
            return exportData;
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