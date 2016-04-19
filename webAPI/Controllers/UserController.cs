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
    public class UserController : ApiController
    {
        private readonly IUserRepository repository;
        private readonly IUserTokenRepository tokenRepository;

        public UserController(IUserRepository repository, IUserTokenRepository tokenRepository)
        {
            this.repository = repository;
            this.tokenRepository = tokenRepository;
        }

        /// <summary>
        /// User signup
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        public async Task<UserToken> SignUp(string email, string password, string firstname, string lastname)
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

            await repository.CreateSync(new User
            {
                Email = email,
                Password = password,
                FirstName = firstname,
                LastName = lastname
            });

            string generatedToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var newToken = new UserToken
            {
                Username = email,
                LastAccessed = DateTime.Now,
                Source = "mobile",
                Token = generatedToken,

            };
            await tokenRepository.CreateSync(newToken);

            return newToken;
        }

        /// <summary>
        /// User login using social
        /// </summary>
        /// <param name="requestAuthenticate"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPut]
        public async Task<UserToken> LoginSignUp(RequestSocialLoginSignUp requestAuthenticate)
        {
            var user = await repository.GetUser(requestAuthenticate.Email);
            if (user == null)
            {
                var newUser = new User
                {
                    Email = requestAuthenticate.Email,
                    FirstName = requestAuthenticate.FirstName,
                    LastName = requestAuthenticate.LastName
                };
                await repository.CreateSync(newUser);
                user = newUser;
            }

            string generatedToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var newToken = new UserToken
            {
                Username = user.Email,
                LastAccessed = DateTime.Now,
                Source = "mobile",
                Token = generatedToken,
            };
            await tokenRepository.CreateSync(newToken);
            return newToken;
        }

        /// <summary>
        /// User login
        /// </summary>
        /// <param name="requestAuthenticate"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        public async Task<UserToken> Login(RequestAuthenticate requestAuthenticate)
        {
            var user = await repository.GetUser(requestAuthenticate.Username);
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
                    var newToken = new UserToken
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
