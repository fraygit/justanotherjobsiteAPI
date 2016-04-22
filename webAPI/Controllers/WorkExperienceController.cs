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
    public class WorkExperienceController : ApiController
    {
        private readonly IWorkExperienceRepository repository;
        private readonly IUserRepository userRepository;
        private readonly IUserTokenRepository tokenRepository;

        public WorkExperienceController(IWorkExperienceRepository repository, IUserRepository userRepository, IUserTokenRepository tokenRepository)
        {
            this.repository = repository;
            this.tokenRepository = tokenRepository;
            this.userRepository = userRepository;
        }


        /// <summary>
        /// Add Work experience
        /// </summary>
        /// <param name="requestUser"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPut]
        public async Task<WorkExperience> Update(WorkExperience workexperience, string token)
        {
            if (await tokenRepository.IsTokenValid(token))
            {
                var user = await userRepository.GetUser(workexperience.Username);
                if (user != null)
                {
                    await repository.CreateSync(workexperience);
                    return workexperience;
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

        [ActionName("GetAll")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        public async Task<List<WorkExperience>> GetAll(string email, string token)
        {
            if (await tokenRepository.IsTokenValid(token))
            {
                var user = await userRepository.GetUser(email);
                if (user != null)
                {
                    var workExperiences = await repository.GetByEmail(email);
                    return workExperiences;
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
