using jajs.MongoData.Interface;
using jajs.MongoData.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace jajs.API.Controllers
{
    public class ParkingSpaceImageController : ApiController
    {
        private readonly IAdminUserTokenRepository adminTokenRepository;
        private readonly IParkingImageRepository repository;

        public ParkingSpaceImageController(IAdminUserTokenRepository adminTokenRepository, IParkingImageRepository repository)
        {
            this.adminTokenRepository = adminTokenRepository;
            this.repository = repository;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage> Get(string token, string parkingId)
        {
            if (await adminTokenRepository.IsTokenValid(token))
            {
                var image = await repository.GetByParkingSpace(parkingId);
                var imagePath = Path.Combine(@"c:\\parko\\ParkingSpace\\Images\\", image.Filename);
                if (File.Exists(imagePath))
                {
                    var resp = new HttpResponseMessage();

                    using (var file = File.OpenRead(imagePath))
                    {
                        using (var stream = new MemoryStream())
                        {
                            file.CopyTo(stream);
                            resp.Content = new ByteArrayContent(stream.ToArray());

                        }

                        // Find the MIME typeB
                        var mime = Path.GetExtension(imagePath);
                        string mimeType = mime.Substring(1, mime.Length - 1);
                        //mimeType = mimeType == "jpg" ? "jpeg" : mimeType;
                        resp.StatusCode = HttpStatusCode.OK;
                        resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/" + mimeType);
                        return resp;
                    }
                }
                else
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("File could not be found."),
                        ReasonPhrase = "File does not exist."
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
