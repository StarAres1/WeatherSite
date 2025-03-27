using System.ComponentModel.DataAnnotations;
using System;

namespace WeatherSite.Models
{
    public class Report
    {
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public short Temperature {  get; set; }
        public byte Humidity { get; set; }
        public short Td {  get; set; }
        public ushort Pressure { get; set; }
        public string? DirectionWind { get; set; }
        public byte? VelocityWind { get; set; }
        public byte? CloudCover { get; set; }
        public ushort? H { get; set; }
        public byte? VV { get; set; }
        public string? Description {  get; set; }
        

    }
}
