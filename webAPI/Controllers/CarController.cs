using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using jajs.MongoData.Interface;
using jajs.MongoData.Model;
using jajs.MongoData.Repository;

namespace jajs.API.Controllers
{
    public class CarController : ApiController
    {
        private readonly ICarRepository repository;

        public CarController(ICarRepository repository)
        {
            this.repository = repository;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<List<Car>> GetAllCars()
        {
            var cars = await repository.ListAll();
            return cars;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostInsert(Car car)
        {
            await repository.CreateSync(car);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutUpdate([FromUri]string id, [FromBody]Car car)
        {
            try
            {
                var result = await repository.Update(id, car);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteCar([FromUri]string id)
        {
            try
            {
                var result = await repository.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

    }
}
