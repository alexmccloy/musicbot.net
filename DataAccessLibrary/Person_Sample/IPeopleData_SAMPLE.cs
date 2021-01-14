using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLibrary.Models;

namespace DataAccessLibrary
{
    public interface IPeopleData_SAMPLE
    {
        Task<IEnumerable<PersonModel_SAMPLE>> GetPeople();
        Task InsertPerson(PersonModel_SAMPLE person);
    }
}