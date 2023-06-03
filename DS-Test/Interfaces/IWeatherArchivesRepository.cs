using DS_Test.Models;

namespace DS_Test.Interfaces
{
    public interface IWeatherArchivesRepository
    {
        List<WeatherRecord> GetAllRecords();
        Task AddRecordsAsync(IEnumerable<WeatherRecord> records);
        Dictionary<DateTime, WeatherRecord> GetRecordsDictionary();
        IQueryable<WeatherRecord> GetRecordsAsQueryable();
    }
}
