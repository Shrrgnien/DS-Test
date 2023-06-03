using DS_Test.Interfaces;
using DS_Test.Models;
using DS_Test.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NPOI.XSSF.UserModel;
using System.Diagnostics;
using System.IO;

namespace DS_Test.Tests
{
    public class ExcelParserTests
    {
        private string _dataFolder = "TestData";
        private string _fileName = "moskva_2010.xlsx";
        private Mock<IWeatherArchivesRepository> _mockRepo;
        private Mock<ILogger<WeatherRecordExcelParser>> _mockLogger;
        private ILogger<WeatherRecordExcelParser> _logger => _mockLogger.Object;
        private IWeatherArchivesRepository _repo => _mockRepo.Object;

        private WeatherRecord _expectedFirstRecord = new()
        {
            Date = DateTime.Parse("01.01.2010 00:00"),
            Temperature = -5.5,
            AirFlowDirection = "З,ЮЗ",
            AirHumidity = 89,
            AirFlowSpeed = 1,
            Cloudiness = 100,
            h = 800,
            VV = null,
            WeatherEvents = "Дымка",
            Pressure = 737,
            Td = -6.9
        };

        private WeatherRecord _expectedLastRecord = new()
        {
            Date = DateTime.Parse("31.12.2010 03:00"),
            Temperature = -11.2,
            AirFlowDirection = "ЮЗ",
            AirHumidity = 90.07,
            AirFlowSpeed = 1,
            Cloudiness = 100,
            h = 600,
            VV = null,
            WeatherEvents = "Непрерывный слабый снег",
            Pressure = 746,
            Td = -12.5
        };
        private int _expectedRecordsCount = 2718;

        private List<WeatherRecord> _weatherRecords = new List<WeatherRecord>
        {
            new WeatherRecord { Id = 1, Date = DateTime.Now },
            new WeatherRecord { Id = 2, Date = DateTime.Now }
        };

        public ExcelParserTests()
        {
            _mockLogger = new Mock<ILogger<WeatherRecordExcelParser>>();
            _mockRepo = new Mock<IWeatherArchivesRepository>();

            _mockRepo.Setup(repo => repo.GetAllRecords()).Returns(_weatherRecords);
            _mockRepo.Setup(repo => repo.GetRecordsDictionary()).Returns(new Dictionary<DateTime, WeatherRecord>());
        }

        [Fact]
        public async void ExcelParser_ShouldParse_xlsx()
        {
            var excelParser = new WeatherRecordExcelParser(_logger, _repo, "TestData", "test.xlsx");
            List<WeatherRecord> actualResult;
            var path = Path.Combine(Environment.CurrentDirectory, _dataFolder, _fileName);
            using (FileStream stream = new(path, FileMode.Open, FileAccess.Read))
            {
                stream.Position = 0;
                var formFile = new FormFile(stream, 0, stream.Length, _fileName, _fileName);
                actualResult = await excelParser.ParseAsync(formFile);
            }

            Assert.NotNull(actualResult);

            var actualFirstRecord = actualResult.First();

            Assert.Equal(_expectedFirstRecord.Date, actualFirstRecord.Date);
            Assert.Equal(_expectedFirstRecord.Temperature, actualFirstRecord.Temperature);
            Assert.Equal(_expectedFirstRecord.AirHumidity, actualFirstRecord.AirHumidity);
            Assert.Equal(_expectedFirstRecord.Td, actualFirstRecord.Td);
            Assert.Equal(_expectedFirstRecord.AirFlowDirection, actualFirstRecord.AirFlowDirection);
            Assert.Equal(_expectedFirstRecord.AirFlowSpeed, actualFirstRecord.AirFlowSpeed);
            Assert.Equal(_expectedFirstRecord.Cloudiness, actualFirstRecord.Cloudiness);
            Assert.Equal(_expectedFirstRecord.h, actualFirstRecord.h);
            Assert.Equal(_expectedFirstRecord.Pressure, actualFirstRecord.Pressure);
            Assert.Equal(_expectedFirstRecord.VV, actualFirstRecord.VV);
            Assert.Equal(_expectedFirstRecord.WeatherEvents, actualFirstRecord.WeatherEvents);

            Assert.Equal(_expectedFirstRecord, actualResult.First(), new WeatherRecordsComparer());
            Assert.Equal(_expectedLastRecord, actualResult.Last(), new WeatherRecordsComparer());

            Assert.Equal(_expectedRecordsCount, actualResult.Count);
        }
    }
}