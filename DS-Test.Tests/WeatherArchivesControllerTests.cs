using DS_Test.Controllers;
using DS_Test.Interfaces;
using DS_Test.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;

namespace DS_Test.Tests
{
    public class WeatherArchivesControllerTests
    {
        private List<WeatherRecord> weatherRecords = new List<WeatherRecord>
        {
            new WeatherRecord { Id = 1, Date = DateTime.Now },
            new WeatherRecord { Id = 2, Date = DateTime.Now }
        };

        private Mock<ILogger<WeatherArchivesController>> _mockLogger;
        private Mock<IExcelParser<WeatherRecord>> _mockParser;
        private Mock<IWeatherArchivesRepository> _mockRepo;
        private IExcelParser<WeatherRecord> _parser => _mockParser.Object;
        private IWeatherArchivesRepository _repo => _mockRepo.Object;
        private ILogger<WeatherArchivesController> _logger => _mockLogger.Object;

        public WeatherArchivesControllerTests()
        {
            _mockRepo = new Mock<IWeatherArchivesRepository>();
            _mockRepo.Setup(repo => repo.GetAllRecords()).Returns(weatherRecords);
            _mockRepo.Setup(repo => repo.GetRecordsAsQueryable()).Returns(weatherRecords.BuildMock());

            _mockLogger = new Mock<ILogger<WeatherArchivesController>>();
            _mockParser = new Mock<IExcelParser<WeatherRecord>>();
        }
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfWeatherRecord()
        {
            var controller = new WeatherArchivesController(_logger, _parser, _repo);

            var result = await controller.Index(null);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<PaginatedList<WeatherRecord>>(
                viewResult.ViewData.Model);

            Assert.Equal(2, model.Count);
            Assert.Equal(1, model.TotalPages);
        }

        [Fact]
        public void ArchiveLoaderGet_ReturnsAViewResult_WithFilesViewModel()
        {
            var controller = new WeatherArchivesController(_logger, _parser, _repo);

            var result = controller.ArchiveLoader();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<FilesViewModel>(
                viewResult.ViewData.Model);
            Assert.Null(model.File);
        }

        [Fact]
        public void ArchiveLoaderPost_ReturnsaViewResult_WithFilesViewModel()
        {
            var controller = new WeatherArchivesController(_logger, _parser, _repo);

        }
    }
}