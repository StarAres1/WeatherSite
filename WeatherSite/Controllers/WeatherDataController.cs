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
using Microsoft.EntityFrameworkCore;

namespace WeatherSite.Controllers
{
    public class WeatherDataController : Controller
    {
        private readonly AppDbContext _context;

        public WeatherDataController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Getting(int year = 0, int month = 0, int page = 1)
        {
            // Количество записей на странице
            int pageSize = 50;

            // Базовый запрос
            var query = _context.Reports.AsQueryable();

            // Фильтрация по году
            if (year != 0)
            {
                query = query.Where(r => r.Date.Year == year);
            }

            // Фильтрация по месяцу
            if (month != 0)
            {
                query = query.Where(r => r.Date.Month == month);
            }

            // Вычисляем общее количество записей
            int totalRecords = query.Count();

            // Выборка данных с учетом пагинации
            var reports = query
                .OrderBy(r => r.Date) // Сортировка по дате
                .Skip((page - 1) * pageSize) // Пропустить записи предыдущих страниц
                .Take(pageSize) // Взять записи текущей страницы
                .ToList();

            // Передаем выбранные значения в ViewBag
            ViewBag.SelectedYear = year;
            ViewBag.SelectedMonth = month;

            // Передаем данные для пагинации
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return View(reports);
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

            var reportsToAdd = new List<Report>();

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
                                Console.WriteLine($"Парсинг {j}");
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

                                        if (dateCell != null)
                                        {
                                            var date = dateCell.StringCellValue;
                                            if (DateTime.TryParse(date, out var parsedDate))
                                            {
                                                report.Date = DateOnly.FromDateTime(parsedDate);
                                            }
                                            else
                                            {
                                                //Console.WriteLine($"Строка {i + 1} содержит некорректный текстовый формат даты: '{date}'");
                                                continue;
                                            }

                                        }
                                        else
                                        {
                                            //Console.WriteLine($"Строка {i + 1} не содержит даты!");
                                            continue;
                                        }

                                        if (timeCell != null)
                                        {
                                            var time = timeCell.ToString();
                                            if (TimeOnly.TryParseExact(time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly parsedTime))
                                            {
                                                report.Time = parsedTime;
                                            }
                                            else
                                            {
                                                //Console.WriteLine($"Строка {i + 1} содержит неизвестный формат времени!");
                                            }
                                        }
                                        else
                                        {
                                            //Console.WriteLine($"Строка {i + 1} не содержит времени!");
                                            continue;
                                        }

                                        if (tCell != null)
                                        {
                                            report.Temperature = (short)tCell.NumericCellValue;
                                        }
                                        else
                                        {
                                            //Console.WriteLine($"Строка {i + 1} не содержит температуры!");
                                            continue;
                                        }
                                        if (humidityCell != null)
                                        {
                                            report.Humidity = (byte)humidityCell.NumericCellValue;
                                        }
                                        else
                                        {
                                            //Console.WriteLine($"Строка {i + 1} не содержит влажности!");
                                            continue;
                                        }

                                        if (tdCell != null)
                                        {
                                            report.Td = (short)tdCell.NumericCellValue;
                                        }
                                        else
                                        {
                                            //Console.WriteLine($"Строка {i + 1} не содержит Td!");
                                            continue;
                                        }

                                        if (pressureCell != null)
                                        {
                                            report.Pressure = (ushort)pressureCell.NumericCellValue;
                                        }
                                        else
                                        {
                                            //Console.WriteLine($"Строка {i + 1} не содержит давления!");
                                            continue;
                                        }
                                        if (directionCell != null)
                                        {
                                            report.DirectionWind = directionCell.ToString();
                                        }
                                        else
                                        {
                                            report.DirectionWind = null;
                                        }

                                        if (velocityCell != null && double.TryParse(velocityCell.ToString(), out var numericValue3))
                                        {
                                            report.VelocityWind = (byte)numericValue3;
                                        }
                                        else
                                        {
                                            report.VelocityWind = null;
                                        }

                                        if (cloudCoverCell != null && double.TryParse(cloudCoverCell.ToString(), out var numericValue2))
                                        {
                                            report.CloudCover = (byte)numericValue2;
                                        }
                                        else
                                        {
                                            report.CloudCover = null;
                                        }

                                        if (hCell != null && double.TryParse(hCell.ToString(), out var numericValue))
                                        {
                                            report.H = (ushort)numericValue;
                                        }
                                        else
                                        {
                                            //Console.WriteLine($"Строка {i + 1} не содержит h!");
                                            continue;
                                        }

                                        if (vvCell != null && double.TryParse(vvCell.ToString(), out var numericValue1))
                                        {
                                            report.VV = (byte)numericValue1;                                            
                                        }
                                        else
                                        {
                                            report.VV = null;
                                        }

                                        if (descriptionCell != null)
                                        {
                                            report.Description = descriptionCell.ToString();
                                        }
                                        else
                                        {
                                            report.Description = null;
                                        }

                                        reportsToAdd.Add(report);
                                    }
                                    else
                                    {
                                        //Console.WriteLine($"В строке {i + 1} ничего нет!");
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
            try
            {
                Console.WriteLine("Запись в бд");
                _context.Reports.AddRange(reportsToAdd);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                return BadRequest("Не удалось добавить записи в бд");
            }
            Console.WriteLine("Финиш");
            return View("~/Views/Home/Success.cshtml");
        }
    }
}


// Можно заносить каждую строку в бд по отдельности, а не массивом. Но в таком случае мы будем проигрывать в
//скорости. Однако в версии реализованной выше есть один недостаток, если хотя бы одна запись будет отклонена 
// на стороне бд (например, из-за повторяющегося первичного ключа), то ничего не будет записана в бд.
// В случае построчной записи не будет  записана только, неподходящая строка


//try
//{
//    _context.Reports.Add(report);
//    _context.SaveChanges();
//}
//catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
//{
//    //Console.WriteLine($"Строка {i + 1} такой первичный ключ уже есть! {e.Message}\"");
//}