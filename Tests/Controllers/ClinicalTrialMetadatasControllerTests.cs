using ClinicalTrialAPI.Controllers;
using ClinicalTrialAPI.Data;
using ClinicalTrialAPI.Helpers;
using ClinicalTrialAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json.Linq;


namespace Tests.Controllers
{
    public class ClinicalTrialMetadatasControllerTests
    {
        private readonly ClinicalTrialAPIContext _context;
        private readonly ClinicalTrialMetadatasController _controller;

        public ClinicalTrialMetadatasControllerTests()
        {
            DbContextOptions<ClinicalTrialAPIContext> options = new DbContextOptionsBuilder<ClinicalTrialAPIContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ClinicalTrialAPIContext(options);
            _controller = new ClinicalTrialMetadatasController(_context);

            //seed data
             _context.ClinicalTrialMetadata.Add(new ClinicalTrialMetadata
            {
                TrialId = "test-id",
                Title = "Test Title",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(40),
                Participants = 100,
                Status = ClinicalTrialStatus.NotStarted,
                TrialDuration = 40
            });
             _context.ClinicalTrialMetadata.Add(new ClinicalTrialMetadata
            {
                TrialId = "test-id-2",
                Title = "Test Title 2",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(365),
                Participants = 200,
                Status = ClinicalTrialStatus.Completed,
                TrialDuration = 365
            });
             _context.ClinicalTrialMetadata.Add(new ClinicalTrialMetadata
            {
                TrialId = "test-id-3",
                Title = "Test Title 3",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(100),
                Participants = 300,
                Status = ClinicalTrialStatus.Completed,
                TrialDuration = 100
            });
            //more data can be added here
             _context.SaveChanges();
        }

        [Fact]
        public async Task GetClinicalTrialMetadata_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            string? trialId = null;
            string? title = null;
            ClinicalTrialStatus? status = null;

            // Act
            ActionResult<IEnumerable<ClinicalTrialMetadata>> result = await _controller.GetClinicalTrialMetadata(
                trialId,
                title,
                status);

            // Assert
             Assert.IsType<List<ClinicalTrialMetadata>>(result.Value);
            Assert.Equal(3, result.Value.Count());
        }

        [Fact]
        public async Task GetClinicalTrialMetadata_WithSpecificTrialId_ReturnsSingleValue()
        {
            // Arrange
            string trialId = "test-id-3";
            string? title = null;
            ClinicalTrialStatus? status = null;

            // Act
            ActionResult<IEnumerable<ClinicalTrialMetadata>> result = await _controller.GetClinicalTrialMetadata(
                trialId,
                title,
                status);

            // Assert
             Assert.IsType<List<ClinicalTrialMetadata>>(result.Value);
             Assert.Single(result.Value);
            Assert.Equal("test-id-3", result.Value.First().TrialId);
        }


        [Fact]
        public async Task GetClinicalTrialMetadata_WithStatusCompleted_ReturnsTwoItems()
        {
            // Arrange
            string? trialId = null;
            string? title = null;
            ClinicalTrialStatus? status = ClinicalTrialStatus.Completed;

            // Act
            ActionResult<IEnumerable<ClinicalTrialMetadata>> result = await _controller.GetClinicalTrialMetadata(
                trialId,
                title,
                status);

            // Assert
             Assert.IsType<List<ClinicalTrialMetadata>>(result.Value);
            Assert.Equal(2, result.Value.Count());
            Assert.All(result.Value, item => Assert.Equal(ClinicalTrialStatus.Completed, item.Status));
        }


