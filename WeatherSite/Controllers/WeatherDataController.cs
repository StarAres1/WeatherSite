using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using System.Globalization;
using WeatherSite.Models;
using System;
using System.IO;
using WeatherSite.Data;
using NPOI.HSSF.Record;
using Microsoft.AspNetCore.Http;

namespace WeatherSite.Controllers
{
    public class WeatherDataController : Controller
    {
        private readonly AppDbContext _context;

        public WeatherDataController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Getting()
        {
            return View();
        }

        public IActionResult Adding()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile[] files)
        {
            if (files == null)
            {
                return BadRequest("Не добавлено ни одного файла!");
            }

            foreach (IFormFile file in files)
            {
                if (file == null || file.Length == 0)
                {
                    continue;
                }

                if (file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        stream.Position = 0;

                        using (var fs = new XSSFWorkbook(stream))
                        {
                            int numberOfSheets = fs.NumberOfSheets;
                            for (int j = 0; j < numberOfSheets; j++)
                            {
                                var sheet = fs.GetSheetAt(j);


                                for (int i = 4; i < sheet.LastRowNum; i++)
                                {
                                    var row = sheet.GetRow(i);

                                    if (row != null)
                                    {
                                        var dateCell = row.GetCell(0);
                                        var timeCell = row.GetCell(1);
                                        var tCell = row.GetCell(2);
                                        var humidityCell = row.GetCell(3);
                                        var tdCell = row.GetCell(4);
                                        var pressureCell = row.GetCell(5);
                                        var directionCell = row.GetCell(6);
                                        var velocityCell = row.GetCell(7);
                                        var cloudCoverCell = row.GetCell(8);
                                        var hCell = row.GetCell(9);
                                        var vvCell = row.GetCell(10);
                                        var descriptionCell = row.GetCell(11);

                                        var report = new Report();

                                        if (dateCell != null && dateCell.CellType != CellType.Blank)
                                        {
                                            var date = dateCell.StringCellValue.Trim();
                                            if (DateTime.TryParse(date, out var parsedDate))
                                            {
                                                report.Date = DateOnly.FromDateTime(parsedDate);
                                            }
                                            else
                                            {
                                                Console.WriteLine($"Строка {i + 1} содержит некорректный текстовый формат даты: '{date}'");
                                                continue;
                                            }

                                        }
                                        else
                                        {
                                            Console.WriteLine($"Строка {i + 1} не содержит даты!");
                                            continue;
                                        }

                                        if (timeCell != null && timeCell.CellType != CellType.Blank)
                                        {
                                            var time = timeCell.ToString();
                                            if (TimeOnly.TryParseExact(time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly parsedTime))
                                            {
                                                report.Time = parsedTime;
                                            }
                                            else
                                            {
                                                Console.WriteLine($"Строка {i + 1} содержит неизвестный формат времени!");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Строка {i + 1} не содержит времени!");
                                            continue;
                                        }

                                        if (tCell != null && tCell.CellType != CellType.Blank)
                                        {
                                            try
                                            {
                                                report.Temperature = (short)tCell.NumericCellValue;
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine($"Строка {i + 1} не удалось записать температуру в бд! {e.Message}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Строка {i + 1} не содержит температуры!");
                                            continue;
                                        }
                                        if (humidityCell != null && humidityCell.CellType != CellType.Blank)
                                        {
                                            try
                                            {
                                                report.Humidity = (byte)humidityCell.NumericCellValue;
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine($"Строка {i + 1} не удалось записать влажность в бд! {e.Message}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Строка {i + 1} не содержит влажности!");
                                            continue;
                                        }

                                        if (tdCell != null && tdCell.CellType != CellType.Blank)
                                        {
                                            try
                                            {
                                                report.Td = (short)tdCell.NumericCellValue;
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine($"Строка {i + 1} не удалось записать Td в бд! {e.Message}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Строка {i + 1} не содержит Td!");
                                            continue;
                                        }

                                        if (pressureCell != null && pressureCell.CellType != CellType.Blank)
                                        {
                                            try
                                            {
                                                report.Pressure = (ushort)pressureCell.NumericCellValue;
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine($"Строка {i + 1} не удалось записать давление в бд! {e.Message}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Строка {i + 1} не содержит давления!");
                                            continue;
                                        }
                                        if (directionCell != null && directionCell.CellType != CellType.Blank)
                                        {
                                            try
                                            {
                                                report.DirectionWind = directionCell.ToString();
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine($"Строка {i + 1} не удалось записать направление ветра в бд! {e.Message}");
                                            }
                                        }
                                        else
                                        {
                                            report.DirectionWind = null;
                                        }

                                        if (velocityCell != null && velocityCell.CellType != CellType.Blank)
                                        {
                                            try
                                            {
                                                report.VelocityWind = (byte)velocityCell.NumericCellValue;
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine($"Строка {i + 1} не удалось записать скорость ветра в бд! {e.Message}");
                                            }
                                        }
                                        else
                                        {
                                            report.VelocityWind = null;
                                        }

                                        if (cloudCoverCell != null && cloudCoverCell.CellType != CellType.Blank)
                                        {
                                            try
                                            {
                                                report.CloudCover = (byte)cloudCoverCell.NumericCellValue;
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine($"Строка {i + 1} не удалось записать облачность в бд! {e.Message}");
                                            }
                                        }
                                        else
                                        {
                                            report.CloudCover = null;
                                        }

                                        if (hCell != null && hCell.CellType != CellType.Blank)
                                        {
                                            try
                                            {
                                                report.H = (ushort)hCell.NumericCellValue;
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine($"Строка {i + 1} не удалось записать h в бд! {e.Message}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Строка {i + 1} не содержит h!");
                                            continue;
                                        }

                                        if (vvCell != null && vvCell.CellType != CellType.Blank)
                                        {
                                            try
                                            {
                                                report.VV = (byte)vvCell.NumericCellValue;
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine($"Строка {i + 1} не удалось записать vv в бд! {e.Message}");
                                            }
                                        }
                                        else
                                        {
                                            report.VV = null;
                                        }

                                        if (descriptionCell != null && descriptionCell.CellType != CellType.Blank)
                                        {
                                            try
                                            {
                                                report.Description = descriptionCell.ToString();
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine($"Строка {i + 1} не удалось записать описание в бд! {e.Message}");
                                            }
                                        }
                                        else
                                        {
                                            report.Description = null;
                                        }

                                        try
                                        {
                                            _context.Reports.Add(report);
                                            _context.SaveChanges();
                                        }
                                        catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                                        {
                                            Console.WriteLine($"Строка {i + 1} такой первичный ключ уже есть! {e.Message}\"");
                                        }
                                    }


                                    else
                                    {
                                        Console.WriteLine($"В строке {i + 1} ничего нет!");
                                    }
                                }

                            }
                        }
                    }
                }
                else
                {
                    return BadRequest("Неверный формат файла");
                }
            }
            return View("~/Views/Home/Success.cshtml");
        }
    }
}
