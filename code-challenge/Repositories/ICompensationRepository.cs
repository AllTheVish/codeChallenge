using challenge.Models;
using System.Linq;
using System.Threading.Tasks;

namespace challenge.Repositories
{
    public interface ICompensationRepository
    {
        Compensation Add(Compensation compensation);
        IQueryable<Compensation> GetByEmployeeId(string employeeId);
        Task SaveAsync();
    }
}