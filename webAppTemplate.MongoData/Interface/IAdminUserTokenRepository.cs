using jajs.MongoData.Model;
using jajs.MongoData.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jajs.MongoData.Interface
{
    public interface IAdminUserTokenRepository : IEntityService<AdminUserToken>
    {
        Task<AdminUserToken> GetUserToken(string username);
        Task<bool> IsTokenValid(string userToken);
    }
}
