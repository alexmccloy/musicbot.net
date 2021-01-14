using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.ActivityLogging
{
    public interface IActivityData
    {
        Task<IEnumerable<Activity>> GetActivityHistory(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Activity>> GetActivityHistory(DateTime startDate);
        Task AddActivity(Activity activity);
        
        //TODO add a method to delete old records?

        Task Init();
    }
}