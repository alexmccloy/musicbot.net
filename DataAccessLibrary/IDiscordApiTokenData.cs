using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLibrary.Models;

namespace DataAccessLibrary
{
    public interface IDiscordApiTokenData
    {
        Task<string> GetApiToken(string serverName) 
            //TODO do this properly with Dapper and not using raw strings from the client MONKAS
            ;

        Task<List<DiscordApiToken>> GetAllKeys();
        Task InsertApiKey(DiscordApiToken key);

        Task Init();
    }
}