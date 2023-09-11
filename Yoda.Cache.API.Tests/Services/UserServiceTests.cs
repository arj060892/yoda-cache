using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using NUnit.Framework;
using Yoda.Cache.API.Models;
using Yoda.Cache.API.Services;
using Yoda.Cache.Entites;

namespace Yoda.Cache.API.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        protected static IFixture CreateFixture()
        {
            return new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        public class GetUserByIdAsync : UserServiceTests
        {
            [Test]
            public async Task ReturnsUserFromCacheIfExists()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                Mock<ICache<int, User>> mockCache = fixture.Freeze<Mock<ICache<int, User>>>();
                User user = fixture.Create<User>();
                mockCache.Setup(c => c.GetAsync(user.Id)).ReturnsAsync(user);

                UserService service = new UserService(mockCache.Object);

                // Act
                User retrievedUser = await service.GetUserByIdAsync(user.Id);

                // Assert
                Assert.That(retrievedUser, Is.EqualTo(user));
            }

            [Test]
            public async Task ReturnsUserFromListAndAddsToCacheIfNotInCache()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                Mock<ICache<int, User>> mockCache = fixture.Freeze<Mock<ICache<int, User>>>();
                User user = new User { Id = 1, Name = "User 1" };
                mockCache.Setup(c => c.GetAsync(user.Id)).ReturnsAsync((User)null);

                UserService service = new UserService(mockCache.Object);

                // Act
                User retrievedUser = await service.GetUserByIdAsync(user.Id);

                // Assert
                Assert.That(retrievedUser.Id, Is.EqualTo(user.Id));
                mockCache.Verify(c => c.PutAsync(retrievedUser.Id, retrievedUser), Times.Once);
            }
        }

        public class AddUserAsync : UserServiceTests
        {
            [Test]
            public async Task AddsUserToListAndCache()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                Mock<ICache<int, User>> mockCache = fixture.Freeze<Mock<ICache<int, User>>>();
                User user = fixture.Create<User>();

                UserService service = new UserService(mockCache.Object);

                // Act
                await service.AddUserAsync(user);

                // Assert
                mockCache.Verify(c => c.PutAsync(user.Id, user), Times.Once);
            }
        }

        public class UpdateUserAsync : UserServiceTests
        {
            [Test]
            public async Task UpdatesUserInListAndCache()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                Mock<ICache<int, User>> mockCache = fixture.Freeze<Mock<ICache<int, User>>>();
                User user = new User { Id = 1, Name = "Updated Name" };

                UserService service = new UserService(mockCache.Object);

                // Act
                await service.UpdateUserAsync(user);

                // Assert
                mockCache.Verify(c => c.PutAsync(user.Id, user), Times.Once);
            }
        }

        public class DeleteUserAsync : UserServiceTests
        {
            [Test]
            public async Task RemovesUserFromList()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                Mock<ICache<int, User>> mockCache = fixture.Freeze<Mock<ICache<int, User>>>();
                int userId = 1;

                UserService service = new UserService(mockCache.Object);

                // Act
                await service.DeleteUserAsync(userId);

                // Assert
                User user = await service.GetUserByIdAsync(userId);
                Assert.That(user, Is.Null);
            }
        }
    }
}