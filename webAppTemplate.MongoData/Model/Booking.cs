using jajs.MongoData.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jajs.MongoData.Model
{
    public class Booking : MongoEntity
    {
        public string Username { get; set; }
        public string ParkingSpaceId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime DateTimeCheckedIn { get; set; }
        public DateTime DateTimeCheckedOut { get; set; }
    }
}
