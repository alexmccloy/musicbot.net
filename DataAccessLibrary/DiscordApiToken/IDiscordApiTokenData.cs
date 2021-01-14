using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.DiscordApiToken
{
    public interface IDiscordApiTokenData
    {
        Task<string> GetApiToken(string serverName);

        Task<IEnumerable<Models.DiscordApiToken>> GetAllKeys();
        Task InsertApiKey(Models.DiscordApiToken key);

        Task Init();
    }
}