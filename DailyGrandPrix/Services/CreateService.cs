using DailyGrandPrix.Entities;
using DailyGrandPrix.Enums;

namespace DailyGrandPrix.Services
{
    internal sealed class CreateService
    {
        public SaveService Saves { get; set; }

        public CreateService(SaveService saves)
        {
            Saves = saves;
        }

        public void CreateDriver()
        {
            try
            {
                string name = InputService.GetStringInput(message: "Driver's name:");
                foreach (char letter in name)
                {
                    if (letter == '\\' || letter == '/')
                    {
                        throw new ArgumentException("Slashes can't be part of the name.");
                    }
                }

                string username = InputService.GetStringInput(message: "Driver's usernname:");             
                int number = InputService.GetIntInput("Driver's number:");
                Teams team = InputService.GetEnumInput<Teams>(message: "Driver's team:");
                int id = Saves.Drivers.Count() + 1;
                Saves.Drivers.Add(new Driver(id, name, username, number, team));
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Format error! " + ex.Message);
                Console.WriteLine("Press enter to continue.");
                Console.ReadLine();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Argument error! " + ex.Message);
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

        public void CreateTrack()
        {
            try
            {
                int id = Saves.Tracks.Count() + 1;
                string name = InputService.GetStringInput(message: "Name:");
                foreach (char letter in name)
                {
                    if (letter == '\\' || letter == '/')
                    {
                        throw new ArgumentException("Slashes can't be part of the name.");
                    }
                }
                int stepsPerLap = InputService.GetIntInput(message: "Steps per lap:");
                Saves.Tracks.Add(new(id, name, stepsPerLap));
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Format error! " + ex.Message);
                Console.WriteLine("Press enter to continue.");
                Console.ReadLine();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Argument error! " + ex.Message);
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
    
        public void CreateChampionship()
        {
            try
            {
                int id = Saves.Championships.Count + 1;
                int year = DateTime.Today.Year;
                string name = InputService.GetStringInput(message: "Championship's name:");
                foreach (char letter in name)
                {
                    if (letter == '\\' || letter == '/')
                    {
                        throw new ArgumentException("Slashes can't be part of the name.");
                    }
                }
                Saves.Championships.Add(new(id, year, name));
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Format error! " + ex.Message);
                Console.WriteLine("Press enter to continue.");
                Console.ReadLine();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Argument error! " + ex.Message);
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
    
        public void CreateRace()
        {
            try
            {
                Console.WriteLine("Choose a championship to add the race to:");
                foreach (Championship c in Saves.Championships)
                {
                    Console.WriteLine(c.Id + " - " + c.Name);
                }
                int num = InputService.GetIntInput();
                Championship champ = Saves.Championships.Where(c => c.Id == num).First();
                string path = Saves.ChampionshipPath + $@"\{champ.Name}";
                string[] races = Directory.GetFiles(path);
                int id = (int)Math.Ceiling((double) (races.Length) / 3);

                Console.WriteLine("Choose the track:");
                foreach (Track t in Saves.Tracks)
                {
                    Console.WriteLine(t.Id + " - " + t.Name + " - " + t.StepsPerLap + " steps a lap");
                }
                num = InputService.GetIntInput();
                Track track = Saves.Tracks.Where(t => t.Id == num).First();

                List<int> rain = [];
                if (InputService.GetStringInput(message: "Rain? (y/n)").Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    for (int i = 1; i <= 20; i++)
                    {
                        rain.Add(InputService.GetIntInput(message: $"Rain in day {i}"));
                    }
                }                

                champ.Races.Add(new(id, champ, track) { RainHistory = rain });
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Format error! " + ex.Message);
                Console.WriteLine("Press enter to continue.");
                Console.ReadLine();
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Error! There is nothing that matches the number input.");
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
    }
}
