using System;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using challenge.Data;

namespace challenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<ICompensationRepository> _logger;

        public CompensationRepository(ILogger<ICompensationRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Compensation Add(Compensation compensation)
        {
            //check to see if the employee on the compensation request exists.
            Employee employee = _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == compensation.EmployeeId);
            if(employee == null)
            {
                throw new ArgumentException("employeeId does not exist.");
            }
            compensation.CompensationId = Guid.NewGuid().ToString();
            _employeeContext.Compensations.Add(compensation);
            compensation.Employee = employee;
            return compensation;
        }

        public IQueryable<Compensation> GetByEmployeeId(string employeeId)
        {
            return _employeeContext.Compensations.Include(c=> c.Employee).Where(c => c.EmployeeId == employeeId);                
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }
    }
}