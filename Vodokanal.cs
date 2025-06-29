using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WaterUtilitySystem
{
    // Класс для мониторинга насосной станции водоканала
    public class PumpStationMonitor
    {
        private List<PumpData> _pumpData = new List<PumpData>();
        private const string LogFile = "pump_logs.csv";  // Файл с данными датчиков
        
        // Загрузка данных из файла журналов
        public void LoadData()
        {
            if (!File.Exists(LogFile)) return;
            
            try 
            {
                var lines = File.ReadAllLines(LogFile);
                foreach (var line in lines.Skip(1))  // Пропуск заголовка
                {
                    var values = line.Split(';');
                    _pumpData.Add(new PumpData {
                        Timestamp = DateTime.Parse(values[0]),  // Время измерения
                        Pressure = double.Parse(values[1]),     // Давление (бар)
                        FlowRate = double.Parse(values[2]),     // Расход (м³/ч)
                        Status = (PumpStatus)Enum.Parse(typeof(PumpStatus), values[3])
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки: {ex.Message}");
            }
        }

        // Анализ и вывод статистики работы
        public void AnalyzePerformance()
        {
            if (_pumpData.Count == 0)
            {
                Console.WriteLine("Нет данных для анализа");
                return;
            }

            Console.WriteLine("Анализ работы насосной станции:");
            Console.WriteLine($"Всего записей: {_pumpData.Count}");
            Console.WriteLine($"Среднее давление: {_pumpData.Average(p => p.Pressure):F2} бар");
            Console.WriteLine($"Макс. расход: {_pumpData.Max(p => p.FlowRate):F2} м³/ч");
            
            // Подсчет аварийных ситуаций
            var alarms = _pumpData.Count(p => p.Status == PumpStatus.Alarm);
            Console.WriteLine($"Аварийных ситуаций: {alarms}");
        }

        // Проверка текущего состояния системы
        public void CheckCurrentStatus()
        {
            var last = _pumpData.LastOrDefault();
            if (last == null) return;
            
            Console.WriteLine("\nТекущий статус:");
            Console.WriteLine($"Давление: {last.Pressure:F1} бар");
            Console.WriteLine($"Расход: {last.FlowRate:F1} м³/ч");
            Console.WriteLine($"Состояние: {GetStatusDescription(last.Status)}");
            
            if (last.Pressure > 5.5)  // Проверка критического давления
                Console.WriteLine("ВНИМАНИЕ: Высокое давление!");
        }

        // Преобразование статуса в текст
        private string GetStatusDescription(PumpStatus status)
        {
            return status switch {
                PumpStatus.Normal => "Норма",
                PumpStatus.Warning => "Предупреждение",
                PumpStatus.Alarm => "Авария",
                _ => "Неизвестно"
            };
        }
    }

    // Класс для хранения данных насоса
    public class PumpData
    {
        public DateTime Timestamp { get; set; }  // Время замера
        public double Pressure { get; set; }     // Текущее давление
        public double FlowRate { get; set; }     // Скорость потока
        public PumpStatus Status { get; set; }   // Состояние оборудования
    }

    // Статусы работы насоса
    public enum PumpStatus { Normal, Warning, Alarm }

    class Program
    {
        static void Main()
        {
            var monitor = new PumpStationMonitor();
            monitor.LoadData();               // Загрузка данных
            monitor.AnalyzePerformance();     // Анализ
            monitor.CheckCurrentStatus();     // Проверка состояния
            
            Console.WriteLine("\nДля выхода нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}