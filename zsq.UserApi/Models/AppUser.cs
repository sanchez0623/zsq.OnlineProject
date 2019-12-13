using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zsq.UserApi.Models
{
    public class AppUser
    {
        public AppUser()
        {
            Properties = new List<UserProperty>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Company { get; set; }

        public string Title { get; set; }

        public string Avatar { get; set; }

        public byte Gender { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Tel { get; set; }

        public string Email { get; set; }

        public int ProvinceId { get; set; }

        public string Province { get; set; }

        public int CityId { get; set; }

        public string City { get; set; }

        public string NameCard { get; set; }

        public List<UserProperty> Properties { get; set; }

    }
}
