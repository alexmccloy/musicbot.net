using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary
{
    public interface ISqlDataAccess
    {
        string ConnectionStringName { get; set; }
        Task<IEnumerable<T>> LoadData<T, U>(string sql, U parameters);
        Task SaveData<T>(string sql, T parameters);

        Task CreateTable(string sql);
    }
}