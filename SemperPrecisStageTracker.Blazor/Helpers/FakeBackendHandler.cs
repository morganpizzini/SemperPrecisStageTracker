using System.Net;
using System.Text;
using System.Text.Json;

namespace SemperPrecisStageTracker.Blazor.Helpers
{
    public class FakeBackendHandler : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var users = new[] { new { Id = 1, Username = "test", Password = "test", FirstName = "Test", LastName = "User" } };
            var path = request.RequestUri?.AbsolutePath;
            var method = request.Method;

            if (path == "/users/authenticate" && method == HttpMethod.Post)
            {
                return await authenticate();
            }
            else if (path == "/users" && method == HttpMethod.Get)
            {
                return await getUsers();
            }
            else
            {
                // pass through any requests not handled above
                return await base.SendAsync(request, cancellationToken);
            }

            // route functions

            async Task<HttpResponseMessage> authenticate()
            {
                var bodyJson = request.Content != null ? await request.Content.ReadAsStringAsync() : string.Empty;

                var body = JsonSerializer.Deserialize<Dictionary<string, string>>(bodyJson);
                
                if(body == null)
                    return await error("Request failed");

                var user = users.FirstOrDefault(x => x.Username == body["username"] && x.Password == body["password"]);

                if (user == null)
                    return await error("Username or password is incorrect");

                return await ok(new
                {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                });
            }

            async Task<HttpResponseMessage> getUsers()
            {
                if (!isLoggedIn()) return await unauthorized();
                return await ok(users);
            }

            // helper functions

            async Task<HttpResponseMessage> ok(object body)
            {
                return await jsonResponse(HttpStatusCode.OK, body);
            }

            async Task<HttpResponseMessage> error(string message)
            {
                return await jsonResponse(HttpStatusCode.BadRequest, new { message });
            }

            async Task<HttpResponseMessage> unauthorized()
            {
                return await jsonResponse(HttpStatusCode.Unauthorized, new { message = "Unauthorized" });
            }

            async Task<HttpResponseMessage> jsonResponse(HttpStatusCode statusCode, object content)
            {
                var response = new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json")
                };

                // delay to simulate real api call
                await Task.Delay(500,CancellationToken.None);

                return response;
            }

            bool isLoggedIn()
            {
                return request.Headers.Authorization?.Parameter == "test:test".EncodeBase64();
            }
        }
    }
}