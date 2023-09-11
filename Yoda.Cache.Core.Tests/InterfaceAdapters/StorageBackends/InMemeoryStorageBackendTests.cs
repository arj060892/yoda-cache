using AutoFixture;
using AutoFixture.AutoMoq;
using Yoda.Cache.API.Models;
using Yoda.Cache.Core.InterfaceAdapters.StorageBackends;

namespace Yoda.Cache.Core.Tests.InterfaceAdapters.StorageBackends
{
    [TestFixture]
    public class InMemoryStorageBackendTests
    {
        protected static IFixture CreateFixture()
        {
            return new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        public class GetAsync : InMemoryStorageBackendTests
        {
            [Test]
            public async Task RetrieveExistingItemSuccessfully()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                int key = fixture.Create<int>();
                User user = fixture.Create<User>();
                InMemoryStorageBackend<int, User> storage = new InMemoryStorageBackend<int, User>();
                await storage.PutAsync(key, user);

                // Act
                User retrievedUser = await storage.GetAsync(key);

                // Assert
                Assert.That(retrievedUser, Is.EqualTo(user));
            }

            [Test]
            public async Task RetrieveNonExistentItemReturnsDefault()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                int key = fixture.Create<int>();
                InMemoryStorageBackend<int, User> storage = new InMemoryStorageBackend<int, User>();

                // Act
                User retrievedUser = await storage.GetAsync(key);

                // Assert
                Assert.That(retrievedUser, Is.EqualTo(default(User)));
            }
        }

        public class PutAsync : InMemoryStorageBackendTests
        {
            [Test]
            public async Task AddAndRetrieveItemSuccessfully()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                int key = fixture.Create<int>();
                User user = fixture.Create<User>();
                InMemoryStorageBackend<int, User> storage = new InMemoryStorageBackend<int, User>();

                // Act
                await storage.PutAsync(key, user);
                User retrievedUser = await storage.GetAsync(key);

                // Assert
                Assert.That(retrievedUser, Is.EqualTo(user));
            }
        }

        public class RemoveAsync : InMemoryStorageBackendTests
        {
            [Test]
            public async Task RemoveItemMakesItUnreachable()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                int key = fixture.Create<int>();
                User user = fixture.Create<User>();
                InMemoryStorageBackend<int, User> storage = new InMemoryStorageBackend<int, User>();
                await storage.PutAsync(key, user);

                // Act
                await storage.RemoveAsync(key);
                User retrievedUser = await storage.GetAsync(key);

                // Assert
                Assert.That(retrievedUser, Is.EqualTo(default(User)));
            }
        }

        public class GetSizeAsync : InMemoryStorageBackendTests
        {
            [Test]
            public async Task GetSizeReflectsNumberOfItems()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                int key1 = fixture.Create<int>();
                User user1 = fixture.Create<User>();
                int key2 = fixture.Create<int>();
                User user2 = fixture.Create<User>();
                InMemoryStorageBackend<int, User> storage = new InMemoryStorageBackend<int, User>();

                // Act
                await storage.PutAsync(key1, user1);
                await storage.PutAsync(key2, user2);
                int size = await storage.GetSizeAsync();

                // Assert
                Assert.That(size, Is.EqualTo(2));
            }
        }
    }
}