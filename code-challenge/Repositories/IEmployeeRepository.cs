using challenge.Models;
using System.Threading.Tasks;

namespace challenge.Repositories
{
    public interface IEmployeeRepository
    {
        ///VSHA - Added support for taking in include properties so that you have the ability to expand subobjects when needed.
        Employee GetById(string id, string includeProperties);
        Employee Add(Employee employee);
        Employee Remove(Employee employee);
        Task SaveAsync();

        int GetNumberOfReports(string id);
    }
}