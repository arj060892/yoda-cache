using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using Yoda.Cache.API.Models;
using Yoda.Cache.Core.InterfaceAdapters.EvictionStrategies;
using Yoda.Cache.Core.InterfaceAdapters.StorageBackends;
using Yoda.Cache.Core.UseCases;
using Yoda.Cache.Entites;

namespace Yoda.Cache.Core.Tests.UseCases
{
    [TestFixture]
    public class CacheOperationsTests
    {
        protected static IFixture CreateFixture()
        {
            return new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        public class GetAsync : CacheOperationsTests
        {
            [Test]
            public async Task GetUsersBasedOnKeyIfPresenetInCache()
            {
                //Arrange
                IFixture fixture = CreateFixture();

                fixture.Freeze<Mock<IEvictionStrategy<int, User>>>()
                    .Setup(e => e.NotifyAccessAsync(It.IsAny<CacheNode<int, User>>()))
                    .Returns(Task.CompletedTask);

                User user = fixture.Create<User>();

                fixture.Freeze<Mock<IStorageBackend<int, User>>>()
                    .Setup(s => s.GetAsync(user.Id))
                    .ReturnsAsync(user);

                // Act
                CacheOperations<int, User> cache = fixture.Create<CacheOperations<int, User>>();
                User retrievedUser = await cache.GetAsync(user.Id);

                // Assert
                Assert.That(retrievedUser, Is.EqualTo(user));
            }

            [Test]
            public async Task ShouldNotifyEvictionModelOfRecentAccessOfNodeIfItemPresentInCache()
            {
                //Arrange
                IFixture fixture = CreateFixture();

                Mock<IEvictionStrategy<int, User>> mockEviction = fixture.Freeze<Mock<IEvictionStrategy<int, User>>>();

                mockEviction.Setup(e => e.NotifyAccessAsync(It.IsAny<CacheNode<int, User>>()))
                    .Returns(Task.CompletedTask);

                User user = fixture.Create<User>();

                fixture.Freeze<Mock<IStorageBackend<int, User>>>()
                    .Setup(s => s.GetAsync(user.Id))
                    .ReturnsAsync(user);

                // Act
                CacheOperations<int, User> cache = fixture.Create<CacheOperations<int, User>>();
                User retrievedUser = await cache.GetAsync(user.Id);

                // Assert
                mockEviction.Verify(e => e.NotifyAccessAsync(It.IsAny<CacheNode<int, User>>()), Times.Once());
            }

            [Test]
            public async Task ReturnNullIfNotPresenetInCache()
            {
                //Arrange
                IFixture fixture = CreateFixture();

                User user = fixture.Create<User>();

                fixture.Freeze<Mock<IStorageBackend<int, User>>>()
                    .Setup(s => s.GetAsync(user.Id))
                    .ReturnsAsync((User)null);

                // Act
                CacheOperations<int, User> cache = fixture.Create<CacheOperations<int, User>>();
                User retrievedUser = await cache.GetAsync(user.Id);

                // Assert
                Assert.That(retrievedUser, Is.EqualTo(null));
            }
        }

        public class PutAsync : CacheOperationsTests
        {
            [Test]
            public async Task AddItemToCacheWhenNotFull()
            {
                //Arrange
                IFixture fixture = CreateFixture();

                User user = fixture.Create<User>();

                fixture.Freeze<Mock<IStorageBackend<int, User>>>()
                    .Setup(s => s.GetSizeAsync())
                    .ReturnsAsync(0);

                fixture.Freeze<Mock<IStorageBackend<int, User>>>()
                    .Setup(s => s.PutAsync(user.Id, user))
                    .Returns(Task.CompletedTask);

                // Act
                CacheOperations<int, User> cache = fixture.Create<CacheOperations<int, User>>();
                await cache.PutAsync(user.Id, user);

                // Assert
                // No assertion needed as we're just testing if it runs without exceptions.
            }

            [Test]
            public async Task EvictItemWhenCacheIsFull()
            {
                //Arrange
                IFixture fixture = CreateFixture();

                User userToEvict = fixture.Create<User>();
                User userToAdd = fixture.Create<User>();

                fixture.Freeze<Mock<IStorageBackend<int, User>>>()
                    .Setup(s => s.GetSizeAsync())
                    .ReturnsAsync(2);

                fixture.Freeze<Mock<IEvictionStrategy<int, User>>>()
                    .Setup(e => e.GetNodeToEvictAsync())
                    .ReturnsAsync(new CacheNode<int, User>(userToEvict.Id, userToEvict));

                fixture.Customize<CacheOperations<int, User>>(composer => composer
                    .FromFactory((IEvictionStrategy<int, User> evictionStrategy, IStorageBackend<int, User> storageBackend) =>
                        new CacheOperations<int, User>(2, evictionStrategy, storageBackend)));

                // Act
                CacheOperations<int, User> cache = fixture.Create<CacheOperations<int, User>>();
                await cache.PutAsync(userToAdd.Id, userToAdd);

                // Assert
                fixture.Freeze<Mock<IStorageBackend<int, User>>>()
                    .Verify(s => s.RemoveAsync(userToEvict.Id), Times.Once());
            }

            [Test]
            public async Task RaiseItemEvictedEventWhenAnItemIsEvicted()
            {
                // Arrange
                IFixture fixture = CreateFixture();

                User userToEvict = fixture.Create<User>();
                User userToAdd = fixture.Create<User>();

                fixture.Freeze<Mock<IStorageBackend<int, User>>>()
                    .Setup(s => s.GetSizeAsync())
                    .ReturnsAsync(2);

                fixture.Freeze<Mock<IEvictionStrategy<int, User>>>()
                    .Setup(e => e.GetNodeToEvictAsync())
                    .ReturnsAsync(new CacheNode<int, User>(userToEvict.Id, userToEvict));

                fixture.Customize<CacheOperations<int, User>>(composer => composer
                    .FromFactory((IEvictionStrategy<int, User> evictionStrategy, IStorageBackend<int, User> storageBackend) =>
                        new CacheOperations<int, User>(2, evictionStrategy, storageBackend)));

                CacheOperations<int, User> cache = fixture.Create<CacheOperations<int, User>>();

                bool eventWasRaised = false;
                cache.ItemEvicted += (sender, args) =>
                {
                    eventWasRaised = true;
                    Assert.Multiple(() =>
                    {
                        Assert.That(args.Key, Is.EqualTo(userToEvict.Id));
                        Assert.That(args.Value, Is.EqualTo(userToEvict));
                    });
                };

                // Act
                await cache.PutAsync(userToAdd.Id, userToAdd);

                // Assert
                Assert.That(eventWasRaised, Is.True);
            }


            [Test]
            public async Task NotifyEvictionModelOfNewItemAdded()
            {
                //Arrange
                IFixture fixture = CreateFixture();

                User user = fixture.Create<User>();

                Mock<IEvictionStrategy<int, User>> mockEviction = fixture.Freeze<Mock<IEvictionStrategy<int, User>>>();

                mockEviction.Setup(e => e.NotifyAdditionAsync(It.IsAny<CacheNode<int, User>>()))
                    .Returns(Task.CompletedTask);

                // Act
                CacheOperations<int, User> cache = fixture.Create<CacheOperations<int, User>>();
                await cache.PutAsync(user.Id, user);

                // Assert
                mockEviction.Verify(e => e.NotifyAdditionAsync(It.IsAny<CacheNode<int, User>>()), Times.Once());
            }

        }
    }
}