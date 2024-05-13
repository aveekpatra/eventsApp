using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

public class CalendarApp
{
    public DateTime Date { get; set; }
    public string Description { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to Events");
        while (true)
        {
            List<CalendarApp> events = LoadEvents("events.txt");
            
            Console.WriteLine("\nChoose an option:");
            Console.WriteLine("1. Display nearest future events (today and tomorrow)");
            Console.WriteLine("2. Show schedule for the current week (including past events)");
            Console.WriteLine("3. Display future events until the end of the month");
            Console.WriteLine("4. Display all events");
            Console.WriteLine("5. Add a new event");
            Console.WriteLine("6. Edit events");
            Console.WriteLine("7. Quit the application or type 'quit'");
            Console.WriteLine("");

            string option = Console.ReadLine();
            Console.WriteLine("");

            if (option == "1")
                DisplayNearestEvents(events);
            else if (option == "2")
                DisplayWeeklySchedule(events);
            else if (option == "3")
                DisplayMonthlyEvents(events);
            else if (option == "4")
                DisplayAllEvents(events);
            else if (option == "5")
                AddNewEvent(events);
            else if (option == "6")
                EditTasks();
            else if (option == "7" || option == "quit")
                break;
            else
                Console.WriteLine("Invalid option. Please try again.");

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadKey();
        }

        Console.WriteLine("Thank you for using this app. \nMade with 🩷 by Aveek Patra");
        Console.ReadKey();
    }

    static List<CalendarApp> LoadEvents(string filename)
    {
        List<CalendarApp> events = new List<CalendarApp>();
        try
        {
            string[] lines = File.ReadAllLines(filename);

            foreach (string line in lines)
            {
                string[] parts = line.Split(';');
                DateTime date = new DateTime(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
                string input = parts[3];
                if (input.Length >= 4)
                {
                    int hours = int.Parse(input.Substring(1, 2));
                    int minutes = int.Parse(input.Substring(3, 2));
                    TimeSpan time = new TimeSpan(hours, minutes, 0);
                    date = date.Add(time);
                }
                else
                {
                    Console.WriteLine("Invalid time format in the events file.");
                }
                
                string description = parts[4].Trim();

                events.Add(new CalendarApp { Date = date, Description = description });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading events from {filename}: {ex.Message}");
        }

        return events.OrderBy(e => e.Date).ToList();
    }


    static void DisplayNearestEvents(List<CalendarApp> events)
    {
        Console.WriteLine("\nNearest events:");
        DateTime now = DateTime.Now;
        DateTime tomorrow = now.AddDays(1);

        IEnumerable<CalendarApp> nearestEvents = events.Where(e => e.Date >= now && e.Date <= tomorrow);

        foreach (CalendarApp e in nearestEvents)
        {
            Console.WriteLine($"{e.Date}: {e.Description}");
        }
    }

    static void DisplayWeeklySchedule(List<CalendarApp> events)
    {
        Console.WriteLine("\nWeekly events:");
        DateTime now = DateTime.Now;
        DateTime startOfWeek = now.AddDays(-(int)now.DayOfWeek);
        DateTime endOfWeek = startOfWeek.AddDays(7);

        IEnumerable<CalendarApp> weeklyEvents = events.Where(e => e.Date >= startOfWeek && e.Date <= endOfWeek);

        foreach (CalendarApp e in weeklyEvents)
        {
            Console.WriteLine($"{e.Date}: {e.Description}");
        }
    }

    static void DisplayMonthlyEvents(List<CalendarApp> events)
    {
        Console.WriteLine("\nMonthly events:");
        DateTime now = DateTime.Now;
        DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
        DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        IEnumerable<CalendarApp> monthlyEvents = events.Where(e => e.Date >= startOfMonth && e.Date <= endOfMonth);

        foreach (CalendarApp e in monthlyEvents)
        {
            Console.WriteLine($"{e.Date}: {e.Description}");
        }
    }

    static void DisplayAllEvents(List<CalendarApp> events)
    {
        Console.WriteLine("\nAll events:");

        foreach (CalendarApp e in events)
        {
            Console.WriteLine($"{e.Date}: {e.Description}");
        }
    }

    static void AddNewEvent(List<CalendarApp> events)
    {
        DateTime date;
        while (true)
        {
            Console.WriteLine("Enter event date (yyyy mm dd):");
            string[] dateParts = Console.ReadLine().Split(' ');
            if (dateParts.Length == 3 && 
                int.TryParse(dateParts[0], out int year) && 
                int.TryParse(dateParts[1], out int month) && 
                int.TryParse(dateParts[2], out int day) && 
                DateTime.TryParse($"{year}-{month}-{day}", out date))
            {
                break;
            }
            Console.WriteLine("Invalid date. Please try again.");
        }

        TimeSpan time;
        while (true)
        {
            Console.WriteLine("Enter event time (hhmm). Don't use semicolon!! :");
            if (TimeSpan.TryParseExact(Console.ReadLine(), "hhmm", null, out time))
            {
                break;
            }
            Console.WriteLine("Invalid time. Please try again.");
        }
        date = date.Add(time);

        Console.WriteLine("Enter event description:");
        string description = Console.ReadLine();

        events.Add(new CalendarApp { Date = date, Description = description });

        // Save the new event to the file
        File.AppendAllText("events.txt",
            $"\n{date.Year}; {date.Month}; {date.Day}; {time.TotalMinutes}; {description}");
    }


    static void EditTasks()
    {
        string filePath = "events.txt";

        try
        {
            // Open with default text editor
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error opening {filePath}: {ex.Message}");
        }
    }
}
