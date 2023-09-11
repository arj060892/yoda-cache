using AutoFixture;
using AutoFixture.AutoMoq;
using Yoda.Cache.API.Models;
using Yoda.Cache.Core.InterfaceAdapters.EvictionStrategies;
using Yoda.Cache.Entites;

namespace Yoda.Cache.Core.Tests.InterfaceAdapters.EvictionStrategies
{
    [TestFixture]
    public class LRUEvictionStrategyTests
    {
        protected static IFixture CreateFixture()
        {
            return new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        public class GetNodeToEvictAsync : LRUEvictionStrategyTests
        {
            [Test]
            public async Task ReturnsNullWhenNoNodes()
            {
                // Arrange
                LRUEvictionStrategy<int, User> strategy = new LRUEvictionStrategy<int, User>();

                // Act
                CacheNode<int, User> nodeToEvict = await strategy.GetNodeToEvictAsync();

                // Assert
                Assert.That(nodeToEvict, Is.Null);
            }

            [Test]
            public async Task ReturnsLeastRecentlyUsedNode()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                LRUEvictionStrategy<int, User> strategy = new LRUEvictionStrategy<int, User>();
                CacheNode<int, User> node1 = new CacheNode<int, User>(fixture.Create<int>(), fixture.Create<User>());
                CacheNode<int, User> node2 = new CacheNode<int, User>(fixture.Create<int>(), fixture.Create<User>());
                await strategy.NotifyAdditionAsync(node1);
                await strategy.NotifyAdditionAsync(node2);

                // Act
                CacheNode<int, User> nodeToEvict = await strategy.GetNodeToEvictAsync();

                // Assert
                Assert.That(nodeToEvict, Is.EqualTo(node1));
            }
        }

        public class NotifyAccessAsync : LRUEvictionStrategyTests
        {
            [Test]
            public async Task MovesNodeToFront()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                LRUEvictionStrategy<int, User> strategy = new LRUEvictionStrategy<int, User>();
                CacheNode<int, User> node1 = new CacheNode<int, User>(fixture.Create<int>(), fixture.Create<User>());
                CacheNode<int, User> node2 = new CacheNode<int, User>(fixture.Create<int>(), fixture.Create<User>());
                await strategy.NotifyAdditionAsync(node1);
                await strategy.NotifyAdditionAsync(node2);
                await strategy.NotifyAccessAsync(node1);

                // Act
                CacheNode<int, User> nodeToEvict = await strategy.GetNodeToEvictAsync();

                // Assert
                Assert.That(nodeToEvict, Is.EqualTo(node2));
            }
        }

        public class NotifyAdditionAsync : LRUEvictionStrategyTests
        {
            [Test]
            public async Task AddsNodeToFront()
            {
                // Arrange
                IFixture fixture = CreateFixture();
                LRUEvictionStrategy<int, User> strategy = new LRUEvictionStrategy<int, User>();
                CacheNode<int, User> node1 = new CacheNode<int, User>(fixture.Create<int>(), fixture.Create<User>());
                await strategy.NotifyAdditionAsync(node1);

                // Act
                CacheNode<int, User> nodeToEvict = await strategy.GetNodeToEvictAsync();

                // Assert
                Assert.That(nodeToEvict, Is.EqualTo(node1));
            }
        }
    }
}