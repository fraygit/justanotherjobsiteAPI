using jajs.API.Models;
using jajs.API.Service;
using jajs.MongoData.Interface;
using jajs.MongoData.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace jajs.API.Controllers
{
    
    public class ParkingSpaceController : ApiController
    {
        private readonly IParkingSpaceRepository repository;
        private readonly IUserRepository UserRepository;
        private readonly IEmailNotificationRepository EmailRepository;
        private readonly IUserTokenRepository tokenRepository;
        private readonly IAdminUserTokenRepository adminTokenRepository;
        private readonly IParkingImageRepository parkingImageRepository;

        public ParkingSpaceController(IParkingSpaceRepository repository, IUserRepository userRepository, IEmailNotificationRepository emailRepository, IUserTokenRepository tokenRepository, IParkingImageRepository parkingImageRepository, IAdminUserTokenRepository adminTokenRepository)
        {
            this.repository = repository;
            this.UserRepository = userRepository;
            this.EmailRepository = emailRepository;
            this.tokenRepository = tokenRepository;
            this.parkingImageRepository = parkingImageRepository;
            this.adminTokenRepository = adminTokenRepository;
        }

        /// <summary>
        /// Get all parking spaces
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<List<ParkingSpace>> GetAllParkingSpace(string token)
        {
            if (await tokenRepository.IsTokenValid(token))
            {
                var parkingSpace = await repository.ListAll();
                return parkingSpace;
            }

            if (await adminTokenRepository.IsTokenValid(token))
            {
                var parkingSpace = await repository.ListAll();
                return parkingSpace;
            }
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("Invalid token"),
                ReasonPhrase = "Invalid token. Please login."
            });
        }

        /// <summary>
        /// Upload a parking space photo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutFile(HttpRequestMessage request)
        {
            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            //var provider = new MultipartFormDataStreamProvider(root);
            var provider = HttpContext.Current.Request.Form;
            try
            {
                //Request.Content.ReadAsMultipartAsync(provider);
                //var parkingSpaceId = provider.FormData.GetValues(0)[0];
                var parkingSpaceId = provider.Get("parkingspaceid");
                //var filename = provider.FormData.GetValues(1)[0];
                var filename = provider.Get("filename");

                //foreach (MultipartFileData file in provider.FileData)
                var uploadedFile = HttpContext.Current.Request.Files[0];
                var newFilename = string.Format("{0}-{1}", DateTime.Now.Ticks.ToString(), filename);
                var newFile = File.Create(Path.Combine("c:\\parko\\ParkingSpace\\Images\\", newFilename));
                uploadedFile.InputStream.CopyTo(newFile);
                //File.Open(file.LocalFileName, FileMode.Open).CopyTo(newFile);
                newFile.Close();

                //foreach (var file in HttpContext.Current.Request.Files)
                //{
                //    var fileInfo = new FileInfo(file.LocalFileName);
                //    var newFilename = string.Format("{0}-{1}", DateTime.Now.Ticks.ToString(), filename);

                    
                //    var newFile = File.Create(Path.Combine("c:\\ParkingSpace\\Images\\", newFilename));
                //    File.Open(file.LocalFileName, FileMode.Open).CopyTo(newFile);
                //    newFile.Close();

                    
                //    // empty
                //}          
                await parkingImageRepository.CreateSync(new ParkingImage
                {
                    DateTimeUploaded = DateTime.UtcNow,
                    Filename = newFilename,
                    ParkingSpaceId = parkingSpaceId.ToString()
                }); 
            }
            catch (Exception ex)
            {
                var emessage = ex.Message + " - " + ex.StackTrace + " - " + ex.InnerException;
                //throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.OK)
                //{
                //    Content = new StringContent(ex.InnerException.ToString() +  " - " + ex.Message),
                //    ReasonPhrase = ex.StackTrace.ToString()
                //});
                return Request.CreateResponse(HttpStatusCode.OK, emessage);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Add a parking space
        /// </summary>
        /// <param name="requestParkingSpace"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostInsert(RequestParkingSpace requestParkingSpace)
        {
            var parkingSpace = new ParkingSpace
            {
                Fullname = requestParkingSpace.Firstname + " " + requestParkingSpace.Surname,
                Username = requestParkingSpace.Username,
                Email = requestParkingSpace.Email,
                Address = requestParkingSpace.Address,
                Phone = requestParkingSpace.Phone,
                //DaysAvailable = requestParkingSpace.DaysAvailable,
                Lat = requestParkingSpace.Lat,
                Lng = requestParkingSpace.Lng,
                NumberOfVehicle = requestParkingSpace.NumberOfVehicle,
                //PricePerHour = requestParkingSpace.PricePerHour,
                //TimeFrom = requestParkingSpace.TimeFrom,
                //TimeTo = requestParkingSpace.TimeTo,
                VehicleType = requestParkingSpace.VehicleType,
                OtherDetails = requestParkingSpace.OtherDetails,
                Availability = requestParkingSpace.Availability,
                IsApproved = false,
                IsDeleted = false,
                DateRegistered = Helper.SetDateForMongo(DateTime.Now),
                Status = 1
            };

            await repository.CreateSync(parkingSpace);

            var user = new User
            {
                Email = parkingSpace.Email.Trim(),
                FirstName = requestParkingSpace.Firstname,
                LastName = requestParkingSpace.Surname,
                Password = requestParkingSpace.Password
            };

            var users = await UserRepository.ListAll();
            if (!users.Any(n => n.Email == requestParkingSpace.Email.Trim()))
            {
                await UserRepository.CreateSync(user);
            }

            var emailService = new MailService(EmailRepository);
            emailService.VendorRegistrationNotification(parkingSpace);
            emailService.VendorRegistrationSendToVendor(parkingSpace);

            return Request.CreateResponse(HttpStatusCode.OK, parkingSpace, Configuration.Formatters.JsonFormatter);
        }
    }
}
