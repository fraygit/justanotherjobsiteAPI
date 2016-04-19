using jajs.MongoData.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jajs.MongoData.Model
{
    public class ParkingSpace : MongoEntity
    {
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string VehicleType { get; set; }
        public int NumberOfVehicle { get; set; }
        //public decimal PricePerHour { get; set; }
        //public DaysAvailable DaysAvailable { get; set; }
        //public string TimeFrom { get; set; }
        //public string TimeTo { get; set; }
        public List<Availability> Availability { get; set; }
        public string ImagePath { get; set; }
        public string OtherDetails { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDeleted { get; set; }
        public int Status { get; set; } // 1 = registered; 0=approved;
        public DateTime DateRegistered { get; set; }
    }

    public class Availability
    {
        public int Day { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public double Price { get; set; }
        //public List<RatesDetail> Sun { get; set; }
        //public List<RatesDetail> Mon { get; set; }
        //public List<RatesDetail> Tue { get; set; }
        //public List<RatesDetail> Wed { get; set; }
        //public List<RatesDetail> Thu { get; set; }
        //public List<RatesDetail> Fri { get; set; }
        //public List<RatesDetail> Sat { get; set; }
    }

    public class RatesDetail
    {
        public decimal Price { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }

    public class DaysAvailable
    {
        public bool Sun { get; set; }
        public bool Mon { get; set; }
        public bool Tue { get; set; }
        public bool Wed { get; set; }
        public bool Thu { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
    }
}
