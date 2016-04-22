using jajs.MongoData.Interface;
using jajs.MongoData.Model;
using jajs.MongoData.Service;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jajs.MongoData.Repository
{
    public class WorkExperienceRepository : EntityService<WorkExperience>, IWorkExperienceRepository
    {
        public async Task<List<WorkExperience>> GetByEmail(string email)
        {
            var builder = Builders<WorkExperience>.Filter;
            var filter = builder.Eq("Username", email);
            var workExperience = await ConnectionHandler.MongoCollection.Find(filter).ToListAsync();
            return workExperience;
        }
    }
}
