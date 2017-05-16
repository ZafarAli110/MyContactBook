using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MyContactBook
{
    [MetadataType(typeof(ContactValidation))] //Apply Validation
    public partial class Contact
    {
        public HttpPostedFileBase File { get; set; }

        public string CountryName { get; set; }

        public string StateName { get; set; }

    }
}