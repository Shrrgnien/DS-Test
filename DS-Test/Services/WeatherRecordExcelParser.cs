using DS_Test.Interfaces;
using DS_Test.Models;
using Microsoft.IdentityModel.Tokens;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DS_Test.Services
{
    public class WeatherRecordExcelParser : IExcelParser<WeatherRecord>
    {
        private static string _folderPath = "Data";
        private ILogger<WeatherRecordExcelParser> _logger;
        private IWeatherArchivesRepository _repository;
        private string? _folderLocation;
        private string? _fileName;

        public WeatherRecordExcelParser(ILogger<WeatherRecordExcelParser> logger, IWeatherArchivesRepository repository, string? folderLocation = null, string? fileName = null)
        {
            _logger = logger;
            _repository = repository;
            _folderLocation = folderLocation;
            _fileName = fileName;
        }

        public async Task<List<WeatherRecord>> ParseAsync(IFormFile file)
        {
            List<WeatherRecord> weatherArchive = new();
            try
            {
                var fileLocation = Path.Combine(Environment.CurrentDirectory,
                    _folderLocation ?? _folderPath, _fileName ?? file.FileName);

                using (FileStream fileStream = new(fileLocation, FileMode.Create, FileAccess.ReadWrite))
                {
                    await file.CopyToAsync(fileStream);
                    fileStream.Position = 0;

                    weatherArchive = ParseXSSFWorkbook(new XSSFWorkbook(fileStream));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return weatherArchive;
        }
        public async Task<List<WeatherRecord>> ParseAsync(IEnumerable<IFormFile> files)
        {
            List<WeatherRecord> records = new();
            foreach(var file in files)
            {
                records.AddRange(await ParseAsync(file));
            }
            return records;
        }

        private List<WeatherRecord> ParseXSSFWorkbook(XSSFWorkbook workBook)
        {
            var savedRecords = _repository.GetRecordsDictionary();

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
                            var record = ParseRow(row, date);
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

        private WeatherRecord ParseRow(IRow row, DateTime date)
        {
            return new WeatherRecord
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
                if (!res.IsNullOrEmpty() && !string.IsNullOrWhiteSpace(res))
                {
                    double.TryParse(res, out var val);
                    return val;
                }
            }
            return null;
        }
    }
}
