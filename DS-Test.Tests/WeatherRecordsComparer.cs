using DS_Test.Models;
using System.Diagnostics.CodeAnalysis;

namespace DS_Test.Tests
{
    public class WeatherRecordsComparer : IEqualityComparer<WeatherRecord>
    {
        public bool Equals(WeatherRecord? x, WeatherRecord? y)
        {
            if(x == null || y == null)
                return false;
            return 
                x.Date == y.Date &&
                x.WeatherEvents == y.WeatherEvents &&
                x.Cloudiness == y.Cloudiness &&
                x.Temperature == y.Temperature &&
                x.Pressure == y.Pressure &&
                x.h == y.h &&
                x.AirFlowDirection == y.AirFlowDirection &&
                x.AirFlowSpeed == y.AirFlowSpeed &&
                x.AirHumidity == y.AirHumidity &&
                x.Td == y.Td &&
                x.VV == y.VV;
        }

        public int GetHashCode([DisallowNull] WeatherRecord obj)
        {
            throw new NotImplementedException();
        }
    }
}