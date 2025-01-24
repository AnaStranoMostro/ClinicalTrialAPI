using ClinicalTrialAPI.Controllers;
using ClinicalTrialAPI.Data;
using ClinicalTrialAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;


namespace Tests.Controllers
{
    public class ClinicalTrialMetadatasControllerTests
    {
        private readonly ClinicalTrialAPIContext _context;
        private readonly ClinicalTrialMetadatasController _controller;

        public ClinicalTrialMetadatasControllerTests()
        {
            var options = new DbContextOptionsBuilder<ClinicalTrialAPIContext>()
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
            var result = await _controller.GetClinicalTrialMetadata(
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
            var result = await _controller.GetClinicalTrialMetadata(
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
            var result = await _controller.GetClinicalTrialMetadata(
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
            var jsonContent = @"
            {
                ""trialId"": ""test-id-4"",
                ""title"": ""Test Title 4"",
                ""startDate"": ""2023-01-01"",
                ""endDate"": ""2023-02-01"",
                ""participants"": 150,
                ""status"": ""Not Started""
            }";
            var jsonFile = new Mock<IFormFile>();
            var content = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent));
            jsonFile.Setup(_ => _.OpenReadStream()).Returns(content);
            jsonFile.Setup(_ => _.FileName).Returns("test.json");
            jsonFile.Setup(_ => _.Length).Returns(content.Length);

            // Act
            var result = await _controller.PostClinicalTrialMetadata(jsonFile.Object);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdResult = result.Result as CreatedAtActionResult;
            var createdMetadata = createdResult?.Value as ClinicalTrialMetadata;
            Assert.NotNull(createdMetadata);
            Assert.Equal("test-id-4", createdMetadata.TrialId);
            Assert.Equal(31, createdMetadata.TrialDuration);
        }

        [Fact]
        public async Task PostClinicalTrialMetadata_WithoutAddedEndDate_ExpectedBehavior()
        {
            // Arrange
            var jsonContent = @"
            {
                ""trialId"": ""test-id-5"",
                ""title"": ""Test Title 5"",
                ""startDate"": ""2023-01-01"",
                ""participants"": 150,
                ""status"": ""Not Started""
            }";
            var jsonFile = new Mock<IFormFile>();
            var content = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent));
            jsonFile.Setup(_ => _.OpenReadStream()).Returns(content);
            jsonFile.Setup(_ => _.FileName).Returns("test.json");
            jsonFile.Setup(_ => _.Length).Returns(content.Length);

            // Act
            var result = await _controller.PostClinicalTrialMetadata(jsonFile.Object);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdResult = result.Result as CreatedAtActionResult;
            var createdMetadata = createdResult?.Value as ClinicalTrialMetadata;
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
            var result = await _controller.DeleteClinicalTrialMetadata(
                id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(_context.ClinicalTrialMetadata.Find(id));
        }
    }
}