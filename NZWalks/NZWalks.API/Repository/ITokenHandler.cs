using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repository
{
    public interface ITokenHandler
    {
        public Task<string> CreateTokenAsync(User user);
    }
}
