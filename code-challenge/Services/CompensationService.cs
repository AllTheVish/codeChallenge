using System;
using challenge.Models;
using Microsoft.Extensions.Logging;
using challenge.Repositories;
using System.Linq;
using System.Collections.Generic;

namespace challenge.Services
{
    public class CompensationService : ICompensationService
    {
        private readonly ICompensationRepository _compensationRepository;
        private readonly ILogger<CompensationService> _logger;

        public CompensationService(ILogger<CompensationService> logger, ICompensationRepository compensationRepository)
        {
            _compensationRepository = compensationRepository;
            _logger = logger;
        }

        /// <summary>
        /// This method will create a compensation record for the employee on the request if they exist
        /// </summary>
        /// <param name="compensation"></param>
        /// <returns></returns>
        public Compensation Create(Compensation compensation)
        {
            if(compensation == null){
                throw new ArgumentException("compensation is null.");
            }

            _compensationRepository.Add(compensation);
            _compensationRepository.SaveAsync().Wait();

            return compensation;
        }

        /// <summary>
        /// This method will return the compensation with the newest effective date
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public Compensation GetRecentCompensation(string employeeID)
        {
            Compensation recentCompensation = _compensationRepository.GetByEmployeeId(employeeID).OrderByDescending(c => c.EffectiveDate).FirstOrDefault();             
            return recentCompensation;
        }

        public List<Compensation> GetHistoricalCompensation(string employeeID)
        {
            List<Compensation> historicalCompensation = _compensationRepository.GetByEmployeeId(employeeID).OrderByDescending(c => c.EffectiveDate).ToList();
            return historicalCompensation;
        }
    }
}