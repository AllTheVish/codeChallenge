using challenge.Models;
using Microsoft.Extensions.Logging;
using challenge.Repositories;

namespace challenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!string.IsNullOrWhiteSpace(id))
            {
                return _employeeRepository.GetById(id, "DirectReports");
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        public ReportingStructure GetReportingStructure(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }
            var employee = _employeeRepository.GetById(id, null);
        
            //if the employee could not be found then return null and the calling method can handle it as they need to.
            if (employee == null) return null;

            //get the total number of reports for this employee
            // implementation uses recursion, but in a larger use case it would likly be better to alter the data structure or make use of a SPROC inthe DB to do the computation
            var numReports = _employeeRepository.GetNumberOfReports(employee.EmployeeId);

            return new ReportingStructure()
            {
                Employee = employee,
                NumberOfReports = numReports
            };
        }
    }
}