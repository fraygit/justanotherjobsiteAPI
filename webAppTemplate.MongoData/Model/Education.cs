using jajs.MongoData.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jajs.MongoData.Model
{
    public class Education : MongoEntity
    {
        public string Username { get; set; }
        public string Institution { get; set; }
        public string DateAttained { get; set; }
        public string Major { get; set; }
        public string Degree { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
    }
}
