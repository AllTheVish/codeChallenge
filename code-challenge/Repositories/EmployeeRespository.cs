using System;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using challenge.Data;

namespace challenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id, string includeProperties)
        {
            ///VSHAH - Added the .Include for the direct reports as they were always being returned as a null property
            ///         This did not mesh with the schema noted in the ReadMe for the challenge.
            ///         
            if (!string.IsNullOrWhiteSpace(includeProperties)){
                return _employeeContext.Employees.Include(includeProperties).SingleOrDefault(e => e.EmployeeId == id);
            }
            else
            {
                return _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
            }
            
        }

        public int GetNumberOfReports(string id)
        {
            ///VSHAH - Ideally if I had more control over the data structure i would have the information stored in the DB or have a SPROC that can recursivle get the neccessary data.
            int numberOfReports = 0;

           var directReports = _employeeContext.Employees.Include(e => e.DirectReports).SingleOrDefault(e => e.EmployeeId == id).DirectReports;

            if (directReports != null && directReports.Count > 0)
            {
                foreach (Employee employee in directReports)
                {
                    numberOfReports += 1 + GetNumberOfReports(employee.EmployeeId);
                }
            }
            
            return numberOfReports;
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}