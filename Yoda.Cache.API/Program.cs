using System.Reflection;
using Yoda.Cache.API.Helpers;
using Yoda.Cache.API.Models;
using Yoda.Cache.API.Services;
using Yoda.Cache.Core.InterfaceAdapters.EvictionStrategies;
using Yoda.Cache.Core.InterfaceAdapters.StorageBackends;
using Yoda.Cache.Core.UseCases;
using Yoda.Cache.Entites;

namespace Yoda.Cache.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            RegisterDependencies(builder);

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void RegisterDependencies(WebApplicationBuilder builder)
        {
            // Get all types in the current assembly that implement ICachableEntity
            IEnumerable<Type> cachableEntityTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(ICachableEntity).IsAssignableFrom(t) && !t.IsInterface);

            foreach (Type type in cachableEntityTypes)
            {
                // Fetch the CacheKeyTypeAttribute from the entity
                CacheKeyTypeAttribute attribute = (CacheKeyTypeAttribute)type.GetCustomAttribute(typeof(CacheKeyTypeAttribute));

                // Determine the key type. If the attribute isn't found, default to string
                Type keyType = attribute?.KeyType ?? typeof(string);

                // Register the storage backend for this type
                Type storageBackendInterfaceType = typeof(IStorageBackend<,>).MakeGenericType(keyType, type);
                Type storageBackendImplementationType = typeof(InMemoryStorageBackend<,>).MakeGenericType(keyType, type);
                builder.Services.AddSingleton(storageBackendInterfaceType, storageBackendImplementationType);

                // Register the eviction strategy for this type
                Type evictionStrategyInterfaceType = typeof(IEvictionStrategy<,>).MakeGenericType(keyType, type);
                Type evictionStrategyImplementationType = typeof(LRUEvictionStrategy<,>).MakeGenericType(keyType, type);
                builder.Services.AddSingleton(evictionStrategyInterfaceType, evictionStrategyImplementationType);

                // Register the cache for this type
                Type cacheInterfaceType = typeof(ICache<,>).MakeGenericType(keyType, type);
                Type cacheImplementationType = typeof(CacheOperations<,>).MakeGenericType(keyType, type);
                builder.Services.AddSingleton(cacheInterfaceType, serviceProvider => Activator.CreateInstance(
                    cacheImplementationType,
                    100, // can be fetched from config
                    serviceProvider.GetRequiredService(evictionStrategyInterfaceType),
                    serviceProvider.GetRequiredService(storageBackendInterfaceType)));
            }

            builder.Services.AddTransient<IUserService, UserService>();
        }
    }
}