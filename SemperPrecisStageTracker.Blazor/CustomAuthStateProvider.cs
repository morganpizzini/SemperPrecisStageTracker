using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using SemperPrecisStageTracker.Contracts;

namespace SemperPrecisStageTracker.Blazor
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

        public void LoginNotify(ShooterContract user)
        {
            if (user == null)
            {
                LogoutNotify();
                return;
            }

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
            }, "Fake authentication type");

            claimsPrincipal = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void LogoutNotify()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            claimsPrincipal = anonymous;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(claimsPrincipal));
        }
    }
}