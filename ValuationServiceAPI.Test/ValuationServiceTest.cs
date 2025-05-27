using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using ValuationServiceAPI.Models;
using ValuationServiceAPI.Repository;
using ValuationServiceAPI.Services;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ValuationServiceAPI.MSTest
{
    [TestClass]
    public class ValuationServiceTests2
    {
        private Mock<IValuationRepository> _repoMock;
        private Mock<ILogger<ValuationService>> _loggerMock;
        private Mock<IRabbitMqPublisher> _publisherMock;
        private Mock<IConditionReportPdfGenerator> _pdfMock;
        private ValuationService _service;

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
                _repoMock.Object);
        }

        /// <summary>
        /// Tester at SubmitValuationRequest() kalder repository-metoden AddValuationRequestAsync én gang.
        /// Dette sikrer, at en vurderingsanmodning bliver gemt korrekt i databasen.
        /// </summary>
        [TestMethod]
        public async Task SubmitValuationRequest_ShouldCallAddValuationRequest()
        {
            var request = new ValuationRequest { Id = Guid.NewGuid() };

            await _service.SubmitValuationRequest(request);

            _repoMock.Verify(r => r.AddValuationRequestAsync(request), Times.Once);
        }

        /// <summary>
        /// Tester at SubmitFullAssessmentAsync() kaster ArgumentNullException,
        /// hvis vurderingsobjektet (assessment) er null.
        /// Dette tjekker at servicen validerer input korrekt.
        /// </summary>
        [TestMethod]
        public async Task SubmitFullAssessmentAsync_NullAssessment_ShouldThrow()
        {
            var report = new ConditionReport();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _service.SubmitFullAssessmentAsync(null, report));
        }

        /// <summary>
        /// Tester at SubmitFullAssessmentAsync() kaster ArgumentNullException,
        /// hvis tilstandsrapporten (report) er null.
        /// Sikrer at input valideres, så tomme rapporter ikke behandles.
        /// </summary>
        [TestMethod]
        public async Task SubmitFullAssessmentAsync_NullReport_ShouldThrow()
        {
            var assessment = new Assessment();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _service.SubmitFullAssessmentAsync(assessment, null));
        }

        /// <summary>
        /// Tester at SubmitFullAssessmentAsync() kalder de nødvendige metoder, når både vurdering og rapport er gyldige.
        /// Den tjekker at:
        /// - PDF'en bliver genereret og sat på rapporten
        /// - Både rapport og vurdering bliver gemt i databasen
        /// - En besked bliver publiceret til RabbitMQ
        /// </summary>
        [TestMethod]
        public async Task SubmitFullAssessmentAsync_ValidInput_ShouldCallAll()
        {
            var assessment = new Assessment
            {
                /* fyldt ud med testdata */
            };
            var report = new ConditionReport { ConditionReportId = Guid.Empty };

            _pdfMock.Setup(p => p.GeneratePdf(It.IsAny<ConditionReport>())).Returns("http://pdf.url/report.pdf");

            await _service.SubmitFullAssessmentAsync(assessment, report);

            Assert.AreNotEqual(Guid.Empty, report.ConditionReportId); // Tjek at ID blev sat
            Assert.AreEqual("http://pdf.url/report.pdf", report.PdfUrl); // Tjek at PDF blev genereret

            _repoMock.Verify(r => r.AddConditionReportAsync(report), Times.Once);
            _repoMock.Verify(r => r.AddAssessmentAsync(assessment), Times.Once);
            _publisherMock.Verify(p => p.PublishAsync(It.IsAny<ItemAssessmentDTO>()), Times.Once);
        }

        /// <summary>
        /// Tester at UpdateConditionReportAsync() kaster en Exception,
        /// hvis tilstandsrapporten ikke findes i databasen.
        /// Dette sikrer, at man ikke kan opdatere noget, der ikke eksisterer.
        /// </summary>
        [TestMethod]
        public async Task UpdateConditionReportAsync_ReportNotFound_ShouldThrow()
        {
            var report = new ConditionReport { ConditionReportId = Guid.NewGuid() };
            _repoMock.Setup(r => r.GetConditionReportByIdAsync(report.ConditionReportId))
                .ReturnsAsync((ConditionReport)null); // Simulerer at rapporten ikke findes

            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _service.UpdateConditionReportAsync(report));
        }

        /// <summary>
        /// Tester at UpdateConditionReportAsync() opdaterer rapporten og genererer en ny PDF,
        /// når rapporten findes og er gyldig. Det kontrolleres at PDF-linket opdateres og
        /// at repository-metoden bliver kaldt korrekt.
        /// </summary>
        [TestMethod]
        public async Task UpdateConditionReportAsync_ValidReport_ShouldUpdateAndGeneratePdf()
        {
            var reportId = Guid.NewGuid();
            var existingReport = new ConditionReport { ConditionReportId = reportId, PdfUrl = "oldReport.pdf" };
            var updatedReport = new ConditionReport { ConditionReportId = reportId };

            _repoMock.Setup(r => r.GetConditionReportByIdAsync(reportId)).ReturnsAsync(existingReport);
            _pdfMock.Setup(p => p.GeneratePdf(updatedReport)).Returns("newReport.pdf");

            await _service.UpdateConditionReportAsync(updatedReport);

            Assert.AreEqual("newReport.pdf", updatedReport.PdfUrl);
            _repoMock.Verify(r => r.UpdateConditionReportAsync(updatedReport), Times.Once);
        }

        /// <summary>
        /// Tester at UpdateAssessmentAsync() kalder repository-metoden UpdateAssessmentAsync én gang.
        /// Dette bekræfter at en opdateret vurdering gemmes korrekt.
        /// </summary>
        [TestMethod]
        public async Task UpdateAssessmentAsync_ShouldCallUpdateAssessment()
        {
            var assessment = new Assessment { AssessmentId = Guid.NewGuid() };

            await _service.UpdateAssessmentAsync(assessment);

            _repoMock.Verify(r => r.UpdateAssessmentAsync(assessment), Times.Once);
        }
    }
}