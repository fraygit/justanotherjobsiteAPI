using jajs.MongoData.Model;
using jajs.MongoData.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jajs.MongoData.Interface
{
    public interface IParkingImageRepository : IEntityService<ParkingImage>
    {
        Task<ParkingImage> GetByParkingSpace(string parkingSpaceId);
    }
}
