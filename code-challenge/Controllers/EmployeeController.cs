using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using challenge.Services;
using challenge.Models;

namespace challenge.Controllers
{
    [Route("api/employee")]
    public class EmployeeController : Controller
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            ///VSHAH - Added null check to prevent object reference error when trying to write debug log if the request body was null.
            if (employee != null)
            {
                _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");
                _employeeService.Create(employee);
                return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);            }
            else
            {
                _logger.LogError($"Received employee create request with a null body");
                return BadRequest(new { message = "Request body cannot be empty" });
            }
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById([FromRoute] string id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee([FromRoute] string id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        [HttpGet("{id}/reportingStructure", Name = "getEmployeeReportingStructure")]
        public IActionResult GetEmployeeReportingStructure([FromRoute] string id)
        {
            _logger.LogDebug($"Received employee reporting structure request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            var ReportingStructure = _employeeService.GetReportingStructure(id);
            if (ReportingStructure == null) return NotFound();

            return Ok(ReportingStructure);
        }
    }
}