using jajs.MongoData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jajs.API.Models
{
    public class ResponseAvailability
    {
        public bool IsAvailable { get; set; }
        public decimal Price { get; set; }
    }
}