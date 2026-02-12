using DailyGrandPrix.Entities;
using DailyGrandPrix.Enums;
using DailyGrandPrix.Services;
using DailyGrandPrix.Exceptions;
using OfficeOpenXml;

namespace DailyGrandPrix
{
    class Program
    {
        public static void Main(string[] args)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Rafael Floriano");
            SaveService saveService = new();
            CreateService createSerivce = new(saveService);

            saveService.ImportAll();

            int choice = 0;
            while (choice != 100)
            {
                Console.Clear();

                Console.WriteLine("==MANAGING DRIVERS==");
                Console.WriteLine("(1) Create driver");
                Console.WriteLine("(2) See all drivers");
                Console.WriteLine("(3) Edit driver");
                Console.WriteLine("(4) Save drivers in database");
                Console.WriteLine("(5) Delete driver");
                Console.WriteLine();

                Console.WriteLine("==MANAGING TRACKS==");
                Console.WriteLine("(6) Create track");
                Console.WriteLine("(7) See all tracks");
                Console.WriteLine("(8) Edit track");
                Console.WriteLine("(9) Save tracks in database");
                Console.WriteLine("(10) Delete track");
                Console.WriteLine();

                Console.WriteLine("==MANAGING CHAMPIONSHIPS==");
                Console.WriteLine("(11) Create championship");
                Console.WriteLine("(12) See all championships");
                Console.WriteLine("(13) Edit championship");
                Console.WriteLine("(14) Save championships in database");
                Console.WriteLine("(15) Delete championship");
                Console.WriteLine("(16) Create race");
                Console.WriteLine("(17) See races");
                Console.WriteLine("(18) Process race");
                Console.WriteLine("(19) See a championship's standings");
                Console.WriteLine("(20) Generate usernames for pings");
                Console.WriteLine();

                Console.WriteLine("(100) Close program");
                Console.WriteLine();
                try
                {
                    Console.Write("> ");
                    choice = int.Parse(Console.ReadLine());
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Format error! " + ex.Message);
                    Console.WriteLine("Press enter to continue.");
                    Console.ReadLine();
                    continue;
                }

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
                    Console.Clear();
                    Console.WriteLine("Type the Id of the driver to edit:");
                    foreach (Driver d in saveService.Drivers)
                    {
                        Console.WriteLine($"{d.Id} - {d.Name} - {d.Username}");
                    }
                    Console.Write("> ");
                    try
                    {
                        choice = int.Parse(Console.ReadLine());
                        Driver driver = saveService.Drivers.Where(dr => dr.Id == choice).First();
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
                    Console.Clear();
                    Console.WriteLine("Type the Id of the driver to delete:");
                    foreach (Driver d in saveService.Drivers)
                    {
                        Console.WriteLine($"{d.Id} - {d.Name} - {d.Username}");
                    }
                    Console.Write("> ");
                    try
                    {
                        choice = int.Parse(Console.ReadLine());
                        Driver driver = saveService.Drivers.Where(dr => dr.Id == choice).First();
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

                    choice = 5;
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
                    Console.Clear();
                    Console.WriteLine("Choose a track:");
                    foreach (Track t in saveService.Tracks)
                    {
                        Console.WriteLine($"{t.Id} - {t.Name} - {t.StepsPerLap} steps a lap.");
                    }
                    try
                    {
                        Console.Write("> ");
                        choice = int.Parse(Console.ReadLine());
                        Track track = saveService.Tracks.Where(t => t.Id == choice).First();
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

                    choice = 8;
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
                    Console.Clear();
                    Console.WriteLine("Type the Id of the track to delete:");
                    foreach (Track t in saveService.Tracks)
                    {
                        Console.WriteLine($"{t.Id} - {t.Name} - {t.StepsPerLap} steps a lap");
                    }
                    Console.Write("> ");
                    try
                    {
                        choice = int.Parse(Console.ReadLine());
                        Track track = saveService.Tracks.Where(tr => tr.Id == choice).First();
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
                    Console.Clear();
                    Console.WriteLine("Type the Id of the championship to edit:");
                    foreach (Championship champ in saveService.Championships)
                    {
                        Console.WriteLine($"{champ.Id} - {champ.Name} - {champ.Races.Count} races");
                    }
                    Console.Write("> ");
                    try
                    {
                        choice = int.Parse(Console.ReadLine());
                        Championship champ = saveService.Championships.Where(ch => ch.Id == choice).First();
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
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid format!");
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                    }
                    saveService.SaveChampionships();
                    saveService.DeleteUntrackedChampionships();
                    saveService.SaveRaces(ans.ToString().ToLower() == "y");
                }

                //delete championship
                else if (choice == 15)
                {
                    Console.Clear();
                    Console.WriteLine("Type the Id of the championship to delete:");
                    foreach (Championship c in saveService.Championships)
                    {
                        Console.WriteLine($"{c.Id} - {c.Name} - {c.Races.Count} races");
                    }
                    Console.Write("> ");
                    try
                    {
                        choice = int.Parse(Console.ReadLine());
                        Championship champ = saveService.Championships.Where(ch => ch.Id == choice).First();
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
                    Console.Clear();
                    Console.WriteLine("Type the Id of the championship:");
                    foreach (Championship c in saveService.Championships)
                    {
                        Console.WriteLine($"{c.Id} - {c.Name} - {c.Races.Count} races");
                    }
                    Console.Write("> ");
                    try
                    {
                        choice = int.Parse(Console.ReadLine());
                        Championship champ = saveService.Championships.Where(ch => ch.Id == choice).First();
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
                    Console.Clear();
                    Console.WriteLine("Type the Id of the championship:");
                    foreach (Championship c in saveService.Championships)
                    {
                        Console.WriteLine($"{c.Id} - {c.Name} - {c.Races.Count} races");
                    }
                    Console.Write("> ");
                    try
                    {
                        choice = int.Parse(Console.ReadLine());
                        Championship champ = saveService.Championships.Where(ch => ch.Id == choice).First();
                        Console.Clear();
                        Console.WriteLine("Type the Id of the race to process:");
                        foreach (Race r in champ.Races)
                        {
                            Console.WriteLine(r.Id + " - " + r.RaceState + " - " + r.Track.Name);
                        }
                        choice = int.Parse(Console.ReadLine());
                        Race race = champ.Races.Where(ra => ra.Id == choice).First();
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
                    Console.Clear();
                    Console.WriteLine("Type the Id of the championship:");
                    foreach (Championship c in saveService.Championships)
                    {
                        Console.WriteLine($"{c.Id} - {c.Name} - {c.Races.Count} races");
                    }
                    Console.Write("> ");
                    try
                    {
                        choice = int.Parse(Console.ReadLine());
                        Championship champ = saveService.Championships.Where(ch => ch.Id == choice).First();
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

                    choice = 19;
                }

                //generate usernames for pings
                else if (choice == 20)
                {
                    Console.Clear();
                    Console.WriteLine("Type the Id of the championship:");
                    foreach (Championship c in saveService.Championships)
                    {
                        Console.WriteLine($"{c.Id} - {c.Name} - {c.Races.Count} races");
                    }
                    Console.Write("> ");
                    try
                    {
                        choice = int.Parse(Console.ReadLine());
                        Championship champ = saveService.Championships.Where(ch => ch.Id == choice).First();
                        Console.Clear();
                        Console.WriteLine("Type the Id of the race to process:");
                        foreach (Race r in champ.Races)
                        {
                            Console.WriteLine(r.Id + " - " + r.RaceState + " - " + r.Track.Name);
                        }
                        choice = int.Parse(Console.ReadLine());
                        Race race = champ.Races.Where(ra => ra.Id == choice).First();

                        foreach (DriverRace d in race.Drivers)
                        {
                            Console.WriteLine(d.Driver.Username + '\n');
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
                    catch (Exception ex)
                    {
                        Console.WriteLine("UNEXPECTED ERROR");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                    }

                    choice = 20;
                }

                //close program
                else if (choice == 100)
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