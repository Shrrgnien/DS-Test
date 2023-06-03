using DS_Test.Interfaces;
using DS_Test.Models;
using DS_Test.Models.Database;

namespace DS_Test.Services
{
    public class WeatherArchivesRepository: IWeatherArchivesRepository
    {
        private ILogger<WeatherArchivesRepository> _logger;
        private MainContext _context;

        public WeatherArchivesRepository(ILogger<WeatherArchivesRepository> logger, MainContext context) 
        {
            _logger = logger;
            _context = context;
        }

        public async void AddRecords(IEnumerable<WeatherRecord> records)
        {
            await _context.WeatherRecords.AddRangeAsync(records);
            await _context.SaveChangesAsync();
        }

        public List<WeatherRecord> GetAllRecords()
        {
            return _context.WeatherRecords.ToList();
        }

        public IQueryable<WeatherRecord> GetRecordsAsQueryable()
        {
            return _context.WeatherRecords.AsQueryable();
        }

        public Dictionary<DateTime, WeatherRecord> GetRecordsDictionary()
        {
             return _context.WeatherRecords.GroupBy(r => r.Date).ToDictionary(g => g.Key, g => g.First());
        }
    }
}
