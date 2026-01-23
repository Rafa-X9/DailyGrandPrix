using DailyGrandPrix.Entities;
using DailyGrandPrix.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyGrandPrix
{
    static class Services
    {
        public static string Path = @"C:\Users\Lenovo\Desktop\Rafael\projetosCsharp\DailyGrandPrix\Database";
        public static int StepsPerLap = 34;
        public static int RaceLaps = 8;

        public static void CreateDriver(List<Driver> DriverList)
        {
            Console.Write("Enter driver's name: ");
            string name = Console.ReadLine();

            Console.Write("Enter driver's username: ");
            string username = Console.ReadLine();

            Console.Write("Enter driver's number: ");
            int number = int.Parse(Console.ReadLine());

            Console.Write("Enter driver's team: ");
            Teams team = Enum.Parse<Teams>(Console.ReadLine());

            DriverList.Add(new Driver(name, username, number, team));
            FileStream fs = new FileStream(Path + @"\" + name + ".txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine($"{name},{username},{number},{team}");

            sw.Close();
            fs.Close();
        }

        public static void CreateMultipleDrivers()
        {
            char ans = 'y';
            while (ans == 'y' || ans == 'Y')
            {
                List<Driver> drivers = new List<Driver>();
                CreateDriver(drivers);
                Console.Write("Continue? ");
                ans = char.Parse(Console.ReadLine());
            }
        }

        public static void LoadDrivers(List<Driver> DriverList, bool OnlyPersonalInfo)
        {
            string[] files = Directory.GetFiles(Path, "*.txt");

            foreach (string file in files)
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                StreamReader sr = new StreamReader(fs);

                DriverList.Add(new Driver(sr, OnlyPersonalInfo));

                sr.Close();
                fs.Close();
            }
        }

        public static void SaveDrivers(List<Driver> DriverList)
        {
            foreach (Driver d in DriverList)
            {
                StreamWriter sw = new StreamWriter(Path + @"\" + d.Name + ".txt", false);
                sw.WriteLine($"{d.Name},{d.Username},{d.Number},{d.Team}");
                sw.WriteLine($"{d.Tyres},{d.TyreWear},{d.TyreChanges},{d.Fuel}");
                sw.WriteLine($"{d.StepsDriven},{d.MovesMade}");
                sw.WriteLine($"{d.Points},{d.Wins},{d.Podiums}");
                sw.WriteLine($"{d.LastAction},{d.LastSteps}");
                foreach (int s in d.StepsHistory) sw.Write(s + ","); sw.WriteLine();
                sw.Close();
            }
        }

        public static int CalculateSteps(double tyredrag, double tyrelife, double fuellevel, bool IsPushing)
        {
            double CompFactor = 1 - (0.1 * (tyredrag - 1));
            double LifeFactor = tyrelife / 100;
            double FuelFactor = 1 - (fuellevel / 100);

            if (!IsPushing) return (int)Math.Floor((2.5 + (12.5 * (CompFactor * LifeFactor * (0.6 + (0.4 * FuelFactor))))) * 2.5);
            return (int)Math.Floor((2.5 + (12.5 * (CompFactor * LifeFactor * (0.6 + (0.4 * FuelFactor))))) * 3.25);
        }

        public static void SortByPosition(List<Driver> Drivers)
        {
            Drivers.Sort((d1, d2) => d2.StepsDriven.CompareTo(d1.StepsDriven));
        }

        public static void ShowRace(List<Driver> Drivers)
        {
            SortByPosition(Drivers);
            for (int i = 0; i < Drivers.Count; i++)
            {
                if (Drivers[i].LapsDriven < 0)
                {
                    Console.WriteLine($"{i + 1} POSITION");
                    Console.WriteLine($"{Drivers[i].Name} has retired");
                    Console.WriteLine();
                }
                else if (Drivers[i].LapsDriven >= RaceLaps)
                {
                    Console.WriteLine($"{i + 1} POSITION");
                    Console.WriteLine($"{Drivers[i].Name} has finished the race");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine($"{i + 1} POSITION");
                    Console.WriteLine($"{Drivers[i].Name} " +
                        $"\nLaps: {Drivers[i].LapsDriven} " +
                        $"\nSteps in lap: {Drivers[i].StepsInLap}" +
                        $"\nTyres: {Drivers[i].Tyres} ({Drivers[i].TyreWear}/100)" +
                        $"\nFuel: {Drivers[i].Fuel}/100" +
                        $"\n{Drivers[i].MovesMade} moves made");
                    Console.WriteLine();
                }
            }
        }

        public static void SortyByChampionship(List<Driver> Drivers)
        {
            Drivers.Sort((d1, d2) => d2.Points.CompareTo(d1.Points));
        }

        public static void ShowChampionship(List<Driver> Drivers)
        {
            SortyByChampionship(Drivers);
            Console.WriteLine("Championship Standings:");
            foreach (Driver d in Drivers)
            {
                Console.WriteLine($"{d.Points} points: {d.Name}");
            }
        }

        public static void AwardPoints(List<Driver> Drivers)
        {
            int[] points = { 25, 18, 15, 12, 10, 8, 6, 4, 2, 1 };
            for (int i = 0; i < Drivers.Count && i < points.Length; i++)
            {
                if (Drivers[i].LapsDriven < 0) break;
                Drivers[i].Points += points[i];
                if (i == 0) Drivers[i].Wins++;
                if (i < 3) Drivers[i].Podiums++;
            }
        }

        public static void StartRace(List<Driver> Drivers, bool StartSeason)
        {
            foreach (Driver d in Drivers)
            {
                Console.Write($"Insert {d.Name}'s ({d.Username}) compound: ");
                d.Tyres = Enum.Parse<Tyres>(Console.ReadLine());
                d.TyreWear = 100;
                d.TyreChanges = 0;
                Console.Write($"Insert {d.Name}'s starting amount of fuel: ");
                d.Fuel = int.Parse(Console.ReadLine());
                d.StepsDriven = 0;
                d.MovesMade = 0;
                d.LastSteps = 0;
                d.LastAction = Actions.None;

                if (StartSeason)
                {
                    d.Points = 0;
                    d.Wins = 0;
                    d.Podiums = 0;
                }
            }
        }

        public static void MakeMoves(List<Driver> Drivers)
        {
            while (true)
            {
                Console.Clear();
                SortByPosition(Drivers);
                for (int i = 0; i < Drivers.Count; i++)
                {
                    if (Drivers[i].LapsDriven >= 0 && Drivers[i].LapsDriven < RaceLaps)
                        Console.WriteLine($"{i} - {Drivers[i].Name} ({Drivers[i].Username}) - {Drivers[i].MovesMade} moves made");
                }
                Console.WriteLine();
                Console.Write("Choose driver, type -1 to quit: ");
                int choice = int.Parse(Console.ReadLine());
                if (choice == -1) return;
                Driver driver = Drivers[choice];
                Console.Write($"Is {driver.Name} moving, pushing, or boxing? (m,p,b) ");
                char action = char.Parse(Console.ReadLine());

                try
                {
                    if (action == 'm' || action == 'M')
                    {
                        driver.MakeStep(false);
                    }
                    else if (action == 'p' || action == 'P')
                    {
                        driver.MakeStep(true);
                    }
                    else if (action == 'b' || action == 'B')
                    {
                        Console.Write("To which tyre? ");
                        Tyres newtyre = Enum.Parse<Tyres>(Console.ReadLine());
                        driver.ChangeTyres(newtyre);
                    }
                    else
                    {
                        Console.WriteLine("Not recognized, move aborted");
                    }
                }
                catch (ApplicationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
               
            }
        }
    
        public static void GenerateLog(List<Driver> Drivers)
        {
            string path = Path + @"\Log\RaceLog.txt";
            StreamWriter sw = new StreamWriter(path, false);

            SortByPosition(Drivers);
            for (int i = 0; i < Drivers.Count; i++)
            {
                Driver d = Drivers[i];
                if (d.StepsDriven >= 0 && d.Fuel >= 0)
                    sw.WriteLine($"**P{i + 1} - {d.Name}** (/{d.Username}), {d.Team}");
                else sw.WriteLine($"**{d.Name}** (/{d.Username}), {d.Team}");
                sw.WriteLine();

                if (d.StepsDriven >= 0 && d.Fuel >= 0 && d.LapsDriven < RaceLaps)
                {
                    if (d.LastAction == Actions.Conserve)
                    {
                        sw.WriteLine($"{d.Name} conserved");
                        sw.WriteLine();
                        sw.WriteLine($"Steps: {d.StepsDriven - d.LastSteps} + " +
                            $"{d.LastSteps} = {d.StepsDriven}");
                        sw.WriteLine();
                    }
                    else if (d.LastAction == Actions.Push)
                    {
                        sw.WriteLine($"{d.Name} pushed");
                        sw.WriteLine();
                        sw.WriteLine($"Steps: {d.StepsDriven - d.LastSteps} + " +
                            $"{d.LastSteps} = {d.StepsDriven}");
                        sw.WriteLine();
                    }
                    else if (d.LastAction == Actions.Pit)
                    {
                        sw.WriteLine($"Went to the pits and changed to new {d.Tyres}");
                        sw.WriteLine();
                    }
                    sw.WriteLine($"Tyres: {d.Tyres} ({d.TyreWear}/100)");
                    sw.WriteLine();
                    sw.WriteLine($"Fuel: {d.Fuel}/100");
                    sw.WriteLine();
                    sw.WriteLine($"Laps driven: {d.LapsDriven}/{RaceLaps}");
                    sw.WriteLine();
                    sw.WriteLine($"Steps into this lap: {d.StepsInLap}");
                    sw.WriteLine();
                    sw.Write("Steps history: ");
                    foreach (int s in d.StepsHistory) sw.Write(s + " ");
                    sw.WriteLine();
                }
                else
                {
                    if (d.LapsDriven >= RaceLaps)
                    {
                        if (i == 0)
                        {
                            sw.WriteLine($"We have our winner! {d.Name} finishes first " +
                                $"and wins the DailyGrandPrix!");
                            sw.WriteLine();
                        }
                        else if (i == 1)
                        {
                            sw.WriteLine($"Right after {Drivers[0].Name} comes {d.Name} " +
                                $"finishing in second place in the DailyGrandPrix!");
                            sw.WriteLine();
                        }
                        else if (i == 2)
                        {
                            sw.WriteLine($"{d.Name} finishes in third place and " +
                                $"completes the podium of the DailyGrandPrix!");
                            sw.WriteLine();
                        }
                        else
                        {
                            sw.WriteLine($"{d.Name} finishes in {i}th place of the " +
                                $"DailyGrandPrix!");
                        }
                    }
                    else
                    {
                        sw.WriteLine($"{d.Name} has retired from the race");
                        sw.WriteLine();
                    }
                }
                sw.WriteLine("---");
                sw.WriteLine();
            }

            sw.WriteLine("^(This message and all of calculations " +
                "of this series are" +
                " made automatically. If you have questions or concerns, reply to this " +
                "message. This will summon my creator. You can find my source code on " +
                "[GitHub](https://github.com/Rafa-X9/DailyGrandPrix).)");

            sw.Close();
        }
    }
}
