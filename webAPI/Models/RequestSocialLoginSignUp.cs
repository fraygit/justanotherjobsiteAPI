using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jajs.API.Models
{
    public class RequestSocialLoginSignUp
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}