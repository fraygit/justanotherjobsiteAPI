using jajs.API.Models;
using jajs.MongoData.Interface;
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
    public class ParkNowController : ApiController
    {
        private readonly IParkingSpaceRepository repository;
        private readonly IAdminUserTokenRepository adminTokenRepository;
        private readonly IUserTokenRepository tokenRepository;

        public ParkNowController(IParkingSpaceRepository repository, IAdminUserTokenRepository adminTokenRepository, IUserTokenRepository tokenRepository)
        {
            this.repository = repository;
            this.adminTokenRepository = adminTokenRepository;
            this.tokenRepository = tokenRepository;
        }

        /// <summary>
        /// Get the actual price of the booking
        /// </summary>
        /// <param name="token"></param>
        /// <param name="requestPrice"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        public async Task<Double> GetPrice(string token, RequestPrice requestPrice)
        {
            if (await tokenRepository.IsTokenValid(token) || await adminTokenRepository.IsTokenValid(token))
            {
                var parkingSpace = await repository.Get(requestPrice.ParkingSpaceId);
                double actualPrice = 0;
                
                var bookingStart = Helper.ConvertTimeZone(requestPrice.BookingDateTimeStart);
                for (var i = 0; i < requestPrice.NumberOfHours; i++)
                {
                    var bookingHour = int.Parse(bookingStart.ToString("HH"));
                    var bookingDay = (int)bookingStart.DayOfWeek;
                    var rate = parkingSpace.Availability.FirstOrDefault(n => n.Day == bookingDay && bookingHour >= n.From && bookingHour <= n.To);
                    if (rate != null)
                    {
                        actualPrice += rate.Price;
                        bookingStart = bookingStart.AddHours(1);
                    }
                    else
                    {
                        throw new Exception("Error retrieving price.");
                    }
                }
                return actualPrice;
            }

            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("Invalid token"),
                ReasonPhrase = "Invalid token. Please login."
            });
        }
    
    }
}
