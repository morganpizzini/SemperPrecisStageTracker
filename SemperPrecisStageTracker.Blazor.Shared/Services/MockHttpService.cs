using Microsoft.Extensions.Options;
using SemperPrecisStageTracker.Blazor.Services.Models;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public class MockHttpService : IHttpService
    {
        public Task<ApiResponse<T>> Get<T>(string uri, Dictionary<string, string>? queryParameters = null)
            =>
            uri switch
            {
                string s when s.StartsWith("api/Users/") =>
                    Task.FromResult(new ApiResponse<T>
                    {
                        Result = (T)(object)new BaseResponse<UserContract>(
                                                   new UserContract
                                                   {
                                                       UserId = "1",
                                                       FirstName = "John",
                                                       LastName = "Doe",
                                                       Email = "test@email.com",
                                                       City = "New York",
                                                       PostalCode = "10001",
                                                       BirthDate = DateTime.Now,
                                                       Gender = "M",
                                                       IsActive = true
                                                   })
                    }),
                "api/Users" => Task.FromResult(new ApiResponse<T>
                {
                    Result = (T)(object)new BaseResponse<List<UserContract>>(
                        new List<UserContract> {
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
                        },
                        3,
                        string.Empty)
                }),
                _ => Task.FromResult(new ApiResponse<T>())
            };

        public Task<ApiResponse<T>> Post<T>(string uri, object? value = null)
        {
            return Task.FromResult(new ApiResponse<T>());
        }
    }
}