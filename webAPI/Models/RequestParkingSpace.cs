using jajs.MongoData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jajs.API.Models
{
    public class RequestParkingSpace
    {
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string VehicleType { get; set; }
        public decimal PricePerHour { get; set; }
        //public DaysAvailable DaysAvailable { get; set; }
        public int NumberOfVehicle { get; set; }
        //public string TimeFrom { get; set; }
        //public string TimeTo { get; set; }
        public List<Availability> Availability { get; set; }
        public string ImagePath { get; set; }
        public string OtherDetails { get; set; }
        public int Status { get; set; }
    }

    //public class Availability
    //{
    //    public RatesDetail Sun { get; set; }
    //    public RatesDetail Mon { get; set; }
    //    public RatesDetail Tue { get; set; }
    //    public RatesDetail Wed { get; set; }
    //    public RatesDetail Thu { get; set; }
    //    public RatesDetail Fri { get; set; }
    //    public RatesDetail Sat { get; set; }
    //}

    //public class RatesDetail
    //{
    //    public decimal Price { get; set; }
    //    public string Type { get; set; }
    //}
}