        [Fact]
        public async Task PostClinicalTrialMetadata_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            string jsonContent = @"
            {
                ""trialId"": ""test-id-4"",
                ""title"": ""Test Title 4"",
                ""startDate"": ""2023-01-01"",
                ""endDate"": ""2023-02-01"",
                ""participants"": 150,
                ""status"": ""Not Started""
            }";
            Mock<IFormFile> jsonFile = new Mock<IFormFile>();
            MemoryStream content = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent));
            jsonFile.Setup(f => f.OpenReadStream()).Returns(content);
            jsonFile.Setup(f => f.FileName).Returns("test.json");
            jsonFile.Setup(f => f.Length).Returns(content.Length);


            // Act
            ActionResult<ClinicalTrialMetadata> result = await _controller.PostClinicalTrialMetadata(jsonFile.Object);

            // Assert
             Assert.IsType<CreatedAtActionResult>(result.Result);
            CreatedAtActionResult? createdResult = result.Result as CreatedAtActionResult;
            ClinicalTrialMetadata? createdMetadata = createdResult?.Value as ClinicalTrialMetadata;
            Assert.NotNull(createdMetadata);
            Assert.Equal("test-id-4", createdMetadata.TrialId);
            Assert.Equal(31, createdMetadata.TrialDuration);
        }

        [Fact]
        public async Task PostClinicalTrialMetadata_JsonDoesntMatchSchema_ExpectedBehavior()
        {
            // Arrange
            string jsonContent = @"
            {
                ""trialId"": ""test-id-4"",
                ""title"": ""Test Title 4"",
                ""startDate"": ""2023-01-01"",
                ""endDate"": ""2023-02-01"",
                ""participants"": 0,
                ""status"": 42 //the meaning of life! :) 
           }";
            Mock<IFormFile> jsonFile = new Mock<IFormFile>();
            MemoryStream content = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent));
            jsonFile.Setup(f => f.OpenReadStream()).Returns(content);
            jsonFile.Setup(f => f.FileName).Returns("test.json");
            jsonFile.Setup(f => f.Length).Returns(content.Length);


            // Act
            ActionResult<ClinicalTrialMetadata> result = await _controller.PostClinicalTrialMetadata(jsonFile.Object);

            // Assert
            // make sure that the result is BadRequestObjectResult and that the error messages are correct
            Assert.IsType<BadRequestObjectResult>(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            var errorResponse = badRequestResult?.Value as ErrorResponse;
            Assert.NotNull(errorResponse);
            Assert.Contains(errorResponse.Errors, e => e.Contains("Integer 0 is less than minimum value of 1. Path 'participants'"));
            Assert.Contains(errorResponse.Errors, e => e.Contains("Invalid type. Expected String but got Integer. Path 'status'"));
        }


        [Fact]
        public async Task PostClinicalTrialMetadata_EndDateBeforeStartDate_ExpectedBehavior()
        {
            // Arrange
            string jsonContent = @"
            {
                ""trialId"": ""test-id-X"",
                ""title"": ""Test Title X"",
                ""startDate"": ""2023-01-01"",
                ""endDate"": ""2022-02-01"",
                ""participants"": 150,
                ""status"": ""Not Started""
            }";
            Mock<IFormFile> jsonFile = new Mock<IFormFile>();
            MemoryStream content = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent));
            jsonFile.Setup(f => f.OpenReadStream()).Returns(content);
            jsonFile.Setup(f => f.FileName).Returns("test.json");
            jsonFile.Setup(f => f.Length).Returns(content.Length);


            // Act
            ActionResult<ClinicalTrialMetadata> result = await _controller.PostClinicalTrialMetadata(jsonFile.Object);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("End date cannot be before start date.", (result.Result as BadRequestObjectResult)?.Value);
        }

        [Fact]
        public async Task PostClinicalTrialMetadata_WithoutAddedEndDate_ExpectedBehavior()
        {
            // Arrange
            string jsonContent = @"
            {
                ""trialId"": ""test-id-5"",
                ""title"": ""Test Title 5"",
                ""startDate"": ""2023-01-01"",
                ""participants"": 150,
                ""status"": ""Not Started""
            }";
            Mock<IFormFile> jsonFile = new Mock<IFormFile>();
            MemoryStream content = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent));
             jsonFile.Setup(f => f.OpenReadStream()).Returns(content);
             jsonFile.Setup(f => f.FileName).Returns("test.json");
             jsonFile.Setup(f => f.Length).Returns(content.Length);

            // Act
            ActionResult<ClinicalTrialMetadata> result = await _controller.PostClinicalTrialMetadata(jsonFile.Object);

            // Assert
             Assert.IsType<CreatedAtActionResult>(result.Result);
            CreatedAtActionResult? createdResult = result.Result as CreatedAtActionResult;
            ClinicalTrialMetadata? createdMetadata = createdResult?.Value as ClinicalTrialMetadata;
            Assert.NotNull(createdMetadata);
            Assert.Equal("test-id-5", createdMetadata.TrialId);
            Assert.Equal(31, createdMetadata.TrialDuration);
            Assert.Equal("2023-02-01", createdMetadata.EndDate?.ToString("yyyy-MM-dd"));
            Assert.Equal(ClinicalTrialStatus.Ongoing, createdMetadata.Status);
        }

        [Fact]
        public async Task DeleteClinicalTrialMetadata_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            string id = "test-id";

            // Act
            IActionResult result = await _controller.DeleteClinicalTrialMetadata(
                id);

            // Assert
             Assert.IsType<NoContentResult>(result);
            Assert.Null(_context.ClinicalTrialMetadata.Find(id));
        }
    }
}