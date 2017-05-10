using System;
using System.ComponentModel.DataAnnotations;

namespace MyContactBook
{
    public class ContactValidation
    {

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please enter First Name", AllowEmptyStrings = false)]
        public string ContactFName { get; set; }

        [Display(Name = "Last Name")]
        public string ContactLName { get; set; }

        [Display(Name = "Email Id")]
        [RegularExpression(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                           + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$",
            ErrorMessage = "Email not valid")]
        public string EmailAddress { get; set; }

        [Display(Name = "Contact Number1")]
        [Required(ErrorMessage = "Please enter a contact number", AllowEmptyStrings = false)]
        public int ContactNumber1 { get; set; }

        [Display(Name = "Contact Number2")]
        public Nullable<int> ContactNumber2 { get; set; }

    }

    
}