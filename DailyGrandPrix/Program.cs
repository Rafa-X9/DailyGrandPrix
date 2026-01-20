using DailyGrandPrix.Entities;
using DailyGrandPrix.Enums;
using System;

namespace DailyGrandPrix
{
    class Program
    {
        public static void Main(string[] args)
        {
            List<Driver> Drivers = new List<Driver>();

            while (true)
            {
                Console.Clear();

                Console.WriteLine("(1) Add drivers manually");
                Console.WriteLine("(2) Load drivers from files (personal only)");
                Console.WriteLine("(3) Load drivers from files (all)");
                Console.WriteLine("(4) Save drivers in database");
                Console.WriteLine("(5) Check driver's championship stats");
                Console.WriteLine("(6) Start race");
                Console.WriteLine("(7) Make moves");
                Console.WriteLine("(8) Check race stats");
                Console.WriteLine("(9) Award points");
                Console.WriteLine("(10) Generate usernames for pings");
                Console.WriteLine("(11) Generate log");
                Console.WriteLine();

                Console.Write("Choose: ");
                int choice = int.Parse(Console.ReadLine());

                if (choice == 1)
                {
                    Services.CreateMultipleDrivers();
                }
                else if (choice == 2)
                {
                    Services.LoadDrivers(Drivers, true);
                }
                else if (choice == 3)
                {
                    Services.LoadDrivers(Drivers, false);
                }
                else if (choice == 4)
                {
                    Services.SaveDrivers(Drivers);
                }
                else if (choice == 5)
                {
                    Console.Clear();
                    Services.ShowChampionship(Drivers);
                    Console.ReadLine();
                }
                else if (choice == 6)
                {
                    Console.Write("First race? (y/n) ");
                    char ans = char.Parse(Console.ReadLine());
                    Services.StartRace(Drivers, ans == 'y' || ans == 'Y');
                }
                else if (choice == 7)
                {
                    Console.Clear();
                    Services.MakeMoves(Drivers);
                }
                else if (choice == 8)
                {
                    Console.Clear();
                    Services.ShowRace(Drivers);
                    Console.ReadLine();
                }
                else if (choice == 9)
                {
                    Services.AwardPoints(Drivers);
                }
                else if (choice == 10)
                {
                    foreach (Driver d in Drivers)
                    {
                        Console.WriteLine(d.Username);
                        Console.WriteLine();
                    }
                    Console.ReadLine();
                }
                else if (choice == 11)
                {
                    Services.GenerateLog(Drivers);
                }
            }
        }
    }
}