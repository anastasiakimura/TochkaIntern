using System.Text.RegularExpressions;

class HotelCapacity
{
    private static readonly char DateItemsSeparator = '-';
    private static readonly int DayIndex = 2;
    private static readonly int MonthIndex = 1;
    private static readonly int YearIndex = 0;

    static bool CheckCapacity(int maxCapacity, List<Guest> guests)
    {
        var departureSchedule = GetDepartureSchedule(guests);
        departureSchedule.Sort((x, y) => x.date.CompareTo(y.date));
        var count = 0;
        for (var index = 0; index < departureSchedule.Count; index++)
        {
            count += departureSchedule[index].count;
            if (index + 1 < departureSchedule.Count &&
                departureSchedule[index].date == departureSchedule[index + 1].date)
            {
                continue;
            }

            if (count > maxCapacity)
            {
                return false;
            }
        }

        return true;
    }

    private static List<(int date, int count)> GetDepartureSchedule(List<Guest> guests)
    {
        var departureSchedule = new List<(int date, int count)>();
        foreach (var guest in guests)
        {
            departureSchedule.Add((ConvertDate(guest.CheckIn), 1));
            departureSchedule.Add((ConvertDate(guest.CheckOut), -1));
        }

        return departureSchedule;
    }

    private static int ConvertDate(string date)
    {
        var parsedDate = date.Split(DateItemsSeparator)
            .Select(int.Parse)
            .ToArray();
        return parsedDate[YearIndex] * 10000 + parsedDate[MonthIndex] * 100 + parsedDate[DayIndex];
    }

    class Guest
    {
        public string Name { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
    }


    static void Main()
    {
        int maxCapacity = int.Parse(Console.ReadLine());
        int n = int.Parse(Console.ReadLine());


        List<Guest> guests = new List<Guest>();


        for (int i = 0; i < n; i++)
        {
            string line = Console.ReadLine();
            Guest guest = ParseGuest(line);
            guests.Add(guest);
        }


        bool result = CheckCapacity(maxCapacity, guests);


        Console.WriteLine(result ? "True" : "False");
    }


    // Простой парсер JSON-строки для объекта Guest
    static Guest ParseGuest(string json)
    {
        var guest = new Guest();


        // Извлекаем имя
        Match nameMatch = Regex.Match(json, "\"name\"\\s*:\\s*\"([^\"]+)\"");
        if (nameMatch.Success)
            guest.Name = nameMatch.Groups[1].Value;


        // Извлекаем дату заезда
        Match checkInMatch = Regex.Match(json, "\"check-in\"\\s*:\\s*\"([^\"]+)\"");
        if (checkInMatch.Success)
            guest.CheckIn = checkInMatch.Groups[1].Value;


        // Извлекаем дату выезда
        Match checkOutMatch = Regex.Match(json, "\"check-out\"\\s*:\\s*\"([^\"]+)\"");
        if (checkOutMatch.Success)
            guest.CheckOut = checkOutMatch.Groups[1].Value;


        return guest;
    }
}