using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace zsq.ContactApi.Models
{
    [BsonIgnoreExtraElements]
    public class ContactBook
    {
        public ContactBook()
        {
            Contacts = new List<Contact>();
        }

        public int UserId { get; set; }

        public List<Contact> Contacts { get; set; }
    }
}
