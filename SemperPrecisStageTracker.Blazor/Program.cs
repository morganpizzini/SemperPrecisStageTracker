using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using DnetIndexedDb;
using DnetIndexedDb.Models;
using Microsoft.Extensions.Localization;
using SemperPrecisStageTracker.Blazor.Utils;
using SemperPrecisStageTracker.Blazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using SemperPrecisStageTracker.Contracts;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using SemperPrecisStageTracker.Blazor.Models;
using SemperPrecisStageTracker.Blazor.Services.IndexDB;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;

namespace SemperPrecisStageTracker.Blazor
{
    public class PermissionHandler : AuthorizationHandler<RolesAuthorizationRequirement>
    {
        private readonly IAuthenticationService _authService;
        private readonly ILogger<PermissionHandler> _logger;
        public PermissionHandler(IAuthenticationService authService, ILogger<PermissionHandler> logger)
        {
            _logger = logger;
            _authService = authService;
        }
        

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       RolesAuthorizationRequirement requirement)
        {
            
            if (context.User.Identity is {IsAuthenticated: false})
            {
                context.Fail();
                return Task.CompletedTask;
            }

            if (!requirement.AllowedRoles.Any())
            {
                // it means any logged in user is allowed to access the resource    
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var roles = requirement.AllowedRoles;
            
            // get profile id from resource, passed in from blazor 
            //  page component
            var resourceId = context.Resource?.ToString() ?? string.Empty;
            
            if (_authService.CheckPermissions(roles, resourceId))
            {
                context.Succeed(requirement);
                _logger.LogInformation("OOOKKK");
                return Task.CompletedTask;
            }

            _logger.LogInformation("NOOOO");

            context.Fail();
            return Task.CompletedTask;
        }
    }


    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                    options.ValidationMessageLocalizer = (message, arguments) =>
                    {
                        var stringLocalizer = options.Services.GetService<IStringLocalizer<Program>>();

                        return stringLocalizer != null && arguments?.Count() > 0
                            ? string.Format(stringLocalizer[message], arguments.ToArray())
                            : message;
                    };
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>()
                            .AddScoped<StateService>()
                            .AddScoped<IAuthorizationHandler, PermissionHandler>();



            builder.Services
                .AddScoped<IHttpService, HttpService>()
                .AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<ILocalStorageService, LocalStorageService>()
                .AddScoped<NetworkService>()
                .AddScoped<MainServiceLayer>();

            //builder.Services.AddScoped(x =>
            //{
            //    Console.WriteLine("TestHttpClient");
            //    var apiUrl = new Uri(builder.Configuration["baseAddress"]);
            //    // use fake backend if "fakeBackend" is "true" in appsettings.json
            //    // if (builder.Configuration["fakeBackend"] == "true")
            //    //     return new HttpClient(new FakeBackendHandler()) { BaseAddress = apiUrl };

            //    return new HttpClient() { BaseAddress = apiUrl };
            //});

            // load configuration from server

            var apiUrl = new Uri(builder.Configuration["baseAddress"]);

            // use fake backend if "fakeBackend" is "true" in appsettings.json
            // if (builder.Configuration["fakeBackend"] == "true")
            //     return new HttpClient(new FakeBackendHandler()) { BaseAddress = apiUrl };

            var httpClient = new HttpClient() { BaseAddress = apiUrl };

            //configure http client
            builder.Services.AddScoped(x => httpClient);

            using var serverConfig = new HttpRequestMessage(HttpMethod.Get, "api/config/GetConfig");
            using var responseConfig = await httpClient.SendAsync(serverConfig);

            await using var stream = await responseConfig.Content.ReadAsStreamAsync();

            builder.Configuration.AddJsonStream(stream);

            builder.Services.AddScoped(sp =>
            {
                var config = sp.GetService<IConfiguration>();
                var result = new ClientConfiguration();
                config.Bind(result);

                return result;
            });

            // index db
            builder.Services.AddIndexedDbDatabase<MatchServiceIndexedDb>(options =>
            {
                var indexedDbDatabaseModel = new IndexedDbDatabaseModel()
                    .WithName("OfflineContent")
                    .WithVersion(1);

                indexedDbDatabaseModel.AddStore<MatchContract>();
                indexedDbDatabaseModel.AddStore<ShooterContract>();
                indexedDbDatabaseModel.AddStore<StageContract>();
                indexedDbDatabaseModel.AddStore<GroupContract>();
                indexedDbDatabaseModel.AddStore<ShooterStageAggregationResult>();
                indexedDbDatabaseModel.AddStore<ShooterMatchContract>();
                indexedDbDatabaseModel.AddStore<ShooterSOStageContract>();
                indexedDbDatabaseModel.AddStore<EditedEntity>();

                // add offline settings
                // clean/download all method
                // services for avoid api call and use indexDB
                // services for push all
                // api services for update edited entities

                options.UseDatabase(indexedDbDatabaseModel);
            });


