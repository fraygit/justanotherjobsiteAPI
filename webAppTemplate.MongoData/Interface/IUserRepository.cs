using jajs.MongoData.Model;
using jajs.MongoData.Service;
using System.Threading.Tasks;

namespace jajs.MongoData.Interface
{
    public interface IUserRepository : IEntityService<User>
    {
        Task<User> GetUser(string username);
    }
}
