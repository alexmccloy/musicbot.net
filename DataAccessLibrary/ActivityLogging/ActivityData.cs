using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLibrary.ActivityLogging;

namespace DataAccessLibrary.Models
{
    public class ActivityData : IActivityData
    {
        private readonly ISqlDataAccess _db;

        private const string TableName = "CommandHistory";

        public ActivityData(ISqlDataAccess db)
        {
            _db = db;
        }

        public Task<IEnumerable<Activity>> GetActivityHistory(DateTime startDate) => GetActivityHistory(startDate, DateTime.MaxValue);
        
        public Task<IEnumerable<Activity>> GetActivityHistory(DateTime startDate, DateTime endDate)
        {
            string sql = @"SELECT * FROM CommandHistory where OccuredAt BETWEEN @StartDate AND @EndDate";
            return _db.LoadData<Activity, dynamic>(sql, new {StartDate = startDate, EndDate = endDate});
        }


        public Task AddActivity(Activity activity)
        {
            string sql = @"INSERT INTO CommandHistory (OccuredAt, CommandName, Author, Channel, Succeeded, Result) 
                           VALUES (@OccuredAt, @CommandName, @Author, @Channel, @Succeeded, @Result)";
            return _db.SaveData(sql, activity);
        }

        /// <summary>
        /// Creates the table used by this class if it does not exist
        /// </summary>
        public Task Init()
        {
            string sql = @"CREATE TABLE IF NOT EXISTS [CommandHistory](
                           [OccuredAt] DATETIME,
                           [CommandName] VARCHAR,
                           [Author] VARCHAR,
                           [Channel] VARCHAR,
                           [Succeeded] BOOLEAN,
                           [Result] VARCHAR
                           )";

            return _db.CreateTable(sql);
        }
    }
}