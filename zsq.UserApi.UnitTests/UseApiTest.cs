using DotNetCore.CAP;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using zsq.UserApi.Controllers;
using zsq.UserApi.Data;
using zsq.UserApi.Models;

namespace zsq.UserApi.UnitTests
{
    public class UseApiTest
    {
        private UserContext GetUserContext()
        {
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var userContext = new UserContext(options);
            userContext.Users.Add(new AppUser
            {
                Id = 1,
                Name = "sanchez"
            });
            userContext.SaveChanges();
            return userContext;

        }

        private (UserController userController, UserContext userContext) GetUserController()
        {
            var userContext = GetUserContext();
            var loggerMoq = new Mock<ILogger<UserController>>();
            var capPublisher = new Mock<ICapPublisher>();
            var logger = loggerMoq.Object;

            var controller = new UserController(userContext, capPublisher.Object);
            return (controller, userContext);
        }

        [Fact]
        public async Task Get_ReturnUser_WithExpectedPara()
        {
            (UserController controller, UserContext userContext) = GetUserController();
            var result = await controller.Get();
            //Assert.IsType<JsonResult>(result);

            //ʹ��fluent assertions
            var subject = result.Should().BeOfType<JsonResult>().Subject;
            var user = subject.Value.Should().BeAssignableTo<AppUser>().Subject;
            user.Id.Should().Be(1);
            user.Name.Should().Be("sanchez");
        }

        [Fact]
        public async Task Get_UpdateName_WithAddProperty()
        {
            (UserController controller, UserContext context) = GetUserController();
            var document = new JsonPatchDocument<AppUser>();
            document.Replace(u => u.Name, "zou");
            var result = await controller.Patch(document);

            //ʹ��fluent assertions
            var subject = result.Should().BeOfType<JsonResult>().Subject;

            //�ж�api�Ƿ񷵻���ȷ
            var user = subject.Value.Should().BeAssignableTo<AppUser>().Subject;
            user.Name.Should().Be("zou");

            //�ж����ݿ��Ƿ���ȷ
            var userModel = await context.Users.SingleOrDefaultAsync(u => u.Id == 1);
            userModel.Should().NotBeNull();
            userModel.Name.Should().Be("zou");
        }

        [Fact]
        public async Task Get_ReturnNewProperty_WithNewProperty()
        {
            (UserController controller, UserContext context) = GetUserController();
            var document = new JsonPatchDocument<AppUser>();
            document.Replace(u => u.Properties, new List<UserProperty> {
                new UserProperty{ Key="fin_stage", Text="A��", Value="A��"}
            });
            var result = await controller.Patch(document);

            //ʹ��fluent assertions
            var subject = result.Should().BeOfType<JsonResult>().Subject;

            //�ж�api�Ƿ񷵻���ȷ
            var user = subject.Value.Should().BeAssignableTo<AppUser>().Subject;
            user.Properties.Count.Should().Be(1);
            user.Properties.First().Value.Should().Be("A��");
            user.Properties.First().Key.Should().Be("fin_stage");

            //�ж����ݿ��Ƿ���ȷ
            var userModel = await context.Users.SingleOrDefaultAsync(u => u.Id == 1);
            userModel.Properties.Count.Should().Be(1);
            userModel.Properties.First().Value.Should().Be("A��");
            userModel.Properties.First().Key.Should().Be("fin_stage");
        }

        [Fact]
        public async Task Get_ReturnEmtpy_WithRemoveProperty()
        {
            (UserController controller, UserContext context) = GetUserController();
            var document = new JsonPatchDocument<AppUser>();
            document.Replace(u => u.Properties, new List<UserProperty> { });
            var result = await controller.Patch(document);

            //ʹ��fluent assertions
            var subject = result.Should().BeOfType<JsonResult>().Subject;

            //�ж�api�Ƿ񷵻���ȷ
            var user = subject.Value.Should().BeAssignableTo<AppUser>().Subject;
            user.Properties.Should().BeEmpty();

            //�ж����ݿ��Ƿ���ȷ
            var userModel = await context.Users.SingleOrDefaultAsync(u => u.Id == 1);
            userModel.Properties.Should().BeEmpty();
        }
    }
}
