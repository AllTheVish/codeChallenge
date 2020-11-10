using System;

namespace challenge.Models
{
    public class Compensation
    {
        public string CompensationId { get; set; }
        public DateTimeOffset? EffectiveDate { get; set; }
        public Employee Employee { get; set; }
        public string EmployeeId { get; set; }        
        public float? Salary { get; set; }        
    }
}