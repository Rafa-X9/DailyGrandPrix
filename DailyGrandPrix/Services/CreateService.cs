using DailyGrandPrix.Entities;
using DailyGrandPrix.Enums;

namespace DailyGrandPrix.Services
{
    internal class CreateService
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
                Console.Write("Driver's name: ");
                string name = Console.ReadLine();
                foreach (char letter in name)
                {
                    if (letter == '\\' || letter == '/')
                    {
                        throw new ArgumentException("Slashes can't be part of the name.");
                    }
                }
                Console.Write("Driver's username: ");
                string username = Console.ReadLine();
                Console.Write("Driver's number: ");
                int number = int.Parse(Console.ReadLine());
                Console.Write("Driver's team: ");
                Teams team = Enum.Parse<Teams>(Console.ReadLine());
                int id = Saves.Drivers.Count() + 1;
                Saves.Drivers.Add(new Driver(id, name, username, number, team));
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Format error! " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Argument error! " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("UNEXPECTED ERROR");
                Console.WriteLine(ex.Message);
            }
        }

        public void CreateTrack()
        {
            try
            {
                int id = Saves.Tracks.Count() + 1;
                Console.Write("Name: ");
                string name = Console.ReadLine();
                foreach (char letter in name)
                {
                    if (letter == '\\' || letter == '/')
                    {
                        throw new ArgumentException("Slashes can't be part of the name.");
                    }
                }
                Console.Write("Steps per lap: ");
                int stepsPerLap = int.Parse(Console.ReadLine());
                Saves.Tracks.Add(new(id, name, stepsPerLap));
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Format error! " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Argument error! " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("UNEXPECTED ERROR");
                Console.WriteLine(ex.Message);
            }
        }
    
        public void CreateChampionship()
        {
            try
            {
                int id = Saves.Championships.Count + 1;
                int year = DateTime.Today.Year;
                Console.Write("Championship name: ");
                string name = Console.ReadLine();
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
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Argument error! " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("UNEXPECTED ERROR");
                Console.WriteLine(ex.Message);
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
                Console.Write("> ");
                int num = int.Parse(Console.ReadLine());
                Championship champ = Saves.Championships.Where(c => c.Id == num).First();
                string path = SaveService.ChampionshipPath + $@"\{champ.Name}";
                string[] races = Directory.GetFiles(path);
                int id = races.Length;

                Console.WriteLine("Choose the track:");
                foreach (Track t in Saves.Tracks)
                {
                    Console.WriteLine(t.Id + " - " + t.Name + " - " + t.StepsPerLap + " steps a lap");
                }
                Console.Write("> ");
                num = int.Parse(Console.ReadLine());
                Track track = Saves.Tracks.Where(t => t.Id == num).First();

                champ.Races.Add(new(id, champ, track));
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Format error! " + ex.Message);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Error! There is nothing that matches the number input.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("UNEXPECTED ERROR");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
