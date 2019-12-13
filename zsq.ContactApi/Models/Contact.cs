using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zsq.ContactApi.Models
{
    public class Contact
    {
        public Contact()
        {
            Tags = new List<string>();
        }

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Cpmpany { get; set; }

        public string Title { get; set; }

        public string Avatar { get; set; }

        public List<string> Tags { get; set; }
    }
}