            builder.RootComponents.Add<App>("#app");
            var host = builder.Build();

            // init network service
            var network = host.Services.GetRequiredService<NetworkService>();
            await network.InitAsync();

            //init auth service
            var authenticationService = host.Services.GetRequiredService<IAuthenticationService>();
            await authenticationService.Initialize();

            await host.SetDefaultCulture();
            await host.RunAsync();
        }
    }

    public static class IndexedDbDatabaseExtensions
    {
        /// <summary>
        /// Sets Name of Database Model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IndexedDbDatabaseModel WithName(this IndexedDbDatabaseModel model, string name)
        {
            model.Name = name;
            return model;
        }

        /// <summary>
        /// Sets Version of Database Model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static IndexedDbDatabaseModel WithVersion(this IndexedDbDatabaseModel model, int version)
        {
            model.Version = version;
            return model;
        }

        /// <summary>
        /// Sets Database ModelId. Defaults to 0
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IndexedDbDatabaseModel WithModelId(this IndexedDbDatabaseModel model, int id)
        {
            model.DbModelId = id;
            return model;
        }

        /// <summary>
        /// Sets Database UseKeyGenerator Property to true
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IndexedDbDatabaseModel UseKeyGenerator(this IndexedDbDatabaseModel model)
        {
            model.UseKeyGenerator = true;
            return model;
        }

        /// <summary>
        /// Sets Database UseKeyGenerator Property to given value
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IndexedDbDatabaseModel UseKeyGenerator(this IndexedDbDatabaseModel model, bool value)
        {
            model.UseKeyGenerator = value;
            return model;
        }

        /// <summary>
        /// Creates a new IndexedDbStore and adds it to the Database Model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IndexedDbStore AddStore(this IndexedDbDatabaseModel model, string name)
        {
            model.Stores ??= new List<IndexedDbStore>();

            var store = new IndexedDbStore { Name = name };

            model.Stores.Add(store);

            return store;
        }

        /// <summary>
        /// Adds a new store named TEntity.Name and adds Key and Indexes based on IndexedDbKey and IndexedDbIndex Attributes
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IndexedDbStore AddStore<TEntity>(this IndexedDbDatabaseModel model)
        {
            model.Stores ??= new List<IndexedDbStore>();

            var store = new IndexedDbStore { Name = typeof(TEntity).Name };

            store.SetupFrom<TEntity>();

            model.Stores.Add(store);

            return store;
        }

        /// <summary>
        /// Adds a new store and adds Key and Indexes based on IndexedDbKey and IndexedDbIndex Attributes
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <param name="storeName"></param>
        /// <returns></returns>
        public static IndexedDbStore AddStore<TEntity>(this IndexedDbDatabaseModel model, string storeName)
        {
            model.Stores ??= new List<IndexedDbStore>();

            var store = new IndexedDbStore { Name = storeName };

            store.SetupFrom<TEntity>();

            model.Stores.Add(store);

            return store;
        }
    }

    public static class IndexedDbInteropExtensions
    {
        /// <summary>
        /// Add records to a given data store
        /// </summary>
        /// <typeparam name="TEntity">Type of Objects in Data Store</typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static ValueTask<string> AddItems<TEntity>(this IndexedDbInterop idi, List<TEntity> items)
        {
            return idi.AddItems(typeof(TEntity).Name, items);
        }

        public static ValueTask<string> UpdateItems<TEntity>(this IndexedDbInterop idi, List<TEntity> items)
        {
            return idi.UpdateItems(typeof(TEntity).Name, items);
        }
        public static ValueTask<TEntity> GetByKey<TKey, TEntity>(this IndexedDbInterop idi, TKey key)
        {
            return idi.GetByKey<TKey, TEntity>(typeof(TEntity).Name, key);
        }
        public static ValueTask<string> DeleteByKey<TKey, TEntity>(this IndexedDbInterop idi, TKey key)
        {
            return idi.DeleteByKey(typeof(TEntity).Name, key);
        }
        public static ValueTask<string> DeleteAll<TEntity>(this IndexedDbInterop idi)
        {
            return idi.DeleteAll(typeof(TEntity).Name);
        }
        public static ValueTask<List<TEntity>> GetAll<TEntity>(this IndexedDbInterop idi)
        {
            return idi.GetAll<TEntity>(typeof(TEntity).Name);
        }
        public static ValueTask<List<TEntity>> GetRange<TKey, TEntity>(this IndexedDbInterop idi, TKey lowerBound, TKey upperBound)
        {
            return idi.GetRange<TKey, TEntity>(typeof(TEntity).Name, lowerBound, upperBound);
        }
        public static ValueTask<List<TEntity>> GetByIndex<TKey, TEntity>(this IndexedDbInterop idi, TKey lowerBound, TKey upperBound, string dbIndex, bool isRange)
        {
            return idi.GetByIndex<TKey, TEntity>(typeof(TEntity).Name, lowerBound, upperBound, dbIndex, isRange);
        }
        public static ValueTask<TIndex> GetMaxIndex<TIndex, TEntity>(this IndexedDbInterop idi, string dbIndex)
        {
            return idi.GetMaxIndex<TIndex>(typeof(TEntity).Name, dbIndex);
        }
        public static ValueTask<TKey> GetMaxKey<TKey, TEntity>(this IndexedDbInterop idi)
        {
            return idi.GetMaxKey<TKey>(typeof(TEntity).Name);
        }
        public static ValueTask<TIndex> GetMinIndex<TIndex, TEntity>(this IndexedDbInterop idi, string dbIndex)
        {
            return idi.GetMinIndex<TIndex>(typeof(TEntity).Name, dbIndex);
        }
        public static ValueTask<TKey> GetMinKey<TKey, TEntity>(this IndexedDbInterop idi, TKey key)
        {
            return idi.GetMinKey<TKey>(typeof(TEntity).Name);
        }
    }
    public static class IndexedDbStoreExtensions
    {

        // Key Creation

        /// <summary>
        /// Add non-auto-incrementing key to store.
        /// </summary>
        /// <param name="store">IndexedDb Store</param>
        /// <param name="name"></param>
        /// <returns>Given IndexedDbStore Instance</returns>
        public static IndexedDbStore WithKey(this IndexedDbStore store, string name)
        {
            return store.CreateKey(name, autoIncrement: false);
        }

        /// <summary>
        /// Add auto-incrementing key to store.
        /// </summary>
        /// <param name="store">IndexedDb Store</param>
        /// <param name="name"></param>
        /// <returns>Given IndexedDbStore Instance</returns>
        public static IndexedDbStore WithAutoIncrementingKey(this IndexedDbStore store, string name)
        {
            return store.CreateKey(name, autoIncrement: true);
        }

        private static IndexedDbStore CreateKey(this IndexedDbStore store, string name, bool autoIncrement)
        {
            store.Key = new IndexedDbStoreParameter
            {
                KeyPath = name.ToCamelCase(),
                AutoIncrement = autoIncrement
            };

            return store;
        }

        // Index Creation

        /// <summary>
        /// Add unique index to store.
        /// </summary>
        /// <param name="store">IndexedDb Store</param>
        /// <param name="name"></param>
        /// <returns>Given IndexedDbStore Instance</returns>
        public static IndexedDbStore AddUniqueIndex(this IndexedDbStore store, string name)
        {
            return store.CreateIndex(name, new IndexedDbIndexParameter() { Unique = true });
        }

        /// <summary>
        /// Add non-unique index to store.
        /// </summary>
        /// <param name="store">IndexedDb Store</param>
        /// <param name="name"></param>
        /// <returns>Given IndexedDbStore Instance</returns>
        public static IndexedDbStore AddIndex(this IndexedDbStore store, string name)
        {
            return store.CreateIndex(name, new IndexedDbIndexParameter() { Unique = false });
        }

        private static IndexedDbStore CreateIndex(this IndexedDbStore store, string name, IndexedDbIndexParameter definition)
        {
            store.Indexes ??= new List<IndexedDbIndex>();

            var index = new IndexedDbIndex
            {
                Name = DnetIndexedDb.Fluent.IndexedDbStoreExtensions.ToCamelCase(name),
                Definition = definition
            };

            store.Indexes.Add(index);

            return store;
        }

        private static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }

        /// <summary>
        /// Adds Key and Indexes to Store based on IndexedDbKey and IndexedDbIndex Attributes on properties in Type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="store"></param>
        /// <returns></returns>
        public static IndexedDbStore SetupFrom<T>(this IndexedDbStore store)
        {
            store.Indexes = null;

            var type = typeof(T);
            var props = type.GetProperties();

            foreach (var prop in props)
            {
                var keyAttribute = prop.GetCustomAttribute<IndexDbKeyAttribute>();
                var indexAttribute = prop.GetCustomAttribute<IndexDbIndexAttribute>();

                if (keyAttribute is not null)
                {
                    store.CreateKey(prop.Name, keyAttribute.AutoIncrement);
                    store.CreateIndex(prop.Name, new IndexedDbIndexParameter() { Unique = keyAttribute.Unique });
                }

                if (indexAttribute is not null)
                {
                    store.CreateIndex(prop.Name, new IndexedDbIndexParameter() { Unique = indexAttribute.Unique });
                }
            }

            if (store.Key == null)
            {
                throw new System.Exception($"No IndexDbKey Found on Property Attributes in Class {type.Name}");
            }

            return store;
        }
    }
}
