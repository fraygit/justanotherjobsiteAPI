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
    public class EducationController : ApiController
    {
        private readonly IEducationRepository repository;
        private readonly IUserRepository userRepository;
        private readonly IUserTokenRepository tokenRepository;

        public EducationController(IEducationRepository repository, IUserRepository userRepository, IUserTokenRepository tokenRepository)
        {
            this.repository = repository;
            this.tokenRepository = tokenRepository;
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Add education
        /// </summary>
        /// <param name="education"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPut]
        public async Task<Education> Insert(Education education, string token)
        {
            if (await tokenRepository.IsTokenValid(token))
            {
                var user = await userRepository.GetUser(education.Username);
                if (user != null)
                {
                    await repository.CreateSync(education);
                    return education;
                }
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("Unable to retrieve profile."),
                    ReasonPhrase = "User does not exist."
                });
            }
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("Invalid token"),
                ReasonPhrase = "Invalid token. Please login."
            });
        }
    }
}
