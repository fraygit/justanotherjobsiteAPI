using jajs.MongoData.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jajs.MongoData.Model
{
    public class ParkingImage : MongoEntity
    {
        public string Filename { get; set; }
        public string ParkingSpaceId { get; set; }
        public DateTime DateTimeUploaded { get; set; }
    }
}
