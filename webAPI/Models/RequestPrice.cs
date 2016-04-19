using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jajs.API.Models
{
    public class RequestPrice
    {
        public string ParkingSpaceId { get; set; }
        public int NumberOfHours { get; set; }
        public DateTime BookingDateTimeStart { get; set; }
    }
}