using DS_Test.Models;
using DS_Test.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Diagnostics;
using System.Globalization;

namespace DS_Test.Controllers
{
    public class WeatherArchivesController : Controller
    {
        private ILogger<WeatherArchivesController> _logger;
        private MainContext _context;
        private static int _pageSize = 60;
        private static string _folderPath = "Data";
        public WeatherArchivesController(ILogger<WeatherArchivesController> logger, MainContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber, string month = "Выберите месяц", string year = "Выберите год")
        {
            ViewData["SelectedMonth"] = month;
            ViewData["SelectedYear"] = year;
            try
            {
                if(_context.WeatherRecords?.Count() > 0)
                {
                    if (DateTime.TryParseExact(year, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var y))
                    {
                        if (DateTime.TryParseExact(month, "MMMM", CultureInfo.GetCultureInfo("ru"), DateTimeStyles.None, out var m))
                        {
                            return View(await PaginatedList<WeatherRecord>.CreateAsync(_context.WeatherRecords
                                .Where(r => r.Date.Year == y.Year && r.Date.Month == m.Month)
                                .OrderBy(r => r.Date).AsQueryable(), pageNumber ?? 1, _pageSize));
                        }
                        else
                        {
                            return View(await PaginatedList<WeatherRecord>.CreateAsync(_context.WeatherRecords
                                .Where(r => r.Date.Year == y.Year)
                                .OrderBy(r => r.Date)
                                .AsQueryable(), pageNumber ?? 1, _pageSize));
                        }
                    }
                    else if (DateTime.TryParseExact(month, "MMMM", CultureInfo.GetCultureInfo("ru"), DateTimeStyles.None, out var m))
                    {
                        return View(await PaginatedList<WeatherRecord>.CreateAsync(_context.WeatherRecords
                            .Where(r => r.Date.Month == m.Month)
                            .OrderBy(r => r.Date)
                            .AsQueryable(), pageNumber ?? 1, _pageSize));
                    }
                    return View(await PaginatedList<WeatherRecord>
                        .CreateAsync(_context.WeatherRecords.OrderBy(r => r.Date).AsQueryable(), pageNumber ?? 1, _pageSize));
                }

            }
            catch(Exception ex)
            {
                _logger.LogError(new EventId(), ex, ex.Message);
            }
            return View("ArchiveLoader");
        }

        [HttpGet]
        public IActionResult ArchiveLoader()
        {
            return View(new FilesViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ArchiveLoader(List<IFormFile> files)
        {
            var weatherArchive = new List<WeatherRecord>();

            foreach (var file in files)
            {
                try
                {
                    var fileLocation = Path.Combine(Environment.CurrentDirectory, _folderPath, file.FileName);

                    using (FileStream fileStream = new(fileLocation, FileMode.Create, FileAccess.ReadWrite))
                    {
                        await file.CopyToAsync(fileStream);
                        fileStream.Position = 0;

                        weatherArchive = ParseXSSFWorkbook(new XSSFWorkbook(fileStream));
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogWarning(ex.Message);
                }
            }

            await _context.WeatherRecords.AddRangeAsync(weatherArchive);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private List<WeatherRecord> ParseXSSFWorkbook(XSSFWorkbook workBook)
        {
            var savedRecords = _context.WeatherRecords.GroupBy(r => r.Date).ToDictionary(g => g.Key, g => g.FirstOrDefault());
            List<WeatherRecord> records = new();

            for (int i = 0; i < workBook.NumberOfSheets; i++)
            {
                var sheet = workBook.GetSheetAt(i);

                for (int j = 4; j <= sheet.LastRowNum; j++)
                {
                    var row = sheet.GetRow(j);

                    try
                    {
                        var date = DateTime.Parse(row.GetCell(0).StringCellValue + " " + row.GetCell(1).StringCellValue);

                        if (!savedRecords.ContainsKey(date))
                        {
                            var record = new WeatherRecord
                            {
                                Date = date,
                                Temperature = GetDouble(row.GetCell(2)),
                                AirHumidity = GetDouble(row.GetCell(3)),
                                Td = GetDouble(row.GetCell(4)),
                                Pressure = GetDouble(row.GetCell(5)),
                                AirFlowDirection = row.GetCell(6).StringCellValue,
                                AirFlowSpeed = GetDouble(row.GetCell(7)),
                                Cloudiness = GetDouble(row.GetCell(8)),
                                h = GetDouble(row.GetCell(9)),
                                VV = GetDouble(row.GetCell(10)),
                                WeatherEvents = row.GetCell(11)?.StringCellValue
                            };
                            records.Add(record);
                            savedRecords.Add(date, record);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex.Message);
                    }
                }
            }
            return records;
        }
        private double? GetDouble(ICell cell)
        {
            if (cell.CellType == CellType.Numeric)
            {
                return cell.NumericCellValue;
            }
            else if (cell.CellType == CellType.String)
            {
                var res = cell.StringCellValue;
                if (!res.IsNullOrEmpty() && string.IsNullOrWhiteSpace(res))
                {
                    double.TryParse(res, out var val);
                    return val;
                }
            }
            return null;
        }
    }
}
