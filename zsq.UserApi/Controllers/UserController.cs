using DotNetCore.CAP;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zsq.UserApi.Data;
using zsq.UserApi.Dtos;
using zsq.UserApi.Infrastructure;
using zsq.UserApi.Models;
using zsq.UserApi.ViewModels;

namespace zsq.UserApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : BaseController
    {
        private UserContext _userContext { get; set; }
        private ICapPublisher _capPublisher { get; set; }

        public UserController(UserContext userContext, ICapPublisher capPublisher)
        {
            _userContext = userContext;
            _capPublisher = capPublisher;
        }

        private void UserProfileChangedEvent(AppUser user)
        {
            if (_userContext.Entry(user).Property(nameof(user.Name)).IsModified ||
                _userContext.Entry(user).Property(nameof(user.Company)).IsModified ||
                _userContext.Entry(user).Property(nameof(user.Title)).IsModified ||
                _userContext.Entry(user).Property(nameof(user.Avatar)).IsModified)
            {
                _capPublisher.Publish("zsq.onlinePrject.userapi.userprofilechanged", new UserIdentity
                {
                    Avatar = user.Avatar,
                    Company = user.Company,
                    Name = user.Name,
                    Title = user.Title,
                    UserId = user.Id
                });
            }
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await _userContext.Users
                .AsNoTracking()
                .Include(u => u.Properties)
                .SingleOrDefaultAsync(u => u.Id == UserIdentity.UserId);

            if (user == null)
                throw new UserException($"用户id异常：{UserIdentity.UserId}");

            return Json(user);
        }

        [Route("baseInfo/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetBaseInfo(int userId)
        {
            //Todo: 判断是否为好友关系

            //Attention: 如果用到了UserIdentity，则调用方必须传Token

            var user = await _userContext.Users.SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound();

            return Json(new UserInfoViewModel
            {
                Avatar = user.Avatar,
                Company = user.Company,
                Id = user.Id,
                Name = user.Name,
                Title = user.Title
            });
        }

        [Route("")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]JsonPatchDocument<AppUser> patch)
        {
            var user = await _userContext.Users.SingleOrDefaultAsync(u => u.Id == UserIdentity.UserId);

            if (user == null)
                throw new UserException($"用户id异常：{UserIdentity.UserId}");

            patch.ApplyTo(user);

            //如果不detach掉，那么再添加重复添加时，会报异常
            foreach (var property in user?.Properties)
            {
                _userContext.Entry(property).State = EntityState.Detached;
            }

            var originProperty = await _userContext.UserProperties
                .AsNoTracking()
                .Where(p => p.AppUserId == UserIdentity.UserId)
                .ToListAsync();
            var allProperty = originProperty.Union(user.Properties).Distinct();

            var removeProperty = originProperty.Except(user.Properties);
            var addProperty = allProperty.Except(originProperty);
            foreach (var property in removeProperty)
            {
                _userContext.Remove(property);
            }
            foreach (var property in addProperty)
            {
                _userContext.Add(property);
            }

            //简单方式：遇到数组要更新时，可以先全部删除，再添加新的数组数据

            using (var transaction = _userContext.Database.BeginTransaction(_capPublisher))
            {
                //要在SaveChangesAsync之前，否则IsModified因为已经savechanges，将会失效
                UserProfileChangedEvent(user);

                _userContext.Users.Update(user);
                await _userContext.SaveChangesAsync();

                transaction.Commit();
            }

            return Json(user);
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> CheckOrCreate([FromForm]string phone)
        {
            if (StringHelper.IsPhone(phone))
                return BadRequest("手机号错误");

            var user = await _userContext.Users.SingleOrDefaultAsync(u => u.Phone == phone);
            if (user == null)
            {
                user = new AppUser
                {
                    Phone = phone
                };
                _userContext.Users.Add(user);
                await _userContext.SaveChangesAsync();
            }
            return Ok(new UserInfoViewModel
            {
                Id = user.Id,
                Avatar = user.Avatar,
                Company = user.Company,
                Name = user.Name,
                Title = user.Title
            });
        }

        [HttpGet]
        [Route("tags")]
        public async Task<IActionResult> GetUserTags()
        {
            var tags = await _userContext.UserTags.Where(u => u.AppUserId == UserIdentity.UserId).ToListAsync();
            return Ok(tags);
        }

        [HttpGet]
        [Route("search/{phone}")]
        public async Task<IActionResult> Search(string phone)
        {
            if (StringHelper.IsPhone(phone))
                return BadRequest("手机号错误");

            var user = await _userContext.Users.Include(u => u.Properties).SingleOrDefaultAsync(u => u.Phone == phone);
            return Ok(user);
        }

        [HttpPut]
        [Route("tags")]
        public async Task<IActionResult> UpdateUserTags(List<string> tags)
        {
            var originTags = await _userContext.UserTags.Where(u => u.AppUserId == UserIdentity.UserId).ToListAsync();
            var newTags = tags.Except(originTags.Select(t => t.Tag));

            await _userContext.UserTags.AddRangeAsync(newTags.Select(t => new UserTag
            {
                AppUserId = UserIdentity.UserId,
                Tag = t,
                CreateTime = DateTime.Now
            }));
            await _userContext.SaveChangesAsync();

            return Ok();
        }
    }
}
