using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using WeatherSite.Models;

namespace WeatherSite.Controllers
{
    public class WeatherDataController : Controller
    {
        public IActionResult Getting()
        {
            return View();
        }

        public IActionResult Adding()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не добавлен");
            }            

            if (file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {                
                using(var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    stream.Position = 0;

                    using(var fs = new XSSFWorkbook(stream))
                    {
                        var sheet = fs.GetSheetAt(0);

                        
                        for (int i = 4; i < sheet.LastRowNum; i++)
                        {
                            var row = sheet.GetRow(0);

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

                                if(dateCell != null && dateCell.CellType != CellType.Blank)
                                {

                                    if (DateUtil.IsCellDateFormatted(dateCell))
                                    {
                                        var date = dateCell.DateCellValue;

                                        try
                                        {
                                            report.Date = DateOnly.FromDateTime(date.Value);
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine($"Ошибка при записи строки {i + 1} {e.Message}");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Строка {i + 1} содержит неизвестный формат даты!");
                                        continue;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Строка {i + 1} не содержит даты!");
                                    continue;
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
            else
            {
                return BadRequest("Неверный формат файла");
            }
        }
    }
}
