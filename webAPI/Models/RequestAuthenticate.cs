using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jajs.API.Models
{
    public class RequestAuthenticate
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Source { get; set; }
    }
}
