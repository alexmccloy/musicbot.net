using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLibrary.Models;

namespace DataAccessLibrary
{
    public class PeopleData_SAMPLE : IPeopleData_SAMPLE
    {
        private readonly ISqlDataAccess _db;

        public PeopleData_SAMPLE(ISqlDataAccess db)
        {
            _db = db;
        }

        public Task<IEnumerable<PersonModel_SAMPLE>> GetPeople()
        {
            string sql = "select * from dbo.People";
            return _db.LoadData<PersonModel_SAMPLE, dynamic>(sql, new { });
        }

        public Task InsertPerson(PersonModel_SAMPLE person)
        {
            string sql = @"insert into dbo.People (FirstName, LastName, EmailAddress) 
                           values (@FirstName, @LastName, @EmailAddress);";

            return _db.SaveData(sql, person);
        }
    }
}