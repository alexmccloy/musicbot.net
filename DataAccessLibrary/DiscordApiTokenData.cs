using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DataAccessLibrary.Models;

namespace DataAccessLibrary
{
    //TODO create the Table in the database if it doesnt already exist
    public class DiscordApiTokenData : IDiscordApiTokenData
    {
        private readonly ISqlDataAccess _db;

        public DiscordApiTokenData(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<string> GetApiToken(string serverName) 
            //TODO do this properly with Dapper and not using raw strings from the client MONKAS
        {
            string sql = $"select ApiKey from DiscordApiKey where ServerName = {serverName} LIMIT 1";
            var result = await _db.LoadData<string, dynamic>(sql, new { });

            if (result.Count != 1) throw new InvalidDataException($"The sql database does not contain a servername of {serverName}");

            return result[0];
        }

        public Task<List<DiscordApiToken>> GetAllKeys()
        {
            string sql = "select * from DiscordApiKey";
            return _db.LoadData<DiscordApiToken, dynamic>(sql, new { });
        }

        public Task InsertApiKey(DiscordApiToken key)
        {
            string sql = "insert into DiscordApiKey (ServerName, ApiKey) values (@ServerName, @ApiKey)";
            return _db.SaveData(sql, key);
        }

        /// <summary>
        /// Create SQL Tables if they dont already exist
        /// </summary>
        public Task Init()
        {
            string sql = @"CREATE TABLE IF NOT EXISTS [DiscordApiKey](
                           [ServerName] VARCHAR PRIMARY KEY ON CONFLICT REPLACE NOT NULL UNIQUE, 
                           [ApiKey] VARCHAR NOT NULL);";

            return _db.CreateTable(sql);
        }
    }
}