using jajs.MongoData.Entities.Base;

namespace jajs.MongoData.Model
{
    public class User : MongoEntity
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Linkedin { get; set; }
        public string Statement { get; set; }
    }
}
