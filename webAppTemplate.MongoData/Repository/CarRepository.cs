using jajs.MongoData.Interface;
using jajs.MongoData.Model;
using jajs.MongoData.Service;

namespace jajs.MongoData.Repository
{
    public class CarRepository : EntityService<Car>, ICarRepository
    {
    }
}
