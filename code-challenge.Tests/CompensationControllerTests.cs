using challenge.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using code_challenge.Tests.Integration.Extensions;
using System.Net;
using System.Net.Http;
using code_challenge.Tests.Integration.Helpers;
using System.Text;
using System;
using System.Collections.Generic;

namespace code_challenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestServerStartup>()
                .UseEnvironment("Development"));

            _httpClient = _testServer.CreateClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateCompensation_Returns_OK()
        {
            // Arrange
            var compensation = new Compensation()
            {
                EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                Salary = 50000,
                EffectiveDate = DateTimeOffset.Now
            };
            var expectedFirstName = "John";

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
           new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation.EmployeeId);
            Assert.AreEqual(expectedFirstName, newCompensation.Employee.FirstName);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);

        }

        [TestMethod]
        public void CreateCompensation_Returns_BadRequest()
        {
            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(string.Empty, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_Returns_BadRequest_Invalid_EmployeeID()
        {
            // Arrange
            var compensation = new Compensation()
            {
                EmployeeId = "234234-64-23s-f-sdffsf",
                Salary = 100,
                EffectiveDate = DateTimeOffset.Now
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_Returns_BadRequest_missing_RequiredFields()
        {
            // Arrange
            var compensation = new Compensation()
            {
                Salary = 100,
                EffectiveDate = DateTimeOffset.Now
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void GetRecentCompensationByEmployeeId_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";


            // Execute
            CreateCompensationHelper(employeeId, 50000);
            var getRequestTask = _httpClient.GetAsync($"api/compensation/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var recentCompensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(expectedFirstName, recentCompensation.Employee.FirstName);
            Assert.AreEqual(expectedLastName, recentCompensation.Employee.LastName);
            Assert.AreEqual(50000, recentCompensation.Salary);
        }

        [TestMethod]
        public void GetRecentCompensationByEmployeeId_Returns_NotFound()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82casdf";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetHistoricalCompensationByEmployeeId_Returns_Ok()
        {
            // Arrange
            var employeeId = "c0c2293d-16bd-4603-8e08-638a9d18b22c";
            var expectedFirstName = "George";
            var expectedLastName = "Harrison";


            // Execute
            CreateCompensationHelper(employeeId, 50000);
            CreateCompensationHelper(employeeId, 75000);
            CreateCompensationHelper(employeeId, 100000);
            var getRequestTask = _httpClient.GetAsync($"api/compensation/employee/{employeeId}/history");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensationHistory = response.DeserializeContent<List<Compensation>>();
            Assert.AreEqual(3, compensationHistory.Count);
            Assert.AreEqual(expectedFirstName, compensationHistory[0].Employee.FirstName);
            Assert.AreEqual(expectedLastName, compensationHistory[0].Employee.LastName);
            Assert.AreEqual(100000, compensationHistory[0].Salary);
            Assert.AreEqual(75000, compensationHistory[1].Salary);
            Assert.AreEqual(50000, compensationHistory[2].Salary);
        }

        private void CreateCompensationHelper(string employeeID, float salary)
        {
            var compensation = new Compensation()
            {
                EmployeeId = employeeID,
                Salary = salary,
                EffectiveDate = DateTimeOffset.Now
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
           new StringContent(requestContent, Encoding.UTF8, "application/json"));
        }
    }
}
