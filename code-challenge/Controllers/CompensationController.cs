using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using challenge.Services;
using challenge.Models;

namespace challenge.Controllers
{
    [Route("api/compensation")]
    public class CompensationController : Controller
    {
        private readonly ILogger _logger;
        private readonly ICompensationService _compensationService;

        public CompensationController(ILogger<CompensationController> logger, ICompensationService compensationService)
        {
            _logger = logger;
            _compensationService = compensationService;
        }

        [HttpPost]
        public IActionResult CreateCompensation([FromBody] Compensation compensation)
        {
            ///VSHAH - Added null check to prevent object reference error when trying to write debug log if the request body was null.
            if (compensation != null)
            {
                if (!ValidCompensationRequest(compensation)){

                    return BadRequest(ModelState);
                }
                try
                {
                    _logger.LogDebug($"Received compensation create request for '{compensation.EmployeeId}'");
                    var compensationResult = _compensationService.Create(compensation);
                    
                    return CreatedAtRoute("getRecentCompensationByEmployeeId", new { id = compensation.EmployeeId }, compensation);
                }
                catch(Exception e)
                {
                    return BadRequest(new { message = "Error encountered while processing compensation request.", detail= $"{e.Message}"});
                }
                          
            }
            else
            {
                _logger.LogError($"Received compensation create request with a null body");
                return BadRequest(new { message = "Request body cannot be empty" });
            }
        }

        [HttpGet("employee/{id}", Name = "getRecentCompensationByEmployeeId")]
        public IActionResult GetRecentCompensationByEmployeeId([FromRoute] string id)
        {
            _logger.LogDebug($"Received compensation get request for employee '{id}'");

            var compensation = _compensationService.GetRecentCompensation(id);

            if (compensation == null)
                return NotFound();

            return Ok(compensation);
            
        }

        /// <summary>
        /// This end point will aloow for a user to get all the compensation records for an employee
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("employee/{id}/history", Name = "getHistoricalCompensationByEmployeeId")]
        public IActionResult GetHistoricalCompensationByEmployeeId([FromRoute] string id)
        {
            // Depending on if hirtorical data is actually needed to be stored, this could be modifed to support OData Query Options so that the result set could be better filtered
            // making it easier to fetch data from a large data set.

            _logger.LogDebug($"Received historical compensation get request for employee '{id}'");

            var compensation = _compensationService.GetHistoricalCompensation(id);

            if (compensation == null)
                return NotFound();

            return Ok(compensation);

        }

        /// <summary>
        /// This is a basic validation function to check the properties of the compensation request to ensure that certian fields are present.
        /// This would allow to shortcut the processing if the request is deemed invalid up front.
        /// </summary>
        /// <param name="compensation"></param>
        /// <returns></returns>
        private bool ValidCompensationRequest(Compensation compensation)
        {
            bool isValid = true;
            if (String.IsNullOrWhiteSpace(compensation.EmployeeId)) { 
                ModelState.AddModelError("employeeId", "employeeId is required.");
                isValid = false;
            }
            if (!compensation.Salary.HasValue)
            {
                ModelState.AddModelError("salary", "salary is required.");
                isValid = false;
            }
            if (!compensation.EffectiveDate.HasValue)
            {
                ModelState.AddModelError("effectiveDate", "effectiveDate is required.");
                isValid = false;
            }
            return isValid;
        }
    }
}