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
    public class ParkingDetailsController : ApiController
    {
        private readonly IParkingSpaceRepository repository;
        private readonly IAdminUserTokenRepository adminTokenRepository;
        private readonly IUserTokenRepository tokenRepository;

        public ParkingDetailsController(IParkingSpaceRepository repository, IAdminUserTokenRepository adminTokenRepository, IUserTokenRepository tokenRepository)
        {
            this.repository = repository;
            this.adminTokenRepository = adminTokenRepository;
            this.tokenRepository = tokenRepository;
        }

        /// <summary>
        /// Get details of a specific parking space
        /// </summary>
        /// <param name="token"></param>
        /// <param name="parkingId"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<ParkingSpace> GetAllParkingSpace(string token, string parkingId)
        {
            if (await tokenRepository.IsTokenValid(token) || await adminTokenRepository.IsTokenValid(token))
            {
                var parkingSpace = await repository.Get(parkingId);
                return parkingSpace;
            }

            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("Invalid token"),
                ReasonPhrase = "Invalid token. Please login."
            });
        }

        /// <summary>
        /// Update a specific parking space
        /// </summary>
        /// <param name="token"></param>
        /// <param name="parkingId"></param>
        /// <param name="requestParkingSpace"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        public async Task<ParkingSpace> Update(string token, string parkingId, RequestParkingSpace requestParkingSpace)
        {
            if (await adminTokenRepository.IsTokenValid(token))
            {
                try
                {
                    var parkingSpace = new ParkingSpace
                    {
                        Fullname = requestParkingSpace.Firstname + " " + requestParkingSpace.Surname,
                        Username = requestParkingSpace.Username,
                        Email = requestParkingSpace.Email,
                        Address = requestParkingSpace.Address,
                        Phone = requestParkingSpace.Phone,
                        Lat = requestParkingSpace.Lat,
                        Lng = requestParkingSpace.Lng,
                        NumberOfVehicle = requestParkingSpace.NumberOfVehicle,
                        VehicleType = requestParkingSpace.VehicleType,
                        OtherDetails = requestParkingSpace.OtherDetails,
                        Availability = requestParkingSpace.Availability,
                        IsApproved = false,
                        IsDeleted = false,
                        DateRegistered = Helper.SetDateForMongo(DateTime.Now),
                        Status = requestParkingSpace.Status
                    };

                    await repository.Update(parkingId, parkingSpace);
                    return parkingSpace;
                }
                catch (Exception ex)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(ex.Message),
                        ReasonPhrase = ex.StackTrace
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
