using MongoDB.Bson;
using MongoDB.Driver;
using jajs.MongoData.Interface;
using jajs.MongoData.Model;
using jajs.MongoData.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jajs.MongoData.Repository
{
    public class BookingRepository : EntityService<Booking>, IBookingRepository
    {
        public async Task<bool> CheckBookingNoBookingConflict(Booking booking)
        {
            var builder = Builders<Booking>.Filter;
            var filter = builder.Eq("ParkingSpaceId", booking.ParkingSpaceId);
            var bookings = await ConnectionHandler.MongoCollection.Find(filter).ToListAsync();

            foreach (var item in bookings)
            {
                if ((booking.FromDate >= item.FromDate && booking.FromDate <= item.ToDate) 
                    || (booking.FromDate >= item.ToDate && booking.ToDate <= item.ToDate)
                    || (booking.FromDate <= item.FromDate && booking.ToDate >= item.ToDate))
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<Booking> Book(string parkingSpaceId, string username, DateTime from, DateTime to)
        {
            var booking = new Booking
            {
                ParkingSpaceId = parkingSpaceId,
                Username = username,
                FromDate = from,
                ToDate = to
            };
            await ConnectionHandler.MongoCollection.InsertOneAsync(booking);
            return booking;
        }

        public async Task<Booking> Park(string bookingId, DateTime checkIn)
        {
            var id = ObjectId.Parse(bookingId);
            var builder = Builders<Booking>.Filter;
            var filter = builder.Eq("_id", id);
            var booking = await this.Get(bookingId);
            booking.DateTimeCheckedIn = checkIn;
            var result = await ConnectionHandler.MongoCollection.ReplaceOneAsync(filter, booking);
            return booking;
        }

    }
}
