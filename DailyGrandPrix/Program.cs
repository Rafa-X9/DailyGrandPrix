using DailyGrandPrix.Entities;
using DailyGrandPrix.Enums;
using DailyGrandPrix.Services;
using DailyGrandPrix.Exceptions;
using OfficeOpenXml;
using System.Runtime.Intrinsics.Arm;

namespace DailyGrandPrix
{
    class Program
    {
        public static void Main(string[] args)
        {
            ExcelPackage.License.SetNonCommercialPersonal("RafaX9");
            SaveService saveService = new();
            CreateService createSerivce = new(saveService);

            saveService.ImportAll();

            int choice = 0;
            while (choice != 100)
            {
                Console.Clear();

                Console.WriteLine("==MANAGING DRIVERS==");
                Console.WriteLine(" (1) Create driver");
                Console.WriteLine(" (2) See all drivers");
                Console.WriteLine(" (3) Edit driver");
                Console.WriteLine(" (4) Save drivers in database");
                Console.WriteLine(" (5) Delete driver");

                Console.WriteLine("==MANAGING TRACKS==");
                Console.WriteLine(" (6) Create track");
                Console.WriteLine(" (7) See all tracks");
                Console.WriteLine(" (8) Edit track");
                Console.WriteLine(" (9) Save tracks in database");
                Console.WriteLine(" (10) Delete track");

                Console.WriteLine("==MANAGING CHAMPIONSHIPS==");
                Console.WriteLine(" (11) Create championship");
                Console.WriteLine(" (12) See all championships");
                Console.WriteLine(" (13) Edit championship");
                Console.WriteLine(" (14) Save championships in database");
                Console.WriteLine(" (15) Delete championship");
                Console.WriteLine(" (16) Create race");
                Console.WriteLine(" (17) See races");
                Console.WriteLine(" (18) Process race");
                Console.WriteLine(" (19) See a championship's standings");
                Console.WriteLine(" (20) Generate usernames for pings");
                Console.WriteLine(" (21) Simulate race");
                Console.WriteLine();

                Console.WriteLine("(22) Close program");
                Console.WriteLine();
                choice = InputService.GetIntInput();

                //create driver
                if (choice == 1)
                {
                    Console.Clear();
                    createSerivce.CreateDriver();
                }

                //see drivers
                else if (choice == 2)
                {
                    Console.Clear();
                    saveService.Drivers.Sort((d1, d2) => d1.Id.CompareTo(d2.Id));
                    foreach (Driver d in saveService.Drivers)
                    {
                        Console.WriteLine(d);
                    }
                    Console.WriteLine("Press enter to continue.");
                    Console.ReadLine();
                }

                //edit driver
                else if (choice == 3)
                {
                    try
                    {
                        Driver driver = SelectionSerivce.GetDriver(saveService);
                        Console.Clear();
                        Console.WriteLine("(1) Change name: " + driver.Name);
                        Console.WriteLine("(2) Change username: " + driver.Username);
                        Console.WriteLine("(3) Change number: " + driver.Number);
                        Console.WriteLine("(4) Change team: " + driver.Team);
                        Console.Write("> ");
                        choice = int.Parse(Console.ReadLine());

                        if (choice == 1)
                        {
                            Console.Write("New name: ");
                            driver.Name = Console.ReadLine();
                        }
                        else if (choice == 2)
                        {
                            Console.Write("New username: ");
                            driver.Username = Console.ReadLine();
                        }
                        else if (choice == 3)
                        {
                            Console.Write("New number: ");
                            driver.Number = int.Parse(Console.ReadLine());
                        }
                        else if (choice == 4)
                        {
                            Console.Write("New team: ");
                            driver.Team = Enum.Parse<Teams>(Console.ReadLine());
                        }
                        else throw new InvalidOperationException();
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("Format error! " + ex.Message);
                        Console.WriteLine("Driver editing aborted.");
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                        continue;
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("Nothing found under that number!");
                        Console.WriteLine("Driver editing aborted.");
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("UNEXPECTED ERROR");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                    }

                    choice = 3;
                }

                //save drivers
                else if (choice == 4)
                {
                    saveService.SaveDrivers();
                    saveService.DeleteUntrackedDrivers();
                }

                //delete driver
                else if (choice == 5)
                {
                    try
                    {
                        Console.Clear();
                        Driver driver = SelectionSerivce.GetDriver(saveService);
                        Console.Write("Type 'delete' to confirm: ");
                        string confirm = Console.ReadLine();
                        if (confirm != "delete")
                        {
                            throw new NotConfirmedException("Deletion not confirmed.");
                        }
                        saveService.Drivers.Remove(driver);
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("Format error! " + ex.Message);
                        Console.WriteLine("Deletion aborted.");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("No driver found with that number!");
                        Console.WriteLine("Deletion aborted.");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (NotConfirmedException ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Deletion aborted.");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("UNEXPECTED ERROR");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Deletion aborted.");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                }

                //create track
                else if (choice == 6)
                {
                    Console.Clear();
                    createSerivce.CreateTrack();
                }

                //see tracks
                else if (choice == 7)
                {
                    Console.Clear();
                    foreach (Track t in saveService.Tracks)
                    {
                        Console.WriteLine(t);
                    }
                    Console.WriteLine("Press enter to continue.");
                    Console.ReadLine();
                }

                //edit track
                else if (choice == 8)
                {
                    try
                    {
                        Track track = SelectionSerivce.GetTrack(saveService);
                        Console.Clear();
                        Console.WriteLine("(1) Change name: " + track.Name);
                        Console.WriteLine("(2) Change steps per lap: " + track.StepsPerLap);
                        Console.Write("> ");
                        choice = int.Parse(Console.ReadLine());
                        if (choice == 1)
                        {
                            Console.Write("New name: ");
                            track.Name = Console.ReadLine();
                        }
                        else if (choice == 2)
                        {
                            Console.Write("New steps per lap: ");
                            track.StepsPerLap = int.Parse(Console.ReadLine());
                        }
                        else throw new InvalidOperationException();
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("Format error! " + ex.Message);
                        Console.WriteLine("Track editing aborted.");
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                        continue;
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("Nothing found under that number!");
                        Console.WriteLine("Track editing aborted.");
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("UNEXPECTED ERROR");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                    }
                }

                //save tracks
                else if (choice == 9)
                {
                    saveService.SaveTracks();
                    saveService.DeleteUntrackedTracks();
                }

                //delete track
                else if (choice == 10)
                {
                    try
                    {
                        Track track = SelectionSerivce.GetTrack(saveService);
                        Console.Write("Type 'delete' to confirm: ");
                        string confirm = Console.ReadLine();
                        if (confirm != "delete")
                        {
                            throw new NotConfirmedException("Deletion not confirmed.");
                        }
                        saveService.DeleteTrack(track);
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("Format error! " + ex.Message);
                        Console.WriteLine("Deletion aborted.");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("No track found with that number!");
                        Console.WriteLine("Deletion aborted.");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (NotConfirmedException ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Deletion aborted.");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("UNEXPECTED ERROR");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Deletion aborted.");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }

                    choice = 10;
                }

                //create championship
                else if (choice == 11)
                {
                    Console.Clear();
                    createSerivce.CreateChampionship();
                }

                //see all championships
                else if (choice == 12)
                {
                    Console.Clear();
                    foreach (Championship champ in saveService.Championships)
                    {
                        Console.WriteLine(champ);
                    }
                    Console.WriteLine("Press enter to continue.");
                    Console.ReadLine();
                }

                //edit championship
                else if (choice == 13)
                {
                    try
                    {
                        Championship champ = SelectionSerivce.GetChampionship(saveService);
                        Console.Write("New name: ");
                        champ.Name = Console.ReadLine();
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("Format error! " + ex.Message);
                        Console.WriteLine("Championship editing aborted.");
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                        continue;
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("Nothing found under that number!");
                        Console.WriteLine("Championship editing aborted.");
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("UNEXPECTED ERROR");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                    }

                    choice = 13;
                }

                //save championships
                else if (choice == 14)
                {
                    char ans = 'e';
                    try
                    {
                        Console.Write("Make excel log? (y/n) ");
                        ans = char.Parse(Console.ReadLine());
                        saveService.SaveChampionships();
                        saveService.DeleteUntrackedChampionships();
                        saveService.SaveRaces(ans.ToString().ToLower() == "y");
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid format!");
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                    }
                }

                //delete championship
                else if (choice == 15)
                {
                    try
                    {
                        Console.Clear();
                        Championship champ = SelectionSerivce.GetChampionship(saveService);
                        Console.Write("Type 'delete' to confirm: ");
                        string confirm = Console.ReadLine();
                        if (confirm != "delete")
                        {
                            throw new NotConfirmedException("Deletion not confirmed.");
                        }
                        saveService.Championships.Remove(champ);
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("Format error! " + ex.Message);
                        Console.WriteLine("Deletion aborted.");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("No track found with that number!");
                        Console.WriteLine("Deletion aborted.");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (NotConfirmedException ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Deletion aborted.");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("UNEXPECTED ERROR");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Deletion aborted.");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }

                    choice = 15;
                }

                //create race
                else if (choice == 16)
                {
                    Console.Clear();
                    createSerivce.CreateRace();
                }

                //see races
                else if (choice == 17)
                {
                    try
                    {
                        Championship champ = SelectionSerivce.GetChampionship(saveService);
                        Console.Clear();
                        foreach (Race r in champ.Races)
                        {
                            Console.WriteLine(r);
                        }
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("Format error! " + ex.Message);
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("No track found with that number!");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (NotConfirmedException ex)
                    {
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

                    choice = 17;
                }

                //process race
                else if (choice == 18)
                {
                    try
                    {
                        Race race = SelectionSerivce.GetRace(saveService);
                        RaceProcessService rps = new(saveService, race);
                        rps.ProcessRace();
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("Format error! " + ex.Message);
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("No track found with that number!");
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

                    choice = 18;
                }

                //see champ standings
                else if (choice == 19)
                {
                    try
                    {
                        Championship champ = SelectionSerivce.GetChampionship(saveService);
                        Console.Clear();
                        Dictionary<Driver, int> punctuation = new();
                        List<int> points = new() { 25, 18, 15, 12, 10, 8, 6, 4, 2, 1 };

                        foreach (Race race in champ.Races)
                        {
                            race.Drivers.Sort();
                            for (int i = 0; i < race.Drivers.Count; i++)
                            {
                                DriverRace dr = race.Drivers[i];
                                if (!punctuation.ContainsKey(dr.Driver))
                                {
                                    if (i < 10) punctuation.Add(dr.Driver, 0);
                                }
                                if (race.RaceState != RaceState.Finished) continue;
                                if (dr.HasRetired) continue;
                                if (i < 10) punctuation[dr.Driver] += points[i];
                            }
                        }

                        foreach (KeyValuePair<Driver, int> d in punctuation)
                        {
                            Console.WriteLine(d.Key.Name + ", " + d.Value + " points");
                        }

                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
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
                    catch (NotConfirmedException ex)
                    {
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

                    choice = 19;
                }

                //generate usernames for pings
                else if (choice == 20)
                {
                    try
                    {
                        Console.Clear();
                        Race race = SelectionSerivce.GetRace(saveService);

                        foreach (DriverRace d in race.Drivers)
                        {
                            Console.WriteLine(d.Driver.Username /* + " - " + d.Driver.Team */);
                            Console.WriteLine();
                        }

                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("Format error! " + ex.Message);
                        Console.WriteLine("Driver editing aborted.");
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                        continue;
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("Nothing found under that number!");
                        Console.WriteLine("Driver editing aborted.");
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("UNEXPECTED ERROR");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                    }
                }

                //simulation
                else if (choice == 21)
                {
                    try
                    {
                        Race race = SelectionSerivce.GetRace(saveService);
                        Console.Write("How many sims? ");
                        int q = int.Parse(Console.ReadLine());
                        int fastestMoves = 999;
                        double fastestneeded = 10000;
                        string fastest = "";

                        DateTime start = DateTime.Now;
                        for (int i = 0; i < q; i++)
                        {
                            race.Drivers[0] = new(race.Drivers[0].Driver, race);
                            SimulationService sim = new(race.Drivers.First());
                            while (sim.DriverRace.StepsDriven < race.Track.StepsPerLap * race.Track.RaceLaps && !(sim.DriverRace.HasRetired))
                            {
                                if (sim.DriverRace.FuelAmount < 5)
                                {
                                    sim.DriverRace.HasRetired = true;
                                    continue;
                                }
                                if (sim.PitStops.Contains(sim.DriverRace.MovesMade))
                                {
                                    sim.DriverRace.ChangeTyres(sim.Tyres[sim.DriverRace.TyreChanges]);
                                    sim.Actions.Add(Actions.Pit);
                                }
                                else
                                {
                                    Random random = new Random();
                                    int move = random.Next(1, 4);
                                    if (move == 1)
                                    {
                                        sim.DriverRace.MakeMove(Actions.Conserve);
                                        sim.Actions.Add(Actions.Conserve);
                                    }
                                    else
                                    {
                                        sim.DriverRace.MakeMove(Actions.Push);
                                        sim.Actions.Add(Actions.Push);
                                    }
                                }
                            }
                            if (!sim.DriverRace.HasRetired)
                            {
                                //Console.WriteLine("Finished the race on " + sim.DriverRace.MovesMade + " moves.");
                                if (sim.DriverRace.MovesMade < fastestMoves && sim.NeededToFinish() < fastestneeded)
                                {
                                    saveService.SaveRaces(true);
                                    fastestMoves = sim.DriverRace.MovesMade;
                                    fastestneeded = sim.NeededToFinish();
                                    fastest = "";
                                    fastest += $"Started on {sim.Starting}\n";
                                    fastest += $"Started on {sim.StartingFuel} fuel\n";
                                    int qchan = 0;
                                    foreach (var change in sim.PitStops)
                                    {
                                        fastest += $"Changed on move {change} to {sim.Tyres[qchan]}\n";
                                        qchan++;
                                    }
                                    foreach (var move in sim.Actions)
                                    {
                                        fastest += $"{move} ";
                                    }
                                    fastest += "\n";
                                }
                            }
                            else
                            {
                                //Console.WriteLine("Retired.");
                            }
                            //Console.WriteLine("Press enter to continue");
                            //Console.ReadLine();
                            Console.Clear();
                            Console.WriteLine(i + "  /  " + q);
                        }
                        DateTime end = DateTime.Now;
                        TimeSpan ts = end.Subtract(start);

                        Console.Clear();
                        Console.WriteLine("DONE!");
                        Console.WriteLine($"This took {ts.Minutes} minutes and {ts.Seconds} seconds");
                        Console.WriteLine(fastest);
                        Console.ReadLine();
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("Format error! " + ex.Message);
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("No track found with that number!");
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

                //close program
                else if (choice == 22)
                {
                    Environment.Exit(0);
                }

                //invalid choice
                else
                {
                    Console.WriteLine("Invalid choice.");
                    Console.WriteLine("Press enter to continue.");
                    Console.ReadLine();
                    continue;
                }
            }
        }
    }
}