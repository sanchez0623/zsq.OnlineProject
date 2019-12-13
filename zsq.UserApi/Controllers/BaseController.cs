using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zsq.UserApi.Dtos;

namespace zsq.UserApi.Controllers
{
    public class BaseController : Controller
    {
        public UserIdentity UserIdentity => new UserIdentity
        {
            UserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "sub").Value),
            Name = User.Claims.FirstOrDefault(c => c.Type == "name").Value,
            Avatar = User.Claims.FirstOrDefault(c => c.Type == "avatar").Value,
            Company = User.Claims.FirstOrDefault(c => c.Type == "company").Value,
            Title = User.Claims.FirstOrDefault(c => c.Type == "title").Value
        };
    }
}
