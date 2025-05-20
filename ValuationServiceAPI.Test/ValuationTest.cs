using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ValuationServiceAPI.Models;
using ValuationServiceAPI.Services;
using MongoDB.Driver;

namespace ValuationServiceAPI.Test
{
    [TestClass]
    public class ValuationTest
    {
        private Mock<ILogger<ValuationService>> _loggerMock; // Logger mock
        private Mock<IRabbitMqPublisher> _publisherMock;     // RabbitMQ publisher mock
        private Mock<IMongoClient> _mongoClientMock;         // MongoDB client mock
        private Mock<IMongoCollection<ValuationRequest>> _collectionMock; // Mongo collection mock
        private ValuationService _service;                   // Service under test

        [TestInitialize]
        public void Setup()
        {
            // Initialiser mocks før hver test
            _loggerMock = new Mock<ILogger<ValuationService>>();
            _publisherMock = new Mock<IRabbitMqPublisher>();
            _mongoClientMock = new Mock<IMongoClient>();
            _collectionMock = new Mock<IMongoCollection<ValuationRequest>>();

            // Mock database til at returnere vores mocked collection
            var dbMock = new Mock<IMongoDatabase>();
            dbMock.Setup(d => d.GetCollection<ValuationRequest>("ValuationRequests", null))
                  .Returns(_collectionMock.Object);

            // Mock at klienten henter databasen
            _mongoClientMock.Setup(c => c.GetDatabase("ValuationDB", null))
                            .Returns(dbMock.Object);

            // Inject mocks i service
            _service = new ValuationService(_mongoClientMock.Object, _loggerMock.Object, _publisherMock.Object);
        }

        [TestMethod]
        public async Task SubmitValuationRequest_ShouldInsertAndLog()
        {
            // Arrange: Lav en test-request
            var request = new ValuationRequest
            {
                UserId = "user-001",
                Description = "Test item",
                Pictures = new List<string> { "http://test.com/pic.jpg" }
            };

            // Act: Kald metoden vi vil teste
            await _service.SubmitValuationRequest(request);

            // Assert: Tjek at MongoDB insert blev kaldt
            _collectionMock.Verify(c => c.InsertOneAsync(request, null, default), Times.Once);

            // Assert: Tjek at loggeren skrev en infolog
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("ValuationRequest gemt")),
                    It.IsAny<System.Exception>(),
                    It.IsAny<System.Func<It.IsAnyType, System.Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task SendEffectAssessmentAsync_ShouldMapAndPublishDTO()
        {
            // Arrange: Lav en EffectAssessment med data
            var assessment = new EffectAssessment
            {
                Id = "effect-001",
                ExpertId = "expert-123",
                AssessmentPrice = 5000,
                EffectId = "effect-001",
                ConditionReport = new ConditionReport
                {
                    Title = "Test Title",
                    Description = "Test Desc",
                    Rating = 8,
                    Pictures = new List<string> { "http://test.com/pic.jpg" },
                    Date = System.DateTime.UtcNow
                }
            };


            // Act: Kald metoden vi vil teste
            await _service.SendEffectAssessmentAsync(assessment);

            // Assert: Tjek at DTO blev sendt til RabbitMQ korrekt
            _publisherMock.Verify(x => x.PublishAsync(It.Is<ItemAssessmentDTO>(dto =>
                dto.Title == "Test Title" &&
                dto.Picture == "http://test.com/pic.jpg" &&
                dto.SellerId == "expert-123" &&
                dto.Effect.Id == "effect-001"
            )), Times.Once);

            // Assert: Tjek at loggeren skrev en infolog
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("DTO sendt til RabbitMQ")),
                    It.IsAny<System.Exception>(),
                    It.IsAny<System.Func<It.IsAnyType, System.Exception, string>>()),
                Times.Once);
        }
    }
}
