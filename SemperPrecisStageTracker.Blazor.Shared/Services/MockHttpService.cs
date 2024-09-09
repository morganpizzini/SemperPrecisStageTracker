using SemperPrecisStageTracker.Blazor.Services.Models;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Shared.Utils;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public class MockHttpService : IHttpService
    {
        #region Fields

        private static IList<RoleContract> roles = new List<RoleContract>
        {
            new RoleContract
            {
                RoleId = "1",
                Name = "Admin",
                Description = "Admin",
                Permissions = new List<PermissionContract>
                {
                    new PermissionContract
                    {
                        PermissionId = "1",
                        Name = "ManageUser"
                    },
                    new PermissionContract
                    {
                        PermissionId = "2",
                        Name = "ManagePlaces"
                    }
                },
                UserRoles = new List<UserRoleContract>
                {
                    new UserRoleContract
                    {
                        User = new UserContract
                        {
                            UserId = "1",
                            FirstName = "John",
                            LastName = "Doe",
                            Email = "test@email.com"
                        },
                        UserRoleId = "1"
                    },
                    new UserRoleContract
                    {
                        User = new UserContract
                        {
                            UserId = "2",
                            FirstName = "John",
                            LastName = "Doe 2",
                            Email = "test2@email.com"
                        },
                        UserRoleId = "2",
                        EntityId = "1"
                    }
                }
            },
            new RoleContract
            {
                RoleId = "2",
                Name = "Contributor",
                Description = "Contributor"
            }
        };
        private static IList<PlaceContract> places = new List<PlaceContract> {
            new PlaceContract
            {
                PlaceId="1",
                Name="Place 1",
                Holder="John Doe",
                Phone="1234567890",
                Email="sample@mail.com",
                Address="123 Main St",
                City="New York",
                Region="NY",
                PostalCode="10001",
                Country="USA"
            },
            new PlaceContract
            {
                PlaceId="2",
                Name="Place 2",
                Holder="John Doe",
                Phone="1234567890",
                Email="sample@mail.com",
                Address="123 Main St",
                City="New York",
                Region="NY",
                PostalCode="10001",
                Country="USA"
            },
            new PlaceContract
            {
                PlaceId="3",
                Name="Place 3",
                Holder="John Doe",
                Phone="1234567890",
                Email="sample@mail.com",
                Address="123 Main St",
                City="New York",
                Region="NY",
                PostalCode="10001",
                Country="USA"
            },
            new PlaceContract
            {
                PlaceId="4",
                Name="Place 4",
                Holder="John Doe",
                Phone="1234567890",
                Email="sample@mail.com",
                Address="123 Main St",
                City="New York",
                Region="NY",
                PostalCode="10001",
                Country="USA"
            }
        };
        private static IList<UserContract> users = new List<UserContract> {
            new UserContract
            {
                UserId="1",
                Username= "JohnDoe",
                FirstName="John",
                LastName="Doe",
                Email="test@email.com",
                City="New York",
                PostalCode="10001",
                BirthDate=DateTime.Now,
                Gender = "M",
                IsActive = true
            },
            new UserContract
            {
                UserId="6",
                Username= "JohnDoe1",
                FirstName="John",
                LastName="Doe",
                Email="test@email.com",
                City="New York",
                PostalCode="10001",
                BirthDate=DateTime.Now,
                Gender = "M",
                IsActive = true
            },
            new UserContract
            {
                UserId="4",
                Username= "JohnDoe2",
                FirstName="John",
                LastName="Doe",
                Email="test@email.com",
                City="New York",
                PostalCode="10001",
                BirthDate=DateTime.Now,
                Gender = "M",
                IsActive = true
            },
            new UserContract
            {
                UserId="5",
                Username= "JohnDoe3",
                FirstName="John",
                LastName="Doe",
                Email="test@email.com",
                City="New York",
                PostalCode="10001",
                BirthDate=DateTime.Now,
                Gender = "M",
                IsActive = true
            },
            new UserContract
            {
                UserId="2",
                Username= "JohnDoe4",
                FirstName="John",
                LastName="Doe",
                Email="test@email.com",
                City="New York",
                PostalCode="10001",
                BirthDate=DateTime.Now,
                Gender = "M"
            },
            new UserContract
            {
                UserId="3",
                Username= "JohnDoe5",
                FirstName="John",
                LastName="Doe",
                Email="test@email.com",
                City="New York",
                PostalCode="10001",
                BirthDate=DateTime.Now,
                Gender = "M"
            }
        };
        private static IList<PermissionContract> permissions = new List<PermissionContract>
        {
            new PermissionContract
            {
                PermissionId = "1",
                Name = "SamplePermisison"
            },
            new PermissionContract
            {
                PermissionId = "2",
                Name = "AnotherSamplePermission"
            },
        };
        #endregion
        public Task<ApiResponse<T>> Get<T>(string uri, Dictionary<string, string>? queryParameters = null)
            =>
            uri switch
            {
                string s when s.MatchesRegexPattern("api/Users/.+/Roles") =>
                    Task.FromResult(new ApiResponse<T>
                    {
                        Result = (T)(object)new BaseResponse<List<RoleContract>>(
                            roles.Take(3).ToList(),
                            3,
                            string.Empty)
                    }),
                string s when s.MatchesRegexPattern("api/Users/.+") =>
                    Task.FromResult(new ApiResponse<T>
                    {
                        Result = (T)(object)new BaseResponse<UserContract>(users.First())
                    }),
                string s when s.MatchesRegexPattern("api/Places/.+") =>
                    Task.FromResult(new ApiResponse<T>
                    {
                        Result = (T)(object)new BaseResponse<PlaceContract>(places.First())
                    }),
                "api/Permissions" => Task.FromResult(new ApiResponse<T>
                {
                    Result = (T)(object)new BaseResponse<List<PermissionContract>>(
                            permissions.ToList(),
                            permissions.Count,
                            string.Empty)
                }),
                "api/Users" => Task.FromResult(new ApiResponse<T>
                {
                    Result = (T)(object)new BaseResponse<List<UserContract>>(
                            users
                            .Skip(int.Parse(queryParameters?["skip"] ?? "0"))
                            .Take(int.Parse(queryParameters?["take"] ?? "10"))
                            .ToList(),
                            users.Count,
                            string.Empty)
                }),
                "api/Places" => Task.FromResult(new ApiResponse<T>
                {
                    Result = (T)(object)new BaseResponse<List<PlaceContract>>(
                            places
                            .Skip(int.Parse(queryParameters?["skip"] ?? "0"))
                            .Take(int.Parse(queryParameters?["take"] ?? "10"))
                            .ToList(),
                            users.Count,
                            string.Empty)
                }),
                "api/Roles" => Task.FromResult(new ApiResponse<T>
                {
                    Result = (T)(object)new BaseResponse<List<RoleContract>>(
                            roles
                            .Skip(int.Parse(queryParameters?["skip"] ?? "0"))
                            .Take(int.Parse(queryParameters?["take"] ?? "10"))
                            .ToList(),
                            roles.Count,
                            string.Empty)
                }),

                string s when s.StartsWith("api/Roles/") =>
                    Task.FromResult(new ApiResponse<T>
                    {
                        Result = (T)(object)new BaseResponse<RoleContract>(roles.First())
                    }),
                _ => Task.FromResult(new ApiResponse<T>())
            };

        public Task<ApiResponse<T>> Post<T>(string uri, object? value = null)
        =>
            uri switch
            {
                "/api/Authorization/LogIn" => Task.FromResult(new ApiResponse<T>
                {
                    Result = (T)(object)new SignInResponse
                    {
                        User = users.FirstOrDefault(),
                    }
                }),
                string s when s.MatchesRegexPattern("api/permissions/.+/role/.+") =>
                    Task.FromResult(new ApiResponse<T>
                    {
                        Result = (T)(object)new BaseResponse<PermissionContract>(permissions.First())
                    }),
                _ => Task.FromResult(new ApiResponse<T>())
            };

        public Task<ApiResponse<T>> Put<T>(string uri, object? value = null)
        =>
            uri switch
            {
                _ => Task.FromResult(new ApiResponse<T>())
            };

        public Task<ApiResponse<T>> Patch<T>(string uri, object? value = null)
        =>
            uri switch
            {
                _ => Task.FromResult(new ApiResponse<T>())
            };

        public Task<ApiResponse<T>> Delete<T>(string uri)
        =>
            uri switch
            {
                _ => Task.FromResult(new ApiResponse<T>())
            };
    }
}