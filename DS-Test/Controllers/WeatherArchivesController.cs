using DS_Test.Interfaces;
using DS_Test.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DS_Test.Controllers
{
    public class WeatherArchivesController : Controller
    {
        private ILogger<WeatherArchivesController> _logger;
        private IExcelParser<WeatherRecord> _excelParser;
        private IWeatherArchivesRepository _repository;
        private static int _pageSize = 60;
        public WeatherArchivesController(
                ILogger<WeatherArchivesController> logger, 
                IExcelParser<WeatherRecord> excelParser, 
                IWeatherArchivesRepository repository)
        {
            _logger = logger;
            _excelParser = excelParser;
            _repository = repository;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber, string month = "Выберите месяц", string year = "Выберите год")
        {
            ViewData["SelectedMonth"] = month;
            ViewData["SelectedYear"] = year;
            try
            {
                var records = _repository.GetRecordsAsQueryable();
                if(records?.Count() > 0)
                {
                    if (DateTime.TryParseExact(year, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var y))
                    {
                        if (DateTime.TryParseExact(month, "MMMM", CultureInfo.GetCultureInfo("ru"), DateTimeStyles.None, out var m))
                        {
                            return View(await PaginatedList<WeatherRecord>.CreateAsync(records
                                .Where(r => r.Date.Year == y.Year && r.Date.Month == m.Month)
                                .OrderBy(r => r.Date), pageNumber ?? 1, _pageSize));
                        }
                        else
                        {
                            return View(await PaginatedList<WeatherRecord>.CreateAsync(records
                                .Where(r => r.Date.Year == y.Year)
                                .OrderBy(r => r.Date), pageNumber ?? 1, _pageSize));
                        }
                    }
                    else if (DateTime.TryParseExact(month, "MMMM", CultureInfo.GetCultureInfo("ru"), DateTimeStyles.None, out var m))
                    {
                        return View(await PaginatedList<WeatherRecord>.CreateAsync(records
                            .Where(r => r.Date.Month == m.Month)
                            .OrderBy(r => r.Date), pageNumber ?? 1, _pageSize));
                    }
                    return View(await PaginatedList<WeatherRecord>
                        .CreateAsync(records.OrderBy(r => r.Date), pageNumber ?? 1, _pageSize));
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(new EventId(), ex, ex.Message);
            }

            ViewData["RedirectionInfo"] = "В базе нет доступных архивных записей, но Вы можете загрузить новые из excel-файлов";
            return View("ArchiveLoader");
        }

        [HttpGet]
        public IActionResult ArchiveLoader()
        {
            ViewData["RedirectionInfo"] = "Внести данные в базу из excel-файлов:";
            return View(new FilesViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ArchiveLoader(List<IFormFile> files)
        {
            try
            {
                var weatherArchive = await _excelParser.ParseAsync(files);

                if (weatherArchive.Count > 0)
                {
                    _repository.AddRecords(weatherArchive);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while parsing excel - {ex.Message}");
            }

            return RedirectToAction("Index");
        }
    }
}
