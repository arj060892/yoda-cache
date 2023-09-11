using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Yoda.Cache.API.Controllers;
using Yoda.Cache.API.Models;
using Yoda.Cache.API.Services;

namespace Yoda.Cache.API.Tests.Controllers
{
    [TestFixture]
    public class UsersControllerTests
    {
        protected static IFixture CreateFixture()
        {
            return new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        public class Get : UsersControllerTests
        {
            [Test]
            public async Task ReturnsUserIfExists()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                Mock<IUserService> mockUserService = fixture.Freeze<Mock<IUserService>>();
                User user = fixture.Create<User>();
                mockUserService.Setup(u => u.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

                UsersController controller = new UsersController(mockUserService.Object);

                // Act
                IActionResult result = await controller.Get(user.Id);

                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(result, Is.InstanceOf<OkObjectResult>());
                    Assert.That(((OkObjectResult)result).Value, Is.EqualTo(user));
                });
            }

            [Test]
            public async Task ReturnsNotFoundIfUserDoesNotExist()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                Mock<IUserService> mockUserService = fixture.Freeze<Mock<IUserService>>();
                mockUserService.Setup(u => u.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null);

                UsersController controller = new UsersController(mockUserService.Object);

                // Act
                IActionResult result = await controller.Get(fixture.Create<int>());

                // Assert
                Assert.That(result, Is.InstanceOf<NotFoundResult>());
            }
        }

        public class Post : UsersControllerTests
        {
            [Test]
            public async Task AddsUserAndReturnsCreatedAtActionResult()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                Mock<IUserService> mockUserService = fixture.Freeze<Mock<IUserService>>();
                User user = fixture.Create<User>();

                UsersController controller = new UsersController(mockUserService.Object);

                // Act
                IActionResult result = await controller.Post(user);

                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
                    Assert.That(((CreatedAtActionResult)result).Value, Is.EqualTo(user));
                });
            }
        }

        public class Put : UsersControllerTests
        {
            [Test]
            public async Task ReturnsBadRequestIfIdDoesNotMatchUserId()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                User user = fixture.Create<User>();

                UsersController controller = new UsersController(fixture.Freeze<Mock<IUserService>>().Object);

                // Act
                IActionResult result = await controller.Put(user.Id + 1, user);

                // Assert
                Assert.That(result, Is.InstanceOf<BadRequestResult>());
            }

            [Test]
            public async Task UpdatesUserAndReturnsNoContent()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                User user = fixture.Create<User>();

                UsersController controller = new UsersController(fixture.Freeze<Mock<IUserService>>().Object);

                // Act
                IActionResult result = await controller.Put(user.Id, user);

                // Assert
                Assert.That(result, Is.InstanceOf<NoContentResult>());
            }
        }

        public class Delete : UsersControllerTests
        {
            [Test]
            public async Task DeletesUserAndReturnsNoContent()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                int userId = fixture.Create<int>();

                UsersController controller = new UsersController(fixture.Freeze<Mock<IUserService>>().Object);

                // Act
                IActionResult result = await controller.Delete(userId);

                // Assert
                Assert.That(result, Is.InstanceOf<NoContentResult>());
            }
        }
    }
}