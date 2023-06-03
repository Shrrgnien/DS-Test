using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DS_Test.Models
{
    public class WeatherRecord
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double? Temperature { get; set; }
        public double? AirHumidity { get; set; }
        public double? Td { get; set; }
        public double? Pressure { get; set; }
        public string? AirFlowDirection { get; set; }
        public double? AirFlowSpeed { get; set; }
        public double? Cloudiness { get; set; }
        public double? h { get; set; }
        public double? VV { get; set; }
        public string? WeatherEvents { get; set; }
    }
}
