using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Data.Common;

namespace DS_Test.Models.Database
{
    public class MainContext: DbContext
    {
        public DbSet<WeatherRecord> WeatherRecords { get; set; }
        public MainContext(DbContextOptions<MainContext> options) : base(options) 
        {
        }

    }
}
