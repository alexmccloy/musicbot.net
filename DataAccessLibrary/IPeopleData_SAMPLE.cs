using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLibrary.Models;

namespace DataAccessLibrary
{
    public interface IPeopleData_SAMPLE
    {
        Task<List<PersonModel_SAMPLE>> GetPeople();
        Task InsertPerson(PersonModel_SAMPLE person);
    }
}