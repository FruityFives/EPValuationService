using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ValuationServiceAPI.Models;
using ValuationServiceAPI.Services;
using ValuationServiceAPI.Repository;
using Microsoft.Extensions.Logging;

namespace ValuationServiceAPI.MSTest
{
    [TestClass]
    public class ValuationServiceTests
    {
        private Mock<IValuationRepository> _repoMock = null!;
        private Mock<ILogger<ValuationService>> _loggerMock = null!;
        private Mock<IRabbitMqPublisher> _publisherMock = null!;
        private Mock<IConditionReportPdfGenerator> _pdfMock = null!;
        private ValuationService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repoMock = new Mock<IValuationRepository>();
            _loggerMock = new Mock<ILogger<ValuationService>>();
            _publisherMock = new Mock<IRabbitMqPublisher>();
            _pdfMock = new Mock<IConditionReportPdfGenerator>();

            _service = new ValuationService(
                _loggerMock.Object,
                _publisherMock.Object,
                _pdfMock.Object,
                _repoMock.Object
            );
        }

        [TestMethod]
        public async Task SubmitValuationRequest_ShouldInsertIntoRepository_WithCorrectData()
        {
            var request = new ValuationRequest
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Description = "Test beskrivelse",
                Pictures = new List<string> { "http://billede.dk/pic1.jpg" }
            };

            await _service.SubmitValuationRequest(request);

            _repoMock.Verify(r => r.AddValuationRequestAsync(It.Is<ValuationRequest>(x =>
                x.Id == request.Id &&
                x.UserId == request.UserId &&
                x.Description == request.Description &&
                x.Pictures.Count == 1 &&
                x.Pictures[0] == "http://billede.dk/pic1.jpg"
            )), Times.Once);
        }

        [TestMethod]
        public async Task SubmitFullAssessmentAsync_ShouldSaveAll_WithCorrectData()
        {
            // Arrange
            var report = new ConditionReport
            {
                ConditionReportId = Guid.NewGuid(),
                Title = "Vurdering",
                Summary = "I god stand",
                MaterialCondition = "Træ, ubehandlet",
                Functionality = "Stabil",
                ComponentRemarks = "Let patina",
                AuthenticityDetails = "Original 1920",
                Dimensions = "Højde: 100cm, Bredde: 50cm",
                ReportDate = DateTime.UtcNow,
                AssessedBy = "vurderingsekspert@test.dk"
            };

            var assessment = new Assessment
            {
                AssessmentId = Guid.NewGuid(),
                Title = "Gammel gyngestol",
                AssessmentPrice = 2750,
                ExpertId = Guid.NewGuid(),
                ValuationRequestId = Guid.NewGuid(),
                Picture = "https://example.com/stol.jpg",
                Category = "Møbler"
            };

            _pdfMock.Setup(p => p.GeneratePdf(report)).Returns("/files/condition-reports/stol.pdf");

            // Act
            await _service.SubmitFullAssessmentAsync(assessment, report);

            // Assert database saves
            _repoMock.Verify(r => r.AddConditionReportAsync(It.Is<ConditionReport>(cr =>
                cr.ConditionReportId == report.ConditionReportId &&
                cr.Title == report.Title &&
                cr.PdfUrl == "/files/condition-reports/stol.pdf"
            )), Times.Once);

            _repoMock.Verify(r => r.AddAssessmentAsync(It.Is<Assessment>(a =>
                a.Title == "Gammel gyngestol" &&
                a.AssessmentPrice == 2750 &&
                a.ConditionReportId == report.ConditionReportId &&
                a.Category == "Møbler"
            )), Times.Once);

            // Assert RabbitMQ publishing
            _publisherMock.Verify(p => p.PublishAsync(It.Is<ItemAssessmentDTO>(dto =>
                dto.Title == "Gammel gyngestol" &&
                dto.AssessmentPrice == 2750 &&
                dto.Picture == "https://example.com/stol.jpg" &&
                dto.Category == "Møbler" &&
                dto.ConditionReportUrl == "/files/condition-reports/stol.pdf"
            )), Times.Once);
        }
    }
}
