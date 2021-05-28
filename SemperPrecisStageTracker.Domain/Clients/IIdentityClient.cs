using SemperPrecisStageTracker.Models;
using System.Threading.Tasks;
using ZenProgramming.Chakra.Core.Http;

namespace SemperPrecisStageTracker.Domain.Clients
{
    /// <summary>
    /// Client for interact with identity service
    /// </summary>
    public interface IIdentityClient
    {
        /// <summary>
        /// Executes sign-in on identity service
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        /// <returns>Returns user contract if sign-in in completed</returns>
        Task<HttpResponseMessage<Shooter>> SignIn(string userName, string password);

        /// <summary>
        /// Executes sign-on on identity service
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Returns user contract if sign-on in completed</returns>
        Task<HttpResponseMessage<Shooter>> ValidateSignUp(Shooter user);

        /// <summary>
        /// Count valid users
        /// </summary>
        /// <returns>Returns task with value</returns>
        Task<HttpResponseMessage<IntResponse>> CountUsers();

        /// <summary>
        /// Get user contract using username
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>Returns task with value</returns>
        Task<HttpResponseMessage<Shooter>> GetUserByUserName(string userName);

        /// <summary>
        /// Encrypt password
        /// </summary>
        /// <param name="password">Password</param>
        /// <returns>Returns task with value</returns>
        string EncryptPassword(string password);
    }
}
