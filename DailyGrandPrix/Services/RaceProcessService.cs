using DailyGrandPrix.Entities;
using DailyGrandPrix.Exceptions;
using DailyGrandPrix.Enums;

namespace DailyGrandPrix.Services
{
    internal sealed class RaceProcessService
    {
        public SaveService SaveService { get; set; }
        public Race Race { get; set; }

        public RaceProcessService(SaveService saveService, Race race)
        {
            SaveService = saveService;
            Race = race;
        }

        public void ProcessRace()
        {
            Console.Clear();
            if (Race.RaceState == RaceState.AddingDrivers)
            {
                int choice = -1;
                while (choice != 3 && Race.RaceState == RaceState.AddingDrivers)
                {
                    Console.Clear();
                    Console.WriteLine("This race is in the AddingDrivers stage.");
                    Console.WriteLine("(1) Add drivers");
                    Console.WriteLine("(2) Start the race");
                    Console.WriteLine("(3) Quit");
                    try
                    {
                        choice = int.Parse(Console.ReadLine());
                        switch (choice)
                        {
                            case 1:
                                AddDrivers();
                                break;
                            case 2:
                                StartRace();
                                break;
                            case 3:
                                continue;
                            default:
                                throw new ArgumentException();
                        }
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("Format error! " + ex.Message);
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("Invalid choice!");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("UNEXPECTED ERROR");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                }
            }

            else if (Race.RaceState == RaceState.Started)
            {
                int choice = -1;
                while (choice != Race.Drivers.Count + 1)
                {
                    Console.Clear();
                    Console.WriteLine("This race has started.");
                    Console.WriteLine($"All drivers have made {Race.MovesInto} or {Race.MovesInto + 1} moves.");
                    Console.WriteLine("Choose a driver to make a move:");
                    for (int i = 0; i < Race.Drivers.Count; i++)
                    {
                        if (!Race.Drivers[i].HasRetired)
                        {
                            Console.WriteLine($"P{i + 1} - {Race.Drivers[i].Driver.Name}");
                        }
                        else
                        {
                            Console.WriteLine($"DNF - {Race.Drivers[i].Driver.Name}");
                        }
                    }
                    Console.WriteLine((Race.Drivers.Count + 1) + " - Quit");
                    try
                    {
                        Console.Write("> ");
                        choice = int.Parse(Console.ReadLine());
                        if (choice == Race.Drivers.Count + 1) continue;
                        if (Race.Drivers[choice - 1].MovesMade > Race.MovesInto)
                        {
                            throw new AlreadyMadeMoveException("This driver already made their move!");
                        }
                        if (Race.Drivers[choice - 1].StepsDriven >= Race.Track.StepsPerLap * Race.Track.RaceLaps)
                        {
                            throw new DriverAlreadyFinishedException("This driver already finished the race!");
                        }
                        DriverRace dr = Race.Drivers[choice - 1];
                        Race.Drivers.ForEach(dr => dr.Race = Race);
                        Console.WriteLine("Choose move for " + dr.Driver.Name + ":");
                        Console.WriteLine("(1) Conserve");
                        Console.WriteLine("(2) Push");
                        Console.WriteLine("(3) Pitstop for softs");
                        Console.WriteLine("(4) Pitstop for mediums");
                        Console.WriteLine("(5) Pitstop for hards");
                        Console.Write("> ");
                        choice = int.Parse(Console.ReadLine());
                        switch (choice)
                        {
                            case 1:
                                dr.MakeMove(Actions.Conserve);
                                break;
                            case 2:
                                dr.MakeMove(Actions.Push);
                                break;
                            case 3:
                                dr.ChangeTyres(Tyres.Softs);
                                break;
                            case 4:
                                dr.ChangeTyres(Tyres.Mediums);
                                break;
                            case 5:
                                dr.ChangeTyres(Tyres.Hards);
                                break;
                            default:
                                throw new ArgumentException();
                        }

                        for (int i = 0; i < Race.Drivers.Count; i++)
                        {
                            Race.Drivers[i].CheckFinish(Race, i);
                        }

                        Race.Drivers.ForEach(dr => dr.Race = Race);
                        Race.Drivers.Sort();

                        bool everyoneMoved = true;
                        foreach (DriverRace d in Race.Drivers)
                        {
                            if (d.MovesMade == Race.MovesInto
                                && d.HasRetired == false
                                && d.FinalPosition == null)
                            {
                                everyoneMoved = false;
                            }
                        }
                        if (everyoneMoved) Race.MovesInto++;
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("Format error! " + ex.Message);
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("Nothing found with that number!");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("Nothing found with that number!");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (AlreadyMadeMoveException ex)
                    {
                        Console.WriteLine("Error!");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (DriverHasRetiredException ex)
                    {
                        Console.WriteLine("Error!");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (DriverAlreadyFinishedException ex)
                    {
                        Console.WriteLine("Error!");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("UNEXPECTED ERROR");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                }
            }
        }

        private void AddDrivers()
        {
            int choice = -1;
            while (choice != SaveService.Drivers.Count + 1)
            {
                Console.Clear();
                SaveService.Drivers.Sort((d1, d2) => d1.Id.CompareTo(d2.Id));
                Console.WriteLine("Choose driver to add:");
                foreach (Driver d in SaveService.Drivers)
                {
                    Console.WriteLine(d.Id + " - " + d.Name + " - " + d.Username);
                }
                Console.WriteLine($"{SaveService.Drivers.Count + 1} - quit");
                Console.Write("> ");
                try
                {
                    choice = int.Parse(Console.ReadLine());
                    if (choice == SaveService.Drivers.Count + 1) continue;
                    Driver d = SaveService.Drivers.Where(dr => dr.Id == choice).First();
                    Race.AddDriver(d);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Format error! " + ex.Message);
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Nothing found with that number!");
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();
                }
                catch (DriverAlreadyInException ex)
                {
                    Console.WriteLine("Error!");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("UNEXPECTED ERROR");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();
                }
            }
        }

        private void StartRace()
        {
            int choice = -1;
            while (choice != Race.Drivers.Count + 1 && choice != Race.Drivers.Count + 2)
            {
                Console.Clear();
                foreach (DriverRace dr in Race.Drivers)
                {
                    Console.WriteLine($"{dr.Driver.Id} - {dr.Driver.Name}");
                    if (dr.TyreWear != 100) Console.WriteLine("Has not set up for the race yet.");
                    else
                    {
                        Console.WriteLine("Has set up for the race.");
                        Console.WriteLine("Tyres: " + dr.TyreCompound);
                        Console.WriteLine("Fuel amount: " + dr.FuelAmount);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine((Race.Drivers.Count + 1) + " - Start race");
                Console.WriteLine();
                Console.WriteLine((Race.Drivers.Count + 2) + " - Quit");
                try
                {
                    Console.Write("> ");
                    choice = int.Parse(Console.ReadLine());
                    if (choice == Race.Drivers.Count + 1 || choice == Race.Drivers.Count + 2) continue;
                    DriverRace dr = Race.Drivers.Where(d => d.Driver.Id == choice).First();
                    Console.Write($"Which tyre will {dr.Driver.Name} use? ");
                    dr.TyreCompound = Enum.Parse<Tyres>(Console.ReadLine());
                    Console.Write($"How much fuel will {dr.Driver.Name} use? ");
                    dr.FuelAmount = int.Parse(Console.ReadLine());
                    dr.TyreWear = 100;
                    dr.TyreChanges = 0;
                    dr.MovesMade = 0;
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Format error! " + ex.Message);
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Invalid choice!");
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine("Argument error!");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("UNEXPECTED ERROR");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();
                }
            }

            if (choice == Race.Drivers.Count + 1)
            {
                foreach (DriverRace dr in Race.Drivers)
                {
                    if (dr.TyreWear != 100)
                    {
                        Console.WriteLine("Driver " + dr.Driver.Name + " is not set up yet!");
                        Console.WriteLine("Race starting aborted.");
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                        return;
                    }
                }

                Race.RaceState = RaceState.Started;
            }
        }
    }
}
