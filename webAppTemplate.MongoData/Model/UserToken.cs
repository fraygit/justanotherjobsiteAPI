using jajs.MongoData.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jajs.MongoData.Model
{
    public class UserToken : MongoEntity
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string Source { get; set; }
        public DateTime LastAccessed { get; set; }
    }
}
