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
    public class ProfileController : ApiController
    {
        private readonly IUserRepository repository;
        private readonly IUserTokenRepository tokenRepository;

        public ProfileController(IUserRepository repository, IUserTokenRepository tokenRepository)
        {
            this.repository = repository;
            this.tokenRepository = tokenRepository;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        public async Task<User> Update(User requestUser, string token)
        {
            if (await tokenRepository.IsTokenValid(token))
            {
                var user = await repository.GetUser(requestUser.Email);
                if (user != null)
                {
                    await repository.Update(user.Id.ToString(), requestUser);
                    return requestUser;
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

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        public async Task<User> Get(string email, string token)
        {
            if (await tokenRepository.IsTokenValid(token))
            {
                var user = await repository.GetUser(email);
                if (user != null)
                {
                    return user;
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
