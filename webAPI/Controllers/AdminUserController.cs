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
    public class AdminUserController : ApiController
    {
        private readonly IAdminUserRepository repository;
        private readonly IAdminUserTokenRepository tokenRepository;

        public AdminUserController(IAdminUserRepository repository, IAdminUserTokenRepository tokenRepository)
        {
            this.repository = repository;
            this.tokenRepository = tokenRepository;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        public async Task<AdminUserToken> SignUp(string email, string password, string firstname, string lastname)
        {
            var users = await repository.ListAll();
            if (users.Any(n => n.Email == email))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("User already exist."),
                    ReasonPhrase = "User with the same email address already exist."
                });
            }

            await repository.CreateSync(new AdminUser
            {
                Email = email,
                Password = password,
                FirstName = firstname,
                LastName = lastname
            });

            string generatedToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var newToken = new AdminUserToken
            {
                Username = email,
                LastAccessed = DateTime.Now,
                Source = "mobile",
                Token = generatedToken,

            };
            await tokenRepository.CreateSync(newToken);

            return newToken;
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        public async Task<AdminUserToken> Login(RequestAuthenticate requestAuthenticate)
        {
            var user = await repository.Get(requestAuthenticate.Username);
            if (user != null)
            {
                if (user.Password == requestAuthenticate.Password)
                {
                    var token = await tokenRepository.GetUserToken(requestAuthenticate.Username);
                    if (token != null)
                    {
                        return token;
                    }

                    string generatedToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    var newToken = new AdminUserToken
                    {
                        Username = requestAuthenticate.Username,
                        LastAccessed = DateTime.Now,
                        Source = requestAuthenticate.Source,
                        Token = generatedToken,

                    };
                    await tokenRepository.CreateSync(newToken);

                    return newToken;
                }
            }
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("Invalid username or password"),
                ReasonPhrase = "Invalid username or password"
            });
        }
    }
}
