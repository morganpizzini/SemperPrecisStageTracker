using Microsoft.Extensions.Options;
using SemperPrecisStageTracker.Blazor.Services.Models;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using System.Text.RegularExpressions;

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
        private static IList<UserContract> users = new List<UserContract> {
                            new UserContract
                            {
                                UserId="1",
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
                                FirstName="John",
                                LastName="Doe",
                                Email="test@email.com",
                                City="New York",
                                PostalCode="10001",
                                BirthDate=DateTime.Now,
                                Gender = "M"
                            }
                        };
        #endregion
        public Task<ApiResponse<T>> Get<T>(string uri, Dictionary<string, string>? queryParameters = null)
            =>
            uri switch
            {
                string s when s.StartsWith("api/Users/") =>
                    Task.FromResult(new ApiResponse<T>
                    {
                        Result = (T)(object)new BaseResponse<UserContract>(users.First())
                    }),
                "api/Users" => Task.FromResult(new ApiResponse<T>
                {
                    Result = (T)(object)new BaseResponse<List<UserContract>>(
                            users
                            .Skip(int.Parse(queryParameters?["skip"] ?? "10"))
                            .Take(int.Parse(queryParameters?["take"] ?? "0"))
                            .ToList(),
                            users.Count,
                            string.Empty)
                }),
                "api/Roles" => Task.FromResult(new ApiResponse<T>
                {
                    Result = (T)(object)new BaseResponse<List<RoleContract>>(
                            roles
                            .Skip(int.Parse(queryParameters?["skip"] ?? "10"))
                            .Take(int.Parse(queryParameters?["take"] ?? "0"))
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