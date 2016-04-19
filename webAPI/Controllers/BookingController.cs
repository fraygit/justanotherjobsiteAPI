using jajs.API.Models;
using jajs.MongoData.Interface;
using jajs.MongoData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace jajs.API.Controllers
{
    public class BookingController : ApiController
    {
        private readonly IUserTokenRepository tokenRepository;
        private readonly IAdminUserTokenRepository adminTokenRepository;
        private readonly IBookingRepository bookingRepository;
        private readonly IParkingSpaceRepository parkingSpaceRepository;

        public BookingController(IUserTokenRepository tokenRepository, IBookingRepository bookingRepository, IParkingSpaceRepository parkingSpaceRepository, IAdminUserTokenRepository adminTokenRepository)
        {
            this.tokenRepository = tokenRepository;
            this.bookingRepository = bookingRepository;
            this.parkingSpaceRepository = parkingSpaceRepository;
            this.adminTokenRepository = adminTokenRepository;
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPut]
        public async Task<ResponseAvailability> CheckAvailability(string token, Booking booking)
        {
            try
            {
                booking.FromDate = Helper.ConvertTimeZone(booking.FromDate);
                booking.ToDate = Helper.ConvertTimeZone(booking.ToDate);
                if (await tokenRepository.IsTokenValid(token) || await adminTokenRepository.IsTokenValid(token))
                {   
                    if (booking.ParkingSpaceId == null)
                    {
                        throw new Exception("No parking space specified.");
                    }
                    var parkingSpace = await parkingSpaceRepository.Get(booking.ParkingSpaceId);
                    var availability = new ResponseAvailability();

                    if (BasicBookingValidation(booking, parkingSpace))
                    {                      
                        availability.IsAvailable = await bookingRepository.CheckBookingNoBookingConflict(booking);
                        return availability;
                    }
                    return availability;
                }
                else
                {
                    throw new Exception("Invalid token");
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = ex.Message
                });
            }
        }

        private bool BasicBookingValidation(Booking booking, ParkingSpace parkingSpace)
        {
            booking.FromDate = Helper.ConvertTimeZone(booking.FromDate);
            booking.ToDate = Helper.ConvertTimeZone(booking.ToDate);
            foreach (DateTime day in EachDay(booking.FromDate, booking.ToDate))
            {
                if (!parkingSpace.Availability.Any(n => n.Day == (int)day.DayOfWeek))
                {
                    throw new Exception("Date range selected included an closed day.");
                }
                else
                {
                    var dayAvailable = parkingSpace.Availability.FirstOrDefault(n => n.Day == (int)day.DayOfWeek);
                    if (booking.FromDate.Hour < dayAvailable.From || booking.FromDate.Hour > dayAvailable.To || booking.ToDate.Hour > dayAvailable.To)
                    {
                        throw new Exception("Time range selected does not fall into an available time");
                    }
                }
            }

            if (booking.ParkingSpaceId.Length != 24)
            {
                throw new Exception("Invalid parking space.");
            }            

            TimeSpan timeRange = booking.ToDate - booking.FromDate;
            if (timeRange.TotalMinutes < 0)
            {
                throw new Exception("Date time range not allowed.");
            }
            if (timeRange.TotalMinutes < 10)
            {
                throw new Exception("Minimum booking time range is at least 10 minutes.");
            }

            var timeDifference = DateTime.Now.Subtract(booking.FromDate);
            if (timeDifference.Minutes > 10)
            {
                throw new Exception("Time machine is broken. Unable to book you in the past.");
            }

            return true;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        public async Task<Booking> Book(string token, Booking booking)
        {
            if (await tokenRepository.IsTokenValid(token) || await adminTokenRepository.IsTokenValid(token))
            {
                try
                {
                    var parkingSpace = await parkingSpaceRepository.Get(booking.ParkingSpaceId);
                    if (parkingSpace != null)
                    {
                        if (BasicBookingValidation(booking, parkingSpace))
                        {
                            Booking responseBooking;
                            var isAvailable = await bookingRepository.CheckBookingNoBookingConflict(booking);
                            if (isAvailable)
                            {
                                responseBooking = await bookingRepository.Book(booking.ParkingSpaceId, booking.Username, booking.FromDate, booking.ToDate);
                            }
                            else
                            {
                                throw new Exception("The parking space is not available at the date range you have selected.");
                            }
                            return responseBooking;
                        }
                    }
                    else
                    {
                        throw new Exception("The parking space you selected is not available.");
                    }
                
                }
                catch (Exception ex)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(ex.Message),
                        ReasonPhrase = ex.Message
                    });
                }
            }
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("Invalid token"),
                ReasonPhrase = "Invalid token. Please login."
            });
        }
    }
}
