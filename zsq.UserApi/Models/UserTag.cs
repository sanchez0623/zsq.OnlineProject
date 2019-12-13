using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zsq.UserApi.Models
{
    public class UserTag
    {
        public int AppUserId { get; set; }

        public string Tag { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
