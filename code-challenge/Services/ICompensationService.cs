using challenge.Models;
using System.Collections.Generic;

namespace challenge.Services
{
    public interface ICompensationService
    {
        Compensation GetRecentCompensation(string employeeID);
        List<Compensation> GetHistoricalCompensation(string employeeID);
        Compensation Create(Compensation compensation);        
    }
}