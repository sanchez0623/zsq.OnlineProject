using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using zsq.ContactApi.Data;
using zsq.ContactApi.Models;
using zsq.ContactApi.Services;
using zsq.ContactApi.ViewModels;

namespace zsq.ContactApi.Controllers
{
    [Route("api/contacts")]
    [ApiController]
    public class ContactController : BaseController
    {
        public IContactApplyRequestRepository _contactApplyRequestRepository;
        public IContactRepository _contactRepository;
        public IUserService _userService;
        public ILogger<ContactController> _logger;

        public ContactController(IContactApplyRequestRepository contactApplyRequestRepository,
             IContactRepository contactRepository,
            IUserService userService,
            ILogger<ContactController> logger)
        {
            _contactApplyRequestRepository = contactApplyRequestRepository;
            _contactRepository = contactRepository;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// 好友申请列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("applyRequest")]
        public async Task<IActionResult> GetApplyRequests(CancellationToken cancellationToken)
        {
            var applyRequests = await _contactApplyRequestRepository.GetRequestListAsync(UserIdentity.UserId, cancellationToken);
            return Ok(applyRequests);
        }

        /// <summary>
        /// 添加好友请求
        /// </summary>
        /// <param name="userId">被添加的用户id</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("applyRequest/{userId}")]
        public async Task<IActionResult> AddApplyRequest(int userId, CancellationToken cancellationToken)
        {
            //改用从claims中获取用户信息
            var userInfo = await _userService.GetUserInfoAsync(userId);
            ////if (userInfo == null)
            ////{
            ////    throw new ArgumentException($"未找到被添加的用户 {userId}");
            ////}

            if (userId == UserIdentity.UserId)
            {
                return BadRequest("自己还加自己，搞笑吗?");
            }

            var result = await _contactApplyRequestRepository.AddRequestAsync(new ContactApplyRequest
            {
                UserId = userId,
                ApplierId = UserIdentity.UserId,
                CreateTime = DateTime.Now,
                Company = UserIdentity.Company,//userInfo.Company,
                Title = UserIdentity.Title,//userInfo.Title,
                Name = UserIdentity.Name,//userInfo.Name,
                Avatar = UserIdentity.Avatar,//userInfo.Avatar
            }, cancellationToken);

            if (!result)
            {
                _logger.LogWarning($"用户{UserIdentity.UserId} 添加好友-{userId}-请求 失败");
                return BadRequest();
            }

            return Ok();
        }

        /// <summary>
        /// 通过好友请求
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("applyRequest/{applierId}")]
        public async Task<IActionResult> ApproalRequest(int applierId, CancellationToken cancellationToken)
        {
            var result = await _contactApplyRequestRepository.ApprovalAsync(UserIdentity.UserId, applierId, cancellationToken);

            if (!result)
            {
                _logger.LogWarning($"用户{UserIdentity.UserId} 通过好友-{applierId}-请求 失败");
                return BadRequest();
            }

            var applierInfo = await _userService.GetUserInfoAsync(applierId);
            var userInfo = await _userService.GetUserInfoAsync(UserIdentity.UserId);

            await _contactRepository.AddContactAsync(applierId, userInfo, cancellationToken);
            await _contactRepository.AddContactAsync(UserIdentity.UserId, applierInfo, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// 获取好友列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            return Ok(await _contactRepository.GetContactsAsync(UserIdentity.UserId, cancellationToken));
        }

        /// <summary>
        /// 获取好友列表
        /// <para>需要通过网关屏蔽掉外部访问的权限</para>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> Get(int userId, CancellationToken cancellationToken)
        {
            return Ok(await _contactRepository.GetContactsAsync(userId, cancellationToken));
        }

        /// <summary>
        /// 给用户打标签
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("tag")]
        public async Task<IActionResult> TagContact([FromBody]TagCantactViewModel viewModel, CancellationToken cancellationToken)
        {
            var result = await _contactRepository.TagContactAsync(UserIdentity.UserId, viewModel.ContactId,
                viewModel.Tags, cancellationToken);
            if (!result)
            {
                _logger.LogError($"用户 {UserIdentity.UserId} 给好友 {viewModel.ContactId} 打标签失败");
                return BadRequest("打标签失败");
            }

            return Ok();
        }
    }
}
