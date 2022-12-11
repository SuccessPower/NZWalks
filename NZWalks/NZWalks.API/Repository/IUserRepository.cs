using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repository
{
    public interface IUserRepository
    {
        public Task<User> AuthenticateAsync(string username, string password);
    }
}
